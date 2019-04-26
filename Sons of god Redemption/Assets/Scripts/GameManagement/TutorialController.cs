using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour {

    public float rememberTextTime = 10, waitTime = 1;

    public GameObject wasd, lightAt, strongAt, lightElement, lightUtility, fireElement, fireUtility,
        darkElement, darkUtility, dash, goodJob;
    public GameObject enemy, uselessEnemy;
    private InputManager inputManager;
    public int enemyCounter;

    public bool LightAt, Dash, StrongAt, Element, openDoor, wait;
    private int state;
    private float startTime, startWait;

	// Use this for initialization
	void Start () {
        state = 0;
        startTime = Time.time;
        inputManager = GetComponent<InputManager>();

        //Initialization for the enemies spawns
        wait = LightAt = openDoor = Dash = StrongAt = Element = false;
        enemyCounter = 0;

        //Initialization for the tutorial texts
        wasd.SetActive(true);
        lightAt.SetActive(false);
        strongAt.SetActive(false);
        lightElement.SetActive(false);
        lightUtility.SetActive(false);
        fireElement.SetActive(false);
        fireUtility.SetActive(false);
        darkElement.SetActive(false);
        darkUtility.SetActive(false);
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
        Debug.Log("State: " + state);
        if (state == 0)
        {
            if (Time.time - startTime > rememberTextTime)
            {
                startTime = Time.time;
                wasd.GetComponent<TutorialTextScript>().ScaleText();
            }
            if (inputManager.xAxis != 0 || inputManager.yAxis != 0)
            {
                wasd.SetActive(false);
                lightAt.SetActive(true);
                startWait = Time.time;
                wait = true;
            }
            if (wait && Time.time - startWait > waitTime)
            {
                DoSpawnUselessEnemy();
                startTime = Time.time;
                state++;
                wait = false;
            }
        }
        else if (state == 1)
        {
            if (Time.time - startTime > rememberTextTime)
            {
                startTime = Time.time;
                lightAt.GetComponent<TutorialTextScript>().ScaleText();
            }
            if (inputManager.attackButton)
            {
                LightAt = true;
            }
            if (LightAt && enemyCounter == 0)
            {

                lightAt.SetActive(false);
                strongAt.SetActive(true);
                startWait = Time.time;
                LightAt = false;
                wait = true;
            }
            if (wait && Time.time - startWait > waitTime)
            {
                DoSpawnUselessEnemy();
                startTime = Time.time;
                state++;
                wait = false;
            }
        }
        else if (state == 2)
        {
            if (Time.time - startTime > rememberTextTime)
            {
                startTime = Time.time;
                strongAt.GetComponent<TutorialTextScript>().ScaleText();
            }
            if (inputManager.strongAttackButton)
            {
                StrongAt = true;
            }
            if (strongAt && enemyCounter == 0)
            {
                strongAt.SetActive(false);
                lightElement.SetActive(true);
                startWait = Time.time;
                StrongAt = false;
                wait = true;
            }
            if (wait && Time.time - startWait > waitTime)
            {
                DoSpawnUselessEnemy();
                startTime = Time.time;
                state++;
                wait = false;
            }
        }


        enemyCounter = 0;

        //Spawn Light At Enemy
        //       if ((firstEnemySpawnTime-=Time.deltaTime)<=0 && spawnLightAtEnemy)
        //       {
        //           Debug.Log("LightEnemy");
        //           DoSpawnEnemy();
        //           spawnLightAtEnemy = false;
        //           spawnDashEnemy = true;
        //           wasd.SetActive(false);
        //           lightAt.SetActive(true);
        //       }

        //       //Spawn Dash Enemy
        //       else if (enemyCounter == 0 && spawnDashEnemy)
        //       {
        //           Debug.Log("DashEnemy");
        //           DoSpawnEnemy();
        //           spawnDashEnemy = false;
        //           spawnStrongAtEnemies = true;
        //           lightAt.SetActive(false);
        //           dash.SetActive(true);
        //       }

        //       //Spawn Strong At Enemies
        //       else if (enemyCounter == 0 && spawnStrongAtEnemies)
        //       {
        //           Debug.Log("StrongEnemy");
        //           DoSpawnEnemy();
        //           DoSpawnEnemy();
        //           spawnStrongAtEnemies = false;
        //           spawnElementEnemies = true;
        //           dash.SetActive(false);
        //           strongAt.SetActive(true);
        //       }

        //       //Spawn Element Enemies
        //       else if (enemyCounter == 0 && spawnElementEnemies)
        //       {
        //           Debug.Log("ElementEnemy");
        //           DoSpawnEnemy();
        //           DoSpawnEnemy();
        //           spawnElementEnemies = false;
        //           openDoor = true;
        //           strongAt.SetActive(false);
        //           elements.SetActive(true);
        //       }

        //       //Open Door
        //       else if (enemyCounter == 0 && openDoor)
        //       {
        //           Debug.Log("OpenDoor");
        //           gameObject.GetComponent<BoxCollider>().isTrigger = true;
        //           openDoor = false;
        //           elements.SetActive(false);
        //           goodJob.SetActive(true);
        //       }


        //       enemyCounter = 0;
    }


    void DoSpawnEnemy()
    {
        Instantiate(enemy);
    }

    void DoSpawnUselessEnemy()
    {
        Instantiate(uselessEnemy);
    }

}
