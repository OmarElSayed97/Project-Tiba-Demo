using System;
using UnityEngine;

namespace Controllers.Player.Abilities
{
    [RequireComponent(typeof(Rigidbody))]
    public class GravityAbilityPerformer : AbilityPerformerBase
    {

        private Rigidbody _rigidbody;
        [SerializeField] private GameObject selectionEffect;

        protected void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }
        protected override void InitializeAbility()
        {
            ability = AbilityManager.Instance.AbilityConfig.Gravity;
        }
        
        private void OnEnable()
        {
            _rigidbody.useGravity = true;
            _rigidbody.isKinematic = true;
            selectionEffect.SetActive(isAbilitySelected);
        }

        protected override void AbilityStartedLogic()
        {
            _rigidbody.isKinematic = false;
            base.AbilityStartedLogic();
        }

        protected override void AbilityCancelLogic()
        {
            _rigidbody.isKinematic = true;
            _rigidbody.velocity = Vector3.zero;
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
    }
}
