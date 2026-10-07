using System;
using DG.Tweening;
using UnityEngine;

public class PressSwitch : MonoBehaviour
{
    public static event Action<string> SwitchPressed;

    [Header("References")]
    [SerializeField] private Transform _switchVisualPos;

    [Header("Passcode")]
    [SerializeField] private string _connectorPasscode;
    

    private bool _isPressed;
    private Vector3 _initialVisualLocalPosition;


    private void OnEnable()
    {
        GameScreenOverlayUI.RestartLevel += ResetSwitch;    
    }
    
    private void OnDisable()
    {
        GameScreenOverlayUI.RestartLevel -= ResetSwitch;    
    }

    private void Awake()
    {
        _initialVisualLocalPosition =   
            _switchVisualPos.localPosition;    
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || _isPressed)
            return;

        _isPressed = true;

        _switchVisualPos
            .DOLocalMoveY(-0.1f, .4f)
            .SetEase(Ease.InQuad)
            .OnPlay(
                () =>
                AudioManager.Instance.PlaySFXAtPosition(
                    SFXType.SwitchPress,
                    transform.position
                )
            )
            .OnComplete(() =>     
                SwitchPressed?.Invoke(_connectorPasscode)
            );
    }

    private void ResetSwitch()
    {
        _isPressed = false;

        _switchVisualPos.DOKill();
        _switchVisualPos.localPosition = 
            _initialVisualLocalPosition;
    }
}
