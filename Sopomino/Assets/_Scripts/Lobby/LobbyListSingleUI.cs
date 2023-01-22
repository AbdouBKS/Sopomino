using UnityEngine;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine.UI;


namespace MyLobby {
    public class LobbyListSingleUI : MonoBehaviour {


        [SerializeField] private TextMeshProUGUI lobbyNameText;
        [SerializeField] private TextMeshProUGUI playersText;


        private Lobby _lobby;


        private void Awake() {
            GetComponent<Button>().onClick.AddListener(() => {
                LobbyManager.Instance.JoinLobby(_lobby);
            });
        }

        public void UpdateLobby(Lobby lobby) {
            _lobby = lobby;

            lobbyNameText.text = lobby.Name;
            playersText.text = lobby.Players.Count + "/" + lobby.MaxPlayers;
        }


    }
}
