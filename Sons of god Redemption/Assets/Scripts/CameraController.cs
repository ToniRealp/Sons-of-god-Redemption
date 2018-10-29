using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

   public GameObject cam1,cam2;

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            cam1.SetActive(false);
            cam2.SetActive(true); 
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            cam1.SetActive(true);
            cam2.SetActive(false);
        }
    }
}
