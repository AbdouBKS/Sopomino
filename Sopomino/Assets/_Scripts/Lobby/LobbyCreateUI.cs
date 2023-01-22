using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace MyLobby {
    public class LobbyCreateUI : StaticInstance<LobbyCreateUI> {

        [SerializeField] private Button createButton;
        [SerializeField] private Button lobbyNameButton;
        [SerializeField] private Button publicPrivateButton;
        [SerializeField] private Button maxPlayersButton;
        [SerializeField] private TextMeshProUGUI lobbyNameText;
        [SerializeField] private TextMeshProUGUI publicPrivateText;
        [SerializeField] private TextMeshProUGUI maxPlayersText;


        private string _lobbyName;
        private bool _isPrivate;
        private int _maxPlayers;
        protected override void Awake() {
            base.Awake();
            createButton.onClick.AddListener(() => {
                LobbyManager.Instance.CreateLobby(
                    _lobbyName,
                    _maxPlayers,
                    _isPrivate
                );
                Hide();
            });

            lobbyNameButton.onClick.AddListener(() => {
                UI_InputWindow.Show_Static("Lobby Name",
                    _lobbyName,
                    "abcdefghijklmnopqrstuvxywzABCDEFGHIJKLMNOPQRSTUVXYWZ .,-",
                    20,
                onCancel:() => {
                    // Cancel
                },
                onOk: (lobbyName) => {
                    _lobbyName = lobbyName;
                    UpdateText();
                });
            });

            publicPrivateButton.onClick.AddListener(() => {
                _isPrivate = !_isPrivate;
                UpdateText();
            });

            maxPlayersButton.onClick.AddListener(() => {
                UI_InputWindow.Show_Static("Max Players", _maxPlayers,
                () => {
                    // Cancel
                },
                (maxPlayers) => {
                    _maxPlayers = maxPlayers;
                    UpdateText();
                });
            });

            Hide();
        }

        private void UpdateText() {
            lobbyNameText.text = _lobbyName;
            publicPrivateText.text = _isPrivate ? "Private" : "Public";
            maxPlayersText.text = _maxPlayers.ToString();
        }

        private void Hide() {
            gameObject.SetActive(false);
        }

        public void Show() {
            gameObject.SetActive(true);

            _lobbyName = "MyLobby";
            _isPrivate = false;
            _maxPlayers = 4;

            UpdateText();
        }

    }
}
