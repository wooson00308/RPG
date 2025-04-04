# RPG 게임 API 문서

## 1. 캐릭터 시스템 API

### 1.1 CharacterController
```csharp
public class CharacterController : MonoBehaviour
{
    // 캐릭터 이동
    public void Move(Vector3 direction, float speed);
    
    // 캐릭터 회전
    public void Rotate(Vector3 direction);
    
    // 스탯 변경
    public void ModifyStat(StatType statType, float value);
    
    // 상태 효과 적용
    public void ApplyStatusEffect(StatusEffect effect);
}
```

### 1.2 State Machine
```csharp
public interface IState
{
    void Enter();
    void Update();
    void Exit();
}

public class StateMachine
{
    // 상태 변경
    public void ChangeState(IState newState);
    
    // 현재 상태 확인
    public IState GetCurrentState();
}
```

## 2. 전투 시스템 API

### 2.1 CombatSystem
```csharp
public class CombatSystem
{
    // 데미지 계산
    public float CalculateDamage(Character attacker, Character defender);
    
    // 스킬 사용
    public void UseSkill(Character caster, Skill skill);
    
    // 전투 상태 확인
    public bool IsInCombat(Character character);
}
```

### 2.2 SkillSystem
```csharp
public class SkillSystem
{
    // 스킬 습득
    public void LearnSkill(Character character, Skill skill);
    
    // 스킬 사용 가능 여부 확인
    public bool CanUseSkill(Character character, Skill skill);
    
    // 스킬 쿨다운 확인
    public float GetSkillCooldown(Character character, Skill skill);
}
```

## 3. 인벤토리 시스템 API

### 3.1 Inventory
```csharp
public class Inventory
{
    // 아이템 추가
    public bool AddItem(Item item);
    
    // 아이템 제거
    public bool RemoveItem(Item item);
    
    // 아이템 사용
    public void UseItem(Item item);
    
    // 인벤토리 정렬
    public void SortInventory();
}
```

### 3.2 Equipment
```csharp
public class Equipment
{
    // 장비 장착
    public bool EquipItem(EquipmentItem item);
    
    // 장비 해제
    public void UnequipItem(EquipmentSlot slot);
    
    // 장비 스탯 계산
    public Stats CalculateEquipmentStats();
}
```

## 4. 퀘스트 시스템 API

### 4.1 QuestSystem
```csharp
public class QuestSystem
{
    // 퀘스트 시작
    public void StartQuest(Quest quest);
    
    // 퀘스트 진행도 업데이트
    public void UpdateQuestProgress(Quest quest, int progress);
    
    // 퀘스트 완료
    public void CompleteQuest(Quest quest);
}
```

### 4.2 Quest
```csharp
public class Quest
{
    // 퀘스트 정보
    public string Title { get; }
    public string Description { get; }
    public QuestStatus Status { get; }
    
    // 퀘스트 목표
    public List<QuestObjective> Objectives { get; }
    
    // 보상
    public QuestReward Reward { get; }
}
```

## 5. 유틸리티 API

### 5.1 ObjectPooler
```csharp
public class ObjectPooler
{
    // 오브젝트 가져오기
    public GameObject GetFromPool(string poolName);
    
    // 오브젝트 반환
    public void ReturnToPool(GameObject obj);
    
    // 풀 생성
    public void CreatePool(string poolName, GameObject prefab, int size);
}
```

### 5.2 SoundManager
```csharp
public class SoundManager
{
    // 효과음 재생
    public void PlaySFX(string soundName);
    
    // 배경음악 재생
    public void PlayBGM(string musicName);
    
    // 볼륨 설정
    public void SetVolume(float volume);
}
```

## 6. 이벤트 시스템 API

### 6.1 EventSystem
```csharp
public static class EventSystem
{
    // 이벤트 구독
    public static void Subscribe<T>(Action<T> handler);
    
    // 이벤트 발행
    public static void Publish<T>(T eventData);
    
    // 이벤트 구독 해제
    public static void Unsubscribe<T>(Action<T> handler);
}
``` 