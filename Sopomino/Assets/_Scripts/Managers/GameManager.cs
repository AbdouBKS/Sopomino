using System;

public class GameManager : StaticInstance<GameManager> {
    public static event Action<GameState> OnBeforeStateChanged;
    public static event Action<GameState> OnAfterStateChanged;

    public GameState State { get; private set; } = GameState.Initiating;

    void Start() => ChangeState(GameState.Starting);

    public void ChangeState(GameState newState) {
        OnBeforeStateChanged?.Invoke(newState);

        State = newState;
        switch (newState) {
            case GameState.Initiating:
                HandleInitiating();
                break;
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

    private void HandleInitiating()
    {
    }

    private void HandlePause()
    {
    }

    private void HandleStarting()
    {
        GridManager.Instance.StartGame();
        TetriminosManager.Instance.StartGame();
        ChangeState(GameState.Playing);
    }

    private void HandleLoose()
    {
        TetriminosManager.Instance.EndGame();
        GridManager.Instance.EndGame();
    }

    private void HandleTryAgain()
    {
        ChangeState(GameState.Starting);
    }

    private void HandleResetGame()
    {
        ChangeState(GameState.TryAgain);
    }

    private void HandlePlaying()
    {
    }
}

[Serializable]
public enum GameState {
    Initiating,
    Starting,
    Playing,
    Loose,
    TryAgain,
    ResetGame,
    Pause,
}
