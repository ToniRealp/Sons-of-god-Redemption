using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    Rigidbody rb;
    InputManager inputManager;
    public int velocity;
    Vector3 direction;
    public float xAxis;
    public float yAxis;
    enum States { Idle, Walking, Running, MAX};
    Animator animator;
    [SerializeField]
    States states;
    


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
                //rb.velocity = new Vector3(velocity,0,velocity); 
                Rotation();
                if (yAxis == 0 && xAxis == 0)
                {
                    states = States.Idle;
                    animator.SetBool("isWalking", false);
                }

                if (yAxis > 0.7 || yAxis < -0.7 || xAxis > 0.7 || xAxis < -0.7)
                {
                    states = States.Running;
                    animator.SetBool("isRunning", true);
                }

                break;

            case (States.Running):
                //Play runnig animation
                //If input >0.7 switch state to walking
                //rb.velocity = new Vector3(velocity + 2, 0, velocity+2);
                Rotation();
                if (yAxis < 0.7 || yAxis > -0.7 || xAxis < 0.7 || xAxis > -0.7)
                {
                    states = States.Walking;
                    animator.SetBool("isRunning", false);
                    animator.SetBool("isWalking", true);
                }

                if (yAxis == 0 && xAxis == 0)
                {
                    states = States.Idle;
                    animator.SetBool("isWalking", false);
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
        
    }
    void Rotation()
    {
        rb.velocity = new Vector3(xAxis * velocity, 0, velocity * yAxis);
        direction.Set(xAxis, 0, yAxis);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 0.15F);
    }
}
