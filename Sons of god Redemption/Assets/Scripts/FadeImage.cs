using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeImage : MonoBehaviour {

    public float dieTime = 5;
    public Image image;

    private Color c;

    // Use this for initialization
    void Start()
    {
        c = image.color;
    }

    // Update is called once per frame
    void Update()
    {
        dieTime -= Time.deltaTime;
        if (dieTime <= 0)
        {
            c.a -= 0.01f;

            image.color = c;
        }

    }
}
