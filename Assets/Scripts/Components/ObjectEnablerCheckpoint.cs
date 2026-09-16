using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ObjectEnablerCheckpoint : MonoBehaviour
{
    [Header("")]
    [SerializeField] private GameObject _object;
    [SerializeField] private Light2D _activationLight;
    [SerializeField] private int _currentAreaIndex = 1;

    public static event Action<int> MicroCheckpointCrossed;
    private bool _hasTouched;

    private void Awake()
    {
        _object.SetActive(false);
        _activationLight.intensity = 0f;
    }

    private void OnEnable()
    {
        Checkpoint.DeActivateThisFromOtherPoint += ResetActivations;
    }

    private void OnDisable()
    {
        Checkpoint.DeActivateThisFromOtherPoint -= ResetActivations;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || _hasTouched)
            return;

        _hasTouched = true;

        DOTween.To(
            () => _activationLight.intensity,
            value => _activationLight.intensity = value,
            2.2f,
            .65f
        )
        .SetEase(Ease.InOutCubic)
        .OnPlay(() => 
            AudioManager.Instance.PlaySFX(SFXType.Checkpoint)
        )
        .OnComplete(() =>
        {
            _object.SetActive(true);
            MicroCheckpointCrossed?.Invoke(_currentAreaIndex);    
        });
    }

    private void ResetActivations()
    {
        _object.SetActive(false);
        _activationLight.intensity = 0f;
        _hasTouched = false;
    }
}
