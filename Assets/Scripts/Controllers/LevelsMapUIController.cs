using DG.Tweening;
using UnityEngine.UI;
using UnityEngine;

public class LevelsMapUIController : MonoBehaviour
{
    [Header("Texts & Buttons")]
    [SerializeField] private RectTransform titleTextTransform;
    [SerializeField] private GameObject playButton;

    private void Awake()
    {
        // playButton.SetActive(false);
    }

    private void Start()
    {
        titleTextTransform
        .DOAnchorPosY(-40f, .2f)
        .SetEase(Ease.OutCubic);
    }

    public void OnPlayLevel()
    {
        playButton
        .GetComponent<RectTransform>()
        .DOScale(1.1f, .1f)
        .SetEase(Ease.InOutCubic)
        .SetLoops(2, LoopType.Yoyo)
        .OnPlay(
            () => AudioManager.Instance.PlayUISFX(UISFXType.MouseClick)
        )
        .OnComplete(
            () => Debug.Log("Start Level")
        );
    }
}
