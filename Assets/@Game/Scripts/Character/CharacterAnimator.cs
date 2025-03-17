using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    private Character _owner;
    private Animator _animator;

    private bool _isPlayingState;

    private string _curState;
    public string State => _curState;
    public bool IsPlaying => _isPlayingState;

    private void Awake()
    {
        _owner = GetComponentInParent<Character>();
        _animator = GetComponent<Animator>();
    }

    public void CrossFade(string state, float fadeTime)
    {
        _curState = state;
        _animator.CrossFade(state, fadeTime);

        _isPlayingState = true;
    }

    public void OnBasicAttack()
    {
        _owner.Stats.DealDamage(_owner.target.Stats, _owner.Stats.AttackDamage, 0f, true);
    }

    public void OnStateEnd()
    {
        _isPlayingState = false;
    }
 }
