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

    private SpriteRenderer _spriteRenderer;
    private bool _isActivated;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = inactiveSprite;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isActivated || !other.CompareTag("Player")) 
            return;

        _isActivated = true;
        
        DamageController damage = other.GetComponentInChildren<DamageController>();
        damage.SetRespawnPoint(spawnPoint.position);

        ActivateCheckpoint();
    }

    private void ActivateCheckpoint()
    {
        _spriteRenderer.sprite = activeSprite;
        
        transform
            .DOScale(1.02f, 0.40f)
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
}
