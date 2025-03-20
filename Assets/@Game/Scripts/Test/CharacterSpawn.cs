using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine.AI;
using System.Collections;

public class CharacterSpawn : MonoBehaviour
{
    [SerializeField] private CharacterFactory characterFactory;
    [SerializeField] private CharacterData testCharacterData;

    [SerializeField] private Transform friendlySpawnPoint;
    [SerializeField] private Transform enemySpawnPoint;


    [SerializeField, Min(1)] private int friendlySpawnAmount = 1;
    [SerializeField, Min(1)] private int enemySpawnAmount = 1;

    private List<Character> _friendlyCharacters = new List<Character>();
    private List<Character> _enemyCharacters = new List<Character>();

    [Button]
    public void SpawnCharacters()
    {
        if (characterFactory == null)
        {
            Debug.LogError("CharacterFactory is not assigned!");
            return;
        }

        if (testCharacterData == null)
        {
            Debug.LogError("TestCharacterData is not assigned!");
            return;
        }
        if (friendlySpawnPoint == null || enemySpawnPoint == null)
        {
            Debug.LogError("Spawn points are not assigned!");
            return;
        }

        StartCoroutine(
                SpawnTeam(testCharacterData, friendlySpawnAmount, Team.Friendly, friendlySpawnPoint, _friendlyCharacters));
        StartCoroutine(SpawnTeam(testCharacterData, enemySpawnAmount, Team.Enemy, enemySpawnPoint, _enemyCharacters));
    }


    private IEnumerator SpawnTeam(CharacterData data, int amount, Team team, Transform spawnPoint, List<Character> characterList)
    {
        for (int i = 0; i < amount; i++)
        {
            Character spawnedCharacter = characterFactory.Spawn(data, team);
            if (spawnedCharacter != null)
            {
                Vector3 spawnPosition = spawnPoint.position + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
                NavMeshHit hit;
                if (NavMesh.SamplePosition(spawnPosition, out hit, 2.0f, NavMesh.AllAreas)) //Increased the sample distance
                {
                    spawnedCharacter.transform.position = hit.position;
                }
                characterList.Add(spawnedCharacter);

            }
            yield return null;
        }
    }
}