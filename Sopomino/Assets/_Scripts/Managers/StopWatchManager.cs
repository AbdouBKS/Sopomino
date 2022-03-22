using System;
using UnityEngine;

public class StopWatchManager : StaticInstance<StopWatchManager>
{
    public float TimeSpent { get; private set; }
    public TimeSpan CurrentTime { get; private set;}
    [SerializeField]
    private bool _status;

    private void OnEnable() {
        GameManager.OnBeforeStateChanged += GameRestart;
    }

    private void OnDisable() {
        GameManager.OnBeforeStateChanged -= GameRestart;
    }

    void Start()
    {
        ResetStopWatch();
    }

    void Update()
    {
        if (!_status) {
            return;
        }

        TimeSpent += Time.deltaTime;
        CurrentTime = TimeSpan.FromSeconds(TimeSpent);
    }

    public void SetStopWatchStatus(bool status)
    {
        _status = status;
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
