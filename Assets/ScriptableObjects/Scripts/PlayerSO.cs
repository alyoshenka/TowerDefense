using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSO", menuName = "ScriptableObjects/PlayerSO")]
public class PlayerSO : ScriptableObject
{
    [SerializeField] private string playerName = "default player";
    [SerializeField] private int playerHP = 0;
    [SerializeField] private int playerHPMax = 100;
    [SerializeField] private int playerEXP  = 0;

    public string PlayerName { get => playerName; }
    public int PlayerHP { get => playerHP; }
    public int PlayerEXP { get => playerEXP; }
    public void ResetHealth() { playerHP = playerHPMax; }
    public void SaveAndDestroy() { throw new System.NotImplementedException(); }
}
