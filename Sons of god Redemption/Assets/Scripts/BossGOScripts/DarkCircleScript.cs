using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkCircleScript : MonoBehaviour {

    public float lifeTime = 6, speed = 0.02f;
    Vector3 direction;



	// Use this for initialization
	void Start () {

        direction = new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f)).normalized;

    }
	
	// Update is called once per frame
	void Update () {
        transform.position += direction*speed;
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0) Destroy(gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerController>().bossDmg = GameObject.Find("SecondBoss(Clone)").GetComponent<SecondBossBehaviour>().circleDmg;
            other.GetComponent<PlayerController>().fireHit = true;
        }
    }

}
