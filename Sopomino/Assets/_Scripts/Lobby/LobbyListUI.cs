using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;


namespace MyLobby {
    public class LobbyListUI : StaticInstance<LobbyListUI> {

        [SerializeField] private Transform lobbySingleTemplate;
        [SerializeField] private Transform container;
        [SerializeField] private Button refreshButton;
        [SerializeField] private Button createLobbyButton;


        protected override void Awake() {
            base.Awake();

            lobbySingleTemplate.gameObject.SetActive(false);

            refreshButton.onClick.AddListener(RefreshButtonClick);
            createLobbyButton.onClick.AddListener(CreateLobbyButtonClick);
        }

        private void Start() {
            LobbyManager.OnLobbyListChanged += LobbyManager_OnLobbyListChanged;
            LobbyManager.OnJoinedLobby += LobbyManager_OnJoinedLobby;
            LobbyManager.OnLeftLobby += LobbyManager_OnLeftLobby;
            LobbyManager.OnKickedFromLobby += LobbyManager_OnKickedFromLobby;
        }

        private void LobbyManager_OnKickedFromLobby() {
            Show();
        }

        private void LobbyManager_OnLeftLobby() {
            Show();
        }

        private void LobbyManager_OnJoinedLobby(Lobby lobby) {
            Hide();
        }

        private void LobbyManager_OnLobbyListChanged(List<Lobby> lobbies) {
            UpdateLobbyList(lobbies);
        }

        private void UpdateLobbyList(List<Lobby> lobbyList) {
            foreach (Transform child in container) {
                if (child == lobbySingleTemplate) continue;

                Destroy(child.gameObject);
            }

            foreach (Lobby lobby in lobbyList) {
                Transform lobbySingleTransform = Instantiate(lobbySingleTemplate, container);
                lobbySingleTransform.gameObject.SetActive(true);
                LobbyListSingleUI lobbyListSingleUI = lobbySingleTransform.GetComponent<LobbyListSingleUI>();
                lobbyListSingleUI.UpdateLobby(lobby);
            }
        }

        private void RefreshButtonClick() {
            LobbyManager.Instance.RefreshLobbyList();
        }

        private void CreateLobbyButtonClick() {
            LobbyCreateUI.Instance.Show();
        }

        private void Hide() {
            gameObject.SetActive(false);
        }

        private void Show() {
            gameObject.SetActive(true);
        }

    }
}
