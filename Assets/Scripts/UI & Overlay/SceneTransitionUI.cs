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
    private string _destinationScene;
    private string[] _loadingMessages;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;

        _leftDoorPos.anchoredPosition = _leftDoorOpenPosition;
        _rightDoorPos.anchoredPosition = _rightDoorOpenPosition;
        _loadingText.text = "";
        _progressValue.text = "0%";
        _loadingBarFill.fillAmount = 0f;
        _loadingContentWrapper.SetActive(false);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void PlayClose(
        string destinationScene,
        string[] loadingMessages
    )
    {
        if (string.IsNullOrEmpty(destinationScene))
            return;

        _destinationScene = destinationScene;
        _loadingMessages = loadingMessages;

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
        
        _loadingBarFill.fillAmount = 0f;
        _progressValue.text = "0%";

        if (_loadingMessages != null &&
        _loadingMessages.Length > 0)
            _loadingText.text = _loadingMessages[0];
        else
            _loadingText.text = "";

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
            );

        if (_loadingMessages != null &&
            _loadingMessages.Length > 1)
        {
            loadingSequence.InsertCallback(
                _loadingDuration / 3f,
                () =>
                {
                    _loadingText.text = _loadingMessages[1];
                }    
            );
        }
            
        if (_loadingMessages != null &&
            _loadingMessages.Length > 2)
        {
            loadingSequence.InsertCallback(
                (_loadingDuration / 3f) * 2f,
                () =>
                {
                    _loadingText.text = _loadingMessages[2];
                }    
            );
        }
            
        loadingSequence.OnComplete(() =>
            {
                Debug.Log($"Loaded to -> {_destinationScene}"); 
                AudioManager.Instance.Halted = false;
                
                SceneManager.LoadScene(_destinationScene);
                Time.timeScale = 1f;
            }
        );
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!_canPlayOpen) return;

        PlayOpen();
    }

    private void PlayOpen()
    {
        if (!_canPlayOpen) return;

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
