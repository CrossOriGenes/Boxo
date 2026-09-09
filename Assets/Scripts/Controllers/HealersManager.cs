using System.Collections.Generic;
using UnityEngine;

public class HealersManager : MonoBehaviour
{
    [Header("")]
    [SerializeField] private List<GameObject> _healers;

    private void OnEnable()
    {
        GameScreenOverlayUI.RestartLevel += HandleAllHealersReset;
    } 

    private void OnDisable()
    {
        GameScreenOverlayUI.RestartLevel -= HandleAllHealersReset;
    }

    private void HandleAllHealersReset()
    {
        foreach (GameObject healer in _healers)
            healer
                .GetComponent<Medikit>()
                .ReActivate();
    }
}
