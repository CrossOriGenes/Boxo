using UnityEngine;

public class HorizontalMovingBlocks : MonoBehaviour
{
    [Header("Block-1 Path Endpoints")]
    [SerializeField] private Transform _pointA1;
    [SerializeField] private Transform _pointB1;

    [Header("Block-2 Path Endpoints")]
    [SerializeField] private Transform _pointA2;
    [SerializeField] private Transform _pointB2;

    [Header("Blocks")]
    [SerializeField] private Rigidbody2D _block1Rb;
    [SerializeField] private Rigidbody2D _block2Rb;

    [Header("Controls")]
    [Range(0f, 5f)]
    [SerializeField] private float _speed = 3f;

    private bool _movingToB1 = true;
    private bool _movingToB2 = true;

    private void FixedUpdate()
    {
        _movingToB1 = MoveBlock(
            _movingToB1,
            _pointA1,
            _pointB1,
            _block1Rb
        );

        _movingToB2 = MoveBlock(
            _movingToB2,
            _pointA2,
            _pointB2,
            _block2Rb
        );
    }

    private bool MoveBlock(
        bool movingToB,
        Transform pointA,
        Transform pointB,
        Rigidbody2D rb
    )
    {
        Transform target = movingToB ? pointB : pointA;

        Vector2 targetPosition = new(
            target.position.x,
            rb.position.y
        );

        Vector2 nextPosition = Vector2.MoveTowards(
            rb.position,
            targetPosition,
            _speed * Time.fixedDeltaTime
        );

        rb.MovePosition(nextPosition);
            
        if (Mathf.Abs(rb.position.x - target.position.x) < 0.01f)
            movingToB = !movingToB;

        return movingToB;
    }
}
