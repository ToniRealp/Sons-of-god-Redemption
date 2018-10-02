using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    public GameObject player;
    public Vector3 offset;
    

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {


        gameObject.transform.position = player.transform.position + offset;
		
	}
}
