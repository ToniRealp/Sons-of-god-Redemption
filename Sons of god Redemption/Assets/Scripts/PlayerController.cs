using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    Rigidbody rb;
    InputManager inputManager;
    public int walkVelocity, runVelocity, dashDistance;
    public Vector3 direction;
    public float dashCooldownCounter,dashCooldownTime, dashDuration, dashTimeFraction, actualDashTime;
    private float xAxis, yAxis;
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
        dashCooldownCounter = dashCooldownTime;
        actualDashTime = dashDuration;
    }
	
	// Update is called once per frame
	void FixedUpdate () {

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
                Rotation();           
                transform.Translate (transform.forward * walkVelocity*Time.deltaTime,Space.World);

                if (Mathf.Abs(xAxis) > velChange || Mathf.Abs(yAxis) > velChange)
                {
                    states = States.Running;
                    animator.SetBool("isRunning", true);
                    //animator.SetBool("isWalking", false);
                }
                if (yAxis == 0 && xAxis == 0)
                {
                    states = States.Idle;
                    animator.SetBool("isWalking", false);
                }


                break;

            case (States.Running):

                //Play runnig animation
                //If input <0.7 switch state to walking
                //If input==0 return to idle
                //Dash();
                Rotation();                       
                transform.Translate(transform.forward * runVelocity * Time.deltaTime, Space.World);

                if (Mathf.Abs(yAxis) < velChange && Mathf.Abs(xAxis) < velChange)
                {
                    states = States.Walking;
                    animator.SetBool("isWalking", true);
                    animator.SetBool("isRunning", false);
                }
                if (dash)
                    states = States.Dashing;

                break;

            case (States.Dashing):

                actualDashTime -= Time.fixedDeltaTime;
                if (actualDashTime >= 0)
                {
                    Dash();
                }
                else
                {
                    dashed = true;
                    states = States.Running;
                    actualDashTime = dashDuration;
                }
               
                 
                break;

            default:
                break;
            
        }
        
        DashCooldown(); 
    }

    void GetInput()
    {
       xAxis = inputManager.xAxis;
       yAxis = inputManager.yAxis;
       dash = inputManager.dashButton;
        
    }

    void Rotation()
    {
        direction.Set(xAxis,0,yAxis);
        if(direction.magnitude!=0)
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction,Vector3.up), 0.15F);
    }

    void Dash()
    {

        //Vector3.Lerp(transform.position, transform.position + transform.forward * dashDistance, actualDashTime);
        //actualDashTime += dashTimeFraction;
        if (!dashed) {
            transform.position += transform.forward * Time.fixedDeltaTime * (dashDistance/dashDuration);
        }
    }

    void DashCooldown()
    {
        if (dashed)
        {
            dashCooldownCounter -= Time.deltaTime;

            if (dashCooldownCounter <= 0f)
            {
                dashed = false;
                dashCooldownCounter = dashCooldownTime;

            }
        }
    }
}
