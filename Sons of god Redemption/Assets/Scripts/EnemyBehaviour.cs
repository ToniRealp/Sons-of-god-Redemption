using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour {

    public float movingRange = 5;
    public float changePosTime = 5;
    public float attackDistance = 2;
    public float attackAnimationTime = 2;
    public NavMeshAgent NavAgent;

    private Vector3 initialPosition;
    public Vector3 destinationPosition;
    private float xMin, xMax, zMin, zMax;
    private float initTime, initAttackTime;
    public bool playerDetected = false;

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

        // Go to destination
        NavAgent.SetDestination(destinationPosition);

        switch (state)
        {
            case State.SEARCHING:
                //Debug.Log("SEARCHING");
                if (!playerDetected)
                {
                    // Reset destiny location
                    if (Time.time - initTime >= changePosTime)
                    {
                        destinationPosition.x = Random.Range(xMin, xMax);
                        destinationPosition.z = Random.Range(zMin, zMax);

                        initTime = Time.time;
                    }
                }
                else
                {
                    state = State.SEEKING;
                }


                break;
            case State.SEEKING:
                if (NavAgent.remainingDistance <= attackDistance) {
                    initAttackTime = Time.time;
                    NavAgent.isStopped = true;
                    state = State.ATTAKING;
                }

                break;
            case State.ATTAKING:
                if (Time.time - initAttackTime >= attackAnimationTime) {
                    NavAgent.isStopped = false;
                    state = State.SEEKING;
                }
                    break;
            default:
                break;
        }


	}
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerDetected = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log(other.tag);
        if (other.CompareTag("Player"))
        {
            destinationPosition = other.GetComponent<Transform>().position;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerDetected = false;
            state = State.SEARCHING;
        }
    }
}
