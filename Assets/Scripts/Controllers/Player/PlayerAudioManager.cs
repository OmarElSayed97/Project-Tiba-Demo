using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    AudioSource audioSource;
    AudioClip clip;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        clip = audioSource.clip;
    }

   


    void Step()
    {
        audioSource.PlayOneShot(clip);
    }
}
