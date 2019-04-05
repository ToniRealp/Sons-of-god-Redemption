using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCinematic : MonoBehaviour {

    public GameObject cam1, cam2;

    void ChangeCamera()
    {
        cam1.SetActive(false);
        cam2.SetActive(true);
    }
    void ResetCamera()
    {
        cam1.SetActive(true);
        cam2.SetActive(false);
    }
}
