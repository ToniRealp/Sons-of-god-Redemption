using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinalCinematic : MonoBehaviour {


    public InputManager inputManager;
    public SceneController sceneController;
    public Image image;

    public float startTime, textInScreenTime = 4.0f;
    private int imageNum;
    private bool textShown, changeSceneFlag;
    private Color c;

    // Use this for initialization
    void Start()
    {
        textShown = changeSceneFlag = false;
        c = image.color;
        c.a = 0;
        image.color = c;

    }

    // Update is called once per frame
    void Update()
    {
        c = image.color;
        if (image.color.a < 1 && !textShown)
        {
            c.a += 0.01f;
            if (inputManager.attackButton || inputManager.dashButton || inputManager.interact || inputManager.strongAttackButton) { c.a = 1; }
            image.color = c;
            startTime = Time.time;
        }
        else
        {
            textShown = true;
        }
        if (textShown && (Time.time - startTime >= textInScreenTime || inputManager.attackButton || inputManager.dashButton || inputManager.interact || inputManager.strongAttackButton))
        {
            c.a -= 0.01f;
            if (inputManager.attackButton || inputManager.dashButton || inputManager.interact || inputManager.strongAttackButton) { c.a = 0; }
            image.color = c;
            if (image.color.a <= 0 && !changeSceneFlag)
            {
                sceneController.changeScene("MainMenu");
                changeSceneFlag = true;
            }
        }

    }
}
