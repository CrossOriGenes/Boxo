using UnityEngine;

public class PortalKey : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Key Collected");
            Destroy(gameObject);            
        }
    }
}
