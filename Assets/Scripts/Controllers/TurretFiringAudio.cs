using DG.Tweening;
using UnityEngine;

public class TurretFiringAudio : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;

    [Header("Volume")]
    [Range(0f, 1f)]
    [SerializeField] private float _maxVolume = 0.5f;

    private float _fadeInDuration = .15f,
                  _fadeOutDuration = .2f;
    private bool _isPlaying,
                 _isPaused,
                 _wasPlayingBeforePause;
    private Tween _volumeTween;

    private void Awake()
    {
        if (_audioSource == null)
            _audioSource = 
                gameObject
                .transform
                .parent
                .GetComponentInChildren<AudioSource>();

        _audioSource.playOnAwake = false;
        _audioSource.loop = true;
        _audioSource.volume = 0f;
    }

    private void OnEnable()
    {
        GameScreenOverlayUI.PauseGame += Pause;
        GameScreenOverlayUI.ResumeGame += Resume;
        GameScreenOverlayUI.RestartLevel += Restart;
    }

    private void OnDisable()
    {
        _volumeTween?.Kill();

        if (_audioSource != null)
        {
            _audioSource.Stop();
            _audioSource.volume = 0f;
        }

        _isPlaying = false;
        _isPaused = false;
        _wasPlayingBeforePause = false;


        GameScreenOverlayUI.PauseGame -= Pause;
        GameScreenOverlayUI.ResumeGame -= Resume;
        GameScreenOverlayUI.RestartLevel -= Restart;
    }

    /* -----------------------------------
    ** FIRING AUDIO
    ------------------------------------ */
    public void StartFiringAudio()
    {
        if (_audioSource == null || _isPaused)
            return;

        _volumeTween?.Kill();

        if (!_isPlaying)
        {
            _isPlaying = true;
            _audioSource.volume = 0f;
            _audioSource.Play();
        }

        _volumeTween = 
            _audioSource
            .DOFade(
                _maxVolume,
                _fadeInDuration
            )
            .SetEase(Ease.InOutSine);
    }

    public void StopFiringAudio()
    {
        if (_audioSource == null || !_isPlaying)
            return;

        _volumeTween?.Kill();

        _volumeTween = 
            _audioSource
            .DOFade(
                0f,
                _fadeOutDuration
            )
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                _audioSource.Stop();
                _audioSource.volume = 0f;
                _isPlaying = false;
            });
    }

    /* -------------------------------------
    ** PAUSE / RESUME / RESTART
    -------------------------------------- */
    private void Pause()
    {
        if (_isPaused) return;

        _wasPlayingBeforePause = _isPlaying;
        _isPaused = true;

        _volumeTween?.Kill();

        if (_wasPlayingBeforePause)
            _audioSource.Pause();
    }

    private void Resume()
    {
        if (!_isPaused) return;

        _isPaused = false;

        if (_wasPlayingBeforePause)
            _audioSource.UnPause();

        _wasPlayingBeforePause = false;
    }

    private void Restart()
    {
        _volumeTween?.Kill();

        if (_audioSource != null)
        {
            _audioSource.Stop();
            _audioSource.volume = 0f;
        }

        _isPlaying = false;
        _isPaused = false;
        _wasPlayingBeforePause = false;
    }
}
