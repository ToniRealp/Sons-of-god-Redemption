using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossTitle : MonoBehaviour {

    public float fadeTime = 5;
    public float dieTime = 10;
    public Text text;

    private Color c;
    private bool opaque;

    // Use this for initialization
    void Start()
    {
        opaque = false;
        c = text.color = new Color(1, 1, 1, 0);
        text.gameObject.transform.localScale = new Vector3(0f, 0f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!opaque)
        {
            if (text.gameObject.transform.localScale.x < 1)
            {
                text.gameObject.transform.localScale += new Vector3(0.05f, 0.05f, 0.05f);
            }
            c.a += 0.01f;
            text.color = c;
            if (c.a >= 1f)
            {
                opaque = true;
            }
        }
        else
        {
            fadeTime -= Time.deltaTime;
            if (fadeTime <= 0)
            {
                if (text.gameObject.transform.localScale.x > 0)
                {
                    text.gameObject.transform.localScale -= new Vector3(0.01f,0.01f,0.01f);
                }

                c.a -= 0.01f;
                text.color = c;
            }
        }
        dieTime -= Time.deltaTime;
        if (dieTime <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
