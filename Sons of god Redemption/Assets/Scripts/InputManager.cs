using System.Collections;
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

        padXAxis = Input.GetAxis("PadXaxis");
        padYAxis = Input.GetAxis("PadYaxis");
    }
}
