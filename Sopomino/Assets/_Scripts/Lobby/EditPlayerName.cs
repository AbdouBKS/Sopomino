using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace MyLobby
{
    public class EditPlayerName : StaticInstance<EditPlayerName> {

        public event Action<string> OnNameChanged;


        [SerializeField] private TextMeshProUGUI playerNameText;


        private string _playerName;


        protected override void Awake() {
            base.Awake();

            GetComponent<Button>().onClick.AddListener(() => ShowPlayerNameInputWindow());

            var playerName = "Player-" + Random.Range(1, 100).ToString("000");

            SetPLayerName(playerName);
        }

        private void Start() {
            OnNameChanged += EditPlayerName_OnNameChanged;
        }

        private void OnDestroy()
        {
            OnNameChanged -= EditPlayerName_OnNameChanged;
        }

        private void ShowPlayerNameInputWindow() {
            UI_InputWindow.Show_Static(
                "Player Name",
                _playerName,
                "abcdefghijklmnopqrstuvxywzABCDEFGHIJKLMNOPQRSTUVXYWZ .,-",
                16,
                () => { /* Cancel */},
                playerName => { SetPLayerName(playerName); }
            );
        }

        private void SetPLayerName(string playerName) {
            _playerName = playerName;
            playerNameText.text = _playerName;
            OnNameChanged?.Invoke(_playerName);
        }


        private void EditPlayerName_OnNameChanged(string playerName) {
            LobbyManager.Instance.UpdatePlayerName(playerName);
        }

        public string GetPlayerName() {
            return _playerName;
        }


    }
}
