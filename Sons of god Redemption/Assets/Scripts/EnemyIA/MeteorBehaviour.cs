using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorBehaviour : MonoBehaviour {

    public GameObject explosion;
    public AudioManager audioManager;

    private void Start()
    {
        audioManager = GetComponent<AudioManager>();
        audioManager.Play("MeteorFall");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag=="Player")
        {
            collision.gameObject.GetComponent<PlayerController>().bossDmg = GameObject.Find("FirstBoss").GetComponent<FirstBossBehaviour>().rainDmg;
            collision.gameObject.GetComponent<PlayerController>().meteorHit = true;
        }
        Instantiate(explosion, transform.position, transform.rotation);


        //Destroy(this.gameObject);
    }

}
