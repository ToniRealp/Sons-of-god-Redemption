using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionForce : MonoBehaviour {

    public float minForce, maxForce, radius;
    // Use this for initialization
    private void Start()
    {
        Explode();
    }

    public void Explode()
    {
        foreach(Transform t in transform)
        {
            Rigidbody rb = t.GetComponent<Rigidbody>();

            if (rb != null) rb.AddExplosionForce(Random.Range(minForce,maxForce), transform.position, radius);
        }
        
    }

}
