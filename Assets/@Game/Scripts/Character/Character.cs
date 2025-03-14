using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CharacterStats))]
public class Character : MonoBehaviour
{
    public const int MOVE_SPEED_FACTOR = 50;

    private CharacterStats _stats;
    private CharacterFactory _factory;
    private CharacterData _data;

    private NavMeshAgent _agent;

    private Team _team;
    private bool _isInitialized;

    public CharacterData Data => _data;

    public Team Team => _team;
    public bool IsInitialized => _isInitialized;

    public Character target;
    public Animator model;

    private void Awake()
    {
        _stats = GetComponent<CharacterStats>();        
    }

    public void Initialized(CharacterFactory factory, CharacterData data, Team team)
    {
        _factory = factory;
        _data = data;
        _team = team;

        _stats.Initialized(_data);

        _isInitialized = true;
    }

    public void Move(Vector2 moveVector)
    {
        _agent.isStopped = false;
        _agent.velocity = GameTime.DeltaTime * (_stats.moveSpeed * MOVE_SPEED_FACTOR) * moveVector;

        Rotate(moveVector);
    }

    public void Rotate(Vector2 rotVector)
    {
        if (rotVector.x != 0)
        {
            float rotY = rotVector.x > 0 ? 0 : 180;
            model.transform.rotation = Quaternion.Euler(0, rotY, 0);
        }
    }

    public void Stop()
    {
        _agent.isStopped = true;
        _agent.velocity = Vector2.zero;
    }

    private void OnEnable()
    {
        _isInitialized = false;
    }
}

public enum Team
{
    Friendly,
    Enemy,
}