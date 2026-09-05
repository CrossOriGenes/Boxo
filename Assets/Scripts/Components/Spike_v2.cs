using DG.Tweening;
using UnityEngine;

public class Spike_v2 : MonoBehaviour
{
    [Header("Control Values")]
    [SerializeField] private float _spikeHeight = 1f;
    [SerializeField] private float _hiddenDuration = 1.5f;

    [Header("Animation")]
    [SerializeField] private float _moveDuration = .5f;

    private Sequence _spikeSequence;
    private float _baselineY;
    private float _hiddenY;
    private DamageController _controller;

    private void Awake()
    {
        _baselineY = transform.localPosition.y;
        _hiddenY = _baselineY - _spikeHeight;
    }

    private void OnEnable()
    {
        StartSpikeAnimation();
    }

    private void OnDisable()
    {
        _spikeSequence?.Kill();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _controller = other.GetComponentInChildren<DamageController>();

        if (other.CompareTag("Player"))
            if (_controller != null)
                _controller.TakeDamage(10);
    }

    private void StartSpikeAnimation()
    {
        _spikeSequence?.Kill();

        Vector3 hiddenPosition = transform.localPosition;
        hiddenPosition.y = _hiddenY;
        Vector3 baselinePosition = transform.localPosition;
        baselinePosition.y = _baselineY;

        transform.localPosition = hiddenPosition;
    
        _spikeSequence = DOTween.Sequence();
        _spikeSequence
            .AppendInterval(_hiddenDuration)
            .Append(
                transform
                .DOLocalMove(baselinePosition, _moveDuration)
                .SetEase(Ease.OutQuad)
            )
            .Append(
                transform
                .DOLocalMove(hiddenPosition, _moveDuration)
                .SetEase(Ease.InQuad)
            )
            .AppendInterval(_hiddenDuration)
            .SetLoops(-1);
    }
}
