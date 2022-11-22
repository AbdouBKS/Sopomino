using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenalityUI : MonoBehaviour
{

    private void OnEnable() {
        GridManager.OnPenalityCountChange += scalePenalityBar;
    }

    private void OnDisable()
    {
        GridManager.OnPenalityCountChange += scalePenalityBar;
    }

    private void scalePenalityBar(int penalityCount)
    {
        transform.localScale = new Vector3(0.5f, penalityCount, 1);
    }
}
