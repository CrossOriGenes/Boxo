using UnityEngine;

public class ContextManager : MonoBehaviour
{
    public static ContextManager Instance { get; private set; }

    private const string LAST_UNLOCKED_LEVEL_KEY = "LastUnlockedLevel";

    private int _currentLevel;
    private int _nextLevel;
    private bool _isKeyCollected;

    /* ---------------------------
    ** PERSISTENT LEVEL PROGRESS
    *---------------------------- */
    public int LastUnlockedLevel
    {
        get => PlayerPrefs.GetInt(LAST_UNLOCKED_LEVEL_KEY, 1);
        set
        {
            int newValue = Mathf.Max(1, value);
            PlayerPrefs.SetInt(LAST_UNLOCKED_LEVEL_KEY, newValue);
            PlayerPrefs.Save();
        }
    }

    /* ---------------------------
    ** PERSISTENT LEVEL PROGRESS
    *---------------------------- */
    public int CurrentLevel
    {
        get => _currentLevel;
        set => _currentLevel = value;
    }

    public int NextLevel
    {
        get => _nextLevel;
        set => _nextLevel = value;
    }

    public bool IsKeyCollected
    {
        get => _isKeyCollected;
        set => _isKeyCollected = value;
    } 

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
