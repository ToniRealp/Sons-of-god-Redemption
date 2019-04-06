using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstNarrative : MonoBehaviour {

    public InputManager inputManager;
    public SceneController sceneController;
    public Image[] image = new Image[7];

    public float startTime, textInScreenTime = 4.0f;
    private int imageNum;
    private bool textShown;
    private Color c;

    // Use this for initialization
    void Start()
    {
        textShown = false;
        imageNum = 0;
        for (int i = 0; i < 7; i++)
        {
            c = image[i].color;
            c.a = 0;
            image[i].color = c;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (inputManager.escape) { imageNum = 7; }
        if (imageNum < 7)
        {
            c = image[imageNum].color;
            if (image[imageNum].color.a<1 && !textShown)
            {
                c.a += 0.01f;
                if (inputManager.attackButton || inputManager.dashButton ||  inputManager.interact || inputManager.strongAttackButton) { c.a = 1; }
                image[imageNum].color = c;
                startTime = Time.time;
            }
            else {
                textShown = true;
            }
            if (textShown && (Time.time - startTime >= textInScreenTime ||  inputManager.attackButton || inputManager.dashButton || inputManager.interact || inputManager.strongAttackButton))
            {
                c.a -= 0.01f;
                if (inputManager.attackButton || inputManager.dashButton || inputManager.interact || inputManager.strongAttackButton) { c.a = 0; }
                image[imageNum].color = c;
                if (image[imageNum].color.a <= 0)
                {
                    imageNum++;
                    textShown = false;
                }
            }
        }
        else
        {
            sceneController.changeScene("TutorialScene");
        }

    }
}
