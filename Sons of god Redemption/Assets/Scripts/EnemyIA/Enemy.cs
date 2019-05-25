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
    public int health, movementSpeed, baseAttack;
    protected float movingRange, rotationSpeed, viewDistance, hearDistance, attackDistance, attackSpeed;

    //Animations
    protected Animator animator;
    protected Dictionary<string, AnimInfo> animTimes;

    public NavMeshAgent NavAgent;
    public Vector3 destination, playerPosition, initialPosition;
    protected float xMin, xMax, zMin, zMax;
    public bool playerDetected, damaged, attackOnCooldown, reactsToDamage, finishedDeathAnimation;
    protected float damagedCooldown, actualDamagedCooldown, moveCooldown, timeToMove=5f, alpha;
    public float attackCooldown;
    public RaycastHit[] hit;
    public Ray[] ray;

    protected Text healthText;
    protected GameObject canvas, textPos;
    public GameObject healthTextGO, weapon;
    public Font font;
    private GameObject player;

    public AudioManager audioManager;

    protected enum State { SEARCHING, CHASING, ATTAKING, DAMAGED, DEATH };
    [SerializeField] protected State state;

    protected string lastTag;

    //blood particles
    public GameObject blood;
    public Transform bloodPosition;
    public GameObject dieParticles;

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
        playerDetected = finishedDeathAnimation = false;
        initialPosition = transform.position;
        NavAgent = GetComponent<NavMeshAgent>();
        damaged = false;
        moveCooldown = timeToMove;
        actualDamagedCooldown = 0;
        damagedCooldown = 3f;
        attackCooldown = attackSpeed;
        audioManager = this.GetComponent<AudioManager>();
        //Animator
        animator = GetComponent<Animator>();
        animTimes = new Dictionary<string, AnimInfo>();
        GetAnimations();
        SetSearchingRange();
        SetRandomDestination();
        alpha = 1;
        player = GameObject.Find("Leliel");
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Untagged")
        {
            lastTag = other.tag;

            damaged = true;
            animator.SetTrigger("Damaged");
            animTimes["Reaction Hit"].cooldown = animTimes["Reaction Hit"].duration;


            if (other.tag == "Arrow")
            {
                health -= 15;
            }

            Instantiate(blood, bloodPosition.position, bloodPosition.rotation, transform);
            health -= (int)other.GetComponentInParent<PlayerController>().damage;
        }
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
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(destination.x - transform.position.x, transform.position.y, destination.z - transform.position.z)), rotationSpeed);
    }

    protected void LookToPlayer()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(playerPosition.x - transform.position.x, transform.position.y, playerPosition.z - transform.position.z)), rotationSpeed);
    }

    protected bool IsMoving()
    {
        return NavAgent.velocity.magnitude > 0.1f;
    }

    protected float DistanceToDestination(Vector3 _destination)
    {
        return Vector3.Distance(gameObject.transform.position, _destination);
    }

    protected void DetectPlayer()
    {
        playerPosition = player.transform.position;
    }

    protected void UseFullDetectionSystem()
    {
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
        Instantiate(dieParticles,bloodPosition.position,bloodPosition.rotation);
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

    protected void SetMaterialTransparent()
    {
        foreach (Material m in GetComponentInChildren<Renderer>().materials)
        {
            m.SetFloat("_Mode", 2);
            m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            m.SetInt("_ZWrite", 0);
            m.DisableKeyword("_ALPHATEST_ON");
            m.EnableKeyword("_ALPHABLEND_ON");
            m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            m.renderQueue = 3000;
        }
    }

    protected void FadeOut()
    {
        if (finishedDeathAnimation)
        {
            if (alpha >= 0)
            {
                alpha -= Time.fixedDeltaTime / 4;
                Debug.Log(alpha);
            }
            else
            {
                alpha = 0;
                Destroy(this.gameObject);
            }


            foreach (Material m in GetComponentInChildren<Renderer>().materials)
            {
                m.color = new Color(m.color.r, m.color.g, m.color.b, alpha);
            }
        }
    }

    protected float CurrentAnimLength()
    {
        return (animator.GetCurrentAnimatorStateInfo(0).length * animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
    }
    #endregion
}