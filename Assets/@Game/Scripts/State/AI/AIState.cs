using UnityEngine;

[RequireComponent (typeof(AIStateHub))]
public class AIState : State
{
    protected AIStateHub AIHub => Hub as AIStateHub;

    public override void OnEnter()
    {
        base.OnEnter();
    }

    protected virtual bool CanState()
    {
        if (!AIHub.Character.IsInitialized) return false;
        if (!IsRunning) return false;
        return true;
    }
}
