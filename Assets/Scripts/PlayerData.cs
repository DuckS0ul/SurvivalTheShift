using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PlayerData
{
    public float[] playerStats; //[0] - health, [1] - Calories, [2] - Hydration

    public float[] playerPositionAndRotation; // position x,y,z, rotation x,y,z

    //public float[] inventoryContent;


    public PlayerData(float[] _playerStats, float[] _playerPosAndRot)
    {
        playerStats = _playerStats;
        playerPositionAndRotation = _playerPosAndRot;
    }
}
