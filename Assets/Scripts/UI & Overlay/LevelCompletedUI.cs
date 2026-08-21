using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelCompletedUI : MonoBehaviour
{
    [Header("Panel & Title")]
    [SerializeField] private RectTransform panel;
    [SerializeField] private RectTransform title;

    [Header("Stars")]
    [SerializeField] private Image[] stars;
    
    [Header("Texts")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text xpText;
    [SerializeField] private TMP_Text gemsText;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text reviveText;
    [SerializeField] private TMP_Text accuracyText;

    [Header("Button")]
    [SerializeField] private RectTransform confirmButton;

    private CanvasGroup _canvasGroup;
    private LevelConfig _config;
    
    private void Awake()
    {
        _canvasGroup = gameObject.GetComponent<CanvasGroup>();
        _config = ScoreManager.Instance.LevelConfig;

        scoreText.text = "0%";
        xpText.text = "0";
        gemsText.text = "0";
        healthText.text = "0%";
        reviveText.text = "0";
        accuracyText.text = "0%";
    }

    public void Show(
        int score,
        int gemsEarned,
        int xp,
        float health,
        int revives,
        float accuracy
        )
    {
        _canvasGroup
            .DOFade(1f, .45f)
            .SetEase(Ease.OutQuad)
            .SetUpdate(true);
        
        Sequence sequence = DOTween.Sequence().SetUpdate(true);
        sequence.Append(
            panel
            .DOScale(1f, .35f)
            .SetEase(Ease.OutBack)
        );
        sequence.Join(
            title
            .DOAnchorPosX(0, .65f)
            .SetEase(Ease.OutCubic)
        );
        sequence.AppendCallback(() =>
        {
            AnimateNumbers(
                score,
                gemsEarned,
                xp,
                health,
                revives,
                accuracy
            );
            AnimateStars(accuracy);
        });
        sequence.AppendInterval(.45f);
        sequence.Append(
            confirmButton
            .DOAnchorPosY(-190, .35f)
            .SetEase(Ease.OutBounce)
        );
        sequence.AppendCallback(() =>
        {
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        });
    }

    private void AnimateNumbers(
        int score,
        int gemsEarned,
        int xp,
        float health,
        int revives,
        float accuracy
    )
    {
        DOTween.To(
            () => 0,
            value =>
            {
                scoreText.text = value.ToString();
            },
            score,
            .8f
        ).SetUpdate(true);
        DOTween.To(
            () => 0,
            value =>
            {
                gemsText.text = $"{value}/{_config.totalGems}";
            },
            gemsEarned,
            .8f
        ).SetUpdate(true);
        DOTween.To(
            () => 0,
            value =>
            {
                xpText.text = value.ToString();
            },
            xp,
            .8f
        ).SetUpdate(true);
        DOTween.To(
            () => 0,
            value =>
            {
                healthText.text = (int)value + "%";
            },
            health,
            .8f
        ).SetUpdate(true);
        DOTween.To(
            () => 0,
            value =>
            {
                reviveText.text = value.ToString();
            },
            revives,
            .8f
        ).SetUpdate(true);
        DOTween.To(
            () => 0,
            value =>
            {
                accuracyText.text = (int)value + "%";
            },
            accuracy,
            .8f
        ).SetUpdate(true);
    }

    private void AnimateStars(float accuracy)
    {
        int count = CalculateStars(accuracy);

        Sequence seq = DOTween.Sequence().SetUpdate(true);

        for (int i = 0; i < count; i++)
        {
            int index = i;

            Image starFill = stars[index];
            RectTransform starRoot = starFill.transform.parent as RectTransform;

            seq.AppendInterval(.15f);
            seq.Append(
                starFill
                    .DOFillAmount(1f, .25f)
                    .SetEase(Ease.OutQuad)
            );
            seq.Join(
                starRoot
                    .DOScale(1.15f, .18f)
                    .SetEase(Ease.OutQuad)
                    .SetLoops(2, LoopType.Yoyo)
                    .OnComplete(
                        () => AudioManager.Instance.PlayUISFX(UISFXType.BlingPop)
                    )
            );
        }
    }

    private int CalculateStars(float accuracy)
    {
        if (accuracy >= 90) return 3;
        if (accuracy >= 55) return 2;
        if (accuracy >= 25) return 1;

        return 0;
    }
}
