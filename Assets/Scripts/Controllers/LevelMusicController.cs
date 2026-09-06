using UnityEngine;

public class LevelMusicController : MonoBehaviour
{
    void Start()
    {
        if (!AudioManager.Instance.Halted)
            AudioManager.Instance.CrossFadeToGameplayMusic();
    }
}
