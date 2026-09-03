using System.Collections.Generic;
using UnityEngine;

public class GemsManager : MonoBehaviour
{
    [Header("All Gems")]
    [SerializeField] private List<GameObject> _gems;

    private void OnEnable()
    {
        GameScreenOverlayUI.RestartLevel += HandleAllGemsReset;
    } 

    private void OnDisable()
    {
        GameScreenOverlayUI.RestartLevel -= HandleAllGemsReset;
    }

    private void HandleAllGemsReset()
    {
        foreach (GameObject gem in _gems)
            gem.SetActive(true);
    }
}
