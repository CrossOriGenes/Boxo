using UnityEngine;

public class PlatformDetector : MonoBehaviour
{
    private HorizontalMovingPlatform _platform;

    private void Awake()
    {
        _platform = GetComponentInParent<HorizontalMovingPlatform>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _platform.SetPassenger(other.attachedRigidbody);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _platform.SetPassenger(null);
        }
    }
}
