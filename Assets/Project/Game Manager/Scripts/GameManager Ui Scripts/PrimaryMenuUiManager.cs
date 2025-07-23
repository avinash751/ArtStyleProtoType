using Game_Manager.Events;
using System;
using UnityEngine;

namespace Game_Manager.UI
{
    [AddComponentMenu("Game Manager System/UI/PrimaryMenusUIManager")]
    public class PrimaryMenusUIManager : MonoBehaviour
    {
        [Header("Game UI Objects")]
        [SerializeField] private GameObject inGameUI;
        [Header("Menu GameObjects")]
        [SerializeField] private GameObject startMenu;
        [SerializeField] private GameObject pauseMenu;
        [SerializeField] private GameObject winMenu;
        [SerializeField] private GameObject loseMenu;

        #region Subscriptions
        private void OnEnable()
        {
            GameManagerEventBus.Subscribe(GameStateEvent.OnStart, ShowStartMenu);
            GameManagerEventBus.Subscribe(GameStateEvent.OnPlay, HideAllGameMenus);
            GameManagerEventBus.Subscribe(GameStateEvent.OnPaused, ShowPauseMenu);
            GameManagerEventBus.Subscribe(GameStateEvent.OnUnPaused, HideAllGameMenus);
            GameManagerEventBus.Subscribe(GameStateEvent.OnWin, ShowWinMenu);
            GameManagerEventBus.Subscribe(GameStateEvent.OnLose, ShowLoseMenu);
            GameManagerEventBus.Subscribe(GameStateEvent.OnInGameUIActive, ShowInGameUI);
            GameManagerEventBus.Subscribe(GameStateEvent.OnInGameUIInactive, HideInGameUI);
        }

        private void OnDisable()
        {
            GameManagerEventBus.Unsubscribe(GameStateEvent.OnStart, ShowStartMenu);
            GameManagerEventBus.Unsubscribe(GameStateEvent.OnPlay, HideAllGameMenus);
            GameManagerEventBus.Unsubscribe(GameStateEvent.OnPaused, ShowPauseMenu);
            GameManagerEventBus.Unsubscribe(GameStateEvent.OnUnPaused, HideAllGameMenus);
            GameManagerEventBus.Unsubscribe(GameStateEvent.OnWin, ShowWinMenu);
            GameManagerEventBus.Unsubscribe(GameStateEvent.OnLose, ShowLoseMenu);
            GameManagerEventBus.Unsubscribe(GameStateEvent.OnInGameUIActive, ShowInGameUI);
            GameManagerEventBus.Unsubscribe(GameStateEvent.OnInGameUIInactive, HideInGameUI);
        }
        #endregion


        #region Menu & UI Visibility Control - LAMBDA EXPRESSIONS

        public void ShowStartMenu() => HideMenusAndSetUIVisibility(startMenu, true);
        public void HideStartMenu() => SetUIVisibility(startMenu, false);
        public void ShowPauseMenu() => HideMenusAndSetUIVisibility(pauseMenu, true);
        public void HidePauseMenu() => SetUIVisibility(pauseMenu, false);
        public void ShowWinMenu() => HideMenusAndSetUIVisibility(winMenu, true);
        public void HideWinMenu() => SetUIVisibility(winMenu, false);
        public void ShowLoseMenu() => HideMenusAndSetUIVisibility(loseMenu, true);
        public void HideLoseMenu() => SetUIVisibility(loseMenu, false);
        public void ShowInGameUI() => HideMenusAndSetUIVisibility(inGameUI, true);
        public void HideInGameUI() => SetUIVisibility(inGameUI, false);

        #endregion

        public void HideAllGameMenus()
        {
            HideStartMenu();
            HidePauseMenu();
            HideWinMenu();
            HideLoseMenu();
        }

        public void HideAllUI()
        {
            HideInGameUI();
            HideAllGameMenus();
        }

        private void HideMenusAndSetUIVisibility(GameObject menu, bool visible)
        {
            HideAllGameMenus();
            SetUIVisibility(menu, visible);
        }

        private void SetUIVisibility(GameObject menu, bool visible)
        {
            if (menu != null)
            {
                menu.SetActive(visible);
            }
        }
    }
}
