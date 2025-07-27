using Game_Manager;
using Game_Manager.Conditions;
using UnityEngine;

namespace Game_Manager.Configuration
{
    [CreateAssetMenu(fileName = "WinBehaviorConfigSO", menuName = "Game Manager/Behavior Configs/Win Behavior Config", order = 5)]
    public class WinBehaviorConfigSO : BaseGameBehaviorConfigSO
    {
        public override string BehaviorName => "Win Behavior";

        public override GameBehaviorBase CreateBehavior()
        {
            WinBehavior winBehavior = new WinBehavior(this);
            return winBehavior;

        }

        public override GameCondition CreateGameCondition()
        {
            WinCondition winCondition = new WinCondition(this);
            return winCondition;
        }
    }
}