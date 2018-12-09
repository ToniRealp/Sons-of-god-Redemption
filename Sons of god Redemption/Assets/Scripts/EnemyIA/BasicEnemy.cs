using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : Enemy {

    new void Start () {
        base.Start();

	}

    private void Update()
    {
        if (health <= 0)
            Die();

        UpdateHealthText();
        UseFullDetectionSystem();
        
        if (damaged)
        {
            state = State.DAMAGED;
        }

        if (animTimes["Reaction Hit"].cooldown > 0f)
            animTimes["Reaction Hit"].cooldown -= Time.deltaTime;
        

        if (attackOnCooldown)
        {
            if ((actualAttackCooldown -= Time.deltaTime) <= 0)
            {
                actualAttackCooldown = attackCooldown;
                attackOnCooldown = false;
            }
        }

        if (actualDamagedCooldown > 0f)
            actualDamagedCooldown -= Time.deltaTime;

        damaged = false;

        // Set animation state depending on speed
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

        switch (state)
        {
            case State.SEARCHING:
                // Full speed
                ChangeSpeed(movementSpeed);
                LookToDestination();

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
                // Full speed
                ChangeSpeed(movementSpeed);
                // Go to player Position
                destination = playerPosition;
                LookToDestination();
                MoveToDestination();

                if (playerDetected)
                {
                    // If in attack conditions, go to attack
                    if (DistanceToDestination() <= attackDistance && !attackOnCooldown)
                    {
                        state = State.ATTAKING;
                        animator.SetBool("Attack", true);
                    }
                }
                else
                {
                    state = State.SEARCHING;
                }
                break;

            case State.ATTAKING:
                // No movement or rotation
                ChangeSpeed(0);

                // Save player last known position
                destination = playerPosition;

                // When attack time finishes
                animTimes["Attack"].cooldown -= Time.deltaTime;
                if (animTimes["Attack"].cooldown <= animTimes["Attack"].start)
                    weapon.tag = "EnemyWeapon";
                if (animTimes["Attack"].cooldown <= animTimes["Attack"].end)
                    weapon.tag = "Untagged";
                if (animTimes["Attack"].cooldown >= animTimes["Attack"].duration* 0.75f)
                {
                    LookToDestination();
                }
                if (animTimes["Attack"].cooldown - animTimes["Attack"].duration / 2 <= 0)
                {
                    animator.SetBool("Attack", false);
                }
                if (animTimes["Attack"].cooldown <= 0)
                {
                    //Put it on cooldown and change status
                    attackOnCooldown = true;
                    animTimes["Attack"].cooldown = animTimes["Attack"].duration;

                    if (playerDetected)
                    {
                        state = State.CHASING;
                    }
                    else
                    {
                        state = State.SEARCHING;
                    }
                };
                break;

            case State.DAMAGED:
                // Cancel Attack animation if getting hit
                animator.SetBool("Attack", false);
                weapon.tag = "Untagged";
                animTimes["Attack"].cooldown = animTimes["Attack"].duration;
                // No movement
                ChangeSpeed(0);
                // But rotation
                LookToDestination();

                // When damage animation finishes change state
                animTimes["Reaction Hit"].cooldown -= Time.deltaTime;
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
    protected void OnTriggerEnter(Collider other)
    {
        if (other.tag != lastTag && other.tag != "Untagged")
        {
            lastTag = other.tag;
            if (actualDamagedCooldown <= 0f)
            {
                damaged = true;
                actualDamagedCooldown = damagedCooldown;
                    animator.SetTrigger("Damaged");
            }
            Instantiate(blood, bloodPosition.position, bloodPosition.rotation, transform);
            health -= (int)other.GetComponentInParent<PlayerController>().damage;
        }
    }
}