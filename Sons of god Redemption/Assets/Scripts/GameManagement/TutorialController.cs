using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour {

    public float rememberTextTime = 10, waitTime = 1;

    public GameObject player, wasd, lightAt, strongAt, lightElement, lightUtility, fireElement, fireUtility,
        darkElement, darkUtility, heal, dash, goodJob;
    public GameObject enemy, uselessEnemy;
    private InputManager inputManager;
    public int enemyCounter;

    public bool LightAt, Dash, StrongAt, wait, condition;
    public int state;
    public float startTime, startWait;

	// Use this for initialization
	void Start () {
        Cursor.visible = false;
        state = 0;
        startTime = Time.time;
        inputManager = GetComponent<InputManager>();

        //Initialization for the enemies spawns
        condition = wait = LightAt = Dash = StrongAt = false;
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


        // Number of enemies Controller
        foreach (var item in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            enemyCounter++;
        }
        if (state == 0)
        {
            if (Time.time - startTime > rememberTextTime)
            {
                startTime = Time.time;
                wasd.GetComponent<TutorialTextScript>().ScaleText();
            }
            if ((inputManager.xAxis != 0 || inputManager.yAxis != 0) && !wait)
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
            if (inputManager.attackButton && !condition)
            {
                LightAt = true;
                condition = true;
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
                condition = false;
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
            if (inputManager.strongAttackButton && !condition)
            {
                StrongAt = true;
                condition = true;
            }
            if (StrongAt && enemyCounter == 0)
            {
                strongAt.SetActive(false);
                dash.SetActive(true);
                startWait = Time.time;
                StrongAt = false;
                wait = true;
            }
            if (wait && Time.time - startWait > waitTime)
            {
                startTime = Time.time;
                state++;
                condition = false;
                wait = false;
            }
        }
        else if (state == 3)
        {
            if (Time.time - startTime > rememberTextTime)
            {
                startTime = Time.time;
                dash.GetComponent<TutorialTextScript>().ScaleText();
            }
            if (inputManager.dashButton)
            {
                dash.SetActive(false);
                lightElement.SetActive(true);
                startWait = Time.time;
                wait = true;
            }
            if (wait && Time.time - startWait > waitTime)
            {
                startTime = Time.time;
                state++;
                wait = false;
            }
        }
        else if (state == 4)
        {
            if (Time.time - startTime > rememberTextTime)
            {
                startTime = Time.time;
                lightElement.GetComponent<TutorialTextScript>().ScaleText();
            }
            if (inputManager.padXAxis == -1)
            {
                lightElement.SetActive(false);
                lightUtility.SetActive(true);
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
        else if (state == 5)
        {
            if (Time.time - startTime > rememberTextTime)
            {
                startTime = Time.time;
                lightUtility.GetComponent<TutorialTextScript>().ScaleText();
            }
            if (player.GetComponent<PlayerController>().lightUI.activeSelf && enemyCounter == 0 && !condition)
            {
                lightUtility.SetActive(false);
                fireElement.SetActive(true);
                startWait = Time.time;
                condition = true;
                wait = true;
            }
            if (wait && Time.time - startWait > waitTime)
            {
                startTime = Time.time;
                state++;
                condition = false;
                wait = false;
            }
        }
        else if (state == 6)
        {
            if (Time.time - startTime > rememberTextTime)
            {
                startTime = Time.time;
                fireElement.GetComponent<TutorialTextScript>().ScaleText();
            }
            if (inputManager.padXAxis == 1)
            {
                fireElement.SetActive(false);
                fireUtility.SetActive(true);
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
        else if (state == 7)
        {
            if (Time.time - startTime > rememberTextTime)
            {
                startTime = Time.time;
                fireUtility.GetComponent<TutorialTextScript>().ScaleText();
            }
            if (player.GetComponent<PlayerController>().fireUI.activeSelf && enemyCounter == 0 && !condition)
            {
                fireUtility.SetActive(false);
                darkElement.SetActive(true);
                startWait = Time.time;
                condition = true;
                wait = true;
            }
            if (wait && Time.time - startWait > waitTime)
            {
                startTime = Time.time;
                state++;
                condition = false;
                wait = false;
            }
        }
        else if (state == 8)
        {
            if (Time.time - startTime > rememberTextTime)
            {
                startTime = Time.time;
                darkElement.GetComponent<TutorialTextScript>().ScaleText();
            }
            if (inputManager.padYAxis == -1)
            {
                darkElement.SetActive(false);
                darkUtility.SetActive(true);
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
        else if (state == 9)
        {
            if (Time.time - startTime > rememberTextTime)
            {
                startTime = Time.time;
                darkUtility.GetComponent<TutorialTextScript>().ScaleText();
            }
            if (player.GetComponent<PlayerController>().darkUI.activeSelf && enemyCounter == 0 && !condition)
            {
                darkUtility.SetActive(false);
                heal.SetActive(true);
                startWait = Time.time;
                condition = true;
                wait = true;
            }
            if (wait && Time.time - startWait > waitTime)
            {
                startTime = Time.time;
                state++;
                condition = false;
                wait = false;
            }
        }
        else if (state == 10)
        {
            if (Time.time - startTime > rememberTextTime)
            {
                startTime = Time.time;
                heal.GetComponent<TutorialTextScript>().ScaleText();
            }
            if (inputManager.padYAxis == 1)
            {
                heal.SetActive(false);
                goodJob.SetActive(true);
                this.GetComponent<BoxCollider>().isTrigger = true;
                state++;
            }
        }



        enemyCounter = 0;

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
