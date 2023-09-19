using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealth
{
    public float MaxHealth { get; set; }

    public float CurrentHealth { get; set; }

    public void TakeDamage(float damage);

    public void Heal(float amount);

    public void Die();

    public void ToggleInvuln(bool active);
}
