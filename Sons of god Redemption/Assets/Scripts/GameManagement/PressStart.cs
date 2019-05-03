using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressStart : MonoBehaviour {

    public InputManager inputManager;
    public SceneController sceneController;


	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        if (inputManager.attackButton || inputManager.dashButton || inputManager.escape || inputManager.interact || inputManager.strongAttackButton)
        {
            sceneController.changeScene("MainMenu");
        }
	}
}
