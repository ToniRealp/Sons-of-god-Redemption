using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpBehaviour : MonoBehaviour {

    public bool picked;
    public float ascensionHeight=1;
    public float ascendingSpeed = 0.01f;
    public float rotationSpeed = 1;
    public float movingSpeed = 0.01f;
    public float scalingSpeed = 0.005f;

    private Vector3 initPosition;
    private GameObject player;
    private bool ascending;

	// Use this for initialization
	void Start () {
        picked = false;
        ascending = false;
        initPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        if (picked)
        {
            if (ascending)
            {
                if (transform.position.y<initPosition.y+ascensionHeight)
                {
                    transform.position += new Vector3(0, ascendingSpeed, 0);
                    transform.Rotate(0, rotationSpeed, 0);
                }
                else
                {
                    ascending = false;
                }
            }
            else
            {
                transform.Rotate(0, rotationSpeed, 0);
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(player.transform.position.x, player.transform.position.y+1, player.transform.position.z), movingSpeed);
                if (transform.localScale.x > 0.01f && transform.localScale.y > 0.01f && transform.localScale.z > 0.01f)
                {
                    transform.localScale -= new Vector3(scalingSpeed, scalingSpeed, scalingSpeed);
                }
                else
                {
                    GameObject.Find("Leliel").GetComponent<PlayerController>().baseAttack += 2;
                    GameObject.Find("Leliel").GetComponent<PlayerController>().stats.health += 10;
                    GameObject.Find("Leliel").GetComponent<PlayerController>().health += 10;
                    GameObject.Find("Leliel").GetComponent<PlayerController>().healthBar.value = GameObject.Find("Leliel 1").GetComponent<PlayerController>().health;

                    Destroy(this.gameObject);
                }


            }
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag=="Player")
        {
            player = other.gameObject;
            //Show pick text
        }
    
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (other.gameObject.GetComponent<PlayerController>().interact)
            {
                ascending = true;
                picked = true;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //Hide pick text
        }
    }

}
