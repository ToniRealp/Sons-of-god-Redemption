using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour {

    public float movingRange = 5;
    public float changePosTime = 5;
    public float attackDistance = 2;
    public float attackCooldown = 0.5f;
    public float attackAnimationTime;
    public float damagedAnimationTime;
    public float viewDistance = 15;
    public float hearDistance = 8;
    public NavMeshAgent NavAgent;
    public Animator animator;
    public GameObject player;

    private float actualPosTime, actualAttackAnimationTime, actualDamagedAnimationTime;
    private Vector3 playerPosition, initialPosition, destinationPosition;
    private float initSpeed, xMin, xMax, zMin, zMax;
    public bool playerDetected, damaged;
    private RaycastHit[] hit;
    private Ray[] ray;

    enum State { SEARCHING, CHASING, ATTAKING, DAMAGED };
    [SerializeField] State state = State.SEARCHING;

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
        actualPosTime = changePosTime;
        playerDetected = damaged = false;
        attackAnimationTime = AnimationLength("Zombie Attack", animator);
        damagedAnimationTime = AnimationLength("Zombie Reaction Hit", animator);
        hit = new RaycastHit[33];
        ray = new Ray[33];
    }
	
	// Update is called once per frame
	void Update () {

        ray[0] = new Ray(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), transform.forward);
        for (int i = 1; i < 17; i++)
        {
            ray[i] = new Ray(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), new Vector3(Mathf.Sin(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y + 10*i)), 0, Mathf.Cos(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y + 10*i))));
            ray[i+16] = new Ray(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), new Vector3(Mathf.Sin(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y - 10*i)), 0, Mathf.Cos(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y - 10*i))));
        }

        //// Vision Rays
        //ray[0] = new Ray(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), transform.forward);
        //ray[1] = new Ray(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), new Vector3(Mathf.Sin(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y + 10)), 0, Mathf.Cos(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y + 10))));
        //ray[2] = new Ray(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), new Vector3(Mathf.Sin(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y - 10)), 0, Mathf.Cos(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y - 10))));
        //ray[3] = new Ray(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), new Vector3(Mathf.Sin(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y + 20)), 0, Mathf.Cos(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y + 20))));
        //ray[4] = new Ray(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), new Vector3(Mathf.Sin(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y - 20)), 0, Mathf.Cos(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y - 20))));

        //// Hear Rays
        //ray[5] = new Ray(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), new Vector3(Mathf.Sin(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y + 30)), 0, Mathf.Cos(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y + 30))));
        //ray[6] = new Ray(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), new Vector3(Mathf.Sin(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y - 30)), 0, Mathf.Cos(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y - 30))));
        //ray[7] = new Ray(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), new Vector3(Mathf.Sin(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y + 40)), 0, Mathf.Cos(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y + 40))));
        //ray[8] = new Ray(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), new Vector3(Mathf.Sin(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y - 40)), 0, Mathf.Cos(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y - 40))));
        //ray[9] = new Ray(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), new Vector3(Mathf.Sin(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y + 50)), 0, Mathf.Cos(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y + 50))));
        //ray[10] = new Ray(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), new Vector3(Mathf.Sin(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y - 50)), 0, Mathf.Cos(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y - 50))));
        //ray[11] = new Ray(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), new Vector3(Mathf.Sin(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y + 60)), 0, Mathf.Cos(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y + 60))));
        //ray[12] = new Ray(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), new Vector3(Mathf.Sin(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y - 60)), 0, Mathf.Cos(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y - 60))));
        //ray[5] = new Ray(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), new Vector3(Mathf.Sin(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y + 30)), 0, Mathf.Cos(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y + 70))));
        //ray[6] = new Ray(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), new Vector3(Mathf.Sin(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y - 30)), 0, Mathf.Cos(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y - 70))));
        //ray[7] = new Ray(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), new Vector3(Mathf.Sin(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y + 40)), 0, Mathf.Cos(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y + 80))));
        //ray[8] = new Ray(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), new Vector3(Mathf.Sin(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y - 40)), 0, Mathf.Cos(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y - 80))));
        //ray[9] = new Ray(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), new Vector3(Mathf.Sin(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y + 50)), 0, Mathf.Cos(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y + 90))));
        //ray[10] = new Ray(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), new Vector3(Mathf.Sin(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y - 50)), 0, Mathf.Cos(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y - 90))));
        //ray[11] = new Ray(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), new Vector3(Mathf.Sin(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y + 60)), 0, Mathf.Cos(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y + 100))));
        //ray[12] = new Ray(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), new Vector3(Mathf.Sin(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y - 60)), 0, Mathf.Cos(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y - 100))));


        // Debug Raycasting 
        for (int i = 0; i < 5; i++)
        {
            Debug.DrawRay(ray[i].GetPoint(0), ray[i].direction * viewDistance, Color.red);
        }
        for (int i = 5; i < 33; i++)
        {
            Debug.DrawRay(ray[i].GetPoint(0), ray[i].direction * hearDistance, Color.cyan);
        }


        playerDetected = false;

        for (int i = 0; i < 5; i++)
        {
            if (Physics.Raycast(ray[i], out hit[i], viewDistance))
            {
                if (hit[i].collider.gameObject.tag == "Player")
                {
                    playerDetected = true;
                    playerPosition = hit[i].collider.gameObject.transform.position;
                }


            }
        }
        for (int i = 5; i < 33; i++)
        {
            if (Physics.Raycast(ray[i], out hit[i], hearDistance))
            {
                if (hit[i].collider.gameObject.tag == "Player")
                {
                    playerDetected = true;
                    playerPosition = hit[i].collider.gameObject.transform.position;
                }


            }
        }


        if (damaged)
        {
            state = State.DAMAGED;
        }


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

        switch (state)
        {
            case State.SEARCHING:
                NavAgent.SetDestination(destinationPosition);
                if (!playerDetected) { 

                    // Reset destiny location
                    if ((actualPosTime-=Time.deltaTime)<=0)
                    {
                        destinationPosition.x = Random.Range(xMin, xMax);
                        destinationPosition.z = Random.Range(zMin, zMax);
                        actualPosTime = changePosTime;
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
                if (playerDetected)
                {
                    if (Vector3.Distance(gameObject.transform.position,player.transform.position) <= attackDistance)
                    {
                        NavAgent.speed = 0;
                        state = State.ATTAKING;
                        animator.SetTrigger("Attack");
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
                if (playerDetected)
                {
                    actualAttackAnimationTime -= Time.deltaTime;
                    if (actualAttackAnimationTime<=0) 
                        NavAgent.speed = 0.1f;
                    if (actualAttackAnimationTime+attackCooldown<=0) {
                        actualAttackAnimationTime = attackAnimationTime;
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
                actualDamagedAnimationTime -= Time.deltaTime;
                NavAgent.speed = 0.1f;
                if (actualDamagedAnimationTime<=0)
                {
                    actualDamagedAnimationTime = damagedAnimationTime;
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
        if (other.tag == "Weapon" && !damaged)
        {
            damaged = true;
            Debug.Log("hit");
            animator.SetTrigger("Damaged");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerDetected = false;
        }
        if (other.tag == "Weapon")
        {
            damaged = false;
        }
    }

    float AnimationLength(string animName, Animator animator)
    {
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;

        for (int i = 0; i < clips.Length; i++)
        {
            if (clips[i].name == animName)
                return (clips[i].length);
        }
        return -1f;
    }
}
