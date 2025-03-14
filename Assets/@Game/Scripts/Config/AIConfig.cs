using UnityEngine;

[CreateAssetMenu(fileName = "AIConfig", menuName = "Scriptable Objects/AIConfig")]
public class AIConfig : ScriptableObject
{
    public float chaseDistance;
    public float attackDistance;
}
