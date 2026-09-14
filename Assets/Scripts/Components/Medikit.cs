using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Medikit : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _visual;
    [SerializeField] private Light2D _light;
    [SerializeField] private ParticleSystem _healthReviveSpark;

    private DamageController _damageController;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
            
        _damageController = other.GetComponentInChildren<DamageController>();
        
        if (_damageController != null)
            if (_damageController.CurrentHealth < 60f)
                PlayHealthGainAnimation();
    }

    private void PlayHealthGainAnimation()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.JoinCallback(
            () => _visual.SetActive(false)
        )
        .Append(
            DOTween.To(
                () => _light.intensity,
                value => _light.intensity = value,
                4.4f,
                .85f
            )
            .OnComplete(() =>
                DOTween.To(
                    () => _light.intensity,
                    value => _light.intensity = value,
                    0f,
                    .85f
                )   
            )
        )
        .JoinCallback(() =>
            _healthReviveSpark.Play()
        )
        .OnPlay(() =>
        {
            AudioManager.Instance.PlaySFX(SFXType.Healer);
            _damageController.TakeMedikit();   
        });

        sequence.OnComplete(() =>
            gameObject.SetActive(false)
        );
    }

    public void ReActivate()
    {
        _visual.SetActive(true);
        gameObject.SetActive(true);
    }
}
