using UnityEngine;
using Game_Manager.Configuration;

namespace Game_Manager.Conditions
{
    [System.Serializable]
    public class LoseBehavior : GameBehaviorBase
    {
        public LoseBehavior(BaseGameBehaviorConfigSO _behaviorConfigSO) : base(_behaviorConfigSO)
        {
            eventType = GameStateEvent.OnLose;
        }
    }
}