using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionForce : MonoBehaviour {

    public float minForce, maxForce, radius;
    Vector3 tf;
    
    // Use this for initialization
    private void Start()
    {
        Explode();
    }

    public void Explode()
    {
        foreach(Transform t in transform)
        {
            tf = transform.position;
            Rigidbody rb = t.GetComponent<Rigidbody>();
            if (t.gameObject.tag == "Barrilete")
            {
                tf.z += 1.2f;
            }

            if (rb != null) rb.AddExplosionForce(Random.Range(minForce,maxForce), tf, radius);
        }
        
    }

}
