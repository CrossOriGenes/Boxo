using UnityEngine;

public class LevelRuntimeData : MonoBehaviour
{
    [SerializeField] private LevelConfig _levelConfig;

    public LevelConfig Config => _levelConfig;
}
