using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Level3Controller : LevelController {

    public GameObject player;
    public GameObject bossHandler;
    public GameObject winPanel;
    public Image image;
    public Transform bossSpawnPos;
    public SceneController sceneController;
    public AudioManager audioManager;
 
    InputManager inputManager;

    private Color c;

    private bool bossSpawn, volumeSet, bossDead, changeSceneFlag;

    // Use this for initialization
    void Start()
    {
        Cursor.visible = false;
        actualSpawn = spawns[0];
        bossDead = volumeSet = bossSpawn = changeSceneFlag = false;

        trigger = new bool[1];
        trigger[0] = false;
        inputManager = gameObject.GetComponent<InputManager>();

        //Fade
        c = image.color;
        c.a = 0;
        image.color = c;
        winPanel.SetActive(false);
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
                actualSpawn = spawns[1];
                bossSpawn = true;
            }
        }

        if (bossDead)
        {
            if (c.a < 1)
            {
                c.a += 0.01f;
                image.color = c;
            }
            else
            {
                if (!changeSceneFlag)
                {
                    Cursor.visible = true;
                    sceneController.changeScene("FinalCinematic");
                }
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
        bossDead = true;
        winPanel.SetActive(true);
        
    }

    override public void OpenAllDoors()
    {

    }

    //public void SaveGame()
    //{
    //    SaveSystem.SaveData(this, player.GetComponent<PlayerController>());
    //}

}
