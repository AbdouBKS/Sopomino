using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunishmentUI : MonoBehaviour
{

    private void OnEnable() {
        TetriminosManager.OnPunishmentCountChange += scalePunishmentBar;
    }

    private void OnDisable()
    {
        TetriminosManager.OnPunishmentCountChange += scalePunishmentBar;
    }

    private void scalePunishmentBar(int punishmentCount)
    {
        transform.localScale = new Vector3(0.5f, punishmentCount, 1);
    }
}
