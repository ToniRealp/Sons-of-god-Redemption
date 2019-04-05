using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level3Controller : MonoBehaviour {

    public GameObject player;
    public GameObject bossHandler;
    public Transform bossSpawnPos;
    public SceneController sceneController;
    public GameObject camera;
    public AudioClip audioClip;
    public bool[] trigger;
    [SerializeField] Transform[] spawns = new Transform[1];
    public RoomController[] roomControllers;
    InputManager inputManager;

    public Transform actualSpawn;
    public int roomsExplored;

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
                camera.GetComponent<AudioSource>().clip = audioClip;
                camera.GetComponent<AudioSource>().volume = 0;
                camera.GetComponent<AudioSource>().Play();
                bossSpawn = true;
                actualSpawn = spawns[1];
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

    }

    //public void SaveGame()
    //{
    //    SaveSystem.SaveData(this, player.GetComponent<PlayerController>());
    //}

}
