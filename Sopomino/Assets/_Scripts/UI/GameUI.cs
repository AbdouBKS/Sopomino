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
    private GameObject winningScreen;

    [SerializeField]
    private GameObject pauseScreen;

    [SerializeField]
    private GameObject countDownScreen;

    private GameObject[] _screens;

    #endregion

    private void OnEnable() {
        GameManager.OnBeforeStateChanged += SetScreenByState;
    }

    private void OnDisable() {
        GameManager.OnBeforeStateChanged -= SetScreenByState;
    }

    private void Start()
    {
        _screens = new[]
        {
            loosingScreen,
            winningScreen,
            pauseScreen,
            countDownScreen
        };

    }

    private void SetScreenByState(GameState state)
    {
        DeactivateAllScreens();

        switch (state)
        {
            case GameState.Countdown:
                countDownScreen.SetActive(true);
                break;
            case GameState.Loose:
                loosingScreen.SetActive(true);
                break;
            case GameState.Pause:
                pauseScreen.SetActive(true);
                break;
            case GameState.Win:
                winningScreen.SetActive(true);
                break;
        }

    }

    private void DeactivateAllScreens()
    {
        foreach (var screen in _screens)
        {
            screen.SetActive(false);
        }
    }

    private void Update()
    {
        HandleEscapeKey();
    }

    private void HandleEscapeKey()
    {
        if (!Input.GetKeyDown(KeyCode.Escape) || Helpers.IsEndGameState(GameManager.Instance.State)) {
            return;
        }

        if (GameManager.Instance.State == GameState.Pause) {
            BUTTON_Resume();
            return;
        }
        BUTTON_Pause();
    }

    #region Buttons

    public void BUTTON_Pause()
    {
        GameManager.Instance.ChangeState(GameState.Pause);
    }

    public void BUTTON_Resume()
    {
        GameManager.Instance.ChangeState(GameState.Playing);
    }

    public void BUTTON_ResetGame()
    {
        GameManager.Instance.ChangeState(GameState.ResetGame);
    }

    public void BUTTON_Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    public void BUTTON_GoToMenu()
    {
        GameManager.Instance.ChangeState(GameState.Menu);
        SceneManager.LoadScene(ConstInfo.MAIN_MENU_SCENE);
    }

    #endregion Buttons
}
