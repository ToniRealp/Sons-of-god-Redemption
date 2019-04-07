using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorBehaviour : MonoBehaviour {

    public GameObject explosion;
    public GameObject decal;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag=="Player")
        {
            collision.gameObject.GetComponent<PlayerController>().bossDmg = GameObject.Find("FirstBoss").GetComponent<FirstBossBehaviour>().rainDmg;
            collision.gameObject.GetComponent<PlayerController>().meteorHit = true;
        }
        //else if(collision.gameObject.tag=="Floor")
        //{
        //    Vector3 position = transform.position;
        //    position.y = 0;
        //    Instantiate(explosion, transform.position, transform.rotation);
        //    Quaternion rotation = new Quaternion
        //    {
        //        eulerAngles = new Vector3(0, 0, 0)
        //    };
        //    Instantiate(decal, transform.position, rotation);
        //}
        
        Destroy(this.gameObject);
    }

}
