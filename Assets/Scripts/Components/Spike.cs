using UnityEngine;

public class Spike : MonoBehaviour
{
    private DamageController _controller;
    private bool _hasInjured;

    private void OnTriggerEnter2D(Collider2D other)
    {
        _controller = other.GetComponentInChildren<DamageController>();

        if (other.CompareTag("Player") && !_hasInjured)
        {
            _hasInjured = true;
            if (_controller != null) 
                _controller.Die();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        _controller = other.GetComponentInChildren<DamageController>();

        if (other.CompareTag("Player") && _hasInjured)
        {
            _hasInjured = false;
        }
    }
}
