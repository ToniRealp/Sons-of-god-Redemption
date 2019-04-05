using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallScript : MonoBehaviour {

    public GameObject explosion;
    public float Speed = 1;

	// Use this for initialization
	void Start () {
        gameObject.GetComponent<Rigidbody>().velocity = gameObject.transform.forward * Speed;
	}
	

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().bossDmg = GameObject.Find("GoDFinal(Clone)").GetComponent<FinalBossBehaviour>().fireDmg;
            collision.gameObject.GetComponent<PlayerController>().meteorHit = true;
        }
        Instantiate(explosion, transform.position, transform.rotation);
        Destroy(this.gameObject);
    }
}
