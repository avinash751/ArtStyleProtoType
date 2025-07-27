using Game_Manager.Conditions;
using UnityEngine;
using Game_Manager.GameBehaviors;

namespace Game_Manager.Configuration
{
    [CreateAssetMenu(fileName = "PauseBehaviorConfig", menuName = "Game Manager/Behavior Configs/Pause Behavior Config", order = 3)]
    public class PauseBehaviorConfigSO :BaseGameBehaviorConfigSO
    {
        [Header("Pause Behavior Settings")]
        public KeyCode PauseKey = KeyCode.Escape;
        public KeyCode ControllerPauseKey;
        public string PauseAudioKey;
        public string UnPauseAudioKey;

        public override string BehaviorName => "Pause Behavior";

        public override GameBehaviorBase CreateBehavior()
        {
            PauseBehavior pauseBehavior = new PauseBehavior(this);
            return pauseBehavior;
        }

        public override GameCondition CreateGameCondition()
        {
            PauseCondition pauseCondition = new PauseCondition(this);
            return pauseCondition;
        }
    }
}