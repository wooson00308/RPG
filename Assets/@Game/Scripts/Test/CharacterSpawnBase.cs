using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine.AI;
using System.Collections;

public abstract class CharacterSpawnBase : MonoBehaviour
{
    [SerializeField] protected CharacterFactory characterFactory;
    [SerializeField] protected Transform friendlySpawnPoint;
    [SerializeField] protected Transform enemySpawnPoint;

    protected List<Character> _friendlyCharacters = new List<Character>();
    protected List<Character> _enemyCharacters = new List<Character>();

    protected virtual void Awake() // or Start, depending on when lists need initialization
    {
        //List initialization moved here, as suggested
        _friendlyCharacters = new List<Character>();
        _enemyCharacters = new List<Character>();
    }


    protected IEnumerator SpawnTeam(CharacterData data, int amount, Team team, Transform spawnPoint, List<Character> characterList)
    {
        for (int i = 0; i < amount; i++)
        {
            Character spawnedCharacter = characterFactory.Spawn(data, team);
            if (spawnedCharacter != null)
            {
                Vector3 spawnPosition = spawnPoint.position + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
                NavMeshHit hit;
                if (NavMesh.SamplePosition(spawnPosition, out hit, 2.0f, NavMesh.AllAreas))
                {
                    spawnedCharacter.transform.position = hit.position;
                    spawnedCharacter.gameObject.SetActive(true); // Ensure the character is active
                }
                else
                {
                    Debug.LogWarning($"Could not find a valid NavMesh position near {spawnPosition} for character {spawnedCharacter.name}.");
                }
                characterList.Add(spawnedCharacter);
            }
            yield return null; // added for safety.
        }
    }


    protected bool ValidateSpawnParameters(CharacterData data) //validation refactored
    {
        if (characterFactory == null)
        {
            Debug.LogError("CharacterFactory is not assigned!");
            return false;
        }

        if (data == null)
        {
            Debug.LogError("CharacterData is not assigned!");
            return false;
        }

        if (friendlySpawnPoint == null || enemySpawnPoint == null)
        {
            Debug.LogError("Spawn points are not assigned!");
            return false;
        }

        return true; // All checks passed
    }

    // Abstract method to be implemented by child classes
    public abstract void SpawnCharacters();
}