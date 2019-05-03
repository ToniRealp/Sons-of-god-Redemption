using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UselessEnemy : Enemy {


    new void Start()
    {
        base.Start();

    }

    private void Update()
    {
        if (health <= 0)
            Die();

        UpdateHealthText();

        if (damaged)
        {
            state = State.DAMAGED;
        }


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


                break;

            case State.CHASING:

                break;

            case State.ATTAKING:

                break;

            case State.DAMAGED:
                // Cancel Attack animation if getting hit
                weapon.tag = "Untagged";
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

}