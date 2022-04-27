using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{

    [SerializeField]
    private GameObject _mainMenu;
    [SerializeField]
    private GameObject _credit;

    public void BUTTON_Play()
    {
        SceneManager.LoadScene(ConstInfo.GAME_SCENE);
    }

    public void BUTTON_Credit()
    {
        _mainMenu.SetActive(false);
        _credit.SetActive(true);
    }

    public void BUTTON_Quit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }

    public void BUTTON_BackToMain()
    {
        _credit.SetActive(false);
        _mainMenu.SetActive(true);
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
