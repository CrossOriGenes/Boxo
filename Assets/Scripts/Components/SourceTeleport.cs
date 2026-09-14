using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SourceTeleport : MonoBehaviour
{
    [Header("")]
    [SerializeField] private GameObject _player;
    [SerializeField] private int _destinationIndex = 1;

    // Local References
    public static event Action<int> Teleported;
    private Transform _teleportingPoint;
    private bool _hasCrossedCheckpoint;
    public bool CrossedCheckpoint
    {
        get => _hasCrossedCheckpoint;
        set => _hasCrossedCheckpoint = value;
    } 
    private bool _hasEntered;

    // Player references
    private Rigidbody2D _rb;
    private PlayerController _playerController;
    private Light2D _playerLight;
    private TrailRenderer _trail;

    private void Awake()
    {
        _teleportingPoint = gameObject.transform.Find("Teleporting_point");

        InitiatePlayerComponents();
    }

    private void OnEnable()
    {
        Checkpoint.CheckpointCrossed += HandleIsCheckpointCrossed;
        GameScreenOverlayUI.RestartLevel += ResetTeleportStats;
    }

    private void OnDisable()
    {
        Checkpoint.CheckpointCrossed += HandleIsCheckpointCrossed;
        GameScreenOverlayUI.RestartLevel -= ResetTeleportStats;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || 
        !CrossedCheckpoint ||
        _hasEntered)
            return;

        _hasEntered = true;

        StartTeleportSequence();
    }

    private void StartTeleportSequence()
    {
        Sequence _teleportSequence = DOTween.Sequence();

        _teleportSequence
            .JoinCallback(() =>
            {
                _rb.linearVelocity = Vector2.zero;
                _rb.simulated = false;
                _trail.emitting = false;
                _playerController.ResetMovement();
                _playerController.SetControlsEnabled(false);
            })
            .Append(
                _player.transform
                .DOMove(
                    _teleportingPoint.position,
                    .35f
                )
                .SetEase(Ease.OutQuad)
            )
            .JoinCallback(() =>
            {
                _player
                    .GetComponent<Animation>()
                    .Play("PortalIn");
                AudioManager.Instance.PlaySFXAtPosition(
                    SFXType.PortalIn,
                    transform.position
                );
            })
            .Append(
                DOTween.To(
                    () => _playerLight.intensity,
                    value => _playerLight.intensity = value,
                    0f,
                    .35f
                )
                .SetEase(Ease.OutQuad)
                .OnPlay(
                    () => Teleported?.Invoke(_destinationIndex)                  
                )
                // .OnComplete(() =>
                //     gameObject.transform
                //     .Find("visual")
                //     .GetComponent<SpriteRenderer>()
                //     .DOFade(0f, .35f)
                //     .SetEase(Ease.OutQuad)
                // )
            );
    }

    private void InitiatePlayerComponents()
    {
        if (_player == null) return;

        _rb = _player.GetComponent<Rigidbody2D>();
        _playerController = _player.GetComponent<PlayerController>();
        _trail = _player.GetComponent<TrailRenderer>();
        _playerLight = _player.GetComponentInChildren<Light2D>();
    }

    private void HandleIsCheckpointCrossed()
    {
        CrossedCheckpoint = true;
    }

    private void ResetTeleportStats()
    {
        CrossedCheckpoint = false;
        _hasEntered = false;
        // gameObject.transform
        //         .Find("visual")
        //         .GetComponent<SpriteRenderer>()
        //         .DOFade(1f, .35f)
        //         .SetEase(Ease.InQuad);
    }
}
