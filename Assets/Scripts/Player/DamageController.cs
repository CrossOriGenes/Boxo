using System.Collections;
using DG.Tweening;
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
    [SerializeField] private GameObject _healthBar;

    [Header("Player moods")]
    [SerializeField] private Sprite _happyMood;
    [SerializeField] private Sprite _noFeelingsMood;
    [SerializeField] private Sprite _sadMood;

    private GameObject _player;
    private Vector3 _respawnPosition;
    private SpriteRenderer _visualRenderer;
    private HealthBar _healthBarController;
    private const float MAX_HEALTH = 100f;
    private float _currentHealth;
    public static float GetMaxHealth => MAX_HEALTH;

    private void Awake()
    {
        _player = transform.root.gameObject;
        _respawnPosition = _player.transform.position;
        _visualRenderer = _visual.GetComponent<SpriteRenderer>();
        _healthBarController = _healthBar.GetComponent<HealthBar>();
        _currentHealth = MAX_HEALTH;
    }

    public void SetRespawnPoint(Vector3 position)
    {
        _respawnPosition = position;
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        _currentHealth = Mathf.Max(0, _currentHealth);
        _healthBarController.UpdateHealthBar(
            _currentHealth, 
            MAX_HEALTH
        );
        ChangePlayerFaceByHealth();
        if (_currentHealth == 0) 
            Die();
        else
            AudioManager.Instance.PlaySFX(SFXType.Damage);
    }

    private void Die()
    {
        _deathParticle.Play();
        AudioManager.Instance.PlaySFX(SFXType.Death);
        _playerController.enabled = false;
        _groundCheck.SetActive(false);
        _sideCheck.SetActive(false);
        _trail.emitting = false;
        _visual.SetActive(false);
        _healthBar.SetActive(false);
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
        _healthBar.SetActive(true);
        _currentHealth = MAX_HEALTH;
        _healthBarController.ResetHealthBar();
        FaceChanger(_happyMood);
        _trail.emitting = true;
        _groundCheck.SetActive(true);
        _sideCheck.SetActive(true);
        _playerController.ResetMovement();
        _playerController.enabled = true;
    }

    private void ChangePlayerFaceByHealth()
    {
        if (_currentHealth <= 33.3f)
            FaceChanger(_sadMood);
        else if (_currentHealth < 66.6f)
            FaceChanger(_noFeelingsMood);
        else
            FaceChanger(_happyMood);
    }

    private void FaceChanger(Sprite newSprite)
    {
        if (_visualRenderer.sprite == newSprite) 
            return;

        _visualRenderer.DOKill();
        _visualRenderer.color = Color.white;

        _visualRenderer
        .DOFade(0f, 0.12f)
        .OnComplete(
            () =>
            {
                _visualRenderer.sprite = newSprite;
                _visualRenderer.DOFade(1f, 0.12f);
            }
        );
    }
}
