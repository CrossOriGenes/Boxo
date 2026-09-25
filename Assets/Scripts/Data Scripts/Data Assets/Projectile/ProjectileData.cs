using UnityEngine;

[CreateAssetMenu(
    fileName = "ProjectileData", 
    menuName = "Game/Projectile Data"
)]
public class ProjectileData : ScriptableObject
{
    [Header("Projectile")]
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private float _damage;
    [SerializeField] private float _speed;
    [SerializeField] private float _lifetime;
    [SerializeField] private float _fireRate;

    public GameObject ProjectilePrefab => _projectilePrefab;
    public float Damage => _damage;
    public float Speed => _speed;
    public float Lifetime => _lifetime;
    public float FireRate => _fireRate;
}
