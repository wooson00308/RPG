public class ChaseAIState : AIState
{
    public override void OnEnter()
    {
        base.OnEnter();
        AIHub.Character.Model.CrossFade("Chase", 0f);
    }

    public override void OnUpdate()
    {
        if (!CanState()) return;

        if (AIHub.Character.Stats.IsRooted || AIHub.Character.target == null) 
        {
            Hub.NextState<IdleAIState>();
            return;
        }

        if (AIHub.TargetDistance <= AIHub.Character.Stats.AttackRange)
        {
            AIHub.NextState<AttackAIState>();
            return;
        }

        AIHub.Character.Move(AIHub.TargetDirection);
    }

    protected override bool CanState()
    {
        
        return base.CanState();
    }
}
