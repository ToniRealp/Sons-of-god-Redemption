using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallScript : MonoBehaviour {

    public float Speed = 1;

	// Use this for initialization
	void Start () {
        gameObject.GetComponent<Rigidbody>().velocity = gameObject.transform.forward * Speed;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().bossDmg = GameObject.Find("FinalBoss").GetComponent<FinalBossBehaviour>().fireDmg;
            collision.gameObject.GetComponent<PlayerController>().meteorHit = true;
        }
        Destroy(this.gameObject);
    }
}
