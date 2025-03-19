using UnityEngine;

public class AttackAIState : AIState
{
    public override void OnEnter()
    {
        base.OnEnter();
        AIHub.Character.Rotate(AIHub.TargetDirection);
        AIHub.Character.Model.CrossFade("Attack", 0f);
        AIHub.Character.Model.UpdateAttackSpeed(AIHub.Character.Stats.AttackSpeed);
    }

    public override void OnUpdate()
    {
        if (!CanState()) return;

        if(!AIHub.Character.Model.IsPlaying)
        {
            AIHub.NextState<IdleAIState>();
        }
    }
}
