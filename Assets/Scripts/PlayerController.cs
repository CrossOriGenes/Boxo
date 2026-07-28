using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 20f;
    [SerializeField] private float deceleration = 25f;

    [Header("Visual")]
    [SerializeField] private Transform visual;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private GroundCollider groundDetector;

    private Rigidbody2D _rb;
    private Vector2 _moveInput;
    private bool _isFacingRight = true;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
        if ((_moveInput.x > 0 && !_isFacingRight) || 
        (_moveInput.x < 0 && _isFacingRight))
            Flip();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.performed || !groundDetector.IsGrounded) 
            return;
        
        _rb.linearVelocity = new Vector2(
            _rb.linearVelocity.x,
            jumpForce
        );
    }

    private void FixedUpdate()
    {
        HandleMovement();
    } 

    private void HandleMovement()
    {
        float _targetSpeed = _moveInput.x * moveSpeed;
        float _speedDifference = _targetSpeed - _rb.linearVelocity.x;
        float _accelerationRate = Mathf.Abs(_targetSpeed) > 0.01f ? acceleration : deceleration;
        float _movement = _speedDifference * _accelerationRate;
        _rb.AddForce(
            Vector2.right * _movement,
            ForceMode2D.Force
        );
    }

    private void Flip()
    {
        _isFacingRight = !_isFacingRight;
        visual.Rotate(0, 180, 0);
    }

}
