using Game_Manager;
using Game_Manager.Conditions;
using UnityEngine;
using Game_Manager.GameBehaviors;

namespace Game_Manager.Configuration
{
    [CreateAssetMenu(fileName = "LoseBehaviourConfigSO", menuName = "Game Manager/Behavior Configs/Lose Behavior Config", order = 6)]
    public class LoseBehaviorConfigSO : BaseGameBehaviorConfigSO
    {
        public override string BehaviorName => "Lose Behavior";

        public override GameBehaviorBase CreateBehavior()
        {
            LoseBehavior loseBehavior = new LoseBehavior(this);
            return loseBehavior;
        }

        public override GameCondition CreateGameCondition()
        {
            LoseCondition loseCondition = new LoseCondition(this);
            return loseCondition;
        }
    }
}