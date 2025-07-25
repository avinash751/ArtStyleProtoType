using UnityEngine;
using UnityEngine.SceneManagement;
using Game_Manager.Events;
using Game_Manager.Configuration;


namespace Game_Manager
{

    [System.Serializable]
    public abstract class GameBehaviorBase : IGameBehavior
    {
        [SerializeField][HideInInspector] string behaviorName;
        [SerializeField] protected BaseGameBehaviorConfigSO behaviorConfigSO;

        [HideInInspector][SerializeField] protected bool isInitialEnter = true;
        [field: SerializeField][HideInInspector] public GameStateEvent eventType { get; protected set; }
        [field: SerializeField][HideInInspector] public GameStateEvent InGameUIEventType { get; protected set; }
        public BaseGameBehaviorConfigSO BehaviorConfigSO => behaviorConfigSO;

        public GameBehaviorBase(BaseGameBehaviorConfigSO _behaviorConfigSO)
        {
            behaviorConfigSO = _behaviorConfigSO;
            behaviorName = _behaviorConfigSO.BehaviorType.ToString() + " Behavior";
            isInitialEnter = true;
            SetInGameUiEventType();
        }

        public void Enter(bool skipSceneLoading = false)
        {
            OnEnter();
            ApplyBaseSettings(skipSceneLoading);
        }

        /// <summary>
        /// This is intended to be called every frame when the behavior is active
        /// This needs to be overridden in the derived class if custom logic needs 
        /// to run every frame
        /// </summary>
        public virtual void OnUpdate() { }
        public virtual void Exit() { }

        public void Reset()
        {
            isInitialEnter = true;
        }
        /// <summary>
        ///  If The behavior needs to have custom logic when
        ///  entering the state,then it needs to be overridden in the derived class
        /// </summary>
        protected virtual void OnEnter() { }

        /// <summary>
        /// if the behavior needs to have custom logic when
        /// resetting the state, then it needs to be overridden in the derived class
        protected virtual void OnReset() { }

        private void ApplyBaseSettings(bool skipSceneLoading)
        {
            if (behaviorConfigSO == null)
            {
                Debug.LogError("Config So is not assigned to this Game Manager Behavior.Please assign it in the Inspector.");

            }
            //Debug.Log("Executing " + GetType().ToString());
            SetTimescale(behaviorConfigSO.IsTimeZeroOnExecution ? 0f : 1f);
            SetCursorLockMode(behaviorConfigSO.IsCursorLockedOnExecution);
            SetCursorVisible(behaviorConfigSO.IsCursorVisibleOnExecution);
            if(!skipSceneLoading)
            {
                HandleSceneLoading(behaviorConfigSO.SceneLoadTypeOnExecution);       
            }
            isInitialEnter = false;
            GameManagerEventBus.Raise(eventType);
            GameManagerEventBus.Raise(InGameUIEventType);
        }

        private void SetInGameUiEventType()
        {
            if (behaviorConfigSO.ShowGameUIOnExecution)
            {
                InGameUIEventType = GameStateEvent.OnInGameUIActive;
            }
            else
            {
                InGameUIEventType = GameStateEvent.OnInGameUIInactive;
            }
        }

        protected virtual void SetTimescale(float customScale)
        {
            Time.timeScale = customScale;
        }

        protected virtual void SetCursorLockMode(bool enabled)
        {
            if (enabled)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
            }
        }

        protected virtual void SetCursorVisible(bool visible)
        {
            Cursor.visible = visible;
        }

        protected void HandleSceneLoading(SceneLoadType loadType)
        {
            switch (loadType)
            {
                case SceneLoadType.NoSceneLoad:
                    break;
                case SceneLoadType.LoadSceneOnce:
                    if (!isInitialEnter) break;
                    LoadScene(behaviorConfigSO.SceneToLoad);
                    break;
                case SceneLoadType.LoadSceneAlways:
                    LoadScene(behaviorConfigSO.SceneToLoad);
                    break;
            }
        }

        void LoadScene(int sceneIndex)
        {
            if (sceneIndex == SceneManager.GetActiveScene().buildIndex)
            {
                Debug.LogWarning("Trying to load the same scene again. Please check the scene index.");
                return;
            }
            SceneManager.LoadScene(sceneIndex);
        }
    }
}