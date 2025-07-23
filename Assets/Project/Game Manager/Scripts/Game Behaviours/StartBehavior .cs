using Game_Manager.Configuration;

namespace Game_Manager.GameBehaviors
{
    [System.Serializable]
    public class StartBehavior : GameBehaviorBase
    {
        public StartBehavior(BaseGameBehaviorConfigSO _behaviorConfigSO) : base(_behaviorConfigSO)
        {
            eventType = GameStateEvent.OnStart;
        }
    }
}