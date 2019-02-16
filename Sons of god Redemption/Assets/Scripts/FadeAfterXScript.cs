using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeAfterXScript : MonoBehaviour {

    public float dieTime = 5;
    public Text text;

    private Color c;

    // Use this for initialization
    void Start()
    {
        c = text.color = new Color(1, 1, 1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        dieTime -= Time.deltaTime;
        if (dieTime <= 0)
        {
            c.a -= 0.01f;

            text.color = c;
        }

    }
}
