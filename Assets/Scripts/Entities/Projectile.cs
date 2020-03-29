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
        transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageable ent = other.GetComponent<IDamageable>();
        if (null != ent)
        {
            ent.ApplyDamage(damage);
            Explode();
        }
    }

    private void Explode()
    {
        Debug.Log("explosion");
        Destroy(gameObject);
    }
}
