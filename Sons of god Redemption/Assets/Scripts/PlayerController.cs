using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    Rigidbody rb;
    InputManager inputManager;
    public int walkVelocity, runVelocity, force;
    Vector3 direction;
    public float xAxis, yAxis, dashCounter, upTime;
    public bool dash, dashed;
    enum States { Idle, Walking, Running, Dashing, MAX};
    Animator animator;
    [SerializeField]
    States states;
    const float velChange = 0.5f;
    


    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        inputManager = GetComponent<InputManager>();
        animator = GetComponent<Animator>();
        states = States.Idle;
        dashed = false;
        dashCounter = upTime;
    }
	
	// Update is called once per frame
	void Update () {

        GetInput();

        switch (states) {

            case(States.Idle):

                //play idle animation
                //check if we got input
                //Dash();

                if (yAxis != 0 || xAxis != 0)
                {
                   
                        states = States.Walking;
                        animator.SetBool("isWalking", true);
                    
                }
                    break;

            case (States.Walking):

                //Play walking animation
                //If input > 0.7 switch states to running
                //If input==0 return to idle
                //Dash();

                if (dash)
                    states = States.Dashing;

                if (yAxis == 0 && xAxis == 0)
                {
                    states = States.Idle;
                    animator.SetBool("isWalking", false);
                }

                if (yAxis > velChange || yAxis < -velChange || xAxis > velChange || xAxis < -velChange)
                {
                    states = States.Running;
                    animator.SetBool("isRunning", true);
                }

                Rotation();
                rb.velocity = transform.forward * walkVelocity;

                break;

            case (States.Running):

                //Play runnig animation
                //If input >0.7 switch state to walking
                //Dash();

                if (dash)
                    states = States.Dashing;

                if (yAxis < velChange && yAxis > -velChange && xAxis < velChange && xAxis > -velChange)
                {
                    states = States.Walking;
                    animator.SetBool("isRunning", false);
                    animator.SetBool("isWalking", true);
                }

                Rotation();
                rb.velocity = transform.forward * runVelocity;

                break;

            case (States.Dashing):

                Dash();
                

                break;

            default:
                break;
            
        }
    }

    void GetInput()
    {
       xAxis = inputManager.xAxis;
       yAxis = inputManager.yAxis;
       dash = inputManager.dashButton;
        
    }

    void Rotation()
    {
        direction.Set(xAxis, 0, yAxis);
        rb.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 0.15F);
    }

    void Dash()
    {
        //Time.time;
        if (!dashed)
        {
            rb.velocity += transform.forward * force;
            dashed = true;
        }
        

        if (dashed)
        {
            dashCounter -= Time.deltaTime;
   
            if (dashCounter <= 0f)
            {
                dashed = false;
                dashCounter = upTime;

            }
        }
    }
}
