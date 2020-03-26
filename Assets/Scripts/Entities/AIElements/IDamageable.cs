using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A damageable entity
/// </summary>
public interface IDamageable
{
    void ResetHealth();

    void ApplyDamage(int damage);

    void OnDeath();
}