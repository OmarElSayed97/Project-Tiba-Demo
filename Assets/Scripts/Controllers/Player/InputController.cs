using System;
using Classes.Enums;
using Managers;
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
        private InputAction _pauseAction;

        public bool walk;
        public bool jump;
        public bool performAbility;
        public bool GameStarted { get;  set; }
        public Vector2 Move { get; private set; }
        public Vector2 Look { get; private set; }

        public static event Action<int> OnAbilitySwitched; 
        public static event Action OnNextDialogue; //Added By Omar
        

        [HideInInspector]
        public int currSelectedAbility; //Added by Omar & To be adjusted

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
            _pauseAction = _playerInput.actions["Pause"];
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
            _pauseAction.performed += PauseActionPerformed;
            GameManager.GamePaused += OnLevelPaused;
            GameManager.GameResumed += OnLevelResumed;
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
            _pauseAction.performed -= PauseActionPerformed;
            GameManager.GamePaused -= OnLevelPaused;
            GameManager.GameResumed -= OnLevelResumed;
        }

        private void PauseActionPerformed(InputAction.CallbackContext obj)
        {
            switch (GameManager.Instance.GameState)
            {
                case eGameState.Playing:
                    GameManager.Instance.PauseGame();
                    break;
                case eGameState.Paused:
                    GameManager.Instance.ResumeGame();
                    break;
            }
        }
        
        private void OnLevelResumed()
        {
            _playerInput.SwitchCurrentActionMap("Player");
        }

        private void OnLevelPaused()
        {
            _playerInput.SwitchCurrentActionMap("UI");
        }

        private void WalkActionOnStarted(InputAction.CallbackContext obj)
        {
            if(GameStarted)
                walk = true;
        }
        
        private void WalkActionOnCanceled(InputAction.CallbackContext obj)
        {
            if (GameStarted)
                walk = false;
        }
        
        private void JumpActionOnStarted(InputAction.CallbackContext obj)
        {
            if (GameStarted)
                jump = true;
            else
                OnNextDialogue?.Invoke();
        }
        
        private void JumpActionOnCanceled(InputAction.CallbackContext obj)
        {
            if (GameStarted)
                jump = false;
        }
        private void AbilityActionOnStarted(InputAction.CallbackContext obj)
        {
            if (GameStarted)
                performAbility = true;
        }
        private void AbilityActionOnCanceled(InputAction.CallbackContext obj)
        {
            if (GameStarted)
                performAbility = false;
        }
        private void AbilityOneActionOnPerformed(InputAction.CallbackContext obj)
        {
            if (GameStarted)
                OnAbilitySwitched?.Invoke(0);
        }
        private void AbilityTwoActionOnPerformed(InputAction.CallbackContext obj)
        {
            if (GameStarted)
                OnAbilitySwitched?.Invoke(1);
        }
        
        private void SwitchAbilityOnPerformed(InputAction.CallbackContext obj)
        {
            if(currSelectedAbility == 0)
            {
                OnAbilitySwitched?.Invoke(1);
                currSelectedAbility = 1;
                // UIManager._instance.SwitchAbility(1);
            }
               
            else
            {
                OnAbilitySwitched?.Invoke(0);
                currSelectedAbility = 0;
                // UIManager._instance.SwitchAbility(0);
            }
                
        }

        private void Update()
        {
            if (GameStarted)
            {
                Move = _moveAction.ReadValue<Vector2>();
                Look = _lookAction.ReadValue<Vector2>();
            }
            
        }
    }
}
