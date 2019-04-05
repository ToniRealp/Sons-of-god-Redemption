using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerScript2 : MonoBehaviour {

    public GameObject LevelController;
    public int id;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            LevelController.GetComponent<Level2Controller>().trigger[id] = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            LevelController.GetComponent<Level2Controller>().trigger[id] = false;
        }
    }

}
