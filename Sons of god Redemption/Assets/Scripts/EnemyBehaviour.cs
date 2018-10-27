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
    public float hearDistance = 5;
    public NavMeshAgent NavAgent;
    public Animator animator;
    public GameObject player;

    private float actualPosTime, actualAttackAnimationTime, actualDamagedAnimationTime;
    private Vector3 playerPosition, initialPosition, destinationPosition;
    private float initSpeed, xMin, xMax, zMin, zMax;
    public bool playerDetected, damaged;
    private RaycastHit hit1, hit2, hit3;
    private Ray ray1, ray2, ray3;


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

    }
	
	// Update is called once per frame
	void Update () {

        ray1 = new Ray(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), transform.forward);
        ray2 = new Ray(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), new Vector3(Mathf.Sin(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y + 20)), 0, Mathf.Cos(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y + 20))));
        ray3 = new Ray(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), new Vector3(Mathf.Sin(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y - 20)), 0, Mathf.Cos(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y - 20))));

        // Raycasting 
        Debug.DrawRay(ray1.GetPoint(0), ray1.direction * viewDistance, Color.red);
        Debug.DrawRay(ray2.GetPoint(0), ray2.direction * viewDistance, Color.cyan);
        Debug.DrawRay(ray3.GetPoint(0), ray3.direction * viewDistance, Color.yellow);

        if (Physics.Raycast(ray1, out hit1, viewDistance)){
            if (hit1.collider.gameObject.tag == "Player")
            {
                playerDetected = true;
            }


        }
        if (Physics.Raycast(ray2, out hit2, viewDistance))
        {
            if (hit2.collider.gameObject.tag == "Player")
            {
                playerDetected = true;
            }

        }
        if (Physics.Raycast(ray3, out hit3, viewDistance))
        {
            if (hit3.collider.gameObject.tag == "Player")
            {
                playerDetected = true;
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
