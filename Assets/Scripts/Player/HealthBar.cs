using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image _fill;

    private Tween _fillTween;
    private Tween _colorTween;

    private void Awake()
    {
        ResetHealthBar();
    }

    public void UpdateHealthBar(float currentLives, float maxLives)
    {
        float healthPercentage = Mathf.Clamp01(currentLives / maxLives);

        _fillTween?.Kill();
        _colorTween?.Kill();
        _fillTween = _fill.DOFillAmount(healthPercentage, 0.5f);
    
        Color targetColor = GetHealthBarColor(healthPercentage);
        _colorTween = _fill.DOColor(targetColor, 0.5f);
    }

    private Color GetHealthBarColor(float health)
    {
        if (health > 0.5f)
        {
            // Green -> Orange
            float t = Mathf.InverseLerp(1f, 0.5f, health);
            return Color.Lerp(
                Color.green, 
                new Color(1f, 0.5f, 0f),
                t
            );
        }
        else
        {
            // Orange -> Red
            float t = Mathf.InverseLerp(0.5f, 0f, health);
            return Color.Lerp(
                new Color(1f, 0.5f, 0f),
                Color.red,
                t
            );
        }
    }

    public void ResetHealthBar()
    {
        _fillTween?.Kill();
        _colorTween?.Kill();
        _fill.DOFillAmount(1f, 0.9f);
        _fill.DOColor(Color.green, 0.9f);
    }
}
