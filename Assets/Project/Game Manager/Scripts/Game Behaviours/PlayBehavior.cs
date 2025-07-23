using Game_Manager.UI;
using UnityEngine;
using Game_Manager.Configuration;

namespace Game_Manager.GameBehaviors
{
    [System.Serializable]
    public class PlayBehavior : GameBehaviorBase
    {
        public PlayBehavior(BaseGameBehaviorConfigSO _behaviorConfigSO): base(_behaviorConfigSO)
        {
            eventType = GameStateEvent.OnPlay;
        }
    }
}