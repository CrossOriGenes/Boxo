using UnityEngine;
using DG.Tweening;

public class PortalKey : MonoBehaviour
{
    [Header("Exit Teleport")]
    [SerializeField] private GameObject _exitPortal;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Sequence sequence = DOTween.Sequence();

            sequence.AppendCallback(() => 
            {
                AudioManager.Instance.PlaySFXAtPosition(
                    SFXType.Pickup,
                    transform.position
                );
                
                _exitPortal.SetActive(true);
            
                ContextManager.Instance.IsKeyCollected = true;
            });
            
            sequence
            .AppendInterval(0.2f)
            .OnComplete(() => 
                gameObject.SetActive(false)
            );
        }
    }

    public void DeactivateExitPortal()
    {
        if (_exitPortal.activeInHierarchy)
            _exitPortal.SetActive(false);
    }
}
