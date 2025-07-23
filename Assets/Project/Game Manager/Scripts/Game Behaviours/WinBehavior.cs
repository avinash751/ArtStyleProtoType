using Game_Manager.Configuration;

namespace Game_Manager.Conditions
{
    [System.Serializable]
    public class WinBehavior : GameBehaviorBase
    {
        public WinBehavior(BaseGameBehaviorConfigSO _behaviorConfigSO) : base( _behaviorConfigSO)
        {
            eventType = GameStateEvent.OnWin;
        }
    }
}