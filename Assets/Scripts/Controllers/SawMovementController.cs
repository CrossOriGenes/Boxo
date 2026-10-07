using UnityEngine;

public class SawMovementController : MonoBehaviour
{
    [Header("Rail-movement Points")]
    [SerializeField] private Transform _pointA;
    [SerializeField] private Transform _pointB;

    [Header("Passcode")]
    [SerializeField] private string _connectorPasscode;

    [Header("Metrics")]
    [Range(1f, 5f)]
    [SerializeField] private float _maxMoveSpeed = 2.5f;
    [Range(1f, 10f)]
    [SerializeField] private float _moveAcceleration = 4f;
    [Range(1f, 10f)]
    [SerializeField] private float _moveDeceleration = 5f;
    [Range(1f, 1000f)]
    [SerializeField] private float _maxRotationSpeed = 720f;
    [Range(1f, 2000f)]
    [SerializeField] private float _rotationAcceleration = 1200f;
    [Range(1f, 2000f)]
    [SerializeField] private float _rotationDeceleration = 1500f;

    private float _currentMoveSpeed;
    public float CurrentMoveSpeed => _currentMoveSpeed;
    private float _currentRotationSpeed;
    private int _moveDirection = 1;
    public int MoveDirection => _moveDirection;
    private bool _isSawActive;
    public bool IsSawActive => _isSawActive;
    private bool _isStopping;
    public bool IsStopping => _isStopping;
    private Vector3 _initialPosition; 
    private Quaternion _initialBladeRotation;


    private void Awake()
    {
        _initialPosition = transform.position;
        Transform blade = transform.Find("Blade");
        if (blade != null)
            _initialBladeRotation = blade.localRotation;
    }
    
    private void OnEnable()
    {
        PressSwitch.SwitchPressed += HandleSawActivation;
        GameScreenOverlayUI.RestartLevel += ResetSaw;
    }

    private void OnDisable()
    {
        PressSwitch.SwitchPressed -= HandleSawActivation;
        GameScreenOverlayUI.RestartLevel -= ResetSaw;
    }

    private void Update()
    {
        HandleBladeRotation();
        HandleHorizontalMovement();
    }

    private void HandleSawActivation(string passcode)
    {
        if (passcode == _connectorPasscode)
            ActivateSaw();
    }

    private void ActivateSaw()
    {
        if (_isSawActive) return;

        _isSawActive = true;
        _isStopping = false;
    }

    private void HandleSawDeactivation(string passcode)
    {
        if (passcode == _connectorPasscode)
            DeactivateSaw();
    }

    private void DeactivateSaw()
    {
        if (!_isSawActive) return;

        _isStopping = true;
    }

    private void HandleBladeRotation()
    {
        float targetRotationSpeed = 
            _isSawActive && !_isStopping
            ? _maxRotationSpeed
            : 0f;
        float acceleration = 
            targetRotationSpeed > _currentRotationSpeed
            ? _rotationAcceleration
            : _rotationDeceleration;

        _currentRotationSpeed = 
            Mathf.MoveTowards(
                _currentRotationSpeed,
                targetRotationSpeed,
                acceleration * Time.deltaTime
            );

        if (_currentRotationSpeed > 0f)
        {
            Transform blade = transform.Find("Blade");
            
            if (blade != null)
                blade.Rotate(
                    0f,
                    0f,
                    -_currentRotationSpeed * Time.deltaTime
                );
        }
    }

    private void HandleHorizontalMovement()
    {
        if (_pointA == null || _pointB == null)
            return;

        float targetMoveSpeed = 
            _isSawActive && !_isStopping
            ? _maxMoveSpeed
            : 0f;
        float acceleration = 
            targetMoveSpeed > _currentMoveSpeed
            ? _moveAcceleration
            : _moveDeceleration;

        _currentMoveSpeed = 
            Mathf.MoveTowards(
                _currentMoveSpeed,
                targetMoveSpeed,
                acceleration * Time.deltaTime
            );
    
        if (_currentMoveSpeed <= 0f)
        {
            if (_isStopping &&
                _currentRotationSpeed <= 0f)
                _isSawActive = false;

            return;
        }

        float targetX = _moveDirection > 0
                            ? _pointB.position.x
                            : _pointA.position.x;
        Vector3 currentPosition = transform.position;
        float newX = 
            Mathf.MoveTowards(
                currentPosition.x,
                targetX,
                _currentMoveSpeed * Time.deltaTime
            );
        transform.position = 
            new Vector3(
                newX,
                currentPosition.y,
                currentPosition.z
            );
        bool reachedTarget = 
            Mathf.Approximately(newX, targetX);
        
        if (reachedTarget) _moveDirection *= -1;
    }

    private void ResetSaw()
    {
        _isSawActive = false;
        _isStopping = false;

        _currentMoveSpeed = 0f;
        _currentRotationSpeed = 0f;

        _moveDirection = 1;

        transform.position = _initialPosition;
        Transform blade = transform.Find("Blade");
        if (blade != null)
            blade.localRotation = _initialBladeRotation;
    }
}
