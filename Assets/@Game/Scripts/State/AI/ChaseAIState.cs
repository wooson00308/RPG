public class ChaseAIState : AIState
{
    public override void OnEnter()
    {
        base.OnEnter();
        Hub.Character.model?.CrossFade("Chase", 0f);
    }

    public override void OnUpdate()
    {
        if (!CanState()) return;

        if (Hub.TargetDistance <= Hub.config.attackDistance)
        {
            Hub.NextState<AttackAIState>();
        }
        else
        {
            Hub.Character.Move(Hub.TargetDirection);
        }
    }
}
