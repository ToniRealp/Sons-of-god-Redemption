using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour {

    public float movingRange = 5;
    public float changePosTime = 5;
    public NavMeshAgent NavAgent;

    private Vector3 initialPosition;
    private Vector3 destinationPosition;
    private float xMin, xMax, zMin, zMax;
    private float initTime;

    enum State { SEARCHING, SEEKING, ATTAKING };
    State state = State.SEARCHING;

    void SetMovementRange(float _x, float _z){

        xMin = _x - movingRange;
        xMax = _x + movingRange;
        zMin = _z - movingRange;
        zMax = _z + movingRange;
    }

	// Use this for initialization
	void Start () {
        initialPosition = this.GetComponent<Transform>().position;
        SetMovementRange(initialPosition.x, initialPosition.z);
        destinationPosition = new Vector3(Random.Range(xMin, xMax), 5 , Random.Range(zMin, zMax));
        initTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {

        switch (state)
        {
            case State.SEARCHING:

                // Reset destiny location
                if (Time.time - initTime >= changePosTime) {
                    destinationPosition.x = Random.Range(xMin, xMax);
                    destinationPosition.z = Random.Range(zMin, zMax);
                    
                    initTime = Time.time;
                }

                // Go to destination
                NavAgent.SetDestination(destinationPosition);

                break;
            case State.SEEKING:



                // Go to destination
                NavAgent.SetDestination(destinationPosition);


                break;
            case State.ATTAKING:
                break;
            default:
                break;
        }





	}
}
