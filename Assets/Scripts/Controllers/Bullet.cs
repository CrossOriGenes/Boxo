using UnityEngine;

public class Bullet : MonoBehaviour
{
    private ProjectileData _projectileData;
    private float _lifeTimer;
    private Vector2 _direction;

    public void Initialize(
        ProjectileData projectileData,
        Vector2 direction
    )
    {
        _projectileData = projectileData;

        _direction = direction.normalized;

        float angle =
            Mathf.Atan2(
                _direction.y,
                _direction.x
            ) * Mathf.Rad2Deg;

        transform.rotation =
            Quaternion.Euler(
                0f,
                0f,
                angle - 90f
            );

        _lifeTimer = _projectileData.Lifetime;
    }

    private void Update()
    {
        transform.position +=
            (Vector3)(
                _direction * 
                _projectileData.Speed *
                Time.deltaTime
            );

        _lifeTimer -= Time.deltaTime;

        if (_lifeTimer <= 0f)
            gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            DamageController damageCtrlr =
                other.GetComponentInChildren<DamageController>();

            damageCtrlr.TakeDamage(
                _projectileData.Damage
            );

            gameObject.SetActive(false);

            return;
        }

        if (other.CompareTag("Boundary") ||
            other.CompareTag("Ground"))
        {
            gameObject.SetActive(false);
        }
    }
}