using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A damageable entity
/// </summary>
public interface IDamageable
{
    /// <summary>
    /// reset health to max value
    /// </summary>
    void ResetHealth();

    /// <summary>
    /// decrease health value
    /// </summary>
    /// <param name="damage">damage points to deal</param>
    void ApplyDamage(int damage);

    /// <summary>
    /// actions upon health reaching 0
    /// </summary>
    void OnDeath();
}