using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScriptManager : MonoBehaviour {

    public GameObject PausePanel;
    public GameObject OptionsPanel;
    public InputManager inputManager;

	// Use this for initialization
	void Start () {
        PausePanel.SetActive(false);
        OptionsPanel.SetActive(false);
	}

    private void Update()
    {
        if (inputManager.escape && !OptionsPanel.activeSelf)
        {
            activatePauseMenu();
        }
    }

    public void activatePauseMenu()
    {
        Time.timeScale = 0f;
        PausePanel.SetActive(true);
        Cursor.visible = true;
    }

    public void exitPauseMenu()
    {
        Time.timeScale = 1.0f;
        PausePanel.SetActive(false);
        Cursor.visible = false;
    }

}
