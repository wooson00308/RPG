using UnityEngine;

/// <summary>
/// �ּ����� ��ɸ� ������ ���׸� Singleton MonoBehaviour
/// </summary>
/// <typeparam name="T">MonoBehaviour�� ��ӹ޴� Ÿ��</typeparam>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    /// <summary>
    /// �̱��� �ν��Ͻ� ������Ƽ
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
                    Logger.LogError($"[Singleton] '{typeof(T)}' Ÿ���� �ν��Ͻ��� ã�� �� �����ϴ�.");
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
            Logger.LogWarning($"[Singleton] '{typeof(T)}'�� �ٸ� �ν��Ͻ��� �̹� �����մϴ�. ���� �ν��Ͻ��� �ı��մϴ�.");
            Destroy(gameObject);
        }
    }
}