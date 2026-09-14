using UnityEngine;

public class PoisonPool : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        DamageController damageController = 
            other.GetComponentInChildren<DamageController>();
        if (damageController == null) 
            return;

        damageController.TakeFullDamage();
    }
}
