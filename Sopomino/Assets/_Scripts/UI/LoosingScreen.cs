using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class LoosingScreen :MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI text;

    private void OnEnable() {
        SetLooseText();
    }

    public void SetLooseText()
    {
        var lines = ScoreManager.Instance.Lines.ToString();
        var score = ScoreManager.Instance.Score.ToString();
        var time = StopWatchManager.Instance.GetTimeToString();

        text.text = $"You broke {lines} lines for a score of {score}" +
                    $"\n Your time is {time}";
    }
}
