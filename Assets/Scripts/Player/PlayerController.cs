using System.Collections;
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
    private bool _canControl = true;
    private bool _isKnockedBack;

    private IInteractable _currentInteractable;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!_canControl || _isKnockedBack) 
            return;

        _moveInput = context.ReadValue<Vector2>();

        if ((_moveInput.x > 0 && !_isFacingRight) || 
          (_moveInput.x < 0 && _isFacingRight))
            Flip();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.performed || 
          !groundDetector.IsGrounded || 
          !_canControl ||
          _isKnockedBack) 
            return;
        
        _rb.linearVelocity = new Vector2(
            _rb.linearVelocity.x,
            jumpForce
        );
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed ||
        !_canControl ||
        _isKnockedBack)
            return;

        _currentInteractable?.Interact();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    } 

    private void HandleMovement()
    {
        if (_isKnockedBack) return;

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

    public void SetControlsEnabled(bool value)
    {
        _canControl = value;
    } 

    public void ApplyKnockback(
        Vector2 force,
        float duration
    )
    {
        StopAllCoroutines();

        StartCoroutine(
            KnockbackRoutine(force, duration)
        );
    }

    private IEnumerator KnockbackRoutine(
        Vector2 force,
        float duration
    )
    {
        _isKnockedBack = true;
        _moveInput = Vector2.zero;

        _rb.linearVelocity = force;

        yield return new WaitForSeconds(duration);

        _isKnockedBack = false;
    }

    public void SetCurrentInteractable(
        IInteractable interactable
    )
    {
        _currentInteractable = interactable;
    }

    public void ClearCurrentInteractable(
        IInteractable interactable
    )
    {
        if (_currentInteractable == interactable)
            _currentInteractable = null;
    }
}
