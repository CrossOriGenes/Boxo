using System.Collections;
using UnityEngine;

public class ElectricCoilController : MonoBehaviour
{
    private static readonly int IsEmittingShockHash =
        Animator.StringToHash("isEmittingShock");
    private static readonly int IsGettingShockHash =
        Animator.StringToHash("isGettingShock");

    [Header("References")]
    [SerializeField] private Transform _shockReleasePoint;
    [SerializeField] private CircleCollider2D _detectionRange;
    [SerializeField] private Animator _orbAnimator;
    [SerializeField] private LineRenderer _shockLine;
    
    [Header("Shock Animation")]
    [SerializeField] private Material _shockMaterial1;
    [SerializeField] private Material _shockMaterial2;

    [Header("Player")]
    [SerializeField] private GameObject _player;
    [SerializeField] private Sprite _playerShockFace;
    private DamageController _damageController;
    private Animator _playerVisualAnimator;
    private Transform _playerPos;

    [Header("Metrics")]
    [Range(1, 12)]
    [SerializeField] private float _coilDetectionRangeRadius = 3.5f;
    [Range(1, 30)]
    [SerializeField] private float _shockAnimationFPS = 12f;
    [SerializeField] private float _damagePerSecond = 3f;

    private bool _isPlayerInRange;
    private float _shockAnimationTimer;
    private bool _shockAnimationFrame;
    private Material _runtimeShockMaterial;
    private Coroutine _restoreFaceCoroutine;


    private void Awake()
    {
        _playerPos = _player.transform;
        _damageController = 
            _player.GetComponentInChildren<DamageController>();
        _playerVisualAnimator = 
            _player.transform
            .Find("Visual")
            .GetComponent<Animator>();
        
        _shockLine.enabled = false;
        _runtimeShockMaterial = new Material(_shockMaterial1);
        _shockLine.material = _runtimeShockMaterial;

        _detectionRange.radius = _coilDetectionRangeRadius;
        
        _orbAnimator.SetBool(
            IsEmittingShockHash,
            false
        );
        _playerVisualAnimator.SetBool(
            IsGettingShockHash,
            false
        ); 
    }

    private void Update()
    {
        if (_player == null) return;

        CheckPlayerRange();

        if (_isPlayerInRange)
        {
            UpdateShockLine();
            UpdateShockAnimation();
        }
    }

    private void CheckPlayerRange()
    {
        Vector2 coilPosition = 
            _detectionRange.transform.position;

        float radius = 
            _detectionRange.radius * Mathf.Max(
                _detectionRange.transform.lossyScale.x,
                _detectionRange.transform.lossyScale.y
            );
        float distance = 
            Vector2.Distance(
                coilPosition,
                _playerPos.position
            );

        bool playerDetected = distance <= radius;
        if (playerDetected == _isPlayerInRange) 
            return;
        _isPlayerInRange = playerDetected;

        if (_isPlayerInRange) StartShock();
        else StopShock();
    }

    private void StartShock()
    {
        _shockAnimationTimer = 0f;
        _shockAnimationFrame = false;

        _runtimeShockMaterial.mainTexture =
            _shockMaterial1.mainTexture;
        
        _orbAnimator.SetBool(
            IsEmittingShockHash,
            true    
        );
        _playerVisualAnimator.SetBool(
            IsGettingShockHash,
            true
        );
        
        _shockLine.enabled = true;
        UpdateShockLine();

        _damageController.SetShockFace(
            _playerShockFace
        );
        _damageController.StartContinuousDamage(
            _damagePerSecond
        );
    }

    private void StopShock()
    {
        _orbAnimator.SetBool(
            IsEmittingShockHash,
            false
        );
        _playerVisualAnimator.SetBool(
            IsGettingShockHash,
            false
        );

        _shockLine.enabled = false;
        _shockAnimationTimer = 0f;
        _shockAnimationFrame = false;
        _runtimeShockMaterial.mainTexture =
            _shockMaterial1.mainTexture;
    
        _damageController.StopContinuousDamage();
    
        if (_restoreFaceCoroutine != null)
            StopCoroutine(_restoreFaceCoroutine);
        _restoreFaceCoroutine = 
            StartCoroutine(RestorePlayerFaceAfterShock());
    }

    private IEnumerator RestorePlayerFaceAfterShock()
    {
        while (
            _playerVisualAnimator.IsInTransition(0) ||
            _playerVisualAnimator
                .GetCurrentAnimatorStateInfo(0)
                .IsName("GettingElectricShock")
        )
        {
            yield return null;
        }

        yield return null;

        _damageController.RefreshFaceByHealth();

        _restoreFaceCoroutine = null;
    }

    private void UpdateShockLine()
    {
        _shockLine.positionCount = 2;
        
        _shockLine.SetPosition(
            0,
            _shockReleasePoint.position
        );
        _shockLine.SetPosition(
            1,
            _playerPos.position
        );
    }

    private void UpdateShockAnimation()
    {
        _shockAnimationTimer -= Time.deltaTime;

        if (_shockAnimationTimer > 0f) 
            return;

        _shockAnimationTimer = 
            1f / _shockAnimationFPS;
        _shockAnimationFrame = !_shockAnimationFrame;

        _runtimeShockMaterial.mainTexture = 
            _shockAnimationFrame 
                ? _shockMaterial2.mainTexture
                : _shockMaterial1.mainTexture;

    }

    private void OnDrawGizmosSelected()
    {
        if (_detectionRange == null)
            return;

        Gizmos.DrawWireSphere(
            _detectionRange.transform.position,
            _coilDetectionRangeRadius
        );
    }
}
