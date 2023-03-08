using UnityEngine;
using UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GameMode
{
    public class GameModeUI : MonoBehaviour
    {
        #region Fields

        [Header("Data")]
        [SerializeField] private GameMode[] gameModes;

        [Header("Prefabs")]
        [SerializeField] private HoverButton prefabGameModeButton;

        [Header("Scene References")]
        [SerializeField] private Transform transformButtonParent;
        [SerializeField] private Button playButton;

        #endregion Fields

        #region Methods

        private void Start()
        {
            playButton.interactable = false;
            GenerateGameModeUI();

            void GenerateGameModeUI()
            {
                transformButtonParent.DestroyChildren();

                foreach (var gameMode in gameModes)
                {
                    GenerateGameModeButton(gameMode);
                }

                void GenerateGameModeButton(GameMode gameMode)
                {
                    var button = Instantiate(prefabGameModeButton, transformButtonParent);

                    if (!button.TryGetComponent<HoverInfo>(out var hoverInfo))
                    {
                        Debug.LogWarning("HoverInfo component not found on button prefab.");
                        return;
                    }

                    button.name = gameMode.gameModeName;
                    button.onClick.AddListener(() =>
                    {
                        playButton.interactable = true;
                        GameModeManager.Instance.SetGameMode(gameMode);
                    });
                    hoverInfo.title = gameMode.gameModeName;
                    hoverInfo.description = gameMode.description;
                }
            }
        }

        #endregion Methods

        #region Button

        public void GAME_MODE_UI_OnClickBack()
        {
            SceneManager.LoadScene(ConstInfo.MAIN_MENU_SCENE);
        }

        public void GAME_MODE_UI_OnClickPlay()
        {
            SceneManager.LoadScene(ConstInfo.GAME_SCENE);
        }

        #endregion
    }
}
