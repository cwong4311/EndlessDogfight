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

    public GameObject DeathEffectPF;

    public int ScoreOnDefeat;

    private bool _isInuvlnerable;
    private bool isDead;

    public AudioClip HurtSFX;

    [SerializeField]
    private AudioSource _audioSource;

    public void OnEnable()
    {
        CurrentHealth = MaxHealth;
        _isInuvlnerable = false;
        isDead = false;
    }

    public void Die()
    {
        isDead = true;
        ScoreController.onEnemyDeathScore.Invoke(ScoreOnDefeat);
        WaveController.onEnemyDeathPowerup.Invoke(transform.position);

        if (DeathEffectPF)
            ObjectPoolManager.Spawn(DeathEffectPF, transform.position, transform.rotation);
       
        if (ObjectPoolManager.IsPooledObject(gameObject))
            ObjectPoolManager.ReturnToPool(gameObject);
        else
            Destroy(gameObject);
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
        if (CurrentHealth <= 0 && !isDead)
        {
            Die();
        }

        if (_audioSource != null)
        {
            _audioSource.PlayOneShot(HurtSFX);
        }
    }

    public void ToggleInvuln(bool active)
    {
        _isInuvlnerable = active;
    }
}
