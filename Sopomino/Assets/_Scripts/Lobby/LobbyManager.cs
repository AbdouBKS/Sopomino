using System;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;


namespace MyLobby {
    public class LobbyManager : StaticInstance<LobbyManager> {

        public const string KeyPlayerName = "PlayerName";

        public static event Action OnLeftLobby;

        public static event Action<Lobby> OnJoinedLobby;
        public static event Action<Lobby> OnJoinedLobbyUpdate;
        public static event Action OnKickedFromLobby;
        public static event Action<List<Lobby>> OnLobbyListChanged;

        private const float HEAR_BEAT_TIMER_MAX = 15f;
        private const float LOBBY_POLL_TIMER_MAX = 1.1f;
        private const float REFRESH_LOBBY_LIST_TIMER_MAX = 5f;

        private float _heartbeatTimer;
        private float _lobbyPollTimer;
        private float _refreshLobbyListTimer = 5f;

        private Lobby _joinedLobby;
        private string _playerName;

        private void Update() {
            HandleRefreshLobbyList();
            HandleLobbyHeartbeat();
            HandleLobbyPolling();
        }

        public async void Authenticate(string playerName) {
            this._playerName = playerName;
            InitializationOptions initializationOptions = new InitializationOptions();
            initializationOptions.SetProfile(playerName);

            await UnityServices.InitializeAsync(initializationOptions);

            AuthenticationService.Instance.SignedIn += () => {
                // do nothing
                Debug.Log("Signed in! " + AuthenticationService.Instance.PlayerId);

                RefreshLobbyList();
            };

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        private void HandleRefreshLobbyList() {
            if (UnityServices.State == ServicesInitializationState.Initialized && AuthenticationService.Instance.IsSignedIn) {
                _refreshLobbyListTimer -= Time.deltaTime;
                if (_refreshLobbyListTimer < 0f) {
                    _refreshLobbyListTimer = REFRESH_LOBBY_LIST_TIMER_MAX;

                    RefreshLobbyList();
                }
            }
        }

        private async void HandleLobbyHeartbeat() {
            if (IsLobbyHost()) {
                _heartbeatTimer -= Time.deltaTime;
                if (_heartbeatTimer < 0f) {
                    _heartbeatTimer = HEAR_BEAT_TIMER_MAX;

                    Debug.Log("Heartbeat");
                    await LobbyService.Instance.SendHeartbeatPingAsync(_joinedLobby.Id);
                }
            }
        }

        private async void HandleLobbyPolling() {
            if (_joinedLobby != null) {
                _lobbyPollTimer -= Time.deltaTime;
                if (_lobbyPollTimer < 0f) {
                    _lobbyPollTimer = LOBBY_POLL_TIMER_MAX;

                    _joinedLobby = await LobbyService.Instance.GetLobbyAsync(_joinedLobby.Id);

                    OnJoinedLobbyUpdate?.Invoke(_joinedLobby);

                    if (!IsPlayerInLobby()) {
                        // Player was kicked out of this lobby
                        Debug.Log("Kicked from Lobby!");

                        OnKickedFromLobby?.Invoke();

                        _joinedLobby = null;
                    }
                }
            }
        }

        public Lobby GetJoinedLobby() {
            return _joinedLobby;
        }

        public bool IsLobbyHost() {
            return _joinedLobby != null && _joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
        }

        private bool IsPlayerInLobby() {
            if (_joinedLobby != null && _joinedLobby.Players != null) {
                foreach (Player player in _joinedLobby.Players) {
                    if (player.Id == AuthenticationService.Instance.PlayerId) {
                        // This player is in this lobby
                        return true;
                    }
                }
            }
            return false;
        }

        private Player GetPlayer() {
            return new Player(AuthenticationService.Instance.PlayerId, null, new Dictionary<string, PlayerDataObject> {
                { KeyPlayerName, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, _playerName) },
            });
        }

        public async void CreateLobby(string lobbyName, int maxPlayers, bool isPrivate) {
            Player player = GetPlayer();

            CreateLobbyOptions options = new CreateLobbyOptions {
                Player = player,
                IsPrivate = isPrivate,
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);

            _joinedLobby = lobby;

            OnJoinedLobby?.Invoke(lobby);
          Debug.Log("Created Lobby " + lobby.Name);
        }

        public async void RefreshLobbyList() {
            try {
                QueryLobbiesOptions options = new QueryLobbiesOptions();
                options.Count = 25;

                // Filter for open lobbies only
                options.Filters = new List<QueryFilter> {
                    new QueryFilter(
                        field: QueryFilter.FieldOptions.AvailableSlots,
                        op: QueryFilter.OpOptions.GT,
                        value: "0")
                };

                // Order by newest lobbies first
                options.Order = new List<QueryOrder> {
                    new QueryOrder(
                        asc: false,
                        field: QueryOrder.FieldOptions.Created)
                };

                QueryResponse lobbyListQueryResponse = await Lobbies.Instance.QueryLobbiesAsync();

                OnLobbyListChanged?.Invoke(lobbyListQueryResponse.Results);
            } catch (LobbyServiceException e) {
                Debug.Log(e);
            }
        }

        public async void JoinLobbyByCode(string lobbyCode) {
            Player player = GetPlayer();

            Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, new JoinLobbyByCodeOptions {
                Player = player
            });

            _joinedLobby = lobby;

            OnJoinedLobby?.Invoke(lobby);     }

        public async void JoinLobby(Lobby lobby) {
            Player player = GetPlayer();

            _joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id, new JoinLobbyByIdOptions {
                Player = player
            });

            OnJoinedLobby?.Invoke(lobby);     }

        public async void UpdatePlayerName(string playerName) {
            this._playerName = playerName;

            if (_joinedLobby != null) {
                try {
                    UpdatePlayerOptions options = new UpdatePlayerOptions();

                    options.Data = new Dictionary<string, PlayerDataObject>() {
                        {
                            KeyPlayerName, new PlayerDataObject(
                                visibility: PlayerDataObject.VisibilityOptions.Public,
                                value: playerName)
                        }
                    };

                    string playerId = AuthenticationService.Instance.PlayerId;

                    Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(_joinedLobby.Id, playerId, options);
                    _joinedLobby = lobby;

                    OnJoinedLobbyUpdate?.Invoke(_joinedLobby);
                } catch (LobbyServiceException e) {
                    Debug.Log(e);
                }
            }
        }

        public async void QuickJoinLobby() {
            try {
                QuickJoinLobbyOptions options = new QuickJoinLobbyOptions();

                Lobby lobby = await LobbyService.Instance.QuickJoinLobbyAsync(options);
                _joinedLobby = lobby;

                OnJoinedLobby?.Invoke(lobby);
            } catch (LobbyServiceException e) {
                Debug.Log(e);
            }
        }

        public async void LeaveLobby() {
            if (_joinedLobby == null) {
                return;
            }
            try {
                await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, AuthenticationService.Instance.PlayerId);

                _joinedLobby = null;

                OnLeftLobby?.Invoke();
            } catch (LobbyServiceException e) {
                Debug.Log(e);
            }
        }

        public async void KickPlayer(string playerId) {
            if (IsLobbyHost()) {
                try {
                    await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, playerId);
                } catch (LobbyServiceException e) {
                    Debug.Log(e);
                }
            }
        }
    }
}
