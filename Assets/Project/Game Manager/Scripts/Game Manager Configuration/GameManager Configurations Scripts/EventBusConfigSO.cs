
using System.Collections.Generic;
using UnityEngine;


namespace Game_Manager.Configuration
{
    ///<Summary>
    /// Events listed here will be marked 'persistent'
    /// New listeners subscribing to a  event will be immediately notified
    /// if that event is the current active state."
    ///</Summary>
    [CreateAssetMenu(fileName = "EventBusConfig", menuName = "Game Manager/Event Bus Config", order = 100)]
    public class EventBusConfigSO : ScriptableObject
    {
        public List<GameStateEvent> PersistentStateEvents;
    }
}
