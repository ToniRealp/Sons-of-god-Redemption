using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinalCinematic : MonoBehaviour {


    public InputManager inputManager;
    public SceneController sceneController;
    public Image[] image = new Image[4];

    public float startTime, textInScreenTime = 4.0f;
    public int imageNum;
    private bool textShown, changeSceneFlag, imageShown;
    private Color c, c1;

    // Use this for initialization
    void Start()
    {
        imageShown = textShown = changeSceneFlag = false;
        imageNum =  0;
        for (int i = 0; i < 4; i++)
        {
            c = image[i].color;
            c.a = 0;
            image[i].color = c;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (imageNum < 2)
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
            if (imageShown && Time.time - startTime >= textInScreenTime)
            {
                c.a -= 0.01f;
                image[imageNum].color = c;
                if (image[imageNum].color.a <= 0)
                {
                    imageNum++;
                    imageShown = false;
                }
            }
        }
        else
        {
            if (imageNum == 2)
            {
                c = image[2].color;
                c1 = image[3].color;
                if (image[2].color.a < 1 && !imageShown)
                {
                    c.a += 0.01f;
                    image[imageNum].color = c;
                    startTime = Time.time;
                }
                else
                {
                    imageShown = true;
                }
                if (imageShown && Time.time - startTime >= textInScreenTime)
                {
                    c.a -= 0.01f;
                    c1.a += 0.01f;
                    image[2].color = c;
                    image[3].color = c1;
                    if (image[2].color.a <= 0)
                    {
                        imageNum++;
                        startTime = Time.time;
                        imageShown = true;
                    }
                }
            }
            else
            {
                if (imageNum < 4)
                {
                    c1 = image[3].color;
                    if (imageShown && Time.time - startTime >= textInScreenTime)
                    {
                        c1.a -= 0.01f;
                        image[3].color = c1;
                        if (image[3].color.a <= 0)
                        {
                            imageNum++;
                            imageShown = false;
                        }
                    }
                }
                else
                {
                    if (!changeSceneFlag)
                    {
                        sceneController.changeScene("MainMenu");
                        changeSceneFlag = true;
                    }
                }
            }
        }


    }

}
