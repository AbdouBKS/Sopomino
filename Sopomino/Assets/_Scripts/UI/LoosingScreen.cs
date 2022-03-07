using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoosingScreen :MonoBehaviour
{
    public void BUTTON_TryAgain()
    {
        GameManager.Instance.ChangeState(GameState.TryAgain);
    }
}
