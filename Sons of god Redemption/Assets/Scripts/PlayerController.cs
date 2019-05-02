using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    //Player stats
    public Stats stats;
    public int health, movementSpeed, baseAttack, fireDmg, lightDmg, darkLifeSteal;

    //Canvas
    public Slider healthBar;
    public Slider potionBar;
    public GameObject fireUI;
    public GameObject lightUI;
    public GameObject darkUI;

    //Enums
    enum States { Idle, Walking, Running, Dashing, Attacking, Damaged, Dead, MAX };
    enum Attacks { LightAttack1, LightAttack2, LightAttack3, StrongAttack1, StrongAttack2, StrongAttack3, NotAtt };
    enum ButtonInputs { Dash, LightAttack, StrongAttack, padLeft, padRight, padUp, padDown, Interact, MAX };
    enum Elements {Holy, Fire, Dark, MAX };

    //External attributes
    InputManager inputManager;
    Animator animator;
    public GameObject canvas, youDead, flameCone, lightHit, healParticles;
    [SerializeField] GameObject weapon;
    [SerializeField] GameObject[] elements = new GameObject[(int)Elements.MAX];
    public float bossDmg;

    //Inputs
    bool[] inputs = new bool[(int)ButtonInputs.MAX];
    private float xAxis, yAxis;

    //General atributes
    public LevelController levelController;
    public Vector3 direction;
    public int walkVelocity, runVelocity, dashDistance;    
    public float dashCooldownTime, dashDuration, deadDuration, onHitAnimDelay, damage, lightCooldown, darkCooldown, healCooldown, actualHealCooldown, damagedCooldown, actualDamagedCooldown;
    private float dashCooldownCounter, actualDashTime, actualDeadTime, animLength, animDuration, onHitDelay, actualLightCooldown, actualDarkCooldown;
    public bool interact, fireHit, explosionHit, meteorHit, spawnMe, healOnCD, finalDashHit, onCinematic;
    private bool dashed, attacked, transition, hit, isLightHit, lightOnCD, darkOnCD,  dead, darkHit, attackTransition;
    const float velChange = 0.5f;

    public static bool damaged;

    //State machine
    [SerializeField] States states, nextState;
    [SerializeField] Attacks attacks;

    void Start () {

        //initialize player stats
        health = stats.health;
        movementSpeed = stats.movementSpeed;
        baseAttack = stats.baseAttack;
        healthBar.maxValue = health;
        healthBar.value = health;
        potionBar.maxValue = healCooldown;
        potionBar.value = healCooldown;
        walkVelocity = movementSpeed;
        runVelocity = movementSpeed * 2;

        //initialize player atributes(not stats)
        inputManager = GetComponent<InputManager>();
        animator = GetComponent<Animator>();
        canvas = GameObject.Find("Canvas");
        states = States.Idle;
        attacks = Attacks.NotAtt;
        onCinematic = finalDashHit = dead = lightOnCD = darkOnCD = healOnCD = darkHit = isLightHit = dashed = attacked = transition = hit = fireHit = damaged = attackTransition = false;
        spawnMe = true;
        dashCooldownCounter = dashCooldownTime;
        actualDashTime = dashDuration;
        actualDeadTime = deadDuration = AnimationLength("Dying",animator);
        actualLightCooldown = lightCooldown;
        actualDarkCooldown = darkCooldown;
        actualHealCooldown = healCooldown;
        onHitDelay = onHitAnimDelay;
        weapon = GameObject.Find("Sword");
        flameCone = GameObject.Find("FlameCone");
        elements[(int)Elements.Fire] = GameObject.Find("Fire particles");
        elements[(int)Elements.Holy] = GameObject.Find("Light particles");
        elements[(int)Elements.Dark] = GameObject.Find("Dark particles");
        elements[(int)Elements.Fire].SetActive(false);
        elements[(int)Elements.Holy].SetActive(false);
        elements[(int)Elements.Dark].SetActive(false);
        flameCone.SetActive(false);
        actualDamagedCooldown = damagedCooldown = AnimationLength("Reaction Hit", animator);
    }
	
	void Update () {

        if (inputManager.addEnemy)
        {
            baseAttack = 200000;
        }
        if (inputManager.clearEnemies)
        {
            baseAttack = stats.baseAttack;
        }
        if (onCinematic)
        {
            movementSpeed = 0;
            states = States.Idle;
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
            animator.SetBool("isIdle", true);
            animator.Play("Idle");
        }
        else
        {
            GetInput();
            movementSpeed = stats.movementSpeed;
        }

        if (actualHealCooldown >= 0 && actualHealCooldown != healCooldown)
        {
            potionBar.value = healCooldown - actualHealCooldown;
        }
        else
        {
            potionBar.value = healCooldown;
        }

        //Element controller
        if (inputs[(int)ButtonInputs.padRight])
        {
            elements[(int)Elements.Fire].SetActive(true);
            elements[(int)Elements.Holy].SetActive(false);
            elements[(int)Elements.Dark].SetActive(false);
            fireUI.SetActive(true);
            lightUI.SetActive(false);
            darkUI.SetActive(false);
            //health = stats.health;
            //healthBar.value = health;
        }
        if (inputs[(int)ButtonInputs.padLeft])
        {
            elements[(int)Elements.Fire].SetActive(false);
            elements[(int)Elements.Holy].SetActive(true);
            elements[(int)Elements.Dark].SetActive(false);
            fireUI.SetActive(false);
            lightUI.SetActive(true);
            darkUI.SetActive(false);
        }
        if (inputs[(int)ButtonInputs.padDown])
        {
            elements[(int)Elements.Fire].SetActive(false);
            elements[(int)Elements.Holy].SetActive(false);
            elements[(int)Elements.Dark].SetActive(true);
            fireUI.SetActive(false);
            lightUI.SetActive(false);
            darkUI.SetActive(true);
        }
        if (inputs[(int)ButtonInputs.padUp])
        {
            if (!healOnCD)
            {
                Instantiate(healParticles, transform);
                healOnCD = true;
                if (health <= stats.health - 40)
                    health += 40;
                else health = stats.health;
                healthBar.value = health;
            }
        }
        if (inputs[(int)ButtonInputs.Dash] && !dashed && !dead && !damaged)
        {
            this.gameObject.layer = 11;
            states = States.Dashing;
            animator.SetBool("isDashing",true);
            dashed = true;
        }
        if (health<=0)
        {
            states = States.Dead;
        }


        //Player state machine
        switch (states) {

            case(States.Idle):

                //play idle animation
                //check if we got input
                //if attack input switch to attack state
                //if dash input switch to dash state
                if (yAxis != 0 || xAxis != 0)
                {
                    states = States.Walking;
                    animator.SetBool("isWalking", true);
                }
                if (inputs[(int)ButtonInputs.LightAttack])
                {
                    states = States.Attacking;
                    attacks = Attacks.LightAttack1;
                }
                if (inputs[(int)ButtonInputs.StrongAttack])
                {
                    states = States.Attacking;
                    attacks = Attacks.StrongAttack1;
                }

                break;

            case (States.Walking):

                //Play walking animation
                //If input > 0.7 switch states to running
                //If input==0 return to idle
                //if attack input switch to attack state
                //if dash input switch to dash state
                Rotation();           
                transform.Translate (transform.forward * walkVelocity*Time.deltaTime,Space.World);

                if (inputs[(int)ButtonInputs.LightAttack])
                {
                    states = States.Attacking;
                    attacks = Attacks.LightAttack1;
                }
                if (inputs[(int)ButtonInputs.StrongAttack])
                {
                    states = States.Attacking;
                    attacks = Attacks.StrongAttack1;
                }
                else if (Mathf.Abs(xAxis) > velChange || Mathf.Abs(yAxis) > velChange)
                {
                    states = States.Running;
                    animator.SetBool("isRunning", true);
                }
                else if (yAxis == 0 && xAxis == 0)
                {
                    states = States.Idle;
                    animator.SetBool("isWalking", false);
                }
               

                break;

            case (States.Running):

                //Play runnig animation
                //If input <0.7 switch state to walking
                //If input==0 return to idle
                //if attack input switch to attack state
                //if dash input switch to dash state
                Rotation();                       
                transform.Translate(transform.forward * runVelocity * Time.deltaTime, Space.World);

                if (inputs[(int)ButtonInputs.LightAttack])
                {
                    states = States.Attacking;
                    attacks = Attacks.LightAttack1;
                }
                if (inputs[(int)ButtonInputs.StrongAttack])
                {
                    states = States.Attacking;
                    attacks = Attacks.StrongAttack1;
                }
                else if (Mathf.Abs(yAxis) < velChange && Mathf.Abs(xAxis) < velChange)
                {
                    states = States.Walking;
                    animator.SetBool("isWalking", true);
                    animator.SetBool("isRunning", false);
                }
                
               
                break;

            case (States.Dashing):
                animator.ResetTrigger("Hit");
                actualDashTime -= Time.deltaTime;
                if (actualDashTime >= 0)
                    Dash();
                else
                {
                    ResetDash();
                    this.gameObject.layer = 0;
                }
                 
                break;
            case (States.Attacking):
                Rotation();
                animator.SetBool("isIdle", false);
                animator.SetBool("isRunning", false);
                animator.SetBool("isWalking", false);
                switch (attacks) {

                    case (Attacks.LightAttack1):
                        damage = baseAttack;

                        if (!attacked)
                        {
                            animator.SetTrigger("lightAttack1");
                            animLength = animDuration = AnimationLength("light attack", animator);
                            attacked = true;
                        }
                        if (inputs[(int)ButtonInputs.LightAttack] && animLength > animDuration * 0.3)
                        {
                            transition = true;
                        }
                        if (animLength < animDuration * 0.7)
                        {
                            //weapon.tag = "Weapon";
                            weapon.tag = "LightAttack1";
                        }
                        if (attackTransition)
                        {
                            if (transition)
                            {
                                attacks = Attacks.LightAttack2;
                            }
                            else
                            {
                                attacks = Attacks.NotAtt;
                                states = CheckState();
                              
                                weapon.tag = "Untagged";
                            }
                            attackTransition = attacked = transition = false;
                        }

                        break;

                    case (Attacks.LightAttack2):

                        if (!attacked)
                        {
                            animator.SetTrigger("lightAttack2");
                            animLength = animDuration = AnimationLength("light attack 2", animator);
                            attacked = true;
                            weapon.tag = "LightAttack2";
                        }
                        if (inputs[(int)ButtonInputs.LightAttack])
                        {
                            transition = true;
                        }
                        if (attackTransition)
                        {
                            if (transition)
                            {
                                attacks = Attacks.LightAttack3;

                            }
                            else
                            {
                                attacks = Attacks.NotAtt;
                                states = CheckState();

                                weapon.tag = "Untagged";
                            }
                            attackTransition = attacked = transition = false;
                        }                                            

                        break;
                    case (Attacks.LightAttack3):

                        if (!attacked)
                        {
                            animator.SetTrigger("lightAttack3");
                            animLength = animDuration = AnimationLength("light attack 3", animator);
                            attacked = true;
                            weapon.tag = "LightAttack3";
                        }
                        if (animLength < animDuration * 0.7)
                        {
                            if (elements[(int)Elements.Fire].activeSelf)
                            {
                                damage = baseAttack + fireDmg;
                                flameCone.SetActive(true);
                            }
                            else if (elements[(int)Elements.Holy].activeSelf)
                            {
                                isLightHit = true;
                            }
                            else if (elements[(int)Elements.Dark].activeSelf)
                            {
                                darkHit = true;
                            }
                        }
                        if (animLength < animDuration * 0.3)
                        {
                            weapon.tag = "Untagged";
                        }
                        if (attackTransition)
                        {
                            attacks = Attacks.NotAtt;
                            states = CheckState();
                            flameCone.SetActive(false);
                            isLightHit = false;
                            darkHit = false;
                            attackTransition = attacked = transition = false;
                        }

                        break;

                    case (Attacks.StrongAttack1):
                        damage = baseAttack + (baseAttack * 0.5f);
                        if (!attacked)
                        {
                            animator.SetTrigger("strongAttack1");
                            animLength = animDuration = AnimationLength("strong attack", animator);
                            attacked = true;                           
                        }
                        if (inputs[(int)ButtonInputs.StrongAttack])
                        {
                            transition = true;
                        }
                        if (animLength < animDuration * 0.8)
                        {
                            weapon.tag = "StrongAttack1";
                            //weapon.tag = "Weapon";                          
                        }
                        if (attackTransition)
                        {
                            if (transition)
                            {
                                attacks = Attacks.StrongAttack2;
                            }
                            else
                            {
                                attacks = Attacks.NotAtt;
                                states = CheckState();
                                weapon.tag = "Untagged";
                            }
                            attackTransition = attacked = transition = false;
                        }

                        break;

                    case (Attacks.StrongAttack2):

                        if (!attacked)
                        {
                            animator.SetTrigger("strongAttack2");
                            animDuration = animLength = AnimationLength("strong attack 2", animator);
                            attacked = true;
                            //weapon.tag = "Weapon";
                            weapon.tag = "StrongAttack2";
                        }
                        if (animLength < animDuration * 0.5)
                        {
                            if (elements[(int)Elements.Fire].activeSelf)
                            {
                                flameCone.SetActive(true);
                            }
                            else if (elements[(int)Elements.Holy].activeSelf)
                            {
                                isLightHit = true;
                            }
                            else if (elements[(int)Elements.Dark].activeSelf)
                            {
                                darkHit = true;
                            }
                        }
                        if (attackTransition)
                        {
                            attacks = Attacks.NotAtt;
                            states = CheckState();
                            weapon.tag = "Untagged";
                            isLightHit = false;
                            darkHit = false;
                            flameCone.SetActive(false);
                            attackTransition = attacked = transition = false;
                        } 

                        break;

                    default:

                        break;

                }
                animLength -= Time.deltaTime;
                //nextState = CheckState();
                break;

            case (States.Dead):
                actualDeadTime -= Time.deltaTime;
                if (!dead)
                {
                    Instantiate(youDead, canvas.transform);
                    animator.SetTrigger("Dead");
                    dead = true;
                    animator.ResetTrigger("Hit");
                }
                if (actualDeadTime<=0)
                {
                    health = stats.health;
                    healthBar.value = health;
                    spawnMe = true;
                    states = States.Idle;
                    actualDeadTime = deadDuration;
                    ResetAll();
                }
                    break;

            case (States.Damaged):
                actualDamagedCooldown -= Time.deltaTime;
                if (actualDamagedCooldown <= 0)
                {
                    ResetHit();
                }

                break;
            default:
                break;
            
        }
        
        DashCooldown();
        LightCooldown();
        DarkCooldown();
        HealCooldown();

        if (hit)
        {
            onHitDelay -= Time.deltaTime;
            if (onHitDelay <= 0)
            {
                onHitDelay = onHitAnimDelay;
                hit = false;
            }
        }

        
        if (fireHit)
        {
            health -= (int)bossDmg;
            healthBar.value = health;
        }

        if (explosionHit)
        {
            health -= (int)bossDmg;
            healthBar.value = health;
            explosionHit = false;
        }

        if (meteorHit)
        {
            health -= (int)bossDmg;
            healthBar.value = health;
            meteorHit = false;
        }


        fireHit = false;

    }

    public void AttackTransition()
    {
        attackTransition = true;
    }

    private void ResetAll()
    {
        weapon.tag = "Untagged";
        states = States.Idle;
        attacks = Attacks.NotAtt;
        dead = lightOnCD = darkOnCD = darkHit =  isLightHit = dashed = attacked = transition = hit = fireHit = damaged = false;
        dashCooldownCounter = dashCooldownTime;
        actualDashTime = dashDuration;
        actualLightCooldown = lightCooldown;
        actualDarkCooldown = darkCooldown;
        onHitDelay = onHitAnimDelay;
        animator.ResetTrigger("lightAttack1");
        animator.ResetTrigger("lightAttack2");
        animator.ResetTrigger("lightAttack3");
        animator.ResetTrigger("strongAttack1");
        animator.ResetTrigger("strongAttack2");
        animator.ResetTrigger("Hit");
        animator.SetBool("isIdle", true);
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", false);
        flameCone.SetActive(false);
        levelController.OpenAllDoors();
    }

    private void ResetDash()
    {
        weapon.tag = "Untagged";
        states = States.Idle;
        attacks = Attacks.NotAtt;
        lightOnCD = darkOnCD = darkHit = isLightHit = attacked = transition = hit = fireHit = damaged = false;
        actualLightCooldown = lightCooldown;
        actualDarkCooldown = darkCooldown;
        actualDashTime = dashDuration;
        onHitDelay = onHitAnimDelay;
        animator.ResetTrigger("lightAttack1");
        animator.ResetTrigger("lightAttack2");
        animator.ResetTrigger("lightAttack3");
        animator.ResetTrigger("strongAttack1");
        animator.ResetTrigger("strongAttack2");
        animator.SetBool("isIdle", true);
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", false);
        animator.SetBool("isDashing", false);
        flameCone.SetActive(false);
    }

    private void ResetHit()
    {
        weapon.tag = "Untagged";
        attacks = Attacks.NotAtt;
        states = States.Idle;
        damaged = lightOnCD = darkOnCD = darkHit = isLightHit = attacked = transition = hit = fireHit = false;
        actualLightCooldown = lightCooldown;
        actualDarkCooldown = darkCooldown;
        actualDashTime = dashDuration;
        onHitDelay = onHitAnimDelay;
        actualDamagedCooldown = damagedCooldown;
        animator.ResetTrigger("lightAttack1");
        animator.ResetTrigger("lightAttack2");
        animator.ResetTrigger("lightAttack3");
        animator.ResetTrigger("strongAttack1");
        animator.ResetTrigger("strongAttack2");
        animator.SetBool("isIdle", true);
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", false);
        flameCone.SetActive(false);
    }

    void GetInput()
    {
       xAxis = inputManager.xAxis;
       yAxis = inputManager.yAxis;
       inputs[(int)ButtonInputs.Dash] = inputManager.dashButton;
       inputs[(int)ButtonInputs.LightAttack] = inputManager.attackButton;
       inputs[(int)ButtonInputs.StrongAttack] = inputManager.strongAttackButton;
       interact = inputs[(int)ButtonInputs.Interact] = inputManager.interact;
        
       if(inputManager.padXAxis==1)
            inputs[(int)ButtonInputs.padRight] = true;
       else
            inputs[(int)ButtonInputs.padRight] = false;

        if (inputManager.padXAxis == -1)
            inputs[(int)ButtonInputs.padLeft] = true;
        else
            inputs[(int)ButtonInputs.padLeft] = false;

        if (inputManager.padYAxis == 1)
            inputs[(int)ButtonInputs.padUp] = true;
        else
            inputs[(int)ButtonInputs.padUp] = false;

        if (inputManager.padYAxis == -1)
            inputs[(int)ButtonInputs.padDown] = true;
        else
            inputs[(int)ButtonInputs.padDown] = false;
    }

    void Rotation()
    {
            direction.Set(xAxis, 0, yAxis);
            if (direction.magnitude != 0)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction, Vector3.up), 0.15F);
    }

    void Dash()
    {
            transform.position += transform.forward * Time.deltaTime * (dashDistance/dashDuration);
    }

    void DashCooldown()
    {
        if (dashed)
        {
            dashCooldownCounter -= Time.deltaTime;

            if (dashCooldownCounter <= 0f)
            {
                dashed = false;
                dashCooldownCounter = dashCooldownTime;

            }
        }
    }

    void HealCooldown()
    {
        if (healOnCD)
        {
            actualHealCooldown -= Time.deltaTime;
            if (actualHealCooldown <= 0f)
            {
                healOnCD = false;
                actualHealCooldown = healCooldown;
            }
        }
    }

    void LightCooldown()
    {
        if (lightOnCD)
        {
            actualLightCooldown -= Time.deltaTime;
            if (actualLightCooldown<=0f)
            {
                lightOnCD = false;
            }
        }
    }

    void DarkCooldown()
    {
        if (darkOnCD)
        {
            actualDarkCooldown -= Time.deltaTime;
            if (actualDarkCooldown <= 0f)
            {
                darkOnCD = false;
            }
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

    States CheckState()
    {
        animator.SetBool("isIdle", true);
        if (Mathf.Abs(xAxis) > 0.5 || Mathf.Abs(yAxis) > 0.5)
        {
            animator.SetBool("isWalking", true);
            animator.SetBool("isRunning", true);
            return (States.Running);
        }
        else if (xAxis == 0 || yAxis == 0)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
            return (States.Idle);
        }
        else
        {
            animator.SetBool("isWalking", true);
            animator.SetBool("isRunning", false);
            return (States.Walking);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy" && !dead)
        {
            if (weapon.tag != "Untagged")
            {
                hit = true;
                if (darkHit && !darkOnCD && health<=stats.health-15)
                {
                    darkOnCD = true;
                    health += 15;
                    healthBar.value = health;
                    darkHit = false;
                }
            }
            if (isLightHit && !lightOnCD)
            {
                Instantiate(lightHit, other.transform);
                lightOnCD = true;
                if (other.gameObject.name == "FirstBoss")
                {
                    other.GetComponentInParent<FirstBossBehaviour>().health -= lightDmg;
                }
                else if (other.gameObject.name == "SecondBoss(Clone)")
                {
                    other.GetComponentInParent<SecondBossBehaviour>().health -= lightDmg;
                }
                else if (other.gameObject.name == "GoDFinal(Clone)")
                {
                    other.GetComponentInParent<FinalBossBehaviour>().health -= lightDmg;
                }
                else
                {
                    other.GetComponentInParent<Enemy>().health -= lightDmg;
                }
            }
        }
        if (!dead && !damaged)
        {
            if (other.tag == "EnemyWeapon")
            {
                health -= (int)other.GetComponentInParent<Enemy>().baseAttack;
                healthBar.value = health;
                damaged = true;
                states = States.Damaged;
                animator.SetTrigger("Hit");
            }
            if (other.tag == "Arrow")
            {
                health -= 10;
                healthBar.value = health;
                damaged = true;
            }
            if (other.tag == "FirstBossWeapon")
            {
                health -= (int)other.GetComponentInParent<FirstBossBehaviour>().damage;
                healthBar.value = health;
                damaged = true;
                states = States.Damaged;
                animator.SetTrigger("Hit");
            }
            if (other.tag == "FinalBossWeapon" && !finalDashHit)
            {
                finalDashHit = true;
                health -= (int)other.GetComponentInParent<FinalBossBehaviour>().damage;
                healthBar.value = health;
                damaged = true;
                states = States.Damaged;
                animator.SetTrigger("Hit");
            }
        }
        
    }
}