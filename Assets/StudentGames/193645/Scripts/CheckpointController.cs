using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _193645
{
    public class CheckpointController : MonoBehaviour
    {
        [SerializeField] private Sprite unusedSprite;
        [SerializeField] private Sprite usedSprite;
        [SerializeField] private ParticleSystem confettiSystem;
        private SpriteRenderer sr;
        private bool used = false;
        private bool active = false;

        private void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
            sr.sprite = unusedSprite;
        }

        public void useCheckpoint()
        {
            this.sr.sprite = usedSprite;
            this.used = true;
            confettiSystem.Emit(30);
        }

        public void setActive()
        {
            active = true;
        }

        public bool isUsed()
        {
            return used;
        }

        public bool isActive()
        {
            return active;
        }
    }
}