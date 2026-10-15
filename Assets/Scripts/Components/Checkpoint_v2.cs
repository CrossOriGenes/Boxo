using UnityEngine;

public class Checkpoint_v2 : MonoBehaviour
{
    private static readonly int HasCrossedCheckpointHash = 
        Animator.StringToHash("hasCrossedCheckpoint");

    [Header("References")]
    [SerializeField] private Animator _visualAnimator;
    [SerializeField] private GameObject _defaultRing;
    [SerializeField] private GameObject _checkpointCrossedRing;

    private bool hasCrossedCheckpoint;


    private void Awake()
    {
        ShowCheckpointDefaultRing();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
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
}
