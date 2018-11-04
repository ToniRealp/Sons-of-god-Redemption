﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

    public float yAxis;
    public float xAxis;
    public bool attackButton;
    public bool strongAttackButton;
    public bool dashButton;
    public float padXAxis;
    public float padYAxis;
    public bool interact;
    public bool addEnemy;
    public bool addBoss;
    public bool clearEnemies;
    public bool escape;

	void Update () {
        yAxis = Input.GetAxis("Vertical");
        xAxis = Input.GetAxis("Horizontal");

        if (Input.GetButtonDown("Attack"))
            attackButton = true;
        else
            attackButton = false;

        if (Input.GetButtonDown("StrongAttack"))
            strongAttackButton = true;
        else
            strongAttackButton = false;

        if (Input.GetButtonDown("Dash"))
            dashButton = true;
        else 
            dashButton = false;

        if (Input.GetButtonDown("Interact"))
            interact = true;
        else
            interact = false;

        if (Input.GetButtonDown("AddEnemy"))
            addEnemy = true;
        else
            addEnemy = false;

        if (Input.GetButtonDown("AddBoss"))
            addBoss = true;
        else
            addBoss = false;

        if (Input.GetButtonDown("ClearEnemies"))
            clearEnemies = true;
        else
            clearEnemies = false;

        if (Input.GetButtonDown("Cancel"))
            escape = true;
        else
            escape = false;

        padXAxis = Input.GetAxis("PadXaxis");
        padYAxis = Input.GetAxis("PadYaxis");
    }
}
