using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeScript : MonoBehaviour {

    public Text text;

    private Color c;

	// Use this for initialization
	void Start () {
        c = text.color = new Color(1,1,1,0);
	}
	
	// Update is called once per frame
	void Update () {

        c.a += 0.01f;

        text.color = c;
    }
}
