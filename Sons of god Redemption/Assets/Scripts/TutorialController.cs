using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour {

    public float firstEnemySpawnTime = 5;
    public int numberOfSimultaneousEnemies = 1;
    public int enemiesTillNextIncrease = 2;
    public int enemiesTillBoss = 6;

    public GameObject enemy;
    public GameObject boss;
    private InputManager inputManager;
    public int lastEnemyCounter, enemyCounter, enemiesDefeated, enemiesSpawned;

    public bool spawnEnemy, lastEnemyState, spawnBoss, lastBossState, clearAll, lastClearState, spawnFirstEnemy, enemyIncrease, spawnFirstBoss;

	// Use this for initialization
	void Start () {
        inputManager = GetComponent<InputManager>();
        spawnFirstBoss = enemyIncrease = spawnFirstEnemy = false;
        lastEnemyCounter = enemiesSpawned = enemiesDefeated = enemyCounter = 0;
    }
	
	// Update is called once per frame
	void Update () {

        GetInput();

        if(inputManager.escape)
        {
            GetComponent<SceneController>().changeScene("MainMenu");
        }

        //Spawn First Enemy
        if ((firstEnemySpawnTime-=Time.deltaTime)<=0 && !spawnFirstEnemy)
        {
            spawnFirstEnemy = true;
            DoSpawnEnemy();
            enemiesSpawned++;
        }



        // Number of enemies Controller
        foreach (var item in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            enemyCounter++;
        }
        if (spawnFirstEnemy && enemyCounter < numberOfSimultaneousEnemies  && !spawnFirstBoss)
        {
            if (lastEnemyCounter - enemyCounter > 0)
            {
                enemiesDefeated += lastEnemyCounter - enemyCounter;
            }
            if (enemiesSpawned <= enemiesTillBoss)
            {
                DoSpawnEnemy();
                enemiesSpawned++;
            }
        }

        // Aumentar el numero de enemigos simultaneos
        if (enemiesDefeated == enemiesTillNextIncrease && !enemyIncrease)
        {
            enemyIncrease = true;
            enemiesDefeated--;
            numberOfSimultaneousEnemies++;
        }

        if (enemiesDefeated == enemiesTillBoss && !spawnFirstBoss)
        {
            spawnFirstBoss = true;
            DoSpawnBoss();
        }

        if (spawnFirstBoss && enemyCounter == 0)
        {
            GetComponent<BoxCollider>().isTrigger = true;
        }



        // Manual Spawner

        if (spawnEnemy==true && lastEnemyState==false)
        {
            DoSpawnEnemy();
        }

        if (spawnBoss == true && lastBossState == false)
        {
            DoSpawnBoss();
        }

        if (clearAll == true && lastClearState == false)
        {
            DoClearAll();
        }

        lastEnemyCounter = enemyCounter;
        lastEnemyState = spawnEnemy;
        lastBossState = spawnBoss;
        lastClearState = clearAll;
        enemyCounter = 0;
	}

    void GetInput()
    {
        if (inputManager.addEnemy==true){
            spawnEnemy = true;
        }
        else
        {
            spawnEnemy = false;
        }

        if (inputManager.addBoss == true)
        {
            spawnBoss = true;
        }
        else
        {
            spawnBoss = false;
        }

        if (inputManager.clearEnemies == true)
        {
            clearAll = true;
        }
        else
        {
            clearAll = false;
        }

    }

    void DoSpawnEnemy()
    {
        Instantiate(enemy);
    }

    void DoSpawnBoss()
    {
        Instantiate(boss);
    }

    void DoClearAll()
    {
        foreach (var GameObject in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if (GameObject.GetComponent<BasicEnemy>()!=null)
            {
                Destroy(GameObject.GetComponent<BasicEnemy>().healthTextGO);
            }
            Destroy(GameObject);
        }
        ;
    }
}
