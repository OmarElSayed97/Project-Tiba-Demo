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
        private InputAction _walkAction;
        private InputAction _jumpAction;
        private InputAction _abilityAction;
        private InputAction _abilityOneAction;
        private InputAction _abilityTwoAction;
        private InputAction _switchAbilityAction;

        public bool walk;
        public bool jump;
        public bool performAbility;
        public Vector2 Move { get; private set; }
        public Vector2 Look { get; private set; }

        public event Action<int> OnAbilitySwitched;

        private int currSelectedAbility;

        protected override void OnAwakeEvent()
        {
            base.OnAwakeEvent();
            _playerInput = GetComponent<PlayerInput>();
            _moveAction = _playerInput.actions["Move"];
            _walkAction = _playerInput.actions["Walk"];
            _jumpAction = _playerInput.actions["Jump"];
            _lookAction = _playerInput.actions["Look"];
            _abilityAction = _playerInput.actions["PerformAbility"];
            _abilityOneAction = _playerInput.actions["Ability_One"];
            _abilityTwoAction = _playerInput.actions["Ability_Two"];
            _switchAbilityAction = _playerInput.actions["SwitchAbility"];
            currSelectedAbility = 0;
        }

        private void OnEnable()
        {
            _walkAction.started += WalkActionOnStarted;
            _walkAction.canceled += WalkActionOnCanceled;
            _jumpAction.started += JumpActionOnStarted;
            _jumpAction.canceled += JumpActionOnCanceled;
            _abilityAction.started += AbilityActionOnStarted;
            _abilityAction.canceled += AbilityActionOnCanceled;
            _abilityOneAction.performed += AbilityOneActionOnPerformed;
            _abilityTwoAction.performed += AbilityTwoActionOnPerformed;
            _switchAbilityAction.performed += SwitchAbilityOnPerformed;
        }

       

        public override void OnDisable()
        {
            base.OnDisable();
            _walkAction.started -= WalkActionOnStarted;
            _walkAction.canceled -= WalkActionOnCanceled;
            _jumpAction.started -= JumpActionOnStarted;
            _jumpAction.canceled -= JumpActionOnCanceled;
            _abilityAction.started -= AbilityActionOnStarted;
            _abilityAction.canceled -= AbilityActionOnCanceled;
            _abilityOneAction.performed -= AbilityOneActionOnPerformed;
            _abilityTwoAction.performed -= AbilityTwoActionOnPerformed;
            _switchAbilityAction.performed -= SwitchAbilityOnPerformed;
        }
        
        private void WalkActionOnStarted(InputAction.CallbackContext obj)
        {
            walk = true;
        }
        
        private void WalkActionOnCanceled(InputAction.CallbackContext obj)
        {
            walk = false;
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


        private void SwitchAbilityOnPerformed(InputAction.CallbackContext obj)
        {
           if(currSelectedAbility == 0)
            {
                OnAbilitySwitched?.Invoke(1);
                currSelectedAbility = 1;
            }
               
           else
            {
                OnAbilitySwitched?.Invoke(0);
                currSelectedAbility = 0;
            }
                
        }

        private void Update()
        {
            Move = _moveAction.ReadValue<Vector2>();
            Look = _lookAction.ReadValue<Vector2>();
        }
    }
}
