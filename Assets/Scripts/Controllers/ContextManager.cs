using UnityEngine;

public class ContextManager : MonoBehaviour
{
    private static ContextManager _instance;
    public static ContextManager Instance 
    {
        get
        {
            if (_instance == null)
            {
                GameObject ctxObj = new GameObject("ContextManager");
                _instance = ctxObj.AddComponent<ContextManager>();
            }

            return _instance;
        }
    }

    private const string LAST_UNLOCKED_LEVEL_KEY = "LastUnlockedLevel";
    private const string TOTAL_KEYS_COLLECTED_KEY = "TotalKeys";

    private int _lastUnlockedLevel;
    private int _totalKeys;
    private bool _isKeyCollected;

    public int LastUnlockedLevel
    {
        get => _lastUnlockedLevel;
        set
        {
            _lastUnlockedLevel = Mathf.Max(1, value);
            PlayerPrefs.SetInt(
                LAST_UNLOCKED_LEVEL_KEY, 
                _lastUnlockedLevel
            );
            PlayerPrefs.Save();
        }
    }

    public int TotalKeys
    {
        get => _totalKeys;
        set
        {
            _totalKeys = Mathf.Max(0, value);
            PlayerPrefs.SetInt(
                TOTAL_KEYS_COLLECTED_KEY, 
                _totalKeys
            );
            PlayerPrefs.Save();
        }
    }

    public bool IsKeyCollected
    {
        get => _isKeyCollected;
        set => _isKeyCollected = value;
    } 

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        _lastUnlockedLevel = PlayerPrefs.GetInt(
            LAST_UNLOCKED_LEVEL_KEY,
            1
        );
        _totalKeys = PlayerPrefs.GetInt(
            TOTAL_KEYS_COLLECTED_KEY,
            0
        );
        _isKeyCollected = false;
    }

    public void AddKey()
    {
        TotalKeys++;
    }

    public void AddKeys(int amount)
    {
        if (amount <= 0) return;
        TotalKeys += amount;
    }

    public void UseKey()
    {
        if (TotalKeys <= 0) return;
        TotalKeys--;
    }

    public void ResetLevelsUnlocked()
    {
        PlayerPrefs.DeleteKey(LAST_UNLOCKED_LEVEL_KEY);
        PlayerPrefs.Save();

        _lastUnlockedLevel = 1;
    }
    
    public void ResetKeysClaimed()
    {
        PlayerPrefs.DeleteKey(TOTAL_KEYS_COLLECTED_KEY);
        PlayerPrefs.Save();
        
        _totalKeys = 0;
    }
}
