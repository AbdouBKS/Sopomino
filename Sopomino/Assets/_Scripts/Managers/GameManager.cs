using System;
using UnityEngine;

public class GameManager : StaticInstance<GameManager> {
    public static event Action<GameState> OnBeforeStateChanged;
    public static event Action<GameState> OnAfterStateChanged;

    [Header("UI")]
    [SerializeField]
    private GameObject _loosingScreen;

    [SerializeField]
    private GameObject _pause;

    public GameState State { get; private set; }

    void Start() => ChangeState(GameState.Starting);

    public void ChangeState(GameState newState) {
        OnBeforeStateChanged?.Invoke(newState);

        State = newState;
        switch (newState) {
            case GameState.Starting:
                HandleStarting();
                break;
            case GameState.Loose:
                HandleLoose();
                break;
            case GameState.TryAgain:
                HandleTryAgain();
                break;
            case GameState.Pause:
                HandlePause();
                break;
            case GameState.Playing:
                HandlePlaying();
                break;
            case GameState.ResetGame:
                HandleResetGame();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnAfterStateChanged?.Invoke(newState);

    }

    private void HandlePause()
    {
        _pause.SetActive(true);
    }

    private void HandleStarting()
    {
        TetriminosManager.Instance.enabled = true;
        ChangeState(GameState.Playing);
    }

    private void HandleLoose()
    {
        _loosingScreen.SetActive(true);
        TetriminosManager.Instance.enabled = false;
    }

    private void HandleTryAgain()
    {
        TetriminosManager.Instance.CleanTetriminos();
        ChangeState(GameState.Starting);
    }

    private void HandleResetGame()
    {
        TetriminosManager.Instance.enabled = false;
        ChangeState(GameState.TryAgain);
    }

    private void HandlePlaying()
    {
        ResetUI();
    }

    private void ResetUI()
    {
        _loosingScreen.SetActive(false);
        _pause.SetActive(false);
    }
}

[Serializable]
public enum GameState {
    Starting,
    Playing,
    Loose,
    TryAgain,
    ResetGame,
    Pause,
}
