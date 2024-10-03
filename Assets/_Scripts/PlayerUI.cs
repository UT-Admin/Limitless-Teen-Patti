using DG.Tweening;
using Mirror;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace TP
{
    public class PlayerUI : MonoBehaviour
    {
        enum cardAnim
        {
            Reset = 0,
            Deal = 1,
            Reveal = 2
        }

        [Header("======================================================================")]

        [Header("Private")]
        [SerializeField] private RectTransform betContainer;
        [SerializeField] private RectTransform potContainer;
        [SerializeField] private RectTransform dummyPosition;
        [SerializeField] private RectTransform dummyPositionCards;
        [SerializeField] private Vector3 localStartPosision;
        [SerializeField] private Vector3 localEndPosision;
        [SerializeField] private GameObject sitButton;
        [SerializeField] private TextMeshProUGUI playerStatus;
        [SerializeField] private Image ImageStatusColor;
        [SerializeField] private Image avatharTimer;
        [SerializeField] private GameObject[] lifes;
        [SerializeField] private GameObject[] cards;
        [SerializeField] private TextMeshProUGUI betAmount;
        [SerializeField] private Image betChip, ProfileBalanceChip, amountChip;
        [SerializeField] private GameObject poolcards;
        [SerializeField] private GameObject jokerCard;
        [SerializeField] private AudioSource warnclip;
        [SerializeField] private TMP_Text _chat;
        [SerializeField] private TMP_Text _ChatPrivate;
        [SerializeField] private GameObject _chatContainer;
        [SerializeField] private GameObject _chatContainerPrivate;
        [SerializeField] private GameObject[] emojiList;
        [SerializeField] private Transform emojiPoint;
        [SerializeField] private int idOrder;
        [SerializeField] private bool IsInitialized = false;
        [SerializeField] private Color colorPacked = new Color(.2f, .2f, .2f);
        [SerializeField] private bool isGiveAmountToPot = false, isGetAmountFromPot = false;
        [SerializeField] private int availableLives;
        [SerializeField] private string betcoinImage = "";
        [SerializeField] private string[] TutorialBotnames = { "Wall-E", "Eva" };
        [SerializeField] private Image Packed;
        [SerializeField] private Color color1;
        [SerializeField] private Color color2;
        [SerializeField] private Color color3;



        [Header("======================================================================")]

        [Header("Public")]
        public Image playerAvatar;
        public TextMeshProUGUI playerName;
        public GameObject winnerBanner;
        public GameObject winnerBannerGlow;
        public GameObject winnerBannerGlow2;
        public TextMeshProUGUI playerAmount;
        public GameObject invite, inviteDuluxe, gift;
        public GameObject playerImage, profilePictureGlow, playerBalance, profilebg;
        public Card[] cardComponent;
        public PlayerState myPlayerState;
        public string playerID = "";
        public bool isMine;
        public Animator PlayerHandAC;
        public Image BetAmountSpritePic;
        public Image PlayerAmountSpritePic;
        public List<GameObject> CoinAnimation;
        public int UIIndex;

        public GameObject playerMeGlow1;
        public GameObject playerMeGlow2;
        public GameObject playerMeGlow3;
        public GameObject TimerCountDown;
        public GameObject RaiseUp;
        public Image PotSecondShowSplit;
        public TMP_Text TimerCountDownTxt;
        [SerializeField] public Vector3 MovePostion;
        [SerializeField] public Vector3 StartPostion;
        [SerializeField] public Vector3 PotSplitPositionStart;

        private void Awake()
        {


        }
        public void CloseWinnerBanner()
        {
            winnerBannerGlow2.SetActive(false);
            winnerBannerGlow.SetActive(false);
            winnerBanner.SetActive(false);
        }
        private void OnEnable()
        {

            ClearUI();



        }

        private void OnDisable()
        {
            RaiseUp.SetActive(false);
        }
        public void CheckTimerFalse()
        {

            isTimerRunning = false;


        }

        public bool isTimerRunning = false;
        public void StartTurn()
        {
            Debug.Log("Check Start Turn");
            TimerCountDown.SetActive(true);
            StopCoroutine(nameof(StartCountdown));
            StartCoroutine(nameof(StartCountdown));
            StopCoroutine(nameof(StartTimer));
            StartCoroutine(nameof(StartTimer));
        }
        public void EndTurn()
        {

            try
            {
                MasterAudioController.instance.StopAudio(AudioEnum.TIMER);
            }
            catch
            {

            }

            StopCoroutine(nameof(StartTimer));
            StopCoroutine(nameof(StartCountdown));
            avatharTimer.enabled = false;
            TimerCountDown.SetActive(false);

        }
        double currCountdownValue;
        public IEnumerator StartCountdown()
        {

            TimerCountDownTxt.text = "15";
            currCountdownValue = myPlayerState.myTurnTime + 15;
            while (currCountdownValue > NetworkTime.time)
            {
                if (((int)(currCountdownValue - NetworkTime.time)) < 15 && ((int)(currCountdownValue - NetworkTime.time)) >= 1)
                {
                    TimerCountDownTxt.text = ((int)(currCountdownValue - NetworkTime.time)).ToString();
                    DebugHelper.LogError("sideShowSeconds1    =   " + ((int)(currCountdownValue - NetworkTime.time)));
                }

                yield return new WaitForSeconds(1f);
            }

        }


        IEnumerator StartTimer()
        {
            avatharTimer.enabled = true;


            avatharTimer.color = Color.green;

            avatharTimer.fillAmount = 0;
            double myTurnTime = myPlayerState.myTurnTime;
            Debug.Log("======================> StartTimer Check" + myPlayerState.lives + " =============> " + myPlayerState.isMyTurn);
            while (/*myPlayerState.lives > 0 &&*/ myPlayerState.isMyTurn)
            {
                bool playedSound = false;
                bool playedVibration = false;
                float timerVal = 0f;
                while (timerVal < 1)
                {
                    if (isMine)
                        GamePlayUI.instance.ShowHud();
                    timerVal = (float)((NetworkTime.time - (myTurnTime)) / 15f);
                    timerVal = Mathf.Clamp(timerVal, 0, 1);
                    avatharTimer.enabled = true;
                    avatharTimer.fillAmount = 1 - timerVal;

                    // Color col = CommonFunctions.Instance.ReMapColor(0.6f, 0.8f, Color.green, Color.red, timerVal);
                    Color col = Color.green;
                    avatharTimer.color = col;


                    if (timerVal > 0.7f && !playedSound)
                    {
                        playedSound = true;
                        if (playerID == GameController.Instance.CurrentPlayerData.GetPlayfabID())
                        {
                            if (MasterAudioController.instance.CheckSoundToggle())
                                MasterAudioController.instance.PlayAudio(AudioEnum.TIMER);

                        }

                    }
                    if (timerVal > 0.78 && !playedVibration)
                    {


                    }

                    if (timerVal > 0.7)
                    {
                        Color color1 = Color.red;
                        avatharTimer.color = color1;

                    }


                    yield return new WaitForSeconds(0.02f);
                }
                MasterAudioController.instance.StopAudio(AudioEnum.TIMER);
                myPlayerState.lives--;






                // lifes[myPlayerState.lives].SetActive(false);


            }
            if (isMine)
                GamePlayUI.instance.HideHud();

        }

        PlayerManager myPlayerManager;
        public void UpdatePlayer(PlayerManager playerManager)
        {
            myPlayerManager = playerManager;
            UpdateStatus(playerManager.GetPlayerState());
        }



        //public void UpdateState(PlayerState ps)
        //{
        //    myPlayerState = ps;
        //    playerID = myPlayerState.playerData.playerID;
        //    if (ps.isMyTurn && GameManager.localInstance.gameState.currentState == 2)
        //    {
        //        StartTurn();
        //    }
        //    else
        //    {
        //        MasterAudioController.instance.StopAudio(AudioEnum.TIMER);
        //        Debug.Log("Timer  END CALLED  -------- >  IS TIMER RUNNIUNG  ------------> UpdateState ");
        //        EndTurn();
        //        availableLives = ps.lives;
        //    }

        //    if (!IsInitialized)
        //        InitUI();
        //    SetInfoText();
        //}

        bool isSeeClicked;
        public void SetSeeBool()
        {
            isSeeClicked = true;
            GamePlayUI.instance.increaseBet.interactable = false;
        }


        public void UpdateStatus(PlayerState ps, bool force = false)
        {
            myPlayerState = ps;
            playerID = myPlayerState.playerData.playerID;
            Debug.Log("==================>YYYYYYYYYYYYYYYYYYYYY" + ps.playerData.playerName + "==================> " + ps.isMyTurn + " ==============================> " + GameManager.localInstance.gameState.currentState);



            if (ps.isMyTurn && GameManager.localInstance.gameState.currentState == 2)
            {

                if (isMine)
                {
                    playerMeGlow1.SetActive(true);
                    playerMeGlow2.SetActive(true);
                    playerMeGlow3.SetActive(true);
                }

                isTimerRunning = true;
                StartTurn();
                if (!isTimerRunning)
                {


                }

                setRaiseOf();

                if (isMine)
                {
                    if (APIController.instance.userDetails.balance <= (myPlayerState.hasSeenCard ? GameManager.localInstance.gameState.currentStake : (GameManager.localInstance.gameState.currentStake / 2)))
                    {
                        Debug.Log("CHECK ALL IN ==============>");
                        GamePlayUI.instance.allinButton.gameObject.SetActive(true);
                        GamePlayUI.instance.allinAmountText.text = CommonFunctions.Instance.GetAmountDecimalSeparator(APIController.instance.userDetails.balance);
                        GamePlayUI.instance.DisableButtonColorForAllin();
                        return;
                    }



                    GamePlayUI.instance.EnableButtonColor();


                    if (GameManager.localInstance.CanIncreaseBoot(GameManager.localInstance.myPlayerID))
                    {
                        GamePlayUI.instance.GlowPlusButton.SetActive(true);
                        GamePlayUI.instance.increaseBet.image.color = GamePlayUI.instance.activeButtonColor;
                        GamePlayUI.instance.Plus.color = GamePlayUI.instance.PlusMinus;
                        GamePlayUI.instance.increaseBet.interactable = true;

                        if (GamePlayUI.instance.ChaalIncrease == GameManager.localInstance.gameState.currentStake && isSeeClicked)
                        {
                            GamePlayUI.instance.GlowMinusButton.SetActive(true);
                            GamePlayUI.instance.decreaseBet.image.color = GamePlayUI.instance.activeButtonColor;
                            //GamePlayUI.instance.decreaseBet.image.sprite = GamePlayUI.instance.ActivePlusMinus;
                            GamePlayUI.instance.Minus.color = GamePlayUI.instance.PlusMinus;
                            GamePlayUI.instance.decreaseBet.interactable = true;
                            GamePlayUI.instance.GlowPlusButton.SetActive(false);
                            GamePlayUI.instance.increaseBet.image.color = GamePlayUI.instance.InactiveButtonColor;
                            GamePlayUI.instance.Plus.color = GamePlayUI.instance.Inactive;
                            GamePlayUI.instance.increaseBet.interactable = false;
                        }
                    }
                    else
                    {
                        GamePlayUI.instance.GlowPlusButton.SetActive(false);
                        GamePlayUI.instance.increaseBet.image.color = GamePlayUI.instance.InactiveButtonColor;
                        //  GamePlayUI.instance.increaseBet.image.sprite = GamePlayUI.instance.InActivePlusMinus;
                        GamePlayUI.instance.Plus.color = GamePlayUI.instance.Inactive;
                        GamePlayUI.instance.increaseBet.interactable = false;
                    }

                    if (((GameManager.localInstance.GetSeenPlayerCount() > 1 && GameManager.localInstance.myPlayerState.hasSeenCard) || (GameManager.localInstance.GetContestingPlayers().Count <= 2)))
                    {
                        GamePlayUI.instance.showButton.image.sprite = GamePlayUI.instance.ActiveShowPic;
                        GamePlayUI.instance.Show.color = GamePlayUI.instance.ActiveChaal;
                        GamePlayUI.instance.showButtonText.text = "Show";
                        GamePlayUI.instance.GlowShowButton.SetActive(true);
                        GamePlayUI.instance.showButton.interactable = true;
                    }
                    else
                    {
                        GamePlayUI.instance.showButton.image.color = GamePlayUI.instance.InactiveButtonColor;
                        // GamePlayUI.instance.showButton.image.sprite = GamePlayUI.instance.InActiveCommon;
                        GamePlayUI.instance.Show.color = GamePlayUI.instance.Inactive;
                        GamePlayUI.instance.GlowShowButton.SetActive(false);
                        GamePlayUI.instance.showButton.interactable = false;
                    }

                    if (GameManager.localInstance.GetSeenPlayerCount() > 1 && GameManager.localInstance.myPlayerState.hasSeenCard && GameManager.localInstance.GetContestingPlayers().Count > 2)
                    {
                        GamePlayUI.instance.GlowShowButton.SetActive(true);
                        GamePlayUI.instance.showButton.image.sprite = GamePlayUI.instance.ActiveShowPic;
                        GamePlayUI.instance.Show.color = GamePlayUI.instance.ActiveChaal;
                        GamePlayUI.instance.showButton.interactable = true;
                        GamePlayUI.instance.showButtonText.text = "Side Show";

                    }
                    else if (GameManager.localInstance.GetContestingPlayers().Count == 2 && GameManager.localInstance.myPlayerState.hasSeenCard)
                    {
                        GamePlayUI.instance.GlowShowButton.SetActive(true);
                        GamePlayUI.instance.showButton.image.sprite = GamePlayUI.instance.ActiveShowPic;
                        GamePlayUI.instance.Show.color = GamePlayUI.instance.ActiveChaal;
                        GamePlayUI.instance.showButton.interactable = true;
                        GamePlayUI.instance.showButtonText.text = "Show";
                    }
                    else
                    {
                        GamePlayUI.instance.GlowShowButton.SetActive(false);
                        GamePlayUI.instance.showButton.image.color = GamePlayUI.instance.InactiveButtonColor;
                        //  GamePlayUI.instance.showButton.image.sprite = GamePlayUI.instance.InActiveCommon;
                        GamePlayUI.instance.Show.color = GamePlayUI.instance.Inactive;
                        GamePlayUI.instance.showButton.interactable = false;
                        GamePlayUI.instance.showButtonText.text = "Side Show";
                        if (GameController.Instance.CurrentGameMode == GameMode.POTBLIND)
                        {
                            //  GlowShow.gameObject.SetActive(false);
                            GamePlayUI.instance.showButton.interactable = false;
                        }
                    }
                }




            }
            else
            {
                TempHolderForSee = false;
                if (isMine)
                {

                    isSeeClicked = false;
                    playerMeGlow1.SetActive(false);
                    playerMeGlow2.SetActive(false);
                    playerMeGlow3.SetActive(false);
                    GamePlayUI.instance.DisableButtonColor();
                }
        
                isTimerRunning = false;
                MasterAudioController.instance.StopAudio(AudioEnum.TIMER);

                Debug.Log("======================> Timer Check UpdateStatus");
                EndTurn();
                availableLives = ps.lives;
            }

            if (!IsInitialized || force)
                InitUI();
            SetInfoText();

            if (GameManager.localInstance != null && GameManager.localInstance.gameState.currentState == 4)
            {
                setRaiseOf();

            }

                if (myPlayerState.hasPacked && isMine)
            {
                setRaiseOf();
            }
        }

        public void SetDisconnected()
        {

        }

        public bool IsFull()
        {
            if (playerID == "")
                return false;
            else
                return true;
        }
        public void SetInfoText(string info)
        {
            if (info == "")
            {
                ImageStatusColor.gameObject.SetActive(false);
                profilePictureGlow.SetActive(false);
                playerStatus.text = "";
                playerStatus.color = Color.white;
            }
            else
            {
                ImageStatusColor.gameObject.SetActive(false);
                playerStatus.text = info;
                profilePictureGlow.SetActive(true);
            }
        }
        bool TempHolderForSee;
        bool TempHolderForBoot;
        public void SetInfoText()
        {
            if (isMine) GamePlayUI.instance.SeeButtonActive(false);
            playerAvatar.color = Color.white;
            if (myPlayerState.hasSeenCard && GameController.Instance.CurrentGameMode != GameMode.POTBLIND)
            {

                if (myPlayerState.hasSeenCardBoolCheck)
                {
                    Debug.Log("Check For SEE");
                    playerStatusShow();
                    profilePictureGlow.SetActive(true);
                    playerStatus.text = "Seen";
                    playerStatus.color = color1;
                    ImageStatusColor.gameObject.SetActive(true);
                  //  ImageStatusColor.color = color1;

                }
                else if (myPlayerState.hasAllin)
                {

                    playerStatusShow();
                    profilePictureGlow.SetActive(true);
                    playerStatus.text = "All in";
                    playerStatus.color = color2;
                    ImageStatusColor.gameObject.SetActive(true);
                    //ImageStatusColor.color = color2;
                }
                else
                {
                    playerStatusShow();
                    profilePictureGlow.SetActive(true);
                    playerStatus.text = "Chaal";
                    playerStatus.color = color1;
                    ImageStatusColor.gameObject.SetActive(true);
                    //ImageStatusColor.color = color1;
                }


            }
            else
            {
                playerStatusShow();
                profilePictureGlow.SetActive(true);
                if (GameManager.localInstance.gameState.currentState == 1)
                {
                    TempHolderForBoot = true;
                }

                if (GameManager.localInstance.gameState.currentState == 2)
                {

                    if (TempHolderForBoot)
                    {

                        playerStatus.text = "Boot";
                        ImageStatusColor.gameObject.SetActive(true);
                        playerStatus.color = color2;
                      //  ImageStatusColor.color = color2;
                        TempHolderForBoot = false;
                    }
   
                    else
                    {
                        playerStatus.text = "Blind";
                        playerStatus.color = color3;
                        ImageStatusColor.gameObject.SetActive(false);
                      //  ImageStatusColor.color = color2;
                    }

                }
                else
                {
                    playerStatus.text = "";
                    ImageStatusColor.gameObject.SetActive(false);
                }
            }

            if (myPlayerState.hasPacked)
            {


                playerAvatar.color = colorPacked;



                playerStatusShow();
                profilePictureGlow.SetActive(true);
                playerStatus.color = Color.white;
                playerStatus.text = "Packed";
                ImageStatusColor.gameObject.SetActive(false);
                if (isMine)
                {
                    GamePlayUI.instance.SeeButtonActive(false);
                    GamePlayUI.instance.HideHud();
                    ResetCard(true);

                    Invoke(nameof(PlayAudioDelay), 0f);
                }
            }

            if (GameManager.localInstance.gameState.waitingPlayers.Exists(x => x.playerData.playerID == playerID))
            {

                playerAvatar.color = colorPacked;

                playerStatusShow();
                profilePictureGlow.SetActive(true);
                playerStatus.text = "Waiting";
                playerStatus.color = Color.white;
                ImageStatusColor.gameObject.SetActive(false);
                ResetCard(true);

                if (isMine)
                {
                    GamePlayUI.instance.SeeButtonActive(false);

                    GamePlayUI.instance.HideHud();
                }
            }

            if (myPlayerState.isSpectator)
            {


                playerAvatar.color = colorPacked;

                playerStatusShow();
                profilePictureGlow.SetActive(true);
                ImageStatusColor.gameObject.SetActive(false);
                playerStatus.text = "";

                ResetCard(true);
                if (isMine)
                {
                    GamePlayUI.instance.SeeButtonActive(false);
                    sitButton.SetActive(true);
                    GamePlayUI.instance.HideHud();
                }
            }

            else if (isMine)
            {
                sitButton.SetActive(false);
                if (!GameManager.localInstance.gameState.waitingPlayers.Exists(x => x.playerData.playerID == playerID) && !myPlayerState.hasSeenCard && !myPlayerState.hasPacked && GameManager.localInstance.gameState.currentState == 2 && GameManager.localInstance.gameState.isDealCard)
                {
                    GamePlayUI.instance.SeeButtonActive(true);
                }
                else
                {
                    GamePlayUI.instance.SeeButtonActive(false);
                }
            }
            if (myPlayerState.disconnectTime > 0)
            {

            }


            HideLife();
            if (myPlayerState != null)
            {
                for (int i = 0; i < myPlayerState.lives; i++)
                    lifes[i].SetActive(true);
            }
        }
        #region Tutorial

        public void PlayAudioDelay()
        {
            if (!GameManager.localInstance.PackSound)
            {
                if (MasterAudioController.instance.CheckSoundToggle())
                    MasterAudioController.instance.PlayAudio(AudioEnum.PACK);
                GameManager.localInstance.PackSound = true;
            }

        }

        public void UpdateTimer(float value)
        {
            if (value == 0)
            {
                // avatharTimer.enabled = false;
                return;
            }

            avatharTimer.enabled = true;
            avatharTimer.fillAmount = 1 - value;
        }
        public void SetWinText()
        {


            betContainer.gameObject.SetActive(false);
            winnerBannerGlow2.SetActive(true);
            winnerBannerGlow.SetActive(true);
            winnerBanner.SetActive(true);
            if (MasterAudioController.instance.CheckSoundToggle())
                MasterAudioController.instance.PlayAudio(AudioEnum.WINNERTEENPATTI);
        }
        public string GetPlayerName()
        {
            return playerName.text;
        }
        public void InitTutorial()
        {
            ImageStatusColor.gameObject.SetActive(false);
            betContainer.gameObject.SetActive(false);
            winnerBannerGlow2.SetActive(false);
            winnerBannerGlow.SetActive(false);
            winnerBanner.SetActive(false);
            playerName.text = string.Empty;
            playerAmount.text = string.Empty;
            playerStatus.text = string.Empty;
            _chatContainer.SetActive(false);


            avatharTimer.fillAmount = 0;
            betContainer.gameObject.SetActive(false);

            HideLife();
            playerID = "";
            myPlayerState = null;
            IsInitialized = false;
            //   ResetCard();
            EndTurn();


        }
        public void InitUIForTutorial()
        {
            ShowGiftIcon();
            avatharTimer.fillAmount = 0;
            profilePictureGlow.SetActive(true);
            playerStatusShow();
            playerImage.SetActive(true);

            if (isMine)
            {
                playerBalance.SetActive(true);

                playerName.text = "You";

                string valAmount = CommonFunctions.Instance.TpvAmountSeparator(1000, true);


                //playerAmount.text = valAmount;
            }
            else
                playerName.text = TutorialBotnames[idOrder - 1];

            ImageStatusColor.gameObject.SetActive(false);
            playerStatus.text = "";
            if (idOrder == 2)
                playerAvatar.sprite = GameController.Instance.avatharPicture[5];
            else
                playerAvatar.sprite = GameController.Instance.avatharPicture[4];
            SetInfoText("Waiting..");
        }
        public void GetAmountFromPotTutorial(float betAmountVal, float profileAmountVal)
        {

            string val = CommonFunctions.Instance.TpvAmountSeparator(betAmountVal, true);

            betAmount.text = val;
            betContainer.position = potContainer.position;
            betContainer.gameObject.SetActive(true);

            double value = 0;
            TeenPattiTutorial.Instance.tablePot_Txt.text = CommonFunctions.Instance.GetAmountDecimalSeparator(value);


            betContainer.DOMove(dummyPosition.position, .5f, false).OnComplete(() =>
            {

                string val = betcoinImage + CommonFunctions.Instance.TpvAmountSeparator(profileAmountVal, true);


                // playerAmount.text = val;

                TeenPattiTutorial.Instance.tablePot_Obj.SetActive(false);

                betContainer.gameObject.SetActive(false);
            });


        }

        public void GiveAmountToPotTutorial(float betAmountVal, float potAmountVal, float profileAmountVal)
        {
            if (MasterAudioController.instance.CheckSoundToggle())
                MasterAudioController.instance.PlayAudio(AudioEnum.CHIPSOUND);
            isTimerRunning = false;
            avatharTimer.enabled = false;

            string valAmount = CommonFunctions.Instance.TpvAmountSeparator(betAmountVal, true);
            betAmount.text = valAmount;

            valAmount = profileAmountVal.ToString();
            valAmount = potAmountVal.ToString();

            betContainer.position = gameObject.transform.position;
            betContainer.gameObject.SetActive(true);
            DOTween.Sequence()
                  .Append(betContainer.DOMove(dummyPosition.position, .5f)).OnComplete(() =>
                  {
                      if (isMine)
                      {

                      }
                      // playerAmount.text = valAmount;

                  })
                  .Append(betContainer.DOMove(potContainer.position, .5f, false).OnComplete(() =>
                  {
                      TeenPattiTutorial.Instance.tablePot_Obj.SetActive(true);
                      TeenPattiTutorial.Instance.tablePot_Txt.text = valAmount;
                      betContainer.gameObject.SetActive(false);
                  }));

        }
        public IEnumerator SeeCardTutorial(CardData[] cards)
        {
            for (int i = 0; i < 3; i++)
            {
                SeeCardTutorial(cards[i], i);
                yield return new WaitForSeconds(0.1f);
            }
        }

        public void SeeCardTutorial(CardData card, int numCard)
        {
            jokerCard.SetActive(false);
            Card cardToChange = cardComponent[numCard];
            Sprite empty = TeenPattiTutorial.Instance.emptySprite;
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
                SetCardsDetails(cardToChange, cardNumber, suitImage, suitImage, empty, cardColor, fontColor);
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



            ResetTrigger(numCard);
            cards[numCard].gameObject.GetComponent<Animator>().SetTrigger("Card" + (numCard + 1) + "Reveal");

        }

        #endregion
        public void ClearUI(bool isforce = false)
        {
            Debug.Log("Clear UI called");

            isTimerRunning = false;
            //if (isMine)
            //    Debug.LogError("your ui is deleted");
            playerID = "";
            betContainer.gameObject.SetActive(false);
            RaiseUp.SetActive(false);
            winnerBannerGlow2.SetActive(false);
            winnerBannerGlow.SetActive(false);
            winnerBanner.SetActive(false);
            if (!isMine)
                playerBalance.SetActive(false);
            ShowInviteIcon();
            playerImage.SetActive(false);
            playerName.text = string.Empty;
            playerAmount.text = string.Empty;
            playerStatus.text = string.Empty;
            ImageStatusColor.gameObject.SetActive(false);
            ImageStatusColor.gameObject.SetActive(false);
            _chatContainer.SetActive(false);
            avatharTimer.fillAmount = 0;
            betContainer.gameObject.SetActive(false);
            HideLife();
            myPlayerState = null;

            IsInitialized = false;
            ResetCard(true);
            EndTurn();
        }

        public void ResetUI()
        {
            ShowLife();
            playerStatus.text = string.Empty;


            betContainer.gameObject.SetActive(false);


            avatharTimer.fillAmount = 0;


            playerAvatar.color = colorPacked;

        }


        public void InitUI()
        {

            ShowGiftIcon();
            avatharTimer.fillAmount = 0;
            profilePictureGlow.SetActive(true);
            playerStatusShow();
            playerImage.SetActive(true);
            RaiseUp.SetActive(false);

            if (GameManager.localInstance.gameState.currentState == 1)
            {
               // GamePlayUI.instance.isRaisedBet = false;
            }

            SetSpectator(false);



            if (isMine)
            {
                string val = betcoinImage + CommonFunctions.Instance.TpvAmountSeparator(APIController.instance.userDetails.balance, true);
                Debug.Log("*** InitUI  ----->" + val);
                playerAmount.text = val;
                playerName.text = "You";
            }
            else
            {

                playerName.text = myPlayerState.playerData.playerName;
            }

            ImageStatusColor.gameObject.SetActive(false);
            playerStatus.text = "";

            //if (myPlayerState.playerData.avatarIndex == (int)ProfilePicType.Index)
            //{
            //    int picPos = 0;
            //    if (int.TryParse(myPlayerState.playerData.profilePicURL, out picPos))
            //        playerAvatar.sprite = GameController.Instance.avatharPicture[picPos];
            //}
            //else if (myPlayerState.playerData.avatarIndex == (int)ProfilePicType.FBIndex)
            //{
            //    int picPos = 0;
            //    if (int.TryParse(myPlayerState.playerData.profilePicURL, out picPos))
            //        playerAvatar.sprite = GameController.Instance.fbAvatharPicture[picPos];
            //}
            //else if (myPlayerState.playerData.avatarIndex == (int)ProfilePicType.Facebook)
            //{
            //    if (ImageCacheUtils.Instance.HasCachedImage(myPlayerState.playerData.profilePicURL))
            //    {
            //        CachedImage cachedImage = new CachedImage();
            //        if (ImageCacheUtils.Instance.GetFromCachedImage(myPlayerState.playerData.profilePicURL, out cachedImage))
            //            playerAvatar.sprite = cachedImage.sprite;
            //        else
            //            playerAvatar.sprite = GameController.Instance.avatharPicture[UnityEngine.Random.Range(0, GameController.Instance.avatharPicture.Length)];
            //    }
            //    else
            //    {

            //    }
            //}
            //else
            //if (myPlayerState.playerData.avatarIndex == (int)ProfilePicType.Url)
            //{
            //    if (ImageCacheUtils.Instance.HasCachedImage(myPlayerState.playerData.profilePicURL))
            //    {
            //        CachedImage cachedImage = new CachedImage();
            //        if (ImageCacheUtils.Instance.GetFromCachedImage(myPlayerState.playerData.profilePicURL, out cachedImage))
            //            playerAvatar.sprite = cachedImage.sprite;
            //        else
            //            playerAvatar.sprite = GameController.Instance.avatharPicture[UnityEngine.Random.Range(0, GameController.Instance.avatharPicture.Length)];
            //    }
            //    else
            //    {
            //        Sprite fallbackImage = GameController.Instance.avatharPicture[UnityEngine.Random.Range(0, GameController.Instance.avatharPicture.Length)];

            //        ImageCacheUtils.Instance.LoadFromCacheOrDownload(myPlayerState.playerData.profilePicURL, myPlayerState.playerData.profilePicURL, fallbackImage, cacheImage =>
            //        {
            //            playerAvatar.sprite = cacheImage;
            //        });
            //    }
            //}
            //else
            //{
            //    playerAvatar.sprite = GameController.Instance.avatharPicture[UnityEngine.Random.Range(0, GameController.Instance.avatharPicture.Length)];
            //}
            playerAvatar.sprite = GameController.Instance.avatharPicture[myPlayerState.ui];
            ShowLife();
            IsInitialized = true;
        }

        public void SetSpectator(bool isTrue)
        {
            try
            {
                if (isTrue)
                {
                    StopAllCoroutines();
                    sitButton.SetActive(true);

                    profilebg.SetActive(false);
                }
                else
                {
                    sitButton.SetActive(false);
                    invite.SetActive(false);

                    profilebg.SetActive(true);
                }
            }
            catch
            {
            }
        }

        public void ShowInviteIcon()
        {

        }
        public void ShowGiftIcon()
        {

            inviteDuluxe.SetActive(false);

        }
        public void ShowLife()
        {
            lifes[0].SetActive(true);
            lifes[1].SetActive(true);
            lifes[2].SetActive(true);
        }
        public void HideLife()
        {
            lifes[0].SetActive(false);
            lifes[1].SetActive(false);
            lifes[2].SetActive(false);
        }

        public void GiveAmountToPot(double amount, double totalAmount ,bool BootCollection =false)
        {

            if (!isGetAmountFromPot)
            {
                isGiveAmountToPot = true;
                string valAmount = CommonFunctions.Instance.TpvAmountSeparator(amount, true);
                betAmount.text = valAmount;
                betContainer.position = gameObject.transform.position;
                betContainer.gameObject.SetActive(true);
                if (isMine)
                {


                    if (BootCollection)
                    {
                        RaiseUp.SetActive(true);
                    }





                    betContainer.DOAnchorPos(MovePostion, .5f, false).OnComplete(() =>
                        {

                            GamePlayUI.instance.potAmount.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.3f)
                        .SetEase(Ease.OutQuad)
                        .OnComplete(() =>
                        {
                            GamePlayUI.instance.GlowPotAmount.SetActive(false);
                            GamePlayUI.instance.potAmount.transform.DOScale(new Vector3(1f, 1f, 1f), 0.3f)
                                .SetEase(Ease.InQuad);
                        }).OnStart(() => { GamePlayUI.instance.GlowPotAmount.SetActive(true); });
                            double pot = GameManager.localInstance.gameState.totalPot;

                            string val = CommonFunctions.Instance.TpvAmountSeparator(APIController.instance.userDetails.balance, true);
                            Debug.Log("*** GiveAmountToPot1 ----->" + val);
                            playerAmount.text = val;
                            if (GameManager.localInstance.gameState.totalPot != 0)
                            {

                                GamePlayUI.instance.potAmount.text = CommonFunctions.Instance.TpvAmountSeparator(GameManager.localInstance.gameState.totalPot);

                            }
                            GamePlayUI.instance.ParentCoinsStack.GetComponent<TotalPotFiller>().SetGameObjectToThisList(CoinAnimation[0]);
                            CoinAnimation[0].transform.SetParent(potContainer.transform,true);

                            CoinAnimation.RemoveAt(0);
                            betContainer.gameObject.SetActive(false);
                            betContainer.anchoredPosition = StartPostion;
                            isGiveAmountToPot = false;
                            GamePlayUI.instance.potAmountPannel.SetActive(true);
                        }).OnStart(()=> {

                            //if (BootCollection)
                            //{
                            //    playerStatus.text = "Boot";
                            //    ImageStatusColor.gameObject.SetActive(true);
                            //    ImageStatusColor.color = color2;
                            //}
         
                            CoinAnimation[0].SetActive(true);
                        });

                    ////////////////////////////////////////////////
                    if (CoinAnimation[0] == null)
                    {
                        Debug.LogError("CoinAnimation[0] is null");
                    }
                    else
                    {
                        var coinAnimationCheck = CoinAnimation[0].GetComponent<CoinAnimationCheck>();
                        if (coinAnimationCheck == null)
                        {
                            Debug.LogError("CoinAnimationCheck component is missing");
                        }
                        else
                        {
                            coinAnimationCheck.SliderValueBasedOnAmount((float)amount);
                        }
                    }
                    ////////////////////////////////////////////////////////////////////////////////////////


                }
                else
                {

                    if (BootCollection)
                    {
                        RaiseUp.SetActive(true);
                    }

                    betContainer.position = dummyPosition.position;

                    DOTween.Sequence()
                      .Append(betContainer.DOAnchorPos(MovePostion, .5f)).OnPlay(() =>
                      {
                          CoinAnimation[0].GetComponent<Image>().sprite = GamePlayUI.instance.Coins[0];
                          CoinAnimation[0].SetActive(true);
                          playerBalance.SetActive(true);
                          playerAmount.text = "";

                      })
                      .AppendCallback(() =>
                      {
                          string val = CommonFunctions.Instance.TpvAmountSeparator(amount, true);

                          playerAmount.text = val;
                      })
                      .Append(betContainer.DOAnchorPos(MovePostion, .5f, false).OnComplete(() =>
                      {
            
                          GamePlayUI.instance.potAmount.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.3f)
                               .SetEase(Ease.OutQuad)
                                 .OnComplete(() =>
                                  {
                                      GamePlayUI.instance.GlowPotAmount.SetActive(false);
                                      GamePlayUI.instance.potAmount.transform.DOScale(new Vector3(1f, 1f, 1f), 0.3f)
                                     .SetEase(Ease.InQuad);
                                  }).OnStart(() => { GamePlayUI.instance.GlowPotAmount.SetActive(true); });

                          if (UIIndex == 1)
                          {

                              GamePlayUI.instance.ParentCoinsStack.GetComponent<TotalPotFiller>().SetGameObjectToThisListplayer1(CoinAnimation[0]);
                              CoinAnimation[0].transform.SetParent(potContainer.transform, true);
                              CoinAnimation.RemoveAt(0);
                          }
                          else if (UIIndex == 2)
                          {
                              GamePlayUI.instance.ParentCoinsStack.GetComponent<TotalPotFiller>().SetGameObjectToThisListplayer2(CoinAnimation[0]);
                              CoinAnimation[0].transform.SetParent(potContainer.transform, true);
                              CoinAnimation.RemoveAt(0);
                          }
                          else if (UIIndex == 4)
                          {
                              GamePlayUI.instance.ParentCoinsStack.GetComponent<TotalPotFiller>().SetGameObjectToThisListplayer4(CoinAnimation[0]);
                              CoinAnimation[0].transform.SetParent(potContainer.transform, true);
                              CoinAnimation.RemoveAt(0);
                          }
                          else if (UIIndex == 5)
                          {
                              GamePlayUI.instance.ParentCoinsStack.GetComponent<TotalPotFiller>().SetGameObjectToThisListplayer5(CoinAnimation[0]);
                              CoinAnimation[0].transform.SetParent(potContainer.transform, true);
                              CoinAnimation.RemoveAt(0);
                          }

                          double pot = GameManager.localInstance.gameState.totalPot;
                          CancelInvoke("HideProfileAmount");
                          Invoke("HideProfileAmount", 2);


                          if (GameManager.localInstance.gameState.totalPot != 0)
                          {

                              GamePlayUI.instance.potAmount.text = CommonFunctions.Instance.TpvAmountSeparator(GameManager.localInstance.gameState.totalPot);

                          }

                          betContainer.gameObject.SetActive(false);
                          betContainer.anchoredPosition = StartPostion;
                          isGiveAmountToPot = false;
                          GamePlayUI.instance.potAmountPannel.SetActive(true);
                      }).OnStart(() => {

      
                 

    
                         // CoinAnimation[0].GetComponent<CoinAnimationCheck>().SliderReset();
                          try
                          {
                            //  CoinAnimation[0].GetComponent<CoinAnimationCheck>().SliderValueBasedOnAmount((float)amount);
                          }
                          catch
                          {
                              Debug.Log("Check this Amount =============> " + amount);
                          }

                      }));

                    ////////////////////////////////////////////////
                    if (CoinAnimation[0] == null)
                    {
                        Debug.LogError("CoinAnimation[0] is null");
                    }
                    else
                    {
                        var coinAnimationCheck = CoinAnimation[0].GetComponent<CoinAnimationCheck>();
                        if (coinAnimationCheck == null)
                        {
                            Debug.LogError("CoinAnimationCheck component is missing");
                        }
                        else
                        {
                            coinAnimationCheck.SliderValueBasedOnAmount((float)amount);
                        }
                    }
                    ////////////////////////////////////////////////////////////////////////////////////////


                }
            }
            else
            {


                if (isMine)
                {

                    string val = CommonFunctions.Instance.TpvAmountSeparator(GameManager.localInstance.GetPlayerState(myPlayerState.playerData.playerID).playerData.money, true);
                    // playerAmount.text = val;
                }
                else
                {

                    string val = CommonFunctions.Instance.TpvAmountSeparator(amount, true);
                    Debug.Log("GiveAmountToPot3 ----->" + val);
                    playerAmount.text = val;
                    playerBalance.SetActive(true);
                    CancelInvoke("HideProfileAmount");
                    Invoke("HideProfileAmount", 2);
                }
                if (myPlayerState.playerData.playerID == GameController.Instance.CurrentPlayerData.GetPlayfabID())
                {

                }

            }

        }

        public void setRaiseOf()
        {
            RaiseUp.SetActive(false);
        }

        public void GetAmountFromPot(float amount)
        {
            Debug.Log("Check pot location");
            StartCoroutine(GetAmount(amount));
        }
       

        IEnumerator GetAmount(float amount)
        {

            switch (amount)
            {
                case <= 10:
                    PotSecondShowSplit.sprite = GamePlayUI.instance.Coins[5];
                    break;
                case <= 30:
                    PotSecondShowSplit.sprite = GamePlayUI.instance.Coins[5];
                    break;
                case <= 50:
                    PotSecondShowSplit.sprite = GamePlayUI.instance.Coins[5];
                    break;
                case <= 100:
                    PotSecondShowSplit.sprite = GamePlayUI.instance.Coins[5];
                    break;
                case <= 150:
                    PotSecondShowSplit.sprite = GamePlayUI.instance.Coins[5];
                    break;
                case <= 200:
                    PotSecondShowSplit.sprite = GamePlayUI.instance.Coins[5];
                    break;
                case <= 250:
                    PotSecondShowSplit.sprite = GamePlayUI.instance.Coins[6];
                    break;
                case <= 300:
                    PotSecondShowSplit.sprite = GamePlayUI.instance.Coins[7];
                    break;
                case <= 360:
                    PotSecondShowSplit.sprite = GamePlayUI.instance.Coins[8];
                    break;
            }


            GameManager.localInstance.CountTemp++;
            yield return new WaitForSeconds(2);
            isGetAmountFromPot = true;
            string val = CommonFunctions.Instance.TpvAmountSeparator(amount, true);

            betAmount.text = val;
            betContainer.position = potContainer.position;
            betContainer.gameObject.SetActive(true);

            double value = 0;

            GamePlayUI.instance.potAmount.text = CommonFunctions.Instance.TpvAmountSeparator(value);

            GamePlayUI.instance.potAmountPannel.SetActive(false);
            if (MasterAudioController.instance.CheckSoundToggle())
                MasterAudioController.instance.PlayAudio(AudioEnum.CHIPSOUND);
            if (isMine)
            {

                betContainer.gameObject.SetActive(false);
                betContainer.DOMove(dummyPosition.position, .5f, false).OnComplete(() =>
                {

                    string val = CommonFunctions.Instance.TpvAmountSeparator(GameManager.localInstance.GetPlayerState(myPlayerState.playerData.playerID).playerData.money, true);
                    if (myPlayerState.playerData.playerID == GameController.Instance.CurrentPlayerData.GetPlayfabID())
                    {

                        string valMoney = CommonFunctions.Instance.TpvAmountSeparator(myPlayerState.playerData.money, true);

                    }
                    isGetAmountFromPot = false;
     
                });
                Debug.Log(GameManager.localInstance.CountTemp + "Check pot location");

                if (GameManager.localInstance.CountTemp == 1)
                {
                    GamePlayUI.instance.ParentCoinsStack.transform.DOMove(dummyPosition.position, .5f, false).OnComplete(() => {

                        GamePlayUI.instance.ParentCoinsStack.gameObject.SetActive(false);
                        GamePlayUI.instance.ParentCoinsStack.gameObject.transform.localPosition = new Vector3(0, 590, 0);


                    });
                }
                else
                {

                    GamePlayUI.instance.ParentCoinsStack.gameObject.SetActive(false);
                    GamePlayUI.instance.ParentCoinsStack.gameObject.transform.localPosition = new Vector3(0, 590, 0);
                    PotSecondShowSplit.transform.DOMove(dummyPosition.position, .5f, false).OnComplete(() =>
                    {
                        PotSecondShowSplit.gameObject.SetActive(false);
                        PotSecondShowSplit.gameObject.transform.localPosition = PotSplitPositionStart;


                    }).OnStart(() =>
                    {
                        PotSecondShowSplit.gameObject.SetActive(true);
                    });
                }



            }
            else
            {
                betContainer.gameObject.SetActive(false);
                betContainer.DOMove(playerBalance.GetComponent<RectTransform>().position, .5f, false).OnComplete(() =>
                {
                    betContainer.gameObject.SetActive(false);
                    string val = CommonFunctions.Instance.TpvAmountSeparator(amount, true);
                    playerAmount.text = val;
                    playerBalance.SetActive(true);
                    CancelInvoke("HideProfileAmount");
                    Invoke("HideProfileAmount", 2);
                    isGetAmountFromPot = false;
                    betContainer.gameObject.SetActive(false);

                });

                Debug.Log(GameManager.localInstance.CountTemp + "Check pot location");


                if(GameManager.localInstance.CountTemp == 1)
                {
                    GamePlayUI.instance.ParentCoinsStack.transform.DOMove(dummyPosition.position, .5f, false).OnComplete(() => {

                        GamePlayUI.instance.ParentCoinsStack.gameObject.SetActive(false);
                        GamePlayUI.instance.ParentCoinsStack.gameObject.transform.localPosition = new Vector3(0, 590, 0);


                    });
                }
                else
                {

                    GamePlayUI.instance.ParentCoinsStack.gameObject.SetActive(false);
                    GamePlayUI.instance.ParentCoinsStack.gameObject.transform.localPosition = new Vector3(0, 590, 0);
                    PotSecondShowSplit.transform.DOMove(dummyPosition.position, .5f, false).OnComplete(() =>
                    {
                        PotSecondShowSplit.gameObject.SetActive(false);
                        PotSecondShowSplit.gameObject.transform.localPosition = PotSplitPositionStart;


                    }).OnStart(() =>
                    {
                        PotSecondShowSplit.gameObject.SetActive(true);
                    });
                }


               
            }
        }

        cardAnim cardAnimination;
        public void ResetTrigger(int cardID)
        {
            cardAnimination = cardAnim.Reset;

            if (cards[cardID].gameObject == null)
            {

            }
            cards[cardID].gameObject.GetComponent<Animator>().ResetTrigger("Card" + (cardID + 1) + "Reveal");
            cards[cardID].gameObject.GetComponent<Animator>().ResetTrigger("Card" + (cardID + 1) + "Deal");
            cards[cardID].gameObject.GetComponent<Animator>().ResetTrigger("Reset");


        }
        bool isseeCard;
        public void ResetCard(bool isforce = false)
        {

            isseeCard = false;
            if (GameManager.localInstance != null)
            {
                if (GameManager.localInstance.gameState.currentState == 2 && !isforce)
                    return;
            }
            for (int i = 0; i < 3; i++)
            {
                jokerCard.SetActive(false);
                ResetTrigger(i);
                cards[i].gameObject.GetComponent<Animator>().SetTrigger("Reset");

            }
       ;
        }
        IEnumerator GetAmountFromPotUpdate(float amount)
        {

            yield return new WaitForSecondsRealtime(1.5f);
            if (!isGiveAmountToPot)
            {
                isGetAmountFromPot = true;

                string val = CommonFunctions.Instance.TpvAmountSeparator(amount, true);

                betAmount.text = val;
                betContainer.transform.localPosition = localEndPosision;
                betContainer.gameObject.SetActive(true);

                GamePlayUI.instance.potAmountPannel.SetActive(false);
                double value = 0;


                GamePlayUI.instance.potAmount.text = CommonFunctions.Instance.TpvAmountSeparator(value);
                if (MasterAudioController.instance.CheckSoundToggle())
                    MasterAudioController.instance.PlayAudio(AudioEnum.CHIPSOUND);
                betContainer.GetComponent<RectTransform>().DOLocalMove(localStartPosision, .5f, false).OnComplete(() =>
                {

                    if (isMine)
                    {

                        string val = CommonFunctions.Instance.TpvAmountSeparator(GameManager.localInstance.GetPlayerState(myPlayerState.playerData.playerID).playerData.money, true);


                        // playerAmount.text = val;


                    }
                    else
                    {

                        string val = CommonFunctions.Instance.TpvAmountSeparator(amount, true);

                        playerAmount.text = val;

                        playerBalance.SetActive(true);
                        CancelInvoke("HideProfileAmount");
                        Invoke("HideProfileAmount", 2);
                    }
                    betContainer.gameObject.SetActive(false);
                    isGetAmountFromPot = false;

                });
            }
            else
            {
                StartCoroutine(GetAmountFromPotUpdate(amount));

            }
        }
        public void SetProfileAmount()
        {
            string val = CommonFunctions.Instance.TpvAmountSeparator(myPlayerState.playerData.money, true);
            // playerAmount.text = val;


        }

        public void DealCard(int numCard)
        {
            ResetTrigger(numCard);
            cardAnimination = cardAnim.Deal;
            cards[numCard].gameObject.GetComponent<Animator>().SetTrigger("Card" + (numCard + 1) + "Deal");
            StartCoroutine(nameof(DealSoundDelay));
        }

        public IEnumerator DealSoundDelay()
        {
            yield return new WaitForSeconds(0.1f);
            if (MasterAudioController.instance.CheckSoundToggle())
                MasterAudioController.instance.PlayAudio(AudioEnum.CARDDEALTEENPATTI);
            yield return new WaitForSeconds(0.1f);
            if (MasterAudioController.instance.CheckSoundToggle())
                MasterAudioController.instance.PlayAudio(AudioEnum.CARDDEALTEENPATTI);
            yield return new WaitForSeconds(0.1f);
            if (MasterAudioController.instance.CheckSoundToggle())
                MasterAudioController.instance.PlayAudio(AudioEnum.CARDDEALTEENPATTI);
        }


        public IEnumerator SetCard(bool isForce = false)
        {

            if ((cardAnimination == cardAnim.Reveal || myPlayerState.isSpectator || myPlayerState.hasPacked) && !isForce)
                yield break;
            if (myPlayerState.playerData.currentCards[0].rankCard == 0 || myPlayerState.playerData.currentCards[0].suitCard == 0)
                yield break;
            for (int i = 0; i < 3; i++)
            {
                SeeCard(myPlayerState.playerData.currentCards[i], i);
                yield return new WaitForSeconds(0f);
            }
            isseeCard = true;
        }
        bool isPlaying(Animator anim, string stateName)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName(stateName) &&
                    anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
                return true;
            else
                return false;
        }
        public void SeeCard(CardData card, int numCard)
        {
            if (!IsFull() || GameManager.localInstance.gameState.currentState == 1 || GameManager.localInstance.gameState.currentState == 0)
                return;
            if (isMine)
            {
                GamePlayUI.instance.HandStrengthMeter();

                GamePlayUI.instance.SeeButtonActive(false);
            }
            if (GameController.Instance.CurrentGameMode == GameMode.JOKER && numCard == 2)
            {
                jokerCard.SetActive(true);
            }
            else
            {
                jokerCard.SetActive(false);
            }
            Card cardToChange = cardComponent[numCard];
            Sprite empty = GamePlayUI.instance.emptySprite;
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
                SetCardsDetails(cardToChange, cardNumber, suitImage, suitImage, empty, cardColor, fontColor);
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



            ResetTrigger(numCard);
            cardAnimination = cardAnim.Reveal;

            cards[numCard].gameObject.GetComponent<Animator>().SetTrigger("Card" + (numCard + 1) + "Reveal");
            if (isMine)
            {
                Invoke(nameof(FoldSound), 0.5f);
                StartCoroutine(GameManager.localInstance.fetchData());
                GamePlayUI.instance.strengthMeterActive(true);
            }
            else
            {

                Invoke(nameof(FoldSound), 1f);
            }


        }


        public void FoldSound()
        {
            if (MasterAudioController.instance.CheckSoundToggle())
                MasterAudioController.instance.PlayAudio(AudioEnum.FOLD);
        }
        public void SetCardsDetails(Card cardToChange, string rankText, Sprite smallSuitSprite, Sprite bigSpriteImage, Sprite coutSprite, Color cardColor, Color fontColor)
        {

            cardToChange.cardRankTxt.ConvertAll(x => x.text = rankText);
            cardToChange.suitImageSmall.ConvertAll(x => x.sprite = smallSuitSprite);
            cardToChange.cardRankTxt.ConvertAll(x => x.color = fontColor);
            cardToChange.suitImageSmall.ConvertAll(x => x.color = cardColor);
            cardToChange.suitImageBig.ConvertAll(x => x.color = cardColor);
        }




        #region  DoTweenCardsAnimation


        public void CardDeal()
        {

            poolcards = TeenPattiPoolCards.PoolCardInstance.GetPooledCards();
            poolcards.SetActive(true);

            DOTween.Sequence()
                           .Append(poolcards.GetComponent<RectTransform>().DOMove(dummyPositionCards.position, .5f))
                           .Append(poolcards.GetComponent<RectTransform>().DOMove(dummyPositionCards.position, .5f))
                           .Append(poolcards.GetComponent<RectTransform>().DOMove(dummyPositionCards.position, .5f, false).OnComplete(() =>
                           {

                           }));

        }

        #endregion DoTweenCardsAnimation

        public void playerStatusShow()
        {

        }

    }

}
