using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;


public class GameHUD : MonoBehaviour
{
    #region Fields

    [Header("UI Data")]

    [SerializeField]
    private TextMeshProUGUI scoreText;

    [FormerlySerializedAs("_linesText")] [SerializeField]
    private TextMeshProUGUI linesText;

    [SerializeField]
    private TextMeshProUGUI time;
    private string _minutes;

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
        if (GameManager.Instance.State != GameState.Pause) {
            SetTimerText();
        }
        HandleEscapeKey();
    }

    private void HandleEscapeKey()
    {
        if (GameManager.Instance.State == GameState.Loose || !Input.GetKeyDown(KeyCode.Escape)) {
            return;
        }

        if (GameManager.Instance.State == GameState.Pause) {
            BUTTON_Resume();
            return;
        }
        BUTTON_Pause();
    }

    private void SetTimerText()
    {
        TimeSpan currentTime = StopWatchManager.Instance.CurrentTime;

        _minutes = currentTime.Minutes > 0 ? currentTime.Minutes.ToString("00") + ":" : "";
        time.text = _minutes + currentTime.Seconds.ToString("00") + ":" + currentTime.Milliseconds.ToString("000");
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

    #region Buttons

    public void BUTTON_Pause()
    {
        GameManager.Instance.ChangeState(GameState.Pause);
    }

    public void BUTTON_Resume()
    {
        GameManager.Instance.ChangeState(GameState.Playing);
    }

    public void BUTTON_ResetGame()
    {
        GameManager.Instance.ChangeState(GameState.ResetGame);
    }

    public void BUTTON_Quit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }

    public void BUTTON_GoToMenu()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer) {
            SceneManager.LoadScene(ConstInfo.WEBGL_MAIN_MENU_SCEN);
            return;
        }

        SceneManager.LoadScene(ConstInfo.MAIN_MENU_SCENE);
    }

    #endregion Buttons

    #endregion Methods
}
