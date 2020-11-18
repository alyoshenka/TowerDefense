using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [Tooltip("movement speed, scaled")] public float speed;
    [Tooltip("damage upon impact")] public int damage;

    private void Update()
    {
        transform.position += transform.up * Time.deltaTime * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageable ent = other.GetComponentInParent<IDamageable>();
        if (null != ent)
        {
            ent.ApplyDamage(damage);
            Explode();
        }
    }

    /// <summary>
    /// action upon impact
    /// </summary>
    private void Explode()
    {
        // Debug.Log("explosion");
        Destroy(gameObject);
        Destroy(this);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.up);
    }
}
