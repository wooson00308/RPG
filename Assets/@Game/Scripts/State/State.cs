using Sirenix.OdinInspector;
using UnityEngine;

public abstract class State : MonoBehaviour
{
    public IStateHub Hub { get; protected set; }
    public bool IsProcessing { get; protected set; }

    public virtual void OnEnter()
    {
        IsProcessing = true;
    }

    public virtual void OnUpdate() { } 
    public virtual void OnExit()
    {
        IsProcessing = false;
    } 

    public virtual void Initialzed(IStateHub hub)
    {
        Hub = hub;
    }

    [Button]
    public void OnChangeProcess()
    {
        Hub.NextState(this);
    }
}
