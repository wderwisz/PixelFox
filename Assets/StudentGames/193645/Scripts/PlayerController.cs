using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace _193645
{
    public class PlayerController : MonoBehaviour
    {

        [Header("Movement parameters: ")]
        [Range(0.01f, 500.0f)][SerializeField] private float baseMoveSpeed = 0.1f; // moving speed of the player
        [Range(0.01f, 20.0f)][SerializeField] private float jumpForce = 6f;
        [Range(0.01f, 500.0f)][SerializeField] private float dashForce = 6f;
        [Range(0.01f, 20.0f)][SerializeField] private float bounceOfEnemiesForce = 15f;
        [SerializeField] private float dashTime = 0.1f;
        [SerializeField] private float dashingCooldown = 1f;
        private bool canDash = true;
        private bool isDashing = false;
        private bool canExitLevel = false;
        private bool isMoving = false;
        private bool isCollidingWithEnemy = false;
        private bool jump;

        private float coyoteTime = 0.2f;
        private float coyoteTimeCounter;
        private float jumpTimer = 1;

        public const float rayLength = 1.5f;

        public LayerMask groundLayer;
        public ParticleSystem dust;
        public TrailRenderer trail;
        public GameObject finishDoor;
        public CinemachineVirtualCamera virtualCamera;
        public GameObject cameraBottomTarget;

        private Rigidbody2D rb;
        private Animator animator;
        private SpriteRenderer spriteRenderer;
        private BoxCollider2D feetCollider;
        private BoxCollider2D fullBodyCollider;
        private GameManager gm;
        private AudioController audioController;

        private bool dblJumped = false;
        private float moveSpeed;
        private bool isFacingLeft = false;
        private Vector2 colliderSize;
        private Vector2 spawnPoint;
        private Vector2 move;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            feetCollider = GetComponentInChildren<BoxCollider2D>();
            fullBodyCollider = GetComponent<BoxCollider2D>();
            audioController = GetComponent<AudioController>();
            gm = GameManager.instance;
            finishDoor.GetComponent<SpriteRenderer>().enabled = false;
            spawnPoint = transform.position;
            colliderSize = fullBodyCollider.size;
        }

        private bool IsGrounded()
        {
            if (Physics2D.Raycast(this.transform.position - new Vector3(0.6f, 0, 0), Vector2.down, rayLength, groundLayer.value) || Physics2D.Raycast(this.transform.position + new Vector3(0.4f, 0, 0), Vector2.down, rayLength, groundLayer.value))
            {
                return true;
            }
            else return false;
        }

        private bool canJump()
        {
            if (coyoteTimeCounter > 0f && !isDashing) return true;
            else return false;
        }
        private bool isFalling()
        {
            if (!IsGrounded() && rb.velocity.y < 0f) return true;
            else return false;
        }
        private void CreateDust()
        {
            dust.Emit(1);
            if (!IsGrounded()) dust.Emit(8);
        }

        private IEnumerator Dash()
        {
            if (isCollidingWithEnemy) yield return null;
            audioController.playDashSound();
            canDash = false;
            isDashing = true;
            float gravityValue = rb.gravityScale;
            float velocityValue = rb.velocity.x;
            rb.gravityScale = 0;
            trail.emitting = true;
            Vector2 v;
            if (isFacingLeft) v = new Vector2(-1f * dashForce, 0.0f);
            else v = new Vector2(1f * dashForce, 0f);
            rb.velocity = Vector2.zero;
            rb.velocity = v;
            yield return new WaitForSeconds(dashTime);
            rb.gravityScale = gravityValue;
            rb.velocity = new Vector2(velocityValue, 0.0f);
            trail.emitting = false;
            isDashing = false;
            yield return new WaitForSeconds(dashingCooldown);
            canDash = true;
        }

        private void enemyKilled()
        {
            rb.velocity = Vector2.zero;
            rb.AddForce(Vector2.up * bounceOfEnemiesForce, ForceMode2D.Impulse);
            gm.AddEnemyPoints(1);
            dblJumped = false;
            audioController.playEnemySound();
        }

        private void Death()
        {
            gm.LoseLife();
            audioController.playFallSound();
            this.transform.position = spawnPoint;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Bonus"))
            {
                gm.AddPoints(1);
                Debug.Log("Picked a cherry!");
                other.gameObject.SetActive(false);
                audioController.playBonusSound();
            }

            if (other.CompareTag("Finish"))
            {
                finishDoor.GetComponent<SpriteRenderer>().enabled = true;
                canExitLevel = true;
            }

            if (other.CompareTag("UI_Remover"))
            {
                finishDoor.GetComponent<SpriteRenderer>().enabled = false;
                canExitLevel = false;
            }

            if (other.CompareTag("Enemy"))
            {
                isCollidingWithEnemy = true;
                trail.emitting = false;
                if (feetCollider.gameObject.transform.position.y > other.gameObject.transform.position.y)
                {
                    enemyKilled();
                }
                else
                {
                    Death();
                }
            }
            else isCollidingWithEnemy = false;

            if (other.CompareTag("Key"))
            {
                Color color = other.GetComponent<SpriteRenderer>().color;
                gm.AddKey(color);
                audioController.playGemSound();
                other.gameObject.SetActive(false);
            }

            if (other.CompareTag("Heart"))
            {
                if (gm.GetLives() < gm.GetMaxLives())
                {
                    other.gameObject.SetActive(false);
                    gm.AddLife();
                    audioController.playHeartSound();
                }
            }

            if (other.CompareTag("FallLevel"))
            {
                Death();
            }

            if (other.CompareTag("Checkpoint"))
            {
                CheckpointController checkpoint = other.GetComponentInChildren<CheckpointController>();
                spawnPoint = other.gameObject.transform.position;
                if (!checkpoint.isUsed())
                {
                    checkpoint.useCheckpoint();
                    audioController.playCheckpointSound();
                }
            }

            if (other.CompareTag("MovingPlatform"))
            {
                transform.SetParent(other.transform);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("MovingPlatform"))
            {
                transform.SetParent(null);
            }
        }

        private void FinishLevel()
        {
            gm.LevelCompleted();
        }

        private void FixedUpdate()
        {
            if (jump && (canJump() || !dblJumped) && !isDashing)
            {
                jump = false;
                rb.velocity = Vector2.zero;
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                CreateDust();
                audioController.playJumpSound();
                if (!IsGrounded() && coyoteTimeCounter <= 0) dblJumped = true;
            }
            else if (jump)
            {
                jumpTimer--;
                if (jumpTimer == 0)
                {
                    jumpTimer = 15;
                    jump = false;
                }
            }
        }
        void Update()
        {
            if (GameManager.instance.currentGameState != GameManager.GameState.GS_GAME) Time.timeScale = 0f;
            else Time.timeScale = 1f;

            if (IsGrounded())
            {
                dblJumped = false;
                coyoteTimeCounter = coyoteTime;
            }
            else coyoteTimeCounter -= Time.deltaTime;

            animator.SetBool("isGrounded", IsGrounded());

            animator.SetBool("isFalling", isFalling());

            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                jump = true;
            }
            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                coyoteTimeCounter = 0f;
            }
            if ((Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) && IsGrounded())
            {
                animator.SetBool("isCrouching", true);
                moveSpeed = 0;
                virtualCamera.GetComponent<CinemachineVirtualCamera>().Follow = cameraBottomTarget.transform;
            }
            else
            {
                animator.SetBool("isCrouching", false);
                moveSpeed = baseMoveSpeed;
                virtualCamera.GetComponent<CinemachineVirtualCamera>().Follow = this.transform;
            }
            if (Input.GetKeyDown(KeyCode.Space) && canDash)
            {
                StartCoroutine(Dash());
            }
            move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) && !isDashing)
            {
                transform.Translate(moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
                animator.SetBool("isMoving", true);
                spriteRenderer.flipX = false;
                isFacingLeft = false;
                isMoving = true;

            }
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) && !isDashing)
            {
                transform.Translate(-moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
                animator.SetBool("isMoving", true);
                spriteRenderer.flipX = true;
                isFacingLeft = true;
                isMoving = true;
            }
            else
            {
                animator.SetBool("isMoving", false);
                isMoving = false;
            }

            if (isMoving)
            {
                if (IsGrounded()) CreateDust();
            }

            animator.SetBool("isDashing", isDashing);

            if (isDashing)
            {
                fullBodyCollider.size = new Vector2(25f, 35f);
            }
            else fullBodyCollider.size = colliderSize;

            if (canExitLevel && Input.GetKeyDown(KeyCode.E) && gm.allKeysCollected())
            {
                FinishLevel();
            }
        }
    }
}