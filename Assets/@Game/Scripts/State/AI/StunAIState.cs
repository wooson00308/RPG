using UnityEngine;

public class StunAIState : AIState
{
    public override void OnEnter()
    {
        base.OnEnter();
        Hub.Character.Model?.CrossFade("Stun", 0f);
        Hub.Character.Stop();
    }

    public override void OnUpdate()
    {
        if (!CanState()) return;

        Hub.NextState<IdleAIState>();
    }

    protected override bool CanState()
    {
        if (Hub.Character.Stats.IsStunned) return false;
        return base.CanState();
    }
}
