public struct ConstInfo
{

    #region Scenes

    #if UNITY_WEBGL
        public const string MAIN_MENU_SCENE = "MainMenu - WebGL";
    #else
        public const string MAIN_MENU_SCENE = "MainMenu";
    #endif

    public const string GAME_SCENE = "Game";
    public const string GAME_MODE_SCENE = "ChooseGameMode";

    #endregion Scenes

    #region Links

    public const string TWITTER_LINK = "https://twitter.com/AbdouSopo";
    public const string TWITCH_LINK = "https://twitch.tv/AbdouSopo";
    public const string GITHUB_LINK = "https://github.com/AbdouBKS/Sopomino";

    #endregion Links
}
