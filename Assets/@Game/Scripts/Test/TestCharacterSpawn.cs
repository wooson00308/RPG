using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections;

public class TestCharacterSpawn : CharacterSpawnBase
{
    [SerializeField] private CharacterData testCharacterData;
    [SerializeField, Min(1)] private int friendlySpawnAmount = 1;
    [SerializeField, Min(1)] private int enemySpawnAmount = 1;

    [Button]
    public override void SpawnCharacters()
    {
        if (!ValidateSpawnParameters(testCharacterData))//use validation
            return;

        StartCoroutine(SpawnTeam(testCharacterData, friendlySpawnAmount, Team.Friendly, friendlySpawnPoint, _friendlyCharacters));
        StartCoroutine(SpawnTeam(testCharacterData, enemySpawnAmount, Team.Enemy, enemySpawnPoint, _enemyCharacters));
    }
}