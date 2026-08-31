using DG.Tweening;
using UnityEngine.UI;
using UnityEngine;

public class LevelsMapSceneController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject[] _levelButtons;
    [SerializeField] private GameObject _playLvlBtn;

    private void OnEnable()
    {
        LevelsMapUIController.OnUnlockNextLevelRequest += TestUnlockLevel2;
    }

    private void OnDisable()
    {
        LevelsMapUIController.OnUnlockNextLevelRequest -= TestUnlockLevel2;
    }

    private void Start()
    {
        InitializeUnlockedLevels();

        if (SceneTransitionUI.Instance != null)
            SceneTransitionUI.Instance.PlayOpen();
        
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

    private void TestUnlockLevel2()
    {
        GameObject levelButton = _levelButtons[1];

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
            levelButton.GetComponent<Button>().interactable = true;
        });
    }
}
