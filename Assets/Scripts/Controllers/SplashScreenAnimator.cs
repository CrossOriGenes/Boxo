using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SplashScreenAnimator : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] private RectTransform bgTransform;
    [SerializeField] private GameObject content;

    private void Start()
    {
        Sequence splashSequence = DOTween.Sequence();

        splashSequence.Append(
            bgTransform
            .DOScale(1.05f, 3f)
            .SetEase(Ease.InOutCubic)
        );
        splashSequence.Join(
            content.GetComponent<CanvasGroup>()
            .DOFade(1f, .7f)
            .SetEase(Ease.InCubic)
        );
        splashSequence.Join(
            content.GetComponent<RectTransform>()
            .DOScale(1.07f, 2f)
            .SetEase(Ease.OutCubic)
        );

        splashSequence.OnComplete(
            () => SceneManager.LoadScene("Home Screen")
        );
    }
}
