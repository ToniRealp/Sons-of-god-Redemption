using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour {

    public float movingRange = 5;
    public float changePosTime = 5;
    public float attackDistance = 2;
    public float attackCooldown = 0.5f;
    public float viewDistance = 15;
    public float hearDistance = 8;
    public NavMeshAgent NavAgent;
    public Animator animator;
    public GameObject player;

    public float attackAnimationTime, damagedAnimationTime, actualPosTime, actualAttackAnimationTime, actualDamagedAnimationTime, actualAttackCooldown;
    private Vector3 playerPosition, initialPosition, destinationPosition;
    private float initSpeed, xMin, xMax, zMin, zMax;
    public bool playerDetected, damaged, attackOnCooldown;
    private Quaternion quaternion;
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
        // Set initial position and movement range area
        initialPosition = this.GetComponent<Transform>().position;
        SetMovementRange(initialPosition.x, initialPosition.z);
        
        // Set random initial destination
        destinationPosition = new Vector3(Random.Range(xMin, xMax), 5 , Random.Range(zMin, zMax));

        // Valors initialization
        actualPosTime = changePosTime;
        initSpeed = NavAgent.speed;
        actualAttackCooldown = attackCooldown;
        playerDetected = damaged = false;

        // Animation time initialization
        actualAttackAnimationTime = attackAnimationTime = AnimationLength("Zombie Attack", animator);
        actualDamagedAnimationTime = damagedAnimationTime = AnimationLength("Zombie Reaction Hit", animator);

        // Raycasts arrays intantiation
        hit = new RaycastHit[37];
        ray = new Ray[37];

    }
	
	// Update is called once per frame
	void Update () {

        // Raycast direction update
        ray[0] = new Ray(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), transform.forward);
        for (int i = 1; i < 19; i++)
        {
            ray[i] = new Ray(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), new Vector3(Mathf.Sin(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y + 10*i)), 0, Mathf.Cos(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y + 10*i))));
            ray[i+18] = new Ray(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), new Vector3(Mathf.Sin(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y - 10*i)), 0, Mathf.Cos(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y - 10*i))));
        }

        // Debug Raycasting 
            // View Raycasts
        for (int i = 0; i < 5; i++)
        {
            Debug.DrawRay(ray[i].GetPoint(0), ray[i].direction * viewDistance, Color.red);
        }
            // Hear Raycasts
        for (int i = 5; i < 37; i++)
        {
            Debug.DrawRay(ray[i].GetPoint(0), ray[i].direction * hearDistance, Color.cyan);
        }

        // Raycasting Logic
        playerDetected = false;
            // View Raycasts
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
            // Hear Raycasts
        for (int i = 5; i < 37; i++)
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

        // Getting damage has to change status
        if (damaged)
        {
            state = State.DAMAGED;
        }

        // Attack On Cooldown management
        if (attackOnCooldown)
        {
            if((actualAttackCooldown -= Time.deltaTime) <= 0)
            {
                actualAttackCooldown = attackCooldown;
                attackOnCooldown = false;
            }
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

        // PlayerPosition update
        playerPosition = player.GetComponent<Transform>().position;


        // State Machine
        switch (state)
        {
            case State.SEARCHING:
                // Full speed
                NavAgent.speed = initSpeed;
                // Go to random destination
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
                // Full speed
                NavAgent.speed = initSpeed;
                // Go to player Position
                destinationPosition = playerPosition;
                NavAgent.SetDestination(playerPosition);
                // Look to player
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(playerPosition.x - transform.position.x, playerPosition.y - transform.position.y, playerPosition.z - transform.position.z)), 0.1f);

                if (playerDetected)
                {
                    // If in attack conditions, go to attack
                    if (Vector3.Distance(gameObject.transform.position,player.transform.position) <= attackDistance && !attackOnCooldown)
                    {
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
                // No movement or rotation
                NavAgent.speed = 0;
                // Save player last known position
                destinationPosition = playerPosition;
                NavAgent.SetDestination(playerPosition);

                // When attack time finishes
                actualAttackAnimationTime -= Time.deltaTime;
                if (actualAttackAnimationTime - attackAnimationTime/2 <= 0)
                    animator.SetBool("Attack", false);
                if (actualAttackAnimationTime <= 0)
                {
                    //Put it on cooldown and change status
                    attackOnCooldown = true;
                    actualAttackAnimationTime = attackAnimationTime;
                
                    if (playerDetected)
                    {
                        state = State.CHASING;
                    }
                    else
                    {
                        state = State.SEARCHING;
                    }
                }
                break;

            case State.DAMAGED:
                // Cancel attack animation if getting hit
                animator.SetBool("Attack", false);
                // No movement
                NavAgent.speed = 0;
                // But rotation
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(playerPosition.x - transform.position.x, playerPosition.y - transform.position.y, playerPosition.z - transform.position.z)), 0.1f);
                
                // When damage animation finishes change state
                actualDamagedAnimationTime -= Time.deltaTime;
                if (actualDamagedAnimationTime<=0)
                {
                    actualDamagedAnimationTime = damagedAnimationTime;
                    damaged = false;
                    if (playerDetected)
                    {
                        state = State.CHASING;
                    }
                    else
                    {
                        state = State.SEARCHING;
                    }
                }
                break;
            default:
                break;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Weapon" && !damaged)
        {
            damaged = true;
            Debug.Log("hit");
            animator.SetTrigger("Damaged");
        }
    }

    private void OnTriggerExit(Collider other)
    {

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
