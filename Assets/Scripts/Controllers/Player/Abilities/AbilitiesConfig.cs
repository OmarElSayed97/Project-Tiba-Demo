using System;
using UnityEditor;
using UnityEngine;

namespace Controllers.Player.Abilities
{
    [CreateAssetMenu()]
    public class AbilitiesConfig : ScriptableObject
    {
        [SerializeField] private Ability gravityAbility;
        [SerializeField] private Ability portalAbility;
        
        public Ability GravityAbility => gravityAbility;
        public Ability PortalAbility => portalAbility;
    }

    [Serializable]
    public class Ability
    {
        [SerializeField] private bool isTimeBased = false;
        [SerializeField] private float cost;

        public bool IsTimeBased => isTimeBased;
        public float Cost => cost;
    }
}
