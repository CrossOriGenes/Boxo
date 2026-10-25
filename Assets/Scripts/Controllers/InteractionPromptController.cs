using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;


[Serializable]
public class PromptIconSet
{
    public PromptDeviceType deviceType;
    public PromptActionType actionType;
    public Sprite icon;
}

public class InteractionPromptController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text _textPart1; 
    [SerializeField] private TMP_Text _textPart2; 
    [SerializeField] private TMP_Text _textPart3; 
    [SerializeField] private TMP_Text _textPart4; 
    [SerializeField] private Image _buttonIconForKbd; 
    [SerializeField] private Image _buttonIconForGamepad; 

    [Header("Input")]
    [SerializeField] private PlayerInput _playerInput;

    [Header("Input Icons")]
    [SerializeField] private PromptIconSet[] _iconSets;

    private string _text1 = "",
                   _text2 = "",
                   _text3 = "",
                   _text4 = "";

    
    private void Awake()
    {
        _textPart1.text = _text1;
        _textPart2.text = _text2;
        _textPart3.text = _text3;
        _textPart4.text = _text4;
        _buttonIconForKbd.sprite = null;
        _buttonIconForGamepad.sprite = null;
        gameObject.SetActive(false);
    }

    /* 
    private PromptDeviceType DetectDeviceType()
    {
        if (_playerInput == null)
            return PromptDeviceType.Keyboard;

        InputDevice device = null;

        if (_playerInput.currentControlScheme == "Gamepad")
            device = Gamepad.current;

        if (device == null)
            return PromptDeviceType.Keyboard;

        string product =
            device.description.product?.ToLowerInvariant() ?? "";
        string manufacturer =
            device.description.manufacturer?.ToLowerInvariant() ?? "";
        string deviceName =
            device.displayName?.ToLowerInvariant() ?? "";
        string combined =
            $"{product} {manufacturer} {deviceName}";

        if (combined.Contains("dualsense") ||
        combined.Contains("dualshock") ||
        combined.Contains("playstation"))
            return PromptDeviceType.PlayStation;

        if (combined.Contains("switch") ||
        combined.Contains("nintendo"))
            return PromptDeviceType.Nintendo;

        if (combined.Contains("xbox"))
            return PromptDeviceType.Xbox;

        return PromptDeviceType.GenericGamepad;
    } 
    */

    private Sprite GetCurrentDeviceIcon(
        PromptActionType actionType,
        PromptDeviceType deviceType
    )
    {
        foreach (PromptIconSet iconSet in _iconSets)
        {
            if (iconSet.deviceType == deviceType &&
            iconSet.actionType == actionType)
                return iconSet.icon;
        }

        return null;
    }

    public void ShowLeverSwitchInteractionPrompt()
    {
        _text1 = "Press";
        _text2 = "key of keyboard or";
        _text3 = "of gamepad";
        _text4 = "to switch Lever";
        _textPart1.text = _text1;
        _textPart2.text = _text2;
        _textPart3.text = _text3;
        _textPart4.text = _text4;
        _buttonIconForKbd.sprite = GetCurrentDeviceIcon(
            PromptActionType.Lever,
            PromptDeviceType.Keyboard
        );
        _buttonIconForGamepad.sprite = 
            GetCurrentDeviceIcon(
                PromptActionType.Lever,
                PromptDeviceType.GenericGamepad
            );

        gameObject.SetActive(true);
    }

    public void ShowVentInInteractionPrompt()
    {
        _text1 = "Press";
        _text2 = "key of keyboard or";
        _text3 = "of gamepad";
        _text4 = "to enter Vent";
        _textPart1.text = _text1;
        _textPart2.text = _text2;
        _textPart3.text = _text3;
        _textPart4.text = _text4;
        _buttonIconForKbd.sprite = 
            GetCurrentDeviceIcon(
                PromptActionType.VentIn,
                PromptDeviceType.Keyboard
            );
        _buttonIconForGamepad.sprite = 
            GetCurrentDeviceIcon(
                PromptActionType.VentIn,
                PromptDeviceType.GenericGamepad
            );

        gameObject.SetActive(true);
    }
    public void ShowVentOutInteractionPrompt()
    {
        _text1 = "Press";
        _text2 = "key of keyboard or";
        _text3 = "of gamepad";
        _text4 = "to exit Vent";
        _textPart1.text = _text1;
        _textPart2.text = _text2;
        _textPart3.text = _text3;
        _textPart4.text = _text4;
        _buttonIconForKbd.sprite = 
            GetCurrentDeviceIcon(
                PromptActionType.VentOut,
                PromptDeviceType.Keyboard
            );
        _buttonIconForGamepad.sprite = 
            GetCurrentDeviceIcon(
                PromptActionType.VentOut,
                PromptDeviceType.GenericGamepad
            );

        gameObject.SetActive(true);
    }

    public void HideInteractionPrompt()
    {
        _text1 = _text2 = _text3 = _text4 = "";
        _textPart1.text = _text1;
        _textPart2.text = _text2;
        _textPart3.text = _text3;
        _textPart4.text = _text4;
        _buttonIconForKbd.sprite = null;
        _buttonIconForGamepad.sprite = null;
        
        gameObject.SetActive(false);
    }
}
