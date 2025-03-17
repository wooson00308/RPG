using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Scriptable Objects/CharacterData")]
public class CharacterData : Data
{
    [BoxGroup("기본 스탯", CenterLabel = true), Space(10)]
    [Tooltip("기본 이동 속도 (레벨업 시 증가 없음)")]
    [LabelText("이동 속도")]
    public float moveSpeed;

    [BoxGroup("기본 스탯")]
    [Tooltip("기본 공격 범위")]
    [LabelText("공격 범위")]
    public float attackRange;

    [BoxGroup("방어 스탯", CenterLabel = true), Space(10)]
    [Tooltip("기본 체력 (레벨업 시 증가)")]
    [LabelText("기본 체력")]
    public float baseHealth;

    [BoxGroup("방어 스탯")]
    [Tooltip("레벨당 증가하는 체력량")]
    [LabelText("레벨당 체력 증가량")]
    public float healthPerLevel;

    [BoxGroup("방어 스탯")]
    [Tooltip("기본 체력 재생 (HP5, 5초당 회복량)")]
    [LabelText("기본 체력 재생 (HP5)")]
    public float baseHealthRegen;

    [BoxGroup("방어 스탯")]
    [Tooltip("레벨당 증가하는 체력 재생량 (HP5)")]
    [LabelText("레벨당 체력 재생 증가량")]
    public float healthRegenPerLevel;

    [BoxGroup("방어 스탯")]
    [Tooltip("기본 방어 (물리 피해 감소)")]
    [LabelText("기본 방어")]
    public float baseArmor;

    [BoxGroup("방어 스탯")]
    [Tooltip("레벨당 증가하는 방어량")]
    [LabelText("레벨당 방어 증가량")]
    public float armorPerLevel;

    [BoxGroup("방어 스탯")]
    [Tooltip("기본 마법 저항 (마법 피해 감소)")]
    [LabelText("기본 마법 저항")]
    public float baseMagicResist;

    [BoxGroup("방어 스탯")]
    [Tooltip("레벨당 증가하는 마법 저항량")]
    [LabelText("레벨당 마법 저항 증가량")]
    public float magicResistPerLevel;

    [BoxGroup("방어 스탯")]
    [Tooltip("강인함 (군중 제어 지속 시간 감소 비율, 0~1 사이)")]
    [LabelText("강인함")]
    [Range(0f, 1f)]
    public float tenacity;

    [BoxGroup("공격 스탯", CenterLabel = true), Space(10)]
    [Tooltip("기본 공격력 (레벨업 시 증가)")]
    [LabelText("기본 공격력")]
    public float baseAttackDamage;

    [BoxGroup("공격 스탯")]
    [Tooltip("레벨당 증가하는 공격력")]
    [LabelText("레벨당 공격력 증가량")]
    public float attackDamagePerLevel;

    [BoxGroup("공격 스탯")]
    [Tooltip("기본 공격 속도 (레벨업 시 증가)")]
    [LabelText("기본 공격 속도")]
    public float baseAttackSpeed;

    [BoxGroup("공격 스탯")]
    [Tooltip("레벨당 증가하는 공격 속도")]
    [LabelText("레벨당 공격 속도 증가량")]
    public float attackSpeedPerLevel;

    [BoxGroup("공격 스탯")]
    [Tooltip("기본 주문력 (레벨업 시 증가)")]
    [LabelText("기본 주문력")]
    public float baseAbilityPower;

    [BoxGroup("공격 스탯")]
    [Tooltip("레벨당 증가하는 주문력")]
    [LabelText("레벨당 주문력 증가량")]
    public float abilityPowerPerLevel;

    [BoxGroup("공격 스탯")]
    [Tooltip("치명타 확률 (0~1 사이)")]
    [LabelText("치명타 확률")]
    [Range(0f, 1f)]
    public float critChance;

    [BoxGroup("공격 스탯")]
    [Tooltip("치명타 피해 배율 (기본 1.75, 즉 175%)")]
    [LabelText("치명타 피해 배율")]
    public float critDamage = 1.75f;

    [BoxGroup("공격 스탯")]
    [Tooltip("방어 관통 (적 방어를 무시)")]
    [LabelText("방어 관통")]
    public float armorPenetration;

    [BoxGroup("공격 스탯")]
    [Tooltip("마법 관통 (적 마법 저항을 무시)")]
    [LabelText("마법 관통")]
    public float magicPenetration;

    [BoxGroup("공격 스탯")]
    [Tooltip("흡혈 (기본 공격 피해의 회복 비율, 0~1 사이)")]
    [LabelText("흡혈")]
    [Range(0f, 1f)]
    public float lifeSteal;

    [BoxGroup("공격 스탯")]
    [Tooltip("물리 흡혈 (물리 피해의 회복 비율, 0~1 사이, 광역/소환수 피해는 33% 적용)")]
    [LabelText("물리 흡혈")]
    [Range(0f, 1f)]
    public float physicalVamp;

    [BoxGroup("공격 스탯")]
    [Tooltip("전체 흡혈 (모든 피해의 회복 비율, 0~1 사이, 비챔피언 대상은 20% 적용)")]
    [LabelText("전체 흡혈")]
    [Range(0f, 1f)]
    public float omniVamp;

    [BoxGroup("유틸리티 스탯", CenterLabel = true), Space(10)]
    [Tooltip("기본 마나 (레벨업 시 증가)")]
    [LabelText("기본 마나")]
    public float baseMana;

    [BoxGroup("유틸리티 스탯")]
    [Tooltip("레벨당 증가하는 마나량")]
    [LabelText("레벨당 마나 증가량")]
    public float manaPerLevel;

    [BoxGroup("유틸리티 스탯")]
    [Tooltip("기본 마나 재생 (MP5, 5초당 회복량)")]
    [LabelText("기본 마나 재생 (MP5)")]
    public float baseManaRegen;

    [BoxGroup("유틸리티 스탯")]
    [Tooltip("레벨당 증가하는 마나 재생량 (MP5)")]
    [LabelText("레벨당 마나 재생 증가량")]
    public float manaRegenPerLevel;

    [BoxGroup("유틸리티 스탯")]
    [Tooltip("주문 가속 (능력의 재사용 대기시간 감소)")]
    [LabelText("주문 가속")]
    public float abilityHaste;
}