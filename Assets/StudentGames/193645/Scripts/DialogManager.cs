using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace _193645
{
    public class DialogManager : MonoBehaviour
    {
        public GameObject dialogBox;
        public TMP_Text dialogText;
        public string dialog;

        public bool playerInRange;
        private GameObject pressEPrompt;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                playerInRange = true;
                pressEPrompt.SetActive(true);
            }
        }
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                playerInRange = false;
                dialogBox.SetActive(false);
                pressEPrompt.SetActive(false);
            }
        }

        void Start()
        {
            pressEPrompt = this.transform.GetChild(0).gameObject;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.E) && playerInRange)
            {
                if (dialogBox.activeInHierarchy)
                {
                    dialogBox.SetActive(false);
                }
                else
                {
                    dialogBox.SetActive(true);
                    dialogText.text = dialog;
                }
            }
        }

    }
}