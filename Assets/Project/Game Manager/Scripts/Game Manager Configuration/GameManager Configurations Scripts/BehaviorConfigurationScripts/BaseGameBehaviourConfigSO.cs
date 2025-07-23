using Game_Manager;
using Game_Manager.Conditions;
using UnityEngine;

namespace Game_Manager.Configuration
{
    public abstract class BaseGameBehaviorConfigSO : ScriptableObject
    {
        [Header("Base Behavior Configuration")]
        public GameBehaviorType BehaviorType;
        public bool IsTimeZeroOnExecution = true;
        public bool IsCursorLockedOnExecution = true;
        public bool IsCursorVisibleOnExecution = true;
        public bool ShowGameUIOnExecution = false;
        public SceneLoadType SceneLoadTypeOnExecution = SceneLoadType.NoSceneLoad;
        public int SceneToLoad = 0;

        public abstract GameBehaviorBase CreateBehavior();
        public abstract GameCondition CreateGameCondition();

    }
}
