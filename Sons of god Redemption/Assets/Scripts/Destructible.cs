using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour {

    public GameObject fracturedMesh;

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag=="LightAttack1" || other.tag == "LightAttack2" || other.tag == "LightAttack3" || other.tag == "StrongAttack1" || 
            other.tag == "StrongAttack2")
        {
            Quaternion rotation = this.transform.rotation;
            rotation.eulerAngles = new Vector3(0, 0, 0);
            Vector3 position = this.transform.position;
            position.z-=1.2f;
            Instantiate(fracturedMesh,position,rotation);
            Destroy(this.gameObject);
        }
    }
}
