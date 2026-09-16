using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DestinationTeleport : MonoBehaviour
{
    [Header("")]
    [SerializeField] private GameObject _player;
    [SerializeField] private int _teleportPairIndex = 1;

    // Local references
    public static event Action<int> ChangeGameLevelCameraToNewArea;
    private Transform _teleportingPoint;

    // Player references
    private Rigidbody2D _rb;
    private PlayerController _playerController;
    private Light2D _playerLight;
    private TrailRenderer _trail;
    private DamageController _damageController;

    private void Awake()
    {
        _teleportingPoint = gameObject.transform.Find("Teleporting_point");

        InitiatePlayerComponents();
    }

    private void OnEnable()
    {
        SourceTeleport_v2.Teleported += EndTeleportSequence;
        SourceTeleport.Teleported += EndTeleportSequence;
    }

    private void OnDisable()
    {
        SourceTeleport_v2.Teleported -= EndTeleportSequence;
        SourceTeleport.Teleported -= EndTeleportSequence;
    }

    private void EndTeleportSequence(int sourceTeleportIndex)
    {
        if (sourceTeleportIndex != _teleportPairIndex) 
            return;

        Sequence _teleportSequence = DOTween.Sequence();
        
        int targetAreaIndex = ++sourceTeleportIndex;
        ChangeGameLevelCameraToNewArea?.Invoke(targetAreaIndex);  

        _teleportSequence
            .AppendInterval(.8f)
            .OnComplete(() =>
                _player.transform.position = _teleportingPoint.position
            )  
            .Append(
                DOTween.To(
                    () => _playerLight.intensity,
                    value => _playerLight.intensity = value,
                    1f,
                    .35f
                )
                .SetEase(Ease.InQuad)
            )
            .JoinCallback(() =>
            {
                _player
                    .GetComponent<Animation>()
                    .Play("PortalOut");
                AudioManager.Instance.PlaySFXAtPosition(
                    SFXType.PortalOut,
                    transform.position
                );
            })
            .AppendCallback(() =>
            {
                _rb.simulated = true;
                _rb.linearVelocity = Vector2.zero;
                _rb.angularVelocity = 0f;
                _trail.Clear();
                _playerController.ResetMovement();
                _playerController.SetControlsEnabled(true);
                _trail.emitting = true;
                _damageController
                    .SetRespawnPoint(_teleportingPoint.position);
            });
    }
    
    private void InitiatePlayerComponents()
    {
        if (_player == null) return;

        _rb = _player.GetComponent<Rigidbody2D>();
        _playerController = _player.GetComponent<PlayerController>();
        _trail = _player.GetComponent<TrailRenderer>();
        _playerLight = _player.GetComponentInChildren<Light2D>();
        _damageController = _player.GetComponentInChildren<DamageController>();
    }
}
