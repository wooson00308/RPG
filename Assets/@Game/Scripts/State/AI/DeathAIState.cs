using UnityEngine;

public class DeathAIState : AIState
{
    public override void OnEnter()
    {
        base.OnEnter();

        AIHub.Character.Model.CrossFade("Death", 0f);
    }

    public override void OnUpdate()
    {
        if (!CanState()) return;

        if(!AIHub.Character.Model.IsPlaying)
        {
            AIHub.Character.Factory.ReturnToPool(AIHub.Character);
        }
    }
}
