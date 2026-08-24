using DG.Tweening;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class HomeScreenController : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private RectTransform headingTextPart1; 
    [SerializeField] private RectTransform headingTextPart2; 
    [SerializeField] private RectTransform headingTextPart3; 
    [SerializeField] private GameObject boxoEmoji; 
    [SerializeField] private RectTransform startGameBtn;
    [SerializeField] private RectTransform leaderBoardBtn;
    [SerializeField] private RectTransform levelsMapBtn;
    [SerializeField] private RectTransform skinsBtn;
    [SerializeField] private RectTransform exitBtn;
    [SerializeField] private RectTransform profileBtn;
    [SerializeField] private RectTransform settingsBtn;

    private void Start()
    {
        PlayElementsEnterSequence();
    }

    private void PlayElementsEnterSequence()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(
            headingTextPart1
            .DOAnchorPosX(-200f, .35f)
            .SetEase(Ease.OutCubic)
        );
        sequence.Append(
            headingTextPart2
            .DOAnchorPosY(-137f, .35f)
            .SetEase(Ease.OutCubic)
        );
        sequence.Append(
            headingTextPart3
            .DOAnchorPosX(143f, .35f)
            .SetEase(Ease.OutCubic)
        );
        sequence.Append(
            boxoEmoji.GetComponent<Image>()
            .DOFade(1f, .25f)
            .SetEase(Ease.OutQuad)
        );
        sequence.Join(
            boxoEmoji.GetComponent<RectTransform>()
            .DOScale(1.15f, .3f)
            .SetEase(Ease.OutQuad)
            .SetLoops(2, LoopType.Yoyo)
            .OnComplete(
                () => boxoEmoji
                        .GetComponent<Animation>()
                        .Play("FloatImage")
            )
        );
        sequence.Append(
            startGameBtn
            .DOAnchorPosY(-56f, .5f)
            .SetEase(Ease.OutCubic)
            .OnComplete(() => 
                startGameBtn
                .DOScaleX(2.75f, .3f)
                .SetEase(Ease.InOutQuad)
                .SetLoops(2, LoopType.Yoyo)
            )
        );
        sequence.Join(
            leaderBoardBtn
            .DOAnchorPosY(45f, .65f)
            .SetEase(Ease.OutCubic)
            .OnComplete(() => 
                leaderBoardBtn
                .DOScaleX(1.55f, .3f)
                .SetEase(Ease.InOutQuad)
                .SetLoops(2, LoopType.Yoyo)
            )
        );
        sequence.Append(
            levelsMapBtn
            .DOAnchorPosX(387f, .8f)
            .SetEase(Ease.OutCubic)
        );
        sequence.Join(
            skinsBtn
            .DOAnchorPosX(160f, .95f)
            .SetEase(Ease.OutCubic)
        );
        sequence.Join(
            exitBtn
            .DOAnchorPosX(-260f, .75f)
            .SetEase(Ease.OutCubic)
        );
        sequence.Join(
            profileBtn
            .DOAnchorPosX(-170f, .85f)
            .SetEase(Ease.OutCubic)
        );
        sequence.Join(
            settingsBtn
            .DOAnchorPosX(-82f, .95f)
            .SetEase(Ease.OutCubic)
        );
    } 

    public void OnStartGame()
    {
        startGameBtn
        .DOScale(2.65f, .1f)
        .SetEase(Ease.OutQuad)
        .SetLoops(2, LoopType.Yoyo)
        .OnPlay(
            () => AudioManager.Instance.PlayUISFX(UISFXType.MouseClick)
        )
        .OnComplete(
            () => SceneManager.LoadScene("Levels Map")
        );
    }

    public void OnShowLeaderBoard()
    {
        leaderBoardBtn
        .DOScale(1.45f, .1f)
        .SetLoops(2, LoopType.Yoyo)
        .OnPlay(
            () => AudioManager.Instance.PlayUISFX(UISFXType.MouseClick)
        )
        .OnComplete(
            () => Debug.Log("Show Leaderbords Stats")
        );
    }

    public void OnOpenProfile()
    {
        profileBtn
        .DOScale(1.05f, .1f)
        .SetLoops(2, LoopType.Yoyo)
        .OnPlay(
            () => AudioManager.Instance.PlayUISFX(UISFXType.MouseClick)
        )
        .OnComplete(
            () => Debug.Log("Open User Profile Popup dialog")
        );
    }

    public void OnOpenSettings()
    {
        settingsBtn
        .DOScale(1.05f, .1f)
        .SetLoops(2, LoopType.Yoyo)
        .OnPlay(
            () => AudioManager.Instance.PlayUISFX(UISFXType.MouseClick)
        )
        .OnComplete(
            () => Debug.Log("Open settings popup")
        );
    }

    public void OnOpenSkinsSelectionSection()
    {
        skinsBtn
        .DOScale(1.05f, .1f)
        .SetLoops(2, LoopType.Yoyo)
        .OnPlay(
            () => AudioManager.Instance.PlayUISFX(UISFXType.MouseClick)
        )
        .OnComplete(
            () => Debug.Log("Open skins chooser scene")
        );
    }

    public void OnOpenLevelsMap()
    {
        levelsMapBtn
        .DOScale(1.05f, .1f)
        .SetLoops(2, LoopType.Yoyo)
        .OnPlay(
            () => AudioManager.Instance.PlayUISFX(UISFXType.MouseClick)
        )
        .OnComplete(
            () => SceneManager.LoadScene("Levels Map")
        );
    }

    public void OnQuitGame()
    {
        exitBtn
        .DOScale(1.05f, .1f)
        .SetLoops(2, LoopType.Yoyo)
        .OnPlay(
            () => AudioManager.Instance.PlayUISFX(UISFXType.MouseClick)
        )
        .OnComplete(
            () => StartCoroutine(QuitGame())
        );
    }

    private IEnumerator QuitGame()
    {
        Debug.Log("Quitting from Game...");
        yield return new WaitForSeconds(.5f);
        Application.Quit(1);
    }
}
