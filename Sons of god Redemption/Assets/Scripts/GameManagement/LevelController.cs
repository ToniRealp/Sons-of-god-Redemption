using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LevelController : MonoBehaviour
{
    public int roomsExplored;
    public RoomController[] roomControllers;
    public Transform actualSpawn;
    public Transform[] spawns;
    public bool[] trigger;
    // Use this for initialization


    public virtual void OpenAllDoors() {}

    protected void LoadGame()
    {
        if (SaveSystem.LoadData() != null)
        {
            SaveData saveData = SaveSystem.LoadData();

            roomsExplored = saveData.roomsExplored;
            actualSpawn = spawns[roomsExplored];

            for (int i = 0; i < roomControllers.Length; i++)
            {
                if (!saveData.rooms[i])
                    roomControllers[i].InstantiateEnemies();
            }
        }
        else
        {
            roomsExplored = 0;
            actualSpawn = spawns[0];


            foreach (RoomController room in roomControllers)
            {
                room.InstantiateEnemies();
            }
        }
    }
}
