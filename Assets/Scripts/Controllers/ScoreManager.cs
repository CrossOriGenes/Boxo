using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private LevelRuntimeData _levelRuntimeData;
    private LevelResult _result;
    public LevelResult Result => _result;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        GameStatsManager.OnLevelCompleted += HandleLevelCompletedResults;
    }

    private void OnDisable()
    {
        GameStatsManager.OnLevelCompleted -= HandleLevelCompletedResults;
    }

    private void HandleLevelCompletedResults()
    {
        GameStatsManager stats = GameStatsManager.Instance;
        
        if (stats == null)
        {
            Debug.LogError("ScoreManager: GameStatsManager not found!");
            return;
        }
        if (_levelRuntimeData == null ||
        _levelRuntimeData.Config == null)
        {
            Debug.LogError("ScoreManager: LevelRuntimeData or LevelConfig missing.");
            return;
        }

        LevelConfig config = _levelRuntimeData.Config;
        if (config.scoringProfile == null)
        {
            Debug.LogError("ScoreManager: Scoring Profile missing.");
            return;
        }

        LevelScoringProfile profile = config.scoringProfile;
        
        /* --------------------------
        ** RAW VALUES
        ** -------------------------- */
        float completionTime = stats.CompletionTime;
        float healthPercent = stats.HealthPercent;
        int gemsCollected = stats.GemsCollected;
        int revives = stats.Revives;

        /* --------------------------
        ** PERFORMANCE
        ** -------------------------- */
        float timePerformance = CalculateTimePerformance(
                completionTime,
                profile.maxAllottedTime
            );
        float gemPerformance = CalculateGemPerformance(
                gemsCollected,
                config.totalGems
            );
        float healthPerformance = Mathf.Clamp01(healthPercent / 100f);
        float revivePerformance = CalculateRevivePerformance(revives);

        /* --------------------------------
        ** ACCURACY
        ** -------------------------------- */
        float accuracy = CalculateAccuracy(
                timePerformance,
                healthPerformance
            );

        /* --------------------------------
        ** OVERALL PERFORMANCE
        ** -------------------------------- */
        float overallPerformance = CalculateOverallPerformance(
                timePerformance,
                gemPerformance,
                healthPerformance,
                revivePerformance
            );

        /* --------------------------------
        ** SCORE
        ** -------------------------------- */
        int score = CalculateScore(
                profile.baseScore,
                overallPerformance
            );

        /* --------------------------------
        ** XP
        ** -------------------------------- */
        int xp = CalculateXP(
                profile.baseXP,
                overallPerformance,
                profile.xpDifficultyMultiplier
            );

        _result = new LevelResult
        {
            Score = score,
            Gems = gemsCollected,
            XP = xp,
            Health = healthPercent,
            Revives = revives,
            Accuracy = accuracy
        };

        Debug.Log(
            "========== LEVEL RESULT ==========\n" +

            $"Level: {config.levelName}\n" +
            $"Difficulty: {profile.difficulty}\n\n" +
            $"Time: {completionTime:F2}s / " +
            $"{profile.maxAllottedTime:F2}s\n" +
            $"Time Performance: {timePerformance * 100f:F0}%\n" +
            $"Gems: {gemsCollected}/{config.totalGems}\n" +
            $"Gem Performance: {gemPerformance * 100f:F0}%\n" +
            $"Health: {healthPercent:F0}%\n" +
            $"Revives: {revives}\n" +
            $"Revive Performance: {revivePerformance * 100f:F0}%\n\n" +
            $"Accuracy: {accuracy:F0}%\n" +
            $"Overall Performance: {overallPerformance * 100f:F0}%\n\n" +
            $"FINAL SCORE: {score}\n" +
            $"XP: {xp}\n" +

            "=================================="
        );
    }

    private float CalculateTimePerformance(
        float completionTime,
        float maxAllottedTime
    )
    {
        if (maxAllottedTime <= 0f) return 0f;
        float timeRatio = Mathf.Clamp01(completionTime / maxAllottedTime);
        float performance = Mathf.Sqrt(1f - timeRatio);
        return Mathf.Clamp01(performance);
    }

    private float CalculateGemPerformance(
        int gemsCollected,
        int totalGems
    )
    {
        if (totalGems <= 0) return 1f;
        return Mathf.Clamp01((float)gemsCollected / totalGems);
    }

    private float CalculateRevivePerformance(int revives)
    {
        const float penaltyPerRevive = 0.20f;
        float performance = 1f - (revives * penaltyPerRevive);
        return Mathf.Clamp01(performance);
    }

    private float CalculateAccuracy(
        float timePerformance,
        float healthPerformance
    )
    {
        float accuracy = 
            (timePerformance * 0.5f) + 
            (healthPerformance * 0.5f);
        
        return Mathf.Clamp(
            accuracy * 100f,
            0f,
            100f
        );
    }

    private float CalculateOverallPerformance(
        float timePerformance,
        float gemPerformance,
        float healthPerformance,
        float revivePerformance
    )
    {
        float performance =
            (timePerformance * 0.30f) +
            (gemPerformance * 0.25f) +
            (healthPerformance * 0.30f) +
            (revivePerformance * 0.15f);

        return Mathf.Clamp01(performance);
    }

    private int CalculateScore(
        int baseScore,
        float overallPerformance
    )
    {
        return Mathf.RoundToInt(
            baseScore * overallPerformance
        );
    }

    private int CalculateXP(
        int baseXP,
        float overallPerformance,
        float difficultyMultiplier
    )
    {
        float performanceMultiplier = 
            Mathf.Lerp(
                0.5f,
                1f,
                overallPerformance
            );
        float finalXP = baseXP * difficultyMultiplier * performanceMultiplier;

        return Mathf.RoundToInt(finalXP);
    }
}
