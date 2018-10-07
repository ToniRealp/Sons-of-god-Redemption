using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    Rigidbody rb;
    InputManager inputManager;
    public int walkVelocity, runVelocity, dashDistance;
    public Vector3 direction;
    public float dashCounter, upTime, dashDuration, currentDashTime;
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
        dashCounter = upTime;
        currentDashTime = dashDuration;
    }
	
	// Update is called once per frame
	void FixedUpdate () {

        GetInput();

        if (yAxis == 0 && xAxis == 0)
        {
            states = States.Idle;
        }
        else if (Mathf.Abs(yAxis) < velChange && Mathf.Abs(xAxis) < velChange)
        {
            states = States.Walking;
        }
        else
        {
            if (dash)
                states = States.Dashing;
            else
                states = States.Running;
        }

        switch (states) {

            case(States.Idle):

                //play idle animation
                //check if we got input
                //Dash();
                animator.SetBool("isWalking", false);
                animator.SetBool("isRunning", false);

                break;

            case (States.Walking):

                //Play walking animation
                //If input > 0.7 switch states to running
                //If input==0 return to idle
                //Dash();
                Rotation();
                animator.SetBool("isWalking", true);
                animator.SetBool("isRunning", false);

                transform.Translate (transform.forward * walkVelocity*Time.deltaTime,Space.World);

                break;

            case (States.Running):

                //Play runnig animation
                //If input <0.7 switch state to walking
                //If input==0 return to idle
                //Dash();
                Rotation();
                animator.SetBool("isRunning", true);
                animator.SetBool("isWalking", false);

                transform.Translate(transform.forward * runVelocity * Time.deltaTime, Space.World);

                break;

            case (States.Dashing):

                Dash();
                currentDashTime -= Time.deltaTime;
                if (currentDashTime <= 0f) {
                    states = States.Running;
                    currentDashTime = dashDuration;
                }
                
                break;

            default:
                break;
            
        }
        

        
        //dash cooldown
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
        direction = new Vector3(xAxis,0,yAxis);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction,Vector3.up), 0.15F);
    }

    void Dash()
    {
        //Time.time;
        if (!dashed)
        {
            Vector3.Lerp(transform.position, transform.forward * dashDistance, 0.15f);
            dashed = true;

        }
        
    }

    void DashCooldown()
    {
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
