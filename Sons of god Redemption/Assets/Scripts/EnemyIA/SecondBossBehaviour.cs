using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SecondBossBehaviour : MonoBehaviour
{

    public Animator animator;
    public float attackDistance = 4f, explosionRange = 5, movingSpeed = 0.02f, rotationSpeed = 0.05f, circlesRange = 5;
    public int circlesNum = 4, actualAttack, actualAttack2, patron;
    public Vector3[] circlePosition;


    private GameObject player;
    private float cinematicAnimationTime, circleAnimationTime, kameAnimationTime, explosionAnimationTime, initcirclesTime, deadAnimationTime;
    private float actualCinematicTime, actualAttackInterval,  actualCircleTime, actualKameTime, actualExplosionTime, actualDeadTime;
    private int circlesCounter;
    private bool explosionChecked, patron1switched, patron2switched, patron3switched, dead;

    enum State { CINEMATIC, IDLE, WALKING, CIRCLES, KAME, EXPLOSION, DEATH };
    [SerializeField] State state = State.CINEMATIC;

    public float playerDistance, maxHealth = 2300, health, damage, circleDmg = 1, kameDmg = 25, explosionDmg = 45;
    string lastTag;
    public Text healthText;
    public GameObject healthTextGO, canvas, textPos;
    public Font font;

    public GameObject darkCircle, darkExplosion, darkKame;
    public GameObject title, blood;
    public Transform bloodPosition;
    public GameObject dieParticles;

    // Use this for initialization
    void Awake()
    {
        health = maxHealth;
        actualAttackInterval = 1;
        player = GameObject.FindGameObjectWithTag("Player");
        actualCinematicTime = cinematicAnimationTime = AnimationLength("Standing", animator);
        actualCircleTime = circleAnimationTime = AnimationLength("Circles", animator);
        actualKameTime = kameAnimationTime = AnimationLength("Kame", animator);
        actualExplosionTime = explosionAnimationTime = AnimationLength("Explosion", animator);
        actualDeadTime = deadAnimationTime = AnimationLength("Death", animator);
        darkKame.SetActive(false);
        darkExplosion.SetActive(false);
        lastTag = "value";

        //Health Text
        canvas = GameObject.Find("Canvas");
        Instantiate(title, canvas.transform);
        healthTextGO = new GameObject();
        healthTextGO.transform.SetParent(canvas.transform);
        healthText = healthTextGO.AddComponent<Text>();
        healthText.font = font;
        healthTextGO.name = "Enemy Health";
        healthText.alignment = TextAnchor.MiddleCenter;
        textPos = this.gameObject.transform.Find("HealthTextPos").gameObject;

        patron = 1;
        patron1switched = patron2switched = patron3switched = false;
        actualAttack2 = actualAttack = circlesCounter = 0;
        circlePosition = new Vector3[circlesNum];
        dead = explosionChecked = false;

        //Cinematic
        player.GetComponent<PlayerController>().onCinematic = true;
        
    }

    // Update is called once per frame
    void Update()
    {

        playerDistance = Vector3.Distance(transform.position, player.transform.position);

        if (health <= 0 && !dead)
        {
            GameObject.Find("Level2Controller").GetComponent<Level2Controller>().BossDead();
            Destroy(healthTextGO);
            animator.SetTrigger("Dead");
            state = State.DEATH;
            actualCircleTime = circleAnimationTime;
            actualKameTime = kameAnimationTime;
            actualExplosionTime = explosionAnimationTime;
            darkKame.SetActive(false);
            darkExplosion.SetActive(false);
            dead = true;
        }
        if (!dead)
        {
            // Health text update
            healthTextGO.GetComponent<Transform>().position = Camera.main.WorldToScreenPoint(textPos.transform.position);
            healthText.text = health.ToString();
        }


        if (health < maxHealth * 0.75 && !patron1switched)
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
            case State.CINEMATIC:
                if ((actualCinematicTime -= Time.deltaTime) <= 0)
                {
                    GetComponent<BossCinematic>().ResetCamera();
                    player.GetComponent<PlayerController>().onCinematic = false;
                    actualCinematicTime = cinematicAnimationTime;
                    state = State.IDLE;
                }
                break;
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
                                        Explosion();
                                        actualAttack++;
                                        actualAttackInterval = 2.5f;
                                        break;
                                    case 1:
                                        Circles();
                                        actualAttack++;
                                        actualAttackInterval = 2.5f;
                                        break;
                                    case 2:
                                        Kame();
                                        actualAttack++;
                                        actualAttackInterval = 2.5f;
                                        break;
                                    case 3:
                                        Kame();
                                        actualAttack++;
                                        actualAttackInterval = 2.5f;
                                        break;
                                    case 4:
                                        Circles();
                                        actualAttack=0;
                                        actualAttackInterval = 2.5f;
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case 2:
                                switch (actualAttack)
                                {
                                    case 0:
                                        Kame();
                                        actualAttack++;
                                        actualAttackInterval = 2.5f;
                                        break;
                                    case 1:
                                        Circles();
                                        actualAttack++;
                                        actualAttackInterval = 2.5f;
                                        break;
                                    case 2:
                                        Explosion();
                                        actualAttack++;
                                        actualAttackInterval = 2.5f;
                                        break;
                                    case 3:
                                        Kame();
                                        actualAttack=0;
                                        actualAttackInterval = 2.5f;
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case 3:
                                switch (actualAttack)
                                {
                                    case 0:
                                        Kame();
                                        actualAttack++;
                                        actualAttackInterval = 2;
                                        break;
                                    case 1:
                                        Kame();
                                        actualAttack++;
                                        actualAttackInterval = 0;
                                        break;
                                    case 2:
                                        Circles();
                                        actualAttack++;
                                        actualAttackInterval = 2;
                                        break;
                                    case 3:
                                        Explosion();
                                        actualAttack++;
                                        actualAttackInterval = 0;
                                        break;
                                    case 4:
                                        Kame();
                                        actualAttack++;
                                        actualAttackInterval = 2;
                                        break;
                                    case 5:
                                        Circles();
                                        actualAttack=0;
                                        actualAttackInterval = 2;
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case 4:
                                switch (actualAttack)
                                {
                                    case 0:
                                        Explosion();
                                        actualAttack++;
                                        actualAttackInterval = 1;
                                        break;
                                    case 1:
                                        Kame();
                                        actualAttack++;
                                        actualAttackInterval = 1;
                                        break;
                                    case 2:
                                        Circles();
                                        actualAttack++;
                                        actualAttackInterval = 2;
                                        break;
                                    case 3:
                                        Kame();
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
                                        Kame();
                                        actualAttack2++;
                                        actualAttackInterval = 2.5f;
                                        break;
                                    case 1:
                                        Explosion();
                                        actualAttack2++;
                                        actualAttackInterval = 2.5f;
                                        break;
                                    case 2:
                                        Circles();
                                        actualAttack2 = 0;
                                        actualAttackInterval = 2.5f;
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case 2:
                                switch (actualAttack2)
                                {
                                    case 0:
                                        Kame();
                                        actualAttack2++;
                                        actualAttackInterval = 2.5f;
                                        break;
                                    case 1:
                                        Kame();
                                        actualAttack2++;
                                        actualAttackInterval = 2.5f;
                                        break;
                                    case 2:
                                        Explosion();
                                        actualAttack2 = 0;
                                        actualAttackInterval = 2.5f;
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
                                        actualAttackInterval = 2;
                                        break;
                                    case 1:
                                        Kame();
                                        actualAttack2++;
                                        actualAttackInterval = 0;
                                        break;
                                    case 2:
                                        Circles();
                                        actualAttack2 = 0;
                                        actualAttackInterval = 2;
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case 4:
                                switch (actualAttack2)
                                {
                                    case 0:
                                        Explosion();
                                        actualAttack2++;
                                        actualAttackInterval = 2;
                                        break;
                                    case 1:
                                        Kame();
                                        actualAttack2++;
                                        actualAttackInterval = 1;
                                        break;
                                    case 2:
                                        Circles();
                                        actualAttack2++;
                                        actualAttackInterval = 2;
                                        break;
                                    case 3:
                                        Kame();
                                        actualAttack2 = 0;
                                        actualAttackInterval = 1;
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
            case State.CIRCLES:

                actualCircleTime -= Time.deltaTime;
                if (actualCircleTime <= circleAnimationTime *0.8)
                {
                    if (circlesCounter < circlesNum)
                    {
                        Instantiate(darkCircle, circlePosition[circlesCounter], new Quaternion(0, 0, 0, 0));
                        circlesCounter++;
                    }
                }
                if (actualCircleTime <=0 && circlesCounter>=circlesNum)
                {
                    actualCircleTime = circleAnimationTime;
                    state = State.IDLE;
                }


                break;

        
            case State.KAME:

                actualKameTime -= Time.deltaTime;
                if (actualKameTime > kameAnimationTime * 0.75)
                    LookPlayer();
                if (actualKameTime <= kameAnimationTime * 0.65f)
                    darkKame.SetActive(true);
                if (actualKameTime <= 0)
                {
                    darkKame.SetActive(false);
                    actualKameTime = kameAnimationTime;
                    state = State.IDLE;
                }


                break;



            case State.EXPLOSION:
                actualExplosionTime -= Time.deltaTime;

                if (actualExplosionTime <= explosionAnimationTime * 0.35)
                    darkExplosion.SetActive(true);
                if (actualExplosionTime <= 0)
                {
                    darkExplosion.SetActive(false);
                    actualExplosionTime = explosionAnimationTime;
                    state = State.IDLE;
                }


                break;
            case State.DEATH:
                movingSpeed = 0;
                actualDeadTime -= Time.deltaTime;
                if (actualDeadTime <= 0)
                {
                    Instantiate(dieParticles, bloodPosition.position, bloodPosition.rotation);
                    Destroy(this.gameObject);
                }
                break;
            default:
                break;
        }



    }

    void LookPlayer()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(player.transform.position.x - transform.position.x, transform.position.y, player.transform.position.z - transform.position.z)), rotationSpeed);
    }

    void Kame()
    {
        animator.SetTrigger("Kame");
        damage = kameDmg;
        state = State.KAME;
    }

    void Circles()
    {
        animator.SetTrigger("Circles");
        damage = circleDmg;
        initcirclesTime = Time.time;
        circlesCounter = 0;
        for (int i = 0; i < circlesNum; i++)
        {
            circlePosition[i] = new Vector3(Random.Range(transform.position.x-circlesRange, transform.position.x + circlesRange),this.gameObject.transform.position.y, Random.Range(transform.position.z - circlesRange, transform.position.z + circlesRange));
        }
        state = State.CIRCLES;
    }

    void Explosion()
    {
        animator.SetTrigger("Explosion");
        damage = explosionDmg;
        state = State.EXPLOSION;
    }


    public void Heal()
    {
        if (health <= maxHealth - 15)
        {
            health += 15;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Untagged")
        {
            Instantiate(blood, bloodPosition.position, bloodPosition.rotation, transform);
            health -= (int)other.GetComponentInParent<PlayerController>().damage;
        }
    }
}