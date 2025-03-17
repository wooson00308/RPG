public class ChaseAIState : AIState
{
    public override void OnEnter()
    {
        base.OnEnter();
        Hub.Character.Model.CrossFade("Chase", 0f);
        Hub.Character.Move(Hub.TargetDirection);
    }

    public override void OnUpdate()
    {
        if (!CanState()) return;

        if (Hub.Character.Stats.IsRooted) 
        {
            Hub.NextState<IdleAIState>();
            return;
        }

        if (Hub.TargetDistance <= Hub.config.attackDistance)
        {
            Hub.NextState<AttackAIState>();
            return;
        }
    }

    protected override bool CanState()
    {
        
        return base.CanState();
    }
}
