using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionForce : MonoBehaviour {

    // Use this for initialization
    bool flag = false;
	void Update () {
        if (!flag)
        {
            GetComponent<Rigidbody>().AddExplosionForce(3000.0f, transform.position, 5.0f);
            flag = true;
        }
  
	}

}
