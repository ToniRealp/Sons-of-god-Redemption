using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstBossBehaviour : MonoBehaviour {

    public Animator animator;
    public float attackDistance = 3.5f, attackMinInterval=2, attackMaxInterval=4;
    public int randomAttack;
    public float movingSpeed = 0.05f, rotationSpeed = 0.05f;


    private GameObject player;
    private float actualAttackInterval;

    enum State { IDLE, WALKING, LIGHTATTACK, JUMP, ROAR, DAMAGED };
    [SerializeField] State state = State.IDLE;

    public float playerDistance;

    

    // Use this for initialization
    void Start () {
        actualAttackInterval = Random.Range(attackMinInterval, attackMaxInterval);
        player = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {

        playerDistance = Vector3.Distance(transform.position, player.transform.position);




        switch (state)
        {
            case State.IDLE:
                animator.SetBool("isIdle", true);
                // Look to player
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y, player.transform.position.z - transform.position.z)), rotationSpeed);
                if (playerDistance<=attackDistance)
                {
                    if ((actualAttackInterval -= Time.deltaTime) <= 0)
                    {
                        actualAttackInterval = Random.Range(attackMinInterval, attackMaxInterval);
                        randomAttack = Random.Range(0,3);
                        Debug.Log(randomAttack);
                        animator.SetBool("isIdle", false);
                        switch (randomAttack)
                        {
                            case 0:
                                state = State.LIGHTATTACK;
                                break;
                            case 1:
                                state = State.JUMP;
                                break;
                            case 2:
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
                        state = State.JUMP;
                    }
                }
                else
                {
                    animator.SetBool("isMoving", false);
                    state = State.IDLE;
                }
                break;
            case State.LIGHTATTACK:
                animator.SetBool("LightAttack", true);
                break;
            case State.JUMP:
                animator.SetBool("Jump", true);
                break;
            case State.ROAR:
                animator.SetBool("Roar", true);
                break;
            case State.DAMAGED:
                break;
            default:
                break;
        }





    }






}
