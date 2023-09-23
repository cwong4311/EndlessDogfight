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

    public RectTransform HPBar;
    private float originalHPBarSize;

    private bool _isInuvlnerable;

    public void OnEnable()
    {
        CurrentHealth = MaxHealth;
        _isInuvlnerable = false;

        originalHPBarSize = HPBar.sizeDelta.x;
        UpdateHealthBar();
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

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(10f);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            Heal(20f);
        }
    }
}
