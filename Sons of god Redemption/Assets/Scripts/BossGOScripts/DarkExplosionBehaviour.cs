using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkExplosionBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerController>().bossDmg = GameObject.Find("SecondBoss").GetComponent<SecondBossBehaviour>().explosionDmg;
            other.gameObject.GetComponent<PlayerController>().explosionHit = true;
        }
    }
}
