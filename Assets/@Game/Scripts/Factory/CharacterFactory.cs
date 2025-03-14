using System.Collections.Generic;
using UnityEngine;

public class CharacterFactory : MonoBehaviour
{
    private List<Character> _characters = new();

    public Transform root;
    public string prefabPath = "Prefabs/Character/";

    public Character Spawn(CharacterData data, Team team)
    {
        var spawnObj = ObjectPooler.Instance.SpawnFromPath($"{prefabPath}/{data.id}", root);
        Character character = spawnObj.GetComponent<Character>();
        character.Initialized(this, data, team);

        _characters.Add(character);

        return null;
    }

    public List<Character> Spawn(CharacterData data, int amount, Team team)
    {
        var list = new List<Character>();
        for (int i = 0; i < amount; i++)
        {
            list.Add(Spawn(data, team));
        }

        return list;
    }

    public void ReturnToPool(Character character)
    {
        _characters.Remove(character);
        ObjectPooler.Instance.ReturnToPool(character.gameObject);
    }
}
