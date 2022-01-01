using System;
using UnityEngine;

namespace Controllers.Player.Abilities
{
    [RequireComponent(typeof(Rigidbody))]
    public class GravityAbilityPerformer : AbilityPerformerBase
    {

        private Rigidbody _rigidbody;
        [SerializeField] private GameObject selectionEffect;
        private Action _abilityFixedUpdateLogic;
        private readonly Vector3 _gravity = new Vector3(0,-10,0);
        private Vector3 _downVelocity;
        private Vector3 _downDeltaPosition;
        private RaycastHit hit;
        
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
            if (_rigidbody.SweepTest(-transform.up, out hit, 4f, QueryTriggerInteraction.Ignore))
            {
                // Debug.Log($"{hit.distance}");
                if (newYValue - hit.point.y < 0.1f)
                {
                    _downVelocity = Vector3.zero;
                    _downDeltaPosition.y = hit.point.y;
                }
            }
            _rigidbody.position += _downDeltaPosition;
        }
    }
}
