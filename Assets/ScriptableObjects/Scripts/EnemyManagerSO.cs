using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyManagerSO", menuName = "ScriptableObjects/EnemyManagerSO")]
public class EnemyManagerSO : ScriptableObject, ICollectionManager<EnemyType>
{
    public List<EnemySO> allEnemies;

    public int Count { get => allEnemies.Count; }
    ICollectionObject ICollectionManager<EnemyType>.Get(EnemyType type)
    {
        return allEnemies.Find(e => e.enemyType == type);
    }

}
