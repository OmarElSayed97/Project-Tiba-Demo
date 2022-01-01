using UnityEngine;

namespace Controllers.Player.Abilities
{
    public class PortalAbilityPerformer : AbilityPerformerBase
    {
        [SerializeField] private PortalAbilityPerformer otherPortal;
        [SerializeField] private GameObject selectionEffect;
        [SerializeField] private GameObject openedEffect;
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
    }
}
