using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TP;
using TMPro;
using UnityEngine.UI;

public class TeenPattiTutorial : SingletonMonoBehaviour<TeenPattiTutorial>
{
    [System.Serializable]
    public struct PlayerCards
    {
        public CardData[] playerCards;
    }
    // Start is called before the first frame update
    public Sprite emptySprite;

    public List<PlayerUI> UIDetails;
    public GameObject tablePot_Obj, gameInfo_Obj, dealerMessage_Obj, gameHud_Obj, seeButton_Obj, handStrengthMeater_Obj;
    public GameObject blindPannel, seePannel, incrementPannel, challPannel, showPannel, sideShowPannel, handStrengthMeterPannel, showPanel, WinnerPanelIngame, WinnerPanelOutside, Exitpanel;
    public TextMeshProUGUI tablePot_Txt, dealerMessage_Text, blindMsg_Txt, blindVal_Txt, blindMsg1_Txt, blindVal1_Txt, challMsg1_Txt, challVal1_Txt;
    public TextMeshProUGUI sideShowSenderName, sideShowSeconds;
    public Button sideShowAccept_Btn;
    public UIHandler Rules;
    public ScrollRect scroll;
    [SerializeField]
    public PlayerCards[] playerCardDeatils;
    float currentBoot = 0;
    void Start()
    {

    }
    private void OnEnable()
    {
        hasClickInfo = false;
        handStrengthMeater_Obj.SetActive(false);
        dealerMessage_Obj.SetActive(false);
        challCount = 0;
        tablePot_Obj.SetActive(false);
        blindPannel.SetActive(false);
        seePannel.SetActive(false);
        incrementPannel.SetActive(false);
        challPannel.SetActive(false);
        showPannel.SetActive(false);
        sideShowPannel.SetActive(false);
        seeButton_Obj.SetActive(false);
        WinnerPanelIngame.SetActive(false);
        Exitpanel.SetActive(false);
        StartCoroutine(TutorialSteps1());

    }

    public void OnclickRestartButton()
    {
        hasClickInfo = false;
        challCount = 0;
        tablePot_Obj.SetActive(false);
        blindPannel.SetActive(false);
        seePannel.SetActive(false);
        incrementPannel.SetActive(false);
        challPannel.SetActive(false);
        showPannel.SetActive(false);
        sideShowPannel.SetActive(false);
        seeButton_Obj.SetActive(false);
        Exitpanel.SetActive(false);
        StartCoroutine(TutorialSteps1());
        WinnerPanelIngame.SetActive(false);
    }

    public void OnClickSkipTutorial()
    {

        StopAllCoroutines();
        Rules.HideMe();
      
        gameObject.SetActive(false);

    }

    public void ShowExitPanel()
    {
        Exitpanel.SetActive(true);
    }
    public void OnFinishButton()
    {
        this.gameObject.SetActive(false);
        Debug.LogError(CommonFunctions.Instance.GetGullakIntroShowedOnce());
        if (CommonFunctions.Instance.GetGullakIntroShowedOnce() == 1)
        {
            WinnerPanelOutside.SetActive(true);
            

        }
        else
        {
            WinnerPanelOutside.SetActive(false);
        }
    }
    IEnumerator PlayerTurn(int index)
    {
        int count = 0;
        int FixedCount = Random.Range(5, 10);
        for (float i = 0; i < 1; i = i + .01f)
        {
            UIDetails[index].UpdateTimer(i);
            yield return new WaitForSeconds(.1f);
            count += 1;
            if (count == FixedCount)
                break;
        }

    }
    IEnumerator TutorialSteps1()
    {
        UIDetails[0].InitTutorial();
        yield return new WaitForSeconds(.5f);
        ShowDealerMessage("Welcome to Teenpatti Gold tutorial", 4);
        yield return new WaitForSeconds(5);
        UIDetails[0].InitUIForTutorial();
        yield return new WaitForSeconds(1);
        tablePot_Obj.SetActive(false);
        gameInfo_Obj.SetActive(true);
        yield return new WaitForSeconds(7);
        if (!hasClickInfo)
            StartCoroutine(Split());
    }
    bool hasClickInfo = false;
    public void OnclickOKButton()
    {
        StartCoroutine(Split());
    }
    public IEnumerator Split()
    {
        hasClickInfo = true;
        UIDetails[1].InitUIForTutorial();
        UIDetails[2].InitUIForTutorial();
        gameInfo_Obj.SetActive(false);
        for (int i = 5; i > 0; i--)
        {
     
            ShowDealerMessage("Game starts in " + i + " seconds");
            yield return new WaitForSeconds(1);
        }
        //  ShowDealerMessage("Game Started");
        for (int i = 0; i < 3; i++)
        {
            foreach (PlayerUI playerUI in UIDetails)
            {
                playerUI.DealCard(i);
                yield return new WaitForSeconds(.2f);
            }
        }
        ShowDealerMessage("Your turn to play");
        SetBlindValue("BLIND", .5f);
        ShowHud();
        seeButton_Obj.SetActive(true);
        foreach (PlayerUI playerUI in UIDetails)
            playerUI.SetInfoText("BLIND");
        yield return StartCoroutine(PlayerTurn(0));
        blindPannel.SetActive(true);
    }

    public void BlindButton()
    {
        StartCoroutine(TutorialSteps2());

    }
    IEnumerator TutorialSteps2()
    {
        ShowDealerMessage(UIDetails[0].GetPlayerName() + " played BLIND");
        blindPannel.SetActive(false);
        HideHud();
        UIDetails[0].GiveAmountToPotTutorial(.5f, .5f, 999.5f);
        yield return new WaitForSeconds(1);
        yield return StartCoroutine(PlayerTurn(1));
        ShowDealerMessage(UIDetails[1].GetPlayerName() + " played BLIND");
        UIDetails[1].GiveAmountToPotTutorial(.5f, 1f, 999.5f);
        yield return new WaitForSeconds(1);
        yield return StartCoroutine(PlayerTurn(2));
        ShowDealerMessage(UIDetails[2].GetPlayerName() + " played BLIND");
        UIDetails[2].GiveAmountToPotTutorial(1f, 2f, 999.5f);
        yield return new WaitForSeconds(1);

        ShowDealerMessage("Your turn to play");
        SetBlindValue("BLIND", 1);
        ShowHud();
        yield return StartCoroutine(PlayerTurn(0));
        seePannel.SetActive(true);

    }
    public void SeeButtonClick()
    {
        StartCoroutine(TutorialSteps3());
    }
    IEnumerator TutorialSteps3()
    {
        seePannel.SetActive(false);
        seeButton_Obj.SetActive(false);
        ShowDealerMessage(UIDetails[0].GetPlayerName() + " has seen their cards");
        UIDetails[0].SetInfoText(" SEEN");
        currentBoot = 2;
        SetBlindValue("CHAAL", 2);
        yield return StartCoroutine(UIDetails[0].SeeCardTutorial(playerCardDeatils[0].playerCards));
        handStrengthMeater_Obj.SetActive(true);
        handStrengthMeterPannel.SetActive(true);
        yield return new WaitForSeconds(2);
        handStrengthMeterPannel.SetActive(false);
        yield return new WaitForSeconds(1);
        Rules.ShowMe();
        yield return new WaitForSeconds(0.5f);
        for (float i = 1; i > 0; i -= 0.01f)
        {
            scroll.verticalNormalizedPosition = i;
            yield return new WaitForSeconds(.01f);

        }

        yield return new WaitForSeconds(0.5f);
        for (float i = 0; i < 1; i += 0.01f)
        {
            scroll.verticalNormalizedPosition = i;
            yield return new WaitForSeconds(.01f);

        }
        scroll.verticalNormalizedPosition = 1;
        yield return new WaitForSeconds(0.5f);
        Rules.HideMe();
        ShowDealerMessage("You have trail sequence, bet on it");
        yield return new WaitForSeconds(1.5f);
        challPannel.SetActive(true);

    }
    int challCount = 0;
    public void IncrementButtonClick()
    {
        incrementPannel.SetActive(false);
        currentBoot = currentBoot * 2;
        SetBlindValue("CHAAL", currentBoot);
        if (challCount > 0)
            challPannel.SetActive(true);


    }

    public void ChallButtonClick()
    {
        if (challCount == 0)
        {
            StartCoroutine("TutorialSteps4");

            challPannel.SetActive(false);
        }
        else
        {

            StartCoroutine(TutorialSteps6());
        }
        challCount += 1;
    }
    IEnumerator TutorialSteps4()
    {
        ShowDealerMessage(UIDetails[0].GetPlayerName() + " Played CHAAL");
        challPannel.SetActive(false);
        HideHud();
        UIDetails[0].GiveAmountToPotTutorial(4f, 6f, 995.5f);
        yield return new WaitForSeconds(1);
        ShowDealerMessage(UIDetails[1].GetPlayerName() + " has seen their cards");
        UIDetails[1].SetInfoText("SEEN");
        yield return new WaitForSeconds(1);

        yield return StartCoroutine(PlayerTurn(1));
        ShowDealerMessage(UIDetails[1].GetPlayerName() + " has requested SideShow");
        UIDetails[1].GiveAmountToPotTutorial(4f, 10f, 995.5f);
        yield return new WaitForSeconds(.5f);
        sideShowAccept_Btn.interactable = true;
        sideShowSeconds.text = "10 sec";
        sideShowSenderName.text = UIDetails[1].GetPlayerName();
        sideShowPannel.SetActive(true);
        for (int i = 9; i > 5; i--)
        {
            sideShowSeconds.text = i + " sec";
            yield return new WaitForSeconds(1);
        }
        sideShowAccept_Btn.interactable = true;
    }

    float potAmount = 0;
    float profileAmount = 0;
    IEnumerator TutorialSteps7()
    {
        yield return new WaitForSeconds(1);
        yield return StartCoroutine(UIDetails[2].SeeCardTutorial(playerCardDeatils[2].playerCards));
        yield return new WaitForSeconds(2);
        UIDetails[2].SetInfoText("LOSS");
        UIDetails[0].SetInfoText("WINNER");
        UIDetails[0].SetWinText();
        UIDetails[0].GetAmountFromPotTutorial(potAmount, profileAmount + potAmount);
        yield return new WaitForSeconds(4);
       // UIDetails[0].ResetCard(true);
     //   UIDetails[2].ResetCard(true);
        handStrengthMeater_Obj.SetActive(false);
        UIDetails[0].CloseWinnerBanner();
        yield return new WaitForSeconds(1);
        WinnerPanelIngame.SetActive(true);

    }
    IEnumerator TutorialSteps8()
    {
        yield return new WaitForSeconds(.5f);
        showPanel.SetActive(true);
        ShowHud();
    }

    public void OnclickShowButton()
    {
        StartCoroutine(ShowButtonClick());
    }
    IEnumerator ShowButtonClick()
    {
        showPanel.SetActive(false);
        potAmount += 16;
        profileAmount -= 16;
        UIDetails[0].GiveAmountToPotTutorial(16f, potAmount, profileAmount);
        yield return new WaitForSeconds(1);
        StartCoroutine(TutorialSteps7());
        HideHud();
    }
    IEnumerator TutorialSteps6()
    {
        ShowDealerMessage(UIDetails[0].GetPlayerName() + " played CHAAL");
        challPannel.SetActive(false);
        HideHud();
        potAmount += 16;
        profileAmount -= 16;
        UIDetails[0].GiveAmountToPotTutorial(16f, potAmount, profileAmount);
        yield return new WaitForSeconds(1);
        yield return StartCoroutine(PlayerTurn(2));
        ShowDealerMessage(UIDetails[2].GetPlayerName() + " played CHAAL");
        potAmount += 16;
        UIDetails[2].GiveAmountToPotTutorial(16f, potAmount, 995.5f);
        yield return new WaitForSeconds(1);
        if (potAmount > 40)
        {
            StartCoroutine(TutorialSteps8());
            yield break;
        }
        ShowHud();
        yield return StartCoroutine(PlayerTurn(0));
        challPannel.SetActive(true);
    }
    public void SideShowButtonClick()
    {
        StopCoroutine("TutorialSteps4");
        sideShowPannel.SetActive(false);
        ShowDealerMessage(UIDetails[0].GetPlayerName() + " Accept SideShow");
        StartCoroutine(TutorialSteps5());
    }
    IEnumerator TutorialSteps5()
    {
        yield return StartCoroutine(UIDetails[1].SeeCardTutorial(playerCardDeatils[1].playerCards));
        yield return new WaitForSeconds(1);
        ShowDealerMessage(UIDetails[0].GetPlayerName() + " Won SideShow");
        yield return new WaitForSeconds(1);
        UIDetails[1].SetInfoText("PACKED");
        yield return new WaitForSeconds(.5f);
      //  UIDetails[1].ResetCard(true);

        ShowDealerMessage(UIDetails[2].GetPlayerName() + " has seen their cards");
        UIDetails[2].SetInfoText("SEEN");
        yield return new WaitForSeconds(1);

        yield return StartCoroutine(PlayerTurn(2));
        ShowDealerMessage(UIDetails[2].GetPlayerName() + " played CHAAL");
        UIDetails[2].GiveAmountToPotTutorial(8f, 18f, 995.5f);
        yield return new WaitForSeconds(1);
        currentBoot = 8;
        SetBlindValue("CHAAL", 8);
        ShowHud();
        yield return StartCoroutine(PlayerTurn(0));
        profileAmount = 995.5f;
        potAmount = 18;
        incrementPannel.SetActive(true);

    }
    void ShowDealerMessage(string message, float delay = 1.5f)
    {
        dealerMessage_Text.text = "";
        dealerMessage_Obj.SetActive(true);
        dealerMessage_Text.text = message;
        dealerMessage_Text.text = message + " ";

        CancelInvoke("HideDealerMessage");
        Invoke("HideDealerMessage", delay);
    }
    void HideDealerMessage()
    {
        dealerMessage_Obj.SetActive(false);
    }
    void ShowHud()
    {
        gameHud_Obj.GetComponent<Animator>().Play("ShowGameHud");
    }
    void HideHud()
    {
        gameHud_Obj.GetComponent<Animator>().Play("HideGameHud");
    }
    void SetBlindValue(string status, float value)
    {
        blindMsg1_Txt.text = status;
        blindMsg_Txt.text = status;
        challMsg1_Txt.text = status;
        string valAmount = CommonFunctions.Instance.GetAmountDecimalSeparator(value, true);
        blindVal1_Txt.text = valAmount;
        blindVal_Txt.text = valAmount;
        challVal1_Txt.text = valAmount;
    }


    public void onclickCloseButton()
    {
        //hasClickInfo = false;
        //handStrengthMeater_Obj.SetActive(false);
        //dealerMessage_Obj.SetActive(false);
        //challCount = 0;
        //tablePot_Obj.SetActive(false);
        //blindPannel.SetActive(false);
        //seePannel.SetActive(false);
        //incrementPannel.SetActive(false);
        //challPannel.SetActive(false);
        //showPannel.SetActive(false);
        //sideShowPannel.SetActive(false);
        //seeButton_Obj.SetActive(false);
        //WinnerPanelIngame.SetActive(false);
        //UIDetails[0].UpdateTimer(0);
        //UIDetails[0].UpdateTimer(1);
        //UIDetails[0].UpdateTimer(2);
        //UIDetails[0].ResetCard();
        //UIDetails[1].ResetCard();
        //UIDetails[2].ResetCard();
        //HideHud();
        //StartCoroutine(TutorialSteps1());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            
            Exitpanel.SetActive(true);
        }
    }
}
