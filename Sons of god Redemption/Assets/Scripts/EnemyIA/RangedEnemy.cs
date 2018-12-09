using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : Enemy {

    public GameObject arrow;
    public Transform spawnPos;
    public float bulletSpeed;
    private bool shoot;

    new private void Start()
    {
        base.Start();
        shoot = true;
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
            if ((attackCooldown -= Time.deltaTime) <= 0)
            {
                attackCooldown = attackSpeed;
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
                if ((moveCooldown -= Time.deltaTime) <= 0)
                {
                    SetRandomDestination();
                    moveCooldown = timeToMove;
                }

                MoveToDestination();
                LookToPlayer();

                if (playerDetected)
                {
                    // If in attack conditions, go to attack
                    if (DistanceToDestination() <= attackDistance && !attackOnCooldown)
                    {
                        state = State.ATTAKING;
                        animator.SetTrigger("Shoot");
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

                animTimes["Shoot"].cooldown -= Time.deltaTime;

                // When attack time finishes
                if(animTimes["Shoot"].cooldown <= animTimes["Shoot"].duration/2 && shoot)
                {
                    Shoot();
                    shoot = false;
                }

                if (animTimes["Shoot"].cooldown >= animTimes["Shoot"].duration / 2)
                    LookToPlayer();

                if (animTimes["Shoot"].cooldown <= 0)
                {
                    //Put it on cooldown and change status
                    attackOnCooldown = true;
                    shoot = true;
                    animTimes["Shoot"].cooldown = animTimes["Shoot"].duration;

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
                animator.SetBool("Shoot", false);
                weapon.tag = "Untagged";
                animTimes["Shoot"].cooldown = animTimes["Shoot"].duration;
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
    void Shoot()
    {
        // Create the Bullet from the Bullet Prefab
        var bullet = (GameObject)Instantiate(arrow, spawnPos.position, spawnPos.rotation);
        // Add velocity to the bullet
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * bulletSpeed;
        // Destroy the bullet after 2 seconds
        Destroy(bullet, 2.0f);
    }
}
