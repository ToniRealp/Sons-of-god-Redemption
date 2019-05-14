using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RangedEnemy : Enemy {

    public GameObject arrow;
    public Transform spawnPos;
    public float bulletSpeed;
    private float optimalPos, optimalPosOffset;
    public float timeToRun, runCooldown;
    private bool shoot, damagedSound;

    new private void Start()
    {
        base.Start();
        shoot = true;
        damagedSound = false;
        optimalPosOffset = 2f;
        optimalPos = attackDistance - optimalPosOffset;
        runCooldown = timeToRun = 4;
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
                if (runCooldown > 0)
                    runCooldown -= Time.deltaTime;


                ChangeSpeed(movementSpeed);
                if (runCooldown <= 0)
                {
                    runCooldown = timeToRun;
                    // Go to player Position
                    if (DistanceToDestination(playerPosition) - optimalPos >= optimalPosOffset)
                    {
                        OptimalDestination(false);
                    }
                    else if (DistanceToDestination(playerPosition) - optimalPos < optimalPosOffset)
                    {
                        OptimalDestination(true);
                    }
                }

                MoveToDestination();

                if (playerDetected)
                {
                    // If in attack conditions, go to attack
                    if (DistanceToDestination(destination) <= attackDistance && !attackOnCooldown)
                    {
                        state = State.ATTAKING;
                        animator.SetTrigger("Shoot");
                        audioManager.Play("SkeletonShoot");
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
                LookToPlayer();
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

                if (!damagedSound)
                {
                    damagedSound = true;
                    audioManager.Play("SkeletonHit");
                }
                // Cancel Attack animation if getting hit
                animator.SetBool("Shoot", false);
                animTimes["Shoot"].cooldown = animTimes["Shoot"].duration;
                // No movement
                ChangeSpeed(0);
                // But rotation
                LookToPlayer();

                // When damage animation finishes change state
                animTimes["Reaction Hit"].cooldown -= Time.deltaTime;
                if (animTimes["Reaction Hit"].cooldown <= 0)
                {
                    animTimes["Reaction Hit"].cooldown = animTimes["Reaction Hit"].duration;
                    damaged = damagedSound = false;
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

    void Shoot()
    {
        // Create the Bullet from the Bullet Prefab
        var bullet = (GameObject)Instantiate(arrow, spawnPos.position, spawnPos.rotation);
        //bullet.transform.parent = this.gameObject.transform;
        // Add velocity to the bullet
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * bulletSpeed;
        // Destroy the bullet after 2 seconds
        Destroy(bullet, 2.0f);
    }

    void OptimalDestination(bool isCloser)
    {

        SetRandomDestination();


        //float angle = Vector3.SignedAngle(Vector3.forward,transform.position - playerPosition,Vector3.up); //Random.Range(0, 360)
        //float distance;

        //if (isCloser)
        //    distance = optimalPos + Random.Range(0, optimalPosOffset);
        //else
        //    distance = optimalPos - Random.Range(0, optimalPosOffset);

        //bool angleFound = false;
        //float angleOffset=0;
        //do
        //{
        //    Vector3 position = new Vector3(Mathf.Sin(Mathf.Deg2Rad * (angle)), 0, Mathf.Cos(Mathf.Deg2Rad * (angle))) * distance;
        //    destination = playerPosition + position;
        //    NavMeshHit hit;
        //    if (NavMesh.Raycast(gameObject.transform.position, destination, out hit, NavAgent.areaMask))
        //    {
        //        if (!angleFound)
        //        {
        //            if (Mathf.Abs(((new Vector3(Mathf.Sin(Mathf.Deg2Rad * (angle + 30)), 0, Mathf.Cos(Mathf.Deg2Rad * angle + 30)) * distance) - playerPosition).magnitude) >
        //           Mathf.Abs(((new Vector3(Mathf.Sin(Mathf.Deg2Rad * (angle - 30)), 0, Mathf.Cos(Mathf.Deg2Rad * angle - 30)) * distance) - playerPosition).magnitude))
        //            {
        //                angleOffset = +30;
        //            }
        //            else
        //            {
        //                angleOffset = -30;
        //            }
        //            angleFound = true;
        //        }
        //        angle += angleOffset;
        //    }
        //    else
        //    {
        //        angleFound = false;
        //        break;
        //    }

        //} while (true);
    }
}