using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour {

    public float movingRange = 5;
    public float changePosTime = 5;
    public float attackDistance = 2;
    public float attackCooldown = 0.5f;
    public float attackAnimationTime = 2.617f;
    public float damagedAnimationTime = 2.167f;
    public Rigidbody rb;
    public NavMeshAgent NavAgent;
    public Animator animator;
    public GameObject player;
    public int status;

    private Vector3 playerPosition;
    private Vector3 initialPosition;
    private float initSpeed;
    public Vector3 destinationPosition;
    private float xMin, xMax, zMin, zMax;
    private float initTime, initAttackTime, initDamagedTime;
    public bool playerDetected = false;
    private bool attackOnCooldown = false;
    public bool damaged = false;

    enum State { SEARCHING, CHASING, ATTAKING, DAMAGED };
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
        initSpeed = NavAgent.speed;
        destinationPosition = new Vector3(Random.Range(xMin, xMax), 5 , Random.Range(zMin, zMax));
        initTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {

        if (damaged)
        {
            state = State.DAMAGED;
        }

        //if (state == State.ATTAKING)
        //{
        //    GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        //}
        //else
        //{
        //    GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        //}

        // Set animation state depending on speed
        if (NavAgent.velocity.magnitude>0.1f)
        {
            animator.SetBool("IsIdle", false);
            animator.SetBool("IsRunning", true);
        }
        else
        {
            animator.SetBool("IsRunning", false);
            animator.SetBool("IsIdle", true);
        }

        // PlayerPosition
        playerPosition = player.GetComponent<Transform>().position;

        // Debug Status
        status = (int)state;

        switch (state)
        {
            case State.SEARCHING:
                NavAgent.SetDestination(destinationPosition);
                if (!playerDetected) { 

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
                    state = State.CHASING;
                }

                break;
            case State.CHASING:
                destinationPosition = playerPosition;
                NavAgent.SetDestination(playerPosition);
                initTime = Time.time;
                if (playerDetected)
                {
                    if (Vector3.Distance(gameObject.transform.position,player.transform.position) <= attackDistance)
                    {
                        initAttackTime = Time.time;
                        NavAgent.speed = 0;
                        state = State.ATTAKING;
                        animator.SetBool("Attack",true);
                    }
                }
                else
                {
                    state = State.SEARCHING;
                }

                break;
            case State.ATTAKING:
                destinationPosition = playerPosition;
                NavAgent.SetDestination(playerPosition);
                initTime = Time.time;
                if (playerDetected)
                {
                    if (Time.time - initAttackTime >= attackAnimationTime/2)
                        animator.SetBool("Attack", false);
                    if (Time.time - initAttackTime >= attackAnimationTime) 
                        NavAgent.speed = 0.1f;
                    if (Time.time - initAttackTime >= attackAnimationTime+attackCooldown) {
                        initAttackTime = Time.time;
                        NavAgent.speed = initSpeed;
                        state = State.CHASING;
                    }
                }
                else
                {
                    NavAgent.speed = initSpeed;
                    state = State.SEARCHING;
                }
                break;
            case State.DAMAGED:
                animator.SetBool("Attack", false);
                if (Time.time - initDamagedTime < damagedAnimationTime)
                {
                    NavAgent.speed = 0.1f;
                }
                if (Time.time - initDamagedTime >= damagedAnimationTime)
                {
                    damaged = false;
                    NavAgent.speed = initSpeed;
                    state = State.CHASING;
                }

                break;
            default:
                break;
        }




    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            playerDetected = true;
        }
    }

     private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerDetected = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name == "RightHandThumb1" && player.GetComponent<PlayerController>().attacked)
        {
            damaged = true;
            initDamagedTime = Time.time;
            animator.SetTrigger("Damaged");
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.name == "RightHandThumb1")
        {
            damaged = false;
        }
    }
}
