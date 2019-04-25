using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialTextScript : MonoBehaviour {

    public float maxScale = 1.1f, startReducingTime = 2f, normalReducingTime = 0.1f, movingSpeed = 1f, increaseValue = 0.005f;
    public Vector3 finalPosition = new Vector3(-130, -205, 0);

    private float startTime;
    private int state;
    private bool moved;

	// Use this for initialization
	void Start () {
        moved = false;
        state = 0;

	}
	
	// Update is called once per frame
	void Update () {
        ScaleTextP(startReducingTime);
        if (state>=2) {
            TranslateText(finalPosition);
        }
    }

    private void TranslateText(Vector3 finalPosition)
    {
        if (!moved)
        {
            if (gameObject.GetComponent<RectTransform>().localPosition.x <= finalPosition.x) {
                moved = true;
            }
            else
            {
                gameObject.GetComponent<RectTransform>().localPosition = Vector3.MoveTowards(gameObject.GetComponent<RectTransform>().localPosition, finalPosition, movingSpeed);
            }

        }
        
    }

    private void ScaleTextP(float reduceTime) {
        if (state < 3) {
            if (state == 0) {
                if (gameObject.GetComponent<RectTransform>().localScale.x < maxScale) {
                    gameObject.GetComponent<RectTransform>().localScale = new Vector3 (gameObject.GetComponent<RectTransform>().localScale.x + increaseValue,
                        gameObject.GetComponent<RectTransform>().localScale.y + increaseValue, gameObject.GetComponent<RectTransform>().localScale.z);
                }
                else {
                    startTime = Time.time;
                    state++;
                }
            }
            else if (state == 1) {
                if (Time.time - startTime >= startReducingTime) {
                    state++;
                }
            }
            else if (state == 2) {
                if (gameObject.GetComponent<RectTransform>().localScale.x > 1f) {
                    gameObject.GetComponent<RectTransform>().localScale = new Vector3(gameObject.GetComponent<RectTransform>().localScale.x - increaseValue/5,
                        gameObject.GetComponent<RectTransform>().localScale.y - increaseValue/5, gameObject.GetComponent<RectTransform>().localScale.z);
                }
                else {
                    state++;
                }
            }
            else {
                state = 0;
            }
        }
    }

    public void ScaleText()
    {
        ScaleTextP(normalReducingTime);
    }


}
