using UnityEngine;
using System;

public class GameStatsManager : MonoBehaviour
{
    public static GameStatsManager Instance { get; private set; }
    /* --------------------------
    ** LEVEL STATS 
    -------------------------- */
    private int _gemsCollected;
    private float _healthPercent;
    private int _revives;
    private float _completionTime;
    private float _elapsedTime;
    private bool _levelCompleted;

    public int GemsCollected => _gemsCollected;
    public float HealthPercent => _healthPercent;
    public int Revives => _revives;
    public float CompletionTime => _completionTime;
    public float ElapsedTime => _elapsedTime;

    /* --------------------------
    ** EVENTS 
    -------------------------- */
    public static event Action<int> OnGemsChanged;
    public static event Action OnLevelCompleted;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        ResetStats();
    }

    private void OnEnable()
    {
        Gem.OnGemCollected += HandleGemCollected;
        DamageController.OnHealthChanged += HandleHealthChanged;
        DamageController.OnPlayerRevived += HandleRevives;
    }

    private void OnDisable()
    {
        Gem.OnGemCollected -= HandleGemCollected;
        DamageController.OnHealthChanged -= HandleHealthChanged;
        DamageController.OnPlayerRevived -= HandleRevives;
    }

    private void FixedUpdate()
    {
        if (_levelCompleted) return;

        _elapsedTime += Time.fixedDeltaTime;
    }

    /* --------------------------
    ** EVENT HANDLERS
    -------------------------- */
    private void HandleGemCollected()
    {
        _gemsCollected++;
        OnGemsChanged?.Invoke(_gemsCollected);
    }

    private void HandleHealthChanged(float healthPercent)
    {
        _healthPercent = healthPercent;
    }

    private void HandleRevives()
    {
        _revives++;
    }

    public void CompleteLevel()
    {
        if (_levelCompleted) return;

        _levelCompleted = true;
        _completionTime = _elapsedTime;

        OnLevelCompleted?.Invoke();
    }

    /* --------------------------
    ** RESET 
    -------------------------- */
    private void ResetStats()
    {
        _gemsCollected = 0;
        _healthPercent = 100f;
        _revives = 0;
        _elapsedTime = 0f;
        _completionTime = 0f;
        _levelCompleted = false;
    }
}
