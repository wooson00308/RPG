using UnityEngine;

/// <summary>
/// 최소한의 기능만 포함한 제네릭 Singleton MonoBehaviour
/// </summary>
/// <typeparam name="T">MonoBehaviour를 상속받는 타입</typeparam>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    /// <summary>
    /// 싱글톤 인스턴스 프로퍼티
    /// </summary>
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<T>();
                if (_instance == null)
                {
                    Logger.LogError($"[Singleton] '{typeof(T)}' 타입의 인스턴스를 찾을 수 없습니다.");
                }
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Logger.LogWarning($"[Singleton] '{typeof(T)}'의 다른 인스턴스가 이미 존재합니다. 현재 인스턴스를 파괴합니다.");
            Destroy(gameObject);
        }
    }
}