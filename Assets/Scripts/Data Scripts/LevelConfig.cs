using UnityEngine;

[CreateAssetMenu(
    fileName = "LevelConfig", 
    menuName = "Game/Level Config"
)]
public class LevelConfig : ScriptableObject
{
    [Header("Level")]
    public string levelId;
    public string levelName;

    [Header("Difficulty")]
    public LevelScoringProfile scoringProfile;

    [Header("Collectibles")]
    [Min(0)]
    public int totalGems;
}
