using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1Controller : MonoBehaviour {

    public GameObject player;
    public GameObject bossHandler;
    public SceneController sceneController;
    public GameObject camera;
    public AudioClip audioClip;
    public bool[] trigger;
    [SerializeField] Transform[] spawns = new Transform[1];
    public RoomController[] roomControllers;

    public Transform actualSpawn;
    public int roomsExplored;

    private bool bossSpawn, volumeSet;

	// Use this for initialization
	void Start () {

        if (SaveSystem.LoadData() != null)
        {
            SaveData saveData = SaveSystem.LoadData();
            bossHandler.GetComponent<FirstBossBehaviour>().movingSpeed = 0f;
            trigger = new bool[19];
            for (int i = 0; i < 19; i++)
            {
                trigger[i] = false;
            }
            roomsExplored = saveData.roomsExplored;
            actualSpawn = spawns[roomsExplored];
            volumeSet = bossSpawn = false;

            for (int i = 0; i < roomControllers.Length; i++)
            {
                if (!saveData.rooms[i])
                    roomControllers[i].InstantiateEnemies();
            }
        }
        else
        {
            bossHandler.GetComponent<FirstBossBehaviour>().movingSpeed = 0f;
            trigger = new bool[19];
            for (int i = 0; i < 19; i++)
            {
                trigger[i] = false;
            }
            roomsExplored = 0;
            actualSpawn = spawns[0];
            volumeSet = bossSpawn = false;

            foreach (RoomController room in roomControllers)
            {
                room.InstantiateEnemies();
            }

        }

    }
	
	// Update is called once per frame
	void Update () {

        if (player.GetComponent<PlayerController>().spawnMe == true)
        {
            Debug.Log("Spawn Me");
            player.transform.position = actualSpawn.position;
            player.GetComponent<PlayerController>().spawnMe = false;
        }

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

        //SpawnBoss
        if (trigger[15])
        {
            if (!bossSpawn)
            {
                bossHandler.GetComponent<FirstBossBehaviour>().movingSpeed=0.005f;
                camera.GetComponent<AudioSource>().clip = audioClip;
                camera.GetComponent<AudioSource>().volume = 0;
                camera.GetComponent<AudioSource>().Play();
                bossSpawn = true;
            }
        }

        if (bossSpawn && !volumeSet)
        {
            camera.GetComponent<AudioSource>().volume += 0.001f;
            if (camera.GetComponent<AudioSource>().volume == 1f)
            {
                volumeSet = true;
            }
        }


    }

    public void BossDead()
    {
        sceneController.changeScene("Win");
    }

    public void OpenAllDoors()
    {
        foreach (RoomController room in roomControllers)
        {
            room.OpenDoors();
        }
    }

    public void SaveGame()
    {
        SaveSystem.SaveData(this, player.GetComponent<PlayerController>());
    }

}
