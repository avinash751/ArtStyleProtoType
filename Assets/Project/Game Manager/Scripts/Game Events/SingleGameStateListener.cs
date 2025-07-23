using Game_Manager;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Game_Manager.Events
{
    [AddComponentMenu("Game Manager System/Events/SingleGameStateListener")]
    public class SingleGameStateListener : MonoBehaviour
    {
        [SerializeField] GameStateEvent stateEventType;
        [SerializeField] UnityEvent OnEventTypeInvoked;

        void OnEnable()
        {
            GameManagerEventBus.Subscribe(stateEventType, OnEventTypeInvoked.Invoke);
        }
        void OnDisable()
        {
            GameManagerEventBus.Unsubscribe(stateEventType, OnEventTypeInvoked.Invoke);
        }

        void OnValidate()
        {
            gameObject.name = stateEventType.ToString() + "Listener_";
        }
    }
}
