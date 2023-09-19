using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IHealth
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

    private bool _isInuvlnerable;

    public void OnEnable()
    {
        CurrentHealth = MaxHealth;
        _isInuvlnerable = false;
    }

    public void Die()
    {
        // TO DO
    }

    public void Heal(float amount)
    {
        CurrentHealth += amount;
        if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }
    }

    public void TakeDamage(float damage)
    {
        if (_isInuvlnerable) return;

        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    public void ToggleInvuln(bool active)
    {
        _isInuvlnerable = active;
    }
}
