using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData {

    public int health, attack, roomsExplored;
    public float[] playerSpawn;
    public bool[] rooms;
    public int level;

    public SaveData(LevelController levelController, PlayerController playerStats, int _level )
    {
        rooms = new bool[levelController.roomControllers.Length];
        roomsExplored = levelController.roomsExplored;
        for (int i = 0; i < levelController.roomControllers.Length; i++)
        {
            rooms[i] = levelController.roomControllers[i].isEmpty;
        }

        health = playerStats.health;
        attack = playerStats.baseAttack;
        level = _level;

        playerSpawn = new float[3];
        playerSpawn[0] = levelController.actualSpawn.position.x;
        playerSpawn[1] = levelController.actualSpawn.position.y;
        playerSpawn[2] = levelController.actualSpawn.position.z;
    }

}
