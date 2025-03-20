using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CharacterStats))]
public class Character : MonoBehaviour
{
    private CharacterStats _stats;
    private CharacterFactory _factory;
    private CharacterData _data;
    private CharacterAnimator _model;
    private AIStateHub _hub;

    private NavMeshAgent _agent;

    private Team _team;
    private bool _isInitialized;

    public CharacterData Data => _data;
    public CharacterStats Stats => _stats;
    public CharacterFactory Factory => _factory;

    public CharacterAnimator Model => _model;

    public Team Team => _team;
    public bool IsInitialized => _isInitialized;

    public Character target;

    private float _originalSpeed;

    private bool _isDeath;
    public bool IsDeath => _isDeath;

    private void Awake()
    {
        _stats = GetComponent<CharacterStats>();
        _agent = GetComponent<NavMeshAgent>();
        _hub = GetComponent<AIStateHub>();
        _model = GetComponentInChildren<CharacterAnimator>();

        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
    }

    public void Initialized(CharacterFactory factory, CharacterData data, Team team)
    {
        _factory = factory;
        _data = data;
        _team = team;

        _stats.Initialized(this, _data);

        _agent.enabled = true;

        _agent.speed = _stats.MoveSpeed / 100f;
        _originalSpeed = _agent.speed; // 초기 속도 저장

        _hub.NextState<IdleAIState>();

        _isDeath = false;
        _isInitialized = true;
    }

    public void Move(Vector2 moveVector)
    {
        _agent.isStopped = false;
        _agent.speed = _stats.MoveSpeed / 100f;

        Vector3 destination = transform.position + (Vector3)moveVector * _agent.speed * GameTime.DeltaTime;
        _agent.SetDestination(destination);

        _model.UpdateMoveSpeed(_agent.speed);

        Rotate(moveVector);
    }

    public void Rotate(Vector2 rotVector)
    {
        if (rotVector.x != 0)
        {
            float rotY = rotVector.x > 0 ? 0 : 180;
            _model.transform.rotation = Quaternion.Euler(0, rotY, 0);
        }
    }

    public void Stop()
    {
        _agent.isStopped = true;
        _agent.velocity = Vector3.zero;
        _agent.ResetPath();
    }

    /// <summary>
    /// 캐릭터를 지정된 위치로 즉시 이동 (점멸).
    /// </summary>
    /// <param name="destination">목표 위치</param>
    /// <param name="maxDistance">최대 이동 거리 (제한 없으면 float.MaxValue)</param>
    public void Blink(Vector3 destination, float maxDistance = float.MaxValue)
    {
        Vector3 direction = (destination - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, destination);
        if (distance > maxDistance)
        {
            destination = transform.position + direction * maxDistance; // 최대 거리 제한
        }

        NavMeshHit hit;
        if (NavMesh.SamplePosition(destination, out hit, 1.0f, NavMesh.AllAreas))
        {
            _agent.Warp(hit.position);
            Rotate(new Vector2(direction.x, direction.z));
            Debug.Log($"Blink to: {hit.position}");
        }
        else
        {
            Debug.LogWarning("Blink destination is outside NavMesh!");
        }
    }

    /// <summary>
    /// 지정된 방향으로 빠르게 이동 (이동기). 속박 시 중단.
    /// </summary>
    /// <param name="direction">이동 방향</param>
    /// <param name="distance">이동 거리</param>
    /// <param name="speed">이동 속도 (기본값은 MoveSpeed * 2)</param>
    public void Dash(Vector2 direction, float distance, float speed = -1f)
    {
        Stop();
        StartCoroutine(DashCoroutine(direction, distance, speed));
    }

    private IEnumerator DashCoroutine(Vector2 direction, float distance, float speed)
    {
        Vector3 moveDirection = new Vector3(direction.x, 0, direction.y).normalized;
        float dashSpeed = speed > 0 ? speed / 100f : _stats.MoveSpeed * 2f / 100f;
        float duration = distance / dashSpeed;
        float elapsed = 0f;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + moveDirection * distance;

        Rotate(direction);
        Debug.Log($"Dash 시작: Distance {distance}, Speed {dashSpeed * 100f}, Duration {duration}");

        while (elapsed < duration)
        {
            if (_stats.IsRooted || _stats.IsSilenced)
            {
                Debug.Log("Dash 중 cc기에 걸려 중단");
                _hub.NextState<IdleAIState>();
                yield break;
            }

            if(_stats.IsStunned)
            {
                Debug.Log("Dash 중 스턴에 걸려 중단");
                _hub.NextState<StunAIState>();
                yield break;
            }

            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            Vector3 newPosition = Vector3.Lerp(startPosition, targetPosition, t);
            Vector3 offset = newPosition - transform.position;
            _agent.Move(offset);

            yield return null;
        }

        // Dash 완료 후 속도 복구
        _agent.speed = _originalSpeed;
        Debug.Log($"Dash 완료: 위치 {transform.position}");
    }

    public void OnHit()
    {
        if (_isDeath) return;

        _hub.NextState<HitAIState>();
    }

    public void OnDeath()
    {
        if (_isDeath) return;
        _isDeath = true;

        _agent.enabled = false;

        _hub.NextState<DeathAIState>();
    }

    private void OnDisable()
    {
        _isInitialized = false;
    }

    private void Update()
    {
        if (!_isInitialized) return;
        _agent.speed = _stats.MoveSpeed / 100f;

        target = _factory.FindNearestEnemy(this);
    }
}

public enum Team
{
    Friendly,
    Enemy,
}