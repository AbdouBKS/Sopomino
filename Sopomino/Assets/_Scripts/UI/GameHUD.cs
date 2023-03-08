using System;
using System.Collections.Generic;
using GameMode;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class GameHUD : MonoBehaviour
{
    #region Fields

    [Header("UI Data")]

    [SerializeField]
    private TextMeshProUGUI scoreText;

    [SerializeField]
    private TextMeshProUGUI linesText;

    [SerializeField]
    private TextMeshProUGUI time;

    [SerializeField]
    private TextMeshProUGUI gameModeName;

    [Header("Tetriminos information")]

    [SerializeField]
    private List<Image> nextTetriminosPlaceholder;

    [SerializeField]
    private Image swappablePlaceholder;

    private const string EMPTY_LOGO = "Empty";

    [Header("Logos")]

    [SerializeField]
    private List<Sprite> tetriminoLogos;
    private Dictionary<string, Sprite> _tetriminoLogoDict;

    #endregion Fields

    #region Methods

    private void Awake()
    {
        InitTetriminosLogoDict();
        ResetTextsUI();
    }

    private void Start()
    {
        SetGameModeName();
    }

    private void OnEnable() {
        ScoreManager.OnScoreChange += SetScore;
        ScoreManager.OnLinesChange += SetLines;
        TetriminosManager.OnSwappableChange += SetSwappable;
        TetriminosManager.OnTetriminoBufferChange += SetNextTetriminos;

        GameManager.OnAfterStateChanged += ResetGameUI;
    }

    private void OnDisable() {
        ScoreManager.OnScoreChange -= SetScore;
        TetriminosManager.OnSwappableChange -= SetSwappable;
        TetriminosManager.OnTetriminoBufferChange -= SetNextTetriminos;

        GameManager.OnAfterStateChanged -= ResetGameUI;
    }

    private void Update()
    {
        if (GameManager.Instance.State == GameState.Pause) return;

        SetTimerText();
    }

    private void SetGameModeName()
    {
        if (!IsGameModeSet()) return;

        gameModeName.text = GameModeManager.Instance.gameMode.gameModeName.ToLower();
    }

    private void SetTimerText()
    {
        time.text = StopWatchManager.Instance.GetTimeToString();
    }

    private void SetScore(int score)
    {
        scoreText.text = score.ToString();
    }

    private void SetLines(int lines)
    {
        linesText.text = lines.ToString();
    }

    private void SetSwappable(string swappableName)
    {
        swappablePlaceholder.sprite = _tetriminoLogoDict[swappableName];
    }

    private void SetNextTetriminos()
    {
        int index = 0;

        foreach (var nextTetrimino in TetriminosManager.Instance.NextTetriminos)
        {
            nextTetriminosPlaceholder[index].sprite = _tetriminoLogoDict[nextTetrimino.name.Split(' ')[0]];
            index++;
        }
    }

    private void InitTetriminosLogoDict()
    {
        _tetriminoLogoDict = new Dictionary<string, Sprite>(tetriminoLogos.Count);

        foreach (var tetriminoLogo in tetriminoLogos) {
            _tetriminoLogoDict.Add(tetriminoLogo.name, tetriminoLogo);
        }
    }

    private void ResetGameUI(GameState newState)
    {
        if (newState != GameState.Starting) {
            return;
        }

        SetSwappable(EMPTY_LOGO);
    }

    private void ResetTextsUI()
    {
        linesText.text = "0";
        scoreText.text = "0";
        time.text = "00:00:000";
    }

    #region Tools

    private bool IsGameModeSet()
    {
        return GameModeManager.Instance && GameModeManager.Instance.gameMode;
    }

    #endregion Tools

    #endregion Methods
}
