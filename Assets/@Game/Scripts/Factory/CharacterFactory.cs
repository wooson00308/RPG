using System.Collections.Generic;
using UnityEngine;

public class CharacterFactory : MonoBehaviour
{
    private List<Character> _characters = new();

    public Transform root;
    public string prefabPath = "Prefabs";

    public Character Spawn(CharacterData data, Team team)
    {
        var spawnObj = ObjectPooler.Instance.SpawnFromPath(data.id, $"{prefabPath}/{data.id}/{data.id}", root);
        Character character = spawnObj.GetComponent<Character>();
        character.Initialized(this, data, team);

        _characters.Add(character);

        return character; // Spawn 메서드가 Character를 반환하도록 수정
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
        ObjectPooler.Instance.ReturnToPool(character.Data.id, character.gameObject);
    }

    // 주어진 캐릭터와 팀이 다른 가장 가까운 적 캐릭터를 찾습니다.
    public Character FindNearestEnemy(Character character)
    {
        Character nearestEnemy = null;
        float nearestDistance = float.MaxValue;

        foreach (Character otherCharacter in _characters)
        {
            // 자기 자신, 초기화되지 않은 캐릭터, 같은 팀은 건너뜁니다.
            if (otherCharacter == character || !otherCharacter.IsInitialized || otherCharacter.Team == character.Team || otherCharacter.IsDeath)
            {
                continue;
            }

            // 2D 거리 계산 (y 좌표는 무시)
            float distance = Vector2.Distance(
                new Vector2(character.transform.position.x, character.transform.position.z),
                new Vector2(otherCharacter.transform.position.x, otherCharacter.transform.position.z)
            );

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = otherCharacter;
            }
        }

        return nearestEnemy;
    }

    // 캐릭터 리스트를 반환하는 메서드 추가 (필요한 경우)
    public List<Character> GetAllCharacters()
    {
        return _characters;
    }

    //특정 팀의 캐릭터 리스트를 반환
    public List<Character> GetCharactersByTeam(Team team)
    {
        List<Character> teamCharacters = new List<Character>();
        foreach (Character character in _characters)
        {
            if (character.Team == team && character.IsInitialized)
            {
                teamCharacters.Add(character);
            }
        }
        return teamCharacters;
    }
}