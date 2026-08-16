using UnityEngine;

public class MenuMusicController : MonoBehaviour
{
    void Start()
    {
        AudioManager.Instance.PlayMenuMusic();
    }
}
