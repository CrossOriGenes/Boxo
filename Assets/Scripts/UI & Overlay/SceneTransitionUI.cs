using System;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneTransitionUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform _leftDoorPos;
    [SerializeField] private RectTransform _rightDoorPos;
    [SerializeField] private TMP_Text _loadingText;
    [SerializeField] private TMP_Text _progressValue;
    [SerializeField] private Image _loadingBarFill;
    [SerializeField] private GameObject _loadingContentWrapper;

    [Header("Animation Durations")]
    [SerializeField] private float _duration = .75f;
    [SerializeField] private float _loadingDuration = 4f;

    [Header("Door positions")]
    [SerializeField] private Vector2 _leftDoorOpenPosition;
    [SerializeField] private Vector2 _rightDoorOpenPosition;
    [SerializeField] private Vector2 _leftDoorClosedPosition;
    [SerializeField] private Vector2 _rightDoorClosedPosition;

    public static SceneTransitionUI Instance { get; private set; }
    public static event Action OnTransitionOpened;
    private bool _canPlayOpen;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);

        _leftDoorPos.anchoredPosition = _leftDoorOpenPosition;
        _rightDoorPos.anchoredPosition = _rightDoorOpenPosition;
        _loadingText.text = "";
        _progressValue.text = "0%";
        _loadingBarFill.fillAmount = 0f;
        _loadingContentWrapper.SetActive(false);
    }

    public void PlayClose()
    {
        Sequence loadingSequence = 
            DOTween.Sequence()
            .SetUpdate(true);

        loadingSequence.Append(
            _leftDoorPos
            .DOAnchorPos(_leftDoorClosedPosition, _duration)
            .SetEase(Ease.InOutQuad)
        );
        loadingSequence.Join(
            _rightDoorPos
            .DOAnchorPos(_rightDoorClosedPosition, _duration)
            .SetEase(Ease.InOutQuad)
        );

        loadingSequence.OnComplete(() => 
        {
            _canPlayOpen = true;
            PlayLoading();
        });
    }

    private void PlayLoading()
    {
        _loadingContentWrapper.SetActive(true);
        _loadingText.text = "Collecting your current stats...";
        _progressValue.text = "0%";

        Sequence loadingSequence = 
            DOTween
            .Sequence()
            .SetUpdate(true);

        loadingSequence
            .Append(
                _loadingBarFill
                .DOFillAmount(1f, _loadingDuration)
                .SetEase(Ease.Linear)
                .OnUpdate(() =>
                {
                    float percentage = _loadingBarFill.fillAmount * 100;
                    _progressValue.text = $"{percentage:0}%";
                })
            )
            .InsertCallback(1.3f, () =>
            {
                _loadingText.text = "Loading Rewards...";
            })
            .InsertCallback(2.6f, () =>
            {
                _loadingText.text = "Preparing your next level...";
            })
            .OnComplete(() =>
            {
                Debug.Log("Loading Completed"); 

                SceneManager.LoadScene("Levels Map");
                Time.timeScale = 1f;
            });
    }

    public void PlayOpen()
    {
        if (!_canPlayOpen) 
            return;
        _canPlayOpen = false;

        _loadingContentWrapper.SetActive(false);

        Sequence openSequence = 
            DOTween.Sequence()
            .SetUpdate(true);

        openSequence.Append(
            _leftDoorPos
            .DOAnchorPos(_leftDoorOpenPosition, _duration)
            .SetEase(Ease.InOutQuad)
        )
        .Join(
            _rightDoorPos
            .DOAnchorPos(_rightDoorOpenPosition, _duration)
            .SetEase(Ease.InOutQuad)
        )
        .OnComplete(
            () => OnTransitionOpened?.Invoke()
        );
    }
}
