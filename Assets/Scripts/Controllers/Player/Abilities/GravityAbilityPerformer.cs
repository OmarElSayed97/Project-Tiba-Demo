using System;
using UnityEngine;

namespace Controllers.Player.Abilities
{
    [RequireComponent(typeof(Rigidbody))]
    public class GravityAbilityPerformer : AbilityPerformerBase
    {

        private Rigidbody _rigidbody;
        [SerializeField] private GameObject selectionObject;

        protected void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }
        protected override void InitializeAbility()
        {
            ability = AbilityManager.Instance.AbilityConfig.GravityAbility;
        }
        
        private void OnEnable()
        {
            _rigidbody.useGravity = true;
            _rigidbody.isKinematic = true;
            selectionObject.SetActive(isAbilitySelected);
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
            selectionObject.SetActive(true);
            base.SelectedLogic();
        }

        protected override void DeselectedLogic()
        {
            selectionObject.SetActive(false);
            base.DeselectedLogic();
        }
    }
}
