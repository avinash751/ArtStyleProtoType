using Game_Manager;
using Game_Manager.Configuration;
using Game_Manager.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game_Manager.Events
{

    public static class GameManagerEventBus
    {
        private static Dictionary<GameStateEvent, Action> assignedGameStateActions = new Dictionary<GameStateEvent, Action>();
        private static Dictionary<GameRequestEvent, Action> assignedGameRequestActions = new Dictionary<GameRequestEvent, Action>();

        // This is generated at run time by the GameManager
        private static HashSet<GameStateEvent> persistentStateEvents = new HashSet<GameStateEvent>();
        private static HashSet<GameStateEvent> persistentNotificationEvents = new HashSet<GameStateEvent>
        {
            GameStateEvent.OnInGameUIActive,
            GameStateEvent.OnInGameUIInactive
        };

        private static GameStateEvent? lastPersistentGameStateEvent = null;
        private static GameStateEvent? lastPersistentNotificationEvent = null;

        /// <summary>
        /// Called once by the GameManager at startup to register all events
        /// that should be treated as persistent "sticky" states.
        /// </summary>
        public static void RegisterStateEvents(HashSet<GameStateEvent> stateEvents)
        {
            persistentStateEvents = stateEvents ?? new HashSet<GameStateEvent>();
        }
        public static void Raise(GameStateEvent eventType)
        {
            if (!assignedGameStateActions.ContainsKey(eventType))
            {
                /* Debug.LogError($"GameManagerEventBus:Has No listeners assigned for event '{eventType}'. " +
                     $"Please ensure that you have subscribed to this event before raising it."); */
                return;
            }

            if (persistentStateEvents.Contains(eventType))
            {
                lastPersistentGameStateEvent = eventType;
            }

            if( persistentNotificationEvents.Contains(eventType))
            {
                lastPersistentNotificationEvent = eventType;
            }

            Delegate[] listeners = assignedGameStateActions[eventType].GetInvocationList();

            foreach (Delegate listener in listeners)
            {
                try
                {
                    ((Action)listener)?.Invoke();
                }
                catch (Exception error)
                {
                    Debug.LogError($"GameManagerEventBus: Exception caught while invoking listener" +
                        $" {listener.Method.DeclaringType?.Name}.{listener.Method.Name} for event '{eventType}'." +
                        $" See details below. Continuing invocation loop.");

                    UnityEngine.Object context = null;
                    if (listener.Target is UnityEngine.Object targetObject)
                    {
                        context = targetObject;
                    }
                    Debug.LogException(error, context);
                }
            }
        }
        public static void Raise(GameRequestEvent eventType)
        {

            if (assignedGameRequestActions.ContainsKey(eventType))
            {
                assignedGameRequestActions[eventType]?.Invoke();
            }
        }
        public static void Subscribe(GameStateEvent eventType, Action action)
        {
            if (assignedGameStateActions.ContainsKey(eventType))
            {
                assignedGameStateActions[eventType] += action;
            }
            else
            {
                assignedGameStateActions.Add(eventType, action);
            }

            // immediately invoke the new subscriber's action to bring it up to speed.
            if (persistentStateEvents.Contains(eventType) && eventType == lastPersistentGameStateEvent)
            {
                action?.Invoke();
            }
            if( persistentNotificationEvents.Contains(eventType) && eventType == lastPersistentNotificationEvent)
            {
                action?.Invoke();
            }
        }
        public static void Subscribe(GameRequestEvent eventType, Action action)
        {
            if (assignedGameRequestActions.ContainsKey(eventType))
            {
                assignedGameRequestActions[eventType] += action;
            }
            else
            {
                assignedGameRequestActions.Add(eventType, action);
            }
        }

        public static void Unsubscribe(GameStateEvent eventType, Action action)
        {
            if (assignedGameStateActions.ContainsKey(eventType))
            {
                assignedGameStateActions[eventType] -= action;
                if (assignedGameStateActions[eventType] == null)
                {
                    assignedGameStateActions.Remove(eventType);
                }
            }
        }

        public static void Unsubscribe(GameRequestEvent eventType, Action action)
        {
            if (assignedGameRequestActions.ContainsKey(eventType))
            {
                assignedGameRequestActions[eventType] -= action;
                if (assignedGameRequestActions[eventType] == null)
                {
                    assignedGameRequestActions.Remove(eventType);
                }
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Reset()
        {
            assignedGameStateActions = new Dictionary<GameStateEvent, Action>();
            assignedGameRequestActions = new Dictionary<GameRequestEvent, Action>();

            persistentStateEvents = new HashSet<GameStateEvent>();
            lastPersistentGameStateEvent = null;
            lastPersistentNotificationEvent = null;
        }
    }
}
