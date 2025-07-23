using UnityEngine;
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
    public class LoseCondition : GameCondition
    {
        public LoseCondition(BaseGameBehaviorConfigSO _configSO) : base(_configSO)
        {
            conditionName = "Lose Condition";
            configSO = _configSO;
            requestEventType = GameRequestEvent.RequestLoseGame;
        }
        public override void Initialize()
        {

        }
        protected override void HandleOnGameConditionMet()
        {

        }
        public override void CleanUp()
        {

        }
    }
}

