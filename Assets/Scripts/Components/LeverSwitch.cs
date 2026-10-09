using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;


[Serializable]
public class LeverTargetBinding
{
    [SerializeField] private MonoBehaviour _targetComponent;
    [SerializeField] private LeverTargetOperation _operation;

    public MonoBehaviour TargetComponent => _targetComponent;
    public LeverTargetOperation Operation => _operation;
}

public class LeverSwitch : MonoBehaviour, IInteractable
{
    public static event Action<string> LeverPressed;

    [Header("References")]
    [SerializeField] private Transform _leverHandlePivot;
    [SerializeField] private Light2D _glowHighlight;
    [SerializeField] private InteractionPromptController _interactionPrompt;

    [Header("Passcode")]
    [SerializeField] private string _connectorPasscode;

    [Header("Targets")]
    [SerializeField] private LeverTargetBinding[] _targets;

    private bool _hasOperated;
    public bool HasOperated => _hasOperated;
    private bool _playerSteppedIn;
    private const float _LIGHT_MAX_INTENSITY = 10f;
    private const float LIGHT_FADING_DURATION = .5f;
    private Quaternion _leverHandleInitialRotation;


    private void OnEnable()
    {
        GameScreenOverlayUI.RestartLevel += ResetLeverSwitch;    
    }

    private void OnDisable()
    {
        GameScreenOverlayUI.RestartLevel -= ResetLeverSwitch;
    }

    private void Awake()
    {
        _glowHighlight.intensity = 0f;
        _leverHandleInitialRotation = 
            _leverHandlePivot.localRotation;
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
        .OnPlay(
            () => _interactionPrompt.ShowLeverSwitchInteractionPrompt()
        )
        .OnComplete(
            () => _playerSteppedIn = true
        );
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
        .OnPlay(
            () => _interactionPrompt.HideLeverSwitchInteractionPrompt()
        );
    }

    public void Interact()
    {
        if (!_playerSteppedIn) return;
        if (_hasOperated) return;

        bool operatedAnyTarget = TryOperateTargets();
        if (!operatedAnyTarget) return;

        _leverHandlePivot
            .DORotate(
                new Vector3(0, 0, 45f), 
                .3f
            )
            .SetEase(Ease.OutQuad)
            .OnPlay(() =>
                AudioManager.Instance.PlaySFXAtPosition(
                    SFXType.LeverPress, 
                    transform.position
                )
            )
            .OnComplete(() =>
            {
                _hasOperated = true;
                _playerSteppedIn = false;
                _interactionPrompt.HideLeverSwitchInteractionPrompt();
                
                _glowHighlight.DOKill();
                DOTween.To(
                    () => _glowHighlight.intensity,
                    value => _glowHighlight.intensity = value,
                    0f, 
                    LIGHT_FADING_DURATION
                )
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                    LeverPressed?.Invoke(_connectorPasscode)
                );
            });
    }

    private bool TryOperateTargets()
    {
        if (_targets == null || 
          _targets.Length == 0)
        {
            Debug.LogWarning(
                $"Lever '{name}' has no targets assigned."
            );
            return false;
        }

        bool operatedAnyTarget = false;
        
        foreach (LeverTargetBinding binding in _targets)
        {
            if (binding == null) continue;

            MonoBehaviour targetComponent = binding.TargetComponent;
            if (targetComponent == null)
            {
                Debug.LogWarning(
                    $"Lever '{name}' has an empty target reference."
                );
                continue;
            }
            if (targetComponent is not ILeverTarget target)
            {
                Debug.LogWarning(
                    $"Target '{targetComponent.name}' on Lever '{name}' " +
                    "Does not implement ILeverTarget."
                );
                continue;
            }

            if (_connectorPasscode != target.Passcode)
            {
                Debug.LogWarning(
                    $"Passcode mismatch between Lever '{name}' " +
                    $"and target '{targetComponent.name}'."
                );
                continue;
            }

            switch (binding.Operation)
            {
                case LeverTargetOperation.Activate:
                    target.Activate();
                    break;

                case LeverTargetOperation.Deactivate:
                    target.Deactivate();
                    break;
            }

            operatedAnyTarget = true;
        }

        return operatedAnyTarget;
    }

    private void ResetLeverSwitch()
    {
        _hasOperated = false;
        _playerSteppedIn = false;

        _leverHandlePivot.DOKill();
        _glowHighlight.DOKill();

        _leverHandlePivot.localRotation = 
            _leverHandleInitialRotation;
        _glowHighlight.intensity = 0f;

        _interactionPrompt
            .HideLeverSwitchInteractionPrompt();
    }
}
