using System;
using UnityEditor;
using UnityEngine;

namespace Controllers.Player.Abilities
{
    [CreateAssetMenu()]
    public class AbilitiesConfig : ScriptableObject
    {
        [SerializeField] private Ability gravity;
        [SerializeField] private Ability portal;

        public Ability Gravity => gravity;
        public Ability Portal => portal;
    }

    [Serializable]
    public class Ability
    {
        [SerializeField] private bool isTimeBased = false;
        [SerializeField] private float cost;
        [SerializeField] private float enabledTime;
        [SerializeField] private LayerMask abilityMask;

        public bool IsTimeBased => isTimeBased;
        public float Cost => cost;
        public float EnabledTime => enabledTime;
        public LayerMask AbilityMask => abilityMask;
    }
}
