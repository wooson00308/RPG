using UnityEngine;

[RequireComponent (typeof(Character))]
public class AIStateHub : StateHub
{
    private Character _character;

    private Vector2 _targetDirection;
    private float _targetDistance;

    public AIConfig config;
    public Character Character => _character;

    public Vector2 TargetDirection => _targetDirection;
    public float TargetDistance => _targetDistance;

    protected override void Awake()
    {
        _character = GetComponent<Character>();

        base.Awake();
    }

    protected override void Update()
    {
        if(_character.target != null)
        {
            _targetDistance = Vector2.Distance(_character.transform.position, _character.target.transform.position);
            _targetDirection = _character.transform.position - _character.target.transform.position;
        }

        base.Update();
    }
}
