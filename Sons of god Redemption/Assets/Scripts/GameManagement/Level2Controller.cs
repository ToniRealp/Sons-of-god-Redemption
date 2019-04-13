using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level2Controller : LevelController {

    public GameObject player;
    public GameObject bossHandler;
    public GameObject bossDoor;
    public Transform bossSpawnPos;
    public SceneController sceneController;
    public AudioManager audioManager;
    InputManager inputManager;

    private bool bossSpawn, volumeSet;

    // Use this for initialization
    void Start()
    {
        LoadGame();
        volumeSet = bossSpawn = false;
        trigger = new bool[9];
        for (int i = 0; i < 9; i++)
        {
            trigger[i] = false;
        }
        inputManager = gameObject.GetComponent<InputManager>();
    }

    // Update is called once per frame
    void Update()
    {

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
                if (trigger[0] || trigger[1])
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
                if (trigger[4] || trigger[5])
                {
                    roomsExplored = 3;
                    actualSpawn = spawns[3];
                }
                break;

            case 3:
                if (trigger[6] || trigger[7])
                {
                    roomsExplored = 4;
                    actualSpawn = spawns[4];
                }
                break;

            default:
                break;
        }

        //SpawnBoss
        if (trigger[8])
        {
            actualSpawn = spawns[5];
            if (!bossSpawn)
            {
                Instantiate(bossHandler, bossSpawnPos.position, bossSpawnPos.rotation);
                player.GetComponent<PlayerController>().onCinematic = true;
                //audioManager.Play("BossTheme");

                bossSpawn = true;
                //Instantiate(bossHandler.GetComponent<SecondBossBehaviour>().title, bossHandler.GetComponent<SecondBossBehaviour>().canvas.transform);
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
        SaveSystem.SaveData(this, player.GetComponent<PlayerController>(),2);
    }

}
