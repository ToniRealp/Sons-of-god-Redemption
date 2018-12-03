﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : Enemy {

    new void Start () {
        base.Start();
	}

    override protected void Attack()
    {
        // No movement or rotation
        ChangeSpeed(0);
        // Save player last known position
        destination = playerPosition;
        MoveToDestination();

        // When attack time finishes
        animTimes["Attack"].cooldown -= Time.deltaTime;
        if (animTimes["Attack"].cooldown <= animTimes["Attack"].start)
            weapon.tag = "EnemyWeapon";
        if (animTimes["Attack"].cooldown <= animTimes["Attack"].end)
            weapon.tag = "Untagged";
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
    }
}


