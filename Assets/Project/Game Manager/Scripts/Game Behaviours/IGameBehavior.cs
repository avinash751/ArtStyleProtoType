using UnityEngine;

public interface IGameBehavior
{
    public void Enter(bool skipSceneLoading = false);

    public virtual void OnUpdate()
    {

    }

    public virtual void Exit()
    {

    }
}