using Game_Manager;
using Game_Manager.Events;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

[System.Serializable]
public class UIButtonData
{
    [HideInInspector] public string ButtonName;
    public ButtonType ButtonType;
    public Button Button;
}

[System.Serializable]
public class MenuRelation
{
    public string MenuName;
    public List<UIButtonData> UIButtonDataList;
}

[AddComponentMenu("Game Manager System/UI/UIButtonLogic")]
public class UIButtonLogic : MonoBehaviour
{
    [SerializeField] List<MenuRelation> MenuRelations;

    private void Start()
    {
        foreach (var menuRelation in MenuRelations)
        {
            foreach (var buttonData in menuRelation.UIButtonDataList)
            {
                List<Action> commandsForListner = new List<Action>();
                CreateButtonCommand(buttonData.ButtonType, commandsForListner);
                buttonData.Button.onClick.AddListener(() => GameCommandExecutorForButton(commandsForListner));
            }
        }
    }

    private void OnValidate()
    {
        foreach (var menuRelation in MenuRelations)
        {
            foreach (var buttonData in menuRelation.UIButtonDataList)
            {
                buttonData.ButtonName = buttonData.ButtonType + " Button";
            }
        }
    }

    void CreateButtonCommand(ButtonType buttonType, List<Action> commandList)
    {
        switch (buttonType)
        {
            case ButtonType.StartGame:
                commandList.Add(() => GameManagerEventBus.Raise(GameRequestEvent.RequestPlayGame));
                break;
            case ButtonType.PauseGame:
                commandList.Add(() => GameManagerEventBus.Raise(GameRequestEvent.RequestPauseGame));
                break;
            case ButtonType.ResumeGame:
                commandList.Add(() => GameManagerEventBus.Raise(GameRequestEvent.RequestUnPauseGame));
                break;
            case ButtonType.RestartGame:
                commandList.Add(() => GameManagerEventBus.Raise(GameRequestEvent.RequestRestartGame));
                break;
            case ButtonType.GoToMainMenu:
                commandList.Add(() => GameManagerEventBus.Raise(GameRequestEvent.RequestStartGame));
                break;
            case ButtonType.QuitGame:
                commandList.Add(() => GameManagerEventBus.Raise(GameRequestEvent.RequestQuitGame));
                break;
        }
    }

    void GameCommandExecutorForButton(List<Action> commands)
    {
        for (int i = 0; i < commands.Count; i++)
        {
            commands[i]?.Invoke();
        }
    }
}