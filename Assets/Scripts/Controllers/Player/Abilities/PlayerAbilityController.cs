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
		
		[SerializeField, Tooltip("The radius of the ability aiming beam.")]
		private float abilityBeamRadius = 1f;

		[SerializeField, Tooltip("Aiming center")]
		private Transform aimingCenter;

		[Space(10), Header("Debug")] [SerializeField, ReadOnly]
		private AbilityPerformerBase currentAbilityPerformerBase;
		
		[SerializeField, ReadOnly] private LayerMask abilityLayer;
		[SerializeField, ReadOnly] private Ability selectedAbility;
		[SerializeField, ReadOnly] private Ability currentAbility;
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
			currentAbilityPerformerBase = null;
			currentAbilityPoints = maxAbilityPoints;
		}

		private void OnEnable()
		{
			AbilityManager.Instance.OnAbilitySelected += OnAbilitySwitchedHandler;
		}

		private void OnDisable()
		{
			if(AbilityManager.Instance)
				AbilityManager.Instance.OnAbilitySelected -= OnAbilitySwitchedHandler;
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
					if (currentAbilityPerformerBase != null)
					{
						if (currentAbilityPerformerBase != ((AbilityPerformerBase)_abilityPerformerComponent))
						{
							if (currentAbilityPerformerBase.IsAbilitySelected)
							{
								currentAbilityPerformerBase.DeselectAbility(OnAbilityDeselected);
							}	
						}
						else
						{
							return;
						}
					}
					if (!((AbilityPerformerBase)_abilityPerformerComponent).IsAbilityStarted)
					{
						currentAbilityPerformerBase = (AbilityPerformerBase)_abilityPerformerComponent;
						currentAbility = currentAbilityPerformerBase.Ability;
						currentAbilityPerformerBase.SelectAbility(OnAbilitySelected);	
					}
					
				}
			}
			else
			{
				if (currentAbilityPerformerBase != null)
				{
					currentAbilityPerformerBase.DeselectAbility(OnAbilityDeselected);
				}
				currentAbility = null;
			}
		}

		private void AbilityHandler()
		{
			switch (_inputController.performAbility)
			{
				case true when !isPerformingAbility:
				{
					if (currentAbilityPerformerBase != null)
					{
						//Perform Ability Logic
						if ((currentAbility.IsTimeBased ? currentAbility.Cost * Time.deltaTime : currentAbility.Cost ) < currentAbilityPoints && !currentAbilityPerformerBase.IsAbilityStarted)
						{
							isPerformingAbility = true;
							currentAbilityPerformerBase.PerformAbility(OnAbilityStarted, OnAbilityCanceled);
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
					currentAbilityPoints -= currentAbility.Cost * Time.deltaTime;

					if (currentAbilityPoints <= 0)
					{
						_inputController.performAbility = false;
						currentAbilityPerformerBase.CancelAbility();
					}

					break;
				}
				case false when isPerformingAbility:
				{
					if (currentAbilityPerformerBase != null)
					{
						currentAbilityPerformerBase.CancelAbility();
					}

					break;
				}
			}
		}

		private void OnAbilityStarted(AbilityPerformerBase abilityPerformerBase)
		{
			if (abilityPerformerBase.Ability != currentAbility)
			{
				Debug.LogError($"Wrong Ability Started. Expected {this.currentAbility} got {abilityPerformerBase.Ability}");
				return;
			}
				
			if (!currentAbility.IsTimeBased)
			{
				currentAbilityPoints -= currentAbility.Cost;
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
			if (currentAbilityPerformerBase != abilityPerformer) return;
			currentAbilityPerformerBase = null;
			currentAbility = null;
		}
		private void OnAbilityCanceled(AbilityPerformerBase abilityPerformer)
		{
			// selectedAbilityPerformer.Canceled -= OnAbilityCanceled;
			if (!abilityPerformer.Ability.IsTimeBased) return;
			if(abilityPerformer.IsAbilitySelected)
				abilityPerformer.DeselectAbility(null);
			isPerformingAbility = false;
			currentAbilityPerformerBase = null;
			currentAbility = null;
		}
		
		private void OnAbilitySwitchedHandler(Ability newAbility)
		{
			
			if (currentAbility is { IsTimeBased: true } && newAbility.AbilityMask != currentAbility.AbilityMask)
			{
				currentAbilityPerformerBase.CancelAbility();
			}

			selectedAbility = newAbility;
			abilityLayer = newAbility.AbilityMask;
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.DrawRay(aimingCenter.position, _abilityBeamDirection);
		}
	}
}