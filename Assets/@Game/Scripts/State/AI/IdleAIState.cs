public class IdleAIState : AIState
{
    public override void OnEnter()
    {
        base.OnEnter();
        AIHub.Character.Model.CrossFade("Idle", 0f);
        AIHub.Character.Stop();
    }

    public override void OnUpdate()
    {
        if (!CanState()) return;

        if (AIHub.Character.target == null) return;

        if (AIHub.TargetDistance <= AIHub.Character.Stats.AttackRange)
        {
            Hub.NextState<AttackAIState>();
            return;
        }

        if (!AIHub.Character.Stats.IsRooted && AIHub.TargetDistance <= AIHub.config.chaseDistance)
        {
            Hub.NextState<ChaseAIState>();
            return;
        }
    }
}
