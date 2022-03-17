using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _scoreText;

    [SerializeField]
    private TextMeshProUGUI _linesText;

    [SerializeField]
    private Image _swappableImage;

    [SerializeField]
    private List<Image> _nextTetriminosImage;
    private const string EMPTY_LOGO = "Empty";

    [SerializeField]
    private List<Sprite> _tetriminosLogo;
    private Dictionary<string, Sprite> _tetriminoLogoDict;

    private void Awake()
    {
        InitTetriminosLogoDict();
    }

    private void OnEnable() {
        TetriminosManager.OnScoreChange += SetScore;
        TetriminosManager.OnSwappableChange += SetSwappable;
        TetriminosManager.OnTetriminoBufferChange += SetNextTetriminos;

        GameManager.OnAfterStateChanged += ResetGameUI;
    }

    private void OnDisable() {
        TetriminosManager.OnScoreChange -= SetScore;
        TetriminosManager.OnSwappableChange -= SetSwappable;
        TetriminosManager.OnTetriminoBufferChange -= SetNextTetriminos;

        GameManager.OnAfterStateChanged -= ResetGameUI;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (GameManager.Instance.State == GameState.Pause) {
                BUTTON_Resume();
                return;
            }
            BUTTON_Pause();
        }
    }

    private void SetScore()
    {
        _scoreText.text = TetriminosManager.Instance.Score.ToString();
        _linesText.text = TetriminosManager.Instance.Lines.ToString();
    }

    private void SetSwappable(string swappableName)
    {
        _swappableImage.sprite = _tetriminoLogoDict[swappableName];
    }

    private void SetNextTetriminos()
    {
        int index = 0;

        foreach (var nextTetrimino in TetriminosManager.Instance.NextTetriminos)
        {
            _nextTetriminosImage[index].sprite = _tetriminoLogoDict[nextTetrimino.name.Split(' ')[0]];
            index++;
        }
    }

    private void InitTetriminosLogoDict()
    {
        _tetriminoLogoDict = new Dictionary<string, Sprite>(_tetriminosLogo.Count);

        foreach (var tetriminoLogo in _tetriminosLogo) {
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
}
