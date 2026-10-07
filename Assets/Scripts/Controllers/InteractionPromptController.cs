using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[Serializable]
public class PromptIconSet
{
    public PromptDeviceType deviceType;
    public Sprite icon;
}

public class InteractionPromptController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text _textPart1; 
    [SerializeField] private TMP_Text _textPart2; 
    [SerializeField] private Image _buttonIcon; 

    [Header("Input")]
    [SerializeField] private PlayerInput _playerInput;

    [Header("Input Icons")]
    [SerializeField] private PromptIconSet[] _iconSets;

    private string _text1 = "",
                   _text2 = "";

    
    private void Awake()
    {
        _textPart1.text = _text1;
        _textPart2.text = _text2;
        _buttonIcon.sprite = null;
        gameObject.SetActive(false);
    }

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

    private Sprite GetCurrentDeviceIcon()
    {
        PromptDeviceType deviceType =
            DetectDeviceType();

        foreach (PromptIconSet iconSet in _iconSets)
        {
            if (iconSet.deviceType == deviceType)
                return iconSet.icon;
        }

        return null;
    }

    public void ShowLeverSwitchInteractionPrompt()
    {
        _text1 = "Press";
        _text2 = "to switch Lever";
        _textPart1.text = _text1;
        _textPart2.text = _text2;
        _buttonIcon.sprite = GetCurrentDeviceIcon();

        gameObject.SetActive(true);
    }

    public void HideLeverSwitchInteractionPrompt()
    {
        _text1 = _text2 = "";
        _textPart1.text = _text1;
        _textPart2.text = _text2;
        _buttonIcon.sprite = null;
        
        gameObject.SetActive(false);
    }
}
