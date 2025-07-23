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
        private GameManagerEventToken tokenRaiser;

        [SerializeReference] private List<GameBehaviorBase> gameBehaviors = new();
        [SerializeReference] List<GameCondition> gameConditions = new();

        #region Game Manager Intialization
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            if (gameManagerConfigSo.IsPersistant)
            {
                // This allows The GameManager to be marked as DontDestroyOnLoad
                transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
            }
            tokenRaiser = new GameManagerEventToken();
            GameManagerEventBus.Raise(GameStateEvent.OnInitialized, tokenRaiser);
        }
        #endregion


        private void OnValidate()
        {
            if (gameManagerConfigSo == null)
            {
                Debug.LogWarning("No Game Manager Config found on GameManager. Please assign one.");
                gameBehaviors.Clear();
                gameConditions.Clear();
            }
            else
            {
                gameBehaviors = gameManagerConfigSo.CreateAllGameBehaviours();
                gameConditions = gameManagerConfigSo.CreateAllGameConditions();
            }
        }

        #region Game State Control

        private void Start()
        {
            gameConditions.ForEach(condition => condition.Initialize());
            StartGame();
        }

        private void Update()
        {
            if (CurrentBehavior != null)
            {
                CurrentBehavior.OnUpdate();
            }
            foreach(GameCondition condition in gameConditions)
            {
                condition.OnUpdate(Time.deltaTime);
            }
        }

        public void StartGame()
        {
            ResetAllBehaviours();
            InitializeGameConditions();
            TransitionToType<StartBehavior>();
        }



        public void PlayGame() => TransitionToType<PlayBehavior>();

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

        public void WinGame() => TransitionToType<WinBehavior>();

        public void LoseGame() => TransitionToType<LoseBehavior>();

        public void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            GameManagerEventBus.Raise(GameStateEvent.OnRestart, tokenRaiser);
            InitializeGameConditions();
            if(gameManagerConfigSo.prefferedRestartGameBehaviour!=null)
            {
                GameBehaviorBase tempBehaviourInstnace = gameManagerConfigSo.prefferedRestartGameBehaviour.CreateBehavior();
                TransitionTo(tempBehaviourInstnace);
            }
            else
            {
                TransitionToType<PlayBehavior>();
            }
        }

        public void QuitGame()
        {
            GameManagerEventBus.Raise(GameStateEvent.OnQuit, tokenRaiser);
            Application.Quit();
        }

        #endregion

        #region Behavior & Conditions Related Helper Fucntions

        private T GetBehavior<T>() where T : GameBehaviorBase
        {
            foreach (var behavior in gameBehaviors)
            {
                if (behavior is T targetBehavior)
                {
                    return targetBehavior;
                }
            }
            Debug.LogWarning($"No GameBehavior of type {typeof(T)} found on GameManager.");
            return null;
        }

        private void TransitionToType<T>() where T : GameBehaviorBase
        {
            T newBehavior = GetBehavior<T>();
            if (newBehavior == null)
            {
                Debug.LogError($"Null Reference,Could Not Tranasition to {typeof(T)} as the the behavior type was not found on GameManager.");
                return;
            }

            if (CurrentBehavior != null) { CurrentBehavior.Exit(); }
            CurrentBehavior = newBehavior;
            CurrentBehavior.Enter();
            GameManagerEventBus.Raise(GameStateEvent.OnStateChanged, tokenRaiser);
        }
        
        private void TransitionTo(GameBehaviorBase behaviourType)
        {
            for(int i = 0; i < gameBehaviors.Count; i++)
            {
                if (gameBehaviors[i].GetType() == behaviourType.GetType())
                {
                    if (CurrentBehavior != null) { CurrentBehavior.Exit(); }
                    CurrentBehavior = gameBehaviors[i];
                    CurrentBehavior.Enter();
                    GameManagerEventBus.Raise(GameStateEvent.OnStateChanged, tokenRaiser);
                    return;
                }
            }
            Debug.LogError($"Null Reference,Could Not Tranasition to {behaviourType.GetType()}" +
                           $" as the the behavior type was not found on GameManager behaviour list.");
        }
        private void ResetAllBehaviours()
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
        }

        /// <summary>
        /// This is used when new scenes are loaded,
        /// It is done to re-raise the current behaviour event, 
        /// to establish connection with new scene specefic GameObjects,
        /// which may have been destroyed in the previous scene on Behavior Entering.
        /// This is especillay true for State behaviours that need to load a scene
        /// </summary>
        private void ReRaiseCurrentBehaviourEvent(Scene scene, LoadSceneMode loadMode)
        {
            if (CurrentBehavior != null)
            {
                GameBehaviorBase gameBehavior = (GameBehaviorBase)CurrentBehavior;
                GameManagerEventBus.Raise(gameBehavior.eventType, tokenRaiser);
                GameManagerEventBus.Raise(gameBehavior.InGameUIEventType, tokenRaiser);
            }
        }
        #endregion

        #region Event Subscriptions
        private void OnEnable()
        {
            if (gameManagerConfigSo != null && gameManagerConfigSo.IsPersistant)
            {
                SceneManager.sceneLoaded += ReRaiseCurrentBehaviourEvent;
            }
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
            gameConditions.ForEach(condition => condition.CleanUp());
            if (gameManagerConfigSo != null && gameManagerConfigSo.IsPersistant)
            {
                SceneManager.sceneLoaded -= ReRaiseCurrentBehaviourEvent;
            }
            GameManagerEventBus.Unsubscribe(GameRequestEvent.RequestStartGame, StartGame);
            GameManagerEventBus.Unsubscribe(GameRequestEvent.RequestPlayGame, PlayGame);
            GameManagerEventBus.Unsubscribe(GameRequestEvent.RequestPauseGame, TogglePause);
            GameManagerEventBus.Unsubscribe(GameRequestEvent.RequestUnPauseGame, TogglePause);
            GameManagerEventBus.Unsubscribe(GameRequestEvent.RequestWinGame, WinGame);
            GameManagerEventBus.Unsubscribe(GameRequestEvent.RequestLoseGame, LoseGame);
            GameManagerEventBus.Unsubscribe(GameRequestEvent.RequestRestartGame, RestartGame);
            GameManagerEventBus.Unsubscribe(GameRequestEvent.RequestQuitGame, QuitGame);
        }
        private void OnDestroy()
        {
            gameConditions.ForEach(condition => condition.CleanUp());
            if (gameManagerConfigSo != null && gameManagerConfigSo.IsPersistant)
            {
                SceneManager.sceneLoaded -= ReRaiseCurrentBehaviourEvent;
            }
            GameManagerEventBus.Unsubscribe(GameRequestEvent.RequestStartGame, StartGame);
            GameManagerEventBus.Unsubscribe(GameRequestEvent.RequestPlayGame, PlayGame);
            GameManagerEventBus.Unsubscribe(GameRequestEvent.RequestPauseGame, TogglePause);
            GameManagerEventBus.Unsubscribe(GameRequestEvent.RequestUnPauseGame, TogglePause);
            GameManagerEventBus.Unsubscribe(GameRequestEvent.RequestWinGame, WinGame);
            GameManagerEventBus.Unsubscribe(GameRequestEvent.RequestLoseGame, LoseGame);
            GameManagerEventBus.Unsubscribe(GameRequestEvent.RequestRestartGame, RestartGame);
            GameManagerEventBus.Unsubscribe(GameRequestEvent.RequestQuitGame, QuitGame);
            Instance = null;
        }
        #endregion
    }
}