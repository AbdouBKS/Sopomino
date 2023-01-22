using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;


namespace MyLobby {
    public class LobbyUI : StaticInstance<LobbyUI> {

        [SerializeField] private Transform playerSingleTemplate;
        [SerializeField] private Transform container;
        [SerializeField] private TextMeshProUGUI lobbyNameText;
        [SerializeField] private TextMeshProUGUI playerCountText;
        [SerializeField] private Button leaveLobbyButton;


        protected override void Awake() {
            base.Awake();
            playerSingleTemplate.gameObject.SetActive(false);

            leaveLobbyButton.onClick.AddListener(() => {
                LobbyManager.Instance.LeaveLobby();
            });
        }

        private void Start() {
            LobbyManager.OnJoinedLobby += UpdateLobby_Event;
            LobbyManager.OnJoinedLobbyUpdate += UpdateLobby_Event;
            LobbyManager.OnLeftLobby += LobbyManager_OnLeftLobby;
            LobbyManager.OnKickedFromLobby += LobbyManager_OnLeftLobby;

            Hide();
        }

        private void LobbyManager_OnLeftLobby() {
            ClearLobby();
            Hide();
        }

        private void UpdateLobby_Event(Lobby lobby) {
            UpdateLobby(lobby);
        }

        private void UpdateLobby(Lobby lobby) {
            ClearLobby();

            foreach (Player player in lobby.Players) {
                Transform playerSingleTransform = Instantiate(playerSingleTemplate, container);
                playerSingleTransform.gameObject.SetActive(true);
                LobbyPlayerSingleUI lobbyPlayerSingleUI = playerSingleTransform.GetComponent<LobbyPlayerSingleUI>();

                lobbyPlayerSingleUI.SetKickPlayerButtonVisible(
                    LobbyManager.Instance.IsLobbyHost() &&
                    player.Id != AuthenticationService.Instance.PlayerId // Don't allow kick self
                );

                lobbyPlayerSingleUI.UpdatePlayer(player);
            }


            lobbyNameText.text = lobby.Name;
            playerCountText.text = lobby.Players.Count + "/" + lobby.MaxPlayers;

            Show();
        }

        private void ClearLobby() {
            foreach (Transform child in container) {
                if (child == playerSingleTemplate) continue;
                Destroy(child.gameObject);
            }
        }

        private void Hide() {
            gameObject.SetActive(false);
        }

        private void Show() {
            gameObject.SetActive(true);
        }

    }
}
