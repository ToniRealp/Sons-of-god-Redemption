using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1Controller : LevelController {

    public GameObject player;
    public GameObject bossHandler;
    public GameObject bossDoor;
    public SceneController sceneController;
    public AudioManager audioManager;
       
    InputManager inputManager;

    
 

    private bool bossSpawn, volumeSet;

	// Use this for initialization
	void Start () {

        LoadGame();

        volumeSet = bossSpawn = false;
        trigger = new bool[19];
        for (int i = 0; i < 19; i++)
        {
            trigger[i] = false;
        }
        bossHandler.GetComponent<FirstBossBehaviour>().movingSpeed = 0f;
        inputManager = gameObject.GetComponent<InputManager>();
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
                bossHandler.GetComponent<FirstBossBehaviour>().movingSpeed=0.02f;
                bossHandler.GetComponent<FirstBossBehaviour>().StandUp();
                //audioManager.Play("BossTheme");

                bossSpawn = true;
            }
        }

        //float volume = 0;
        //float volume2 = 1;
        //if (bossSpawn && !volumeSet)
        //{
        //    volume += 0.001f;
        //    volume2 -= 0.001f;
        //    audioManager.SetVolume("BossTheme", volume);
        //    audioManager.SetVolume("MainTheme", volume2);
        //    if (audioManager.GetVolume("BossTheme") == 1f)
        //    {
        //        volumeSet = true;
        //        audioManager.Stop("MainTheme");
        //    }
        //}


    }

    public void BossDead()
    {
        bossDoor.SetActive(false);
    }

    override public void OpenAllDoors()
    {
        foreach (RoomController room in roomControllers)
        {
            room.OpenDoors();
        }
    }

    public void SaveGame()
    {
        SaveSystem.SaveData(this, player.GetComponent<PlayerController>(),1);
    }

}
