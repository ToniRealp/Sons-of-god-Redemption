using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public abstract class Enemy : MonoBehaviour {

    protected class AnimInfo{
        public float duration, cooldown;
        public float start, end;
        public AnimInfo(float _duration, float _start = 0, float _end = 0) {
            duration = cooldown = _duration;
            start = _start;
            end = _end;
        }
    }

    //Stats
    public Stats stats;
    public int health, movementSpeed, baseAttack, attackSpeed;
    protected float movingRange, rotationSpeed, viewDistance, hearDistance, attackDistance;

    //Animations
    protected Animator animator;
    protected Dictionary<string, AnimInfo> animTimes;

    public NavMeshAgent NavAgent;
    public Vector3 destination, playerPosition, initialPosition;
    protected float xMin, xMax, zMin, zMax;
    public bool playerDetected, damaged, attackOnCooldown, reactsToDamage;
    protected float attackCooldown, actualAttackCooldown, damagedCooldown, actualDamagedCooldown, moveCooldown, timeToMove=5f;

    public RaycastHit[] hit;
    public Ray[] ray;

    protected Text healthText;
    protected GameObject canvas, textPos;
    public GameObject healthTextGO, weapon;
    public Font font;

    protected enum State { SEARCHING, CHASING, ATTAKING, DAMAGED };
    [SerializeField] protected State state;

    protected string lastTag;

    //blood particles
    public GameObject blood;
    public Transform bloodPosition;

    protected void Start()
    {
        //Stats
        health = stats.health;
        movementSpeed = stats.movementSpeed;
        baseAttack = stats.baseAttack;
        attackSpeed = stats.attackSpeed;
        movingRange = stats.movingRange;
        rotationSpeed = stats.rotationSpeed;
        viewDistance = stats.ViewDistance;
        hearDistance = stats.hearDistance;
        attackDistance = stats.attackDistance;
        //Raycasts
        hit = new RaycastHit[73];
        ray = new Ray[73];
        //Health Text
        canvas = GameObject.Find("Canvas");
        healthTextGO = new GameObject();
        healthTextGO.transform.SetParent(canvas.transform);
        healthText = healthTextGO.AddComponent<Text>();
        healthText.font = font;
        healthTextGO.name = "Enemy Health";
        healthText.alignment = TextAnchor.MiddleCenter;
        textPos = this.gameObject.transform.Find("HealthTextPos").gameObject;
        //Other
        playerDetected = false;
        initialPosition = transform.position;
        NavAgent = GetComponent<NavMeshAgent>();
        damaged = false;
        moveCooldown = timeToMove;
        actualDamagedCooldown = 0;
        damagedCooldown = 3f;
        //Animator
        animator = GetComponent<Animator>();
        animTimes = new Dictionary<string, AnimInfo>();
        GetAnimations();
        SetSearchingRange();
        SetRandomDestination();
    }

   


    #region BasicEnemyFunctionalities
    protected void SetSearchingRange()
    {
        xMin = initialPosition.x - movingRange;
        xMax = initialPosition.x + movingRange;
        zMin = initialPosition.z - movingRange;
        zMax = initialPosition.z + movingRange;
    }

    protected void SetRandomDestination()
    {
        destination.x = Random.Range(xMin, xMax);
        destination.y = 1;
        destination.z = Random.Range(zMin, zMax);
    }

    protected void MoveToDestination()
    {
        NavAgent.SetDestination(destination);
    }

    protected void ChangeSpeed(float _speed=0)
    {
        NavAgent.speed = _speed;
    }

    protected void LookToDestination()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(destination.x - transform.position.x, destination.y - transform.position.y, destination.z - transform.position.z)), rotationSpeed);
    }

    protected bool IsMoving()
    {
        return NavAgent.velocity.magnitude > 0.1f;
    }

    protected float DistanceToDestination()
    {
        return Vector3.Distance(gameObject.transform.position, destination);
    }

    protected void UpdateRaycasts()
    {
        // Raycast direction update
        ray[0] = new Ray(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), transform.forward);
        for (int i = 1; i < 37; i++)
        {
            ray[i] = new Ray(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), new Vector3(Mathf.Sin(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y + 5 * i)), 0, Mathf.Cos(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y + 5 * i))));
            ray[i + 36] = new Ray(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), new Vector3(Mathf.Sin(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y - 5 * i)), 0, Mathf.Cos(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y - 5 * i))));
        }
    }

    protected void DebugRaycasts()
    {
        // Debug Raycasting 
        // View Raycasts
        for (int i = 0; i < 5; i++)
        {
            Debug.DrawRay(ray[i].GetPoint(0), ray[i].direction * viewDistance, Color.red);
        }
        for (int i = 37; i < 41; i++)
        {
            Debug.DrawRay(ray[i].GetPoint(0), ray[i].direction * viewDistance, Color.red);
        }
        // Hear Raycasts
        for (int i = 5; i < 37; i++)
        {
            Debug.DrawRay(ray[i].GetPoint(0), ray[i].direction * hearDistance, Color.cyan);
        }
        for (int i = 41; i < 73; i++)
        {
            Debug.DrawRay(ray[i].GetPoint(0), ray[i].direction * hearDistance, Color.cyan);
        }
    }

    protected void DetectPlayer()
    {
        // Raycasting Logic
        // View Raycasts
        for (int i = 0; i < 5; i++)
        {
            if (Physics.Raycast(ray[i], out hit[i], viewDistance))
            {
                if (hit[i].collider.gameObject.tag == "Player")
                {
                    playerDetected = true;
                    playerPosition = hit[i].collider.gameObject.transform.position;
                }
            }
        }
        for (int i = 37; i < 41; i++)
        {
            if (Physics.Raycast(ray[i], out hit[i], viewDistance))
            {
                if (hit[i].collider.gameObject.tag == "Player")
                {
                    playerDetected = true;
                    playerPosition = hit[i].collider.gameObject.transform.position;
                }
            }
        }
        // Hear Raycasts
        for (int i = 5; i < 37; i++)
        {
            if (Physics.Raycast(ray[i], out hit[i], hearDistance))
            {
                if (hit[i].collider.gameObject.tag == "Player")
                {
                    playerDetected = true;
                    playerPosition = hit[i].collider.gameObject.transform.position;
                }
            }
        }
        for (int i = 41; i < 73; i++)
        {
            if (Physics.Raycast(ray[i], out hit[i], hearDistance))
            {
                if (hit[i].collider.gameObject.tag == "Player")
                {
                    playerDetected = true;
                    playerPosition = hit[i].collider.gameObject.transform.position;
                }
            }
        }
    }

    protected void UseFullDetectionSystem()
    {
        UpdateRaycasts();
        DebugRaycasts();
        DetectPlayer();
    }

    protected void UpdateHealthText()
    {
        // Health text update
        healthTextGO.GetComponent<Transform>().position = Camera.main.WorldToScreenPoint(textPos.transform.position);
        healthText.text = health.ToString();
    }

    protected void Die()
    {
            Destroy(healthTextGO);
            Destroy(this.gameObject);
    }

    protected void GetAnimations()
    {
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        foreach (var clip in clips) {
            animTimes[clip.name] = new AnimInfo(clip.length,clip.length*0.7f,clip.length*0.3f);
        }
    }

    protected float AnimationLength(string animName, Animator animator)
    {
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;

        for (int i = 0; i < clips.Length; i++)
        {
            if (clips[i].name == animName)
                return (clips[i].length);
        }
        return -1f;
    }

    protected float CurrentAnimLength()
    {
        return (animator.GetCurrentAnimatorStateInfo(0).length * animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
    }
    #endregion
}