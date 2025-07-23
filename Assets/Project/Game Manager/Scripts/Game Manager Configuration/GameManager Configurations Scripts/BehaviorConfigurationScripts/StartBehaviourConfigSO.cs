using Game_Manager;
using Game_Manager.Conditions;
using UnityEngine;
using Game_Manager.GameBehaviors;

namespace Game_Manager.Configuration
{

    [CreateAssetMenu(fileName = "StartBehaviourConfigSO", menuName = "Game Manager/Behavior Configs/Start Behavior Config", order = 2)]
    public class StartBehaviourConfigSO : BaseGameBehaviorConfigSO
    {
        public override GameBehaviorBase CreateBehavior()
        {
            StartBehavior startBehavior = new StartBehavior(this);
            return startBehavior;
        }

        public override GameCondition CreateGameCondition()
        {
            // Start Behaviour doe not have and doe snot need a game condtion
            // So this is why it not created here and is null
            return null;
        }
    }
}