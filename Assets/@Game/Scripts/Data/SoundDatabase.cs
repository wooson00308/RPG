//SoundDatabase.cs
using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "SoundDatabase", menuName = "Scriptable Objects/SoundDatabase")]
public class SoundDatabase : SerializedScriptableObject
{
    [DictionaryDrawerSettings(KeyLabel = "Clip Name", ValueLabel = "Clip", DisplayMode = DictionaryDisplayOptions.Foldout)]
    public Dictionary<string, AudioClip> bgmClips = new Dictionary<string, AudioClip>();

    [DictionaryDrawerSettings(KeyLabel = "Clip Name", ValueLabel = "Clip Info", DisplayMode = DictionaryDisplayOptions.Foldout)]
    public Dictionary<string, SoundManager.SFXInfo> sfxClips = new Dictionary<string, SoundManager.SFXInfo>();
}