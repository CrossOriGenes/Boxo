using UnityEngine;

public class LeverSwitchInteractionTrigger : MonoBehaviour
{
    private LeverSwitch _leverSwitch;
    private PlayerController _playerController;

    private void Awake()
    {
        _leverSwitch = 
            GetComponentInParent<LeverSwitch>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        _playerController = 
            other.GetComponent<PlayerController>();
        if (_playerController == null) return;

        if (_leverSwitch.HasOperated) return;

        _leverSwitch.HandlePlayerStepInActions();
        
        _playerController.SetCurrentInteractable(
            _leverSwitch
        );
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (_playerController == null) return;

        _leverSwitch.HandlePlayerStepOutActions();
    
        _playerController.ClearCurrentInteractable(
            _leverSwitch
        );
    }
}
