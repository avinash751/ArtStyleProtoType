
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
    /// State Behavior classes like StartBehavior, PlayBehavior, PauseBehavior etc
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

    public enum OperatingMode
    {
        NonPersistent,          // Standard: Starts from scratch every time.
        NonPersistentSceneAware,// Starts in the state matching the active scene.
        Persistent,             // DontDestroyOnLoad is enabled, but still starts from scratch every time.
        PersistentSceneAware    // DontDestroyOnLoad + starts in the state matching the active scene.
    }


}
