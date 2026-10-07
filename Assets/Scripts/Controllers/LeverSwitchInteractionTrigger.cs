using UnityEngine;

public class LeverSwitchInteractionTrigger : MonoBehaviour
{
    private LeverSwitch _leverSwitch;

    private void Awake()
    {
        _leverSwitch = 
            GetComponentInParent<LeverSwitch>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        _leverSwitch.HandlePlayerStepInActions();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        _leverSwitch.HandlePlayerStepOutActions();
    }
}
