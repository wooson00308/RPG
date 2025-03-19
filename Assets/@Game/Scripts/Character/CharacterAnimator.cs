using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    [SerializeField]
    private Character _owner;
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private SpriteRenderer _spriteRenderer; // SpriteRenderer 추가

    private bool _isPlayingState;

    private string _curState;
    public string State => _curState;
    public bool IsPlaying => _isPlayingState;

    [Tooltip("Y 좌표에 곱하여 Sorting Order를 결정할 값")]
    [SerializeField] private int _sortingOrderFactor = -10;  // 기본값 -10, 인스펙터에서 조정

    void Start()
    {
        //_spriteRenderer가 할당되지 않았다면, 이 컴포넌트가 붙어있는 GameObject에서 가져옴
        if (_spriteRenderer == null)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            if (_spriteRenderer == null)
            {
                Debug.LogError("SpriteRenderer를 찾을 수 없습니다.  SpriteRenderer 컴포넌트를 추가하거나, 인스펙터에서 할당해주세요.");
            }
        }
        if (_owner == null)
        {
            _owner = GetComponentInParent<Character>(); // 또는 transform.parent.GetComponent<Character>();
            if (_owner == null)
            {
                Debug.LogError("Character owner를 찾을 수 없습니다.  Character 컴포넌트를 추가하거나, 인스펙터에서 할당해주세요.");
            }
        }
    }

    public void UpdateMoveSpeed(float speed)
    {
        speed *= .2f * GameTime.TimeScale;
        _animator.SetFloat("MoveSpeed", speed);
    }

    public void UpdateAttackSpeed(float speed)
    {
        speed *= GameTime.TimeScale;
        _animator.SetFloat("AttackSpeed", speed);
    }

    public void CrossFade(string state, float fadeTime)
    {
        _curState = state;
        _animator.CrossFade(state, fadeTime);

        _isPlayingState = true;
    }

    public void OnBasicAttack()
    {
        if (_owner.target != null && _owner.target.IsInitialized) // 추가: target이 null이 아니고, 초기화 되었는지 확인
        {
            _owner.Stats.DealDamage(_owner.target.Stats, _owner.Stats.AttackDamage, 0f, true);
        }
        else
        {
            Debug.LogWarning("공격 대상이 없거나, 초기화되지 않았습니다!");
        }
    }

    public void OnPlaySound(AnimationEvent e)
    {
        string id = e.stringParameter;
        SoundManager.Instance.PlaySFX(id);
    }

    public void OnStateEnd()
    {
        _isPlayingState = false;
    }


    void Update()
    {
        if (_spriteRenderer != null && _owner != null)  // Null 체크
        {
            // Y 좌표에 factor를 곱하여 Sorting Order를 계산
            _spriteRenderer.sortingOrder = Mathf.RoundToInt(_owner.transform.position.y * _sortingOrderFactor);
        }
    }
}