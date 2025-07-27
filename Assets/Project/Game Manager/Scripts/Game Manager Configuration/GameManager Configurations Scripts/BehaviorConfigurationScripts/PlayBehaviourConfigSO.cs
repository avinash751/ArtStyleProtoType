using Game_Manager.Conditions;
using UnityEngine;
using Game_Manager.GameBehaviors;

namespace Game_Manager.Configuration
{
    [CreateAssetMenu(fileName = "PlayBehaviorConfig", menuName = "Game Manager/Behavior Configs/Play Behavior Config", order = 4)]
    public class PlayBehaviorConfigSO : BaseGameBehaviorConfigSO
    {
        public override string BehaviorName => "Play Behavior";
        public override GameBehaviorBase CreateBehavior()
        {
            PlayBehavior playBehavior = new PlayBehavior(this);
            return playBehavior;

        }

        public override GameCondition CreateGameCondition()
        {
            // Play behavior does not have and does not need a specific game condition, so we return null.
            return null;
        }
    }
}
