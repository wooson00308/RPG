public class StateHubSingleton : StateHub
{
    public static StateHubSingleton Instance { get; private set; }

    protected override void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        base.Awake();
    }

    protected void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}