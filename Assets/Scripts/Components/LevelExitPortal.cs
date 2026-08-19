using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LevelExitPortal : MonoBehaviour
{
    [Header("Fields")]
    [SerializeField] private Transform _centerPoint;
    [SerializeField] private Light2D _globalLight;
    [SerializeField] private LevelCompletedUI _levelCompletedUI;

    private bool _entered;
    private Rigidbody2D _rb;
    private PlayerController _playerController;
    private Light2D _playerLight;
    private SpriteRenderer _portalSprite;

    private void Awake()
    {
        _portalSprite = GetComponent<SpriteRenderer>();
    }

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
            () => {
                other.GetComponent<Animation>().Play("PortalIn");
                AudioManager.Instance.PlaySFXAtPosition(
                    SFXType.PortalIn,
                    transform.position
                );
            }
        );
        sequence.AppendInterval(0.5f);
        sequence.AppendCallback(
            () => _portalSprite.DOFade(0f, 0.75f)
                               .SetEase(Ease.OutQuad)
        );
        sequence.AppendInterval(0.3f);    
        sequence.AppendCallback(
            () => StartCoroutine(ShowLevelCompletedScreen())
        );
    }

    private IEnumerator ShowLevelCompletedScreen()
    {
        yield return new WaitForSeconds(0.15f);

        Sequence fadeSequence = DOTween.Sequence();
        fadeSequence.Join(
            DOTween.To(
                () => _globalLight.intensity,
                x => _globalLight.intensity = x,
                0f,
                0.7f
            )
            .SetEase(Ease.OutQuad)
        );
        fadeSequence.Join(
            DOTween.To(
                () => _playerLight.intensity,
                x => _playerLight.intensity = x,
                0f,
                .45f
            )
            .SetEase(Ease.InQuad)
        );

        yield return fadeSequence.WaitForCompletion();
        yield return new WaitForSeconds(.15f);

        GameStatsManager.Instance.CompleteLevel();

        AudioManager.Instance.PlaySFX(SFXType.LevelCompleted);

        LevelResult result = ScoreManager.Instance.Result;
        _levelCompletedUI.Show(
            score: result.Score,
            gemsEarned: result.Gems,
            xp: result.XP,
            health: result.Health,
            revives: result.Revives,
            accuracy: result.Accuracy
        );

        Time.timeScale = 0;

        yield return new WaitForSecondsRealtime(.75f);
        
        AudioManager.Instance.CrossFadeToMenuMusic();
    }
}
