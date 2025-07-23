using System;
using UnityEngine;
using Game_Manager.Events;
using Game_Manager.Configuration;

namespace Game_Manager.Conditions
{
    /// <summary>
    /// References cannot be dragged in inspector 
    /// if the Game Manager is marked as persistant
    /// If The Game Manager persists across scenes, then the references
    /// Will be lost and be null in the process
    /// Instead try getting the references 
    /// </summary>
    [System.Serializable]
    public abstract class GameCondition
    {
        [SerializeField][HideInInspector] protected string conditionName;
        [SerializeField] protected BaseGameBehaviorConfigSO configSO;
        [HideInInspector][SerializeField] protected GameManagerEventToken eventToken = new();
        [HideInInspector][SerializeField] protected GameRequestEvent requestEventType;

        public GameCondition(BaseGameBehaviorConfigSO _configSO)
        {
            configSO = _configSO;
        }
        public abstract void Initialize();

        /// <summary>
        /// This is intended to be called every frame when the condition is active
        /// useful if specific game logic conditions needs to be checked every frame
        /// deltaTime is passed in case you want to use it for any calculations 
        /// </summary>
        public virtual void OnUpdate(float deltaTime = 0) { }

        /// <summary>
        /// This is intended to be called When a new scene is loaded
        /// Useful if you want to unsubscribe from events or reset variables in the condition
        ///</summary>
        public virtual void CleanUp() { }
        protected void TriggerGameConditionMet()
        {
            GameManagerEventBus.Raise(requestEventType, eventToken);
            HandleOnGameConditionMet();
        }

        /// <summary>
        /// This needs to be ovverriden in every derived class
        /// Mainly used if you want to any custom or specific logic when the condition is met
        /// </summary>
        protected abstract void HandleOnGameConditionMet();
    }
}