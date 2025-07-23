using UnityEngine;
using Game_Manager.Configuration;
using Game_Manager.Events;

namespace Game_Manager.Conditions
{
    /// <summary>
    /// References cannot be dragged in inspector 
    /// if the Game Manager is marked as persistant
    /// If The Game Manager persists across scenes, then the references
    /// Will be lost and be null in the process
    /// Instead try getting the references 
    /// </summary>
    [System.Serializable]
    public class PauseCondition : GameCondition,IPollableCondition
    {
        [HideInInspector][SerializeField] PauseBehaviorConfigSO pauseConfig;
        [SerializeField][HideInInspector] bool isPaused = false;

        public PauseCondition(BaseGameBehaviorConfigSO _pauseConfig) : base(_pauseConfig)
        {
            conditionName = "Pause Condition";
            pauseConfig = (PauseBehaviorConfigSO)_pauseConfig;
            requestEventType = GameRequestEvent.RequestPauseGame;
        }

        public override void Initialize()
        {
            GameManagerEventBus.Subscribe(GameStateEvent.OnPaused, EnablePause);
            GameManagerEventBus.Subscribe(GameStateEvent.OnUnPaused, DisablePause);
        }

        public void OnUpdate()
        {
            bool onInputPressed = Input.GetKeyDown(pauseConfig.PauseKey) || Input.GetKeyDown(pauseConfig.ControllerPauseKey);

            if (onInputPressed)
            {
                TriggerGameConditionMet();
            }
        }

        protected override void HandleOnGameConditionMet()
        {
            isPaused = !isPaused;
            requestEventType = isPaused ? GameRequestEvent.RequestPauseGame : GameRequestEvent.RequestUnPauseGame;
        }

        void DisablePause() => isPaused = false;
        void EnablePause() => isPaused = true;

        public override void CleanUp()
        {
            GameManagerEventBus.Unsubscribe(GameStateEvent.OnPaused, EnablePause);
            GameManagerEventBus.Unsubscribe(GameStateEvent.OnUnPaused, DisablePause);
        }
    }
}
