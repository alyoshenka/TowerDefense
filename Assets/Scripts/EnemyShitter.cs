using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShitter : MonoBehaviour
{
    public GameObject enemy;
    BasicEnemy butt;

    private void Start()
    {
        butt = Instantiate(enemy.gameObject, transform.position, Quaternion.identity).GetComponent<BasicEnemy>();
        // butt.gameObject.SetActive(false);
    }
}
