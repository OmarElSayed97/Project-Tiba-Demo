using System;
using Controllers.Player.Abilities;
using UnityEngine;

namespace Controllers.Player
{
    public class AbilityManager : Singleton<AbilityManager>
    {
        [Header("Config")]
        [SerializeField] private AbilitiesConfig abilitiesConfig;

        [SerializeField] private Ability selectedAbility;
        private int _currentSwitchedAbilityIndex = -1;
        public event Action<Ability> OnAbilitySwitched;
        public AbilitiesConfig AbilityConfig => abilitiesConfig;
        public Ability SelectedAbility => selectedAbility;
        
        private void OnEnable()
        {
            InputController.Instance.OnAbilitySwitched += AbilitySwitchedHandler;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            if(!InputController.IsInstanceNull)
                InputController.Instance.OnAbilitySwitched -= AbilitySwitchedHandler;
        }
        
        private void AbilitySwitchedHandler(int abilityIndex)
        {
            if (abilityIndex == _currentSwitchedAbilityIndex)
                return;
            
            selectedAbility = abilityIndex switch
            {
                0 => abilitiesConfig.Gravity,
                1 => abilitiesConfig.Portal,
                _ => selectedAbility
            };
            _currentSwitchedAbilityIndex = abilityIndex;
            Debug.Log($"Ability Invoked {abilityIndex} {selectedAbility}");
            OnAbilitySwitched?.Invoke(selectedAbility);   
        }

    }            
}





