using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleScript : MonoBehaviour {

    private void OnParticleCollision(GameObject other)
    {
        if (other.tag=="Player")
        {
            other.GetComponent<PlayerController>().bossDmg = GameObject.Find("FirstBoss").GetComponent<FirstBossBehaviour>().roarDmg;
            other.GetComponent<PlayerController>().fireHit = true;
        }
    }
}
