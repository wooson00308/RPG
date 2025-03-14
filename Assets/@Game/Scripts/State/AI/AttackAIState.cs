using UnityEngine;

public class AttackAIState : AIState
{
    public override void OnEnter()
    {
        base.OnEnter();
        Hub.Character.model?.CrossFade("Attack", 0f);
    }

    public override void OnUpdate()
    {
        if (!CanState()) return;

        base.OnUpdate();
    }
}
