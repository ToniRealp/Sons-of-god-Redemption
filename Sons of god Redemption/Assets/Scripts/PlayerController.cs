using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    //Player stats
    public Stats stats;
    public int health, movementSpeed, baseAttack, attackSpeed;

    //Enums
    enum States { Idle, Walking, Running, Dashing, Attacking, MAX };
    enum Attacks { LightAttack1, LightAttack2, LightAttack3, StrongAttack1, StrongAttack2, StrongAttack3, NotAtt };
    enum ButtonInputs { Dash, LightAttack, StrongAttack, padLeft, padRight, Interact, MAX };
    enum Elements {Holy, Fire, MAX };

    //External attributes
    InputManager inputManager;
    Animator animator;
    [SerializeField] GameObject weapon;
    [SerializeField] GameObject[] elements = new GameObject[(int)Elements.MAX];

    //Inputs
    bool[] inputs = new bool[(int)ButtonInputs.MAX];
    private float xAxis, yAxis;

    //General atributes
    public Vector3 direction;
    public int walkVelocity, runVelocity, dashDistance;    
    public float dashCooldownTime, dashDuration, onHitAnimDelay, damage;
    private float dashCooldownCounter, actualDashTime, animLength, animDuration, onHitDelay;
    private bool dashed, attacked, transition, hit;
    public bool interact;
    const float velChange = 0.5f;
    
    //State machine
    [SerializeField] States states, nextState;
    [SerializeField] Attacks attacks;

    void Start () {

        //initialize player stats
        health = stats.health;
        movementSpeed = stats.movementSpeed;
        baseAttack = stats.baseAttack;
        attackSpeed = stats.attackSpeed;

        //initialize player atributes(not stats)
        inputManager = GetComponent<InputManager>();
        animator = GetComponent<Animator>();
        states = States.Idle;
        attacks = Attacks.NotAtt;
        dashed = attacked = transition = hit = false;
        dashCooldownCounter = dashCooldownTime;
        actualDashTime = dashDuration;
        onHitDelay = onHitAnimDelay;
        weapon = GameObject.Find("Sword");
        elements[(int)Elements.Fire] = GameObject.Find("Fire particles");
        elements[(int)Elements.Holy] = GameObject.Find("Light particles");
        elements[(int)Elements.Fire].SetActive(false);
        elements[(int)Elements.Holy].SetActive(false);

    }
	
	void Update () {

        GetInput();

        //Element controller
        if (inputs[(int)ButtonInputs.padRight])
        {
            elements[(int)Elements.Fire].SetActive(true);
            elements[(int)Elements.Holy].SetActive(false);
        }
        if (inputs[(int)ButtonInputs.padLeft])
        {
            elements[(int)Elements.Fire].SetActive(false);
            elements[(int)Elements.Holy].SetActive(true);
        }

        //Player state machine
        switch (states) {

            case(States.Idle):

                //play idle animation
                //check if we got input
                //if attack input switch to attack state
                //if dash input switch to dash state
                if (yAxis != 0 || xAxis != 0)
                {
                    states = States.Walking;
                    animator.SetBool("isWalking", true);
                }
                if (inputs[(int)ButtonInputs.LightAttack])
                {
                    states = States.Attacking;
                    attacks = Attacks.LightAttack1;
                }
                if (inputs[(int)ButtonInputs.StrongAttack])
                {
                    states = States.Attacking;
                    attacks = Attacks.StrongAttack1;
                }

                break;

            case (States.Walking):

                //Play walking animation
                //If input > 0.7 switch states to running
                //If input==0 return to idle
                //if attack input switch to attack state
                //if dash input switch to dash state
                Rotation();           
                transform.Translate (transform.forward * walkVelocity*Time.deltaTime,Space.World);

                if (inputs[(int)ButtonInputs.LightAttack])
                {
                    states = States.Attacking;
                    attacks = Attacks.LightAttack1;
                }
                if (inputs[(int)ButtonInputs.StrongAttack])
                {
                    states = States.Attacking;
                    attacks = Attacks.StrongAttack1;
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
                //if attack input switch to attack state
                //if dash input switch to dash state
                Rotation();                       
                transform.Translate(transform.forward * runVelocity * Time.deltaTime, Space.World);

                if (inputs[(int)ButtonInputs.LightAttack])
                {
                    states = States.Attacking;
                    attacks = Attacks.LightAttack1;
                }
                if (inputs[(int)ButtonInputs.StrongAttack])
                {
                    states = States.Attacking;
                    attacks = Attacks.StrongAttack1;
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
                        damage = baseAttack;
                        
                        if (!attacked)
                        {
                            animator.SetTrigger("lightAttack1");
                            animLength = animDuration = AnimationLength("light attack", animator);
                            attacked = true;
                            weapon.tag = "Weapon";
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
                            states = nextState;
                            attacked = false;
                            weapon.tag = "Untagged";
                        }
                     
                            

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
                        if (transition && animLength < animDuration*0.45)
                        {
                            attacks = Attacks.LightAttack3;
                            attacked = false;
                            transition = false;
                        }
                        else if (animLength <= 0)
                        {
                            attacks = Attacks.NotAtt;
                            states = nextState;
                            attacked = false;
                            weapon.tag = "Untagged";
                        }     

                        break;
                    case (Attacks.LightAttack3):

                        if (!attacked)
                        {
                            animator.SetTrigger("lightAttack3");
                            animLength = AnimationLength("light attack 3", animator);
                            attacked = true;
                            weapon.tag = "Weapon";
                        }
                        if (animLength <= 0)
                        {
                            attacks = Attacks.NotAtt;
                            states = nextState;
                            attacked = false;
                            weapon.tag = "Untagged";
                        }

                        break;
                    case (Attacks.StrongAttack1):
                        damage = baseAttack + (baseAttack * 0.5f);
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
                            states = nextState;
                            attacked = false;
                            weapon.tag = "Untagged";
                        }

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
                            states = nextState;
                            attacked = false;
                            weapon.tag = "Untagged";
                        }

                        break;

                    default:

                        break;

                }
                animLength -= Time.deltaTime;
                nextState = CheckState();
                break;

            default:
                break;
            
        }
        
        DashCooldown();

        if (hit)
        {
            onHitDelay -= Time.deltaTime;
            if (onHitDelay <= 0)
            {
                animator.speed = 1f;
                onHitDelay = onHitAnimDelay;
                hit = false;
            }
        }
    }

    void GetInput()
    {
       xAxis = inputManager.xAxis;
       yAxis = inputManager.yAxis;
       inputs[(int)ButtonInputs.Dash] = inputManager.dashButton;
       inputs[(int)ButtonInputs.LightAttack] = inputManager.attackButton;
       inputs[(int)ButtonInputs.StrongAttack] = inputManager.strongAttackButton;
       interact = inputs[(int)ButtonInputs.Interact] = inputManager.interact;
        
       if(inputManager.padXAxis==1)
            inputs[(int)ButtonInputs.padRight] = true;
       else
            inputs[(int)ButtonInputs.padRight] = false;

        if (inputManager.padXAxis == -1)
            inputs[(int)ButtonInputs.padLeft] = true;
        else
            inputs[(int)ButtonInputs.padLeft] = false;

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

    States CheckState()
    {
        if (xAxis > 0.5 || yAxis > 0.5)
        {
            animator.SetBool("isWalking", true);
            animator.SetBool("isRunning", true);
            return (States.Running);
        }
        else if (xAxis == 0 || yAxis == 0)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
            return (States.Idle);
        }
        else
        {
            animator.SetBool("isWalking", true);
            animator.SetBool("isRunning", false);
            return (States.Walking);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            if (weapon.tag == "Weapon")
            {
                animator.speed = 0f;
                hit = true;
                weapon.tag = "Untagged";
            }        
        }
    }
}
