using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine.Audio;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SoundManager : Singleton<SoundManager>
{
    [TitleGroup("BGM Settings")]
    [LabelText("BGM Audio Source")]
    public AudioSource bgmSource;

    [TitleGroup("SFX Settings")]
    [LabelText("SFX Pool Size")]
    public int sfxPoolSize = 20;

    [TitleGroup("SFX Settings")]
    public AudioMixerGroup sfxMixerGroup;

    // PooledAudioSource 리스트 사용
    [ReadOnly]
    public List<PooledAudioSource> sfxSourcePool = new List<PooledAudioSource>();

    [TitleGroup("Audio Clips"), InlineEditor(InlineEditorObjectFieldModes.Hidden)]
    public SoundDatabase soundDatabase;

    [System.Serializable]
    public class SFXInfo
    {
        public AudioClip clip;
        [Range(0f, 1f)]
        public float volume = 1f;
        [LabelText("Cooldown (Seconds)")]
        [Range(0f, 5f)]
        public float cooldown = 0.1f;

        public SFXInfo(AudioClip clip)
        {
            this.clip = clip;
            this.volume = 1f;
            this.cooldown = 0.1f;
        }

        public SFXInfo() { }
    }

    [TitleGroup("Volume Control"), Range(0f, 1f)]
    [LabelText("BGM Volume")]
    public float bgmVolume = 1f;

    [TitleGroup("Volume Control"), Range(0f, 1f)]
    [LabelText("Master SFX Volume")]
    public float sfxVolume = 1f;

    private Dictionary<string, float> lastPlayedTimes = new Dictionary<string, float>();


    [TitleGroup("Volume Control"), ProgressBar(0, 1, r: 0.2f, g: 0.7f, b: 1f)]
    [LabelText("Set BGM Volume")]
    public void SetBGMVolume(float volume)
    {
        bgmVolume = volume;
        bgmSource.volume = bgmVolume;
    }

    [TitleGroup("Volume Control"), ProgressBar(0, 1, r: 1f, g: 0.5f, b: 0.2f)]
    [LabelText("Set Master SFX Volume")]
    public void SetMasterSFXVolume(float volume)
    {
        sfxVolume = volume;
        // PooledAudioSource 안의 Source에 접근
        foreach (PooledAudioSource pooledSource in sfxSourcePool)
        {
            pooledSource.Source.volume = sfxVolume;
        }
    }

    private void Update()
    {
        if (bgmSource != null)
            bgmSource.volume = bgmVolume;
    }

    protected override void Awake()
    {
        base.Awake();
        InitializeSFXPool();
        if (soundDatabase == null)
        {
            CreateSoundDatabase();
        }
    }

#if UNITY_EDITOR
    [Button(ButtonSizes.Medium), ButtonGroup("LoadClips")]
    [LabelText("Load Audio Clips")]
    private void LoadAudioClips()
    {
        if (soundDatabase == null)
        {
            Debug.LogError("SoundDatabase is not assigned!");
            return;
        }
        soundDatabase.bgmClips.Clear();
        soundDatabase.sfxClips.Clear();

        AudioClip[] loadedBgmClips = Resources.LoadAll<AudioClip>("Sounds/BGM");
        foreach (AudioClip clip in loadedBgmClips)
        {
            soundDatabase.bgmClips[clip.name] = clip;
        }

        AudioClip[] loadedSfxClips = Resources.LoadAll<AudioClip>("Sounds/SFX");
        foreach (AudioClip clip in loadedSfxClips)
        {
            soundDatabase.sfxClips[clip.name] = new SFXInfo(clip);
        }
        EditorUtility.SetDirty(soundDatabase);
    }

    [Button(ButtonSizes.Medium), ButtonGroup("LoadClips")]
    [LabelText("Clear Audio Clips")]
    private void ClearAudioClips()
    {
        if (soundDatabase != null)
        {
            soundDatabase.bgmClips.Clear();
            soundDatabase.sfxClips.Clear();
            EditorUtility.SetDirty(soundDatabase);
        }
    }
#endif

    private void InitializeSFXPool()
    {
        for (int i = 0; i < sfxPoolSize; i++)
        {
            CreateAndAddSFXSource();
        }
    }

    private void CreateAndAddSFXSource()
    {
        GameObject sfxObject = new GameObject("SFXSource");
        sfxObject.transform.SetParent(transform);
        AudioSource newSource = sfxObject.AddComponent<AudioSource>();
        newSource.outputAudioMixerGroup = sfxMixerGroup;
        newSource.volume = sfxVolume;
        newSource.playOnAwake = false;
        newSource.priority = 128;
        // PooledAudioSource로 래핑
        sfxSourcePool.Add(new PooledAudioSource(newSource));
    }


    private PooledAudioSource GetSFXSourceFromPool()
    {
        foreach (PooledAudioSource pooledSource in sfxSourcePool)
        {
            if (pooledSource.IsAvailable)
            {
                return pooledSource;
            }
        }

        // 사용 가능한 PooledAudioSource가 없으면 새로 생성
        CreateAndAddSFXSource(); // 리스트에 자동 추가
        return sfxSourcePool[sfxSourcePool.Count - 1];

    }


    private void ReturnSFXSourceToPool(PooledAudioSource pooledSource)
    {
        pooledSource.StopAndReset();
        pooledSource.Source.volume = sfxVolume;
    }

    public void PlayBGM(string bgmName)
    {
        if (soundDatabase.bgmClips.ContainsKey(bgmName))
        {
            if (bgmSource.isPlaying)
                bgmSource.Stop();

            bgmSource.clip = soundDatabase.bgmClips[bgmName];
            bgmSource.Play();
        }
        else
        {
            Debug.LogError("BGM not found: " + bgmName);
        }
    }

    public void PlaySFX(string sfxName, float? customVolume = null)
    {
        if (!soundDatabase.sfxClips.ContainsKey(sfxName))
        {
            Debug.LogError("SFX not found: " + sfxName);
            return;
        }

        SFXInfo sfxInfo = soundDatabase.sfxClips[sfxName];

        // 쿨다운 검사
        if (lastPlayedTimes.ContainsKey(sfxName))
        {
            if (Time.time < lastPlayedTimes[sfxName] + sfxInfo.cooldown)
            {
                return;
            }
        }


        // 재생할 PooledAudioSource 결정
        PooledAudioSource pooledAudioSourceToUse = null;

        // 1. 사용 가능한 PooledAudioSource 사용
        foreach (PooledAudioSource pooledSource in sfxSourcePool)
        {
            if (pooledSource.IsAvailable)
            {
                pooledAudioSourceToUse = pooledSource;
                break;
            }
        }

        // 2. 사용 가능한 소스도 없으면 그냥 리턴.
        if (pooledAudioSourceToUse == null)
        {
            return;
        }

        // SFX 재생
        pooledAudioSourceToUse.Play(sfxInfo.clip, customVolume ?? sfxInfo.volume * sfxVolume, 128);

        // 마지막 재생 시간 업데이트
        lastPlayedTimes[sfxName] = Time.time;

        StartCoroutine(ReturnToPoolAfterPlaying(pooledAudioSourceToUse));
    }


    private IEnumerator ReturnToPoolAfterPlaying(PooledAudioSource pooledSource)
    {
        yield return new WaitForSeconds(pooledSource?.Source.clip != null ? pooledSource.Source.clip.length + 0.1f : 0.1f);
        ReturnSFXSourceToPool(pooledSource); //인자가 PooledAudioSource
    }

#if UNITY_EDITOR
    private void CreateSoundDatabase()
    {
        soundDatabase = Resources.Load<SoundDatabase>("SoundDatabase");

        if (soundDatabase == null)
        {
            soundDatabase = ScriptableObject.CreateInstance<SoundDatabase>();
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }
            AssetDatabase.CreateAsset(soundDatabase, "Assets/Resources/SoundDatabase.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
#endif
}
// 별도 파일 (PooledAudioSource.cs)
public class PooledAudioSource
{
    public AudioSource Source { get; private set; }
    public bool IsAvailable { get; set; }

    public PooledAudioSource(AudioSource source)
    {
        Source = source;
        IsAvailable = true;
    }

    public void Play(AudioClip clip, float volume, int priority)
    {
        Source.clip = clip;
        Source.volume = volume;
        Source.priority = priority; // 여기서 priority 설정
        Source.Play();
        IsAvailable = false;
    }

    public void StopAndReset()
    {
        Source.Stop();
        Source.clip = null;
        Source.pitch = 1f;
        IsAvailable = true;
    }
}

