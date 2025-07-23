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
        [HideInInspector][SerializeField] protected GameRequestEvent requestEventType;

        public GameCondition(BaseGameBehaviorConfigSO _configSO)
        {
            configSO = _configSO;
        }
        public abstract void Initialize();

        /// <summary>
        /// This is intended to be called When a new scene is loaded
        /// Useful if you want to unsubscribe from events or reset variables in the condition
        ///</summary>
        public virtual void CleanUp() { }
        protected void TriggerGameConditionMet()
        {
            HandleOnGameConditionMet();
            GameManagerEventBus.Raise(requestEventType);
        }

        /// <summary>
        /// This needs to be ovverriden in every derived class
        /// Mainly used if you want to any custom or specific logic when the condition is met
        /// </summary>
        protected abstract void HandleOnGameConditionMet();
    }
}