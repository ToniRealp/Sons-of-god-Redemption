using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankEnemy : Enemy {
    enum Attacks { Charge, Basic };
    Attacks attacks;
    bool collided, roar, swipe;
    float baseAttackDuration, baseAttackCooldown;
    public float x, accelerationTime;

    new void Start()
    {
        base.Start();
        collided = roar = swipe = false;
        baseAttackDuration = baseAttackCooldown = animTimes["Attack1"].duration + animTimes["Attack2"].duration;
        
    }

    private void Update()
    {
        if (health <= 0)
            Die();

        UpdateHealthText();
        UseFullDetectionSystem();
       

        if (attackOnCooldown)
        {
            if ((actualAttackCooldown -= Time.deltaTime) <= 0)
            {
                actualAttackCooldown = attackCooldown;
                attackOnCooldown = false;
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
                    if (DistanceToDestination() <= attackDistance && !attackOnCooldown)
                    {
                        state = State.ATTAKING;
                        animator.SetTrigger("Attack1");
                    }
                    else
                    {
                        state = State.CHASING;
                        animator.SetTrigger("Roar");
                    }
                }  

                // Go to random destination
                MoveToDestination();

                break;

            case State.CHASING:
                // Full speed
                ChangeSpeed(0);
                // Go to player Position

                if (!roar)
                {
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
                            animTimes["Swipe"].duration = animTimes["Swipe"].cooldown;
                            accelerationTime = 0;
                            if (playerDetected)
                            {
                                destination = playerPosition;
                                if (DistanceToDestination() <= attackDistance && !attackOnCooldown)
                                {
                                    state = State.ATTAKING;
                                    animator.SetTrigger("Attack1");
                                }
                                else
                                {
                                    state = State.CHASING;
                                    animator.SetTrigger("Roar");
                                }
                            }
                            else
                                state = State.SEARCHING;
                        }
                    }
                }
                break;

            case State.ATTAKING:
                // Save player last known position
                ChangeSpeed(0);
                destination = playerPosition;
                LookToDestination();

                baseAttackCooldown -= Time.deltaTime;
                if (baseAttackCooldown < 0)
                {
                    if (playerDetected)
                    {
                        if(DistanceToDestination() <= attackDistance && !attackOnCooldown)
                        {
                            state = State.ATTAKING;
                            animator.SetTrigger("Attack1");
                        }
                        else
                        {
                            state = State.CHASING;
                            animator.SetTrigger("Roar");
                        }
                        
                    }
                    else
                    {
                        state = State.SEARCHING;
                    }
                    baseAttackCooldown = baseAttackDuration;
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