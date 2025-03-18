public class IdleAIState : AIState
{
    public override void OnEnter()
    {
        base.OnEnter();
        Hub.Character.Model.CrossFade("Idle", 0f);
        Hub.Character.Stop();
    }

    public override void OnUpdate()
    {
        if (!CanState()) return;

        if (Hub.TargetDistance <= Hub.Character.Stats.AttackRange)
        {
            Hub.NextState<AttackAIState>();
            return;
        }

        if (!Hub.Character.Stats.IsRooted && Hub.TargetDistance <= Hub.config.chaseDistance)
        {
            Hub.NextState<ChaseAIState>();
            return;
        }
    }
}
