using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScriptManager : MonoBehaviour {

    public GameObject PausePanel;

	// Use this for initialization
	void Start () {
        PausePanel.SetActive(false);
	}

    void activatePauseMenu()
    {
        Time.timeScale = 0f;
        PausePanel.SetActive(true);
    }

    void exitPauseMenu()
    {
        Time.timeScale = 1.0f;
        PausePanel.SetActive(false);
    }

}
