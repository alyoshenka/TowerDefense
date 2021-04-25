using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemySO", menuName = "ScriptableObjects/EnemySO")]
public class EnemySO : ScriptableObject, ICollectionObject
{
    public EnemyType enemyType;
    public GameObject prefab;

    // other data
}
