using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FirstNarrative : MonoBehaviour {

    public InputManager inputManager;
    public SceneController sceneController;
    public Image[] image = new Image[3];
    public TextMeshProUGUI[] text = new TextMeshProUGUI[5];


    public float startTime, textInScreenTime = 6.0f;
    private int imageNum, textNum;
    private bool textShown, imageShown, nextImage, transition, changeSceneFlag;
    private Color c, t;

    // Use this for initialization
    void Start()
    {
        transition = nextImage = imageShown = textShown = changeSceneFlag = false;
        imageNum = textNum = 0;
        for (int i = 0; i < 3; i++)
        {
            c = image[i].color;
            c.a = 0;
            image[i].color = c;
        }
        for (int i = 0; i < 5; i++)
        {
            t = text[i].color;
            t.a = 0;
            text[i].color = t;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (imageNum < 3)
        {
            c = image[imageNum].color;
            if (image[imageNum].color.a < 1 && !imageShown)
            {
                c.a += 0.01f;
                image[imageNum].color = c;
                startTime = Time.time;
            }
            else
            {
                imageShown = true;
            }
            if (imageShown && nextImage)
            {
                c.a -= 0.01f;
                image[imageNum].color = c;
                if (image[imageNum].color.a <= 0)
                {
                    imageNum++;
                    imageShown = false;
                    nextImage = false;
                }
            }
        }
        else
        {
            if (!changeSceneFlag)
            {
                sceneController.changeScene("TutorialScene");
                changeSceneFlag = true;
            }
        }

        if (textNum < 5)
        {
            if (inputManager.attackButton || inputManager.dashButton || inputManager.interact || inputManager.strongAttackButton) { startTime = Time.time - textInScreenTime - 1; }
            c = text[textNum].color;
            if (text[textNum].color.a < 1 && !textShown)
            {
                c.a += 0.01f;
                //if (inputManager.attackButton || inputManager.dashButton || inputManager.interact || inputManager.strongAttackButton) { c.a = 1; }
                text[textNum].color = c;
                startTime = Time.time;
            }
            else
            {
                textShown = true;
            }
            if (textShown && Time.time - startTime >= textInScreenTime)/* || inputManager.attackButton || inputManager.dashButton || inputManager.interact || inputManager.strongAttackButton)*/
            {
                if ((textNum >= 2) && !nextImage)
                {
                    nextImage = true;
                }
                c.a -= 0.01f;
                //if (inputManager.attackButton || inputManager.dashButton || inputManager.interact || inputManager.strongAttackButton) { c.a = 0; }
                text[textNum].color = c;
                if (text[textNum].color.a <= 0)
                {
                    textNum++;
                    textShown = false;
                }
            }
        }






        //if (inputManager.escape) { imageNum = 7; }
        //if (imageNum < 7)
        //{
        //    c = image[imageNum].color;
        //    if (image[imageNum].color.a<1 && !textShown)
        //    {
        //        c.a += 0.01f;
        //        if (inputManager.attackButton || inputManager.dashButton ||  inputManager.interact || inputManager.strongAttackButton) { c.a = 1; }
        //        image[imageNum].color = c;
        //        startTime = Time.time;
        //    }
        //    else {
        //        textShown = true;
        //    }
        //    if (textShown && (Time.time - startTime >= textInScreenTime ||  inputManager.attackButton || inputManager.dashButton || inputManager.interact || inputManager.strongAttackButton))
        //    {
        //        c.a -= 0.01f;
        //        if (inputManager.attackButton || inputManager.dashButton || inputManager.interact || inputManager.strongAttackButton) { c.a = 0; }
        //        image[imageNum].color = c;
        //        if (image[imageNum].color.a <= 0)
        //        {
        //            imageNum++;
        //            textShown = false;
        //        }
        //    }
        //}
        //else
        //{
        //    sceneController.changeScene("TutorialScene");
        //}

    }
}
