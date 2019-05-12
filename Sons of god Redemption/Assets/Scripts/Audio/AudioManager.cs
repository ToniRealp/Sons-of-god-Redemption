using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public Sound[] sounds;
    public AudioMixerGroup masterMixer;

	void Awake () {
        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.loop = s.loop;
            s.source.GetComponent<AudioSource>().outputAudioMixerGroup = masterMixer;
            s.source.spatialize = true;
            s.source.spatialBlend = 1;
            //s.source.rolloffMode = AudioRolloffMode.Logarithmic;
            s.source.minDistance = 1;
            s.source.rolloffMode = s.rolloffMode;
        }
	}
	
    public void Play (string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound:" + name + "notFound");
        }
        s.source.Play();
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound:" + name + "notFound");
        }
        float volume = s.source.volume;
        s.source.volume = 0;
        s.source.Stop();
        s.source.volume = volume;
    }

    public void SetVolume(string name, float vol)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound:" + name + "notFound");
        }
        s.source.volume = vol;
    }

    public float GetVolume(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound:" + name + "notFound");
        }
        return s.source.volume;
    }

    public bool isPlaying(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        return s.source.isPlaying;
    }
}
