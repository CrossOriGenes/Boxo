using UnityEngine;

public class MenuMusicController : MonoBehaviour
{
    void Start()
    {
        if (!AudioManager.Instance.Halted)
            AudioManager.Instance.PlayMenuMusic();
    }
}
