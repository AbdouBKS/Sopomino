using UnityEngine;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine.UI;


namespace MyLobby {
    public class LobbyPlayerSingleUI : MonoBehaviour {


        [SerializeField] private TextMeshProUGUI playerNameText;
        [SerializeField] private Button kickPlayerButton;


        private Player _player;


        private void Awake() {
            kickPlayerButton.onClick.AddListener(KickPlayer);
        }

        public void SetKickPlayerButtonVisible(bool visible) {
            kickPlayerButton.gameObject.SetActive(visible);
        }

        public void UpdatePlayer(Player player) {
            _player = player;
            playerNameText.text = player.Data[LobbyManager.KeyPlayerName].Value;
        }

        private void KickPlayer() {
            if (_player != null) {
                LobbyManager.Instance.KickPlayer(_player.Id);
            }
        }


    }
}
