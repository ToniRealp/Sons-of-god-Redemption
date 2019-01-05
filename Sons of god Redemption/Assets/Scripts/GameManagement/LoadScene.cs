using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadScene : MonoBehaviour {

    public SceneController sceneController;
    // Use this for initialization
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            sceneController.changeScene("LevelGame");
        }
    }
}
