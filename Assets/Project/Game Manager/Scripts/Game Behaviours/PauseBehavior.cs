using UnityEngine;
using Game_Manager.Configuration;
using Game_Manager.Events;

namespace Game_Manager.Conditions
{
    [System.Serializable]
    public class PauseBehavior : GameBehaviorBase
    {
        [HideInInspector][SerializeField] private PauseBehaviorConfigSO config;
        [HideInInspector][SerializeField] private PauseCondition pauseCondition;

        public PauseBehavior( BaseGameBehaviorConfigSO _behaviourConfigSO) : base( _behaviourConfigSO)
        {
            config = _behaviourConfigSO as PauseBehaviorConfigSO;
            eventType = GameStateEvent.OnPaused;
        }

        public override void Exit()
        {
            GameManagerEventBus.Raise(GameStateEvent.OnUnPaused);
        }
    }
}