using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KameScript : MonoBehaviour {

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
            other.gameObject.GetComponent<PlayerController>().bossDmg = GameObject.Find("SecondBoss").GetComponent<SecondBossBehaviour>().kameDmg;
            other.gameObject.GetComponent<PlayerController>().explosionHit = true;
        }
    }
}
