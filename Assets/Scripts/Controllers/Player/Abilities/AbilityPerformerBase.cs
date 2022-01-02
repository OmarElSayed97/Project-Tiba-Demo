using System;
using UnityEngine;

namespace Controllers.Player.Abilities
{
	public abstract class AbilityPerformerBase : MonoBehaviour
	{
		protected Ability ability;
		protected bool isAbilityStarted;
		protected bool isAbilitySelected;
		public Ability Ability => ability;

		public bool IsAbilityStarted => isAbilityStarted;
		public bool IsAbilitySelected => isAbilitySelected;
		
		public event Action<AbilityPerformerBase> Started;
		public event Action<AbilityPerformerBase> Selected;
		public event Action<AbilityPerformerBase> Canceled;
		public event Action<AbilityPerformerBase> Deselected;

		
		private Action<AbilityPerformerBase> _playerStartedAction;
		private Action<AbilityPerformerBase> _playerSelectedAction;
		
		private Action<AbilityPerformerBase> _playerCanceledAction;
		private Action<AbilityPerformerBase> _playerDeselectedAction;
		protected abstract void InitializeAbility();

		protected virtual void Start()
		{
			InitializeAbility();
		}

		public void SelectAbility(Action<AbilityPerformerBase> onSelected)
		{
			if (isAbilitySelected)
				return;
			_playerSelectedAction = onSelected;
			Selected += _playerSelectedAction;
			SelectedLogic();
			isAbilitySelected = true;
		}
		public void DeselectAbility(Action<AbilityPerformerBase> onDeselected)
		{
			if (!isAbilitySelected)
				return;
			_playerDeselectedAction = onDeselected;
			Deselected += _playerDeselectedAction;
			DeselectedLogic();
			isAbilitySelected = false;
		}
		public void PerformAbility(Action<AbilityPerformerBase> onStarted, Action<AbilityPerformerBase> onCanceled)
		{
			if (isAbilityStarted)
				return;
			_playerStartedAction = onStarted;
			_playerCanceledAction = onCanceled;
			Started += _playerStartedAction;
			Canceled += _playerCanceledAction;
			AbilityStartedLogic();
			isAbilityStarted = true;
		}
		public void CancelAbility()
		{
			if (!isAbilityStarted)
				return;
			AbilityCancelLogic();
			isAbilityStarted = false;
		}
		protected virtual void SelectedLogic()
		{
			Selected?.Invoke(this);
			Selected -= _playerSelectedAction;
			_playerSelectedAction = null;
		}
		protected virtual void DeselectedLogic()
		{
			Deselected?.Invoke(this);
			Deselected -= _playerDeselectedAction;
			_playerDeselectedAction = null;
		}
		protected virtual void AbilityStartedLogic()
		{
			Started?.Invoke(this);
			Started -= _playerStartedAction;
			_playerStartedAction = null;
		}
		protected virtual void AbilityCancelLogic()
		{
			Canceled?.Invoke(this);
			Canceled -= _playerCanceledAction;
			_playerCanceledAction = null;
		}
	}
}