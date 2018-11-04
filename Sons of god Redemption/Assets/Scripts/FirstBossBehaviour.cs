using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstBossBehaviour : MonoBehaviour
{

    public GameObject fireParticles;
    public Animator animator;
    public float attackDistance = 3.5f, attackMinInterval = 2, attackMaxInterval = 4;
    public int randomAttack;
    public float movingSpeed = 0.05f, rotationSpeed = 0.05f;


    private GameObject player;
    private float preJumpAnimationTime, jumpAnimationTime, roarAnimationTime, swipeAnimationTime;
    private float actualAttackInterval, actualPreJumpTime, actualJumpTime, actualRoarTime, actualSwipeTime;

    enum State { IDLE, WALKING, SWIPEATTACK, PREJUMP, JUMP, ROAR, DAMAGED };
    [SerializeField] State state = State.IDLE;

    public float playerDistance, health = 500, damage, swipeDmg=15, jumpDmg=25, roarDmg=2;
    string lastTag;
    public Text healthText;
    public GameObject healthTextGO, canvas, textPos, weapon;
    public Font font;


    // Use this for initialization
    void Start()
    {
        actualAttackInterval = Random.Range(attackMinInterval, attackMaxInterval);
        player = GameObject.FindGameObjectWithTag("Player");
        actualPreJumpTime = preJumpAnimationTime = AnimationLength("Mutant Flexing Muscles", animator);
        actualJumpTime = jumpAnimationTime = AnimationLength("Mutant Jump Attack", animator);
        actualRoarTime = roarAnimationTime = AnimationLength("Mutant Roaring", animator);
        actualSwipeTime = swipeAnimationTime = AnimationLength("Mutant Swiping", animator);
        fireParticles.SetActive(false);
        lastTag = "value";

        //Health Text
        canvas = GameObject.Find("Canvas");
        healthTextGO = new GameObject();
        healthTextGO.transform.SetParent(canvas.transform);
        healthText = healthTextGO.AddComponent<Text>();
        healthText.font = font;
        healthTextGO.name = "Enemy Health";
        healthText.alignment = TextAnchor.MiddleCenter;
        textPos = this.gameObject.transform.GetChild(2).gameObject;

    }

    // Update is called once per frame
    void Update()
    {

        playerDistance = Vector3.Distance(transform.position, player.transform.position);

        if (health <= 0)
        {
            Destroy(this.gameObject);
            Destroy(healthTextGO);
        }

        // Health text update
        healthTextGO.GetComponent<Transform>().position = Camera.main.WorldToScreenPoint(textPos.transform.position);
        healthText.text = health.ToString();


        switch (state)
        {
            case State.IDLE:
                animator.SetBool("isIdle", true);
                // Look to player
                LookPlayer();
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
                                weapon.tag = "BossWeapon";
                                damage = swipeDmg;
                                state = State.SWIPEATTACK;
                                break;
                            case 1:
                                animator.SetTrigger("Jump");
                                damage = jumpDmg;
                                state = State.PREJUMP;
                                break;
                            case 2:
                                animator.SetTrigger("Roar");
                                damage = roarDmg;
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
                LookPlayer();
                if (playerDistance > attackDistance)
                {
                    // Follow Player
                    transform.position = Vector3.MoveTowards(transform.position, new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z), movingSpeed);
                    if ((actualAttackInterval -= Time.deltaTime) <= 0)
                    {
                        actualAttackInterval = Random.Range(attackMinInterval, attackMaxInterval);
                        randomAttack = Random.Range(0, 2);
                        animator.SetBool("isMoving", false);
                        switch (randomAttack)
                        {
                            case 0:
                                animator.SetTrigger("Jump");
                                damage = jumpDmg;
                                state = State.PREJUMP;
                                break;
                            case 1:
                                animator.SetTrigger("Roar");
                                damage = roarDmg;
                                state = State.ROAR;
                                break;
                            default:
                                break;
                        }
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
                    weapon.tag = "Untagged";
                    state = State.IDLE;
                }
                break;
            case State.PREJUMP:
                // Look to player
                LookPlayer();
                if ((actualPreJumpTime -= Time.deltaTime) <= 0)
                {
                    actualPreJumpTime = preJumpAnimationTime;
                    state = State.JUMP;
                }
                break;
            case State.JUMP:
                tag = "BossWeapon";
                if ((actualJumpTime -= Time.deltaTime) <= 0)
                {
                    tag = "Enemy";
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

    void LookPlayer()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(player.transform.position.x - transform.position.x, transform.position.y, player.transform.position.z - transform.position.z)), rotationSpeed);
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != lastTag && other.tag != "Untagged")
        {
            lastTag = other.tag;
            health -= (int)other.GetComponentInParent<PlayerController>().damage;
        }
    }


}
