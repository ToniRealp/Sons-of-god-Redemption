using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour {

    bool isEmpty;
    public List<GameObject> enemies;
    public List<GameObject> instEnemies;
    public List<GameObject> roomDoors;
    public Transform spawnPosition;
	// Use this for initialization
	void Start () {

        isEmpty = false;
        instEnemies = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update ()
    {
		foreach(GameObject enemy in instEnemies)
        {
            if (!enemy)
            {
                instEnemies.Remove(enemy);
            }
                
        }
        Debug.Log(instEnemies.Count);
        if (instEnemies.Count == 0)
            Debug.Log("All enemies have been destroyed");
            
	}

    public void InstantiateEnemies()
    {
        foreach(GameObject gameObject in enemies)
        {
            instEnemies.Add(Instantiate(gameObject, spawnPosition.position, spawnPosition.rotation));
        }
    }
}
