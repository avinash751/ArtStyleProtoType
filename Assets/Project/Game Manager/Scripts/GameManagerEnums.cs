
namespace Game_Manager
{

    /// <summary>
    /// This is used to define the different game request events that can be raised 
    /// To trigger GameStateEvents like OnStart, OnPlay, OnPause, OnWin, OnLose etc
    /// </summary>
    public enum GameRequestEvent
    {
        RequestStartGame,
        RequestPlayGame,
        RequestPauseGame,
        RequestUnPauseGame,
        RequestWinGame,
        RequestLoseGame,
        RequestRestartGame,
        RequestQuitGame,
    }

    /// <summary>
    /// This is used to define the different game state events that are primarily raised in 
    /// State Behaviour classes like StartBehaviour, PlayBehaviour, PauseBehaviour etc
    public enum GameStateEvent
    {
        OnStateChanged,
        OnInitialized,
        OnStart,
        OnPlay,
        OnPaused,
        OnUnPaused,
        OnWin,
        OnLose,
        OnRestart, 
        OnQuit,
        OnInGameUIActive,
        OnInGameUIInactive,
    }
    public enum GameBehaviorType
    {
        Start,
        Play,
        Paused,
        Win,
        Lose
    }

    public enum  ButtonType
    {
        StartGame,
        PauseGame,
        ResumeGame,
        RestartGame,
        GoToMainMenu,
        QuitGame
    }

    public enum SceneLoadType
    {
        NoSceneLoad,     // Never load a scene for this behavior
        LoadSceneOnce,   // Load only on the initial Enter() if configured
        LoadSceneAlways  // Always load the scene on Enter() if configured
    }
}
