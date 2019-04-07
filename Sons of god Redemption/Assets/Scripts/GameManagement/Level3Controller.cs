using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level3Controller : LevelController {

    public GameObject player;
    public GameObject bossHandler;
    public Transform bossSpawnPos;
    public SceneController sceneController;
    public AudioManager audioManager;
 
    InputManager inputManager;

   

    private bool bossSpawn, volumeSet;

    // Use this for initialization
    void Start()
    {
        actualSpawn = spawns[0];
        volumeSet = bossSpawn = false;

        trigger = new bool[1];
        trigger[0] = false;
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

        //SpawnBoss
        if (trigger[0])
        {
            if (!bossSpawn)
            {
                Instantiate(bossHandler, bossSpawnPos.position, bossSpawnPos.rotation);
                //audioManager.Play("GodTheme");

                bossSpawn = true;
            }
        }

        //float volume = 0;
        //float volume2 = 1;
        //if (bossSpawn && !volumeSet)
        //{
        //    volume += 0.001f;
        //    volume2 -= 0.001f;
        //    audioManager.SetVolume("GodTheme", volume);
        //    audioManager.SetVolume("MainTheme", volume2);
        //    if (audioManager.GetVolume("GodTheme") == 1f)
        //    {
        //        volumeSet = true;
        //        audioManager.Stop("MainTheme");
        //    }
        //}


    }

    public void BossDead()
    {
        sceneController.changeScene("FinalCinematic");
    }

    override public void OpenAllDoors()
    {

    }

    //public void SaveGame()
    //{
    //    SaveSystem.SaveData(this, player.GetComponent<PlayerController>());
    //}

}
