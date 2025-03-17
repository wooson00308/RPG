using UnityEngine;

public class StatModifier
{
    public enum StatType
    {
        MoveSpeed, AttackRange, MaxHealth, HealthRegen, Armor, MagicResist, Tenacity,
        AttackDamage, AttackSpeed, AbilityPower, CritChance, CritDamage, ArmorPenetration, MagicPenetration,
        LifeSteal, PhysicalVamp, OmniVamp, MaxMana, ManaRegen, AbilityHaste
    }

    public enum ModifierType
    {
        Flat, // 고정값 (예: +50 공격력)
        Percent // 비율 (예: +30% 이동 속도)
    }

    public StatType statType; // 수정할 스탯 타입
    public ModifierType modifierType; // 수정 방식 (고정값 또는 비율)
    public float value; // 수정 값
    public float duration; // 지속 시간 (0이면 영구적)
    public string source; // 출처 (디버깅 및 추적용, 예: "Bloodthirster", "SlowDebuff")

    private float _timeRemaining; // 남은 시간 (내부 관리용)

    public StatModifier(StatType statType, ModifierType modifierType, float value, float duration = 0, string source = "")
    {
        this.statType = statType;
        this.modifierType = modifierType;
        this.value = value;
        this.duration = duration;
        this.source = source;
        _timeRemaining = duration;
    }

    // 남은 시간 업데이트 (버프/디버프 만료 관리용)
    public bool Update(float deltaTime)
    {
        if (duration > 0)
        {
            _timeRemaining -= deltaTime;
            return _timeRemaining <= 0; // true면 만료됨
        }
        return false; // 영구적인 경우 만료되지 않음
    }
}