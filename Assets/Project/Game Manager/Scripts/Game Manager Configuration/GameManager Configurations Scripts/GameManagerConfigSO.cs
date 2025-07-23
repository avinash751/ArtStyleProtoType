using System.Collections.Generic;
using UnityEngine;
using Game_Manager.GameBehaviors;
using Game_Manager.Conditions;

namespace Game_Manager.Configuration
{
    [CreateAssetMenu(fileName = "GameManagerConfig", menuName = "Game Manager/Game Manager Config", order = 0)]
    public class GameManagerConfigSO : ScriptableObject
    {
        public bool IsPersistant;
        public BaseGameBehaviorConfigSO prefferedRestartGameBehaviour;
        public List<BehaviorConfiguration> behaviorConfigurations;

        [System.Serializable]
        public class BehaviorConfiguration
        {
            public string behaviorName;
            public BaseGameBehaviorConfigSO configSO;
        }

        private void OnValidate()
        {
            if (behaviorConfigurations.Count == 0)
            {
                Debug.LogWarning("No Behavior Configurations found in GameManagerConfigSO. Please add some.");
            }
            else
            {

                for (int i = 0; i < behaviorConfigurations.Count; i++)
                {
                    BehaviorConfiguration behaviorConfiguration = behaviorConfigurations[i];
                    if (behaviorConfiguration.configSO == null) continue;
                    behaviorConfiguration.behaviorName = behaviorConfiguration.configSO.BehaviorType + " Behaviour";
                }
            }
        }

        public List<GameBehaviorBase> CreateAllGameBehaviours()
        {
            List<GameBehaviorBase> registeredGameBehaviours = new List<GameBehaviorBase>();
            foreach (BehaviorConfiguration behaviorConfiguration in behaviorConfigurations)
            {
                GameBehaviorBase newCreatedBehaviour = behaviorConfiguration.configSO.CreateBehavior();
                if (newCreatedBehaviour != null)
                {
                    registeredGameBehaviours.Add(newCreatedBehaviour);
                }
                else
                {
                    Debug.LogError("The behaviour created from" + behaviorConfiguration.configSO +
                        " is null please check in code whether it is making its assoiated behaviour or not");
                }
            }
            return registeredGameBehaviours;
        }

        public List<GameCondition> CreateAllGameConditions()
        {
            List<GameCondition> registeredGameConditions = new List<GameCondition>();
            foreach (BehaviorConfiguration behaviorConfiguration in behaviorConfigurations)
            {
                GameCondition newGameCondition = behaviorConfiguration.configSO.CreateGameCondition();
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