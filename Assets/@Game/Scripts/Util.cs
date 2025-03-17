using UnityEngine;

public static class Util
{
    const double EPSILON = 0.0001;

    /// <summary>
    /// LookAt 2D버전 바라보는 각도 반환
    /// </summary>
    public static Vector3 LookAt2D(Transform transform, Transform targetPos)
    {
        Vector3 dir = targetPos.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Vector3 lookDir = new Vector3(0, 0, angle);
        return lookDir;
    }

    public static Vector3 LookAt2D(Vector2 transform, Vector2 targetPos)
    {
        Vector3 dir = targetPos - transform;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Vector3 lookDir = new Vector3(0, 0, angle);
        return lookDir;
    }

    public static bool IsEqual(double x, double y) // 비교 함수.
    {
        return (((x - EPSILON) < y) && (y < (x + EPSILON)));
    }

    /// <summary>
    /// LoL 스타일 스탯 성장 공식: 총 스탯 = 기본값 + ((레벨-1) * 성장값 * (1 + 0.0275 * (레벨-2)))
    /// </summary>
    public static float CalculateStatGrowth(float baseStat, float growthPerLevel, int level)
    {
        if (level <= 1) return baseStat;
        return baseStat + ((level - 1) * growthPerLevel * (1f + 0.0275f * (level - 2)));
    }

    /// <summary>
    /// 피해 계산 공식: 피해량 = 원래 피해량 * (1 - (방어 또는 마법 저항 / (방어 또는 마법 저항 + 300)))
    /// </summary>
    public static float CalculateDamage(float damage, float resistance, bool isPhysical)
    {
        float effectiveResistance = Mathf.Max(0, resistance); // 음수 방지
        float damageReduction = effectiveResistance / (effectiveResistance + 300f);
        return damage * (1f - damageReduction);
    }

    /// <summary>
    /// 주문 가속에 따른 재사용 대기시간 감소율 계산: 감소율 = 주문 가속 / (1 + 0.008 * 주문 가속)
    /// </summary>
    public static float CalculateCooldownReduction(float abilityHaste)
    {
        return abilityHaste / (1f + 0.008f * abilityHaste);
    }

    /// <summary>
    /// 강인함(Tenacity)에 따른 군중 제어 지속 시간 감소 계산
    /// </summary>
    public static float CalculateTenacityEffect(float originalDuration, float tenacity)
    {
        return originalDuration * (1f - Mathf.Clamp01(tenacity));
    }

    /// <summary>
    /// 스탯 변동 적용: 최종 스탯 = (기본값 + 고정 보너스) * (1 + 비율 보너스)
    /// </summary>
    public static float ApplyStatModifiers(float baseValue, float flatBonus, float percentBonus)
    {
        return (baseValue + flatBonus) * (1f + percentBonus);
    }
}