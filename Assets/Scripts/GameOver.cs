using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

public class GameOver : MonoBehaviour
{
  
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.FailLevel();
        }
    }
}
