using UnityEngine;

public class PlatformDetector2 : MonoBehaviour
{
    private VerticalMovingPlatform _platform;

    private void Awake()
    {
        _platform = GetComponentInParent<VerticalMovingPlatform>();
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
