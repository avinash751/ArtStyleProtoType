using Game_Manager.Events;
using UnityEngine;
using UnityEngine.Events;
using Game_Manager;


namespace Game_Manager.Events
{
    /// <summary>
    /// This Class is used to bind Inspector related logic or vfx,sound and last minute stuff
    /// If you want to access these events in code or other scripts
    /// please refer to the GameManagerEnums file for the list of event types
    /// Then use the Event bus to Subscribe to specefic Game Manager Event you want
    /// DO NOT MAKE THESE EVENTS PUBLIC TO ACESS THEM
    /// </summary>
    [AddComponentMenu("Game Manager System/Events/All Game States Event Listeners")]
    public class AllGameStatesEventListners : MonoBehaviour
    {
       //UnityEvents for each specific GameStateEvent
        [Header("State Notifications")]
        [SerializeField] UnityEvent OnInitializedEvent;
        [SerializeField] UnityEvent OnStartEvent;
        [SerializeField] UnityEvent OnPlayEvent;
        [SerializeField] UnityEvent OnPausedEvent;
        [SerializeField] UnityEvent OnUnPausedEvent;
        [SerializeField] UnityEvent OnWinEvent;
        [SerializeField] UnityEvent OnLoseEvent;
        [SerializeField] UnityEvent OnRestartEvent; 
        [SerializeField] UnityEvent OnQuitEvent;    
        [SerializeField] UnityEvent OnStateChangedEvent;


        private void OnEnable()
        {
            // Subscribe the Invoke method of each UnityEvent directly
            GameManagerEventBus.Subscribe(GameStateEvent.OnInitialized, OnInitializedEvent.Invoke);
            GameManagerEventBus.Subscribe(GameStateEvent.OnStart, OnStartEvent.Invoke);
            GameManagerEventBus.Subscribe(GameStateEvent.OnPlay, OnPlayEvent.Invoke);
            GameManagerEventBus.Subscribe(GameStateEvent.OnPaused, OnPausedEvent.Invoke);
            GameManagerEventBus.Subscribe(GameStateEvent.OnUnPaused, OnUnPausedEvent.Invoke);
            GameManagerEventBus.Subscribe(GameStateEvent.OnWin, OnWinEvent.Invoke);
            GameManagerEventBus.Subscribe(GameStateEvent.OnLose, OnLoseEvent.Invoke);
            GameManagerEventBus.Subscribe(GameStateEvent.OnRestart, OnRestartEvent.Invoke);
            GameManagerEventBus.Subscribe(GameStateEvent.OnQuit, OnQuitEvent.Invoke);
            GameManagerEventBus.Subscribe(GameStateEvent.OnStateChanged, OnStateChangedEvent.Invoke);
        }

        private void OnDisable()
        {
            // Unsubscribe the Invoke method of each UnityEvent directly
            GameManagerEventBus.Unsubscribe(GameStateEvent.OnInitialized, OnInitializedEvent.Invoke);
            GameManagerEventBus.Unsubscribe(GameStateEvent.OnStart, OnStartEvent.Invoke);
            GameManagerEventBus.Unsubscribe(GameStateEvent.OnPlay, OnPlayEvent.Invoke);
            GameManagerEventBus.Unsubscribe(GameStateEvent.OnPaused, OnPausedEvent.Invoke);
            GameManagerEventBus.Unsubscribe(GameStateEvent.OnUnPaused, OnUnPausedEvent.Invoke);
            GameManagerEventBus.Unsubscribe(GameStateEvent.OnWin, OnWinEvent.Invoke);
            GameManagerEventBus.Unsubscribe(GameStateEvent.OnLose, OnLoseEvent.Invoke);
            GameManagerEventBus.Unsubscribe(GameStateEvent.OnRestart, OnRestartEvent.Invoke);
            GameManagerEventBus.Unsubscribe(GameStateEvent.OnQuit, OnQuitEvent.Invoke);
            GameManagerEventBus.Unsubscribe(GameStateEvent.OnStateChanged, OnStateChangedEvent.Invoke);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (gameObject.name == "GameObject" || gameObject.name.StartsWith("Listener_"))
            {
                gameObject.name = "All Game States Listener";
            }
        }
#endif
    }
}
