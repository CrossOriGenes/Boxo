using DG.Tweening;
using UnityEngine.Rendering.Universal;
using UnityEngine;

public class VentInteractionController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Vent _vent;
    [SerializeField] private Light2D _activationGlowHighlight;
    [SerializeField] private InteractionPromptController _interactionPrompt;


    private bool _hasCameInRange;
    private const float TARGET_HIGHLIGHT_INTENSITY = 200f;
    
    private void Awake()
    {
        _activationGlowHighlight.intensity = 0f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_vent.HasActivated) return;
        if (!other.CompareTag("Player")) return;

        PlayerController playerController =
            other.GetComponent<PlayerController>();
        if (playerController == null) return;
        playerController.SetCurrentVent(_vent);

        DOTween.To(
            () => _activationGlowHighlight.intensity,
            value => _activationGlowHighlight.intensity = value,
            TARGET_HIGHLIGHT_INTENSITY,
            .35f
        )
        .SetEase(Ease.InCubic)
        .OnPlay(
            () => _interactionPrompt
                    .ShowVentInInteractionPrompt()
        )
        .OnComplete(
            () => _hasCameInRange = true
        );
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerController playerController = 
            other.GetComponent<PlayerController>();
        if (playerController != null)
            playerController.ClearCurrentVent(_vent);

        if (!_hasCameInRange) return;

        _hasCameInRange = false;

        DOTween.To(
            () => _activationGlowHighlight.intensity,
            value => _activationGlowHighlight.intensity = value,
            0f,
            .4f
        )
        .SetEase(Ease.InCubic)
        .OnPlay(
            () => _interactionPrompt
                    .HideInteractionPrompt()
        );
    }
}
