using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstBossBehaviour : MonoBehaviour
{

    public GameObject fireParticles;
    public Animator animator;
    public float attackDistance = 3.5f, attackMinInterval = 0, attackMaxInterval = 1;
    public int randomAttack, randomAttack2;
    public float movingSpeed = 0.05f, rotationSpeed = 0.05f;


    private GameObject player;
    private float chargeAnimationTime, explosionAnimationTime, roarAnimationTime, swipeAnimationTime, rainAnimationTime;
    private float actualAttackInterval, actualChargeTime, actualExplosionTime, actualRoarTime, actualSwipeTime, actualRainTime;

    enum State { IDLE, WALKING, SWIPEATTACK, CHARGE, EXPLOSION, ROAR, RAIN, DAMAGED };
    [SerializeField] State state = State.IDLE;

    public float playerDistance, health = 1800, damage, swipeDmg = 15, explosionDmg = 55, roarDmg = 25, rainDmg=15;
    string lastTag;
    public Text healthText;
    public GameObject healthTextGO, canvas, textPos, weapon;
    public Font font;

    public GameObject blood;
    public Transform bloodPosition;

    // Use this for initialization
    void Start()
    {
        actualAttackInterval = Random.Range(attackMinInterval, attackMaxInterval);
        player = GameObject.FindGameObjectWithTag("Player");
        actualChargeTime = chargeAnimationTime = AnimationLength("Mutant Flexing Muscles", animator);
        actualExplosionTime = explosionAnimationTime = AnimationLength("Mutant Jumping", animator);
        actualRoarTime = roarAnimationTime = AnimationLength("Mutant Roaring", animator);
        actualSwipeTime = swipeAnimationTime = AnimationLength("Mutant Swiping", animator);
        actualRainTime = rainAnimationTime = AnimationLength("Wide Arm Spell Casting", animator);
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

        randomAttack2 = randomAttack = 0;
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
                        animator.SetBool("isIdle", false);
                        switch (randomAttack)
                        {
                            case 0:
                                animator.SetTrigger("SwipeAttack");
                                damage = swipeDmg;
                                randomAttack++;
                                state = State.SWIPEATTACK;
                                break;
                            case 1:
                                animator.SetTrigger("Rain");
                                damage = rainDmg;
                                randomAttack++;
                                state = State.RAIN;
                                break;
                            case 2:
                                animator.SetTrigger("Roar");
                                damage = roarDmg;
                                randomAttack++;
                                state = State.ROAR;
                                break;
                            case 3:
                                animator.SetTrigger("Explosion");
                                damage = explosionDmg;
                                randomAttack = 0;
                                state = State.CHARGE;
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
                        animator.SetBool("isMoving", false);
                        switch (randomAttack2)
                        {
                            case 0:
                                animator.SetTrigger("Explosion");
                                damage = explosionDmg;
                                randomAttack2++;
                                state = State.CHARGE;
                                break;
                            case 1:
                                animator.SetTrigger("Roar");
                                damage = roarDmg;
                                randomAttack2 = 0;
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
                actualSwipeTime -= Time.deltaTime;
                if (actualSwipeTime <= swipeAnimationTime * 0.8)
                    weapon.tag = "BossWeapon";
                if (actualSwipeTime <= swipeAnimationTime * 0.2)
                    weapon.tag = "Untagged";
                if (actualSwipeTime <= 0)
                {
                    actualSwipeTime = swipeAnimationTime;
                    state = State.IDLE;
                }
                break;
            case State.CHARGE:
                // Look to player
                LookPlayer();
                if ((actualChargeTime -= Time.deltaTime) <= 0)
                {
                    actualChargeTime = chargeAnimationTime;
                    state = State.EXPLOSION;
                }
                break;
            case State.EXPLOSION:
                actualExplosionTime -= Time.deltaTime;
                if (actualExplosionTime <= explosionAnimationTime * 0.8)
                    tag = "BossWeapon";
                if (actualExplosionTime <= explosionAnimationTime * 0.5)
                    tag = "Enemy";
                if (actualExplosionTime <= 0)
                {
                    actualExplosionTime = explosionAnimationTime;
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
            case State.RAIN:
                if ((actualRainTime -= Time.deltaTime) <= 0)
                {
                    actualRainTime = rainAnimationTime;
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
            Instantiate(blood, bloodPosition.position, bloodPosition.rotation, transform);
            health -= (int)other.GetComponentInParent<PlayerController>().damage;
        }
    }


}

