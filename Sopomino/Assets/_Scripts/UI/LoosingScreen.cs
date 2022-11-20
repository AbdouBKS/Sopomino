using System;
using TMPro;
using UnityEngine;

public class LoosingScreen :MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _statText;

    private void OnEnable() {
        GameManager.OnAfterStateChanged += SetLooseText;
    }

    private void OnDisable() {
        GameManager.OnAfterStateChanged -= SetLooseText;
    }

    public void SetLooseText(GameState state)
    {
        if (state != GameState.Loose) {
            return;
        }

        TimeSpan currentTime = StopWatchManager.Instance.CurrentTime;
        string minutes = currentTime.Minutes > 0 ? currentTime.Minutes.ToString("00") + ":" : "";
        string time = minutes + currentTime.Seconds.ToString("00") + ":" + currentTime.Milliseconds.ToString("000");

        string lines = ScoreManager.Instance.Lines.ToString();
        string score = ScoreManager.Instance.Score.ToString();

        _statText.text = "You broke " + lines + " lines for a score of " + score + "\n" +
                         "Your time is " + time;
    }
}
