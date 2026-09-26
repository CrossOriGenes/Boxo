using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DangerWarningPost : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Light2D _globalLight;

    [Header("Warning Metrics")]
    [SerializeField] private float _slowMotionScale = .25f;
    [SerializeField] private float _warningDuration = 2f;
    [SerializeField] private float _warningLightIntensity = .025f;
    [SerializeField] private float _warningPlayerSpotlightOuterRadius = 2.4f; 

    private float _normalLightIntensity;
    private float _normalSpotlightOuterRadius;
    private Light2D _playerSpotlight;
    private bool _hasTriggered;
    private Tween _lightTween;

    private void Awake()
    {
        if (_globalLight != null)
            _normalLightIntensity = _globalLight.intensity;
    }

    private void OnEnable()
    {
        GameScreenOverlayUI.RestartLevel += ResetWarning;
    }

    private void OnDisable()
    {
        GameScreenOverlayUI.RestartLevel -= ResetWarning;
        
        _lightTween?.Kill();
        RestoreLights();
        Time.timeScale = 1f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_hasTriggered) return;
        if (!other.CompareTag("Player")) return;
    
        _hasTriggered = true;
        _playerSpotlight = 
            other.GetComponentInChildren<Light2D>();

        if (_playerSpotlight != null)
            _normalSpotlightOuterRadius = 
                _playerSpotlight.pointLightOuterRadius;

        StartWarning();
    }

    private void StartWarning()
    {
        Time.timeScale = _slowMotionScale;
        AudioManager.Instance.PauseAudio();

        _lightTween?.Kill();

        Sequence warningSequence = 
            DOTween.Sequence().SetUpdate(true);

        float halfDuration = _warningDuration * .5f;

        if (_globalLight != null)
        {
            Tween globalLightTween = 
                DOTween.To(
                    () => _globalLight.intensity,
                    value => _globalLight.intensity = value,
                    _warningLightIntensity,
                    halfDuration
                )
                .SetEase(Ease.InOutSine);

            warningSequence.Append(globalLightTween);
        }
        if (_playerSpotlight != null)
        {
            Tween playerSpotlightTween = 
                DOTween.To(
                    () => _playerSpotlight.pointLightOuterRadius,
                    value => _playerSpotlight.pointLightOuterRadius = value,
                    _warningPlayerSpotlightOuterRadius,
                    halfDuration
                )
                .SetEase(Ease.InOutSine);

            warningSequence.Join(playerSpotlightTween);   
        }
        if (_globalLight != null)
        {
            Tween globalLightReturnTween = 
                DOTween.To(
                    () => _globalLight.intensity,
                    value => _globalLight.intensity = value,
                    _normalLightIntensity,
                    halfDuration
                )
                .SetEase(Ease.InOutSine);

            warningSequence.Append(globalLightReturnTween);
        }
        if (_playerSpotlight != null)
        {
            Tween playerSpotlightReturnTween = 
                DOTween.To(
                    () => _playerSpotlight.pointLightOuterRadius,
                    value => _playerSpotlight.pointLightOuterRadius = value,
                    _normalSpotlightOuterRadius,
                    halfDuration
                )
                .SetEase(Ease.InOutSine);

            warningSequence.Join(playerSpotlightReturnTween);   
        }

        _lightTween = warningSequence
                        .OnComplete(EndWarning);
    }

    private void EndWarning()
    {
        RestoreLights();
        Time.timeScale = 1f;
        AudioManager.Instance.ResumeAudio(); 
    }

    private void ResetWarning()
    {
        _lightTween?.Kill();

        if (_globalLight != null)
            _globalLight.intensity = _normalLightIntensity;
        
        Time.timeScale = 1f;

        _hasTriggered = false;
    }

    private void RestoreLights()
    {
        if (_globalLight != null)
            _globalLight.intensity = _normalLightIntensity;
    
        if (_playerSpotlight != null)
            _playerSpotlight.pointLightOuterRadius =
                _normalSpotlightOuterRadius; 
    }
}
