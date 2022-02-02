using System;
using Cinemachine;
using UnityEngine;

namespace Tools
{
    [RequireComponent(typeof(Collider))]
    public class VCameraSwitcher : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera cam;
        [SerializeField] private int priorityBoostValue = 10;

        private int _initialPriority;

        private void Start()
        {
            _initialPriority = cam.Priority;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Player"))
                return;
            cam.Priority = _initialPriority + priorityBoostValue;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.gameObject.CompareTag("Player"))
                return;
            cam.Priority = _initialPriority;
        }
        
    }
    
}
