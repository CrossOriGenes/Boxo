using UnityEngine;

public class Checkpoint_v2 : MonoBehaviour
{
    private static readonly int HasCrossedCheckpointHash = 
        Animator.StringToHash("hasCrossedCheckpoint");

    [Header("References")]
    [SerializeField] private Animator _visualAnimator;
    [SerializeField] private GameObject _defaultRing;
    [SerializeField] private GameObject _checkpointCrossedRing;
    [SerializeField] private Transform _respawnPoint; 

    private bool _hasCrossedCheckpoint;


    private void OnEnable()
    {
        GameScreenOverlayUI.RestartLevel += HandleResetCheckpoint;
    }
    private void OnDisable()
    {
        GameScreenOverlayUI.RestartLevel -= HandleResetCheckpoint;
    } 
    
    private void Awake()
    {
        ShowCheckpointDefaultRing();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_hasCrossedCheckpoint ||
        !other.CompareTag("Player"))
            return;

        _hasCrossedCheckpoint = true;

        AudioManager.Instance.PlaySFXAtPosition(
            SFXType.Checkpoint,
            transform.position
        );

        ShowCheckpointCrossedRing();

        DamageController playerDamageController = 
            other.GetComponentInChildren<DamageController>();
        if (playerDamageController != null)
            playerDamageController.SetRespawnPoint(
                _respawnPoint.position
            );
    }

    private void ShowCheckpointDefaultRing()
    {
        _checkpointCrossedRing.SetActive(false);   
        _defaultRing.SetActive(true);
        _visualAnimator.SetBool(HasCrossedCheckpointHash, false);
    }

    private void ShowCheckpointCrossedRing()
    {
        _checkpointCrossedRing.SetActive(true);   
        _defaultRing.SetActive(false);
        _visualAnimator.SetBool(HasCrossedCheckpointHash, true);
    }

    private void HandleResetCheckpoint()
    {
        _hasCrossedCheckpoint = false;

        ShowCheckpointDefaultRing();
    }
}
