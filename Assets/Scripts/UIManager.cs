using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    Sprite[] portalAbilitySprites, gravityAbilitySprites;
    [SerializeField]
    Image portalIcon, gravityIcon; 

    public static UIManager _instance;

    private void Awake()
    {
        _instance = this;
    }



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

   
}
