using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Controllers.Player;
using System;
using UnityEngine.SceneManagement;
using TMPro;
using Enums;
public class UIManager : MonoBehaviour
{
    [SerializeField]
    Sprite[] portalAbilitySprites, gravityAbilitySprites;
    [SerializeField]
    Image portalIcon, gravityIcon;
    [SerializeField]
    public Image energySource;
    string d1, d2, d3,d4;
    [SerializeField]
    TextMeshProUGUI dialogueBox;
    int currDialogue;
    public static UIManager _instance;
    [HideInInspector]
    [SerializeField]
    GameObject DialoguePanel, HUDPanel, GameOverPanel, EndGamePanel,TutorialPanel;
    float finishingTime,minutes,seconds,milliseconds;
    bool startCounting;
    [SerializeField]
    TextMeshProUGUI finishingTimeText;
    private void Awake()
    {
        _instance = this;
        AssignText();
        dialogueBox.text = d1;
    }

    private void OnEnable()
    {
        InputController.Instance.OnNextDialogue += GetNextDialogue;
    }

    //private void OnDisable()
    //{
    //    if(UIManager._instance!=null)
    //        InputController.Instance.OnNextDialogue -= GetNextDialogue;
    //}


    
    public void SwitchAbility(int abilityNumber)
    {
        if(abilityNumber == 1)
        {
            portalIcon.sprite = portalAbilitySprites[1];
            gravityIcon.sprite = gravityAbilitySprites[0];
        }
        else if(abilityNumber == 0)
        {
            portalIcon.sprite = portalAbilitySprites[0];
            gravityIcon.sprite = gravityAbilitySprites[1];
        }

        AudioManager._instance.Play("Select");
    }

    void AssignText()
    {
         d1 = ArabicFixerTool.FixLine("الناس كلها في سوق المدينة  كانت مقتنعة إن ده يوم القيامة ");
         d2 = ArabicFixerTool.FixLine("بس البنت اللي قابلتها كان كلامها غير كده ");
         d3 = ArabicFixerTool.FixLine("أنا لازم أدخل المعبد وأكتشف اللي حصل جواه");
        d4 = ArabicFixerTool.FixLine(" أكيد هلاقي أجوبة هناك");
    }

    private void Update()
    {
        if (startCounting)
        {
            finishingTime += Time.deltaTime;
           
        }
            

    }
    private void GetNextDialogue()
    {
        if (currDialogue == 0)
        {
            TutorialPanel.SetActive(false);
            currDialogue++;
        }
        else if(currDialogue == 1)
        {
            dialogueBox.text = d2;
            currDialogue++;
        }
        else if (currDialogue == 2)
        {
            dialogueBox.text = d3;
            currDialogue++;
        }
        else if (currDialogue == 3)
        {
            dialogueBox.text = d4;
            currDialogue++;
        }
        else if (currDialogue == 4)
        {
            InputController.Instance.gameStarted = true;
            startCounting = true;
            DialoguePanel.SetActive(false);
            HUDPanel.SetActive(true);

        }

    }



    public void GameOver()
    {
        if (GameOverPanel.activeSelf)
            return;
        Cursor.visible = true;
        Time.timeScale = 0;
        GameOverPanel.SetActive(true);
    }

    public void EndGame()
    {
        Cursor.visible = true;
        EndGamePanel.SetActive(true);
       
        minutes = Mathf.FloorToInt(finishingTime / 60);
        seconds = Mathf.FloorToInt(finishingTime % 60);
        milliseconds = finishingTime % 1;
        string millisecondsString = milliseconds.ToString("f3");
        finishingTimeText.text = minutes + " min, " + seconds + " seconds & " + millisecondsString.Trim('.','0') + " ms";
        Time.timeScale = 0;
    }


    public void RestartLevel()
    {
        Cursor.visible = false;
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
