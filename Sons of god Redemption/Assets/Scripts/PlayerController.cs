﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    enum States { Idle, Walking, Running, Dashing, Attacking, MAX };
    enum Attacks { LightAttack1, LightAttack2, LightAttack3, NotAtt };
    enum ButtonInputs { Dash, LightAttack, MAX };

    Rigidbody rb;
    InputManager inputManager;
    Animator animator;

    bool[] inputs = new bool[(int)ButtonInputs.MAX];
    private float xAxis, yAxis;

    public Vector3 direction;
    public int walkVelocity, runVelocity, dashDistance;    
    public float dashCooldownCounter,dashCooldownTime, dashDuration, actualDashTime, animLength, animDuration;
    public bool dashed, attacked, transition;
    const float velChange = 0.5f;
  
    [SerializeField] States states, lastState;
    [SerializeField] Attacks attacks;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        inputManager = GetComponent<InputManager>();
        animator = GetComponent<Animator>();
        states = States.Idle;
        attacks = Attacks.NotAtt;
        dashed = attacked = transition = false;
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
                if (yAxis != 0 || xAxis != 0)
                {
                    states = States.Walking;
                    animator.SetBool("isWalking", true);
                }
                if (inputs[(int)ButtonInputs.LightAttack])
                {
                    states = States.Attacking;
                    attacks = Attacks.LightAttack1;
                    lastState = States.Idle;
                }

                break;

            case (States.Walking):

                //Play walking animation
                //If input > 0.7 switch states to running
                //If input==0 return to idle
                Rotation();           
                transform.Translate (transform.forward * walkVelocity*Time.deltaTime,Space.World);

                if (inputs[(int)ButtonInputs.LightAttack])
                {
                    states = States.Attacking;
                    attacks = Attacks.LightAttack1;
                    lastState = States.Walking;
                }
                else if (inputs[(int)ButtonInputs.Dash] && !dashed)
                {
                    states = States.Dashing;
                    animator.SetTrigger("isDashing");
                }
                else if (Mathf.Abs(xAxis) > velChange || Mathf.Abs(yAxis) > velChange)
                {
                    states = States.Running;
                    animator.SetBool("isRunning", true);
                    //animator.SetBool("isWalking", false);
                }
                else if (yAxis == 0 && xAxis == 0)
                {
                    states = States.Idle;
                    animator.SetBool("isWalking", false);
                }
               

                break;

            case (States.Running):

                //Play runnig animation
                //If input <0.7 switch state to walking
                //If input==0 return to idle
                Rotation();                       
                transform.Translate(transform.forward * runVelocity * Time.deltaTime, Space.World);

                if (inputs[(int)ButtonInputs.LightAttack])
                {
                    states = States.Attacking;
                    attacks = Attacks.LightAttack1;
                    lastState = States.Running;
                }
                else if(inputs[(int)ButtonInputs.Dash] && !dashed)
                {
                    states = States.Dashing;
                    animator.SetTrigger("isDashing");
                }
                else if (Mathf.Abs(yAxis) < velChange && Mathf.Abs(xAxis) < velChange)
                {
                    states = States.Walking;
                    animator.SetBool("isWalking", true);
                    animator.SetBool("isRunning", false);
                }
                
               
                break;

            case (States.Dashing):
                
                actualDashTime -= Time.fixedDeltaTime;
                if (actualDashTime >= 0)
                {
                    if (!dashed)
                        Dash();
                }
                else
                {
                    dashed = true;
                    states = States.Running;
                    actualDashTime = dashDuration;
                }
                 
                break;
            case (States.Attacking):

                switch (attacks) {

                    case (Attacks.LightAttack1):
                        
                        if (!attacked)
                        {
                            animator.SetTrigger("lightAttack1");
                            animLength = AnimationLength("light attack", animator);
                            attacked = true;
                        }
                        if (inputs[(int)ButtonInputs.LightAttack])
                        {
                            attacks = Attacks.LightAttack2;
                            attacked = false;
                        }
                        animLength -= Time.fixedDeltaTime;
                        if (animLength <= 0)
                        {
                            //animator.SetBool("isWalking", false);
                            //animator.SetBool("isRunning", false);
                            attacks =Attacks.NotAtt;
                            states = lastState;
                            attacked = false;
                        }

                        break;

                    case (Attacks.LightAttack2):

                        if (!attacked)
                        {
                            animator.SetTrigger("lightAttack2");
                            animLength = animDuration = AnimationLength("light attack 2", animator);
                            attacked = true;
                        }
                        if (inputs[(int)ButtonInputs.LightAttack])
                        {
                            transition = true;
                        }

                        if(transition && animDuration < 0.8)
                        {
                            attacks = Attacks.LightAttack3;
                            attacked = false;
                            transition = false;
                        }

                        animLength -= Time.fixedDeltaTime;
                        animDuration -= Time.fixedDeltaTime;

                        if (animLength <= 0)
                        {
                            //animator.SetBool("isWalking", false);
                            //animator.SetBool("isRunning", false);
                            attacks = Attacks.NotAtt;
                            states = lastState;
                            attacked = false;
                        }

                        break;
                    case (Attacks.LightAttack3):

                        if (!attacked)
                        {
                            animator.SetTrigger("lightAttack3");
                            animLength = AnimationLength("light attack 3", animator);
                            attacked = true;
                        }
                        animLength -= Time.fixedDeltaTime;
                        if (animLength <= 0)
                        {
                            //animator.SetBool("isWalking", false);
                            //animator.SetBool("isRunning", false);
                            attacks = Attacks.NotAtt;
                            states = lastState;
                            attacked = false;
                        }

                        break;

                    default:

                        break;

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
       inputs[(int)ButtonInputs.Dash] = inputManager.dashButton;
       inputs[(int)ButtonInputs.LightAttack] = inputManager.attackButton;
        
    }

    void Rotation()
    {
        direction.Set(xAxis,0,yAxis);
        if(direction.magnitude!=0)
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction,Vector3.up), 0.15F);
    }

    void Dash()
    {
            transform.position += transform.forward * Time.fixedDeltaTime * (dashDistance/dashDuration);
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

    float AnimationLength(string animName, Animator animator)
    {
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;

        for (int i = 0; i < clips.Length; i++)
        {
            if (clips[i].name == animName)
                return (clips[i].length);
        }
        return -1f;
    }
}
