using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

namespace _193645
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;
        public enum GameState
        {
            GS_PAUSEMENU,
            GS_GAME,
            GS_LEVELCOMPLETED,
            GS_GAME_OVER,
            GS_OPTIONS
        }
        public GameState currentGameState = GameState.GS_GAME;

        public Canvas inGameCanvas;
        public Canvas pauseMenuCanvas;
        public Canvas levelCompletedCanvas;
        public Canvas optionsCanvas;
        public Canvas gameOverCanvas;
        public TMP_Text scoreText;
        public TMP_Text timerText;
        public TMP_Text enemiesText;
        public TMP_Text levelCompletedScore;
        public TMP_Text levelCompletedHighScore;
        public TMP_Text qualityLabel;
        public Image[] keysTab;
        public Image[] livesTab;

        private int score = 0;
        private int enemiesScore = 0;
        private int keysFound = 0;
        private int maxKeys = 3;
        private float timer = 0;
        private int maxLives = 3;
        private int lives;

        private const string keyHighScore = "HighScore193645";
        private const string masterVolume = "MasterVolume";

        private const string levelName = "193645.Level1";

        private void SetGameState(GameState newGameState)
        {
            currentGameState = newGameState;

            if (currentGameState == GameState.GS_GAME)
            {
                inGameCanvas.enabled = true;
            }
            else inGameCanvas.enabled = false;

            if (currentGameState == GameState.GS_LEVELCOMPLETED)
            {
                Scene currentScene = SceneManager.GetActiveScene();
                if (currentScene.name == levelName)
                {
                    int highScore = PlayerPrefs.GetInt(keyHighScore);
                    if (highScore < score)
                    {
                        highScore = score;
                        PlayerPrefs.SetInt(keyHighScore, highScore);
                    }
                    levelCompletedScore.text = "Your score: " + score;
                    levelCompletedHighScore.text = "Best score: " + highScore;
                }
            }

            pauseMenuCanvas.enabled = currentGameState == GameState.GS_PAUSEMENU;
            levelCompletedCanvas.enabled = currentGameState == GameState.GS_LEVELCOMPLETED;
            optionsCanvas.enabled = currentGameState == GameState.GS_OPTIONS;
            gameOverCanvas.enabled = currentGameState == GameState.GS_GAME_OVER;
        }

        public void AddPoints(int points)
        {
            score += points;
            scoreText.text = score.ToString();
        }
        public void AddEnemyPoints(int points)
        {
            enemiesScore += points;
            enemiesText.text = enemiesScore.ToString();
        }
        public void AddKey(Color color)
        {
            keysTab[keysFound].color = color;
            keysFound++;
        }
        public void LoseLife()
        {
            livesTab[lives - 1].enabled = false;
            lives--;
            if (lives <= 0) SetGameState(GameState.GS_GAME_OVER);
        }
        public void AddLife()
        {
            livesTab[lives].enabled = true;
            lives++;
        }
        public bool allKeysCollected()
        {
            return keysFound == maxKeys;
        }
        public void QualityUp()
        {
            if (currentGameState != GameState.GS_OPTIONS) return;
            QualitySettings.IncreaseLevel();
            Options();
        }
        public void QualityDown()
        {
            if (currentGameState != GameState.GS_OPTIONS) return;
            QualitySettings.DecreaseLevel();
            Options();
        }
        public void PauseMenu()
        {
            SetGameState(GameState.GS_PAUSEMENU);
        }
        public void InGame()
        {
            SetGameState(GameState.GS_GAME);
        }
        public void LevelCompleted()
        {
            score += lives * 100;
            SetGameState(GameState.GS_LEVELCOMPLETED);
        }
        public void GameOver()
        {
            SetGameState(GameState.GS_GAME_OVER);
        }
        public void Options()
        {
            if (currentGameState != GameState.GS_OPTIONS) SetGameState(GameState.GS_OPTIONS);
            qualityLabel.text = "Quality: " + QualitySettings.names[QualitySettings.GetQualityLevel()];
        }
        private void UpdateTimer()
        {
            float seconds = timer % 60;
            float minutes = Mathf.Floor(timer / 60);

            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        public void OnResumeButtonClicked()
        {
            InGame();
        }
        public void OnOptionsButtonClicked()
        {
            Options();
        }
        public void OnRestartButtonClicked()
        {
            if (currentGameState == GameState.GS_GAME) return;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        public void ReturnToMainMenuButtonClicked()
        {
            if (currentGameState == GameState.GS_GAME) return;

            int sceneIndex = SceneUtility.GetBuildIndexByScenePath("Scenes/Main Menu");
            if (sceneIndex >= 0)
            {
                SceneManager.LoadSceneAsync(sceneIndex); //³adowanie sceny ³¹cz¹cej gry
            }
            else
            {
                //sceneIndex jest równe -1. Nie znaleziono sceny.
                //³adowanie innej sceny docelowo na laboratorium
                SceneManager.LoadScene("MainMenu");
            }

        }
        public void SetVolumeSlider(Slider volSlider)
        {
            float vol = PlayerPrefs.GetFloat(masterVolume);
            AudioListener.volume = vol;
            volSlider.value = AudioListener.volume;
        }

        public int GetLives()
        {
            return lives;
        }
        public int GetMaxLives()
        {
            return maxLives;
        }

        private void Awake()
        {
            if (!PlayerPrefs.HasKey(keyHighScore)) PlayerPrefs.SetInt(keyHighScore, 0);
            if (!PlayerPrefs.HasKey(masterVolume)) PlayerPrefs.SetInt(masterVolume, 0);
            instance = this;
            scoreText.text = score.ToString();
            enemiesText.text = enemiesScore.ToString();
            foreach (Image x in keysTab)
            {
                x.color = Color.gray;
            }
            lives = maxLives;
            InGame();
        }

        void Update()
        {
            //Debug.Log("Score: " + score);
            //Debug.Log("High Score: " + PlayerPrefs.GetInt(keyHighScore));
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (currentGameState == GameState.GS_GAME) PauseMenu();
                else if (currentGameState == GameState.GS_PAUSEMENU) InGame();
                else if (currentGameState == GameState.GS_OPTIONS) InGame();
            }

            timer += Time.deltaTime;
            UpdateTimer();
        }
    }
}