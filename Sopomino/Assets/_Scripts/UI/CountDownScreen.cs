using System;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace UI
{
    public class CountDownScreen : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text text;

        [SerializeField]
        private int count = 3;

        private async void OnEnable()
        {
            await LaunchCountDown();
        }

        private async Task LaunchCountDown()
        {
            var time = count;
            while (time > 0)
            {
                text.text = time.ToString();
                await Task.Delay(1000);

                time--;
            }

            text.text = "GO !";

            await Task.Delay(1000);

            GameManager.Instance.ChangeState(GameState.Playing);
            gameObject.SetActive(false);
        }
    }
}
