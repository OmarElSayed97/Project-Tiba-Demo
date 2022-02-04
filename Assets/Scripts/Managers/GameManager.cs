using Classes.Enums;
using Controllers.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
	public class GameManager : Singleton<GameManager>
	{
		#region GameEvents

		public delegate void GameEvent();

		public static event GameEvent GameReset;
		public static event GameEvent GamePaused;
		public static event GameEvent GameResumed;
		public static event GameEvent LevelReset;

		#endregion

		#region LevelEvents

		public delegate void LevelEvent();

		public static event LevelEvent LevelLoaded;
		public static event LevelEvent LevelStarted;
		public static event LevelEvent LevelFailed;
		public static event LevelEvent LevelCompleted;

		#endregion

		public eGameState GameState { get; private set; }

		public bool Tutorial { get; private set; }

		public double LevelStartTime { get; private set; }

		public double LevelEndTime { get; private set; }

		[SerializeField] private bool debug;

		protected override void OnAwakeEvent()
		{
			base.OnAwakeEvent();
			GameState = eGameState.Initialization;
			InitializeScene();
		}

		public override void Start()
		{
			base.Start();
			LoadLevel();
		}

		private void InitializeScene()
		{
			Time.timeScale = 0;
			Cursor.visible = false;
			Tutorial = true;
		}

		#region GameEvent Handlers

		public void ResetGame()
		{
			GameReset?.Invoke();
		}

		public void PauseGame()
		{
			if (GameState is not eGameState.Playing or eGameState.Paused)
				return;
			GameState = eGameState.Paused;
			GamePaused?.Invoke();
			Time.timeScale = 0;
			if(debug)
				Debug.Log("Game Paused");
		}

		public void ResumeGame()
		{
			if (GameState is eGameState.Playing or not eGameState.Paused)
				return;
			GameState = eGameState.Playing;
			Time.timeScale = 1;
			GameResumed?.Invoke();
			if(debug)
				Debug.Log("Game Resumed");
		}

		public void ResetLevel()
		{
			if (GameState is eGameState.Resetting)
				return;
			GameState = eGameState.Resetting;
			Tutorial = false;
			LevelReset?.Invoke();
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
			if(debug)
				Debug.Log("Resetting Level!");
		}

		#endregion

		#region LevelEvent Handlers

		public void LoadLevel()
		{
			if (GameState == eGameState.Loading)
				return;
			GameState = eGameState.Loading;
			InputController.Instance.GameStarted = false;
			LevelLoaded?.Invoke();
			if(debug)
				Debug.Log("Level Loaded");
			
			//TODO Make UI if there is no tutorial
			if(!Tutorial)
				StartLevel();
		}

		public void StartLevel()
		{
			if (GameState is not eGameState.Loading or eGameState.Playing)
				return;
			GameState = eGameState.Playing;
			Time.timeScale = 1;
			LevelStarted?.Invoke();
			LevelStartTime = Time.timeSinceLevelLoadAsDouble;
			InputController.Instance.GameStarted = true;
			if(debug)
				Debug.Log("Level Started");
		}

		public void FailLevel()
		{
			if (GameState is not eGameState.Playing or eGameState.GameOver)
				return;
			Time.timeScale = 0;
			LevelEndTime = Time.timeSinceLevelLoadAsDouble;
			GameState = eGameState.Finished;
			LevelFailed?.Invoke();
			if(debug)
				Debug.Log("Level Failed");
		}

		public void CompleteLevel()
		{
			if (GameState is not eGameState.Playing or eGameState.Finished)
				return;
			Time.timeScale = 0;
			LevelEndTime = Time.timeSinceLevelLoadAsDouble;
			GameState = eGameState.Finished;
			LevelCompleted?.Invoke();
			if(debug)
				Debug.Log("Level Completed");
		}

		#endregion
	}
}