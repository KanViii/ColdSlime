using UnityEngine;

public class UnitBase : MonoBehaviour
{
    public UnitStatsSO myStats;
    protected float currentHealth;
    public float CurrentHealth => currentHealth; // Getter để UI có thể đọc máu

    void Start()
    {
        currentHealth = myStats.maxHealth;
    }

    public virtual void TakeDamage(float damage) {}

    protected virtual void Die()
    {
        Debug.Log($"{myStats.unitName} die!");
        gameObject.SetActive(false);
    }
}
