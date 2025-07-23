using System.Collections.Generic;
using UnityEngine;
using Game_Manager.Conditions;

namespace Game_Manager.Configuration
{
    [CreateAssetMenu(fileName = "GameManagerConfig", menuName = "Game Manager/Game Manager Config", order = 0)]
    public class GameManagerConfigSO : ScriptableObject
    {
        public bool IsPersistent;
        public BaseGameBehaviorConfigSO PreferredRestartGameBehavior;

        [Header("System Configurations")]
        public EventBusConfigSO EventBusConfig;
        public List<BehaviorConfiguration> BehaviorConfigurations;

        [System.Serializable]
        public class BehaviorConfiguration
        {
            public string BehaviorName;
            public BaseGameBehaviorConfigSO ConfigSO;
        }

        private void OnValidate()
        {
            if (BehaviorConfigurations.Count == 0)
            {
                Debug.LogWarning("No Behavior Configurations found in GameManagerConfigSO. Please add some.");
            }
            else
            {

                for (int i = 0; i < BehaviorConfigurations.Count; i++)
                {
                    BehaviorConfiguration behaviorConfiguration = BehaviorConfigurations[i];
                    if (behaviorConfiguration.ConfigSO == null) continue;
                    behaviorConfiguration.BehaviorName = behaviorConfiguration.ConfigSO.BehaviorType + " Behavior";
                }
            }
        }

        public List<GameBehaviorBase> CreateAllGameBehaviors()
        {
            List<GameBehaviorBase> registeredGameBehaviors = new List<GameBehaviorBase>();
            foreach (BehaviorConfiguration behaviorConfiguration in BehaviorConfigurations)
            {
                GameBehaviorBase newCreatedBehavior = behaviorConfiguration.ConfigSO.CreateBehavior();
                if (newCreatedBehavior != null)
                {
                    registeredGameBehaviors.Add(newCreatedBehavior);
                }
                else
                {
                    Debug.LogError("The behavior created from" + behaviorConfiguration.ConfigSO +
                        " is null please check in code whether it is making its associated behavior or not");
                }
            }
            return registeredGameBehaviors;
        }

        public List<GameCondition> CreateAllGameConditions()
        {
            List<GameCondition> registeredGameConditions = new List<GameCondition>();
            foreach (BehaviorConfiguration behaviorConfiguration in BehaviorConfigurations)
            {
                GameCondition newGameCondition = behaviorConfiguration.ConfigSO.CreateGameCondition();
                if (newGameCondition != null)
                {
                    registeredGameConditions.Add(newGameCondition);
                }
                // certain behaviors like PlayBehavior do not have or do not need a game condition,
                // so we skip them and dont log error or warning
            }
            return registeredGameConditions;
        }
    }
}