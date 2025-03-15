using UnityEngine;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.Video;
using TP;

public class CommonBuyInScreen : MonoBehaviour
{

    [Header("======================================")]

    [Header("---TEXT---")]
    [SerializeField] private TMP_Text platformInfo;
    [SerializeField] private TMP_Text entryFee;
    [SerializeField] private TMP_Text chaalLimit;
    [SerializeField] private TMP_Text potLimit;

    [Header("======================================")]

    [Header("---TOGGLES---")]
    [SerializeField] private Toggle[] toggles;
    [SerializeField] private ToggleGroup tog;


    [Header("======================================")]

    [Header("---BUTTON---")]
    [SerializeField] private Button playNowButton;
    public static CommonBuyInScreen Instance;
  


    private void Awake()
    {
        Instance = this;
        playNowButton.onClick.AddListener(() => AcceptButtonClick());

    }


    private void Update()
    {
        
    }


    private void OnEnable()
    {
        AudioListener.volume = 0;
        playNowButton.interactable = true;


        if (APIController.instance.authentication.entryAmountDetails.entryAmounts.Count <= 0)
        {
            APIController.instance.authentication.entryAmountDetails.entryAmounts = new List<int> { 1, 2, 5,10 };
            APIController.instance.authentication.entryAmountDetails.chaalLimits = new List<int> { 32, 64, 160, 320};
            APIController.instance.authentication.entryAmountDetails.potLimits = new List<int> { 100, 200, 500,1000 };
            GameController.Instance.BuyScreenData = new();

            for (int i = 0; i < APIController.instance.authentication.entryAmountDetails.entryAmounts.Count; i++)
            {

                SelectedAmountData data = new();
                data.Commission = APIController.instance.authentication.entryAmountDetails.commission;
                data.Currency = APIController.instance.userDetails.currency_type;
                data.entryFee = APIController.instance.authentication.entryAmountDetails.entryAmounts[i];
                data.ChaalAmount = APIController.instance.authentication.entryAmountDetails.chaalLimits[i];
                data.potAmount = APIController.instance.authentication.entryAmountDetails.potLimits[i];
                toggles[i].GetComponentInChildren<TMP_Text>().text = APIController.instance.authentication.entryAmountDetails.entryAmounts[i].ToString() + " " + APIController.instance.userDetails.currency_type;
                GameController.Instance.BuyScreenData.selectedAmountData.Add(data);
            }


            for (int i = 0; i < toggles.Length; i++)
            {
                toggles[i].isOn = false;
            }
            toggles[1].isOn = true;
            SetToggleOn();

        }
        else
        {
            for (int i = 0; i < APIController.instance.authentication.entryAmountDetails.entryAmounts.Count; i++)
            {
                SelectedAmountData data = new();
                data.Commission = APIController.instance.authentication.entryAmountDetails.commission;
                data.Currency = APIController.instance.userDetails.currency_type;
                data.entryFee = APIController.instance.authentication.entryAmountDetails.entryAmounts[i];
                data.ChaalAmount = APIController.instance.authentication.entryAmountDetails.chaalLimits[i];
                data.potAmount = APIController.instance.authentication.entryAmountDetails.potLimits[i];
                toggles[i].GetComponentInChildren<TMP_Text>().text = APIController.instance.authentication.entryAmountDetails.entryAmounts[i].ToString() + " " + APIController.instance.userDetails.currency_type;
                GameController.Instance.BuyScreenData.selectedAmountData.Add(data);
            }


            for (int i = 0; i < toggles.Length; i++)
            {
                toggles[i].isOn = false;
            }
            toggles[1].isOn = true;
            SetToggleOn();
        }
    }


    public void AcceptButtonClick()
    {
#if !UNITY_SERVER
        APIController.instance.CheckInternetForButtonClick((Success) =>
        {

            playNowButton.interactable = false;

            if (Success)
            {
                if (PlayerManager.localPlayer != null)
                {
                    UIController.Instance.Connecting.SetActive(true);
                    GameController.Instance.StartGameOnButtonClick();
                    this.gameObject.SetActive(false);
                }
                else
                {
          
                    UIController.Instance.FindGameWEBGL();
                    GameController.Instance.SearchOnInternetCheck = true;
                    this.gameObject.SetActive(false);
                }

                SetVolumeOn();
            }
            else
            {

                playNowButton.interactable = true;
            }


        });
#endif

    }

    public void SetToggleTextColor()
    {
       
        for(int i = 0; i < toggles.Length;i++)
        {
            if (toggles[i].isOn)
            {
                DebugHelper.Log("Toggle Text == " + toggles[i].GetComponentInChildren<TMP_Text>().text.ToString());
                          
            }
        }
    }
    public void SetVolumeOn()
    {
        DebugHelper.Log("AUDIO SOUND ============>");
        GameController.Instance.isAudioPlayStarted = true;
        AudioListener.volume = 1;
        GamePlayUI.instance.settingsPanel.MusicToggle.isOn = APIController.instance.userDetails.isBlockApiConnection ? true: APIController.instance.authentication.sound;
        GamePlayUI.instance.settingsPanel.SoundToggle.isOn = APIController.instance.userDetails.isBlockApiConnection ? true : APIController.instance.authentication.music;


    }

    public void SetToggleOn()
    {
        tog.allowSwitchOff = true;
        for (int i = 0; i < toggles.Length; i++)
        {
            toggles[i].isOn = false;
        }

        toggles[0].isOn = true;


        tog.allowSwitchOff = false;
    }

    void ChangeTextColor(int index)
    {
        for (int i = 0; i < toggles.Length; i++)
        {
            toggles[i].GetComponentInChildren<TMP_Text>().color = i == index ? new Color(0.8f, 0.9f, 0.6f, 1f) : new Color(0.2f, 0.7f, 0.7f, 1f);
        }
    }


    public void OnToggleValueChange(int toggleIndex)
    {
        ChangeTextColor(toggleIndex);
        SelectedAmountData dataToSet = GameController.Instance.BuyScreenData.selectedAmountData[toggleIndex];
        platformInfo.text = $"Platform Fee ({APIController.instance.authentication.entryAmountDetails.commission * 100}% of total Pot Amount)";
        entryFee.text =dataToSet.entryFee.ToString() + " " + APIController.instance.userDetails.currency_type;
        chaalLimit.text = dataToSet.ChaalAmount.ToString() + " " + APIController.instance.userDetails.currency_type;
        potLimit.text = dataToSet.potAmount.ToString() + " " + APIController.instance.userDetails.currency_type;
        APIController.instance.userDetails.potLimit = 1000000000;
        APIController.instance.userDetails.challLimit = dataToSet.ChaalAmount;
        APIController.instance.userDetails.bootAmount = dataToSet.entryFee;
        APIController.instance.authentication.challLimit = dataToSet.ChaalAmount;
        APIController.instance.authentication.bootAmount = dataToSet.entryFee;
       
    }
}

