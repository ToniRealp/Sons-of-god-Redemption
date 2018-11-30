using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour {

    //Stats
    public Stats stats;
    protected int health, movementSpeed, baseAttack, attackSpeed;
    public float movingRange, rotationSpeed;

    //Animations
    protected Animator animator;
    protected Dictionary<string, float> animTimes;

    public NavMeshAgent NavAgent;
    protected Vector3 destination, randomPosition, playerPosition, target, initialPosition;
    protected float xMin, xMax, zMin, zMax, viewDistance, hearDistance;
    protected bool playerDetected;
    
    public RaycastHit[] hit;
    public Ray[] ray;

    protected Text healthText;
    protected GameObject healthTextGO, canvas, textPos, weapon;
    public Font font;

    enum State { SEARCHING, CHASING, ATTAKING, DAMAGED };
    [SerializeField] State state;

    protected void Start()
    {
        //Stats
        health = stats.health;
        movementSpeed = stats.movementSpeed;
        baseAttack = stats.baseAttack;
        attackSpeed = stats.attackSpeed;
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
        textPos = this.gameObject.transform.GetChild(3).gameObject;
        //Other
        playerDetected = false;
        initialPosition = transform.position;
        //Animator
        animator = GetComponent<Animator>();
        animTimes = new Dictionary<string, float>();
        GetAnimations();
    }

    #region ClassFunctionalities
    protected void SetSearchingRange()
    {
        xMin = initialPosition.x - movingRange;
        xMax = initialPosition.x + movingRange;
        zMin = initialPosition.z - movingRange;
        zMax = initialPosition.z + movingRange;
    }

    protected void SetRandomDestination()
    {
        randomPosition.x = Random.Range(xMin, xMax);
        randomPosition.y = 1;
        randomPosition.z = Random.Range(zMin, zMax);
    }

    protected void MoveToDestination()
    {
        NavAgent.SetDestination(destination);
    }

    protected void ChangeSpeed(float _speed)
    {
        NavAgent.speed = _speed;
    }

    protected void LookToTarget()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(target.x - transform.position.x, target.y - transform.position.y, target.z - transform.position.z)), rotationSpeed);
    }

    protected bool IsMoving()
    {
        return NavAgent.velocity.magnitude > 0.1f;
    }

    protected float DistanceToTarget()
    {
        return Vector3.Distance(gameObject.transform.position, target);
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

    protected bool DetectPlayer()
    {
        // Raycasting Logic
        bool playerDetected = false;
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
        return playerDetected;
    }

    protected bool UseFullDetectionSystem()
    {
        UpdateRaycasts();
        DebugRaycasts();
        return DetectPlayer();
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
            animTimes[clip.name] = clip.length;
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
    #endregion
}