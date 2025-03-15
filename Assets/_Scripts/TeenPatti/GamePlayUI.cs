using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;
using DG.Tweening;
using System;

namespace TP
{
	public class GamePlayUI : MonoBehaviour
	{

        #region VARIABLES
        //============================================================================================//
        [Header("=======SPRITE=========")]
        public Sprite clubs;
		public Sprite diamonds;
		public Sprite hearts;
		public Sprite spade;
		public Sprite jack;
		public Sprite queen;
		public Sprite king;
	   public  Sprite emptySprite;
		public Sprite CloseSprite;
		public Sprite SettingSprite;
		public Sprite bootCoin;
		public Sprite raiseCoin;
        public Sprite ActiveChalPic;
        public Sprite ActivePackPic;
        public Sprite ActiveShowPic;
        public Sprite ActivePlusMinus;
        public Sprite InActiveCommon;
        public Sprite InActivePlusMinus;
        public Sprite[] Coins;
        public Sprite[] tableBg;


        //============================================================================================//
        [Header("=======IMAGE=========")]
        public Image profileChip;
		public Image potChip;
		public Image hukamSuitImageSmall;
		public Image hukamSuitImageBig;
		public Image hukamCourtImage;
		public Image strengthMeterList;
        public Image Player1SideShowPic;
        public Image Player2SideShowPic;
        public Image Player1SideAlertShowPic;
        public Image Player2SideAlertShowPic;
        public Image FillEffect;
        public Image FillEffect1;


        //============================================================================================//
        [Header("=======TMP_TEXT=========")]
        public TMP_Text challAmountText;
	    public TMP_Text allinAmountText;
		public TMP_Text strengthMeterText;
		public TMP_Text hukamCardRank;
		public TMP_Text showButtonText;
		public TMP_Text challText;
		public TMP_Text profileAmount;
		public TMP_Text sideShowAlertSenderName;
		public TMP_Text sideShowAlertReciverName;
		public TMP_Text sideShowAlertSeconds;
		public TMP_Text sideShowSenderName;
		public TMP_Text sideShowReceiverName;
		public TMP_Text sideShowSeconds;
		public TMP_Text TeenPattiInfoBootAmount;
		public TMP_Text TeenPattiInfoBlindLimit;
		public TMP_Text TeenPattiInfoChallLimit;
		public TMP_Text TeenPattiInfoPotLimit;
		public TMP_Text globalText;
		public TMP_Text potAmount;
        public TMP_Text Chaal;
        public TMP_Text Show;
        public TMP_Text Pack;
        public TMP_Text AllIn;
        public TMP_Text AllInAmount;
        public TMP_Text Plus;
        public TMP_Text Minus;
        public TMP_Text ChaalAmount;
		public TMP_Text PlusTXT;
		public TMP_Text MinusTXT;
		
        [SerializeField] private TMP_Text bottomTextTemp;
		[SerializeField] private TMP_Text gameMode;


        //============================================================================================//
        [Header("=======GAMEOBJECT=========")]
        public GameObject ButtonHolder;
		public GameObject seeButton;
		public GameObject strengthMeter;
		public GameObject hukamCards;
		public GameObject sideShowPannel;
		public GameObject sideShowAlertPannel;
		public GameObject gameTimer;
		public GameObject globalPannel;
		public GameObject switchTable;
		public GameObject sharePannel; 
		public GameObject potAmountPannel;
		public GameObject logo;
		public GameObject potAmountHolder;
		public GameObject bottomTemp;
		public GameObject playerAmount;
        public GameObject GlowShowButton;
        public GameObject GlowPackButton;
        public GameObject GlowPlusButton;
        public GameObject GlowMinusButton;
        public GameObject GlowAllInButton;
        public GameObject GlowChaalButton;
       /* public GameObject GlowPotAmount;*/
        public GameObject AllPacked;
        public GameObject PotLimitReched;
        public GameObject ParentCoinsStack;
        public GameObject GlowPotHolder;
        public GameObject[] StartTimerObjects;
        public GameObject[] zanduCards;
        public GameObject[] PotChipsStack;


        //============================================================================================//
        [Header("=======BUTTON=========")]
        public Button increaseBet;
		public Button decreaseBet;
		public Button chaalButton;
		public Button showButton;
		public Button packButton;
		public Button allinButton;


        //============================================================================================//
        [Header("=======BOOLS=========")]
        public bool isStakeDoubled;
		public bool isBootCollected;
		public bool isRaisedBet;
		public bool isWaitingForRequest = false;
        public bool isHUDShown;
        bool isInteractable = false;


        //============================================================================================//
        [Header("=======DOUBLE=========")]
        double currCountdownValue;
        double hideHudeTime = 0;


        //============================================================================================//
        [Header("=======FLOAT=========")]
        private float targetFillAmount;

        //============================================================================================//
        [Header("=======STRING=========")]
        public string urldata;
        public string json;

        //============================================================================================//
        [Header("=======LIST=========")]
        public List<PlayerUI> playerUIDetails;


        //============================================================================================//
        [Header("=======COLORS=========")]
        public Color Inactive;
        public Color ActiveChaal;
        public Color ActivePack;
        public Color ChaalTextandAmount;
        public Color PlusMinus;
        public Color InactiveButtonColor;
        public Color InactiveButtonTXTColor;
        public Color activeButtonColor;
        public Color NormalTxtColor;
        public Color DisableTxtColor;



        //============================================================================================//
        [Header("=======BOOL=========")]
        public bool FirstCoinAnim = false;

        //============================================================================================//
        [Header("=======REFERANCE=========")]

		public Card[] cardComponent;
		public SettingsPanelHandler settingsPanel;
        public TotalPotFiller ResetCoins;
        public SideShowWinnerEffect sideShowWinnerEffect;
        public SideShowRequest sideShowRequest;
        Coroutine strengthMeterRountine;
		public static GamePlayUI instance;
        public TMP_Text Currency_TypePot;
        public TMP_Text Currency_TypePlayer;
        public TMP_Text Currency_Commision;

        #endregion

        #region UNITY_FUNCTIONS
        /// <summary>
        ///  Runs on Awake of this Application
        /// </summary>
        private void Awake()
        {
            instance = this;
            strengthMeter.SetActive(false);
        }

		/// <summary>
		/// 
		/// </summary>
        private void OnEnable()
        {
            hideHudeTime = 0;
        }

        #endregion

        #region HELPER_FUNCTIONS

        public void AssignCurrency(String currency, string money)
        {
            DebugHelper.Log("assingCurrency =========> " + money);
            profileAmount.text = money;
            Currency_TypePot.text = currency;
            Currency_TypePlayer.text = currency;
            Currency_Commision.text = currency;
        }


        public void EnableButtonColor()
        {
            allinButton.gameObject.SetActive(false);
            GlowShowButton.SetActive(true);
            GlowPackButton.SetActive(true);
            GlowPlusButton.SetActive(true);
            GlowMinusButton.SetActive(false);
            GlowAllInButton.SetActive(true);
            GlowChaalButton.SetActive(true);
            showButton.interactable = true;
            packButton.interactable = true;
            chaalButton.interactable = true;
            decreaseBet.interactable = false;
            showButton.image.sprite = ActiveShowPic;
            packButton.image.sprite = ActivePackPic;
            chaalButton.image.sprite = ActiveChalPic;
            increaseBet.image.sprite = ActivePlusMinus;
           
            allinButton.image.color = activeButtonColor;
            showButton.image.color = activeButtonColor;
            packButton.image.color = activeButtonColor;
            chaalButton.image.color = activeButtonColor;
            increaseBet.image.color = activeButtonColor;
            decreaseBet.image.color = activeButtonColor;
            GamePlayUI.instance.decreaseBet.image.color = GamePlayUI.instance.InactiveButtonColor;
            //decreaseBet.image.sprite = InActivePlusMinus;
            allinButton.image.sprite = ActiveChalPic;
            Chaal.color = ChaalTextandAmount;
            Show.color = ActiveChaal;
            Pack.color = ActivePack;
            AllInAmount.color = Inactive;
            AllIn.color = Inactive;
           
            Minus.color = Inactive;
            ChaalAmount.color = ChaalTextandAmount;
        }

        public void DisableButtonColorForAllin()
        {
            allinButton.gameObject.SetActive(true);
            GlowShowButton.SetActive(false);
            GlowPackButton.SetActive(true);
            GlowPlusButton.SetActive(false);
            GlowMinusButton.SetActive(false);
            GlowAllInButton.SetActive(true);
            GlowChaalButton.SetActive(false);
            showButton.interactable = false;
            packButton.interactable = true;
            chaalButton.interactable = false;
            increaseBet.interactable = false;
           
            decreaseBet.interactable = false;
            allinButton.interactable = true;
            allinButton.image.sprite = ActiveChalPic;
			allinButton.image.color = activeButtonColor;
            showButton.image.color = InactiveButtonColor;
            //showButton.image.sprite = InActiveCommon;
            packButton.image.color = activeButtonColor;
            packButton.image.sprite = ActivePackPic;
            chaalButton.image.color = InactiveButtonColor;
            increaseBet.image.color = InactiveButtonColor;
            decreaseBet.image.color = InactiveButtonColor;
            //chaalButton.image.sprite = InActiveCommon;
            //increaseBet.image.sprite = InActivePlusMinus;
            //decreaseBet.image.sprite = InActivePlusMinus;
            Chaal.color = Inactive;
            Show.color = Inactive;
            Pack.color = ActivePack;
            AllInAmount.color = ChaalTextandAmount;
            AllIn.color = ChaalTextandAmount;
            Plus.color = Inactive;
            Minus.color = Inactive;
            ChaalAmount.color = Inactive;
        }


        public void DisableButtonColor()
        {
            allinButton.gameObject.SetActive(false);
            GlowShowButton.SetActive(false);
            GlowPackButton.SetActive(false);
            GlowPlusButton.SetActive(false);
            GlowMinusButton.SetActive(false);
            GlowAllInButton.SetActive(false);
            GlowChaalButton.SetActive(false);
            showButton.interactable = false;
            packButton.interactable = false;
            chaalButton.interactable = false;
            increaseBet.interactable = false;
            
            decreaseBet.interactable = false;
            allinButton.interactable = false;
			//allinButton.image.sprite = InActiveCommon;
			//showButton.image.sprite = InActiveCommon;
			//packButton.image.sprite = InActiveCommon;
			//chaalButton.image.sprite = InActiveCommon;
			//increaseBet.image.sprite = InActivePlusMinus;
			//decreaseBet.image.sprite = InActivePlusMinus;
			allinButton.image.color = InactiveButtonColor;
			showButton.image.color = InactiveButtonColor;
			packButton.image.color = InactiveButtonColor;
			chaalButton.image.color = InactiveButtonColor;
            increaseBet.image.color = InactiveButtonColor;
            decreaseBet.image.color = InactiveButtonColor;
            Chaal.color = Inactive;
            Show.color = Inactive;
            Pack.color = Inactive;
            AllInAmount.color = Inactive;
            AllIn.color = Inactive;
            Plus.color = Inactive;
            Minus.color = Inactive;
            ChaalAmount.color = Inactive;
        }

        #endregion




        public void SwitchTableClick()
		{
			if (GameManager.localInstance.roomInfo.lobbyName != "Private")
				switchTable.SetActive(true);
		}
		public void InitDetails()
		{
			if (gameMode != null)
				gameMode.text = GameController.Instance.CurrentGameMode.ToString();




			SeeButtonActive(false);
			gameTimer.SetActive(false);
			sideShowPannel.SetActive(false);
			sideShowAlertPannel.SetActive(false);
			ResetZanthuCards();
			ResetHud();
			GameController.Instance.totalEarnings = 0;
			GameController.Instance.totalTrophyEarned = 0;
			GameManager.localInstance.gameState.isSideShowRequestSend = false;
			GameManager.localInstance.gameState.sideShowRequestReceiver = -1;
			GameManager.localInstance.gameState.sideShowRequestSender = -1;
			GameManager.localInstance.gameState.sideShowRequestTime = -1;
			GameManager.localInstance.isSideshow = false;
			GameManager.localInstance.IsSideShowChecked = false;
			GameManager.localInstance.isWinnerDisplayed = false;	
			potAmount.text = "0";
		}

        public void PotGlowAnim()
        {
            GlowPotHolder.gameObject.SetActive(true);

            Image imageComponent = GlowPotHolder.GetComponent<Image>();

            imageComponent.DOFade(1f, .5f)
                        .OnComplete(() =>
                        {
                            // Fade alpha from 1 to 0 after a delay
                            imageComponent.DOFade(0f, .5f).SetDelay(.5f);
                            /*.OnComplete(StartFadeLoop); // Loop the fading effect*/
                        });


        }


        public static string Base64Encode(string plainText)
		{
			var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
			return System.Convert.ToBase64String(plainTextBytes);
		}

		public void SwitchRoom()
		{
			GameManager.localInstance.RemoveMeFromGame(false);
			ClearAllUI();
		}

		public void SeeButtonControl()
		{
			
		}

		public IEnumerator setStrengthMeter(int strength)
		{
			if (strength < 0)
				yield break;
			if (strength > 10)
				strength = 10;

			if(strengthMeterRountine != null)
			{
				StopCoroutine(strengthMeterRountine);
            }

			yield return new WaitForSeconds(0.6f);
			if(strength == 1)
			{
                strengthMeterRountine =	StartCoroutine(SmoothFill(Mathf.InverseLerp(0, 10, 2)));
			}
			else if( strength == 3)
            {
                strengthMeterRountine = StartCoroutine(SmoothFill(Mathf.InverseLerp(0, 10, 4)));
            }
            else if (strength == 5)
            {
                strengthMeterRountine = StartCoroutine(SmoothFill(Mathf.InverseLerp(0, 10, 6)));
            }
            else if (strength == 7)
            {
                strengthMeterRountine= StartCoroutine(SmoothFill(Mathf.InverseLerp(0, 10, 8)));
            }
            else if (strength == 9)
            {
                strengthMeterRountine = StartCoroutine(SmoothFill(Mathf.InverseLerp(0, 10, 9)));
            }
            else if (strength == 10)
            {
                strengthMeterRountine = StartCoroutine(SmoothFill(Mathf.InverseLerp(0, 10, 10)));
            }
            strengthMeterText.text = ((CardsCombination)strength).ToString();

			
		}



        private IEnumerator SmoothFill(float targetFill)
        {
            targetFillAmount = Mathf.Clamp01(targetFill);

            while (Mathf.Abs(strengthMeterList.fillAmount - targetFillAmount) > 0.01f)
            {
                strengthMeterList.fillAmount = Mathf.Lerp(strengthMeterList.fillAmount, targetFillAmount, Time.deltaTime * 1f);
                yield return null;
            }

            strengthMeterList.fillAmount = targetFillAmount;
        }

		public void ShowHud()
		{
			if (hideHudeTime != 0 && hideHudeTime + 2 > NetworkTime.time)
			{
				return;
			}
			hideHudeTime = 0;
			isInteractable = true;
			if (GameManager.localInstance.myPlayerState.hasSeenCard)
			{
				challText.text = "Chaal";
			}
			else
			{
				challText.text = "Blind";
			}
			SeeButtonControl();
			GamePlayUI.instance.DisableBottomNotificationPannel();            
			isHUDShown = true;
		}

		public void HideHud()
		{
			isInteractable = false;
			
			if (packButton.gameObject.transform.localScale.x > 0.9f)
			{
				hideHudeTime = NetworkTime.time;
				if(bottomTemp.activeSelf)
				{
                    ButtonHolder.SetActive(false);
                }
				else
				{
                   // ButtonHolder.SetActive(true);
                }
            }
			isStakeDoubled = false;
             GamePlayUI.instance.GlowMinusButton.SetActive(false);
          //  decreaseBet.image.sprite = InActivePlusMinus;
            Minus.color = Inactive;
            decreaseBet.interactable = false;

			isHUDShown = false;
			SeeButtonControl();
		}
		public void ResetHud()
		{
			isInteractable = false;
			allinButton.gameObject.SetActive(false);
            ButtonHolder.gameObject.SetActive(false);
        }

		public void ResetZanthuCards()
		{

			for (int i = 0; i < 3; i++)
			{
				zanduCards[i].GetComponent<Animator>().Play("cardidlezandu");
			}
		}
		public void ZanduSetCard(CardData card, int numCard)
		{
			Card cardToChange = cardComponent[numCard];

			Sprite empty = emptySprite;
			Sprite suitImage = GameController.Instance.currentPlayingCards.GetCardSuitSplitDesign(card.suitCard);
			Sprite courtImage = GameController.Instance.currentPlayingCards.GetCardRankSplitDesign(card.rankCard);
			Color cardColor = GameController.Instance.currentPlayingCards.GetCardSuitColor(card.suitCard);
			if (courtImage == null || courtImage == empty)
			{
				courtImage = empty;
			}
			Color fontColor = GameController.Instance.currentPlayingCards.GetCardSuitFontColor(card.suitCard);
			string cardNumber = GameController.Instance.currentPlayingCards.GetCardRankCustomText(card.rankCard);

			if (card.rankCard > 10 && card.rankCard < 14)
			{
				SetCardsDetails(cardToChange, cardNumber, suitImage, empty, courtImage, cardColor, fontColor);
			}
			else if (card.rankCard == 14)
			{
				SetCardsDetails(cardToChange, "A", suitImage, suitImage, empty, cardColor, fontColor);
			}
			else if (card.rankCard == 10)
			{
				SetCardsDetails(cardToChange, GameController.Instance.currentPlayingCards.charcTenAltValue, suitImage, suitImage, empty, cardColor, fontColor);
			}
			else
			{
				SetCardsDetails(cardToChange, cardNumber, suitImage, suitImage, empty, cardColor, fontColor);
			}
			cardComponent[numCard].Intialize(card.cardIndex);
            zanduCards[numCard].gameObject.GetComponent<Animator>().Play("zandudeal" + (numCard + 1));
		}







        public void SetCardsDetails(Card cardToChange, string rankText, Sprite smallSuitSprite, Sprite bigSpriteImage, Sprite coutSprite, Color cardColor, Color fontColor)
		{

			cardToChange.cardRankTxt.ConvertAll(x => x.text = rankText);
			cardToChange.suitImageSmall.ConvertAll(x => x.sprite = smallSuitSprite);
			cardToChange.suitImageBig.ConvertAll(x => x.sprite = bigSpriteImage);
			cardToChange.courtImage.ConvertAll(x => x.sprite = coutSprite);
			cardToChange.cardRankTxt.ConvertAll(x => x.color = fontColor);
			cardToChange.suitImageSmall.ConvertAll(x => x.color = cardColor);
			cardToChange.suitImageBig.ConvertAll(x => x.color = cardColor);
		}

		/*public void HukamSetCard(CardData card)
		{
			if (card.rankCard > 10)
			{
				if (card.rankCard == 11)
				{
					hukamCardRank.text = "J";
					hukamCourtImage.sprite = jack;
					hukamCourtImage.enabled = true;
				}
				else if (card.rankCard == 12)
				{
					hukamCardRank.text = "Q";
					hukamCourtImage.sprite = queen;
					hukamCourtImage.enabled = true;
				}
				else if (card.rankCard == 13)
				{
					hukamCardRank.text = "K";

					hukamCourtImage.sprite = king;
					hukamCourtImage.enabled = true;
				}
				else if (card.rankCard == 14)
				{
					hukamCardRank.text = "A";
				}
			}
			else if (card.rankCard <= 10)
			{
				if (card.rankCard == 1)
				{
					card.rankCard = 14;
					hukamCardRank.text = "A";
				}
				else
				{
					hukamCardRank.text = card.rankCard.ToString();
				}
			}
			if (card.suitCard.ToString() == "Clubs")
			{
				hukamSuitImageBig.sprite = clubs;
				hukamSuitImageSmall.sprite = clubs;
				hukamSuitImageSmall.color = Color.black;
				hukamSuitImageBig.color = Color.black;
				hukamCardRank.color = Color.black;
			}
			else if (card.suitCard.ToString() == "Diamonds")
			{
				hukamSuitImageSmall.sprite = diamonds;
				hukamSuitImageSmall.color = Color.red;
				hukamSuitImageBig.sprite = diamonds;
				hukamSuitImageBig.color = Color.red;
				hukamCardRank.color = Color.red;
			}
			else if (card.suitCard.ToString() == "Hearts")
			{
				hukamSuitImageSmall.sprite = hearts;
				hukamSuitImageSmall.color = Color.red;
				hukamSuitImageBig.sprite = hearts;
				hukamSuitImageBig.color = Color.red;
				hukamCardRank.color = Color.red;
			}
			else if (card.suitCard.ToString() == "Spade")
			{
				hukamSuitImageSmall.sprite = spade;
				hukamSuitImageSmall.color = Color.black;
				hukamSuitImageBig.sprite = spade;
				hukamSuitImageBig.color = Color.black;
				hukamCardRank.color = Color.black;
			}
			DebugHelperHelper.LogError("zanthu details");
			hukamCards.SetActive(true);

		}*/


		public void EnableBottomNotificationPannel(string textToDisplay)
		{
			ButtonHolder.SetActive(false);
            UIController.Instance.Connecting.SetActive(false);
            bottomTemp.gameObject.SetActive(true);
			bottomTextTemp.text = textToDisplay;
#if RealTPG
			playerAmount.SetActive(false);
#else
			playerAmount.SetActive(true);
#endif
		}


		public void GlobalMessage(string message)
		{
			CancelInvoke("HideGlobalMessage");
			DebugHelper.Log("GlobalPanelCalled");
			globalPannel.SetActive(true);
			globalText.text = message;
			Invoke("HideGlobalMessage", 2);
		}
		public void HideGlobalMessage()
		{
			if (GameManager.localInstance == null)
				return;
			if (GameManager.localInstance.gameState.currentState == 0)
			{
				EnableBottomNotificationPannel("PLEASE WAIT FOR PLAYERS TO JOIN");
			}
			else
			{
				globalPannel.SetActive(false);
			}
		}

		public void DisableBottomNotificationPannel()
		{
			bottomTemp.gameObject.SetActive(false);
			playerAmount.SetActive(true);


		}
		



		public IEnumerator StartCountdown()
		{
			sideShowSeconds.text = "10";
            currCountdownValue = GameManager.localInstance.gameState.sideShowRequestTime + 10;
			while (currCountdownValue > NetworkTime.time && GameManager.localInstance.gameState.currentState == 3)
			{
                if (((int)(currCountdownValue - NetworkTime.time)) < 10)
                {
                    sideShowSeconds.text = ((int)(currCountdownValue - NetworkTime.time)).ToString();
					DebugHelper.LogError("sideShowSeconds1    =   " + ((int)(currCountdownValue - NetworkTime.time)));
				}
					
				yield return new WaitForSeconds(1f);
			}
			sideShowPannel.SetActive(false);
		}

        private IEnumerator StartCountdownTimer()
        {
            FillEffect.fillAmount = 1.0f;
            float countdownDuration = 9.0f;
            float startTime = countdownDuration;
            float updateInterval = 0.1f;
            while (startTime >= 0.0f && GameManager.localInstance.gameState.currentState == 3)
            {
                float fillAmount = startTime / countdownDuration; 
                FillEffect.fillAmount = fillAmount;
                yield return new WaitForSeconds(updateInterval);
                startTime -= updateInterval;
            }
            FillEffect.fillAmount = 0.0f;
            yield return new WaitForSeconds(0.2f);
        }

        private IEnumerator StartCountdownTimer1()
        {
            FillEffect1.fillAmount = 1.0f;
            float countdownDuration = 9.0f;
            float startTime = countdownDuration;
            float updateInterval = 0.1f;
            while (startTime >= 0.0f && GameManager.localInstance.gameState.currentState == 3)
            {
                float fillAmount = startTime / countdownDuration;
                FillEffect1.fillAmount = fillAmount;
                yield return new WaitForSeconds(updateInterval);
                startTime -= updateInterval;
            }
            FillEffect.fillAmount = 0.0f;
            yield return new WaitForSeconds(0.2f);
        }


        public IEnumerator StartCountdownAlert()
		{
			sideShowAlertSeconds.text = "10";
			currCountdownValue = GameManager.localInstance.gameState.sideShowRequestTime + 10;

			while (currCountdownValue > NetworkTime.time && GameManager.localInstance.gameState.currentState == 3)
			{
				if (((int)(currCountdownValue - NetworkTime.time)) < 10)
                {
					sideShowAlertSeconds.text = ((int)(currCountdownValue - NetworkTime.time)).ToString();
					DebugHelper.LogError("sideShowSeconds2    =   " + ((int)(currCountdownValue - NetworkTime.time)));

				}
					
				yield return new WaitForSeconds(0.5f);

			}
			sideShowAlertPannel.SetActive(false);

		}
		public void StartSideShow()
		{

			DebugHelper.Log("START SIDE SHOW CALLED ****************");
			GlobalMessage("Requesting side show");
			sideShowPannel.SetActive(true);
			PlayerState state = GameManager.localInstance.gameState.players[GameManager.localInstance.gameState.sideShowRequestSender];
            PlayerState state1 = GameManager.localInstance.gameState.players[GameManager.localInstance.gameState.sideShowRequestReceiver];
            Player1SideShowPic.sprite = GameController.Instance.avatharPicture[state.ui];
            Player2SideShowPic.sprite = GameController.Instance.avatharPicture[state1.ui];
            sideShowSenderName.text = GameManager.localInstance.gameState.players[GameManager.localInstance.gameState.sideShowRequestSender].playerData.playerName;
			sideShowReceiverName.text = GameManager.localInstance.myPlayerID == GameManager.localInstance.gameState.players[GameManager.localInstance.gameState.sideShowRequestReceiver].playerData.playerID ? "You" : GameManager.localInstance.gameState.players[GameManager.localInstance.gameState.sideShowRequestReceiver].playerData.playerName;
			StopCoroutine(nameof(StartCountdown));
			StartCoroutine(nameof(StartCountdown));
            StopCoroutine(nameof(StartCountdownTimer));
            StartCoroutine(nameof(StartCountdownTimer));


        }
		public void StartSideShowUser(int sender, int reciver)
		{

			DebugHelper.Log("StartSideShowUser ****************");
			sideShowAlertPannel.SetActive(true);
            PlayerState state = GameManager.localInstance.gameState.players[sender];
            PlayerState state1 = GameManager.localInstance.gameState.players[reciver];
            Player1SideAlertShowPic.sprite = GameController.Instance.avatharPicture[state.ui];
            Player2SideAlertShowPic.sprite = GameController.Instance.avatharPicture[state1.ui];
            sideShowAlertSenderName.text = GameManager.localInstance.myPlayerID == GameManager.localInstance.gameState.players[sender].playerData.playerID ? "You" : GameManager.localInstance.gameState.players[sender].playerData.playerName;
			sideShowAlertReciverName.text = GameManager.localInstance.gameState.players[reciver].playerData.playerName;
            StopCoroutine(nameof(StartCountdownAlert));
			StartCoroutine(nameof(StartCountdownAlert));
            StopCoroutine(nameof(StartCountdownTimer1));
            StartCoroutine(nameof(StartCountdownTimer1));

        }
		public void OnAcceptSideShow()
		{
			DebugHelper.Log("sideshow::: accept:::" + GameManager.localInstance.gameState.sideShowRequestSender);
			GlobalMessage("Side show request accepted " + GameManager.localInstance.gameState.sideShowRequestSender);
			GameManager.localInstance.OnAcceptSideShow(GameManager.localInstance.gameState.sideShowRequestSender, GameManager.localInstance.GetMyPlayerIndex());
		   
		}

		public void OnDeclineSideShow()
		{
			GameManager.localInstance.OnDeclineSideShow(GameManager.localInstance.gameState.sideShowRequestSender, GameManager.localInstance.GetMyPlayerIndex());	
		}


		public void TextCurrebyBoot(float _boot)
		{
			if (isStakeDoubled)
				challAmountText.text = (_boot * 2).ToString() + " " + $"<size=25>{APIController.instance.authentication.currency_type}</size>"; 
			else
				challAmountText.text = (_boot).ToString() + " " + $"<size=25>{APIController.instance.authentication.currency_type}</size>"; 
		}
		public float ChaalIncrease;
        public void UpdateChaalText(float boot, bool RejoinSetAmount = false)
        {
            float finalBoot = 0;
            if (isStakeDoubled)
                finalBoot = boot * 2;
            else
                finalBoot = boot;
            if (APIController.instance.userDetails.balance <= finalBoot)
            {
                HideHud();
                if (RejoinSetAmount)
                {
                    challAmountText.text = finalBoot < 100000 ? CommonFunctions.Instance.GetAmountDecimalSeparator(finalBoot) : CommonFunctions.Instance.GetAmountAbreviation(finalBoot);
                }

            }
            else
            {
                challAmountText.text = finalBoot < 100000 ? CommonFunctions.Instance.GetAmountDecimalSeparator(finalBoot) : CommonFunctions.Instance.GetAmountAbreviation(finalBoot);

            }
            ChaalIncrease = finalBoot;
        }

        public void ExitGame()
		{
			DebugHelper.LogError("call game exit");



			GameManager.localInstance.Exit(false);

		}


		public void Standup()
		{
			GameManager.localInstance.OnStandUpUser();
		}

		public void Sit()
		{
			GameManager.localInstance.OnPlayerSit();
		}

		public void OnChaalClicked()
		{			
			GameManager.localInstance.VerifyAndChaal(false);
		}

		public void OnAllInClicked()
		{
			GameManager.localInstance.VerifyAndAllin();

		}
		public void OnPackClicked()
		{
			GameManager.localInstance.VerifyAndPack();


#if GOP
			foreach	(PlayerUI player in playerUIDetails)
			{
				if (player.isMine)
				{
					player.PlayerHandAC.SetTrigger("End");
				}
			}
#endif
        }

		public void OnincreaseBet()
		{
			GameManager.localInstance.OnIncreaseBet();
		}

		public void OndecreaseBet()
		{
			GameManager.localInstance.OnDecreaseBet();
		}
		public void OnClickStartGamePrivate()
		{
			GameManager.localInstance.StartPrivateTable();
		}

		public void OnShowClicked()
		{
			GameManager.localInstance.VerifyAndShow();
		}
		public bool SoundCheck;
		public void OnSeeClicked()
		{

			SoundCheck = false;
            GameManager.localInstance.VerifyAndSeeCards();
		
		}

		public void ClearAllCards()
		{
			foreach (PlayerUI playerUI in playerUIDetails)
				playerUI.ResetCard();

		}
		public void ClearAllUI()
		{
			HideHud();
			foreach (PlayerUI playerUI in playerUIDetails)
				playerUI.ClearUI();
			sideShowPannel.SetActive(false);
			sideShowAlertPannel.SetActive(false);
		}

		public void ShowProfile(PlayerUI myUi)
		{
			if (myUi.myPlayerState.playerData.playerID != null)
			{
				if (myUi.myPlayerState.playerData.playerID != "" && myUi.myPlayerState.playerData.playerID != "0")
				{
					GameController.Instance.profileControl = 2;
					GameController.Instance.profileData = myUi;
				}
			}
		}

		public void strengthMeterActive(bool active)
		{
			if (active && (GameManager.localInstance.gameState.currentState != 0 && GameManager.localInstance.gameState.currentState != 1) && !GameManager.localInstance.gameState.isDealCard && GameManager.localInstance.myPlayerState.isSpectator)
			{
				strengthMeter.SetActive(!active);
				return;
			}
			strengthMeter.SetActive(active);
			DebugHelper.Log("STRENGTH ************** &&&&&&&&");
		}


		public void HandStrengthMeter()
		{
			
			if (GameController.Instance.CurrentPlayerData.IsFBLoggedInLinked())
			{
				
			}
			else
			{
				
			}
			if ((GameManager.localInstance.gameState.currentState == 0 || GameManager.localInstance.gameState.currentState == 1) || !GameManager.localInstance.gameState.isDealCard || GameManager.localInstance.myPlayerState.isSpectator || GameManager.localInstance.myPlayerState.hasPacked || !GameManager.localInstance.myPlayerState.hasSeenCard || GameManager.localInstance.myPlayerState.currentState == 0)
			{
				DebugHelper.Log("STRENGTH **************");
				strengthMeter.SetActive(false);
			}
		}

		public void SeeButtonActive(bool see)
		{
			if (see)
			{
				/*if (GameController.Instance.CurrentGameMode == GameMode.POTBLIND)
					return;*/
				if (GameManager.localInstance.gameState.currentState < 2 || !GameManager.localInstance.isdistributCard || !GameManager.localInstance.isdistributCardEnd)
				{
					seeButton.SetActive(!see);
					return;
				}
			}

			seeButton.SetActive(see);
		
		}

        public void CallSettingAction()
        {
            if (settingsPanel.PanelTransform.anchoredPosition.x <= -522)
            {
                settingsPanel.ShowSettings();
                //settingsPanel.SwapSpriteSequence = null;
                //settingsPanel.SwapSpriteSequence += () => DOTween.Sequence().Append(settingsbutton.transform.DOScale(Vector2.zero, 0.125f).OnComplete(() => settingsbutton.image.sprite = SettingSprite)).Append(settingsbutton.transform.DOScale(Vector2.one, 0.125f).SetEase(Ease.OutBounce));
                //DOTween.Sequence().Append(settingsbutton.transform.DOScale(Vector2.zero, 0.125f).OnComplete(() => settingsbutton.image.sprite = CloseSprite)).Append(settingsbutton.transform.DOScale(Vector2.one, 0.125f).SetEase(Ease.OutBounce));
            }
            else
            {
                settingsPanel.HideSettings();
            }
        }
    }

}
