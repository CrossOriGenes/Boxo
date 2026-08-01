using UnityEngine;

public class GroundCollider : MonoBehaviour
{
    public bool IsGrounded { get; set; }
    [SerializeField] private LayerMask groundLayers;
    [SerializeField] private ParticleSystem fallParticles;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ground") || 
        other.CompareTag("Boundary") ||
        other.CompareTag("Floating Platforms"))
        {
            fallParticles.Play();
            IsGrounded = true;    
        }     
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ground") || 
        other.CompareTag("Boundary") ||
        other.CompareTag("Floating Platforms"))
        {
            IsGrounded = false;
        }
    }
}
