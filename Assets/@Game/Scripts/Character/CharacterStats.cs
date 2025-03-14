using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    private CharacterData _data;
    public float moveSpeed;

    public void Initialized(CharacterData data)
    {
        _data = data;

        moveSpeed = _data.moveSpeed;
    }
}
