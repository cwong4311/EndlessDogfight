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

    public PlayerDeathSequence DeathSequence;

    public RectTransform HPBar;
    private float originalHPBarSize;

    private bool _isInuvlnerable;
    private float _damageIntensity;
    private float _tempInvulnStart;

    public void OnEnable()
    {
        CurrentHealth = MaxHealth;
        _isInuvlnerable = false;
        _damageIntensity = -1f;

        originalHPBarSize = HPBar.sizeDelta.x;
        UpdateHealthBar();
    }

    public void Update()
    {
        if (_damageIntensity > 0 && Time.time - _tempInvulnStart > _damageIntensity)
        {
            ToggleTemporaryInvulnAfterDamage(false);
        }
    }

    public void Die()
    {
        DeathSequence.OnPlayerDeath();
    }

    public void Heal(float amount)
    {
        CurrentHealth += amount;
        if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }

        UpdateHealthBar();
    }

    public void TakeDamage(float damage)
    {
        if (_isInuvlnerable) return;

        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
        {
            Die();
        }

        ToggleTemporaryInvulnAfterDamage(true, damage);
        UpdateHealthBar();
    }

    public void ToggleInvuln(bool active)
    {
        _isInuvlnerable = active;
    }

    private void UpdateHealthBar()
    {
        HPBar.sizeDelta = new Vector2(originalHPBarSize * CurrentHealth / MaxHealth, HPBar.sizeDelta.y);
    }

    private void ToggleTemporaryInvulnAfterDamage(bool active, float damageTaken = 0f)
    {
        _tempInvulnStart = Time.time;
        _damageIntensity = active ? damageTaken / 100f : -1f;

        ToggleInvuln(active);
    }
}
