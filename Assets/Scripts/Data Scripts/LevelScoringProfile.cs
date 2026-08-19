using UnityEngine;

[CreateAssetMenu(
    fileName = "LevelScoringProfile", 
    menuName = "Game/Level Scoring Profile"
)]
public class LevelScoringProfile : ScriptableObject
{
    [Header("Level Difficulty")]
    public LevelDifficulty difficulty;

    [Header("Time")]
    [Min(1f)]
    public float maxAllottedTime = 120f;

    [Header("Base Rewards")]
    [Min(0)]
    public int baseScore = 100;

    [Min(0)]
    public int baseXP = 1000;

    [Header("XP Difficulty Multiplier")]
    [Min(0f)]
    public float xpDifficultyMultiplier = 1f;
}
