using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    public GameObject player;
    public Vector3 offset;

	void Update () {
        gameObject.transform.position = player.transform.position + offset;
	}
}
