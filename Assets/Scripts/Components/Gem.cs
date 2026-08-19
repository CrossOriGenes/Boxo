using UnityEngine;
using System;

public class Gem : MonoBehaviour
{
    public static event Action OnGemCollected;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            AudioManager.Instance.PlaySFXAtPosition(
                SFXType.Pickup,
                transform.position
            );

            OnGemCollected?.Invoke();
            
            Destroy(gameObject);
        }
    }
}
