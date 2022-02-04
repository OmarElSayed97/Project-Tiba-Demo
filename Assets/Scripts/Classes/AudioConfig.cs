using System;
using Classes.Enums;
using UnityEngine;

namespace Classes
{
    [CreateAssetMenu()]
    public class AudioConfig : ScriptableObject
    {
        [SerializeField] public AudioDict SFXDict { get; private set; }

        public AudioConfig()
        {
            SFXDict = new AudioDict();
        }
    }
    
    [Serializable]
    public class AudioDict : UnitySerializedDictionary<eAudioSFX, Sound> { }
}
