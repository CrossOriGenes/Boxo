using UnityEngine;

public class BladeDamageController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SawMovementController _sawMovementController;
    [SerializeField] private ParticleSystem _leftHitFlicker;
    [SerializeField] private ParticleSystem _rightHitFlicker;

    [Header("Metrics")]
    [SerializeField] private float _damage = 25f;
    [SerializeField] private float _knockbackForce = 7f;
    [SerializeField] private float _verticalKnockbackForce = 3f;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        DamageController damageController =
            other.GetComponentInChildren<DamageController>();

        if (damageController == null) 
            return;
        
        if (_sawMovementController.MoveDirection > 0)
            _rightHitFlicker.Play();
        else
            _leftHitFlicker.Play();

        damageController.TakeDamage(_damage);

        ApplyKnockback(other);
    }

    private void ApplyKnockback(Collider2D playerCollider)
    {
        PlayerController playerController = 
            playerCollider.GetComponent<PlayerController>();
        if (playerController == null)
            playerController =
                playerCollider.GetComponentInParent<PlayerController>();
        if (playerController == null) return;

        int sawDirection = 
            _sawMovementController != null
                ? _sawMovementController.MoveDirection
                : 1;
        Vector2 knockbackForce = 
            new (
                sawDirection * _knockbackForce,
                _verticalKnockbackForce
            );
        playerController.ApplyKnockback(
            knockbackForce,
            .25f
        );
    }
}
