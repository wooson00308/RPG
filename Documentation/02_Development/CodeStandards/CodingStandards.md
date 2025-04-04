# RPG 게임 코딩 표준

## 1. 네이밍 컨벤션

### 1.1 클래스 및 인터페이스
- PascalCase 사용
- 명사 사용
- 인터페이스는 'I' 접두사 사용
```csharp
public class CharacterController
public interface IState
```

### 1.2 메서드
- PascalCase 사용
- 동사로 시작
```csharp
public void MoveCharacter()
public void CalculateDamage()
```

### 1.3 변수 및 필드
- camelCase 사용
- private 필드는 '_' 접두사 사용
```csharp
private int _health;
public float moveSpeed;
```

### 1.4 상수
- UPPER_SNAKE_CASE 사용
```csharp
public const int MAX_HEALTH = 100;
```

## 2. 코드 구조

### 2.1 클래스 구조
```csharp
public class ExampleClass
{
    // 1. 상수
    private const int CONSTANT_VALUE = 10;

    // 2. 필드
    private int _privateField;
    public int PublicField;

    // 3. 프로퍼티
    public int Property { get; private set; }

    // 4. 생성자
    public ExampleClass()
    {
    }

    // 5. 메서드
    public void PublicMethod()
    {
    }

    // 6. private 메서드
    private void PrivateMethod()
    {
    }
}
```

### 2.2 주석 규칙
- XML 문서 주석 사용
- 복잡한 로직에 대한 설명 주석 추가
```csharp
/// <summary>
/// 캐릭터의 체력을 회복합니다.
/// </summary>
/// <param name="amount">회복할 체력량</param>
public void Heal(int amount)
{
    // 체력이 최대치를 넘지 않도록 제한
    _health = Mathf.Min(_health + amount, MAX_HEALTH);
}
```

## 3. 디자인 패턴 적용

### 3.1 싱글톤 패턴
```csharp
public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("GameManager");
                    _instance = obj.AddComponent<GameManager>();
                }
            }
            return _instance;
        }
    }
}
```

### 3.2 상태 패턴
```csharp
public interface IState
{
    void Enter();
    void Update();
    void Exit();
}

public class IdleState : IState
{
    public void Enter() { }
    public void Update() { }
    public void Exit() { }
}
```

## 4. 에러 처리
- 예외 상황에 대한 적절한 처리
- 로깅 시스템 활용
```csharp
try
{
    // 코드
}
catch (Exception e)
{
    DebugLogger.LogError($"에러 발생: {e.Message}");
    // 복구 로직
}
```

## 5. 성능 최적화 가이드라인
- 오브젝트 풀링 사용
- 불필요한 GameObject 생성/파괴 최소화
- Update 메서드 최적화
- 메모리 할당 최소화 