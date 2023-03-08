using GameMode;
using UnityEngine;
using TMPro;

namespace UI
{
    public class WinningScreen : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI text;

        private void OnEnable() {
            SetWinText();
        }

        public void SetWinText()
        {
            text.text = GameModeManager.Instance.gameMode.GetEndGameMessage();
        }
    }
}
