using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.Rendering.Universal;
using UnityEngine;

public class Vent : 
    MonoBehaviour, 
    ILeverTarget,
    IVent
{
    [Header("References")]
    [SerializeField] private GameObject _player;
    [SerializeField] private Transform _ventEntryPoint;
    [SerializeField] private Transform _ventExitPoint;
    [SerializeField] private Transform _ventDoorPosition; 
    [SerializeField] private CanvasGroup _navigationCanvasGroup;
    [SerializeField] private Transform _arrowContainer;
    [SerializeField] private VentArrowUI _arrowPrefab;
    [SerializeField] private InteractionPromptController _interactionPrompt;
    private readonly List<VentArrowUI> _spawnedArrows = new();

    [Header("Passcodes")]
    [SerializeField] private string _connectorPasscode;
    [SerializeField] private string _ventsNetworkPasscode;

    [Header("Vent-Mounting Position")]
    [SerializeField] private VentMountingType _mountingType;

    [Header("Vent Network")]
    [SerializeField] private List<Vent> _connectedVents = new();
    public IReadOnlyList<Vent> ConnectedVents => _connectedVents;

    [Header("Metrics")]
    [Range(1, 10)]
    [SerializeField] private float _floorExitThrust = 7f;
    [Range(1, 10)]
    [SerializeField] private float _ceilingExitThrust = 1.5f;
    [Range(1, 10)]
    [SerializeField] private float _wallExitThrust = 5f;


    private Rigidbody2D _playerRb;
    private PlayerController _playerController;
    private Light2D _playerLight;
    private TrailRenderer _playerTrail;
    private float _initialPlayerLightIntensity;
    private bool _isVentTransitioning;
    private bool _isActive;
    public bool HasActivated
    {
        get => _isActive;
        set => _isActive = value;
    }
    public string Passcode => _connectorPasscode;
    public string VentNetworkPassword => _ventsNetworkPasscode;


    private void OnEnable()
    {
        LeverSwitch.LeverPressed += HandleActivation;
    }

    private void OnDisable()
    {
        LeverSwitch.LeverPressed -= HandleActivation;
    }

    private void Awake()
    {
        if (_player == null)
        {
            Debug.LogError(
                $"Player reference missing on Vent: {name}",
                this
            );
            return;
        }

        _playerRb = _player.GetComponent<Rigidbody2D>();
        _playerController = _player.GetComponent<PlayerController>();
        _playerTrail = _player.GetComponent<TrailRenderer>();
        _playerLight = _player.GetComponentInChildren<Light2D>();
        _initialPlayerLightIntensity = _playerLight.intensity;

        if (_ventEntryPoint == null)
        {
            Debug.LogError(
                $"Venting point missing on Vent: {name}",
                this
            );
        }
    }

    private void Start()
    {
        if (VentsNetworkManager.Instance != null)
            VentsNetworkManager.Instance.RegisterVent(this);
        else
            Debug.LogError(
                $"VentsNetworkManager not found for {name}!",
                this
            );
    }

    public void VentIn()
    {
        if (_isVentTransitioning || !HasActivated)
            return;

        if (_player == null ||
        _ventEntryPoint == null ||
        _ventExitPoint == null ||
        _playerRb == null ||
        _playerController == null ||
        _playerLight == null ||
        _navigationCanvasGroup == null ||
        _arrowContainer == null)
        {
            Debug.LogError($"Vent References are incomplete: {name}", this);
            return;
        }

        if (_playerController.HasVented) return;

        _isVentTransitioning = true;
        
        PlayVentEnterSequence();
    }
    public void VentOut()
    {

        if (_isVentTransitioning || !HasActivated)
            return;

        if (_player == null ||
        _ventEntryPoint == null ||
        _ventExitPoint == null ||
        _playerRb == null ||
        _playerController == null ||
        _playerLight == null ||
        _navigationCanvasGroup == null ||
        _arrowContainer == null)
        {
            Debug.LogError($"Vent References are incomplete: {name}", this);
            return;
        }

        if (!_playerController.HasVented) return;

        _isVentTransitioning = true;

        ClearNavigationArrows();
        SetNavigationUI(false);

        _interactionPrompt.HideInteractionPrompt();
        
        PlayVentExitSequence();
    }

    public void TravelToVent(Vent destinationVent)
    {
        if (_isVentTransitioning ||
          !_playerController.HasVented ||
          destinationVent == null ||
          destinationVent == this)
            return;

        if (!_connectedVents.Contains(destinationVent))
        {
            Debug.LogWarning(
                $"Vent '{destinationVent.name}' is not connected to '{name}'.",
                this
            );
            return;
        }

        if (!destinationVent.HasActivated)
        {
            Debug.LogWarning(
                $"Destination vent '{destinationVent.name}' is not active.",
                destinationVent
            );
            return;
        }

        _isVentTransitioning = true;
        destinationVent._isVentTransitioning = true;

        ClearNavigationArrows();
        SetNavigationUI(false);
        
        PlayVentTravelSequence(destinationVent);
    }

    private void HandleActivation(string passcode)
    {
        if (passcode == _connectorPasscode)
            HasActivated = true;
    }

    public void Activate()
    {
        HasActivated = true;
    }

    public void Deactivate()
    {
        HasActivated = false;
    }

    private void OnDestroy()
    {
        if (VentsNetworkManager.Instance != null)
            VentsNetworkManager.Instance.UnregisterVent(this);   
    }

    private void PlayVentTravelSequence(Vent destinationVent)
    {
        Sequence sequence = DOTween.Sequence();

        sequence.AppendCallback(() =>
        {
            _playerRb.linearVelocity = Vector2.zero;
            _playerRb.angularVelocity = 0f;
            _playerController.ResetMovement();
            _playerController.SetControlsEnabled(false);
        })
        .Append(
            _player.transform
                .DOMove(destinationVent._ventEntryPoint.position, .5f)
                .SetEase(Ease.InOutQuad)
                .OnPlay(() =>
                    AudioManager.Instance.PlaySFXAtPosition(
                        SFXType.DoorHatchShift,
                        transform.position
                    )
                )
        )
        .OnComplete(() =>
        {
            _isVentTransitioning = false;
            destinationVent._isVentTransitioning = false;

            _playerController.UpdateCurrentVent(destinationVent);
            _playerController.SetControlsEnabled(true);
            
            destinationVent.ShowNavigationArrows();
        });

        sequence.OnKill(() =>
        {
            _isVentTransitioning = false;
            destinationVent._isVentTransitioning = false;
        });
    } 
    private void PlayVentEnterSequence()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.AppendCallback(() =>
        {
            _playerRb.linearVelocity = Vector2.zero;
            _playerRb.angularVelocity = 0f;
            _playerRb.simulated = false;
            if (_playerTrail != null)
                _playerTrail.emitting = false;
            _playerController.ResetMovement();
            _playerController.SetControlsEnabled(false);
        })
        .Join(
            _ventDoorPosition
                .DOLocalMoveX(.67f, .45f)
                .SetEase(Ease.OutQuad)
                .OnPlay(() =>
                    AudioManager.Instance.PlaySFXAtPosition(
                        SFXType.VentIn, 
                        transform.position
                    )
                )
        )
        .Append(
            _player.transform
                .DOMove(_ventEntryPoint.position, .35f)
                .SetEase(Ease.OutQuad)
                .OnPlay(() =>
                    AudioManager.Instance.PlaySFX(SFXType.PortalIn)
                )
        )
        .JoinCallback(() =>
        {
            Animation playerAnimation =
                _player.GetComponent<Animation>();
            if (playerAnimation != null)
                playerAnimation.Play("TeleportIn");
            else
                Debug.LogWarning("Player Animation component not found!", _player);
        })
        .AppendInterval(.35f)
        .Append(
            _ventDoorPosition
                .DOLocalMoveX(0f, .45f)
                .SetEase(Ease.InQuad)
                .OnPlay(() =>
                    AudioManager.Instance.PlaySFXAtPosition(
                        SFXType.VentOut, 
                        transform.position
                    )
                )
        )
        .Join(
            DOTween.To(
                () => _playerLight.intensity,
                value => _playerLight.intensity = value,
                .5f,
                .35f
            )
            .SetEase(Ease.OutQuad)
        )
        .OnComplete(() =>
        {
            _playerController.UpdateCurrentVent(this);
            _playerController.HasVented = true;
            _playerController.SetControlsEnabled(true);

            _isVentTransitioning = false;

            ShowNavigationArrows();

            _interactionPrompt.ShowVentOutInteractionPrompt();
        });

        sequence.OnKill(() =>
        {
            //Avoid leaving the vent locked if this sequence is cancelled
            if (_isVentTransitioning)
                _isVentTransitioning = false;
        });
    }
    private void PlayVentExitSequence()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.AppendCallback(() =>
        {
            _playerRb.linearVelocity = Vector2.zero;
            _playerRb.angularVelocity = 0f;
            _playerController.ResetMovement();
            _playerController.SetControlsEnabled(false);
        })
        .Append(
            _ventDoorPosition
                .DOLocalMoveX(.67f, .45f)
                .SetEase(Ease.OutQuad)
                .OnPlay(() => 
                    AudioManager.Instance.PlaySFXAtPosition(
                        SFXType.VentIn, 
                        transform.position
                    )
                )
        )
        .Append(
            _player.transform
                .DOMove(_ventExitPoint.position, .35f)
                .SetEase(Ease.OutQuad)
                .OnPlay(() =>
                    AudioManager.Instance.PlaySFX(SFXType.PortalOut)
                )
        )
        .JoinCallback(() =>
        {
            Animation playerAnimation = 
                _player.GetComponent<Animation>();
            if (playerAnimation != null)
                playerAnimation.Play("TeleportOut");
            DOTween.To(
                () => _playerLight.intensity,
                value => _playerLight.intensity = value,
                _initialPlayerLightIntensity,
                .35f
            ).SetEase(Ease.OutQuad);

            _playerRb.simulated = true;
            _playerRb.linearVelocity = Vector2.zero;
            _playerRb.angularVelocity = 0f;

            _playerController.HasVented = false;
            _playerController.UpdateCurrentVent(null);
            _playerController.SetControlsEnabled(true);

            Vector2 direction = GetExitDirection();
            float thrust = GetExitThrust();
            _playerRb.AddForce(
                direction * thrust,
                ForceMode2D.Impulse
            );
        })
        .AppendInterval(.12f)
        .Join(
            _ventDoorPosition
                .DOLocalMoveX(0f, .45f)
                .SetEase(Ease.InQuad)
                .OnPlay(() => 
                    AudioManager.Instance.PlaySFXAtPosition(
                        SFXType.VentOut, 
                        transform.position
                    )
                )
        )
        .OnComplete(() =>
        {
            _isVentTransitioning = false;
        });

        sequence.OnKill(() =>
        {
            _isVentTransitioning = false;
        });
    }

    public void ShowNavigationArrows()
    {
        ClearNavigationArrows();

        if (VentsNetworkManager.Instance == null)
        {
            Debug.LogError($"VentsNetworkManager not found!", this);
            return;
        }

        var connectedVents = 
            VentsNetworkManager.Instance.GetConnectedVents(this);
        int arrowIndex = 0;
        int totalArrows = connectedVents.Count;
        
        foreach (Vent destinationVent in connectedVents)
        {
            if (destinationVent == null || destinationVent == this)
                continue;

            VentArrowUI arrow = 
                Instantiate(
                    _arrowPrefab,
                    _arrowContainer,
                    false
                );
            arrow.transform.localScale = Vector3.one;   

            float spacingOffset = 
                (arrowIndex - (totalArrows - 1) / 2f) * 20f;
            arrow.Setup(this, destinationVent, spacingOffset);
            arrowIndex++;

            _spawnedArrows.Add(arrow);
        }

        SetNavigationUI(true);
    }

    public void ClearNavigationArrows()
    {
        foreach (VentArrowUI arrow in _spawnedArrows)
        {
            if (arrow != null) 
                Destroy(arrow.gameObject);
        } 

        _spawnedArrows.Clear();
    }

    private void SetNavigationUI(bool visible)
    {
        if (_navigationCanvasGroup == null)
            return;

        _navigationCanvasGroup.alpha = visible ? 1f : 0f;
        _navigationCanvasGroup.interactable = visible;
        _navigationCanvasGroup.blocksRaycasts = visible;
    }

    private Vector2 GetExitDirection()
    {
        return _mountingType switch
        {
            VentMountingType.Floor => Vector2.up,
            VentMountingType.Ceiling => Vector2.down,
            VentMountingType.LeftWall => Vector2.right,
            VentMountingType.RightWall => Vector2.left,
            _ => Vector2.up,
        };
    }

    private float GetExitThrust()
    {
        return _mountingType switch
        {
            VentMountingType.Floor => _floorExitThrust,
            VentMountingType.Ceiling => _ceilingExitThrust,
            VentMountingType.LeftWall or 
            VentMountingType.RightWall => _wallExitThrust,
            _ => _floorExitThrust,
        };
    }
}
