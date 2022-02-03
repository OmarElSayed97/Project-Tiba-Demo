using System;
using Cinemachine.Utility;
using Controllers.Player;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controllers.Enemy
{
    [RequireComponent(typeof(CharacterController))]
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
        [SerializeField] private LayerMask killMask;
        [SerializeField, Range(0f, 1f)] private float gravityFactor = 0.8f;
        [SerializeField] GameObject destructedPrefab;
        private float _playerDistance;
        private CharacterController _controller;

        private RaycastHit _hit;
        private Ray _ray;
        private CollisionFlags _flags;

        private Vector3 _movementDirection;
        private Vector3 _verticalVelocity;

        private float _headDetectionDistance;

        private Action _currentState;
        private Action _previousState;
        private Action _gravityAction;
        private Action _headDetector;
        
        private void Start()
        {
            playerTransform = InputController.Instance.transform;
            _controller = GetComponent<CharacterController>();
            _headDetectionDistance = (_controller.height) + 0.2f;
            GFX.DOLocalMoveY(0.35f, 0.8f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.Linear);
            _currentState = Patrol;
            _gravityAction = ApplyGravity;
            _headDetector = HeadDetector;
        }

        private void FixedUpdate()
        {
            _gravityAction();
            _headDetector();
        }

        private void Update()
        {
            _currentState();
            DetectPlayer();
        }

        private void OnDisable()
        {
            GFX.DOKill();
        }

        private void DetectPlayer()
        {
            var position = transform.position;
            _ray.origin = position + playerOffset;
            _ray.direction = ((playerTransform.position + playerOffset) - position).normalized;
            if (Physics.Raycast(_ray, out _hit, detectionRadius, -1, QueryTriggerInteraction.Ignore) && _hit.collider.gameObject.CompareTag("Player"))
            {
                _playerDistance = _hit.distance;
                followState = true;
                _currentState = FollowPlayer;
                
            }
            else
            {
                followState = false;
                _currentState = Patrol;
            }
        }

        private void ApplyGravity()
        {
            if (!_controller.isGrounded)
            {
                _verticalVelocity += Physics.gravity * (gravityFactor * Time.fixedDeltaTime);
            }
            else
            {
                _verticalVelocity.y = 0;
            }
            _flags = _controller.Move(_verticalVelocity * Time.fixedDeltaTime);
        }

        private void OnFirstPortalEnter()
        {
            _previousState = _currentState;
            _currentState = () => { };
            _gravityAction = () => { };
            _headDetector = () => { };
        }

        private void OnFirstPortalExit()
        {
            _currentState = _previousState;
            _gravityAction = ApplyGravity;
            _headDetector = HeadDetector;
        }

        private void HeadDetector()
        {
            if (Physics.Raycast(transform.position, transform.up, _headDetectionDistance ,killMask, QueryTriggerInteraction.Collide))
            {
                KillEnemy();
            }
        }


        private void FollowPlayer()
        {
            _movementDirection = _ray.direction * (followSpeed * Time.deltaTime);
            _movementDirection.y = 0;
            if(_ray.direction.x * transform.right.x > 0.5f)
            {
                transform.right = -_movementDirection;
            }
            if (_playerDistance > followMinDistance)
            {
                _controller.Move(_movementDirection);
            }
            else
            {
                UIManager._instance.GameOver();
            }
            
        }

        private void Patrol()
        {
            
        }

        private void KillEnemy()
        {
            GameObject obj = Instantiate(destructedPrefab, transform.position, Quaternion.identity);
            Destroy(obj, 1);
            gameObject.SetActive(false);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawRay(_ray);
        }




    }
}
