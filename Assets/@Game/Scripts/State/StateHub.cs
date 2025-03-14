using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StateHub : MonoBehaviour
{
    private State _curState;

    private List<State> _states = new();

    protected virtual void Awake()
    {
        _states = gameObject.GetComponentsInChildren<State>().ToList();

        foreach (var state in _states)
        {
            state.Initialzed(this);
        }

        _curState = _states.First();
        _curState.OnEnter();
    }

    protected virtual void Update()
    {
        _curState?.OnUpdate();
    }

    public void NextState<T>() where T : State
    {
        var state = _states.Find(x => x is T);
        if (state == null)
        {
            Logger.LogWarning($"{typeof(T)} not found");
            return;
        }

        NextState(state);
    }

    public void NextState<T>(T state) where T : State
    {
        _curState?.OnExit();
        state.OnEnter();

        _curState = state;
    }
}
