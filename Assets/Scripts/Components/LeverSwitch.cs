using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LeverSwitch : MonoBehaviour
{
    public static event Action<string> LeverPressed;

    [Header("References")]
    [SerializeField] private Transform _leverHandlePivot;
    [SerializeField] private Light2D _glowHighlight;
    [SerializeField] private InteractionPromptController _interactionPrompt;

    [Header("Passcode")]
    [SerializeField] private string _connectorPasscode;

    private bool _hasOperated;
    private bool _playerSteppedIn;
    private const float _LIGHT_MAX_INTENSITY = 10f;
    private const float LIGHT_FADING_DURATION = .5f;


    private void Awake()
    {
        _glowHighlight.intensity = 0f;
    }

    public void HandlePlayerStepInActions()
    {   
        if (_hasOperated) return;

        DOTween.To(
            () => _glowHighlight.intensity,
            value => _glowHighlight.intensity = value,
            _LIGHT_MAX_INTENSITY, 
            LIGHT_FADING_DURATION
        )
        .SetEase(Ease.InQuad)
        .OnComplete(
            () => {
                _playerSteppedIn = true;
                _interactionPrompt.ShowLeverSwitchInteractionPrompt();
            });
    }

    public void HandlePlayerStepOutActions()
    {   
        if (!_playerSteppedIn) return;

        _playerSteppedIn = false;

        DOTween.To(
            () => _glowHighlight.intensity,
            value => _glowHighlight.intensity = value,
            0f, 
            LIGHT_FADING_DURATION
        )
        .SetEase(Ease.OutQuad)
        .OnComplete(() =>
        {
            _interactionPrompt.HideLeverSwitchInteractionPrompt();
        });
    }
}
