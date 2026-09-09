using DG.Tweening;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class CameraBoundsController : MonoBehaviour
{
    [Header("Camera Confiners")]
    [SerializeField] private List<PolygonCollider2D> _confinerColliders;
    
    [Header("References")]
    [SerializeField] private CinemachineConfiner2D _playerFollowCam;
    [SerializeField] private CanvasGroup _cameraTransitOverlay;

    private void OnEnable()
    {
        DestinationTeleport
        .ChangeGameLevelCameraToNewArea += SetCameraConfinerToBound;
        GameScreenOverlayUI.RestartLevel += ResetConfinerBound;
    }

    private void OnDisable()
    {
        DestinationTeleport
        .ChangeGameLevelCameraToNewArea -= SetCameraConfinerToBound;
        GameScreenOverlayUI.RestartLevel -= ResetConfinerBound;
    }

    private void SetCameraConfinerToBound(int areaNumber)
    {
        int confinerIndex = areaNumber - 1;

        if (confinerIndex < 0)
        {
            Debug.LogError("INVALID AREA SELECTED!");
            return;
        }

        DOTween.To(
            () => _cameraTransitOverlay.alpha,
            value => _cameraTransitOverlay.alpha = value,
            1f,
            .75f
        )
        .SetEase(Ease.InOutCubic)
        .SetLoops(2, LoopType.Yoyo)
        .OnPlay(() => 
            _playerFollowCam.BoundingShape2D = 
            _confinerColliders[confinerIndex]
        );
    }   

    private void ResetConfinerBound()
    {
        DOTween.To(
            () => _cameraTransitOverlay.alpha,
            value => _cameraTransitOverlay.alpha = value,
            1f,
            .75f
        )
        .SetEase(Ease.InOutCubic)
        .SetLoops(2, LoopType.Yoyo)
        .OnPlay(() => 
            _playerFollowCam.BoundingShape2D = 
            _confinerColliders[0]
        );
    }
}
