using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine.AI;

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

        SpawnTeam(testCharacterData, friendlySpawnAmount, Team.Friendly, friendlySpawnPoint, _friendlyCharacters);
        SpawnTeam(testCharacterData, enemySpawnAmount, Team.Enemy, enemySpawnPoint, _enemyCharacters);
    }


    private void SpawnTeam(CharacterData data, int amount, Team team, Transform spawnPoint, List<Character> characterList)
    {
        for (int i = 0; i < amount; i++)
        {
            Character spawnedCharacter = characterFactory.Spawn(data, team);
            if (spawnedCharacter != null)
            {
                // Set the position to the spawn point + a small random offset.
                Vector3 spawnPosition = spawnPoint.position + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));

                // Use NavMesh.SamplePosition to ensure the spawned character is on the NavMesh.
                NavMeshHit hit;
                if (NavMesh.SamplePosition(spawnPosition, out hit, 2.0f, NavMesh.AllAreas)) //Increased the sample distance
                {
                    spawnedCharacter.transform.position = hit.position;
                    // spawnedCharacter.GetComponent<NavMeshAgent>().Warp(hit.position); // Use Warp for NavMeshAgent
                }
                else
                {
                    Debug.LogWarning("Spawn position is outside NavMesh!");
                    // Optionally handle the case where the character can't be placed on the NavMesh.
                    // For example, destroy the character or move it to the default spawn point.
                    // characterFactory.ReturnToPool(spawnedCharacter); //풀링 안함
                    Destroy(spawnedCharacter.gameObject); //바로 삭제
                    continue;  // Skip adding to the list and proceed to the next iteration
                }
                characterList.Add(spawnedCharacter);

            }
        }
    }

    [Button]
    public void FindNearestEnemyForFirstFriendly()
    {
        if (_friendlyCharacters.Count > 0)
        {
            Character firstFriendly = _friendlyCharacters[0];
            if (firstFriendly != null && firstFriendly.IsInitialized)
            {
                Character nearestEnemy = characterFactory.FindNearestEnemy(firstFriendly);
                if (nearestEnemy != null)
                {
                    Debug.Log($"Nearest enemy to {firstFriendly.name} is {nearestEnemy.name} at distance: {Vector2.Distance(new Vector2(firstFriendly.transform.position.x, firstFriendly.transform.position.z), new Vector2(nearestEnemy.transform.position.x, nearestEnemy.transform.position.z))}");
                    // 추가적인 행동 (예: 타겟 설정)
                    firstFriendly.target = nearestEnemy;
                }
                else
                {
                    Debug.Log("No enemies found.");
                }
            }
            else
            {
                Debug.LogWarning("First friendly character is null or not initialized.");
                _friendlyCharacters.RemoveAt(0);
            }

        }
        else
        {
            Debug.Log("No friendly characters spawned.");
        }
    }

    [Button]
    public void FindNearestEnemyOfTeam(Team team)
    {
        Character nearestEnemy = characterFactory.FindNearestEnemyOfTeam(team);

        if (nearestEnemy != null)
        {
            Debug.Log($"Nearest enemy to Team {team} is {nearestEnemy.name}");
        }
        else
        {
            Debug.Log($"No enemy found for Team {team}.");
        }
    }
}