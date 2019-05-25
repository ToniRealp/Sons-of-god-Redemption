using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallExplosion : MonoBehaviour {

    public AudioManager audioManager;
    public string soundName;

	// Use this for initialization
	void Start () {
        audioManager = GetComponent<AudioManager>();
        audioManager.Play(soundName);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
