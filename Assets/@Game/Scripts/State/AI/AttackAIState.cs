using UnityEngine;

public class AttackAIState : AIState
{
    public override void OnEnter()
    {
        base.OnEnter();
        Hub.Character.Model.CrossFade("Attack", 0f);
    }

    public override void OnUpdate()
    {
        if (!CanState()) return;

        base.OnUpdate();
    }
}
