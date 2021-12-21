using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controllers.Player.Abilities
{
	[RequireComponent(typeof(InputController))]
	public class PlayerAbilityController : MonoBehaviour
	{
		[Header("Ability Settings")] [SerializeField]
		private float maxAbilityPoints = 100;

		[SerializeField, Tooltip("The max distance at which the player can a activate an ability at.")]
		private float maxActivationDistance = 10f;

		[SerializeField, Tooltip("The layer for the ability affected objects.")]
		private LayerMask abilityLayer;

		[SerializeField, Tooltip("The radius of the ability aiming beam.")]
		private float abilityBeamRadius = 1f;

		[SerializeField, Tooltip("Aiming center")]
		private Transform aimingCenter;

		[Space(10), Header("Debug")] [SerializeField, ReadOnly]
		private AbilityPerformerBase selected;

		[SerializeField, ReadOnly] private Ability selectedAbility;
		[SerializeField, ReadOnly] private float currentAbilityPoints;
		[SerializeField, ReadOnly] private bool isPerformingAbility;
		[SerializeField, ReadOnly] private bool isAbilityStarted;

		private InputController _inputController;
		private CursorController _cursorController;
		private RaycastHit _abilityBeamHit;
		private Vector3 _abilityBeamDirection;
		private Component _abilityPerformerComponent;
		private void Awake()
		{
			_inputController = GetComponent<InputController>();
		}

		private void Start()
		{
			//Cache the cursor controller
			_cursorController = CursorController.Instance;
			selected = null;
			currentAbilityPoints = maxAbilityPoints;
		}

		private void Update()
		{
			AbilityFinder();
			AbilityHandler();
		}

		private void AbilityFinder()
		{
			//Skip finder if we already performing an ability
			if (isPerformingAbility)
				return;
			var aimingCenterPosition = aimingCenter.position;

			_abilityBeamDirection = _cursorController.CursorWorldPosition - aimingCenterPosition;
			_abilityBeamDirection.z = 0;
			
			if (Physics.BoxCast(aimingCenterPosition, Vector3.one * abilityBeamRadius, _abilityBeamDirection, out _abilityBeamHit,
				Quaternion.identity, maxActivationDistance, abilityLayer))
			{
				if (_abilityBeamHit.collider.gameObject.TryGetComponent(typeof(AbilityPerformerBase), out _abilityPerformerComponent))
				{
					if (selected != null)
					{
						if (selected != ((AbilityPerformerBase)_abilityPerformerComponent))
						{
							if (selected.IsAbilitySelected)
							{
								selected.DeselectAbility(OnAbilityDeselected);
							}	
						}
						else
						{
							return;
						}
					}
					if (!((AbilityPerformerBase)_abilityPerformerComponent).IsAbilityStarted)
					{
						selected = (AbilityPerformerBase)_abilityPerformerComponent;
						selectedAbility = selected.Ability;
						selected.SelectAbility(OnAbilitySelected);	
					}
					
				}
			}
			else
			{
				if (selected != null)
				{
					selected.DeselectAbility(OnAbilityDeselected);
				}
				selectedAbility = null;
			}
		}

		private void AbilityHandler()
		{
			switch (_inputController.performAbility)
			{
				case true when !isPerformingAbility:
				{
					if (selected != null)
					{
						//Perform Ability Logic
						if ((selectedAbility.IsTimeBased ? selectedAbility.Cost * Time.deltaTime : selectedAbility.Cost ) < currentAbilityPoints && !selected.IsAbilityStarted)
						{
							isPerformingAbility = true;
							selected.PerformAbility(OnAbilityStarted, OnAbilityCanceled);
						}
					}
					else
					{
						//Reset perform Ability Input
						_inputController.performAbility = false;
					}

					break;
				}
				case true when isPerformingAbility && isAbilityStarted:
				{
					currentAbilityPoints -= selectedAbility.Cost * Time.deltaTime;

					if (currentAbilityPoints <= 0)
					{
						_inputController.performAbility = false;
						selected.CancelAbility();
					}

					break;
				}
				case false when isPerformingAbility:
				{
					if (selected != null)
					{
						selected.CancelAbility();
					}

					break;
				}
			}
		}

		private void OnAbilityStarted(AbilityPerformerBase abilityPerformerBase)
		{
			if (abilityPerformerBase.Ability != selectedAbility)
			{
				Debug.LogError($"Wrong Ability Started. Expected {this.selectedAbility} got {abilityPerformerBase.Ability}");
				return;
			}
				
			if (!selectedAbility.IsTimeBased)
			{
				currentAbilityPoints -= selectedAbility.Cost;
				isPerformingAbility = false;
			}
			else
			{
				isAbilityStarted = true;
			}
		}

		private void OnAbilitySelected(AbilityPerformerBase abilityPerformerBase)
		{
			
		}

		private void OnAbilityDeselected(AbilityPerformerBase abilityPerformer)
		{
			if (selected != abilityPerformer) return;
			selected = null;
			selectedAbility = null;
		}
		private void OnAbilityCanceled(AbilityPerformerBase abilityPerformer)
		{
			// selectedAbilityPerformer.Canceled -= OnAbilityCanceled;
			if (!abilityPerformer.Ability.IsTimeBased) return;
			if(abilityPerformer.IsAbilitySelected)
				abilityPerformer.DeselectAbility(null);
			isPerformingAbility = false;
			selected = null;
			selectedAbility = null;
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.DrawRay(aimingCenter.position, _abilityBeamDirection);
		}
	}
}