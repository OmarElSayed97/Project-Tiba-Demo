using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Controllers.Player;
using TMPro;
using Managers;

public class UIManager : Singleton<UIManager>
{
	[SerializeField] Sprite[] portalAbilitySprites, gravityAbilitySprites;
	[SerializeField] Image portalIcon, gravityIcon;
	[SerializeField] public Image energySource;
	[SerializeField] TextMeshProUGUI dialogueBox;
	
	private List<string> _dialogue;
	private int _currDialogue;
	
	[SerializeField] GameObject DialoguePanel, HUDPanel, GameOverPanel, EndGamePanel, TutorialPanel, pauseMenuPanel;
	float finishingTime, minutes, seconds, milliseconds;

	[SerializeField] TextMeshProUGUI finishingTimeText;

	protected override void OnAwakeEvent()
	{
		AssignText();
		dialogueBox.text = _dialogue[0];
		OnLevelReset();
	}

	private void OnEnable()
	{
		InputController.OnNextDialogue += GetNextDialogue;
		InputController.OnAbilitySwitched += SwitchAbility;
		GameManager.LevelLoaded += OnLevelLoaded;
		GameManager.LevelFailed += OnLevelFailed;
		GameManager.LevelStarted += OnLevelStarted;
		GameManager.LevelCompleted += OnLevelCompleted;
		GameManager.LevelReset += OnLevelReset;
		GameManager.GamePaused += OnGamePaused;
		GameManager.GameResumed += OnGameResumed;
	}


	public override void OnDisable()
	{
		base.OnDisable();
		if (!InputController.IsInstanceNull)
		{
			InputController.OnNextDialogue -= GetNextDialogue;
			InputController.OnAbilitySwitched -= SwitchAbility;
		}

		if (!GameManager.IsInstanceNull)
		{
			GameManager.LevelLoaded -= OnLevelLoaded;
			GameManager.LevelStarted -= OnLevelStarted;
			GameManager.LevelFailed -= OnLevelFailed;
			GameManager.LevelCompleted -= OnLevelCompleted;
			GameManager.LevelReset -= OnLevelReset;
			GameManager.GamePaused -= OnGamePaused;
			GameManager.GameResumed -= OnGameResumed;
		}
	}


	private void SwitchAbility(int abilityNumber)
	{
		//TODO Listen to the AbilityManager Event Instead
		switch (abilityNumber)
		{
			case 1:
				portalIcon.sprite = portalAbilitySprites[1];
				gravityIcon.sprite = gravityAbilitySprites[0];
				break;
			case 0:
				portalIcon.sprite = portalAbilitySprites[0];
				gravityIcon.sprite = gravityAbilitySprites[1];
				break;
		}
	}

	private void AssignText()
	{
		// d1 = ArabicFixerTool.FixLine("الناس كلها في سوق المدينة  كانت مقتنعة إن ده يوم القيامة ");
		// d2 = ArabicFixerTool.FixLine("بس البنت اللي قابلتها كان كلامها غير كده ");
		// d3 = ArabicFixerTool.FixLine("أنا لازم أدخل المعبد وأكتشف اللي حصل جواه");
		// d4 = ArabicFixerTool.FixLine(" أكيد هلاقي أجوبة هناك");
		_dialogue = new List<string>
		{
			"الناس كلها في سوق المدينة كانت مقتنعةإن ده يوم القيامة",
			"بس البنت اللي قابلتها كان كلامهاغير كده",
			"أنا لازم أدخل المعبد وأكتشف اللي حصل جواه",
			" أكيد هلاقي أجوبة هناك"
		};
	}

	private void GetNextDialogue()
	{
		if (!GameManager.Instance.Tutorial)
		{
			GameManager.Instance.LoadLevel();
			return;
		}
		
		TutorialPanel.SetActive(false);
		DialoguePanel.SetActive(true);
		
		if (_currDialogue < _dialogue.Count)
		{
			dialogueBox.text = _dialogue[_currDialogue];
			_currDialogue++;
		}
		else
		{
			GameManager.Instance.StartLevel();
		}
		
	}
	
	private void OnGamePaused()
	{
		pauseMenuPanel.SetActive(true);
	}

	private void OnGameResumed()
	{
		pauseMenuPanel.SetActive(false);
	}


	private void OnLevelReset()
	{
		DialoguePanel.SetActive(false);
		TutorialPanel.SetActive(false);
		EndGamePanel.SetActive(false);
		HUDPanel.SetActive(false);
	}
	
	private void OnLevelLoaded()
	{ 
		TutorialPanel.gameObject.SetActive(GameManager.Instance.Tutorial);
		DialoguePanel.gameObject.SetActive(GameManager.Instance.Tutorial);
		_currDialogue = 0;
	}
	
	private void OnLevelStarted()
	{
		DialoguePanel.SetActive(false);
		HUDPanel.SetActive(true);
	}

	private void OnLevelFailed()
	{
		Cursor.visible = true;
		GameOverPanel.SetActive(true);
	}

	private void OnLevelCompleted()
	{
		Cursor.visible = true;
		EndGamePanel.SetActive(true);
		finishingTime = (float)(GameManager.Instance.LevelEndTime - GameManager.Instance.LevelStartTime);
		minutes = Mathf.FloorToInt(finishingTime / 60);
		seconds = Mathf.FloorToInt(finishingTime % 60);
		milliseconds = finishingTime % 1;
		var millisecondsString = milliseconds.ToString("f3");
		finishingTimeText.text = minutes + " min, " + seconds + " seconds & " + millisecondsString.Trim('.', '0') + " ms";
	}

	public void ResumeGame()
	{
		GameManager.Instance.ResumeGame();
	}

	public void RestartLevel()
	{
	    Cursor.visible = false;
	    GameManager.Instance.ResetLevel();
	}

	public void Quit()
	{
		//TODO Remove this from here
		Application.Quit();
	}
}