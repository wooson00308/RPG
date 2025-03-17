using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CharacterStats : MonoBehaviour
{
    private CharacterData _data;
    private int _currentLevel = 1;

    private List<StatModifier> _permanentModifiers = new List<StatModifier>();
    private SortedList<float, StatModifier> _timedModifiers = new SortedList<float, StatModifier>(Comparer<float>.Create((a, b) => a.CompareTo(b)));
    private Dictionary<StatModifier.StatType, float> _statCache = new Dictionary<StatModifier.StatType, float>();
    private bool _statsDirty = true;

    // 상태 이상 목록
    private List<StatusEffect> _statusEffects = new List<StatusEffect>();

    public float MoveSpeed => GetCachedStat(StatModifier.StatType.MoveSpeed) * (1f - GetSlowIntensity());
    public float AttackRange => GetCachedStat(StatModifier.StatType.AttackRange);
    public float MaxHealth => GetCachedStat(StatModifier.StatType.MaxHealth);
    public float CurrentHealth { get; private set; }
    public float HealthRegen => GetCachedStat(StatModifier.StatType.HealthRegen);
    public float Armor => GetCachedStat(StatModifier.StatType.Armor);
    public float MagicResist => GetCachedStat(StatModifier.StatType.MagicResist);
    public float Tenacity => GetCachedStat(StatModifier.StatType.Tenacity);
    public float AttackDamage => GetCachedStat(StatModifier.StatType.AttackDamage);
    public float AttackSpeed => GetCachedStat(StatModifier.StatType.AttackSpeed);
    public float AbilityPower => GetCachedStat(StatModifier.StatType.AbilityPower);
    public float CritChance => GetCachedStat(StatModifier.StatType.CritChance);
    public float CritDamage => GetCachedStat(StatModifier.StatType.CritDamage);
    public float ArmorPenetration => GetCachedStat(StatModifier.StatType.ArmorPenetration);
    public float MagicPenetration => GetCachedStat(StatModifier.StatType.MagicPenetration);
    public float LifeSteal => GetCachedStat(StatModifier.StatType.LifeSteal);
    public float PhysicalVamp => GetCachedStat(StatModifier.StatType.PhysicalVamp);
    public float MaxMana => GetCachedStat(StatModifier.StatType.MaxMana);
    public float CurrentMana { get; private set; }
    public float ManaRegen => GetCachedStat(StatModifier.StatType.ManaRegen);
    public float AbilityHaste => GetCachedStat(StatModifier.StatType.AbilityHaste);

    // 상태 이상 확인 메서드
    public bool IsStunned => _statusEffects.Any(e => e.Type == StatusEffect.EffectType.Stun && e.GetCurrentIntensity() > 0);
    public bool IsSilenced => _statusEffects.Any(e => e.Type == StatusEffect.EffectType.Silence && e.GetCurrentIntensity() > 0);
    public bool IsAirborne => _statusEffects.Any(e => e.Type == StatusEffect.EffectType.Airborne && e.GetCurrentIntensity() > 0);
    public bool IsRooted => _statusEffects.Any(e => e.Type == StatusEffect.EffectType.Root && e.GetCurrentIntensity() > 0);
    public float GetSlowIntensity() => Mathf.Clamp01(_statusEffects.Where(e => e.Type == StatusEffect.EffectType.Slow)
                                                                .Select(e => e.GetCurrentIntensity())
                                                                .DefaultIfEmpty(0f)
                                                                .Max());

    public void Initialized(CharacterData data)
    {
        _data = data;
        _statsDirty = true;
        UpdateAllStats();
        CurrentHealth = MaxHealth;
        CurrentMana = MaxMana;
    }

    public void LevelUp()
    {
        _currentLevel++;
        _statsDirty = true;
    }

    public void AddStatModifier(StatModifier modifier)
    {
        if (modifier.duration > 0)
        {
            float expirationTime = Time.time + modifier.duration;
            while (_timedModifiers.ContainsKey(expirationTime))
            {
                expirationTime += 0.0001f;
            }
            _timedModifiers.Add(expirationTime, modifier);
        }
        else
        {
            _permanentModifiers.Add(modifier);
        }
        _statsDirty = true;
        UpdateHealthAndManaOnMaxChange();
    }

    public void RemoveStatModifier(string source)
    {
        _permanentModifiers.RemoveAll(mod => mod.source == source);
        _timedModifiers = new SortedList<float, StatModifier>(
            _timedModifiers.Where(kvp => kvp.Value.source != source)
                           .ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
            Comparer<float>.Create((a, b) => a.CompareTo(b)));
        _statsDirty = true;
        UpdateHealthAndManaOnMaxChange();
    }

    public void AddStatusEffect(StatusEffect effect)
    {
        // 강인함을 적용하여 실제 지속 시간 계산
        StatusEffect adjustedEffect = new StatusEffect(effect.Type, effect.BaseDuration, effect.Intensity, effect.Source, Tenacity);
        _statusEffects.Add(adjustedEffect);
        Debug.Log($"상태 이상 추가: {adjustedEffect.Type}, 기본 지속 시간: {adjustedEffect.BaseDuration}, 실제 지속 시간: {adjustedEffect.Duration}, 강도: {adjustedEffect.Intensity}");
    }

    // 상태 이상 제거 (출처 기준)
    public void RemoveStatusEffect(string source)
    {
        _statusEffects.RemoveAll(e => e.Source == source);
        Debug.Log($"상태 이상 제거: 출처 {source}");
    }

    private float GetCachedStat(StatModifier.StatType statType)
    {
        if (_statsDirty)
        {
            UpdateAllStats();
            _statsDirty = false;
        }
        return _statCache.ContainsKey(statType) ? _statCache[statType] : 0f;
    }

    private void UpdateAllStats()
    {
        _statCache[StatModifier.StatType.MoveSpeed] = ApplyModifiers(_data.moveSpeed, StatModifier.StatType.MoveSpeed);
        _statCache[StatModifier.StatType.AttackRange] = ApplyModifiers(_data.attackRange, StatModifier.StatType.AttackRange);
        _statCache[StatModifier.StatType.MaxHealth] = ApplyModifiers(Util.CalculateStatGrowth(_data.baseHealth, _data.healthPerLevel, _currentLevel), StatModifier.StatType.MaxHealth);
        _statCache[StatModifier.StatType.HealthRegen] = ApplyModifiers(Util.CalculateStatGrowth(_data.baseHealthRegen, _data.healthRegenPerLevel, _currentLevel), StatModifier.StatType.HealthRegen);
        _statCache[StatModifier.StatType.Armor] = ApplyModifiers(Util.CalculateStatGrowth(_data.baseArmor, _data.armorPerLevel, _currentLevel), StatModifier.StatType.Armor);
        _statCache[StatModifier.StatType.MagicResist] = ApplyModifiers(Util.CalculateStatGrowth(_data.baseMagicResist, _data.magicResistPerLevel, _currentLevel), StatModifier.StatType.MagicResist);
        _statCache[StatModifier.StatType.Tenacity] = ApplyModifiers(_data.tenacity, StatModifier.StatType.Tenacity);
        _statCache[StatModifier.StatType.AttackDamage] = ApplyModifiers(Util.CalculateStatGrowth(_data.baseAttackDamage, _data.attackDamagePerLevel, _currentLevel), StatModifier.StatType.AttackDamage);
        _statCache[StatModifier.StatType.AttackSpeed] = ApplyModifiers(Util.CalculateStatGrowth(_data.baseAttackSpeed, _data.attackSpeedPerLevel, _currentLevel), StatModifier.StatType.AttackSpeed);
        _statCache[StatModifier.StatType.AbilityPower] = ApplyModifiers(Util.CalculateStatGrowth(_data.baseAbilityPower, _data.abilityPowerPerLevel, _currentLevel), StatModifier.StatType.AbilityPower);
        _statCache[StatModifier.StatType.CritChance] = ApplyModifiers(_data.critChance, StatModifier.StatType.CritChance);
        _statCache[StatModifier.StatType.CritDamage] = ApplyModifiers(_data.critDamage, StatModifier.StatType.CritDamage);
        _statCache[StatModifier.StatType.ArmorPenetration] = ApplyModifiers(_data.armorPenetration, StatModifier.StatType.ArmorPenetration);
        _statCache[StatModifier.StatType.MagicPenetration] = ApplyModifiers(_data.magicPenetration, StatModifier.StatType.MagicPenetration);
        _statCache[StatModifier.StatType.LifeSteal] = ApplyModifiers(_data.lifeSteal, StatModifier.StatType.LifeSteal);
        _statCache[StatModifier.StatType.PhysicalVamp] = ApplyModifiers(_data.physicalVamp, StatModifier.StatType.PhysicalVamp);
        _statCache[StatModifier.StatType.MaxMana] = ApplyModifiers(Util.CalculateStatGrowth(_data.baseMana, _data.manaPerLevel, _currentLevel), StatModifier.StatType.MaxMana);
        _statCache[StatModifier.StatType.ManaRegen] = ApplyModifiers(Util.CalculateStatGrowth(_data.baseManaRegen, _data.manaRegenPerLevel, _currentLevel), StatModifier.StatType.ManaRegen);
        _statCache[StatModifier.StatType.AbilityHaste] = ApplyModifiers(_data.abilityHaste, StatModifier.StatType.AbilityHaste);
    }

    private float ApplyModifiers(float baseValue, StatModifier.StatType statType)
    {
        float flatBonus = 0f;
        float percentBonus = 0f;

        foreach (var modifier in _permanentModifiers.Where(m => m.statType == statType))
        {
            if (modifier.modifierType == StatModifier.ModifierType.Flat)
            {
                flatBonus += modifier.value;
            }
            else if (modifier.modifierType == StatModifier.ModifierType.Percent)
            {
                percentBonus += modifier.value;
            }
        }

        foreach (var modifier in _timedModifiers.Values.Where(m => m.statType == statType))
        {
            if (modifier.modifierType == StatModifier.ModifierType.Flat)
            {
                flatBonus += modifier.value;
            }
            else if (modifier.modifierType == StatModifier.ModifierType.Percent)
            {
                percentBonus += modifier.value;
            }
        }

        return Util.ApplyStatModifiers(baseValue, flatBonus, percentBonus);
    }

    private void UpdateHealthAndManaOnMaxChange()
    {
        float healthRatio = CurrentHealth / MaxHealth;
        float manaRatio = CurrentMana / MaxMana;
        CurrentHealth = MaxHealth * healthRatio;
        CurrentMana = MaxMana * manaRatio;
    }

    public float TakeDamage(float physicalDamage, float magicDamage, CharacterStats attacker = null)
    {
        float totalDamage = 0f;

        if (physicalDamage > 0)
        {
            float effectiveArmor = Armor - (attacker != null ? attacker.ArmorPenetration : 0);
            float physicalDamageTaken = Util.CalculateDamage(physicalDamage, effectiveArmor, true);
            totalDamage += physicalDamageTaken;
        }

        if (magicDamage > 0)
        {
            float effectiveMagicResist = MagicResist - (attacker != null ? attacker.MagicPenetration : 0);
            float magicDamageTaken = Util.CalculateDamage(magicDamage, effectiveMagicResist, false);
            totalDamage += magicDamageTaken;
        }

        CurrentHealth = Mathf.Max(0, CurrentHealth - totalDamage);
        return totalDamage;
    }

    public float DealDamage(CharacterStats target, float basePhysicalDamage, float baseMagicDamage, bool isBasicAttack = false)
    {
        // 스턴, 공중, 루팅 상태에서는 공격 불가
        if (IsStunned || IsAirborne || IsRooted)
        {
            Debug.Log("상태 이상(스턴/공중/루팅)으로 공격 불가");
            return 0f;
        }

        // 침묵 상태에서는 스킬 공격 불가
        if (IsSilenced && !isBasicAttack)
        {
            Debug.Log("침묵 상태로 스킬 공격 불가");
            return 0f;
        }

        float totalDamage = 0f;

        if (basePhysicalDamage > 0)
        {
            float physicalDamage = basePhysicalDamage;
            if (isBasicAttack && Random.value < CritChance)
            {
                physicalDamage *= CritDamage;
            }
            totalDamage += physicalDamage;
        }

        if (baseMagicDamage > 0)
        {
            totalDamage += baseMagicDamage;
        }

        float damageDealt = target.TakeDamage(basePhysicalDamage, baseMagicDamage, this);
        ApplyVamp(damageDealt, isBasicAttack);

        return damageDealt;
    }

    private void ApplyVamp(float damageDealt, bool isBasicAttack)
    {
        float healAmount = 0f;

        if (isBasicAttack)
        {
            healAmount += damageDealt * LifeSteal;
        }
        healAmount += damageDealt * PhysicalVamp * 0.33f;

        CurrentHealth = Mathf.Min(MaxHealth, CurrentHealth + healAmount);
    }

    private void Update()
    {
        // 스탯 변동 만료 관리
        if (_timedModifiers.Count > 0)
        {
            float currentTime = Time.time;
            while (_timedModifiers.Count > 0 && _timedModifiers.Keys[0] <= currentTime)
            {
                _timedModifiers.RemoveAt(0);
                _statsDirty = true;
            }
        }

        // 상태 이상 만료 관리
        if (_statusEffects.Count > 0)
        {
            _statusEffects.RemoveAll(effect =>
            {
                bool expired = effect.Update(Time.deltaTime);
                if (expired) Debug.Log($"상태 이상 만료: {effect.Type}");
                return expired;
            });
        }

        if (_statsDirty)
        {
            UpdateAllStats();
            _statsDirty = false;
        }

        if (CurrentHealth < MaxHealth && HealthRegen > 0)
        {
            CurrentHealth = Mathf.Min(MaxHealth, CurrentHealth + (HealthRegen / 5f) * Time.deltaTime);
        }

        if (CurrentMana < MaxMana && ManaRegen > 0)
        {
            CurrentMana = Mathf.Min(MaxMana, CurrentMana + (ManaRegen / 5f) * Time.deltaTime);
        }
    }

    public void DebugStats()
    {
        Debug.Log($"Level: {_currentLevel}");
        Debug.Log($"MoveSpeed: {MoveSpeed}");
        Debug.Log($"MaxHealth: {MaxHealth}, CurrentHealth: {CurrentHealth}");
        Debug.Log($"HealthRegen: {HealthRegen}");
        Debug.Log($"Armor: {Armor}, MagicResist: {MagicResist}");
        Debug.Log($"AttackDamage: {AttackDamage}, AttackSpeed: {AttackSpeed}");
        Debug.Log($"CritChance: {CritChance}, CritDamage: {CritDamage}");
        Debug.Log($"LifeSteal: {LifeSteal}, PhysicalVamp: {PhysicalVamp}");
        Debug.Log($"MaxMana: {MaxMana}, CurrentMana: {CurrentMana}");
        Debug.Log($"ManaRegen: {ManaRegen}");
        Debug.Log($"AbilityHaste: {AbilityHaste}");
        Debug.Log($"Status Effects: {string.Join(", ", _statusEffects.Select(e => $"{e.Type} ({e.GetCurrentIntensity()})"))}");
    }
}