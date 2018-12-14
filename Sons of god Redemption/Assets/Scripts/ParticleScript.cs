using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnParticleCollision(GameObject other)
    {
        if (other.tag=="Player")
        {
            other.GetComponent<PlayerController>().boss = gameObject;
            other.GetComponent<PlayerController>().fireHit = true;
        }
    }
}
