using Game_Manager;
using Game_Manager.Configuration;
using Game_Manager.Events;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game_Manager.Events
{

    public static class GameManagerEventBus
    {
        private static Dictionary<GameStateEvent, Action> assignedGameStateActions = new Dictionary<GameStateEvent, Action>();
        private static Dictionary<GameRequestEvent, Action> assignedGameRequestActions = new Dictionary<GameRequestEvent, Action>();

        private static HashSet<GameStateEvent> persistentEvents = new HashSet<GameStateEvent>();
        private static GameStateEvent? lastStickyGameStateEvent = null;
        private static bool isInitialized = false;


        /// <summary>
        /// Must be called Once by GameManager at the start of the game to initialize the event bus.
        /// </summary>
        public static void Initialize(EventBusConfigSO config)
        {
            if (isInitialized) return;
            if (config == null)
            {
                Debug.LogError("EventBusConfigSO is missing! Cannot initialize the GameManagerEventBus.");
                return;
            }
            persistentEvents = new HashSet<GameStateEvent>(config.PersistentStateEvents);
            isInitialized = true;
        }

        public static void Raise(GameStateEvent eventType)
        {
            if (!assignedGameStateActions.ContainsKey(eventType))
            {
                /* Debug.LogError($"GameManagerEventBus:Has No listeners assigned for event '{eventType}'. " +
                     $"Please ensure that you have subscribed to this event before raising it."); */
                return;
            }

            if (persistentEvents.Contains(eventType))
            {
                lastStickyGameStateEvent = eventType;
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
            if (persistentEvents.Contains(eventType) && eventType == lastStickyGameStateEvent)
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

            persistentEvents = new HashSet<GameStateEvent>();
            lastStickyGameStateEvent = null;
            isInitialized = false;
        }
    }
}
