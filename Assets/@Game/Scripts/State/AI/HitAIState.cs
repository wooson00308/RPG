using UnityEngine;

public class HitAIState : AIState
{
    private bool _isRunning;

    public override void OnEnter()
    {
        if (_isRunning) return;
        _isRunning = true;

        base.OnEnter();

        AIHub.Character.Model.CrossFade("Hit", 0f);
    }

    public override void OnUpdate()
    {
        if (!CanState()) return;

        if (!AIHub.Character.Model.IsPlaying)
        {
            AIHub.NextState<IdleAIState>();
        }
    }

    public override void OnExit()
    {
        base.OnExit();

        _isRunning = false;
    }
}
