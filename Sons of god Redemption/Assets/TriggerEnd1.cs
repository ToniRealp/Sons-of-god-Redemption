﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEnd1 : MonoBehaviour {

    public SceneController sceneController;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            sceneController.changeScene("LevelGame2");
        }
    }
}
