using System;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine;

public class LevelsMapUIController : MonoBehaviour
{
    [Header("Scene UI References")]
    [SerializeField] private GameObject _nextButton;
    [SerializeField] private GameObject _prevButton;
    [SerializeField] private RectTransform _content;
    [SerializeField] private ScrollRect _scrollRect; 

    [Header("Overlay UI References")]
    [SerializeField] private CanvasGroup _keyRewardOverlay;    
    [SerializeField] private RectTransform _rewardContent;
    [SerializeField] private Animation _highlightAnim;    
    [SerializeField] private Animation _keyAnim;    
    [SerializeField] private ParticleSystem _rewardParticles;    
    [SerializeField] private RectTransform _claimButtonPos;

    [Header("Movement")]
    [SerializeField] private float _moveDistance = 450f;
    [SerializeField] private float _moveDuration = .45f;
    [SerializeField] private Ease _moveEase = Ease.OutCubic;

    public static event Action OnUnlockNextLevelRequest;
    private Tween _moveTween;

    private void OnEnable()
    {
        SceneTransitionUI.OnTransitionOpened += HandleKeyClaimOverlay;
    }

    private void OnDisable()
    {
        SceneTransitionUI.OnTransitionOpened -= HandleKeyClaimOverlay;
    }

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

    private void HandleKeyClaimOverlay()
    {   
        OpenNextLevelKeyClaimOverlay();
    }

    private void OpenNextLevelKeyClaimOverlay()
    {
        Sequence openSequence = DOTween.Sequence();

        openSequence.Append(
            _keyRewardOverlay
            .DOFade(1f, .45f)
            .SetEase(Ease.InCubic)
        );
        openSequence.Join(
            _rewardContent
            .DOScale(1.1f, .6f)
            .SetEase(Ease.InCubic)
            .SetLoops(2, LoopType.Yoyo)
            .OnPlay(() => 
                AudioManager
                .Instance
                .PlayUISFX(
                    UISFXType.AchievementUnlocked
                )
            )
            .OnComplete(() =>
            {
                _rewardParticles.Play();
                _highlightAnim.Play();
            })
        )
        .JoinCallback(
            () => _keyAnim.Play("RewardFlip")
        );

        openSequence.OnComplete(() =>
        {
            _claimButtonPos
            .DOAnchorPosY(-350f, .9f)
            .SetEase(Ease.InCubic);

            _keyRewardOverlay.interactable = true;
            _keyRewardOverlay.blocksRaycasts = true;
        });
    }

    private void CloseNextLevelKeyClaimOverlay()
    {
        Sequence closeSequence = DOTween.Sequence();

        closeSequence.Append(
            _claimButtonPos
            .DOAnchorPosY(-623f, .9f)
            .SetEase(Ease.OutCubic)
        )
        .JoinCallback(() =>
        {
            _keyAnim.Stop("RewardFlip");
            _rewardParticles.Stop();
            _highlightAnim.Stop();
        })
        .Join(
            _rewardContent
            .DOScale(0f, .6f)
            .SetEase(Ease.OutCubic)
        )
        .Join(
            _keyRewardOverlay
            .DOFade(0f, .45f)
            .SetEase(Ease.OutCubic)
        )
        .OnComplete(() =>
        {
            _keyRewardOverlay.interactable = false;
            _keyRewardOverlay.blocksRaycasts = false;

            OnUnlockNextLevelRequest?.Invoke();
        });
    }

    public void OnKeyToNextLvlClaimBtnClick()
    {
        _claimButtonPos
        .DOScaleX(1.1f, .1f)
        .SetEase(Ease.OutQuad)
        .SetLoops(2, LoopType.Yoyo)
        .OnPlay(
            () => AudioManager.Instance.PlayUISFX(UISFXType.MouseClick)
        );

        CloseNextLevelKeyClaimOverlay();
    }

    private void OnDestroy()
    {
        _moveTween?.Kill();
    }
}