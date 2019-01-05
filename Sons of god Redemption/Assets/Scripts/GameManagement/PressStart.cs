using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressStart : MonoBehaviour {

    public InputManager inputManager;
    public SceneController sceneController;

    private float startTime;

	// Use this for initialization
	void Start () {
        startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
        if (Time.time>= startTime+1.0f)
        {
            if (inputManager.attackButton || inputManager.dashButton || inputManager.escape || inputManager.interact || inputManager.strongAttackButton)
            {
                sceneController.changeScene("MainMenu");
            }
        }
	}
}
