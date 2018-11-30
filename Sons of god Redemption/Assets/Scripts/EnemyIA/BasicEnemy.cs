using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : Enemy {

    public GameObject blood;
    public Transform bloodPosition;

    // Use this for initialization
    new void Start () {
        base.Start();
        SetRandomDestination();
	}
	
	// Update is called once per frame
	void Update () {

        if(health<=0)
            Die();

        UpdateHealthText();
        UseFullDetectionSystem();
        LookToDestination();

        if (damaged)
        {
            state = State.DAMAGED;
        }

        if (attackOnCooldown)
        {
            if ((actualAttackCooldown -= Time.deltaTime) <= 0)
            {
                actualAttackCooldown = attackCooldown;
                attackOnCooldown = false;
            }
        }

        if (animTimes["Zombie Reaction Hit"].cooldown > 0f)
            animTimes["Zombie Reaction Hit"].cooldown -= Time.deltaTime;

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
                MoveToDestination();

                // When attack time finishes
                animTimes["Zombie Attack"].cooldown -= Time.deltaTime;
                if (animTimes["Zombie Attack"].cooldown <= animTimes["Zombie Attack"].start)
                    weapon.tag = "EnemyWeapon";
                if (animTimes["Zombie Attack"].cooldown <= animTimes["Zombie Attack"].end)
                    weapon.tag = "Untagged";
                if (animTimes["Zombie Attack"].cooldown - animTimes["Zombie Attack"].duration / 2 <= 0)
                {
                    animator.SetBool("Attack", false);

                }
                if (animTimes["Zombie Attack"].cooldown <= 0)
                {
                    //Put it on cooldown and change status
                    attackOnCooldown = true;
                    animTimes["Zombie Attack"].cooldown = animTimes["Zombie Attack"].duration;

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

            case State.DAMAGED:
                // Cancel Zombie Attack animation if getting hit
                animator.SetBool("Attack", false);
                weapon.tag = "Untagged";
                animTimes["Zombie Attack"].cooldown = animTimes["Zombie Attack"].duration;
                // No movement
                ChangeSpeed(0);
                // But rotation
                LookToDestination();

                // When damage animation finishes change state
                animTimes["Zombie Reaction Hit"].cooldown -= Time.deltaTime;
                if (animTimes["Zombie Reaction Hit"].cooldown <= 0)
                {
                    animTimes["Zombie Reaction Hit"].cooldown = animTimes["Zombie Reaction Hit"].duration;
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
    private void OnTriggerEnter(Collider other)
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


