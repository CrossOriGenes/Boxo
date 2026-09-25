using System;
using UnityEngine;

public class LaserTurretController : MonoBehaviour
{
    private static readonly int IsFiringHash = 
        Animator.StringToHash("isFiringLaser");

    [Header("References")]
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _headPivot;
    [SerializeField] private Transform _muzzlePoint;
    [SerializeField] private TurretFiringAudio _firingAudio;
    [SerializeField] private ProjectilePooler _projectilePool; 

    [Header("Detection")]
    [Range(1, 50)]
    [SerializeField] private float _detectionRange = 12f;
    [SerializeField] private LayerMask _playerLayer;

    [Header("Rotation")]
    [Range(100, 300)]
    [SerializeField] private float _rotationSpeed = 180f;
    [Range(0, 360)]
    [SerializeField] private float _arcAngle = 180f;

    [Header("Firing")]
    [SerializeField] private ProjectileData _projectileData;

    private float _centerAngle;
    private float _barrelForwardAngle;
    private float _fireTimer;
    private bool _playerDetected,
                _firingTriggerActivated,
                _playerInRange;
    private Animator _fireEmitAnimator;


    private void Awake()
    {
        _centerAngle = _headPivot.localEulerAngles.z;
        Vector2 localBarrelDirection = 
            _muzzlePoint.localPosition.normalized;
        _barrelForwardAngle = 
            Mathf.Atan2(
                localBarrelDirection.y,
                localBarrelDirection.x
            ) * Mathf.Rad2Deg;
        _fireEmitAnimator = 
            _muzzlePoint
            .gameObject
            .GetComponent<Animator>();
    } 

    private void OnEnable()
    {
        GameScreenOverlayUI.RestartLevel += ResetTurret;
    }

    private void OnDisable()
    {
        GameScreenOverlayUI.RestartLevel -= ResetTurret;
    }

    private void Update()
    {
        CheckPlayerRange();

        if (_playerDetected)
            RotateHeadTowardsPLayer();

        if (!_playerInRange) 
            return;

        HandleFiring();
    }

    private void CheckPlayerRange()
    {
        Collider2D playerCollider =
            Physics2D.OverlapCircle(
                transform.position,
                _detectionRange,
                _playerLayer
            );

        bool playerDetected = playerCollider != null;

        if (playerDetected && !_playerDetected)
        {
            _playerDetected = true;
            
            if (_firingTriggerActivated)
                StartFiring();

            return;   
        }

        if (playerDetected) return;
        if(!_playerDetected) return;

        _playerDetected = false;

        StopFiring();

        _firingTriggerActivated = false;
    }

    public void ActivateTurret()
    {
        _firingTriggerActivated = true;

        if (_playerDetected)
            StartFiring();
    }

    private void StartFiring()
    {
        if (_playerInRange)
            return;

        _playerInRange = true;

        _fireEmitAnimator.SetBool(
            IsFiringHash,
            true
        );

        _firingAudio.StartFiringAudio();
    }

    private void StopFiring()
    {
        if (!_playerInRange)
            return;

        _playerInRange = false;

        _fireEmitAnimator.SetBool(
            IsFiringHash,
            false
        );

        _firingAudio.StopFiringAudio();
    }

    private void RotateHeadTowardsPLayer()
    {
        Transform parent = _headPivot.parent;
        Vector2 playerDirection;
        if (parent != null)
            playerDirection =
                parent.InverseTransformPoint(
                    _player.position
                ) - _headPivot.localPosition;
        else
            playerDirection = 
                _player.position - _headPivot.position;

        if (playerDirection.sqrMagnitude < .0001f)
            return;

        float playerAngle = 
            Mathf.Atan2(
                playerDirection.y,
                playerDirection.x
            ) * Mathf.Rad2Deg;
        float targetAngle = playerAngle - _barrelForwardAngle;
        float deltaFromCenter = 
            Mathf.DeltaAngle(
                _centerAngle,
                targetAngle
            );
        float halfArc = _arcAngle * .5f;
        deltaFromCenter = 
            Mathf.Clamp(
                deltaFromCenter,
                -halfArc,
                halfArc
            );
        float clampedTargetAngle = 
            _centerAngle + deltaFromCenter;
        float currentAngle = _headPivot.localEulerAngles.z; 
        float newAngle = 
            Mathf.MoveTowardsAngle(
                currentAngle,
                clampedTargetAngle,
                _rotationSpeed * Time.deltaTime
            );
        _headPivot.localRotation = 
            Quaternion.Euler(
                0f,
                0f,
                newAngle
            );
    }

    private void HandleFiring()
    {
        _fireTimer -= Time.deltaTime;

        if (_fireTimer > 0f) return;

        FireProjectile();

        _fireTimer = _projectileData.FireRate;
    }

    private void FireProjectile()
    {
        GameObject projectile = 
            _projectilePool.GetProjectile(
                _projectileData
            );

        if (projectile == null) 
            return;
        
        projectile.transform.position = _muzzlePoint.position;

        Bullet bullet = projectile.GetComponent<Bullet>();
        bullet.Initialize(
            _projectileData,
            _muzzlePoint.up
        );

        projectile.SetActive(true);
    } 

    private void ResetTurret()
    {
        _headPivot.localRotation =
            Quaternion.Euler(
                0f,
                0f,
                _centerAngle
            );

        _playerDetected = false;
        _firingTriggerActivated = false;
        _playerInRange = false;
        
        _fireTimer = 0f;
        
        _fireEmitAnimator.SetBool(
            IsFiringHash,
            false
        );

        _firingAudio.StopFiringAudio();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(
            transform.position,
            _detectionRange
        );
    }
}
