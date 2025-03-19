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
    public AudioMixerGroup sfxMixerGroup; // Audio Mixer 지원 추가

    [ReadOnly]
    public List<AudioSource> sfxSourcePool = new List<AudioSource>();

    [TitleGroup("Audio Clips"), InlineEditor(InlineEditorObjectFieldModes.Hidden)]
    public SoundDatabase soundDatabase;

    [System.Serializable]
    public class SFXInfo
    {
        public AudioClip clip;
        [Range(0f, 1f)]
        public float volume = 1f;
        [LabelText("Max Concurrent Plays")]
        [Range(1, 20)]
        public int maxConcurrentPlays = 5; // 클립별 최대 동시 재생 수 (기본값 5)

        public SFXInfo(AudioClip clip)
        {
            this.clip = clip;
            this.volume = 1f;
            this.maxConcurrentPlays = 5;
        }

        public SFXInfo() { }
    }

    [TitleGroup("Volume Control"), Range(0f, 1f)]
    [LabelText("BGM Volume")]
    public float bgmVolume = 1f;

    [TitleGroup("Volume Control"), Range(0f, 1f)]
    [LabelText("Master SFX Volume")]
    public float sfxVolume = 1f;

    private Dictionary<string, int> activeSFXCounts = new Dictionary<string, int>(); // 현재 재생 중인 SFX 수 추적

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
        foreach (AudioSource source in sfxSourcePool)
        {
            source.volume = sfxVolume;
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

    private AudioSource CreateAndAddSFXSource()
    {
        GameObject sfxObject = new GameObject("SFXSource");
        sfxObject.transform.SetParent(transform);
        AudioSource newSource = sfxObject.AddComponent<AudioSource>();
        newSource.outputAudioMixerGroup = sfxMixerGroup; // Audio Mixer 연결
        sfxSourcePool.Add(newSource);
        newSource.volume = sfxVolume;
        newSource.playOnAwake = false;
        newSource.priority = 128;
        return newSource;
    }

    private AudioSource GetSFXSourceFromPool()
    {
        foreach (AudioSource source in sfxSourcePool)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }
        AudioSource newSource = CreateAndAddSFXSource();
        return newSource;
    }

    private void ReturnSFXSourceToPool(AudioSource source, string sfxName)
    {
        source.Stop();
        source.clip = null;
        source.pitch = 1f;
        source.volume = sfxVolume;

        // 재생 수 감소
        if (activeSFXCounts.ContainsKey(sfxName))
        {
            activeSFXCounts[sfxName]--;
            if (activeSFXCounts[sfxName] <= 0)
                activeSFXCounts.Remove(sfxName);
        }
    }

    [TitleGroup("Play Function"), Button(ButtonSizes.Large)]
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

    [TitleGroup("Play Function"), Button(ButtonSizes.Large)]
    public void PlaySFX(string sfxName, float? customVolume = null, int priority = 128)
    {
        if (!soundDatabase.sfxClips.ContainsKey(sfxName))
        {
            Debug.LogError("SFX not found: " + sfxName);
            return;
        }

        SFXInfo sfxInfo = soundDatabase.sfxClips[sfxName];
        int currentCount = activeSFXCounts.ContainsKey(sfxName) ? activeSFXCounts[sfxName] : 0;

        if (currentCount >= sfxInfo.maxConcurrentPlays)
        {
            return;
        }

        AudioSource source = GetSFXSourceFromPool();
        source.clip = sfxInfo.clip;
        source.volume = customVolume ?? sfxInfo.volume * sfxVolume;
        source.priority = priority;
        source.Play();

        // 재생 수 증가
        activeSFXCounts[sfxName] = currentCount + 1;

        StartCoroutine(ReturnToPoolAfterPlaying(source, sfxName));
    }

    private IEnumerator ReturnToPoolAfterPlaying(AudioSource source, string sfxName)
    {
        yield return new WaitForSeconds(source.clip.length);
        ReturnSFXSourceToPool(source, sfxName);
    }

    [TitleGroup("Fade Control"), Button(ButtonSizes.Medium)]
    [LabelText("Fade In BGM")]
    public void FadeInBGM(string bgmName, float fadeDuration)
    {
        StartCoroutine(FadeIn(bgmName, fadeDuration));
    }

    private IEnumerator FadeIn(string bgmName, float fadeDuration)
    {
        if (!soundDatabase.bgmClips.ContainsKey(bgmName))
        {
            Debug.LogError("BGM not found: " + bgmName);
            yield break;
        }

        PlayBGM(bgmName);
        float currentTime = 0;
        float start = 0f;
        float targetVolume = bgmVolume;
        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(start, targetVolume, currentTime / fadeDuration);
            yield return null;
        }
        bgmSource.volume = targetVolume;
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