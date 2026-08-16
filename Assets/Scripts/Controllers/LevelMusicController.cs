using UnityEngine;

public class LevelMusicController : MonoBehaviour
{
    void Start()
    {
        AudioManager.Instance.PlayGamePlayMusic();
    }
}
