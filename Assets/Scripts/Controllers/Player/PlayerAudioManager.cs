using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    AudioSource audioSource1, audioSource2;
    [SerializeField]
    AudioClip[] clips;
    void Start()
    {
        audioSource1 = GetComponents<AudioSource>()[0];
        audioSource2 = GetComponents<AudioSource>()[1];

    }

   


    void Step()
    {
        audioSource1.PlayOneShot(clips[0]);
    }

    void Jump()
    {
        audioSource2.PlayOneShot(clips[1]);
    }
}
