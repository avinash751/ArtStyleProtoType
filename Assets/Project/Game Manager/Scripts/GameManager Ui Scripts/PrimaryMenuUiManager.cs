using Game_Manager.Events;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Game_Manager.UI
{
   
    [AddComponentMenu("Game Manager System/UI/PrimaryMenusUIManager")]
    public class PrimaryMenusUIManager : MonoBehaviour
    {
        [Header("Game UI Objects")]
        [SerializeField] private GameObject inGameUI;

        [Header("State-Based Menus")]
        [SerializeField] private List<MenuMapping> menuMappings;

        private Dictionary<GameStateEvent, Action> eventHandlers;
        private List<GameObject> allManagedMenus;

        private void Awake()
        {
            eventHandlers = new Dictionary<GameStateEvent, Action>();
            allManagedMenus = new List<GameObject>();

            // We loop through the mappings and create a unique handler for each one.
            foreach (var mapping in menuMappings)
            {
                if (mapping.MenuObject == null) continue;

                allManagedMenus.Add(mapping.MenuObject);

                // IMPORTANT: We capture the menu object for this specific iteration.
                GameObject menuToShow = mapping.MenuObject;

                // Create a parameter-less action (a lambda) that "closes over" the menuToShow variable.
                // This action is what we will subscribe to the event bus.
                Action handler = () =>
                {
                    HideAllGameMenus();
                    menuToShow.SetActive(true);
                };

                eventHandlers[mapping.TriggeringEvent] = handler;
            }
        }

        private void OnEnable()
        {
            foreach (var pair in eventHandlers)
            {
                GameManagerEventBus.Subscribe(pair.Key, pair.Value);
            }
            GameManagerEventBus.Subscribe(GameStateEvent.OnPlay, HideAllGameMenus);
            GameManagerEventBus.Subscribe(GameStateEvent.OnUnPaused, HideAllGameMenus);
            GameManagerEventBus.Subscribe(GameStateEvent.OnInGameUIActive, ShowInGameUI);
            GameManagerEventBus.Subscribe(GameStateEvent.OnInGameUIInactive, HideInGameUI);
        }

        private void OnDisable()
        {
            foreach (var pair in eventHandlers)
            {
                GameManagerEventBus.Unsubscribe(pair.Key, pair.Value);
            }

            GameManagerEventBus.Unsubscribe(GameStateEvent.OnPlay, HideAllGameMenus);
            GameManagerEventBus.Unsubscribe(GameStateEvent.OnUnPaused, HideAllGameMenus);
            GameManagerEventBus.Unsubscribe(GameStateEvent.OnInGameUIActive, ShowInGameUI);
            GameManagerEventBus.Unsubscribe(GameStateEvent.OnInGameUIInactive, HideInGameUI);
        }

        public void HideAllGameMenus()
        {
            foreach (var menu in allManagedMenus)
            {
                if (menu != null) menu.SetActive(false);
            }
        }

        public void ShowInGameUI() => SetUIVisibility(inGameUI, true);
        public void HideInGameUI() => SetUIVisibility(inGameUI, false);

        private void SetUIVisibility(GameObject uiElement, bool visible)
        {
            if (uiElement != null)
            {
                uiElement.SetActive(visible);
            }
        }
    }
    [Serializable]
    public class MenuMapping
    {
        public string MenuMapName;
        public GameStateEvent TriggeringEvent;
        public GameObject MenuObject;
    }
}