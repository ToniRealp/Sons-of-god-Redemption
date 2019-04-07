using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCinematic : MonoBehaviour {

    public GameObject cam1, cam2;

    private void Start()
    {
        cam1 = GameObject.Find("CM vcam1");
        cam2 = GameObject.Find("Cinematic");
        cam2.SetActive(false);
    }

    public void ChangeCamera()
    {
        cam1.SetActive(false);
        cam2.SetActive(true);
    }
    public void ResetCamera()
    {
        cam1.SetActive(true);
        cam2.SetActive(false);
    }
}
