using UnityEngine;

public class GroundCollider : MonoBehaviour
{
    public bool IsGrounded { get; set; }
    [SerializeField] private LayerMask groundLayers;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ground") || other.CompareTag("Boundary"))
        {
            IsGrounded = true;    
        }     
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ground") || other.CompareTag("Boundary"))
        {
            IsGrounded = false;
        }
    }
}
