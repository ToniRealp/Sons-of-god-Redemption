using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SecondBossBehaviour : MonoBehaviour
{

    public Animator animator;
    public float attackDistance = 4f, explosionRange = 10, movingSpeed = 0.02f, rotationSpeed = 0.05f, circlesRange = 5;
    public int circlesNum = 5, actualAttack, actualAttack2, patron;
    public Vector3[] circlePosition;


    private GameObject player;
    private float circleAnimationTime, kameAnimationTime, explosionAnimationTime, initcirclesTime;
    private float actualAttackInterval,  actualCircleTime, actualKameTime, actualExplosionTime;
    private int circlesCounter;
    private bool explosionChecked, patron1switched, patron2switched, patron3switched, godMode;

    enum State { IDLE, WALKING, CIRCLES, KAME, EXPLOSION };
    [SerializeField] State state = State.IDLE;

    public float playerDistance, maxHealth = 2300, health, damage, circleDmg = 1, kameDmg = 25, explosionDmg = 45;
    string lastTag;
    public Text healthText;
    public GameObject healthTextGO, canvas, textPos;
    public Font font;

    public GameObject darkCircle, darkExplosion, darkKame;
    public GameObject blood;
    public Transform bloodPosition;
    public GameObject dieParticles;

    // Use this for initialization
    void Start()
    {
        health = maxHealth;
        actualAttackInterval = 1;
        player = GameObject.FindGameObjectWithTag("Player");
        actualCircleTime = circleAnimationTime = AnimationLength("Circles", animator);
        actualKameTime = kameAnimationTime = AnimationLength("Kame", animator);
        actualExplosionTime = explosionAnimationTime = AnimationLength("Explosion", animator);
        darkKame.SetActive(false);
        darkExplosion.SetActive(false);
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
        actualAttack2 = actualAttack = circlesCounter = 0;
        circlePosition = new Vector3[circlesNum];
        godMode = explosionChecked = false;
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
            case State.IDLE:
                animator.SetBool("isIdle", true);
                // Look to player
                LookPlayer();
                if (playerDistance <= attackDistance)
                {
                    if ((actualAttackInterval -= Time.deltaTime) <= 0)
                    {
                        //animator.SetBool("isIdle", false);
                        //switch (patron)
                        //{
                        //    case 1:
                        //        switch (actualAttack)
                        //        {
                        //            case 0:
                        //                Dash();
                        //                actualAttack++;
                        //                actualAttackInterval = 1;
                        //                break;
                        //            case 1:
                        //                Dash();
                        //                actualAttack++;
                        //                actualAttackInterval = 1;
                        //                break;
                        //            case 2:
                        //                Fire();
                        //                actualAttack++;
                        //                actualAttackInterval = 1;
                        //                break;
                        //            case 3:
                        //                Dash();
                        //                actualAttack++;
                        //                actualAttackInterval = 1;
                        //                break;
                        //            case 4:
                        //                Dash();
                        //                actualAttack++;
                        //                actualAttackInterval = 1;
                        //                break;
                        //            case 5:
                        //                Dark();
                        //                actualAttack = 0;
                        //                actualAttackInterval = 1;
                        //                break;
                        //            default:
                        //                break;
                        //        }
                        //        break;
                        //    case 2:
                        //        switch (actualAttack)
                        //        {
                        //            case 0:
                        //                Dash();
                        //                actualAttack++;
                        //                actualAttackInterval = 1;
                        //                break;
                        //            case 1:
                        //                Dash();
                        //                actualAttack++;
                        //                actualAttackInterval = 1;
                        //                break;
                        //            case 2:
                        //                Fire();
                        //                actualAttack++;
                        //                actualAttackInterval = 1;
                        //                break;
                        //            case 3:
                        //                Dash();
                        //                actualAttack++;
                        //                actualAttackInterval = 1;
                        //                break;
                        //            case 4:
                        //                Dark();
                        //                actualAttack = 0;
                        //                actualAttackInterval = 1;
                        //                break;
                        //            default:
                        //                break;
                        //        }
                        //        break;
                        //    case 3:
                        //        switch (actualAttack)
                        //        {
                        //            case 0:
                        //                Dash();
                        //                actualAttack++;
                        //                actualAttackInterval = 0;
                        //                break;
                        //            case 1:
                        //                Dash();
                        //                actualAttack++;
                        //                actualAttackInterval = 1;
                        //                break;
                        //            case 2:
                        //                Fire();
                        //                actualAttack++;
                        //                actualAttackInterval = 0;
                        //                break;
                        //            case 3:
                        //                Dark();
                        //                actualAttack = 0;
                        //                actualAttackInterval = 1;
                        //                break;
                        //            default:
                        //                break;
                        //        }
                        //        break;
                        //    case 4:
                        //        switch (actualAttack)
                        //        {
                        //            case 0:
                        //                Fire();
                        //                actualAttack++;
                        //                actualAttackInterval = 0;
                        //                break;
                        //            case 1:
                        //                Dark();
                        //                actualAttack++;
                        //                actualAttackInterval = 0;
                        //                break;
                        //            case 2:
                        //                Dash();
                        //                actualAttack++;
                        //                actualAttackInterval = 0;
                        //                break;
                        //            case 3:
                        //                Dash();
                        //                actualAttack++;
                        //                actualAttackInterval = 0;
                        //                break;
                        //            case 4:
                        //                Dark();
                        //                actualAttack = 0;
                        //                actualAttackInterval = 0;
                        //                break;
                        //            default:
                        //                break;
                        //        }
                        //        break;
                        //}
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
                        Kame();
                        actualAttackInterval = 5;
                        //animator.SetBool("isMoving", false);
                        //switch (patron)
                        //{
                        //    case 1:
                        //        switch (actualAttack2)
                        //        {
                        //            case 0:
                        //                Fire();
                        //                actualAttack2++;
                        //                actualAttackInterval = 1;
                        //                break;
                        //            case 1:
                        //                Fire();
                        //                actualAttack2++;
                        //                actualAttackInterval = 1;
                        //                break;
                        //            case 2:
                        //                Dash();
                        //                actualAttack2 = 0;
                        //                actualAttackInterval = 1;
                        //                break;
                        //            default:
                        //                break;
                        //        }
                        //        break;
                        //    case 2:
                        //        switch (actualAttack2)
                        //        {
                        //            case 0:
                        //                Fire();
                        //                actualAttack2++;
                        //                actualAttackInterval = 1;
                        //                break;
                        //            case 1:
                        //                Dark();
                        //                actualAttack2++;
                        //                actualAttackInterval = 1;
                        //                break;
                        //            case 2:
                        //                Dash();
                        //                actualAttack2 = 0;
                        //                actualAttackInterval = 1;
                        //                break;
                        //            default:
                        //                break;
                        //        }
                        //        break;
                        //    case 3:
                        //        switch (actualAttack2)
                        //        {
                        //            case 0:
                        //                Dash();
                        //                actualAttack2++;
                        //                actualAttackInterval = 1;
                        //                break;
                        //            case 1:
                        //                Dark();
                        //                actualAttack2++;
                        //                actualAttackInterval = 0;
                        //                break;
                        //            case 2:
                        //                Fire();
                        //                actualAttack2 = 0;
                        //                actualAttackInterval = 0;
                        //                break;
                        //            default:
                        //                break;
                        //        }
                        //        break;
                        //    case 4:
                        //        switch (actualAttack2)
                        //        {
                        //            case 0:
                        //                Dark();
                        //                actualAttack2++;
                        //                actualAttackInterval = 0;
                        //                break;
                        //            case 1:
                        //                Dash();
                        //                actualAttack2++;
                        //                actualAttackInterval = 0;
                        //                break;
                        //            case 2:
                        //                Dark();
                        //                actualAttack2++;
                        //                actualAttackInterval = 0;
                        //                break;
                        //            case 3:
                        //                Fire();
                        //                actualAttack2 = 0;
                        //                actualAttackInterval = 0;
                        //                break;
                        //            default:
                        //                break;
                        //        }
                        //        break;
                        //}
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
            circlePosition[i] = new Vector3(Random.Range(transform.position.x-circlesRange, transform.position.x + circlesRange),0, Random.Range(transform.position.z - circlesRange, transform.position.z + circlesRange));
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