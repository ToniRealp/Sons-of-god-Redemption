using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour {

    public Animator animator;
    public NavMeshAgent NavAgent;
    public Vector3 destinationPosition, randomPosition, playerPosition;
    public float xMin, xMax, zMin, zMax, viewDistance, hearDistance;

    public RaycastHit[] hit;
    public Ray[] ray;

    public Text healthText;
    public GameObject healthTextGO, canvas, textPos;
    public Font font;



    private void Start()
    {
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
    }


    void SetSearchingRange(Vector3 _initialPosition, float _movingRange)
    {
        xMin = _initialPosition.x - _movingRange;
        xMax = _initialPosition.x + _movingRange;
        zMin = _initialPosition.z - _movingRange;
        zMax = _initialPosition.z + _movingRange;
    }

    void SetRandomDestination()
    {
        randomPosition.x = Random.Range(xMin, xMax);
        randomPosition.y = 1;
        randomPosition.z = Random.Range(zMin, zMax);
    }

    void MoveTo(Vector3 _destination)
    {
        NavAgent.SetDestination(_destination);
    }

    void ChangeSpeed(float _speed)
    {
        NavAgent.speed = _speed;
    }

    void LookTo(Vector3 _target, float _rotationSpeed)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(_target.x - transform.position.x, _target.y - transform.position.y, _target.z - transform.position.z)), _rotationSpeed);
    }

    bool IsMoving()
    {
        return NavAgent.velocity.magnitude > 0.1f;
    }

    float DistanceTo(Vector3 _target)
    {
        return Vector3.Distance(gameObject.transform.position, _target);
    }

    void UpdateRaycasts()
    {
        // Raycast direction update
        ray[0] = new Ray(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), transform.forward);
        for (int i = 1; i < 37; i++)
        {
            ray[i] = new Ray(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), new Vector3(Mathf.Sin(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y + 5 * i)), 0, Mathf.Cos(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y + 5 * i))));
            ray[i + 36] = new Ray(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), new Vector3(Mathf.Sin(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y - 5 * i)), 0, Mathf.Cos(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y - 5 * i))));
        }
    }

    void DebugRaycasts()
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

    bool DetectPlayer()
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

    bool UseFullDetectionSystem()
    {
        UpdateRaycasts();
        DebugRaycasts();
        return DetectPlayer();
    }

    void UpdateHealthText(float _health)
    {
        // Health text update
        healthTextGO.GetComponent<Transform>().position = Camera.main.WorldToScreenPoint(textPos.transform.position);
        healthText.text = _health.ToString();
    }

    void Die()
    {
        Destroy(healthTextGO);
        Destroy(this.gameObject);
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
