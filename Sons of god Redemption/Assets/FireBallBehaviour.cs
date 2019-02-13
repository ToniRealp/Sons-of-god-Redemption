﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {

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