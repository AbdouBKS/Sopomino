using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MyLobby
{
    public class EditPlayerName : StaticInstance<EditPlayerName> {

        public event EventHandler OnNameChanged;


        [SerializeField] private TextMeshProUGUI playerNameText;


        private string playerName = "Code Monkey";


        protected override void Awake() {
            base.Awake();

            GetComponent<Button>().onClick.AddListener(() => {
                UI_InputWindow.Show_Static("Player Name", playerName, "abcdefghijklmnopqrstuvxywzABCDEFGHIJKLMNOPQRSTUVXYWZ .,-", 20,
                () => {
                    // Cancel
                },
                (string newName) => {
                    playerName = newName;

                    playerNameText.text = playerName;

                    OnNameChanged?.Invoke(this, EventArgs.Empty);
                });
            });

            playerNameText.text = playerName;
        }

        private void Start() {
            OnNameChanged += EditPlayerName_OnNameChanged;
        }

        private void EditPlayerName_OnNameChanged(object sender, EventArgs e) {
            LobbyManager.Instance.UpdatePlayerName(GetPlayerName());
        }

        public string GetPlayerName() {
            return playerName;
        }


    }
}
