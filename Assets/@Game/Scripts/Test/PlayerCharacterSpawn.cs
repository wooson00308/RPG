using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections;

public class PlayerCharacterSpawn : CharacterSpawnBase
{
    [SerializeField] private CharacterData _playerData;
    [SerializeField] private bool _isStartSpawn;


    private void Start()
    {
        if (_playerData != null && _isStartSpawn)
        {
            SpawnCharacters();  //Simplified version
        }
    }

    [Button]  //Keep the button for manual spawning.
    public override void SpawnCharacters()
    {
        if (!ValidateSpawnParameters(_playerData)) //use the validation
            return;

        StartCoroutine(SpawnTeam(_playerData, 1, Team.Friendly, friendlySpawnPoint, _friendlyCharacters));
    }
}