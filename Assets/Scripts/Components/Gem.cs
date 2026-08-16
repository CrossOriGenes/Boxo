using UnityEngine;

public class Gem : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            AudioManager.Instance.PlaySFXAtPosition(
                SFXType.Pickup,
                transform.position
            );

            // Gem collection for UI event here
            
            Destroy(gameObject);
        }
    }
}
