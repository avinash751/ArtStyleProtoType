using System;
using Game_Manager.Configuration;
using Game_Manager.GameBehaviors;
using Game_Manager.Conditions;
using Game_Manager.Events;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Game_Manager
{
    [AddComponentMenu("Game Manager System/Game Manager")]
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public IGameBehavior CurrentBehavior { get; private set; }

        [Header("References")]
        [SerializeField] GameManagerConfigSO gameManagerConfigSo;
        [HideInInspector]
        [SerializeReference] List<GameBehaviorBase> gameBehaviors = new();
        [HideInInspector]
        [SerializeReference] List<GameCondition> gameConditions = new();

        private Dictionary<Type, GameBehaviorBase> behaviorsByType = new();
        private Dictionary<BaseGameBehaviorConfigSO, GameBehaviorBase> behaviorsByConfig = new();

        List<IPollableCondition> pollableConditions = new();
        GameBehaviorBase pendingBehaviorToExecuteOnRestart;


        #region Game Manager Intialization
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            PopulateBehaviorDictionaries();

            if (gameManagerConfigSo.Mode == OperatingMode.Persistent ||
                gameManagerConfigSo.Mode == OperatingMode.PersistentSceneAware)
            {
                transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
            }

            HashSet<GameStateEvent> persistentEvents = new HashSet<GameStateEvent>();
            foreach (var behavior in gameBehaviors)
            {
                persistentEvents.Add(behavior.eventType);
            }
            GameManagerEventBus.RegisterStateEvents(persistentEvents);

            GameManagerEventBus.Raise(GameStateEvent.OnInitialized);
        }
        private void Start()
        {
            ResetAllBehaviors();
            InitializeGameConditions();
            GameBehaviorBase initialBehavior = FindInitialBehavior();
            TransitionTo(initialBehavior);
        }

        private void OnValidate()
        {
            // Skip validation in editor mode to avoid unnecessary warnings.
            if (Application.isPlaying) return;
           
            if (gameManagerConfigSo == null)
            {
                Debug.LogWarning("No Game Manager Config found on GameManager. Please assign one.");
                gameBehaviors.Clear();
                gameConditions.Clear();
                behaviorsByType.Clear();
                behaviorsByConfig.Clear();
                return;
            }

            gameBehaviors = gameManagerConfigSo.CreateAllGameBehaviors();
            PopulateBehaviorDictionaries();
            gameConditions = gameManagerConfigSo.CreateAllGameConditions();
        }
        

        private void PopulateBehaviorDictionaries()
        {
            if (behaviorsByType.Count > 0) return;
            if (gameBehaviors.Count == 0) return;

            behaviorsByType.Clear();
            behaviorsByConfig.Clear();

            foreach (var behavior in gameBehaviors)
            {
                // populate the dictionary for lookups by type (e.g., GetBehavior<PlayBehavior>(),)
                if (behaviorsByType.ContainsKey(behavior.GetType()))
                {
                    Debug.LogWarning($"Duplicate behavior type found: {behavior.GetType()}. Check your GameManagerConfigSO.");
                    return;
                }
                behaviorsByType.Add(behavior.GetType(), behavior);


                // Populate the dictionary for lookups by Config SO (e.g., GetBehaviorFromConfig(...))
                if (behavior.BehaviorConfigSO != null && !behaviorsByConfig.ContainsKey(behavior.BehaviorConfigSO))
                {
                    behaviorsByConfig.Add(behavior.BehaviorConfigSO, behavior);
                }
            }
        }
        #endregion

        private void Update()
        {
            if (CurrentBehavior != null)
            {
                CurrentBehavior.OnUpdate();
            }
            foreach (IPollableCondition condition in pollableConditions)
            {
                condition.OnUpdate();
            }
        }

        #region Game State Functions
        private GameBehaviorBase FindInitialBehavior()
        {
            // If we're not in a Scene Aware mode, we always default to StartBehavior.
            if (gameManagerConfigSo.Mode == OperatingMode.NonPersistent ||
                gameManagerConfigSo.Mode == OperatingMode.Persistent)
            {
                return GetBehavior<StartBehavior>();
            }

            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

            foreach (GameBehaviorBase behavior in gameBehaviors)
            {
                bool doesBehaviorOwnScene = behavior.BehaviorConfigSO.SceneToLoad == currentSceneIndex &&
                                            behavior.BehaviorConfigSO.SceneLoadTypeOnExecution != SceneLoadType.NoSceneLoad;
                if (doesBehaviorOwnScene)
                {
                    return behavior;
                }
            }
            // If no behavior owns the current scene, default to StartBehavior.
            return GetBehavior<StartBehavior>();
        }

        public void StartGame() => TransitionToType<StartBehavior>();
        public void PlayGame() => TransitionToType<PlayBehavior>();
        public void WinGame() => TransitionToType<WinBehavior>();
        public void LoseGame() => TransitionToType<LoseBehavior>();
        public void QuitGame()
        {
            GameManagerEventBus.Raise(GameStateEvent.OnQuit);
            Application.Quit();
        }

        public void TogglePause()
        {
            if (CurrentBehavior is PlayBehavior)
            {
                TransitionToType<PauseBehavior>();
            }
            else if (CurrentBehavior is PauseBehavior)
            {
                TransitionToType<PlayBehavior>();
            }
        }

        public void RestartGame()
        {
            ResetAllBehaviors();
            GameManagerEventBus.Raise(GameStateEvent.OnRestart);
            GameBehaviorBase targetBehavior = GetBehaviorFromConfig(gameManagerConfigSo.PreferredRestartGameBehavior);
            if (targetBehavior == null)
            {
                //Debug.LogWarning("PreferredRestartGameBehavior not set, defaulting to PlayBehavior.");
                targetBehavior = GetBehavior<PlayBehavior>();
            }
            if (targetBehavior == null)
            {
                Debug.LogError("FATAL: Could not find a behavior to restart to. Aborting restart.");
                return;
            }
            SceneLoadType sceneLoadType = targetBehavior.BehaviorConfigSO.SceneLoadTypeOnExecution;
            if (sceneLoadType == SceneLoadType.NoSceneLoad)
            {
                // We still skip its "scene loading" because this is a manual override.
                TransitionTo(targetBehavior, true);
                return;
            }
            // Set the pending flag and load the scene. The callback will handle the rest.
            pendingBehaviorToExecuteOnRestart = targetBehavior;
            SceneManager.LoadScene(targetBehavior.BehaviorConfigSO.SceneToLoad);
        }

        void InitializationOnSceneLoaded(Scene scene, LoadSceneMode loadType)
        {
            InitializeGameConditions();
            if (pendingBehaviorToExecuteOnRestart != null)
            {
                TransitionTo(pendingBehaviorToExecuteOnRestart, true);
                pendingBehaviorToExecuteOnRestart = null;
            }
        }

        #endregion

        #region Behavior & Conditions Related Helper Fucntions

        private T GetBehavior<T>() where T : GameBehaviorBase
        {
            if (behaviorsByType.TryGetValue(typeof(T), out GameBehaviorBase behavior))
            {
                return behavior as T;
            }

            Debug.LogWarning($"No GameBehavior of type {typeof(T)} found on GameManager.");
            return null;
        }

        private GameBehaviorBase GetBehaviorFromConfig(BaseGameBehaviorConfigSO behaviorConfig)
        {
            if (behaviorConfig == null) return null;

            if (behaviorsByConfig.TryGetValue(behaviorConfig, out GameBehaviorBase behavior))
            {
                return behavior;
            }
            Debug.LogWarning($"No GameBehavior found for the provided config: {behaviorConfig.name}");
            return null;
        }

        private void TransitionToType<T>(bool skipSceneLoading = false) where T : GameBehaviorBase
        {
            TransitionTo(GetBehavior<T>(), skipSceneLoading);
        }

        private void TransitionTo(GameBehaviorBase behavior, bool skipSceneLoading = false)
        {
            if (behavior == null)
            {
                Debug.LogError("Transition failed: target behavior is null.");
                return;
            }

            if (CurrentBehavior != null) { CurrentBehavior.Exit(); }
            CurrentBehavior = behavior;
            CurrentBehavior.Enter(skipSceneLoading);
            GameManagerEventBus.Raise(GameStateEvent.OnStateChanged);
        }

        private void ResetAllBehaviors()
        {
            foreach (GameBehaviorBase behavior in gameBehaviors)
            {
                behavior.Reset();
            }
        }

        private void InitializeGameConditions()
        {
            foreach (GameCondition condition in gameConditions)
            {
                condition.CleanUp();
                condition.Initialize();
            }
            pollableConditions.Clear();
            foreach (GameCondition condition in gameConditions)
            {
                if (condition is IPollableCondition pollableCondition)
                {
                    pollableConditions.Add(pollableCondition);
                }
            }
        }
        #endregion

        #region Event Subscriptions
        private void OnEnable()
        {
            SceneManager.sceneLoaded += InitializationOnSceneLoaded;
            GameManagerEventBus.Subscribe(GameRequestEvent.RequestStartGame, StartGame);
            GameManagerEventBus.Subscribe(GameRequestEvent.RequestPlayGame, PlayGame);
            GameManagerEventBus.Subscribe(GameRequestEvent.RequestPauseGame, TogglePause);
            GameManagerEventBus.Subscribe(GameRequestEvent.RequestUnPauseGame, TogglePause);
            GameManagerEventBus.Subscribe(GameRequestEvent.RequestWinGame, WinGame);
            GameManagerEventBus.Subscribe(GameRequestEvent.RequestLoseGame, LoseGame);
            GameManagerEventBus.Subscribe(GameRequestEvent.RequestRestartGame, RestartGame);
            GameManagerEventBus.Subscribe(GameRequestEvent.RequestQuitGame, QuitGame);
        }
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= InitializationOnSceneLoaded;
            GameManagerEventBus.Unsubscribe(GameRequestEvent.RequestStartGame, StartGame);
            GameManagerEventBus.Unsubscribe(GameRequestEvent.RequestPlayGame, PlayGame);
            GameManagerEventBus.Unsubscribe(GameRequestEvent.RequestPauseGame, TogglePause);
            GameManagerEventBus.Unsubscribe(GameRequestEvent.RequestUnPauseGame, TogglePause);
            GameManagerEventBus.Unsubscribe(GameRequestEvent.RequestWinGame, WinGame);
            GameManagerEventBus.Unsubscribe(GameRequestEvent.RequestLoseGame, LoseGame);
            GameManagerEventBus.Unsubscribe(GameRequestEvent.RequestRestartGame, RestartGame);
            GameManagerEventBus.Unsubscribe(GameRequestEvent.RequestQuitGame, QuitGame);
        }
        #endregion
    }
}