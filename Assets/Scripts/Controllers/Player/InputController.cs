using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controllers.Player
{
    [RequireComponent(typeof(PlayerInput))]
    public class InputController : Singleton<InputController>
    {
        private PlayerInput _playerInput;

        private InputAction _moveAction;
        private InputAction _lookAction;
        private InputAction _fireAction;
        private InputAction _runAction;
        private InputAction _jumpAction;
        private InputAction _abilityAction;
        private InputAction _abilityOneAction;
        private InputAction _abilityTwoAction;

        public bool run;
        public bool jump;
        public bool performAbility;
        public Vector2 Move { get; private set; }
        public Vector2 Look { get; private set; }

        public event Action<int> OnAbilitySwitched;

        protected override void OnAwakeEvent()
        {
            base.OnAwakeEvent();
            _playerInput = GetComponent<PlayerInput>();
            _moveAction = _playerInput.actions["Move"];
            _runAction = _playerInput.actions["Run"];
            _jumpAction = _playerInput.actions["Jump"];
            _lookAction = _playerInput.actions["Look"];
            _abilityAction = _playerInput.actions["PerformAbility"];
            _abilityOneAction = _playerInput.actions["Ability_One"];
            _abilityTwoAction = _playerInput.actions["Ability_Two"];
        }

        private void OnEnable()
        {
            _runAction.started += RunActionOnStarted;
            _runAction.canceled += RunActionOnCanceled;
            _jumpAction.started += JumpActionOnStarted;
            _jumpAction.canceled += JumpActionOnCanceled;
            _abilityAction.started += AbilityActionOnStarted;
            _abilityAction.canceled += AbilityActionOnCanceled;
            _abilityOneAction.performed += AbilityOneActionOnPerformed;
            _abilityTwoAction.performed += AbilityTwoActionOnPerformed;
        }
        public override void OnDisable()
        {
            base.OnDisable();
            _runAction.started -= RunActionOnStarted;
            _runAction.canceled -= RunActionOnCanceled;
            _jumpAction.started -= JumpActionOnStarted;
            _jumpAction.canceled -= JumpActionOnCanceled;
            _abilityAction.started -= AbilityActionOnStarted;
            _abilityAction.canceled -= AbilityActionOnCanceled;
            _abilityOneAction.performed -= AbilityOneActionOnPerformed;
            _abilityTwoAction.performed -= AbilityTwoActionOnPerformed;
        }
        
        private void RunActionOnStarted(InputAction.CallbackContext obj)
        {
            run = true;
        }
        
        private void RunActionOnCanceled(InputAction.CallbackContext obj)
        {
            run = false;
        }
        
        private void JumpActionOnStarted(InputAction.CallbackContext obj)
        {
            jump = true;
        }
        
        private void JumpActionOnCanceled(InputAction.CallbackContext obj)
        {
            jump = false;
        }
        private void AbilityActionOnStarted(InputAction.CallbackContext obj)
        {
            performAbility = true;
        }
        private void AbilityActionOnCanceled(InputAction.CallbackContext obj)
        {
            performAbility = false;
        }
        private void AbilityOneActionOnPerformed(InputAction.CallbackContext obj)
        {
            OnAbilitySwitched?.Invoke(0);
        }
        private void AbilityTwoActionOnPerformed(InputAction.CallbackContext obj)
        {
            OnAbilitySwitched?.Invoke(1);
        }
        

        private void Update()
        {
            Move = _moveAction.ReadValue<Vector2>();
            Look = _lookAction.ReadValue<Vector2>();
        }
    }
}
