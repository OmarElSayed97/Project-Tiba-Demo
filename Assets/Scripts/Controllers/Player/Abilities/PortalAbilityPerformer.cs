using UnityEngine;

namespace Controllers.Player.Abilities
{
    public class PortalAbilityPerformer : AbilityPerformerBase
    {
        [SerializeField] private PortalAbilityPerformer otherPortal;
        [SerializeField] private GameObject selectionObject;
        [SerializeField] private GameObject openedObject;
        protected override void InitializeAbility()
        {
            ability = AbilityManager.Instance.AbilityConfig.Portal;
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
            selectionObject.SetActive(true);
        }

        private void OnOtherPortalDeselected(AbilityPerformerBase abilityPerformer)
        {
            isAbilitySelected = false;
            selectionObject.SetActive(false);
        }
        
        private void OnOtherPortalStarted(AbilityPerformerBase abilityPerformerBase)
        {
            isAbilityStarted = true;
            openedObject.SetActive(true);
        }

        private void OnOtherPortalCanceled(AbilityPerformerBase abilityPerformer)
        {
            isAbilityStarted = false;
            openedObject.SetActive(false);
        }
    }
}
