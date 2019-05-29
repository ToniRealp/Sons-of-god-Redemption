using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankEnemy : Enemy {
    enum Attacks { Charge, Basic };
    enum Basics { Basic1, Basic2 };
    enum Charge { Roar, Run, Hit };
    Attacks attacks;
    Basics basics;
    Charge charge;
    bool collided, roar, swipe, rushOnCooldown, lookPlayer, damagedFlag, deathFlag;
    public float baseAttackDuration, baseAttackCooldown, rushCooldown, rushDistance;
    public float x, accelerationTime, baseAcceleration, d;
    public GameObject weapon2;
    public bool transition;


    new void Start()
    {
        base.Start();
        collided = roar = swipe = rushOnCooldown = lookPlayer = damagedFlag = deathFlag = false;
        baseAttackDuration = baseAttackCooldown = animTimes["Attack1"].duration + animTimes["Attack2"].duration;
        rushDistance = attackDistance * 8;
        
    }

    private void Update()
    {
        if (health <= 0)
        {
            animator.SetBool("isDead", true);
            state = State.DEATH;
            if (!audioManager.isPlaying("MinoDeath"))
                audioManager.Play("MinoDeath");
        }
        else
            UpdateHealthText();

        UseFullDetectionSystem();

        d = NavAgent.remainingDistance;

        if (attackOnCooldown)
        {
            if ((attackCooldown -= Time.deltaTime) <= 0)
            {
                attackCooldown = attackSpeed;
                attackOnCooldown = false;
            }
        }

        if (rushOnCooldown)
        {
            if ((rushCooldown -= Time.deltaTime) <= 0)
            {
                rushCooldown = attackSpeed * 2;
                rushOnCooldown = false;
            }
        }

        switch (state)
        {
            case State.SEARCHING:
                // Full speed
                NavAgent.acceleration = 8;
                ChangeSpeed(movementSpeed);
                LookToDestination();

                if (NavAgent.velocity.magnitude > 0.1f)
                {
                    animator.SetBool("IsIdle", false);
                    animator.SetBool("IsRunning", true);
                    if (!audioManager.isPlaying("MinoWalk"))
                        audioManager.Play("MinoWalk");
                }
                else
                {
                    audioManager.Stop("MinoWalk");
                    animator.SetBool("IsRunning", false);
                    animator.SetBool("IsIdle", true);
                }

                if (!playerDetected)
                {
                    // Reset destiny location
                    if ((moveCooldown -= Time.deltaTime) <= 0)
                    {
                        SetRandomDestination();
                        moveCooldown = timeToMove;
                    }
                }
                else
                {
                       state = State.CHASING;                  
                }  

                // Go to random destination
                MoveToDestination();

                break;

            case State.CHASING:
                ChangeSpeed(movementSpeed);
                // Go to player Position
                destination = playerPosition;
                LookToDestination();
                MoveToDestination();

                if (NavAgent.velocity.magnitude > 0.1f)
                {
                    animator.SetBool("IsIdle", false);
                    animator.SetBool("IsRunning", true);
                    if (!audioManager.isPlaying("MinoWalk"))
                        audioManager.Play("MinoWalk");
                }
                else
                {
                    audioManager.Stop("MinoWalk");
                    animator.SetBool("IsRunning", false);
                    animator.SetBool("IsIdle", true);
                }

                if (playerDetected)
                {
                    // If in attack conditions, go to attack
                    if (DistanceToDestination(destination) <= attackDistance && !attackOnCooldown)
                    {
                        audioManager.Stop("MinoWalk");
                        state = State.ATTAKING;
                        attacks = Attacks.Basic;
                        basics = Basics.Basic1;
                        animator.SetTrigger("Attack1");
                    }
                    else if (DistanceToDestination(destination) <= rushDistance && !rushOnCooldown && !attackOnCooldown)
                    {
                        audioManager.Stop("MinoWalk");
                        state = State.ATTAKING;
                        attacks = Attacks.Charge;
                        charge = Charge.Roar;
                        animator.SetTrigger("Roar");
                        if (!audioManager.isPlaying("MinoRoar"))
                            audioManager.Play("MinoRoar");
                    }
                }
                else
                {
                    state = State.SEARCHING;
                }

                break;

            case State.ATTAKING:

                if(lookPlayer) LookToPlayer();
                // Save player last known position
                switch (attacks)
                {
                    case Attacks.Basic:
                        ChangeSpeed(0);
                        switch (basics)
                        {
                            case Basics.Basic1:
                                if (transition)
                                {
                                    if (DistanceToDestination(playerPosition) < attackDistance)
                                    {
                                        animator.SetTrigger("Attack2");
                                        basics = Basics.Basic2;
                                    }
                                    else
                                    {
                                        //attackOnCooldown = true;
                                        state = State.CHASING;
                                        animator.SetTrigger("AttackEnd");
                                        //Debug.Log("exitii");
                                    }
                                    transition = false;
                                }
                                break;
                            case Basics.Basic2:
                                if (transition)
                                {
                                    state = State.CHASING;
                                    transition = false;
                                    animator.SetTrigger("AttackEnd");
                                   
                                    //attackOnCooldown = true;
                                }
                                break;
                        }
                        break;

                    case Attacks.Charge:
                        switch (charge)
                        {
                            case Charge.Roar:
                                ChangeSpeed(0);
                                destination = playerPosition;
                                collided = false;
                                if (transition)
                                {
                                    transition = false;
                                    animator.SetTrigger("Charge");
                                    charge = Charge.Run;
                                    audioManager.Stop("MinoRoar");
                                    if (!audioManager.isPlaying("MinoGallop"))
                                        audioManager.Play("MinoGallop");
                                }
                                break;
                            case Charge.Run:
                                ChangeSpeed(movementSpeed*15);
                                NavAgent.acceleration = 500;
                                MoveToDestination();
                                if(collided||NavAgent.remainingDistance<=2.6f)
                                {
                                    collided = false;
                                    animator.SetTrigger("Swipe");
                                    charge = Charge.Hit;
                                    NavAgent.acceleration = baseAcceleration;
                                    audioManager.Stop("MinoGallop");
                                    if (!audioManager.isPlaying("MinoPunch"))
                                        audioManager.Play("MinoPunch");
                                }
                                break;
                            case Charge.Hit:
                                ChangeSpeed(0);
                                if (transition)
                                {
                                    state = State.CHASING;
                                    transition = false;
                                    animator.SetTrigger("AttackEnd");
                                    audioManager.Stop("MinoPunch");
                                }
                                break;
                        }
                        
                        break;
                }
                break;
            case State.DAMAGED:
                if (health <= 0)
                {
                    animator.SetTrigger("death");
                    state = State.DEATH;
                    animator.SetBool("isDead", true);
                    break;
                }
                if (!damagedFlag)
                {
                    animator.SetTrigger("Damaged");
                    DeactivateWeapon();
                    animator.ResetTrigger("Roar");
                    animator.ResetTrigger("Swipe");
                    animator.ResetTrigger("Attack1");
                    animator.ResetTrigger("Attack2");
                    animator.ResetTrigger("Charge");
                    animator.ResetTrigger("AttackEnd");
                    collided = transition = false;
                    damagedFlag = true;
                }
             
            
                if (transition)
                {
                    damaged = transition = damagedFlag = false;
                    animator.SetTrigger("AttackEnd");
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

            case State.DEATH:
                ChangeSpeed(0);
     
                if (!deathFlag)
                {
                    
                    DeactivateWeapon();
                    animator.ResetTrigger("Roar");
                    animator.ResetTrigger("Swipe");
                    animator.ResetTrigger("Attack1");
                    animator.ResetTrigger("Attack2");
                    animator.ResetTrigger("Charge");
                    animator.ResetTrigger("AttackEnd");
                    animator.ResetTrigger("Damaged");
                    collided = transition = false;
                    deathFlag = true;
                    animator.SetTrigger("death");
                    Destroy(healthTextGO);
                }

                FadeOut();

                break;

            default:
                break;
        }
    }
   

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player")
        {
            collided = true;
            
        }
        ///TODO: Detect collision in child gameobject
        if(state == State.ATTAKING && attacks == Attacks.Basic)
        {
            if (!audioManager.isPlaying("MinoPunch"))
                audioManager.Play("MinoPunch");
        }
    }
    new private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Untagged")
        {
            Instantiate(blood, bloodPosition.position, bloodPosition.rotation, transform);
            health -= (int)other.GetComponentInParent<PlayerController>().damage;
            if (other.tag == "Arrow")
            {
                health -= 15;
            }
            if (other.tag == "StrongAttack2" && !damaged )
            {
                
                state = State.DAMAGED;
                animTimes["Reaction Hit"].cooldown = animTimes["Reaction Hit"].duration;
                damaged = true;
            }
        } 
    }
    public void SetTransition()
    {
        transition = true;
    }

    public void ActivateWeapon1()
    {
        weapon.tag = "EnemyWeapon";
    }

    public void ActivateWeapon2()
    {
        weapon2.tag = "EnemyWeapon";
    }

    public void DeactivateWeapon()
    {
        weapon.tag = weapon2.tag = "Untagged";
    }

    public void ActivateLookPlayer()
    {
        lookPlayer = true;
    }

    public void DeactivateLookPlayer()
    {
        lookPlayer = false;
    }

    public void Death()
    {
        finishedDeathAnimation=true;
        SetMaterialTransparent();
    }

}