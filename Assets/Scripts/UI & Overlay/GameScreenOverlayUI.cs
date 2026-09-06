using System;
using TMPro;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameScreenOverlayUI : MonoBehaviour
{
    [Header("Gems collected HUD")]
    [SerializeField] private TMP_Text _gemsCollected;
    
    [Header("Buttons")]
    [SerializeField] private Button _pauseBtn;
    [SerializeField] private Button _resumeBtn;
    [SerializeField] private Button _restartBtn;
    [SerializeField] private Button _quitBtn;

    [Header("Canvas & Panels")]
    [SerializeField] private CanvasGroup _pauseOverlay;
    [SerializeField] private CanvasGroup _panelContent;
    [SerializeField] private RectTransform _panelTransform;

    [Header("Emoji")]
    [SerializeField] private CanvasGroup _emoji;

    public static event Action RestartLevel;
    private RectTransform _pauseBtnTransform;

    private void Awake()
    {
        _pauseBtnTransform = _pauseBtn.GetComponent<RectTransform>();
        _gemsCollected.text = "0";
        _pauseOverlay.alpha = 0f;
        _pauseOverlay.interactable = false;
        _pauseOverlay.blocksRaycasts = false;
        _panelContent.alpha = 0f;
        _panelContent.interactable = false;
        _panelContent.blocksRaycasts = false;
        _panelTransform.localScale = Vector3.zero;
    }

    private void OnEnable()
    {
        GameStatsManager.OnGemsChanged += UpdateGemsCollectedText;
    }

    private void OnDisable()
    {
        GameStatsManager.OnGemsChanged -= UpdateGemsCollectedText;
    }

    private void UpdateGemsCollectedText(int collectedGems)
    {
        _gemsCollected.text = collectedGems.ToString();
    }

    public void OnPause()
    {
        _pauseBtnTransform
            .DOScale(1.1f, .1f)
            .SetEase(Ease.InOutCubic)
            .SetLoops(2, LoopType.Yoyo)
            .OnPlay(
                () => AudioManager.Instance.PlayUISFX(UISFXType.MouseClick)
            );

        Debug.Log("Game Paused");

        OpenOverlayPanel();
    }

    public void OnResume()
    {
        _resumeBtn
            .GetComponent<RectTransform>()
            .DOScale(1.1f, .1f)
            .SetEase(Ease.InOutCubic)
            .SetLoops(2, LoopType.Yoyo)
            .OnPlay(
                () => AudioManager.Instance.PlayUISFX(UISFXType.MouseClick)
            )
            .SetUpdate(true);

        Sequence resumeSequence = 
            DOTween.Sequence().SetUpdate(true);

        CloseOverlayPanel(resumeSequence);

        resumeSequence.OnComplete(() =>
        {
            Time.timeScale = 1f;
            GameStatsManager.Instance.SetPaused(false);

            Debug.Log("Game Resumed");
            
            AudioManager.Instance.ResumeAudio();
            AmbientLoopAudio.ResumeAll();
        });
    }

    public void OnRestart()
    {
        _restartBtn
            .GetComponent<RectTransform>()
            .DOScale(1.1f, .1f)
            .SetEase(Ease.InOutCubic)
            .SetLoops(2, LoopType.Yoyo)
            .OnPlay(
                () => AudioManager.Instance.PlayUISFX(UISFXType.MouseClick)
            )
            .SetUpdate(true);

        Sequence restartSequence = 
            DOTween.Sequence().SetUpdate(true);

        CloseOverlayPanel(restartSequence);

        restartSequence.OnComplete(() =>
        {
            Time.timeScale = 1f;

            ContextManager.Instance.IsKeyCollected = false;

            AudioManager.Instance.RestartGamePlayMusic();
            AmbientLoopAudio.RestartAll();

            Debug.Log("Game Re-started");
            RestartLevel?.Invoke();
        });
    } 

    public void OnQuitGame()
    {
        _quitBtn
            .GetComponent<RectTransform>()
            .DOScale(1.1f, .1f)
            .SetEase(Ease.InOutCubic)
            .SetLoops(2, LoopType.Yoyo)
            .OnPlay(
                () => AudioManager.Instance.PlayUISFX(UISFXType.MouseClick)
            )
            .SetUpdate(true);

        Sequence quitSequence = 
            DOTween.Sequence().SetUpdate(true);

        CloseOverlayPanel(quitSequence);

        quitSequence.OnComplete(() =>
        {
            Time.timeScale = 1f;

            ContextManager.Instance.IsKeyCollected = false;
            
            Debug.Log("Game Terminated");

            AudioManager.Instance.StopMusic();

            SceneTransitionUI.Instance.PlayClose(
                "Levels Map",
                new string[]
                {
                    "Saving your stats...",
                    "Quitting..."
                }
            );
        });
    }

    private void OpenOverlayPanel()
    {
        Sequence pauseSequence = DOTween.Sequence();

        pauseSequence.Join(
            _pauseBtn
            .GetComponent<Image>()
            .DOFade(0f, .2f)
            .SetEase(Ease.OutQuad)
            .OnComplete(
                () => _pauseBtn.interactable = false
            )
        );
        pauseSequence.Append(
            _pauseOverlay
            .DOFade(1f, .25f)
            .SetEase(Ease.InOutCubic)
            .OnComplete(() =>
            {
                _pauseOverlay.interactable = true;
                _pauseOverlay.blocksRaycasts = true;
            })
        );
        pauseSequence.Join(
            _panelContent
            .DOFade(1f, .3f)
            .SetEase(Ease.OutCubic)
        );
        pauseSequence.Join(
            _panelTransform.
            DOScale(1f, .35f)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                _panelContent.interactable = true;
                _panelContent.blocksRaycasts = true;
            })
        );
        pauseSequence.Join(
            _emoji
            .DOFade(1f, .3f)
            .SetEase(Ease.InOutCubic)
            .OnComplete(() =>
            {
                _emoji.interactable = true;
                _emoji.blocksRaycasts = true;
            })
        );

        pauseSequence.OnComplete(() =>
        {
            AmbientLoopAudio.PauseAll();
            AudioManager.Instance.PauseAudio();

            GameStatsManager.Instance.SetPaused(true);
            Time.timeScale = 0f;
        });
    }

    private void CloseOverlayPanel(Sequence sequence)
    {
        sequence.Join(
            _emoji
            .DOFade(0f, .3f)
            .SetEase(Ease.InOutCubic)
            .OnComplete(() =>
            {
                _emoji.interactable = true;
                _emoji.blocksRaycasts = true;
            })
        );
        sequence.Join(
            _panelTransform.
            DOScale(0f, .35f)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                _panelContent.interactable = false;
                _panelContent.blocksRaycasts = false;
            })
        );
        sequence.Join(
            _panelContent
            .DOFade(0f, .3f)
            .SetEase(Ease.OutCubic)
        );
        sequence.Append(
            _pauseOverlay
            .DOFade(0f, .25f)
            .SetEase(Ease.InOutCubic)
            .OnComplete(() =>
            {
                _pauseOverlay.interactable = false;
                _pauseOverlay.blocksRaycasts = false;
            })
        );
        sequence.Join(
            _pauseBtn
            .GetComponent<Image>()
            .DOFade(1f, .2f)
            .SetEase(Ease.InQuad)
            .OnComplete(
                () => _pauseBtn.interactable = true
            )
        );
    }
}
