using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinalBossBehaviour : MonoBehaviour {

    //public GameObject fireParticles, explosionParticles, meteor;
    public Animator animator;
    public float attackDistance = 4f, explosionRange = 10, movingSpeed = 0.02f, rotationSpeed = 0.05f, meteorInterval = 0.3f, meteorRange = 5, godModeMinTime = 3, godModeMaxTime = 8, godModeMinDelay = 20, godModeMaxDelay = 35;
    public int meteorNum = 5, actualAttack, actualAttack2, patron;
    public Quaternion[] meteorRotation;


    private GameObject player;
    private float godModeTime, godDelayTime, darkAnimationTime, dizzyAnimationTime, dashAnimationTime, fireAnimationTime, initMeteorTime, godModeHealth;
    private float actualAttackInterval, actualGodModeTime, actualGodDelayTime, actualDarkTime, actualDizzyTime, actualDashTime, actualFireTime;
    private int meteorCounter;
    private bool explosionChecked, patron1switched, patron2switched, patron3switched, godMode;

    enum State { IDLE, WALKING, DASH, FIRE, DARK, DIZZY };
    [SerializeField] State state = State.IDLE;

    public float playerDistance, maxHealth = 1800, health, damage, dashDmg = 15, fireDmg = 55, darkDmg = 25;
    string lastTag;
    public Text healthText;
    public GameObject healthTextGO, canvas, textPos;
    public Font font;

    public GameObject fireBall, darkBox, dashTrigger, glow, blood;
    public Transform bloodPosition;
    public GameObject dieParticles;

    // Use this for initialization
    void Start()
    {
        health = maxHealth;
        actualAttackInterval = 1;
        player = GameObject.FindGameObjectWithTag("Player");
        actualDizzyTime = dizzyAnimationTime = AnimationLength("Dizzy", animator);
        actualDarkTime = darkAnimationTime = AnimationLength("Standing2", animator);
        actualDashTime = dashAnimationTime = AnimationLength("Running Slide (1)", animator);
        actualFireTime = fireAnimationTime = AnimationLength("Standing", animator);
        godDelayTime = actualGodDelayTime = Random.Range(godModeMinDelay, godModeMaxDelay);
        darkBox.SetActive(false);
        glow.SetActive(false);
        dashTrigger.SetActive(false);
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
        meteorRotation = new Quaternion[meteorNum];
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

        //GodMode
        if (godMode)
        {
            health = godModeHealth;
            actualGodModeTime -= Time.deltaTime;
            if (actualGodModeTime <= 0)
            {
                glow.SetActive(false);
                godDelayTime = actualGodDelayTime = Random.Range(godModeMinDelay, godModeMaxDelay);
                godMode = false;
            }
        }
        else
        {
            actualGodDelayTime -= Time.deltaTime;
            if (actualGodDelayTime<=0)
            {
                glow.SetActive(true);
                godModeTime = actualGodModeTime = Random.Range(godModeMinTime,godModeMaxTime);
                godMode = true;
                godModeHealth = health;
            }
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
                        animator.SetBool("isIdle", false);
                        switch (patron)
                        {
                            case 1:
                                switch (actualAttack)
                                {
                                    case 0:
                                        Dash();
                                        actualAttack++;
                                        actualAttackInterval = 1;
                                        break;
                                    case 1:
                                        Dash();
                                        actualAttack++;
                                        actualAttackInterval = 1;
                                        break;
                                    case 2:
                                        Fire();
                                        actualAttack++;
                                        actualAttackInterval = 1;
                                        break;
                                    case 3:
                                        Dash();
                                        actualAttack++;
                                        actualAttackInterval = 1;
                                        break;
                                    case 4:
                                        Dash();
                                        actualAttack++;
                                        actualAttackInterval = 1;
                                        break;
                                    case 5:
                                        Dark();
                                        actualAttack=0;
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
                                        Dash();
                                        actualAttack++;
                                        actualAttackInterval = 1;
                                        break;
                                    case 1:
                                        Dash();
                                        actualAttack++;
                                        actualAttackInterval = 1;
                                        break;
                                    case 2:
                                        Fire();
                                        actualAttack++;
                                        actualAttackInterval = 1;
                                        break;
                                    case 3:
                                        Dash();
                                        actualAttack++;
                                        actualAttackInterval = 1;
                                        break;
                                    case 4:
                                        Dark();
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
                                        Dash();
                                        actualAttack++;
                                        actualAttackInterval = 0;
                                        break;
                                    case 1:
                                        Dash();
                                        actualAttack++;
                                        actualAttackInterval = 1;
                                        break;
                                    case 2:
                                        Fire();
                                        actualAttack++;
                                        actualAttackInterval = 0;
                                        break;
                                    case 3:
                                        Dark();
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
                                        Fire();
                                        actualAttack++;
                                        actualAttackInterval = 0;
                                        break;
                                    case 1:
                                        Dark();
                                        actualAttack++;
                                        actualAttackInterval = 0;
                                        break;
                                    case 2:
                                        Dash();
                                        actualAttack++;
                                        actualAttackInterval = 0;
                                        break;
                                    case 3:
                                        Dash();
                                        actualAttack++;
                                        actualAttackInterval = 0;
                                        break;
                                    case 4:
                                        Dark();
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
                                        Fire();
                                        actualAttack2++;
                                        actualAttackInterval = 1;
                                        break;
                                    case 1:
                                        Fire();
                                        actualAttack2++;
                                        actualAttackInterval = 1;
                                        break;
                                    case 2:
                                        Dash();
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
                                        Fire();
                                        actualAttack2++;
                                        actualAttackInterval = 1;
                                        break;
                                    case 1:
                                        Dark();
                                        actualAttack2++;
                                        actualAttackInterval = 1;
                                        break;
                                    case 2:
                                        Dash();
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
                                        Dash();
                                        actualAttack2++;
                                        actualAttackInterval = 1;
                                        break;
                                    case 1:
                                        Dark();
                                        actualAttack2++;
                                        actualAttackInterval = 0;
                                        break;
                                    case 2:
                                        Fire();
                                        actualAttack2=0;
                                        actualAttackInterval = 0;
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case 4:
                                switch (actualAttack2)
                                {
                                    case 0:
                                        Dark();
                                        actualAttack2++;
                                        actualAttackInterval = 0;
                                        break;
                                    case 1:
                                        Dash();
                                        actualAttack2++;
                                        actualAttackInterval = 0;
                                        break;
                                    case 2:
                                        Dark();
                                        actualAttack2++;
                                        actualAttackInterval = 0;
                                        break;
                                    case 3:
                                        Fire();
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
            case State.DASH:

                actualDashTime -= Time.deltaTime;
                if (actualDashTime <= dashAnimationTime)
                    dashTrigger.SetActive(true);
                if (actualDashTime <= dashAnimationTime * 0.1)
                    dashTrigger.SetActive(false);
                if (actualDashTime <= 0)
                {
                    actualDashTime = dashAnimationTime;
                    state = State.IDLE;
                }
                break;

            case State.DARK:

                actualDarkTime -= Time.deltaTime;

                if (actualDarkTime <= darkAnimationTime * 0.6)
                    darkBox.SetActive(true);
                if (actualDarkTime <= 0)
                {
                    darkBox.SetActive(false);
                    actualDarkTime = darkAnimationTime;
                    state = State.IDLE;
                }

                break;



            case State.FIRE:
                actualFireTime -= Time.deltaTime;
                if (actualFireTime <= fireAnimationTime * 0.8)
                {
                    if (Time.time - initMeteorTime >= meteorInterval && meteorCounter < meteorNum)
                    {
                        Vector3 t = transform.position + meteorRotation[meteorCounter] * Vector3.forward * meteorRange;
                        t.y = 2;
                        Instantiate(fireBall, t, meteorRotation[meteorCounter]);
                        meteorCounter++;
                        initMeteorTime = Time.time;
                    }
                }
                if (actualFireTime <= 0 && meteorCounter >= meteorNum)
                {
                    actualFireTime = fireAnimationTime;
                    state = State.IDLE;
                }
                break;
            case State.DIZZY:
                actualDizzyTime -= Time.deltaTime;
                if (actualDizzyTime <= 0)
                {
                    actualDizzyTime = dizzyAnimationTime;
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

    void Dash()
    {
        animator.SetTrigger("Dash");
        damage = dashDmg;
        state = State.DASH;
    }

    void Fire()
    {
        animator.SetTrigger("Fire");
        damage = fireDmg;
        initMeteorTime = Time.time;
        meteorCounter = 0;
        for (int i = 0; i < meteorNum; i++)
        {
            meteorRotation[i] = Quaternion.Euler(0, Random.Range(0, 359), 0).normalized;
        }
        state = State.FIRE;
    }

    void Dark()
    {
        animator.SetTrigger("Dark");
        damage = darkDmg;
        state = State.DARK;
    }

    void Dizzy()
    {
        animator.SetTrigger("GodMode");
        state = State.DIZZY;
    }

    public void Heal()
    {
        if (health<=maxHealth-20)
        {
            health += 20;
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