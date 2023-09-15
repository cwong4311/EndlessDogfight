using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IHealth
{
    [SerializeField]
    private float _maxHealth;

    [SerializeField]
    private float _currentHealth;

    public float MaxHealth
    {
        get { return _maxHealth; }
        set { _maxHealth = value; }
    }

    public float CurrentHealth
    {
        get { return _currentHealth; }
        set { _currentHealth = value; }
    }

    public GameObject deathEffectPF;

    public void OnEnable()
    {
        CurrentHealth = MaxHealth;
    }

    public void Die()
    {
        if (deathEffectPF)
            ObjectPoolManager.Spawn(deathEffectPF, transform.position, transform.rotation);
       
        if (ObjectPoolManager.IsPooledObject(gameObject))
            ObjectPoolManager.ReturnToPool(gameObject);
        else
            Destroy(gameObject);
    }

    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }
}
