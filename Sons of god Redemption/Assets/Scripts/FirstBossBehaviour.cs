using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstBossBehaviour : MonoBehaviour
{

    private GameObject fireParticles;
    public Animator animator;
    public float attackDistance = 3.5f, attackMinInterval = 2, attackMaxInterval = 4;
    public int randomAttack;
    public float movingSpeed = 0.05f, rotationSpeed = 0.05f;

    private GameObject player;
    private float preJumpAnimationTime, jumpAnimationTime, roarAnimationTime, swipeAnimationTime;
    private float actualAttackInterval, actualPreJumpTime, actualJumpTime, actualRoarTime, actualSwipeTime;

    enum State { IDLE, WALKING, SWIPEATTACK, PREJUMP, JUMP, ROAR, DAMAGED };
    [SerializeField] State state = State.IDLE;

    public float playerDistance;



    // Use this for initialization
    void Start()
    {
        fireParticles = GameObject.Find("FlameThrower");
        fireParticles.SetActive(false);
        actualAttackInterval = Random.Range(attackMinInterval, attackMaxInterval);
        player = GameObject.FindGameObjectWithTag("Player");
        actualPreJumpTime = preJumpAnimationTime = AnimationLength("Mutant Flexing Muscles", animator);
        actualJumpTime = jumpAnimationTime = AnimationLength("Mutant Jump Attack", animator);
        actualRoarTime = roarAnimationTime = AnimationLength("Mutant Roaring", animator);
        actualSwipeTime = swipeAnimationTime = AnimationLength("Mutant Swiping", animator);
    }

    // Update is called once per frame
    void Update()
    {

        playerDistance = Vector3.Distance(transform.position, player.transform.position);




        switch (state)
        {
            case State.IDLE:
                animator.SetBool("isIdle", true);
                // Look to player
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y, player.transform.position.z - transform.position.z)), rotationSpeed);
                if (playerDistance <= attackDistance)
                {
                    if ((actualAttackInterval -= Time.deltaTime) <= 0)
                    {
                        actualAttackInterval = Random.Range(attackMinInterval, attackMaxInterval);
                        randomAttack = Random.Range(0, 3);
                        animator.SetBool("isIdle", false);
                        switch (randomAttack)
                        {
                            case 0:
                                animator.SetTrigger("SwipeAttack");
                                state = State.SWIPEATTACK;
                                break;
                            case 1:
                                animator.SetTrigger("Jump");
                                state = State.PREJUMP;
                                break;
                            case 2:
                                animator.SetTrigger("Roar");
                                state = State.ROAR;
                                break;
                            default:
                                break;
                        }
                    }
                }
                else
                {
                    animator.SetBool("isIdle", false);
                    state = State.WALKING;
                }
                break;
            case State.WALKING:
                animator.SetBool("isMoving", true);
                // Look to player
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y, player.transform.position.z - transform.position.z)), rotationSpeed);
                if (playerDistance > attackDistance)
                {
                    // Follow Player
                    transform.position = Vector3.MoveTowards(transform.position, new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z), movingSpeed);
                    if ((actualAttackInterval -= Time.deltaTime) <= 0)
                    {
                        actualAttackInterval = Random.Range(attackMinInterval, attackMaxInterval);
                        animator.SetBool("isMoving", false);
                        animator.SetTrigger("Jump");
                        state = State.PREJUMP;
                    }
                }
                else
                {
                    animator.SetBool("isMoving", false);
                    state = State.IDLE;
                }
                break;
            case State.SWIPEATTACK:
                if ((actualSwipeTime -= Time.deltaTime) <= 0)
                {
                    actualSwipeTime = swipeAnimationTime;
                    state = State.IDLE;
                }
                break;
            case State.PREJUMP:
                // Look to player
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y, player.transform.position.z - transform.position.z)), rotationSpeed);
                if ((actualPreJumpTime -= Time.deltaTime) <= 0)
                {
                    actualPreJumpTime = preJumpAnimationTime;
                    state = State.JUMP;
                }
                break;
            case State.JUMP:
                if ((actualJumpTime -= Time.deltaTime) <= 0)
                {
                    actualJumpTime = jumpAnimationTime;
                    state = State.IDLE;
                }
                break;
            case State.ROAR:
                actualRoarTime -= Time.deltaTime;
                if ((actualRoarTime / roarAnimationTime) < 0.7f)
                {
                    fireParticles.SetActive(true);
                }
                if ((actualRoarTime / roarAnimationTime) < 0.1f)
                {
                    fireParticles.SetActive(false);
                }
                if (actualRoarTime <= 0)
                {
                    actualRoarTime = roarAnimationTime;
                    state = State.IDLE;
                }
                break;
            case State.DAMAGED:
                break;
            default:
                break;
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
