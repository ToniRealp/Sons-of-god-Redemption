using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleMeteor : MonoBehaviour {

    public GameObject fracturedMesh;
    GameObject FM;

    // Update is called once per frame
    private void OnCollisionEnter(Collision other)
    {
       
        Quaternion rotation = this.transform.rotation;
        rotation.eulerAngles = new Vector3(0, 0, 0);
        Vector3 position = this.transform.position;
        FM=Instantiate(fracturedMesh, position, rotation);

        Destroy(this.gameObject);
       
    }
}
