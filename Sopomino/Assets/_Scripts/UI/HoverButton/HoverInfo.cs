using System;
using TMPro;
using UnityEngine;

namespace UI
{
    public class HoverInfo : MonoBehaviour
    {
        public string title;
        public string description;

        private void Start()
        {
            SetButtonText();
        }

        private void SetButtonText()
        {
            var text = GetComponentInChildren<TMP_Text>();

            if (text == null)
            {
                Debug.LogWarning("Text component not found on button prefab.");
                return;
            }

            text.text = title;
        }
    }
}
