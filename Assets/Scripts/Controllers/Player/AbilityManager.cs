using Controllers.Player.Abilities;
using UnityEngine;

namespace Controllers.Player
{
    public class AbilityManager : Singleton<AbilityManager>
    {
        [Header("Config")]
        [SerializeField] private AbilitiesConfig abilitiesConfig;

        public AbilitiesConfig AbilityConfig => abilitiesConfig;

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
