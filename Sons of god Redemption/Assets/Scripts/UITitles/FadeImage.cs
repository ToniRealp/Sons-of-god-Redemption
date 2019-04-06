using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeImage : MonoBehaviour {

    
    public float imageDieTime = 5;
    public Image image;

    private Color i;

    // Use this for initialization
    void Start()
    {
        i = image.color;
        i.a = 0;
        image.color = i;
    }

    // Update is called once per frame
    void Update()
    {

        imageDieTime -= Time.deltaTime;
        if (imageDieTime <= 0)
        {
            i.a -= 0.01f;
            image.color = i;
        }
        else
        {
            if (i.a <= 1)
            {
                i.a += 0.01f;
                image.color = i;
            }
        }

    }
}
