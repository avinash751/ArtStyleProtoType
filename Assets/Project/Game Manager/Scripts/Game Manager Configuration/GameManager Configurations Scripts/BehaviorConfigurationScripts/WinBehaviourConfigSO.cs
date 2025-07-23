using Game_Manager;
using Game_Manager.Conditions;
using UnityEngine;

namespace Game_Manager.Configuration
{
    [CreateAssetMenu(fileName = "WinBehaviourConfigSO", menuName = "Game Manager/Behavior Configs/Win Behavior Config", order = 5)]
    public class WinBehaviourConfigSO : BaseGameBehaviorConfigSO
    {

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