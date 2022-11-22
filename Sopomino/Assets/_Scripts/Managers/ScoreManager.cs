using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : StaticInstance<ScoreManager>
{
    #region Fields

    private int _score;
    public int Score {
        get {
            return _score;
        }
        private set {
            _score = value;
            OnScoreChange?.Invoke(_score);
        }
    }

    private int _lines;
    public int Lines {
        get {
            return _lines;
        }
        private set {
            _lines = value;
            OnLinesChange?.Invoke(_lines);
        }
    }

    #region  Actions

    public static Action<int> OnScoreChange;

    public static Action<int> OnLinesChange;

    #endregion

    #endregion

    #region Methods

    private void OnEnable()
    {
        GridManager.OnLinesComplete += AddScore;
        GridManager.OnLinesComplete += AddLines;
    }

    protected override void Awake()
    {
        base.Awake();
        ResetGame();
    }

    public void ResetGame()
    {
        Score = 0;
        Lines = 0;
    }



    private void AddScore(int score)
    {
        Score += score * 2 - 1;
    }

    private void AddLines(int lines)
    {
        Lines += lines;
    }

    #endregion
}
