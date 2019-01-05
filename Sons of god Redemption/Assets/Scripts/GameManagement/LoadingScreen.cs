using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour {

    public GameObject loadingScreen;
    public Slider slider;
    public SceneController sceneController;

	// Update is called once per frame
	void Update () {

        loadingScreen.SetActive(sceneController.isLoading);
        slider.value = sceneController.progress;

    }
}
