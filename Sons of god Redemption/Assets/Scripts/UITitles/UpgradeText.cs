using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeText : MonoBehaviour {

    public float dieTime = 2;
    public float movingSpeed = 0.2f;
    public TextMeshProUGUI text;

    private bool appeared;
    private Color c;

    // Use this for initialization
    void Start()
    {
        appeared = false;
        text = GetComponent<TextMeshProUGUI>();
        c = text.color = new Color(1, 1, 1, 0);
    }

    // Update is called once per frame
    void Update()
    {

        if (!appeared)
        {
            if (c.a < 1)
            {
                c.a += 0.01f;
                text.color = c;
            }
            else
            {
                appeared = true;
            }
        }

        dieTime -= Time.deltaTime;
        if (dieTime <= 0)
        {
            c.a -= 0.01f;

            text.color = c;
            if (c.a <= 0)
            {
                Destroy(gameObject);
            }
        }
        gameObject.GetComponent<RectTransform>().localPosition = Vector3.MoveTowards(gameObject.GetComponent<RectTransform>().localPosition, gameObject.GetComponent<RectTransform>().localPosition+ new Vector3(0,2,0), movingSpeed);
    }
}
