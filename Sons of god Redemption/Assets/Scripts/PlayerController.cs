using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    Rigidbody rb;
    InputManager inputManager;
    public int walkVelocity, runVelocity, force;
    Vector3 direction;
    public float xAxis, yAxis;
    public bool dash;
    enum States { Idle, Walking, Running, MAX};
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
    }
	
	// Update is called once per frame
	void Update () {

        GetInput();
        
        //angle = Vector3.Angle(new Vector3(0, 0, 1), new Vector3(xAxis, 0, yAxis));
        

        switch (states) {

            case(States.Idle):

                //play idle animation
                //check if we got input

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
            
                Movement();

                if (dash)
                    Dash();

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

                break;

            case (States.Running):

                //Play runnig animation
                //If input >0.7 switch state to walking
             
                Movement();

                if (yAxis < velChange && yAxis > -velChange && xAxis < velChange && xAxis > -velChange)
                {
                    states = States.Walking;
                    animator.SetBool("isRunning", false);
                    animator.SetBool("isWalking", true);
                }

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

    void Movement()
    {
        direction.Set(xAxis, 0, yAxis);
        rb.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 0.15F);
        if (yAxis !=0 || xAxis != 0)
            rb.velocity = transform.forward * walkVelocity;
        if (yAxis > velChange || yAxis < -velChange || xAxis > velChange || xAxis < -velChange)
            rb.velocity = transform.forward * runVelocity;
    }

    void Dash()
    {
        rb.velocity += transform.forward * force;

    }
}
