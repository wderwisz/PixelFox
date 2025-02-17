using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _193645
{
    public class EnemyController : MonoBehaviour
    {
        [Range(0.01f, 20.0f)][SerializeField] private float baseMoveSpeed = 0.1f;
        [Range(0.01f, 20.0f)][SerializeField] private float moveRange = 1f;
        private bool isFacingLeft = true;
        private bool isMovingRight = false;
        private bool isAlive = true;
        private Vector3 theScale;

        private float startPositionX;
        private Animator animator;
        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            startPositionX = this.transform.position.x;
        }

        IEnumerator KillOnAnimationEnd()
        {
            yield return new WaitForSeconds(0.3f);
            gameObject.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                float feetColliderPositionY = other.gameObject.GetComponentInChildren<BoxCollider2D>().transform.position.y;
                if (feetColliderPositionY > this.transform.position.y)
                {
                    animator.SetBool("isDead", true);
                    isAlive = false;
                    StartCoroutine(KillOnAnimationEnd());
                }
            }
        }

        private void Flip()
        {
            isFacingLeft = !isFacingLeft;
            spriteRenderer.flipX = !isFacingLeft;
        }

        private void moveRight()
        {
            if (isAlive) transform.Translate(baseMoveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
        }

        private void moveLeft()
        {
            if (isAlive) transform.Translate(-baseMoveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
        }

        void Update()
        {
            if (isMovingRight)
            {
                if (this.transform.position.x < startPositionX + moveRange)
                {
                    moveRight();
                }
                else
                {
                    isMovingRight = false;
                    Flip();
                }
            }
            else
            {
                if (this.transform.position.x > startPositionX - moveRange)
                {
                    moveLeft();
                }
                else
                {
                    isMovingRight = true;
                    Flip();
                }
            }
        }
    }
}