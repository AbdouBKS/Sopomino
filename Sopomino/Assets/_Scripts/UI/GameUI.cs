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

    private void OnEnable() {
        TetriminosManager.OnScoreChange += SetScore;
    }

    private void OnDisable() {
        TetriminosManager.OnScoreChange -= SetScore;
    }

    private void SetScore()
    {
        _scoreText.text = TetriminosManager.Instance.Score.ToString();
        _linesText.text = TetriminosManager.Instance.Lines.ToString();
    }
}
