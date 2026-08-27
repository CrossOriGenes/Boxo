using DG.Tweening;
using UnityEngine;

public class LevelsMapSceneController : MonoBehaviour
{
    private void OnEnable()
    {
        SceneTransitionUI.OnTransitionOpened += HandleKeyClaimOverlay;
    }

    private void OnDisable()
    {
        SceneTransitionUI.OnTransitionOpened -= HandleKeyClaimOverlay;
    }

    private void Start()
    {
        SceneTransitionUI.Instance.PlayOpen();
    }

    private void HandleKeyClaimOverlay()
    {   
        Debug.Log("Requested to show key reward claim");
    }
}
