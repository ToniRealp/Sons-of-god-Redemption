using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour {

    public float firstEnemySpawnTime = 5;

    public GameObject wasd, lightAt, strongAt, elements, dash, goodJob;
    public GameObject enemy;
    public GameObject boss;
    private InputManager inputManager;
    public int enemyCounter;

    public bool spawnLightAtEnemy, spawnDashEnemy, spawnStrongAtEnemies, spawnElementEnemies, openDoor;

	// Use this for initialization
	void Start () {
        inputManager = GetComponent<InputManager>();

        //Initialization for the enemies spawns
        spawnLightAtEnemy = true;
        openDoor = spawnDashEnemy = spawnStrongAtEnemies = spawnElementEnemies = false;
        enemyCounter = 0;

        //Initialization for the tutorial texts
        wasd.SetActive(true);
        lightAt.SetActive(false);
        strongAt.SetActive(false);
        elements.SetActive(false);
        dash.SetActive(false);
        goodJob.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {

        //GetInput();

        if(inputManager.escape)
        {
            GetComponent<SceneController>().changeScene("MainMenu");
        }

        // Number of enemies Controller
        foreach (var item in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            enemyCounter++;
        }


        //Spawn Light At Enemy
        if ((firstEnemySpawnTime-=Time.deltaTime)<=0 && spawnLightAtEnemy)
        {
            Debug.Log("LightEnemy");
            DoSpawnEnemy();
            spawnLightAtEnemy = false;
            spawnDashEnemy = true;
            wasd.SetActive(false);
            lightAt.SetActive(true);
        }

        //Spawn Dash Enemy
        else if (enemyCounter == 0 && spawnDashEnemy)
        {
            Debug.Log("DashEnemy");
            DoSpawnEnemy();
            spawnDashEnemy = false;
            spawnStrongAtEnemies = true;
            lightAt.SetActive(false);
            dash.SetActive(true);
        }

        //Spawn Strong At Enemies
        else if (enemyCounter == 0 && spawnStrongAtEnemies)
        {
            Debug.Log("StrongEnemy");
            DoSpawnEnemy();
            DoSpawnEnemy();
            spawnStrongAtEnemies = false;
            spawnElementEnemies = true;
            dash.SetActive(false);
            strongAt.SetActive(true);
        }

        //Spawn Element Enemies
        else if (enemyCounter == 0 && spawnElementEnemies)
        {
            Debug.Log("ElementEnemy");
            DoSpawnEnemy();
            DoSpawnEnemy();
            spawnElementEnemies = false;
            openDoor = true;
            strongAt.SetActive(false);
            elements.SetActive(true);
        }

        //Open Door
        else if (enemyCounter == 0 && openDoor)
        {
            Debug.Log("OpenDoor");
            gameObject.GetComponent<BoxCollider>().isTrigger = true;
            openDoor = false;
            elements.SetActive(false);
            goodJob.SetActive(true);
        }


        enemyCounter = 0;
	}


    void DoSpawnEnemy()
    {
        Instantiate(enemy);
    }

}
