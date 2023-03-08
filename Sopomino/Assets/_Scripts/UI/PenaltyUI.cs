using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenaltyUI : MonoBehaviour
{

    private void OnEnable() {
        GridManager.OnPenaltyCountChange += ScalePenaltyBar;
    }

    private void OnDisable()
    {
        GridManager.OnPenaltyCountChange -= ScalePenaltyBar;
    }

    private void ScalePenaltyBar(int penaltyCount)
    {
        transform.localScale = new Vector3(0.5f, penaltyCount, 1);
    }
}
