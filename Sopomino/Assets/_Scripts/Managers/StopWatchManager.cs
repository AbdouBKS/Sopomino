using System;
using UnityEngine;

public class StopWatchManager : StaticInstance<StopWatchManager>
{
    private float _timeSpent;
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

        _timeSpent += Time.deltaTime;
        CurrentTime = TimeSpan.FromSeconds(_timeSpent);
    }

    public void SetStopWatchStatus(bool status)
    {
        pause = status;
    }

    public void ResetStopWatch()
    {
        _timeSpent = 0;
    }

    private void GameRestart(GameState state)
    {
        if (state == GameState.Starting) {
            ResetStopWatch();
        }

        SetStopWatchStatus(state == GameState.Playing ? true : false);
    }

    public string GetTimeToString()
    {
        return CurrentTime.ToString(@"mm\:ss\:fff");
    }

    public int GetTimeInSeconds()
    {
        return (int)CurrentTime.TotalSeconds;
    }
}
