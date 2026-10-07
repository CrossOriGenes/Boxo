using UnityEngine;

public class SawAudioController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioSource _audioSource;

    [Header("Player")]
    [SerializeField] private Transform _player;

    [Header("Distance")]
    [SerializeField] private float _minDistance = 1.5f;
    [SerializeField] private float _maxDistance = 8f;
    
    [Header("Volume")]
    [Range(0f, 1f)]
    [SerializeField] private float _minVolume = .05f;
    [Range(0f, 1f)]
    [SerializeField] private float _maxVolume = .65f;

    [Header("Motion Fade")]
    [SerializeField] private float _startFadeDuration = .6f;
    [SerializeField] private float _stopFadeDuration = .5f;

    private SawMovementController _sawMovement;
    private float _motionVolume;
    private bool _audioStarted,
                _isPaused, 
                _wasPlayingBeforePause;


    private void OnEnable()
    {
        GameScreenOverlayUI.RestartLevel += ResetAudio;
        GameScreenOverlayUI.PauseGame += PauseAudio;
        GameScreenOverlayUI.ResumeGame += ResumeAudio;
    }

    private void OnDisable()
    {
        GameScreenOverlayUI.RestartLevel -= ResetAudio;
        GameScreenOverlayUI.PauseGame -= PauseAudio;
        GameScreenOverlayUI.ResumeGame -= ResumeAudio;
    }

    private void Awake()
    {
        _sawMovement = 
            GetComponentInParent<SawMovementController>();

        _audioSource.playOnAwake = false;
        _audioSource.loop = true;
        _audioSource.spatialBlend = 0f;
        _audioSource.volume = 0f;
    }

    private void Update()
    {
        if (_sawMovement != null)
            HandleSawAudio();
    }

    private void HandleSawAudio()
    {
        if (_sawMovement.IsSawActive)
        {
            StartAudio();

            _motionVolume = 
                Mathf.MoveTowards(
                    _motionVolume,
                    1f,
                    Time.deltaTime / _startFadeDuration
                );
        }
        else if (_sawMovement.IsStopping)
        {
            _motionVolume = 
                Mathf.MoveTowards(
                    _motionVolume,
                    0f,
                    Time.deltaTime / _stopFadeDuration
                );

            if (_motionVolume <= 0f)
            {
                StopAudio();
                return;
            }
        }
        else
        {
            _motionVolume = 0f;
            StopAudio();
            return;
        }

        UpdateVolume();
    }

    private void StartAudio()
    {
        if (_audioStarted || _isPaused) 
            return;

        _audioStarted = true;

        _audioSource.volume = 0f;
        _audioSource.Play();
    }

    private void StopAudio()
    {
        if (!_audioStarted)
            return;

        _audioStarted = false;

        _audioSource.Stop();
        _audioSource.volume = 0f;
    }

    private void UpdateVolume()
    {
        if (!_audioSource || _player == null)
            return;

        float distance = 
            Vector2.Distance(
                transform.position,
                _player.position
            );

        float distanceIntensity = 
            Mathf.InverseLerp(
                _maxDistance,
                _minDistance,
                distance
            );
        distanceIntensity = 
            Mathf.SmoothStep(
                0f,
                1f,
                distanceIntensity
            );

        float distanceVolume = 
            Mathf.Lerp(
                _minVolume,
                _maxVolume,
                distanceIntensity
            );

        _audioSource.volume =   
            distanceVolume * _motionVolume;
    }

    private void PauseAudio()
    {
        if (_isPaused) return;

        _isPaused = true;

        _wasPlayingBeforePause = 
            _audioStarted && _audioSource.isPlaying;
        
        if (_wasPlayingBeforePause)
            _audioSource.Pause();
    }

    private void ResumeAudio()
    {
        if (!_isPaused) return;

        _isPaused = false;

        if (_wasPlayingBeforePause)
            _audioSource.UnPause();
        
        _wasPlayingBeforePause = false;
    }

    private void ResetAudio()
    {
        _isPaused = false;
        _wasPlayingBeforePause = false;

        _motionVolume = 0f;
        _audioStarted = false;

        _audioSource.Stop();
        _audioSource.volume = 0f;
    }
}
