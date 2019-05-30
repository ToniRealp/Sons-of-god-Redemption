using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour {

    public bool isEmpty, doorFlag;
    public List<GameObject> enemies;
    private List<GameObject> instEnemies;
    public List<GameObject> roomDoors;
    public Transform spawnPosition;
	// Use this for initialization
	void Start () {

        isEmpty = doorFlag = false;
        instEnemies = new List<GameObject>();
        foreach (GameObject door in roomDoors)
            door.SetActive(false);
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
        if (instEnemies.Count == 0)
        {
            OpenDoors();
            isEmpty = true;
        }
            
            
	}

    public void InstantiateEnemies()
    {
        foreach(GameObject gameObject in enemies)
        {
            instEnemies.Add(Instantiate(gameObject, spawnPosition.position, spawnPosition.rotation));
        }
    }

    public void OpenDoors()
    {
        foreach (GameObject door in roomDoors)
        {
            doorFlag = true;
            door.GetComponent<DoorScript>().GoDown();
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            foreach(var enemy in instEnemies) enemy.GetComponent<Enemy>().playerDetected = true;
            
            if (!isEmpty)
            {
                foreach (GameObject door in roomDoors)
                    door.SetActive(true); 
                foreach (GameObject door in roomDoors)
                    door.GetComponent<DoorScript>().GoUp();
            } 
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            if (!isEmpty && !doorFlag)
            {
                foreach (GameObject door in roomDoors)
                    door.GetComponent<DoorScript>().GoUp();
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            foreach (var enemy in instEnemies) enemy.GetComponent<Enemy>().playerDetected = false;
        }
    }
}
