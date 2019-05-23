using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Events;

public class CameraShake : MonoBehaviour
{

    public float ShakeDuration = 0.3f;          // Time the Camera Shake effect will last
    public float ShakeAmplitude = 0.1f;         // Cinemachine Noise Profile Parameter
    public float ShakeFrequency = 2.0f;         // Cinemachine Noise Profile Parameter

    private float ShakeElapsedTime = 0f;
    public bool damaged;

    // Cinemachine Shake
    public CinemachineVirtualCamera VirtualCamera;
    private CinemachineBasicMultiChannelPerlin virtualCameraNoise;

    // Use this for initialization
    void Start()
    {
        // Get Virtual Camera Noise Profile
        if (VirtualCamera != null)
            virtualCameraNoise = VirtualCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerController.damaged)
        {
            ShakeElapsedTime = ShakeDuration;
            PlayerController.damaged = false; 
        }
        if (PlayerController.bigHit)
        {
            ShakeElapsedTime = ShakeDuration;
            PlayerController.bigHit = false;
        }

        // If the Cinemachine componet is not set, avoid update
        if (VirtualCamera != null && virtualCameraNoise != null)
        {
            // If Camera Shake effect is still playing
            if (ShakeElapsedTime > 0)
            {
                // Set Cinemachine Camera Noise parameters
                virtualCameraNoise.m_AmplitudeGain = ShakeAmplitude;
                virtualCameraNoise.m_FrequencyGain = ShakeFrequency;

                // Update Shake Timer
                ShakeElapsedTime -= Time.deltaTime;
            }
            else
            {
                // If Camera Shake effect is over, reset variables
                virtualCameraNoise.m_AmplitudeGain = 0f;
                ShakeElapsedTime = 0f;
            }
        }
    }
}