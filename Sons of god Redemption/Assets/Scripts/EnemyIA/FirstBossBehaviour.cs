using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstBossBehaviour : MonoBehaviour
{

    public GameObject fireParticles, explosionParticles, meteor;
    public Animator animator;
    public float attackDistance = 5f, explosionRange = 10, movingSpeed = 0.05f, rotationSpeed = 0.05f, meteorInterval = 0.3f, meteorRange = 5;
    public int meteorNum = 5, actualAttack, actualAttack2, patron;
    public Vector3[] meteorPosition;


    private GameObject player;
    private float chargeAnimationTime, explosionAnimationTime, roarAnimationTime, swipeAnimationTime, rainAnimationTime, initMeteorTime;
    private float actualAttackInterval, actualChargeTime, actualExplosionTime, actualRoarTime, actualSwipeTime, actualRainTime;
    private int meteorCounter;
    private bool explosionChecked, patron1switched, patron2switched, patron3switched;

    enum State { IDLE, WALKING, SWIPEATTACK, CHARGE, EXPLOSION, ROAR, RAIN, DAMAGED };
    [SerializeField] State state = State.IDLE;

    public float playerDistance, maxHealth = 1800, health, damage, swipeDmg = 15, explosionDmg = 55, roarDmg = 25, rainDmg=15;
    string lastTag;
    public Text healthText;
    public GameObject healthTextGO, canvas, textPos, weapon;
    public Font font;

    public GameObject blood;
    public Transform bloodPosition;
    public GameObject dieParticles;

    // Use this for initialization
    void Start()
    {
        health = maxHealth;
        actualAttackInterval = 1;
        player = GameObject.FindGameObjectWithTag("Player");
        actualChargeTime = chargeAnimationTime = AnimationLength("Mutant Flexing Muscles", animator);
        actualExplosionTime = explosionAnimationTime = AnimationLength("Mutant Jumping", animator);
        actualRoarTime = roarAnimationTime = AnimationLength("Mutant Roaring", animator);
        actualSwipeTime = swipeAnimationTime = AnimationLength("Stable Sword Outward Slash", animator);
        actualRainTime = rainAnimationTime = AnimationLength("Wide Arm Spell Casting", animator);
        fireParticles.SetActive(false);
        explosionParticles.SetActive(false);
        lastTag = "value";

        //Health Text
        canvas = GameObject.Find("Canvas");
        healthTextGO = new GameObject();
        healthTextGO.transform.SetParent(canvas.transform);
        healthText = healthTextGO.AddComponent<Text>();
        healthText.font = font;
        healthTextGO.name = "Enemy Health";
        healthText.alignment = TextAnchor.MiddleCenter;
        textPos = this.gameObject.transform.Find("HealthTextPos").gameObject;

        patron = 1;
        patron1switched = patron2switched = patron3switched = false;
        actualAttack2 = actualAttack = meteorCounter = 0;
        meteorPosition = new Vector3[meteorNum];
        explosionChecked = false;
    }

    // Update is called once per frame
    void Update()
    {

        playerDistance = Vector3.Distance(transform.position, player.transform.position);

        if (health <= 0)
        {
            GameObject.Find("Level1Controller").GetComponent<Level1Controller>().BossDead();
            Instantiate(dieParticles, bloodPosition.position, bloodPosition.rotation);
            Destroy(this.gameObject);
            Destroy(healthTextGO);
        }

        // Health text update
        healthTextGO.GetComponent<Transform>().position = Camera.main.WorldToScreenPoint(textPos.transform.position);
        healthText.text = health.ToString();

        if (health<maxHealth * 0.75 && !patron1switched)
        {
            patron = 2;
            actualAttack = 0;
            actualAttack2 = 0;
            patron1switched = true;
        }
        if (health < maxHealth * 0.5 && !patron2switched)
        {
            patron = 3;
            actualAttack = 0;
            actualAttack2 = 0;
            patron2switched = true;
        }
        if (health < maxHealth * 0.25 && !patron3switched)
        {
            patron = 4;
            actualAttack = 0;
            actualAttack2 = 0;
            patron3switched = true;
        }

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
                        animator.SetBool("isIdle", false);
                        switch (patron)
                        {
                            case 1:
                                switch (actualAttack)
                                {
                                    case 0:
                                        Swipe();
                                        actualAttack++;
                                        actualAttackInterval = 1;
                                        break;
                                    case 1:
                                        Swipe();
                                        actualAttack++;
                                        actualAttackInterval = 1;
                                        break;
                                    case 2:
                                        Rain();
                                        actualAttack++;
                                        actualAttackInterval = 1;
                                        break;
                                    case 3:
                                        Swipe();
                                        actualAttack++;
                                        actualAttackInterval = 1;
                                        break;
                                    case 4:
                                        Swipe();
                                        actualAttack++;
                                        actualAttackInterval = 1;
                                        break;
                                    case 5:
                                        Roar();
                                        actualAttack++;
                                        actualAttackInterval = 1;
                                        break;
                                    case 6:
                                        Explosion();
                                        actualAttack = 0;
                                        actualAttackInterval = 1;
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case 2:
                                switch (actualAttack)
                                {
                                    case 0:
                                        Swipe();
                                        actualAttack++;
                                        actualAttackInterval = 0;
                                        break;
                                    case 1:
                                        Swipe();
                                        actualAttack++;
                                        actualAttackInterval = 1;
                                        break;
                                    case 2:
                                        Explosion();
                                        actualAttack++;
                                        actualAttackInterval = 1;
                                        break;
                                    case 3:
                                        Swipe();
                                        actualAttack++;
                                        actualAttackInterval = 0;
                                        break;
                                    case 4:
                                        Swipe();
                                        actualAttack++;
                                        actualAttackInterval = 1;
                                        break;
                                    case 5:
                                        Roar();
                                        actualAttack++;
                                        actualAttackInterval = 1;
                                        break;
                                    case 6:
                                        Swipe();
                                        actualAttack++;
                                        actualAttackInterval = 1;
                                        break;
                                    case 7:
                                        Rain();
                                        actualAttack=0;
                                        actualAttackInterval = 1;
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case 3:
                                switch (actualAttack)
                                {
                                    case 0:
                                        Explosion();
                                        actualAttack++;
                                        actualAttackInterval = 1;
                                        break;
                                    case 1:
                                        Roar();
                                        actualAttack++;
                                        actualAttackInterval = 0;
                                        break;
                                    case 2:
                                        Swipe();
                                        actualAttack++;
                                        actualAttackInterval = 0;
                                        break;
                                    case 3:
                                        Swipe();
                                        actualAttack++;
                                        actualAttackInterval = 1;
                                        break;
                                    case 4:
                                        Rain();
                                        actualAttack++;
                                        actualAttackInterval = 0;
                                        break;
                                    case 5:
                                        Roar();
                                        actualAttack=0;
                                        actualAttackInterval = 1;
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case 4:
                                switch (actualAttack)
                                {
                                    case 0:
                                        Roar();
                                        actualAttack++;
                                        actualAttackInterval = 0;
                                        break;
                                    case 1:
                                        Explosion();
                                        actualAttack++;
                                        actualAttackInterval = 0;
                                        break;
                                    case 2:
                                        Rain();
                                        actualAttack++;
                                        actualAttackInterval = 0;
                                        break;
                                    case 3:
                                        Swipe();
                                        actualAttack++;
                                        actualAttackInterval = 0;
                                        break;
                                    case 4:
                                        Swipe();
                                        actualAttack++;
                                        actualAttackInterval = 0;
                                        break;
                                    case 5:
                                        Swipe();
                                        actualAttack++;
                                        actualAttackInterval = 0;
                                        break;
                                    case 6:
                                        Roar();
                                        actualAttack=0;
                                        actualAttackInterval = 0;
                                        break;
                                    default:
                                        break;
                                }
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
                        animator.SetBool("isMoving", false);
                        switch (patron)
                        {
                            case 1:
                                switch (actualAttack2)
                                {
                                    case 0:
                                        Rain();
                                        actualAttack2++;
                                        actualAttackInterval = 1;
                                        break;
                                    case 1:
                                        Roar();
                                        actualAttack2++;
                                        actualAttackInterval = 1;
                                        break;
                                    case 2:
                                        Explosion();
                                        actualAttack2++;
                                        actualAttackInterval = 1;
                                        break;
                                    case 3:
                                        Roar();
                                        actualAttack2=0;
                                        actualAttackInterval = 1;
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case 2:
                                switch (actualAttack2)
                                {
                                    case 0:
                                        Rain();
                                        actualAttack2++;
                                        actualAttackInterval = 0;
                                        break;
                                    case 1:
                                        Explosion();
                                        actualAttack2++;
                                        actualAttackInterval = 1;
                                        break;
                                    case 2:
                                        Roar();
                                        actualAttack2++;
                                        actualAttackInterval = 1;
                                        break;
                                    case 3:
                                        Roar();
                                        actualAttack2=0;
                                        actualAttackInterval = 1;
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case 3:
                                switch (actualAttack2)
                                {
                                    case 0:
                                        Explosion();
                                        actualAttack2++;
                                        actualAttackInterval = 0;
                                        break;
                                    case 1:
                                        Roar();
                                        actualAttack2++;
                                        actualAttackInterval = 1;
                                        break;
                                    case 2:
                                        Rain();
                                        actualAttack2++;
                                        actualAttackInterval = 0;
                                        break;
                                    case 3:
                                        Roar();
                                        actualAttack2=0;
                                        actualAttackInterval = 1;
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case 4:
                                switch (actualAttack2)
                                {
                                    case 0:
                                        Rain();
                                        actualAttack2++;
                                        actualAttackInterval = 0;
                                        break;
                                    case 1:
                                        Explosion();
                                        actualAttack2++;
                                        actualAttackInterval = 0;
                                        break;
                                    case 2:
                                        Rain();
                                        actualAttack2++;
                                        actualAttackInterval = 0;
                                        break;
                                    case 3:
                                        Roar();
                                        actualAttack2++;
                                        actualAttackInterval = 0;
                                        break;
                                    case 4:
                                        Explosion();
                                        actualAttack2++;
                                        actualAttackInterval = 0;
                                        break;
                                    case 5:
                                        Roar();
                                        actualAttack2=0;
                                        actualAttackInterval = 0;
                                        break;
                                    default:
                                        break;
                                }
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
                if (actualSwipeTime > swipeAnimationTime * 0.6)
                    LookPlayer();
                if (actualSwipeTime <= swipeAnimationTime * 0.95)
                    weapon.tag = "FirstBossWeapon";
                if (actualSwipeTime <= swipeAnimationTime * 0.1)
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
                if (actualExplosionTime <= explosionAnimationTime * 0.5)
                {
                    explosionParticles.SetActive(true);
                    if (!explosionChecked)
                    {
                        if (playerDistance < explosionRange)
                        {
                            player.GetComponent<PlayerController>().bossDmg = explosionDmg;
                            player.GetComponent<PlayerController>().explosionHit = true;
                        }
                        explosionChecked = true;
                    }

                }

                    
                if (actualExplosionTime <= 0)
                {
                    explosionParticles.SetActive(false);
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
                if (Time.time - initMeteorTime >= meteorInterval && meteorCounter < meteorNum)
                {
                    Instantiate(meteor, meteorPosition[meteorCounter], new Quaternion(0, 0, 0, 0));
                    meteorCounter++;
                    initMeteorTime = Time.time;
                }
                if ((actualRainTime -= Time.deltaTime) <= 0 && meteorCounter>=meteorNum)
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

    void Swipe()
    {
        animator.SetTrigger("SwipeAttack");
        damage = swipeDmg;
        state = State.SWIPEATTACK;
    }

    void Rain()
    {
        animator.SetTrigger("Rain");
        damage = rainDmg;
        initMeteorTime = Time.time;
        meteorCounter = 0;
        for (int i = 0; i < meteorNum; i++)
        {
            meteorPosition[i] = new Vector3(Random.Range(player.transform.position.x - meteorRange, player.transform.position.x + meteorRange), 20, Random.Range(player.transform.position.z - meteorRange, player.transform.position.z + meteorRange));
        }
        state = State.RAIN;
    }

    void Roar()
    {
        animator.SetTrigger("Roar");
        damage = roarDmg;
        state = State.ROAR;
    }

    void Explosion()
    {
        animator.SetTrigger("Explosion");
        damage = explosionDmg;
        explosionChecked = false;
        state = State.CHARGE;
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
        if (other.tag != "Untagged")
        {
            Instantiate(blood, bloodPosition.position, bloodPosition.rotation, transform);
            health -= (int)other.GetComponentInParent<PlayerController>().damage;
        }
    }
}

