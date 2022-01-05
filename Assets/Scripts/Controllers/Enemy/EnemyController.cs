using System;
using Cinemachine.Utility;
using Controllers.Player;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controllers.Enemy
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private Transform GFX;
        [SerializeField] private Transform playerTransform;
        [SerializeField] private Vector3 playerOffset = new Vector3(0, 1, 0);
        [SerializeField] private float detectionRadius = 10f;
        [SerializeField] private float patrolSpeed = 5f;
        [SerializeField] private float followSpeed = 10f;
        [SerializeField] private float followMinDistance = 2f;
        [SerializeField] private bool followState;
        private float _playerDistance;

        private RaycastHit _hit;
        private Ray _ray;

        private Vector3 _movementDirection;

        private Action _currentState;
        
        private void Start()
        {
            playerTransform = InputController.Instance.transform;
            GFX.DOLocalMoveY(0.35f, 0.8f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.Linear);
            _currentState = Patrol;
        }

        private void Update()
        {
            _currentState();
            DetectPlayer();
        }

        private void DetectPlayer()
        {
            var position = transform.position;
            _ray.origin = position;
            _ray.direction = ((playerTransform.position + playerOffset) - position).normalized;
            if (Physics.Raycast(_ray, out _hit, detectionRadius))
            {
                if (_hit.collider.gameObject.CompareTag("Player"))
                {
                    followState = true;
                    _currentState = FollowPlayer;
                    _playerDistance = _hit.distance;
                }
            }
            else
            {
                followState = false;
                _currentState = Patrol;
            }
        }
        
        private void FollowPlayer()
        {
            _movementDirection = _ray.direction * (followSpeed * Time.deltaTime);
            _movementDirection.y = 0;
            if (_playerDistance > followMinDistance)
            {
                transform.position += _movementDirection;    
            }
            
        }

        private void Patrol()
        {
            
        }
        
        
    }
}
