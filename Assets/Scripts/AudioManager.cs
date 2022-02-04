using System;
using Classes;
using Controllers.Player;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
	[SerializeField] private Sound[] _sounds;
    protected override void OnAwakeEvent()
    {
	    base.OnAwakeEvent();
	    foreach(var s in _sounds)
	    {
		    s.source =  gameObject.AddComponent<AudioSource>();
		    s.source.clip = s.clip;
		    s.source.volume = s.volume;
		    s.source.pitch = s.pitch;
	    }
    }

    public void Play(string clipName)
    {
	    var s =  Array.Find(_sounds, sound => sound.name == clipName);
	    s.source.Play();
    }
}
