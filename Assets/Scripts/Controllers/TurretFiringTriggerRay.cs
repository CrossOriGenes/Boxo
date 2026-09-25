using UnityEngine;

public class TurretFiringTriggerRay : MonoBehaviour
{
    [Header("Ray")]
    [SerializeField] private TriggeringRayType _rayType;
    [SerializeField] private float _maxDistance = 30f;

    [Header("Blocking")]
    [SerializeField] private LayerMask _blockingLayers;

    [Header("Line")]
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private EdgeCollider2D _edgeCollider;

    private LaserTurretController _turretController;

    private void Awake()
    {
        _turretController =
            GetComponentInParent<LaserTurretController>();

        CreateTriggerLine();
    }

    private void CreateTriggerLine()
    {
        Vector2 start = transform.position;

        _lineRenderer.useWorldSpace = true;
        _lineRenderer.positionCount = 2;

        if (_rayType == TriggeringRayType.Horizontal)
            CreateHorizontalLine(start);
        else 
            CreateVerticalLine(start);
    }

    private void CreateHorizontalLine(Vector2 start)
    {
        Vector2 leftDirection = 
            transform.TransformDirection(Vector2.left);
        Vector2 rightDirection = 
            transform.TransformDirection(Vector2.right);
        Vector2 leftEnd = GetRayEnd(start, leftDirection);
        Vector2 rightEnd = GetRayEnd(start, rightDirection);

        _lineRenderer.SetPosition(0, leftEnd);
        _lineRenderer.SetPosition(1, rightEnd);
    
        SetColliderLine(leftEnd, rightEnd);
    }

    private void CreateVerticalLine(Vector2 start)
    {
        Vector2 direction = 
            transform.TransformDirection(Vector2.up);
        Vector2 end = GetRayEnd(start, direction);

        _lineRenderer.SetPosition(0, start);
        _lineRenderer.SetPosition(1, end);

        SetColliderLine(start, end);
    }

    private Vector2 GetRayEnd(
        Vector2 start,
        Vector2 direction
    )
    {
        RaycastHit2D[] hits = 
            Physics2D.RaycastAll(
                start,
                direction,
                _maxDistance,
                _blockingLayers
            );

        float closestDistance = float.MaxValue;
        Vector2 closestPoint = start + direction * _maxDistance;

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.isTrigger)
                continue;

            if (hit.distance < closestDistance)
            {
                closestDistance = hit.distance;
                closestPoint = hit.point;
            }
        }

        return closestPoint;
    }

    private void SetColliderLine(
        Vector2 worldStart,
        Vector2 worldEnd
    )
    {
        Vector2 localStart = 
            transform.InverseTransformPoint(worldStart);
        Vector2 localEnd = 
            transform.InverseTransformPoint(worldEnd);

        _edgeCollider.points = 
            new Vector2[]{ 
                localStart, 
                localEnd 
            };
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        _turretController.ActivateTurret();
    }
}
