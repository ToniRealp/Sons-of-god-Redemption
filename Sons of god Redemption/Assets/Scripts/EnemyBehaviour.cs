using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour {

    public float movingRange = 5;
    public float changePosTime = 5;

    private Vector3 initialPosition;
    private Vector3 destinationPosition;
    private float xMin, xMax, zMin, zMax;
    private float initTime;

    void setMovementRange(float _x, float _z){
        xMin = _x - movingRange;
        xMax = _x + movingRange;
        zMin = _z - movingRange;
        zMax = _z + movingRange;
    }

	// Use this for initialization
	void Start () {
        initialPosition = this.GetComponent<Transform>().position;
        setMovementRange(initialPosition.x, initialPosition.z);
        initTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
        if (Time.time - initTime >= changePosTime)
        {

        }




	}
}
