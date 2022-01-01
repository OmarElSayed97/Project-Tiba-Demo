using System;
using System.Collections.Generic;
using UnityEngine;

namespace Controllers.Player.Abilities
{
    public class PortalAbilityPerformer : AbilityPerformerBase
    {
        [SerializeField] private PortalAbilityPerformer otherPortal;
        [SerializeField] private GameObject selectionEffect;
        [SerializeField] private GameObject openedEffect;

        [SerializeField] private List<Transform> _teleportedObjects;
        
        protected override void InitializeAbility()
        {
            ability = AbilityManager.Instance.AbilityConfig.Portal;
            _teleportedObjects = new List<Transform>();
            SetOtherPortal(otherPortal);
        }

        public void SetOtherPortal(PortalAbilityPerformer otherNewPortal)
        {
            if (otherPortal != null)
            {
                otherPortal.Selected -= OnOtherPortalSelected;
                otherPortal.Deselected -= OnOtherPortalDeselected;
                otherPortal.Started -= OnOtherPortalStarted;
                otherPortal.Canceled -= OnOtherPortalCanceled;
            }

            otherPortal = otherNewPortal;
            otherPortal.Selected += OnOtherPortalSelected;
            otherPortal.Deselected += OnOtherPortalDeselected;
            otherPortal.Started += OnOtherPortalStarted;
            otherPortal.Canceled += OnOtherPortalCanceled;
        }

        private void OnOtherPortalSelected(AbilityPerformerBase abilityPerformerBase)
        {
            isAbilitySelected = true;
            selectionEffect.SetActive(true);
        }

        private void OnOtherPortalDeselected(AbilityPerformerBase abilityPerformer)
        {
            isAbilitySelected = false;
            selectionEffect.SetActive(false);
        }
        
        private void OnOtherPortalStarted(AbilityPerformerBase abilityPerformerBase)
        {
            isAbilityStarted = true;
            openedEffect.SetActive(true);
        }

        private void OnOtherPortalCanceled(AbilityPerformerBase abilityPerformer)
        {
            isAbilityStarted = false;
            openedEffect.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log($"{gameObject.name} Trigger Enter {other.gameObject.name}");
            
            var parent = GetOuterParent(other.gameObject.transform);
            if (!_teleportedObjects.Contains(parent))
            {
                Debug.Log($"{gameObject.name} Teleporting {parent.name}");
                parent.SendMessage("OnPortalEnter", SendMessageOptions.DontRequireReceiver);
                _teleportedObjects.Add(parent);
                otherPortal.TeleportObject(parent);
            }
            
        }

        private void OnTriggerExit(Collider other)
        {
            Debug.Log($"{gameObject.name} Trigger Exit {other.gameObject.name}");
            var parent = GetOuterParent(other.gameObject.transform);
            if (_teleportedObjects.Contains(parent))
            {
                Debug.Log($"{gameObject.name} Removing {parent.name}");
                parent.SendMessage("OnPortalExit", SendMessageOptions.DontRequireReceiver);
                _teleportedObjects.Remove(parent);
            }
        }

        private void TeleportObject(Transform teleportObj)
        {
            if (_teleportedObjects.Contains(teleportObj))
                return;
            _teleportedObjects.Add(teleportObj);
            teleportObj.position = transform.position;
        }

        private void RemoveObject(Transform removedObject)
        {
            if (_teleportedObjects.Contains(removedObject))
            {
                _teleportedObjects.Remove(removedObject);
            }
        }

        private static Transform GetOuterParent(Transform child)
        {
            // var str = $"Child Enter {child.name} - Parent ";
            while (child != null && child.parent != null && !(child.parent.CompareTag("Container")))
            {
                // Debug.Log($"Getting Parent {child.name}");
                child = child.parent;
            }

            return child;
            // str += $"{child.name}";
            // Debug.Log(str);
        }
    }
}
