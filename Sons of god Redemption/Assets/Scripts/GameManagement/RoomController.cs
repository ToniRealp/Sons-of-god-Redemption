using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour {

    bool isEmpty;
    public List<GameObject> enemies;
    public List<GameObject> roomDoors;
    Transform spawnPosition;
	// Use this for initialization
	void Start () {

        isEmpty = false;

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void InstantiateEnemies()
    {
        foreach(GameObject gameObject in enemies)
        {
            Instantiate(gameObject, spawnPosition.position, spawnPosition.rotation);
        }
    }
}
