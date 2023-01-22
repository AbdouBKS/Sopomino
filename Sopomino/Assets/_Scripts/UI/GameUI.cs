using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameUI : MonoBehaviour
{
    #region Fields

    [Header("UI")]

    [SerializeField]
    private GameObject loosingScreen;

    [SerializeField]
    private GameObject pause;

    #endregion

    private void OnEnable() {
        GameManager.OnAfterStateChanged += SetLoosingScreen;
        GameManager.OnAfterStateChanged += SetPauseScreen;
    }

    private void OnDisable() {
        GameManager.OnAfterStateChanged -= SetLoosingScreen;
        GameManager.OnAfterStateChanged -= SetPauseScreen;
    }

    private void SetLoosingScreen(GameState state) {
        if (state != GameState.Loose) {
            loosingScreen.SetActive(false);
            return;
        }

        loosingScreen.SetActive(true);
    }

    private void SetPauseScreen(GameState state) {
        if (state != GameState.Pause) {
            pause.SetActive(false);
            return;
        }

        pause.SetActive(true);
    }
}
