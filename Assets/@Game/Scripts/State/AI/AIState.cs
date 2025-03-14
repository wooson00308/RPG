using UnityEngine;

[RequireComponent (typeof(AIStateHub))]
public class AIState : State
{
    protected new AIStateHub Hub => Hub as AIStateHub;

    public override void OnEnter()
    {
        base.OnEnter();
    }

    protected virtual bool CanState()
    {
        if (!Hub.Character.IsInitialized) return false;
        if (!IsRunning) return false;
        return true;
    }
}
