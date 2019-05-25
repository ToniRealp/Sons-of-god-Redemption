﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour {

    public Vector3 upPosition;
    public Vector3 downPosition;
    public float speed = 3f;

    public bool up, down;


	// Use this for initialization
	void Start () {
        upPosition = transform.position;
        downPosition =  upPosition - new Vector3(0, 5.1f, 0);
        transform.position = downPosition;
        up = down = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (up)
        {
            if (Vector3.Distance(transform.position, upPosition) >= speed*Time.deltaTime)
            {
                transform.position = Vector3.MoveTowards(transform.position, upPosition, speed * Time.deltaTime);
            }
            else
            {
                up = false;
            }
        }
        if (down)
        {
            if (Vector3.Distance(transform.position, downPosition) >= speed*Time.deltaTime)
            {
                transform.position = Vector3.MoveTowards(transform.position, downPosition, speed * Time.deltaTime);
            }
            else
	        {
                down = false;
                gameObject.SetActive(false);
            }
        }
    }

   public void GoUp()
    {
        up = true;
        down = false;
    }

    public void GoDown()
    {
        down = true;
        up = false;
    }


}
