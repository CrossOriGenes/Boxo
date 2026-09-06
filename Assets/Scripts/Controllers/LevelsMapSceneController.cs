using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LevelsMapSceneController : MonoBehaviour
{
    [Header("Levels")]
    [SerializeField] private GameObject[] _levelButtons;
    
    [Header("Buttons & UIs")]
    [SerializeField] private GameObject _playLvlBtn;
    [SerializeField] private GameObject _useKeyBtn;
    [SerializeField] private TMP_Text _keysCountText; 
    [SerializeField] private RectTransform _levelViewport;
    [SerializeField] private RectTransform _levelGridPages;
    [SerializeField] private RectTransform _backBtn;

    private int _currentPage = 0;
    private int _selectedLevel = 0;

    private void OnEnable()
    {
        LevelsMapUIController.OnUpdateKeysCount += UpdateKeysUI;
    }

    private void OnDisable()
    {
        LevelsMapUIController.OnUpdateKeysCount -= UpdateKeysUI;
    }

    private void Start()
    {
        UpdateKeysUI();
        InitializeUnlockedLevels();
        _playLvlBtn.SetActive(false);
    }

    private void InitializeUnlockedLevels()
    {
        int lastUnlockedLevel = ContextManager.Instance.LastUnlockedLevel;
        
        for (int i = 0; i < _levelButtons.Length; i++)
        {
            if (i < lastUnlockedLevel)
                UnlockLevelInstantly(_levelButtons[i]);    
            else
                _levelButtons[i]
                    .GetComponent<Button>()
                    .interactable = false;
        }
    }

    private void UnlockLevelInstantly(GameObject levelButton)
    {
        Button button = levelButton.GetComponent<Button>();
        Transform lockObj = levelButton.transform.Find("Lock");

        if (lockObj != null) Destroy(lockObj.gameObject);
        if (button != null) button.interactable = true;
    }

    private void UnlockLevel(int levelIndex)
    {
        int buttonIndex = levelIndex - 1;
        if (buttonIndex < 0 || 
        buttonIndex >= _levelButtons.Length)
            return;

        int targetPage = buttonIndex / 10;
        if (targetPage != _currentPage)
        {
            MoveToUnlockPage(targetPage);
            _currentPage = targetPage;
        }

        GameObject levelButton = _levelButtons[buttonIndex];

        Transform lockObj = levelButton.transform.Find("Lock");
        Animation animation = lockObj.GetComponent<Animation>();
        
        Sequence sequence = DOTween.Sequence();

        sequence.AppendCallback(
            () => animation.Play("Unlock")
        )
        .AppendInterval(.85f)
        .OnComplete(() =>
        {
            Destroy(lockObj.gameObject);
            levelButton
                .GetComponent<Button>()
                .interactable = true;
            ContextManager.Instance.LastUnlockedLevel = levelIndex;
        });
    }

    private void MoveToUnlockPage(int targetPage)
    {
        if (_levelViewport == null ||
        _levelGridPages == null)
            return;

        float pageWidth = _levelViewport.rect.width;
        float targetX = -(targetPage * pageWidth);

        _levelGridPages
        .DOAnchorPosX(targetX, .5f)
        .SetEase(Ease.OutCubic);
    }

    private void UpdateKeysUI()
    {
        int totalKeys = ContextManager.Instance.TotalKeys;
        _keysCountText.text = totalKeys.ToString();
        _useKeyBtn
            .GetComponent<Button>()
            .interactable = totalKeys > 0;
    }

    public void OnUseKeyBtnClick()
    {
        _useKeyBtn
        .GetComponent<RectTransform>()
        .DOScale(1.1f, .1f)
        .SetEase(Ease.OutCubic)
        .SetLoops(2, LoopType.Yoyo)
        .OnPlay(() => 
        {
            AudioManager.Instance.PlayUISFX(UISFXType.MouseClick);
            ContextManager.Instance.UseKey();
        })
        .OnComplete(() =>
        {
            UpdateKeysUI();

            int nextLevel = ContextManager.Instance.LastUnlockedLevel + 1;
            UnlockLevel(nextLevel);
        });
    }

    public void OnBackPressed()
    {
        _backBtn
        .DOScale(1.1f, .1f)
        .SetEase(Ease.OutCubic)
        .SetLoops(2, LoopType.Yoyo)
        .OnPlay(() =>
            AudioManager.Instance.PlayUISFX(UISFXType.MouseClick)
        )
        .OnComplete(() =>
        {
            SceneManager.LoadScene("Home Screen");
        });
    }

    public void SelectLevel(int levelNumber)
    {
        if (levelNumber <= 0 || 
        levelNumber > _levelButtons.Length)
            return;

        _selectedLevel = levelNumber;
        _playLvlBtn.SetActive(true);
    }

    public void PlaySelectedLevel()
    {
        if (_selectedLevel < 1)
        {
            Debug.Log("No level selected!");
            return;
        }

        _playLvlBtn
        .GetComponent<RectTransform>()
        .DOScale(1.1f, .1f)
        .SetEase(Ease.OutCubic)
        .SetLoops(2, LoopType.Yoyo)
        .OnPlay(() => 
            AudioManager.Instance.PlayUISFX(UISFXType.MouseClick)
        )
        .OnComplete(() =>
        {
            string levelName = $"Level {_selectedLevel}";

            AudioManager.Instance.StopMusic();
            
            SceneTransitionUI.Instance.PlayClose(
                levelName,
                new string[]
                {
                    "Preparing your level...",
                    "Loading gameplay...",
                    "Get ready!"
                }
            );
        });
    }
}
