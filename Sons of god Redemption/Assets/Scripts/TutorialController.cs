using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour {

    public GameObject enemy;
    public GameObject boss;
    private InputManager inputManager;

    private bool spawnEnemy, lastEnemyState, spawnBoss, lastBossState, clearAll, lastClearState;

	// Use this for initialization
	void Start () {
        inputManager = GetComponent<InputManager>();
    }
	
	// Update is called once per frame
	void Update () {

        GetInput();

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

        lastEnemyState = spawnEnemy;
        lastBossState = spawnBoss;
        lastClearState = clearAll;
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
            if (GameObject.GetComponent<EnemyBehaviour>()!=null)
            {
                Destroy(GameObject.GetComponent<EnemyBehaviour>().healthTextGO);
            }
            Destroy(GameObject);
        }
        ;
    }
}
