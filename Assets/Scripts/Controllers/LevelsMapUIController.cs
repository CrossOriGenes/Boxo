using DG.Tweening;
using UnityEngine.UI;
using UnityEngine;

public class LevelsMapUIController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform _titleTextTransform;
    [SerializeField] private GameObject _playButton;
    [SerializeField] private GameObject _nextButton;
    [SerializeField] private GameObject _prevButton;
    [SerializeField] private RectTransform _content;
    [SerializeField] private ScrollRect _scrollRect; 

    [Header("Movement")]
    [SerializeField] private float _moveDistance = 450f;
    [SerializeField] private float _moveDuration = .45f;
    [SerializeField] private Ease _moveEase = Ease.OutCubic;

    private Tween _moveTween;

    private void Start()
    {
        InitiateNavigation();
    }

    public void MoveNext()
    {
        if (_moveTween != null && _moveTween.IsActive())
            return;

        float currentX = _content.anchoredPosition.x;
        float maxX = Mathf.Max(
            0f,
            _content.rect.width - _scrollRect.viewport.rect.width
        );
        float targetX = Mathf.Max(
            currentX - _moveDistance,
            -maxX
        );

        AnimateTo(targetX);
    }

    public void MovePrevious()
    {
        if (_moveTween != null && _moveTween.IsActive())
            return;

        float currentX = _content.anchoredPosition.x;
        float targetX = Mathf.Min(
            currentX + _moveDistance,
            0f
        );

        AnimateTo(targetX);
    }

    public void OnPlayLevel()
    {
        _playButton
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

    private void InitiateNavigation()
    {
        UpdateNavigationButtons();
    }

    public void UpdateNavigationButtons()
    {
        float currentX = _content.anchoredPosition.x;
        float maxX = Mathf.Max(
            0f,
            _content.rect.width - _scrollRect.viewport.rect.width
        );
        const float tolerance = 1f;

        bool atStart = currentX >= -tolerance;
        bool atEnd = currentX <= -maxX + tolerance;

        _prevButton.GetComponent<Button>().interactable = !atStart;
        _nextButton.GetComponent<Button>().interactable = !atEnd;
    }

    private void AnimateTo(float targetX)
    {
        _moveTween?.Kill();

        Vector2 targetPosition = new Vector2(
            targetX, 
            _content.anchoredPosition.y
        );

        _moveTween = _content
                        .DOAnchorPos(targetPosition, _moveDuration)
                        .SetEase(_moveEase)
                        .OnComplete(UpdateNavigationButtons);
    }

    private void OnDestroy()
    {
        _moveTween?.Kill();
    }
}