using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MyLobby
{
    public class AuthenticateUI : MonoBehaviour {

        [SerializeField] private Button authenticateButton;

        public static event Action OnAuthenticated;

        private void Awake() {
            authenticateButton.onClick.AddListener(() => {
                LobbyManager.Instance.Authenticate(EditPlayerName.Instance.GetPlayerName());

                OnAuthenticated?.Invoke();

                Hide();
            });
        }

        private void Hide() {
            gameObject.SetActive(false);
        }

    }
}
