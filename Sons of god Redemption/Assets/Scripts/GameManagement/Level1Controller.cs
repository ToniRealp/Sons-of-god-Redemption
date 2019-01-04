using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1Controller : MonoBehaviour {

    public GameObject player;
    public bool[] trigger;
    [SerializeField] Transform[] spawns = new Transform[1];

    public Transform actualSpawn;
    public int roomsExplored;

	// Use this for initialization
	void Start () {
        trigger = new bool[19];
        for (int i = 0; i < 19; i++)
        {
            trigger[i] = false;
        }
        roomsExplored = 0;
        actualSpawn = spawns[0];

	}
	
	// Update is called once per frame
	void Update () {

        // SpawnPoint Change
        switch (roomsExplored)
        {
            case 0:
                if (trigger[1])
                {
                    roomsExplored = 1;
                    actualSpawn = spawns[1];
                }
                break;

            case 1:
                if (trigger[2] || trigger[3])
                {
                    roomsExplored = 2;
                    actualSpawn = spawns[2];
                }
                break;

            case 2:
                if (trigger[5] || trigger[6])
                {
                    roomsExplored = 3;
                    actualSpawn = spawns[3];
                }
                break;

            case 3:
                if (trigger[7] || trigger[8] || trigger[9])
                {
                    roomsExplored = 4;
                    actualSpawn = spawns[4];
                }
                break;

            case 4:
                if (trigger[12] || trigger[13])
                {
                    roomsExplored = 5;
                    actualSpawn = spawns[5];
                }
                break;

            case 5:
                if (trigger[14] || trigger[15])
                {
                    roomsExplored = 6;
                    actualSpawn = spawns[6];
                }
                break;

            case 6:
                if (trigger[1])
                {
                    roomsExplored = 7;
                }
                break;

            default:
                roomsExplored = 0;
                actualSpawn = spawns[0];
                break;
        }



    }
}
