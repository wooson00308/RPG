using UnityEngine;

public class StunAIState : AIState
{
    public override void OnEnter()
    {
        base.OnEnter();
        AIHub.Character.Model?.CrossFade("Stun", 0f);
        AIHub.Character.Stop();
    }

    public override void OnUpdate()
    {
        if (!CanState()) return;

        AIHub.NextState<IdleAIState>();
    }

    protected override bool CanState()
    {
        if (AIHub.Character.Stats.IsStunned) return false;
        return base.CanState();
    }
}
