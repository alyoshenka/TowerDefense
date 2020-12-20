using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSO", menuName = "ScriptableObjects/PlayerSO")]
public class PlayerSO : ScriptableObject
{
    [SerializeField] private string playerName;
    [SerializeField] private int playerHP;
    [SerializeField] private int playerEXP;

    public string PlayerName { get => playerName; }
    public int PlayerHP { get => playerHP; }
    public int PlayerEXP { get => playerEXP; }
    public void ResetHealth() { throw new System.NotImplementedException(); }
    public void SaveAndDestroy() { throw new System.NotImplementedException(); }
}
