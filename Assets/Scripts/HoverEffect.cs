using System;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class HoverEffect : MonoBehaviour
{
   
    [SerializeField] private Vector3 angularVelocity;
    [SerializeField] private Vector3 moveValue;
    [SerializeField] private float duration;
    [SerializeField] private bool randomStart;
    
    
    private void OnEnable()
    {
        ApplyTween();
    }
    
    private void OnDisable()
    {
        transform.DOKill();
    }

    private void ApplyTween()
    {
        transform.DOBlendableLocalRotateBy(angularVelocity, duration)
            .SetDelay(randomStart ? Random.Range(0, 1) : 0)
            .SetLoops(-1, LoopType.Incremental)
            .SetEase(Ease.Linear);

        transform.DOBlendableLocalMoveBy(moveValue, duration)
            .SetDelay(randomStart ? Random.Range(0, 1) : 0)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.Linear);

    }

}
