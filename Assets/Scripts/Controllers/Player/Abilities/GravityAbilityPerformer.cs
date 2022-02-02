using System;
using Cinemachine;
using UnityEngine;

namespace Controllers.Player.Abilities
{
    [RequireComponent(typeof(Rigidbody))]
    public class GravityAbilityPerformer : AbilityPerformerBase
    {

        private Rigidbody _rigidbody;
        [SerializeField] private GameObject selectionEffect;
        [SerializeField] private CinemachineImpulseSource impulseSource;
        private Action _abilityFixedUpdateLogic;
        private readonly Vector3 _gravity = new Vector3(0,-10,0);
        private Vector3 _downVelocity;
        private Vector3 _downDeltaPosition;
        private RaycastHit _hit;
        private bool _collisionDetected = false;
        
        protected void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }
        protected override void InitializeAbility()
        {
            ability = AbilityManager.Instance.AbilityConfig.Gravity;
            _downVelocity = Vector3.zero;
            _abilityFixedUpdateLogic = () => { };
        }
        
        private void OnEnable()
        {
            _rigidbody.useGravity = false;
            _rigidbody.isKinematic = true;
            _abilityFixedUpdateLogic = () => { };
            selectionEffect.SetActive(isAbilitySelected);
        }

        private void FixedUpdate()
        {
            _abilityFixedUpdateLogic();
        }

        protected override void AbilityStartedLogic()
        {
            _abilityFixedUpdateLogic = ApplyGravity;
            base.AbilityStartedLogic();
            AudioManager._instance.Play("Gravity");
        }

        protected override void AbilityCancelLogic()
        {
            _abilityFixedUpdateLogic = () => { };
            _downVelocity= Vector3.zero;
            base.AbilityCancelLogic();
        }
        protected override void SelectedLogic()
        {
            selectionEffect.SetActive(true);
            base.SelectedLogic();
        }

        protected override void DeselectedLogic()
        {
            selectionEffect.SetActive(false);
            base.DeselectedLogic();
        }

        private void ApplyGravity()
        {
            var fixedDeltaTime = Time.fixedDeltaTime;
            _downVelocity += _gravity * fixedDeltaTime;
            _downDeltaPosition = _downVelocity * fixedDeltaTime;
            var newYValue = _downDeltaPosition.y + _rigidbody.position.y;
            if (_rigidbody.SweepTest(-transform.up, out _hit, 1.5f, QueryTriggerInteraction.Ignore))
            {
                // Debug.Log($"{hit.distance}");
                if (newYValue - _hit.point.y < 0.05f)
                {
                    _downVelocity = Vector3.zero;
                    _downDeltaPosition.y = _hit.point.y + 0.05f;
                    if (!_collisionDetected)
                    {
                        impulseSource.GenerateImpulse();    
                        // Debug.Log("Impulse");
                    }
                    _collisionDetected = true;
                }
            }
            else
            {
                _collisionDetected = false;
            }
            
            if(_downDeltaPosition.y < 0)
                _rigidbody.position += _downDeltaPosition;
        }
    }
}
