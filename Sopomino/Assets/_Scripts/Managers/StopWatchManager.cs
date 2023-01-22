using System;
using UnityEngine;

public class StopWatchManager : StaticInstance<StopWatchManager>
{
    public float TimeSpent { get; private set; }
    public TimeSpan CurrentTime { get; private set;}
    [SerializeField]
    private bool pause;

    private void OnEnable() {
        GameManager.OnBeforeStateChanged += GameRestart;
    }

    private void OnDisable() {
        GameManager.OnBeforeStateChanged -= GameRestart;
    }

    private void Start()
    {
        ResetStopWatch();
    }

    private void Update()
    {
        if (!pause) {
            return;
        }

        TimeSpent += Time.deltaTime;
        CurrentTime = TimeSpan.FromSeconds(TimeSpent);
    }

    public void SetStopWatchStatus(bool status)
    {
        pause = status;
    }

    public void ResetStopWatch()
    {
        TimeSpent = 0;
    }

    private void GameRestart(GameState state)
    {
        if (state == GameState.Starting) {
            ResetStopWatch();
        }

        SetStopWatchStatus(state == GameState.Playing ? true : false);
    }
}
