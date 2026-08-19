using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public enum SFXType
{
    FallThud,
    TouchThud,
    Damage,
    Death,
    Pickup,
    Checkpoint,
    Healer,
    PortalIn,
    PortalOut,
    SwitchPress,
    LeverPress,
    TurretHeavy,
    TurretLight,
    TurretLaser,
    SawBlade,
    Blower,
    MouseClick,
    LevelCompleted,
    BlingPop,
    AchievementUnlocked
}

[System.Serializable]
public class SFXData
{
    public SFXType type;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 1f;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Music")]
    [SerializeField] private AudioClip _gameplayMusic;
    [SerializeField] private AudioClip _menuMusic;

    [Range(0f, 1f)]
    [SerializeField] private float _musicVolume = 0.35f;
    [SerializeField] private float _musicFadeDuration = 0.8f;

    [Header("SFX")]
    [SerializeField] private SFXData[] _sfxData;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _sfxSource;

    private Dictionary<SFXType, SFXData> _sfxDictionary;
    private Dictionary<Transform, AudioSource> _loopingSources = new();
    private Tween _musicTween;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        BuildSFXDictionary();
        _musicSource.loop = true;
        _musicSource.playOnAwake = false;
        _sfxSource.playOnAwake = false;
    }

    private void BuildSFXDictionary()
    {
        _sfxDictionary = new();

        foreach (SFXData data in _sfxData)
        {
            if (data.clip == null) continue;
            
            if (_sfxDictionary.ContainsKey(data.type))
            {
                Debug.LogWarning($"Duplicate SFX type found: {data.type}");
                continue;
            }

            _sfxDictionary.Add(data.type, data);
        }
    }

    /* ------------------------------------
    ** ONE-SHOT SFX
    * ------------------------------------- */
    public void PlaySFX(SFXType type)
    {
        if (!_sfxDictionary.TryGetValue(type, out SFXData data))
        {
            Debug.LogWarning($"SFX not found: {type}");
            return;    
        }

        _sfxSource.PlayOneShot(
            data.clip, 
            data.volume
        );
    }
    
    /* ------------------------------------
    ** POSITIONAL ONE-SHOT SFX
    * ------------------------------------- */
    public void PlaySFXAtPosition(SFXType type, Vector3 position)
    {
        if (!_sfxDictionary.TryGetValue(type, out SFXData data))
        {
            Debug.LogWarning($"SFX not found: {type}");
            return;    
        }

        GameObject soundObject = new GameObject($"SFX_{type}");
        soundObject.transform.position = position;

        AudioSource source = soundObject.AddComponent<AudioSource>();
        source.clip = data.clip;
        source.volume = data.volume;
        source.spatialBlend = 0.25f;
        source.minDistance = 3f;
        source.maxDistance = 15f;
        source.Play();

        Destroy(
            soundObject, 
            data.clip.length
        );
    }

    /* ------------------------------------
    ** Positional one-shot SFX
    * ------------------------------------- */
    public void StartLoop(SFXType type, Transform sourceTransform)
    {
        if (_loopingSources.ContainsKey(sourceTransform)) return;

        if (!_sfxDictionary.TryGetValue(type, out SFXData data))
        {
            Debug.LogWarning($"SFX not found: {type}");
            return;    
        }

        GameObject loopObject = new GameObject($"Loop_{type}");
        loopObject.transform.SetParent(sourceTransform);
        loopObject.transform.localPosition = Vector3.zero;

        AudioSource source = loopObject.AddComponent<AudioSource>();
        source.clip = data.clip;
        source.volume = data.volume;
        source.loop = true;
        source.playOnAwake = false;
        source.spatialBlend = 1f;
        source.Play();
        _loopingSources.Add(
            sourceTransform,
            source
        );
    }
    
    public void StopLoop(Transform sourceTransform)
    {
        if (!_loopingSources.TryGetValue(sourceTransform, out AudioSource source))
            return;

        if (source != null)
        {
            source.Stop();
            Destroy(source.gameObject);
        }

        _loopingSources.Remove(sourceTransform);
    }

    /* ---------------------------------
    ** MUSIC
    * ---------------------------------- */
    public void PlayGamePlayMusic()
    {
        PlayMusic(_gameplayMusic);
    }

    public void PlayMenuMusic()
    {
        PlayMusic(_menuMusic);
    }

    private void PlayMusic(AudioClip clip)
    {
        if (clip == null || 
        (_musicSource.clip == clip && _musicSource.isPlaying)) 
            return;

        _musicSource.clip = clip;
        _musicSource.volume = _musicVolume;
        _musicSource.Play();    
    }

    /* -------------------------------
    ** MUSIC CROSSFADE
    * -------------------------------- */
    public void CrossFadeToMenuMusic()
    {
        CrossfadeMusic(_menuMusic);
    }

    public void CrossFadeToGameplayMusic()
    {
        CrossfadeMusic(_gameplayMusic);
    }

    private void CrossfadeMusic(AudioClip targetClip)
    {
        if (targetClip == null) return;

        if (_musicSource.clip == targetClip &&
        _musicSource.isPlaying) return;

        _musicTween?.Kill();

        _musicTween = DOTween.To(
            () => _musicSource.volume,
            value => _musicSource.volume = value,
            0f,
            _musicFadeDuration
        )
        .SetEase(Ease.InOutSine)
        .SetUpdate(true)
        .OnComplete(() =>
        {
            _musicSource.clip = targetClip;
            _musicSource.volume = 0f;
            _musicSource.Play();

            _musicTween = DOTween.To(
                () => _musicSource.volume,
                value => _musicSource.volume = value,
                _musicVolume,
                _musicFadeDuration
            )
            .SetEase(Ease.InOutSine)
            .SetUpdate(true);
        });
    }


    public void StopMusic()
    {
        _musicTween?.Kill();
        _musicSource.Stop();
    }
}
