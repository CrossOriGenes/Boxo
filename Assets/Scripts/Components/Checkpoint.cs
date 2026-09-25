using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class Checkpoint : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite inactiveSprite;
    [SerializeField] private Sprite activeSprite;

    [Header("Additionals")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Light2D checkpointLight;

    [Header("Next Level Key")]
    [SerializeField] private GameObject _key;

    public static event Action CheckpointCrossed;
    // public static event Action DeActivateThisFromOtherPoint;
    private SpriteRenderer _spriteRenderer;
    private bool _isActivated;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = inactiveSprite;
    }

    private void OnEnable()
    {
        GameScreenOverlayUI.RestartLevel += DeActivateCheckpoint;
    }

    private void OnDisable()
    {
        GameScreenOverlayUI.RestartLevel -= DeActivateCheckpoint;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isActivated || !other.CompareTag("Player")) 
            return;

        _isActivated = true;
        
        DamageController damage = other.GetComponentInChildren<DamageController>();
        damage.SetRespawnPoint(spawnPoint.position);

        AudioManager.Instance.PlaySFXAtPosition(
            SFXType.Checkpoint,
            transform.position
        );

        _key.SetActive(true);

        CheckpointCrossed?.Invoke();

        ActivateCheckpoint();
    }

    private void ActivateCheckpoint()
    {
        _spriteRenderer.sprite = activeSprite;
        
        transform
            .DOScale(1f, 0.2f)
            .SetEase(Ease.InOutQuad)
            .SetLoops(2, LoopType.Yoyo);
        
        checkpointLight.intensity = 0f;
        
        Sequence glowSequence = DOTween.Sequence();
        glowSequence.Append(
            DOTween.To(
                () => checkpointLight.intensity,
                x => checkpointLight.intensity = x,
                2.87f,
                0.2f
            )
        );
        glowSequence.Append(
            DOTween.To(
                () => checkpointLight.intensity,
                x => checkpointLight.intensity = x,
                1.07f,
                0.45f
            )
        );
    }

    private void DeActivateCheckpoint()
    {
        if (!_isActivated) return;

        checkpointLight.intensity = 0f;

        transform.localScale = new Vector3(
            0.6826329f, 
            0.6826329f, 
            0.6826329f
        );
       
        _spriteRenderer.sprite = inactiveSprite;
       
        _key
            .GetComponentInChildren<PortalKey>()
            .DeactivateExitPortal();
        _key.SetActive(false);
        
        _isActivated = false;

        // DeActivateThisFromOtherPoint?.Invoke();
    }
}
