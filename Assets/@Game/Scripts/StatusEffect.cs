using UnityEngine;

public class StatusEffect
{
    public enum EffectType
    {
        Stun, Silence, Airborne, Root, Slow
    }

    public EffectType Type { get; private set; }
    public float BaseDuration { get; private set; }  // 기본 지속 시간
    public float Duration { get; private set; }      // 강인함 적용된 실제 지속 시간
    public float Intensity { get; private set; }
    public string Source { get; private set; }
    private float _remainingTime;

    public StatusEffect(EffectType type, float baseDuration, float intensity = 1f, string source = "", float tenacity = 0f)
    {
        Type = type;
        BaseDuration = baseDuration;
        Duration = type == EffectType.Airborne ? baseDuration : baseDuration * (1f - tenacity); // 공중은 강인함 미적용
        Intensity = Mathf.Clamp01(intensity);
        Source = source;
        _remainingTime = Duration;
    }

    public bool Update(float deltaTime)
    {
        _remainingTime -= deltaTime;
        return _remainingTime <= 0;
    }

    public float GetCurrentIntensity()
    {
        return _remainingTime > 0 ? Intensity : 0f;
    }
}