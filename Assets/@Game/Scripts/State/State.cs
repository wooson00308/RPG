using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(StateHub))]
public abstract class State : MonoBehaviour
{
    public StateHub Hub { get; protected set; }
    public bool IsRunning { get; protected set; }

    public virtual void OnEnter()
    {
        IsRunning = true;
    }

    public virtual void OnUpdate() { } 
    public virtual void OnExit()
    {
        IsRunning = false;
    } 

    public virtual void Initialzed(StateHub hub)
    {
        Hub = hub;
    }

    [Button]
    public void OnChangeProcess()
    {
        Hub.NextState(this);
    }
}
