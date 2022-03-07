using System;
using UnityEngine;

public class GameManager : StaticInstance<GameManager> {
    public static event Action<GameState> OnBeforeStateChanged;
    public static event Action<GameState> OnAfterStateChanged;

    public GameObject loosingScreen;

    public GameState State { get; private set; }

    void Start() => ChangeState(GameState.Starting);

    public void ChangeState(GameState newState) {
        OnBeforeStateChanged?.Invoke(newState);

        State = newState;
        switch (newState) {
            case GameState.Starting:
                HandleGameStart();
                break;
            case GameState.Loose:
                HandleLoose();
                break;
            case GameState.TryAgain:
                HandleTryAgain();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnAfterStateChanged?.Invoke(newState);

    }

    private void HandleGameStart()
    {
        loosingScreen.SetActive(false);
        TetriminosManager.Instance.enabled = true;
    }

    private void HandleLoose()
    {
        loosingScreen.SetActive(true);
        TetriminosManager.Instance.enabled = false;
    }

    private void HandleTryAgain()
    {
        TetriminosManager.Instance.CleanTetriminos();
        ChangeState(GameState.Starting);
    }
}

[Serializable]
public enum GameState {
    Starting = 0,
    Loose = 1,
    TryAgain = 2,
}
