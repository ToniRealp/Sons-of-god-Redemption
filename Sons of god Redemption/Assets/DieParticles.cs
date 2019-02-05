using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieParticles : MonoBehaviour {

    private float dieTime = 2;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        dieTime -= Time.deltaTime;
        if (dieTime<=0)
        {
            Destroy(this.gameObject);
        }

	}
}
