using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpBehaviour : MonoBehaviour {

    public bool picked;
    public Animator animator;

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
           
            GameObject.Find("Leliel").GetComponent<PlayerController>().baseAttack += 2;
            GameObject.Find("Leliel").GetComponent<PlayerController>().stats.health += 10;
            GameObject.Find("Leliel").GetComponent<PlayerController>().health += 10;
            GameObject.Find("Leliel").GetComponent<PlayerController>().healthBar.value = GameObject.Find("Leliel").GetComponent<PlayerController>().health;
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
                picked = true;
                animator.SetTrigger("open");
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

    public void DestroyPickUp()
    {
        Destroy(this.gameObject);
    }

}
