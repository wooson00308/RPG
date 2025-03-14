using UnityEditor.Animations;
public class IdleAIState : AIState
{
    public override void OnEnter()
    {
        base.OnEnter();
        Hub.Character.model?.CrossFade("Idle", 0f);
        Hub.Character.Stop();
    }

    public override void OnUpdate()
    {
        if (!CanState()) return;

        if (Hub.TargetDistance <= Hub.config.chaseDistance)
        {
            Hub.NextState<ChaseAIState>();
        }
    }
}
