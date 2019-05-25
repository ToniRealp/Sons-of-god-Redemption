using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pruebadoor : MonoBehaviour {

    public GameObject door;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    private void OnTriggerEnter(Collider other)
    {
        door.GetComponent<DoorScript>().GoDown();
    }

    private void OnTriggerExit(Collider other)
    {
        door.GetComponent<DoorScript>().GoUp();
    }
}
