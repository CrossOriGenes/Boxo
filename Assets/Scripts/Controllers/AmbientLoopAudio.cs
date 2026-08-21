using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AmbientLoopAudio : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;

    [Header("Detection")]
    [SerializeField] private Transform _player;

    [Header("Distance")]
    [SerializeField] private float _fullVolumeDistance = 1.5f;
    [SerializeField] private float _fadeStartDistance = 5.5f;
    [SerializeField] private float _stopDistance = 6f;

    [Header("Volume")]
    [Range(0f, 1f)]
    [SerializeField] private float _maxVolume = 0.2f;

    [Header("Fade")]
    [SerializeField] private float _fadeInDuration = 0.5f;
    [SerializeField] private float _fadeOutDuration = 0.6f;

    private static readonly HashSet<AmbientLoopAudio> _instances = new();

    private Tween _volumeTween;
    private bool _isPlaying,
    _isFadingOut,
    _isPaused,
    _wasPlayingBeforePause;
    private float _currentTargetVolume = -1f;

    private void Awake()
    {
        if (_audioSource == null)
            _audioSource = GetComponent<AudioSource>();
        
        _audioSource.playOnAwake = false;
        _audioSource.loop = true;
        _audioSource.spatialBlend = 0f;
        _audioSource.volume = 0f;
    }

    private void Update()
    {
        if (_isPaused || _player == null) 
            return;

        float distance = Vector2.Distance(
            transform.position,
            _player.position
        );

        if (distance >= _stopDistance)
        {
            FadeOut();
            return;
        }
        
        float volume = CalculateVolume(distance);
        if (volume > 0f) 
            FadeIn(volume);
        else
            FadeOut();
    }

    private void OnEnable()
    {
        _instances.Add(this);
    }

    private void OnDisable()
    {
        _instances.Remove(this);
        _volumeTween?.Kill();

        if (_audioSource != null)
        {
            _audioSource.Stop();
            _audioSource.volume = 0f;
        }

        _isPlaying = false;
        _isFadingOut = false;
        _isPaused = false;
        _wasPlayingBeforePause = false; 
        _currentTargetVolume = -1f;
    }


    /* ---------------------------
    ** PAUSE / RESUME
    ----------------------------- */
    public void Pause()
    {
        if (_isPaused) return;

        _wasPlayingBeforePause = _isPlaying;
        _isPaused = true;
        _volumeTween?.Kill();

        if (_wasPlayingBeforePause && _audioSource != null) 
            _audioSource.Pause();
    }

    public void Resume()
    {
        if (!_isPaused) return;
        _isPaused = false;
        if (_wasPlayingBeforePause && _audioSource != null)
            _audioSource.UnPause();
        _wasPlayingBeforePause = false;
    }

    public static void PauseAll()
    {
        foreach (AmbientLoopAudio ambient in _instances)
        {
            if (ambient != null) 
                ambient.Pause();
        }
    }

    public static void ResumeAll()
    {
        foreach (AmbientLoopAudio ambient in _instances)
        {
            if (ambient != null)
                ambient.Resume();
        }
    }


    /* ---------------------------
    ** VOLUME INTENSITY CALCULATION
    ----------------------------- */
    private float CalculateVolume(float distance)
    {
        if (distance <= _fullVolumeDistance)
            return _maxVolume;

        float t = Mathf.InverseLerp(
            _fadeStartDistance,
            _fullVolumeDistance,
            distance
        );
        t = Mathf.SmoothStep(0f, 1f, t);

        return Mathf.Lerp(0f, _maxVolume, t);
    }


    /* ---------------------------
    ** FADE IN
    ----------------------------- */
    private void FadeIn(float targetVolume)
    {
        if (!_isPlaying)
        {
            _isPlaying = true;
            _isFadingOut = false;
            _audioSource.volume = 0f;
            _audioSource.Play();
        }

        _isFadingOut = false;

        if (Mathf.Abs(_currentTargetVolume - targetVolume) < 0.01f)
            return;

        _currentTargetVolume = targetVolume;
        _volumeTween?.Kill();
        _volumeTween = _audioSource
                    .DOFade(targetVolume, _fadeInDuration)
                    .SetEase(Ease.InOutSine);
    }

    /* ---------------------------
    ** FADE OUT
    ----------------------------- */
    private void FadeOut()
    {
        if (!_isPlaying || _isFadingOut) 
            return;

        _isFadingOut = true;
        _currentTargetVolume = 0f;
        
        _volumeTween?.Kill();

        _volumeTween = 
            _audioSource
            .DOFade(0f, _fadeOutDuration)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
                {
                    _audioSource.Stop();
                    _isPlaying = false;
                    _currentTargetVolume = -1f;
                }
            );
    }
}
