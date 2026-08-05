using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LevelExitPortal : MonoBehaviour
{
    [Header("Fields")]
    [SerializeField] private Transform _centerPoint;
    [SerializeField] private Light2D _globalLight;

    private bool _entered;
    private Rigidbody2D _rb;
    private PlayerController _playerController;
    private Light2D _playerLight;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || _entered) 
            return;
            
        _entered = true;
        _rb = other.GetComponent<Rigidbody2D>();
        _playerController = other.GetComponent<PlayerController>();
        _playerLight = other.GetComponentInChildren<Light2D>();

        _rb.linearVelocity = Vector2.zero;
        _rb.simulated = false;
        _playerController.ResetMovement();
        _playerController.SetControlsEnabled(false);

        Sequence sequence = DOTween.Sequence();
        sequence.Append(
            other.transform.DOMove(
                _centerPoint.position,
                0.35f
            )
            .SetEase(Ease.InOutSine)
        );
        sequence.AppendCallback(
            () => other.GetComponent<Animation>().Play("PortalIn")
        );
        sequence.AppendInterval(0.45f);
            
        sequence.AppendCallback(
            () => StartCoroutine(ShowLevelCompletedScreen())
        );
    }

    private IEnumerator ShowLevelCompletedScreen()
    {
        DOTween.To(
            () => _globalLight.intensity,
            x => _globalLight.intensity = x,
            0f,
            0.7f
        )
        .SetEase(Ease.OutQuad);
        DOTween.To(
            () => _playerLight.intensity,
            x => _playerLight.intensity = x,
            0f,
            .45f
        )
        .SetEase(Ease.InQuad);

        yield return new WaitForSeconds(0.75f);
        Debug.Log("Level Completed");
    }
}
