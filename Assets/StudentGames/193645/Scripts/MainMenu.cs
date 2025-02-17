using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _193645
{
    public class MainMenu : MonoBehaviour
    {
        public void OnLevel1ButtonPressed()
        {
            SceneManager.LoadScene("193645.Level1");
        }

        public void OnExitToDesktopButtonPressed()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }

    }
}