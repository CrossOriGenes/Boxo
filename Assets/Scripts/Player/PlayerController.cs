using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

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
        _rb.linearVelocity = new Vector2(
            _targetSpeed,
            _rb.linearVelocityY
        );
    }

    private void Flip()
    {
        _isFacingRight = !_isFacingRight;
        visual.Rotate(0, 180, 0);
    }
    
    public bool IsMoving => Mathf.Abs(_moveInput.x) > 0.01f;

    public void ResetMovement()
    {
        _moveInput = Vector2.zero;
    }
}
