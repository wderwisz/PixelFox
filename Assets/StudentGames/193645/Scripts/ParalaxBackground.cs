using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace _193645
{
    public class ParalaxBackground : MonoBehaviour
    {
        [SerializeField] private Vector2 paralaxEffectMultiplier;
        private Transform cameraTransform;
        private Vector3 lastCameraPosition;
        private float textureUnitSizeX;
        public SpriteAtlas spriteAtlas;
        void Start()
        {
            cameraTransform = Camera.main.transform;
            lastCameraPosition = cameraTransform.position;
            Sprite sprite = GetComponent<SpriteRenderer>().sprite;
            Texture2D texture = sprite.texture;
            textureUnitSizeX = texture.width / sprite.pixelsPerUnit / 5 * 1.5f;
        }

        void LateUpdate()
        {
            Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
            transform.position += new Vector3(deltaMovement.x * paralaxEffectMultiplier.x, deltaMovement.y * paralaxEffectMultiplier.y);
            lastCameraPosition = cameraTransform.position;

            if (Mathf.Abs(cameraTransform.position.x - transform.position.x) >= textureUnitSizeX)
            {
                float offsetPositionX = (cameraTransform.position.x - transform.position.x) % textureUnitSizeX;
                transform.position = new Vector3(cameraTransform.position.x + offsetPositionX, transform.position.y);
            }
        }
    }
}