using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour {

    public float walkingSpeed = 3;
    public float movingRange = 5;
    public float changePosTime = 15;
    public float distanceAproach = 0.1f; // Never put this to 0
    public float rotationSpeed = 0.05f;


    private Vector3 initialPosition;
    public Vector3 destinationPosition;
    public Vector3 travelVector;
    private float xMin, xMax, zMin, zMax, xDest, zDest;
    private float initTime;

    enum State { SEARCHING, SEEKING, ATTAKING };
    State state = State.SEARCHING;

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
        destinationPosition.x = xDest = Random.Range(xMin, xMax);
        destinationPosition.y = initialPosition.y;
        destinationPosition.z = zDest = Random.Range(zMin, zMax);
        initTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {

        switch (state)
        {
            case State.SEARCHING:

                // Calculate travel vector
                travelVector = new Vector3(destinationPosition.x - this.GetComponent<Transform>().position.x,0, destinationPosition.z - this.GetComponent<Transform>().position.z);

                // Rotate and go to destiny location
                if (travelVector.magnitude > distanceAproach) {
                    this.GetComponent<Rigidbody>().rotation = Quaternion.Slerp(this.GetComponent<Rigidbody>().rotation, Quaternion.LookRotation(travelVector), rotationSpeed);
                    this.GetComponent<Rigidbody>().velocity = new Vector3(travelVector.normalized.x*walkingSpeed, 0, travelVector.normalized.z*walkingSpeed);
                }
                else {
                    this.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
                }


                // Reset destiny location
                if (Time.time - initTime >= changePosTime) {
                    destinationPosition.x = xDest = Random.Range(xMin, xMax);
                    destinationPosition.z = zDest = Random.Range(zMin, zMax);
                    
                    initTime = Time.time;
                }

                break;
            case State.SEEKING:
                break;
            case State.ATTAKING:
                break;
            default:
                break;
        }





	}
}
