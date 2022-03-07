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

        GameManager.OnAfterStateChanged += ResetUi;
    }

    private void OnDisable() {
        TetriminosManager.OnScoreChange -= SetScore;
        TetriminosManager.OnSwappableChange -= SetSwappable;
        TetriminosManager.OnTetriminoBufferChange -= SetNextTetriminos;

        GameManager.OnAfterStateChanged -= ResetUi;
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

    private void ResetUi(GameState newState)
    {
        if (newState != GameState.Starting) {
            return;
        }

        SetSwappable("Empty");
    }
}
