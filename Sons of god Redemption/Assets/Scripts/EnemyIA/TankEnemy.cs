using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankEnemy : Enemy {
    enum Attacks { Charge, Basic };
    enum Basics { Basic1, Basic2};
    Attacks attacks;
    Basics basics;
    bool collided, roar, swipe, rushOnCooldown;
    public float baseAttackDuration, baseAttackCooldown, rushCooldown, rushDistance;
    public float x, accelerationTime;
    public GameObject weapon2;
    public bool transition;


    new void Start()
    {
        base.Start();
        collided = roar = swipe = rushOnCooldown = false;
        baseAttackDuration = baseAttackCooldown = animTimes["Attack1"].duration + animTimes["Attack2"].duration;
        rushDistance = attackDistance * 8;
        
    }

    private void Update()
    {
        if (health <= 0)
            Die();

        UpdateHealthText();
        UseFullDetectionSystem();
       

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
                }
                else
                {
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
                }
                else
                {
                    animator.SetBool("IsRunning", false);
                    animator.SetBool("IsIdle", true);
                }

                if (playerDetected)
                {
                    // If in attack conditions, go to attack
                    if (DistanceToDestination(destination) <= attackDistance && !attackOnCooldown)
                    {
                        state = State.ATTAKING;
                        attacks = Attacks.Basic;
                        basics = Basics.Basic1;
                        animator.SetTrigger("Attack1");
                    }
                    //else if(DistanceToDestination(destination)<= rushDistance && !rushOnCooldown && !attackOnCooldown)
                    //{
                    //    state = State.ATTAKING;
                    //    attacks = Attacks.Charge;
                    //    animator.SetTrigger("Roar");
                    //}
                }
                else
                {
                    state = State.SEARCHING;
                }

                break;

            case State.ATTAKING:
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
                                        LookToPlayer();
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
                        
                        break;
                }
                break;
            case State.DAMAGED:
                DeactivateWeapon();
                baseAttackCooldown = baseAttackDuration;
                animTimes["Reaction Hit"].cooldown -= Time.deltaTime;
                animTimes["Roar"].cooldown = animTimes["Roar"].duration;
                animTimes["Swipe"].duration = animTimes["Swipe"].cooldown;
                collided = false;
                if (animTimes["Reaction Hit"].cooldown <= 0)
                {
                    animTimes["Reaction Hit"].cooldown = animTimes["Reaction Hit"].duration;
                    damaged = false;
                    
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
                animator.SetTrigger("Damaged");
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
}