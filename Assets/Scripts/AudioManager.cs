using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager _instance;
    public Sound[] sounds;
    void Awake()
    {
        _instance = this;
        foreach(Sound s in sounds)
        {
           s.source =  gameObject.AddComponent<AudioSource>();
           s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
        }
    }

   public void Play(string name)
    {
       Sound s =  Array.Find(sounds, sound => sound.name == name);
        s.source.Play();
    }
}
