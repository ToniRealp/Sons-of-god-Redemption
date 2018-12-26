using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankEnemy : Enemy {
    enum Attacks { Charge, Basic };
    Attacks attacks;
    bool collided, roar, swipe, rushOnCooldown;
    public float baseAttackDuration, baseAttackCooldown, rushCooldown, rushDistance;
    public float x, accelerationTime;


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
                        animator.SetTrigger("Attack1");
                    }
                    else if(DistanceToDestination(destination)<= rushDistance && !rushOnCooldown && !attackOnCooldown)
                    {
                        state = State.ATTAKING;
                        attacks = Attacks.Charge;
                        animator.SetTrigger("Roar");
                    }
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
                        destination = playerPosition;
                        LookToDestination();

                        baseAttackCooldown -= Time.deltaTime;
                        if (baseAttackCooldown < 0)
                        {
                            attackOnCooldown = true;
                            if (playerDetected)
                            {
                                state = State.CHASING;
                            }
                            else
                            {
                                state = State.SEARCHING;
                            }
                            baseAttackCooldown = baseAttackDuration;
                        }

                        break;

                    case Attacks.Charge:
                        if (!roar)
                        {
                            ChangeSpeed(0);
                            destination = playerPosition;
                            LookToDestination();
                            animTimes["Roar"].cooldown -= Time.deltaTime;
                            if (animTimes["Roar"].cooldown < 0)
                            {
                                roar = true;
                                animTimes["Roar"].cooldown = animTimes["Roar"].duration;
                            }
                        }
                        else
                        {
                            ChangeSpeed(movementSpeed * 17.5f);
                            NavAgent.acceleration = 1000;
                            MoveToDestination();
                            accelerationTime += Time.deltaTime;
                            if (collided || NavAgent.velocity.magnitude < 0.5 && accelerationTime > 0.2)
                            {
                                ChangeSpeed(0);
                                animTimes["Swipe"].duration -= Time.deltaTime;
                                if (!swipe)
                                {
                                    animator.SetTrigger("Swipe");
                                    swipe = true;
                                }

                                if (animTimes["Swipe"].duration < 0)
                                {
                                    roar = false;
                                    collided = false;
                                    swipe = false;
                                    rushOnCooldown = true;
                                    animTimes["Swipe"].duration = animTimes["Swipe"].cooldown;
                                    accelerationTime = 0;
                                    if (playerDetected)
                                    {
                                        state = State.CHASING;
                                    }
                                    else
                                        state = State.SEARCHING;
                                }
                            }
                        }
                        break;
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
}