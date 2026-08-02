using UnityEngine;

public class VerticalMovingPlatform : MonoBehaviour
{
    [Header("Waypoints")]
    [SerializeField] private Transform _pointA;
    [SerializeField] private Transform _pointB;

    [Header("Controls")]
    [Range(0f, 5f)]
    [SerializeField] private float _speed = 1.2f;

    private Rigidbody2D _rb, _passenger;
    private PlayerController _playerController;
    private bool _movingToB = true;
    private Vector2 _lastPosition;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _lastPosition = _rb.position;
    }

    private void FixedUpdate()
    {
        Transform target = _movingToB ? _pointB : _pointA;

        Vector2 targetPosition = new Vector2(
            _rb.position.x,
            target.position.y
        );
        Vector2 nextPosition = Vector2.MoveTowards(
            _rb.position,
            targetPosition,
            _speed * Time.fixedDeltaTime
        );
        _rb.MovePosition(nextPosition);

        Vector2 delta = nextPosition - _lastPosition;
        if (_passenger != null)
            if (!_playerController.IsMoving)
                _passenger.position += delta;
        _lastPosition = nextPosition;

        if (Mathf.Abs(_rb.position.y - target.position.y) < 0.01f)
            _movingToB = !_movingToB;
    }

    public void SetPassenger(Rigidbody2D passenger)
    {
        _passenger = passenger;

        if (_passenger != null)
            _playerController = _passenger.GetComponent<PlayerController>();
        else
            _playerController = null;
    }
}
