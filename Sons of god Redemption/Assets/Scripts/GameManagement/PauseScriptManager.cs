using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScriptManager : MonoBehaviour {

    public GameObject PausePanel;
    public InputManager inputManager;

	// Use this for initialization
	void Start () {
        PausePanel.SetActive(false);
	}

    private void Update()
    {
        if (inputManager.escape)
        {
            activatePauseMenu();
        }
    }

    public void activatePauseMenu()
    {
        Time.timeScale = 0f;
        PausePanel.SetActive(true);
    }

    public void exitPauseMenu()
    {
        Time.timeScale = 1.0f;
        PausePanel.SetActive(false);
    }

}
