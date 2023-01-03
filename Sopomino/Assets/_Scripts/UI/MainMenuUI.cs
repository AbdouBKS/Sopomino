using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{

    [SerializeField]
    private GameObject _mainMenu;

    [SerializeField]
    private GameObject _credit;

    [SerializeField]
    private GameObject _controls;

    private GameObject _currentMenu;

    public void BUTTON_Play()
    {
        SceneManager.LoadScene(ConstInfo.GAME_SCENE);
    }

    private void Start()
    {
        _currentMenu = _mainMenu;

        _controls.SetActive(false);
        _credit.SetActive(false);
        _mainMenu.SetActive(false);

        _currentMenu.SetActive(true);
    }

    public void BUTTON_Credit()
    {
        _currentMenu.SetActive(false);
        _credit.SetActive(true);
        _currentMenu = _credit;
    }

    public void BUTTON_Quit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }

    public void BUTTON_Controls()
    {
        _currentMenu.SetActive(false);
        _controls.SetActive(true);
        _currentMenu = _controls;
    }

    public void BUTTON_BackToMain()
    {
        _currentMenu.SetActive(false);
        _mainMenu.SetActive(true);
        _currentMenu = _mainMenu;
    }

    public void BUTTON_Twitter()
    {
        Application.OpenURL(ConstInfo.TWITTER_LINK);
    }

    public void BUTTON_Github()
    {
        Application.OpenURL(ConstInfo.GITHUB_LINK);
    }

        public void BUTTON_Twitch()
    {
        Application.OpenURL(ConstInfo.TWITCH_LINK);
    }
}
