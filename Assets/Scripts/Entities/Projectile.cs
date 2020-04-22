using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    public float speed;
    public int damage;

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

    private void Explode()
    {
        // Debug.Log("explosion");
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.up);
    }
}
