
using System;
using UnityEngine;
using UI;

namespace GameMode
{
    public class GameModeManager : PersistentSingleton<GameModeManager>
    {
        #region Fields

        public GameMode gameMode { get; private set; }

        private bool _canChangeGameMode = true;
        private bool _isPlaying;
        private bool _checkTime;

        #endregion Fields

        #region Methods

        private void OnEnable()
        {
            GameManager.OnAfterStateChanged += UpdateCanChangeGameMode;
            GameManager.OnBeforeStateChanged += UpdateIsPlaying;
            ScoreManager.OnLinesChange += HandleGameMode;
        }

        private void OnDisable()
        {
            GameManager.OnAfterStateChanged -= UpdateCanChangeGameMode;
            GameManager.OnBeforeStateChanged -= UpdateIsPlaying;

            if (!gameMode) return;

            switch (gameMode.limitType)
            {
                case GameMode.LimitType.Time:
                    _checkTime = false;
                    break;
                case GameMode.LimitType.Lines:
                    ScoreManager.OnLinesChange -= HandleGameMode;
                    break;
                case GameMode.LimitType.Score:
                    ScoreManager.OnScoreChange -= HandleGameMode;
                    break;
            }
        }

        private void FixedUpdate()
        {
            if (!_checkTime) return;

            if (!StopWatchManager.Instance) return;

            HandleGameMode(StopWatchManager.Instance.GetTimeInSeconds());
        }

        public void SetGameMode(GameMode selectedGameMode)
        {
            if (!_canChangeGameMode) {
                Debug.LogWarning($"Status {GameManager.Instance.State.ToString()}: doesn't permit to change game mode.");
                return;
            }

            gameMode = selectedGameMode;

            SetLimitType();

            void SetLimitType()
            {
                switch (gameMode.limitType)
                {
                    case GameMode.LimitType.Time:
                        _checkTime = true;
                        break;
                    case GameMode.LimitType.Lines:
                        ScoreManager.OnLinesChange += HandleGameMode;
                        break;
                    case GameMode.LimitType.Score:
                        ScoreManager.OnScoreChange += HandleGameMode;
                        break;
                }
            }
        }

        #region Actions

        private void HandleGameMode(int value)
        {
            if (!_isPlaying) return;

            if (value < gameMode.limit) return;

            GameManager.Instance.ChangeState(GameState.Win);
        }

        private void UpdateIsPlaying(GameState gameState)
        {
            if (gameState != GameState.Playing) {
                _isPlaying = false;
                return;
            }

            _isPlaying = true;
        }

        private void UpdateCanChangeGameMode(GameState gameState)
        {
            if (StatusPermitsToChangeGameMode(gameState)) {
               _canChangeGameMode = true;
                return;
            }

            _canChangeGameMode = false;
        }

        #endregion Actions

        #region Tools

        private bool StatusPermitsToChangeGameMode(GameState gameState)
        {
            return gameState == GameState.Menu;
        }

        #endregion Tools

        #endregion Methods
    }
}
