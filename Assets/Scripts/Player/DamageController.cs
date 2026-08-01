using System.Collections;
using UnityEngine;

public class DamageController : MonoBehaviour
{
    [Header("Reference fields")]
    [SerializeField] private ParticleSystem _deathParticle;
    [SerializeField] private GameObject _visual;
    [SerializeField] private TrailRenderer _trail;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private GameObject _groundCheck;
    [SerializeField] private GameObject _sideCheck;

    private GameObject _player;
    private Vector3 _respawnPosition;

    private void Awake()
    {
        _player = transform.root.gameObject;
        _respawnPosition = _player.transform.position;
    }

    public void SetRespawnPoint(Vector3 position)
    {
        _respawnPosition = position;
    }

    public void Die()
    {
        _deathParticle.Play();
        _playerController.enabled = false;
        _groundCheck.SetActive(false);
        _sideCheck.SetActive(false);
        _trail.emitting = false;
        _visual.SetActive(false);
        _rb.linearVelocity = Vector2.zero;
        _rb.simulated = false;
        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(1f);
        _player.transform.position = _respawnPosition;
        _rb.simulated = true;
        _rb.linearVelocity = Vector2.zero;
        _rb.angularVelocity = 0f;
        _trail.Clear();
        _visual.SetActive(true);
        _trail.emitting = true;
        _groundCheck.SetActive(true);
        _sideCheck.SetActive(true);
        _playerController.ResetMovement();
        _playerController.enabled = true;
    }
}
