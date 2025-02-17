using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _193645
{
    public class MovingPlatformController : MonoBehaviour
    {
        [Range(0.01f, 200.0f)][SerializeField] private float baseMoveSpeed = 0.1f;
        [Range(0.01f, 20.0f)][SerializeField] private float moveRange = 1f;
        private bool isMovingRight = false;
        //private float move;
        Rigidbody2D rb;

        private float startPositionX;

        private void Awake()
        {
            startPositionX = this.transform.position.x;
            rb = GetComponent<Rigidbody2D>();
        }
        private void FixedUpdate()
        {
            //rb.velocity = new(baseMoveSpeed * Time.deltaTime * move, 0.0f);
        }
        private void moveRight()
        {
            //move = 1.0f;
            transform.Translate(baseMoveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
        }

        private void moveLeft()
        {
            //move = -1.0f;
            transform.Translate(-baseMoveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
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
                }
            }
        }
    }
}