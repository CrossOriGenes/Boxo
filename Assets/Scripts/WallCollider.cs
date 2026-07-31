using UnityEngine;

public class WallCollider : MonoBehaviour
{
    [SerializeField] private ParticleSystem touchParticles;

    private bool _hasTouchedWall;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_hasTouchedWall && 
        (other.CompareTag("Ground") || other.CompareTag("Boundary")))
        {
            touchParticles.Play();
            _hasTouchedWall = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (_hasTouchedWall && 
        (other.CompareTag("Ground") || other.CompareTag("Boundary")))
        {
            _hasTouchedWall = false;
        }
    }
}
