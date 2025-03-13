using UnityEngine;

public class GameTime : Singleton<GameTime>
{
    public float timeScale;

    public static float TimeScale
    {
        get
        {
            return Instance.timeScale;
        }
        set
        {
            Instance.timeScale = value;
        }
    }

    public static float DeltaTime { get; private set; }

    public void FixedUpdate()
    {
        DeltaTime = Time.unscaledDeltaTime * timeScale;
    }
}
