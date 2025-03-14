using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(StateHubSingleton))]
public class GameState : State
{
    [SerializeField]
    protected string _requiredScene = "";

    [SerializeField]
    protected bool _useAsyncLoading = false;

    public override async void OnEnter()
    {
        if (!string.IsNullOrEmpty(_requiredScene) && SceneManager.GetActiveScene().name != _requiredScene)
        {
            if (_useAsyncLoading)
            {
                await LoadSceneAsync(_requiredScene);
            }
            else
            {
                SceneManager.LoadScene(_requiredScene);
            }
        }

        base.OnEnter();
    }

    private async Task LoadSceneAsync(string sceneName)
    {
        var operation = SceneManager.LoadSceneAsync(sceneName);
        while (!operation.isDone)
        {
            await Task.Yield();
        }
    }
}
