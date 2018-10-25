using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    enum States { Idle, Walking, Running, Dashing, Attacking, MAX };
    enum Attacks { LightAttack1, LightAttack2, LightAttack3, StrongAttack1, StrongAttack2, StrongAttack3, NotAtt };
    enum ButtonInputs { Dash, LightAttack, StrongAttack, MAX };

    InputManager inputManager;
    Animator animator;
    GameObject weapon;

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
        inputManager = GetComponent<InputManager>();
        animator = GetComponent<Animator>();
        states = States.Idle;
        attacks = Attacks.NotAtt;
        dashed = attacked = transition = false;
        dashCooldownCounter = dashCooldownTime;
        actualDashTime = dashDuration;
        weapon = GameObject.Find("RightHandThumb1");
    }
	
	// Update is called once per frame
	void Update () {


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
                if (inputs[(int)ButtonInputs.StrongAttack])
                {
                    states = States.Attacking;
                    attacks = Attacks.StrongAttack1;
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
                if (inputs[(int)ButtonInputs.StrongAttack])
                {
                    states = States.Attacking;
                    attacks = Attacks.StrongAttack1;
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
                if (inputs[(int)ButtonInputs.StrongAttack])
                {
                    states = States.Attacking;
                    attacks = Attacks.StrongAttack1;
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
                
                actualDashTime -= Time.deltaTime;
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
                            animLength = animDuration = AnimationLength("light attack", animator);
                            attacked = true;
                        }
                        if (inputs[(int)ButtonInputs.LightAttack])
                        {
                            transition = true;
                        }
                        if (transition && animLength < animDuration * 0.3)
                        {
                            attacks = Attacks.LightAttack2;
                            attacked = false;
                            transition = false;
                        }
                        else if (animLength <= 0)
                        {
                            attacks =Attacks.NotAtt;
                            states = lastState;
                            attacked = false;
                        }

                        if (animLength < animDuration * 0.3) 
                            weapon.tag = "Untagged";
                        else if (animLength < animDuration * 0.8)
                            weapon.tag = "Weapon";
                                              
                        animLength -= Time.deltaTime;

                        break;

                    case (Attacks.LightAttack2):

                        if (!attacked)
                        {
                            animator.SetTrigger("lightAttack2");
                            animLength = animDuration = AnimationLength("light attack 2", animator);
                            attacked = true;
                            weapon.tag = "Weapon";
                        }
                        if (inputs[(int)ButtonInputs.LightAttack])
                        {
                            transition = true;
                        }
                        if(transition && animLength < animDuration*0.45)
                        {
                            attacks = Attacks.LightAttack3;
                            attacked = false;
                            transition = false;
                        }
                        else if (animLength <= 0)
                        {
                            attacks = Attacks.NotAtt;
                            states = lastState;
                            attacked = false;
                        }
                        if (animLength < animDuration * 0.3)
                            weapon.tag = "Untagged";
                        animLength -= Time.deltaTime;

                        break;
                    case (Attacks.LightAttack3):

                        if (!attacked)
                        {
                            animator.SetTrigger("lightAttack3");
                            animLength = AnimationLength("light attack 3", animator);
                            attacked = true;
                            weapon.tag = "Weapon";
                        }
                        animLength -= Time.deltaTime;
                        if (animLength <= 0)
                        {
                            attacks = Attacks.NotAtt;
                            states = lastState;
                            attacked = false;
                        }
                        if (animLength < animDuration * 0.5)
                            weapon.tag = "Untagged";

                        break;
                    case (Attacks.StrongAttack1):

                        if (!attacked)
                        {
                            animator.SetTrigger("strongAttack1");
                            animLength = animDuration = AnimationLength("strong attack", animator);
                            attacked = true;
                            weapon.tag = "Weapon";
                        }
                        if (inputs[(int)ButtonInputs.StrongAttack])
                        {
                            transition = true;
                        }
                        if (transition && animLength < animDuration*0.2)
                        {
                            attacks = Attacks.StrongAttack2;
                            attacked = false;
                            transition = false;
                        }
                        else if (animLength <= 0)
                        {                       
                            attacks = Attacks.NotAtt;
                            states = lastState;
                            attacked = false;
                        }
                        if (animLength < animDuration * 0.3)
                            weapon.tag = "Untagged";
                        animLength -= Time.deltaTime;

                        break;

                    case (Attacks.StrongAttack2):

                        if (!attacked)
                        {
                            animator.SetTrigger("strongAttack2");
                            animLength = AnimationLength("strong attack 2", animator);
                            attacked = true;
                            weapon.tag = "Weapon";
                        }                    
                        if (animLength <= 0)
                        {
                            attacks = Attacks.NotAtt;
                            states = lastState;
                            attacked = false;
                        }

                        if (animLength < animDuration * 0.3)
                            weapon.tag = "Untagged";

                        animLength -= Time.deltaTime;

                        break;

                    default:

                        break;

                }


                break;

            default:
                break;
            
        }
        
        DashCooldown();
    //    if (attacked && animLength<animDuration-0.2)
    //    {
    //        weapon.tag = "Weapon";
    //    }
    //    else
    //    {
    //        weapon.tag = "Untagged";
    //    }
    }

    void GetInput()
    {
       xAxis = inputManager.xAxis;
       yAxis = inputManager.yAxis;
       inputs[(int)ButtonInputs.Dash] = inputManager.dashButton;
       inputs[(int)ButtonInputs.LightAttack] = inputManager.attackButton;
       inputs[(int)ButtonInputs.StrongAttack] = inputManager.strongAttackButton;
        
    }

    void Rotation()
    {
        direction.Set(xAxis,0,yAxis);
        if(direction.magnitude!=0)
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction,Vector3.up), 0.15F);
    }

    void Dash()
    {
            transform.position += transform.forward * Time.deltaTime * (dashDistance/dashDuration);
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
