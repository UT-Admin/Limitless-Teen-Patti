using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using System.Globalization;

namespace TP.Poker
{
    public class PlayerUI : MonoBehaviour
    {
        public PlayerManager myPlayerManager;
        public PlayerState myPlayerState;

        [Header("RoomProperties")]        
        [SerializeField] GameObject invitebutton;
        [SerializeField] GameObject playerProfile;
        [SerializeField] GameObject sitButton;

        

        //[SerializeField] private GameObject[] cards;
        //public Card[] cardComponent;

        [Space(10)]
        [Header("Containers")]
        [SerializeField] private GameObject playerStatusContainer;
        [SerializeField] private GameObject betContainer;     

        [Header("StatusContainer")]        
        [SerializeField] private Image statusImage;
        [SerializeField] private TextMeshProUGUI statusText;

        [Header("Text Fields")]
        [SerializeField] private TextMeshProUGUI playerNameTxt;
        [SerializeField] private TextMeshProUGUI playerStatusTxt;
        public TextMeshProUGUI playerAmount;

        [Header("BetContainer")]
        [SerializeField] private TextMeshProUGUI betAmount;
        [SerializeField] private TextMeshProUGUI betText;
        [SerializeField] private Image betImage;
        [SerializeField] private Image chipImage;
        
        

        [Header("Sprites And Images")]
        public  Image profileImage;
        [SerializeField] private Image avatharTimer;

        [Header("GameObjects")]
        [SerializeField] private GameObject playerImage;
        //[SerializeField] private GameObject profilePictureGlow;
        //[SerializeField] private GameObject playerBalance;
        [SerializeField] GameObject winnerBanner;
        [SerializeField] GameObject looseBanner;

        [Header("String")]
        public string playerID = "";

        [Header("Int")]
        [SerializeField] private int availableLives;

        [Header("Bool")]
        public bool IsInitialized = false;
        [SerializeField] private bool isMine = false;
        [SerializeField] bool cardColorWhite;

        [Space(30)]
        [SerializeField] private Transform localStartPosision;
        [SerializeField] private Transform localEndPosision;

        public TextMeshProUGUI rankKind;
        public TextMeshProUGUI looseRankText;

        [Space(30)]
        [Header("Coin Movement Ease")]
        public Ease xEase;
        public Ease yEase;

        [SerializeField] private TMP_Text _chat;
        [SerializeField] private GameObject _chatContainer;

        [SerializeField] private Sprite cashSprite;
        [SerializeField] private Sprite chipSprite;

        bool playerDisconnected = false;

        private void OnEnable()
        {
            ClearUI();
        }


        public void ClearUI()
        {
           // Debug.LogError("Clear Ui");
            winnerBanner.gameObject.SetActive(false);
            looseBanner.gameObject.SetActive(false);
           
            playerStatusContainer.gameObject.SetActive(false);
            playerImage.SetActive(false);
            playerNameTxt.gameObject.SetActive(false);
            playerStatusContainer.SetActive(false);
            betContainer.gameObject.SetActive(false);
            playerAmount.gameObject.SetActive(false);
            playerProfile.gameObject.SetActive(false);
            if(!isMine )
                    invitebutton.gameObject.SetActive(true);
            avatharTimer.enabled = false;
            rankKind.text = string.Empty;
            looseRankText.text = string.Empty;
            playerID = string.Empty;
            betAmount.text = string.Empty;
            playerAmount.text = string.Empty;
            playerNameTxt.text = string.Empty;
            playerStatusTxt.text = string.Empty;
            statusText.text = string.Empty;
            playerDisconnected = false;
            profileImage.color = Color.white;
            myPlayerState = null;
            IsInitialized = false;
            _chatContainer.SetActive(false);
            
           
            ResetCard();            
            EndTurn();
        }

        public void UpdatePlayer(PlayerManager playerManager)
        {
            myPlayerManager = playerManager;
           
        }

        public void StartTurn()
        {
            StopCoroutine("StartTimer");
            StartCoroutine("StartTimer");
        }

        public void EndTurn()
        {
            avatharTimer.enabled = false;
            StopCoroutine("StartTimer");
            //------------------------
            //warnclip.Stop();
        }

   

        public void InitUI()
        {
            //playerImage.SetActive(true);
            playerNameTxt.gameObject.SetActive(true);

            playerNameTxt.text = CommonFunctions.Instance.GetTruncatedPlayerName(myPlayerManager.GetPlayerState().playerData.playerName);

            playerStatusContainer.gameObject.SetActive(true);
            statusText.text = "Waiting";
            //GameManager.instance.gameHUD.pokerHandStrengthMeter.ClearStrength();
            profileImage.color = Color.gray;
            playerDisconnected = false;
            UpdatePlayerAmtTxt(myPlayerManager.GetPlayerState().playerData.money);

            playerImage.SetActive(true);

            playerStatusContainer.gameObject.SetActive(false);

            playerAmount.gameObject.SetActive(true);

            Debug.LogError("Player Profile Active");

            playerProfile.gameObject.SetActive(true);
            invitebutton.gameObject.SetActive(false);

            if(GameController.Instance.CurrentAmountType == CashType.CASH)
            {
                chipImage.sprite = cashSprite;
            }
            else
            {
                chipImage.sprite = chipSprite;
            }


            if (isMine)
            {
                playerNameTxt.text = "";
            }

            //---------------------------------------------------------------------------------------------------------------
            //  profileImage.sprite = GameHUD.instance.GetDP(myPlayerManager.GetPlayerState().playerData.playerImageIndex);
            //---------------------------------------------------------------------------------------------------------------

            //profileImage.sprite = GameController.Instance.avatharPicture[myPlayerManager.GetPlayerState().playerData.avatarIndex];

            if (myPlayerManager.GetPlayerState().playerData.avatarIndex == (int)ProfilePicType.Index)
            {
                int picPos = 0;
                if (int.TryParse(myPlayerManager.GetPlayerState().playerData.profilePicURL, out picPos))
                    profileImage.sprite = GameController.Instance.avatharPicture[picPos];
            }
            else if (myPlayerManager.GetPlayerState().playerData.avatarIndex == (int)ProfilePicType.FBIndex)
            {
                int picPos = 0;
                if (int.TryParse(myPlayerManager.GetPlayerState().playerData.profilePicURL, out picPos))
                    profileImage.sprite = GameController.Instance.fbAvatharPicture[picPos];
            }
            else if (myPlayerManager.GetPlayerState().playerData.avatarIndex == (int)ProfilePicType.Facebook)
            {
                if (ImageCacheUtils.Instance.HasCachedImage(myPlayerManager.GetPlayerState().playerData.profilePicURL))
                {
                    CachedImage cachedImage = new CachedImage();
                    if (ImageCacheUtils.Instance.GetFromCachedImage(myPlayerManager.GetPlayerState().playerData.profilePicURL, out cachedImage))
                        profileImage.sprite = cachedImage.sprite;
                    else
                        profileImage.sprite = GameController.Instance.avatharPicture[UnityEngine.Random.Range(0, GameController.Instance.avatharPicture.Length)];
                }
                else
                {
                  
                }
            }
            else
            if (myPlayerManager.GetPlayerState().playerData.avatarIndex == (int)ProfilePicType.Url)
            {
                if (ImageCacheUtils.Instance.HasCachedImage(myPlayerManager.GetPlayerState().playerData.profilePicURL))
                {
                    CachedImage cachedImage = new CachedImage();
                    if (ImageCacheUtils.Instance.GetFromCachedImage(myPlayerManager.GetPlayerState().playerData.profilePicURL, out cachedImage))
                        profileImage.sprite = cachedImage.sprite;
                    else
                        profileImage.sprite = GameController.Instance.avatharPicture[UnityEngine.Random.Range(0, GameController.Instance.avatharPicture.Length)];
                }
                else
                {
                    Sprite fallbackImage = GameController.Instance.avatharPicture[UnityEngine.Random.Range(0, GameController.Instance.avatharPicture.Length)];

                    ImageCacheUtils.Instance.LoadFromCacheOrDownload(myPlayerManager.GetPlayerState().playerData.profilePicURL, myPlayerManager.GetPlayerState().playerData.profilePicURL, fallbackImage, cacheImage => {
                        profileImage.sprite = cacheImage;
                    });
                }

                /*WebApiManager.Instance.GetDownloadImage(myPlayerManager.GetPlayerState().playerData.profilePicURL, (bool isNetworkError, bool isHttpError, string error, Texture2D imageTex) => {
                    if (!isNetworkError || !isHttpError)
                    {
                        var _tex = CommonFunctions.Instance.DoScaleTex(imageTex, 512, 512);
                        playerAvatar.sprite = CommonFunctions.Instance.CreateSpriteFromTex(_tex, _tex.width, _tex.height);
                    }
                });*/
            }            
            IsInitialized = true;
        }

        private void Start()
        {
            profileImage = playerImage.GetComponent<Image>();
        }

        public void ResetPlayerStatusContainer()
        {
            playerStatusContainer.gameObject.SetActive(false);
            statusText.text = string.Empty;
        }

        public void UpdatePlayerAmtTxt(double value)
        {
            if (playerAmount != null)
                playerAmount.text = CommonFunctions.Instance.GetAmountDecimalSeparator(value);
        }

        public void StopTimer()
        {
            avatharTimer.enabled = false;
            StopCoroutine("StartTimer");
        }

        public void UpdateBetAmtText(double value)
        {
           // Debug.Log(value);
            betAmount.gameObject.SetActive(value != 0);
            betAmount.text = CommonFunctions.Instance.GetAmountDecimalSeparator(value);
        }


       

        public void SetReconnect()
        {
            playerDisconnected = false;
            playerStatusContainer.gameObject.SetActive(false);
            statusText.text = "";
            profileImage.color = Color.white;
        }

        public void SetWaiting()
        {
            playerDisconnected = false;
            playerStatusContainer.gameObject.SetActive(false);
            statusText.text = "Waiting";
            profileImage.color = Color.white;
        }

        public void SetDisconnected()
        {
            playerDisconnected = true;
            if (playerStatusContainer)
            {
                playerStatusContainer.gameObject.SetActive(true);
                statusText.text = "OFFLINE";
                profileImage.color = Color.gray;
            }
        }

        public bool IsFull()
        {
            if (playerID == "")
                return false;
            else
                return true;
        }

        public void ShowWinnerBanner(CardsCombination cardCombo,bool enableText = true)
        {
            //Vector3 newScale = 1.2f * Vector3.one;
            // winnerBanner.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
            // winnerBanner.GetComponent<SpriteRenderer>().DOFade(1, 0.5f);
            // winnerBanner.transform.localScale = new Vector3(15, 15, 15);
            //winnerBanner.transform.DOScale(newScale, 0.5f).OnComplete(() =>
            //{
            //    winnerBanner.transform.DOScale(newScale * 1.1f, 0.4f).SetLoops(20, LoopType.Yoyo);
            //});
            rankKind.transform.parent.gameObject.SetActive(enableText);
            looseBanner.gameObject.SetActive(false);
            CardsCombination combo = (cardCombo);
            rankKind.text = GetTextForCardCombinations(combo);
            looseRankText.text = GetTextForCardCombinations(combo);
            winnerBanner.SetActive(true);
        }

        public void ShowLooseBanner()
        {
            winnerBanner.SetActive(false);
            
           
            looseBanner.SetActive(true);
        }

        string GetTextForCardCombinations(CardsCombination cardCombination)
        {
            string toShow = "";
          

            return toShow;
        }


        public void CloseWinnerBanner()
        {
            playerStatusContainer.SetActive(false);
            statusText.text = "";
            playerDisconnected = false;
            profileImage.color = Color.white;
            winnerBanner.SetActive(false);
            looseBanner.SetActive(false);
        }

        public void SetBet(double amount, int totalAmount)
        {
           
        }

        public void SetSpectator(bool isTrue)
        {
            if (isTrue)
            {
                StopAllCoroutines();
                ResetCard();
                sitButton.SetActive(true);
                invitebutton.SetActive(false);
                playerProfile.SetActive(false);
            }
            else
            {
                Debug.LogError("Player Profile Active");
                playerProfile.SetActive(true);
                sitButton.SetActive(false);
            }
        }

        public void ShowInviteIcon()
        {
            invitebutton.SetActive(true);
            //gift.SetActive(false);
        }
        public void ShowGiftIcon()
        {
            invitebutton.SetActive(false);
            //gift.SetActive(true);
        }


        public void SetStatus(BetType betType)
        {
            string betText = GetCallText(betType);
            if (betType == BetType.Blind)
                return;
            playerStatusContainer.SetActive(true);
            statusText.text = betText;
            playerDisconnected = false;
            profileImage.color = Color.white;
        }

        public string GetCallText(BetType betType)
        {
            string returnText = "";
            
            return returnText;
        }

        void SetContainerColor(Color colour)
        {

            betImage.color = colour;
            betAmount.color = colour;
            statusImage.color = colour;
        }


        public void ClearStatus(int amount, bool hasFolded = false)
        {
            if (!hasFolded)
            {
                playerStatusContainer.SetActive(false);
                statusText.text = "";
                playerDisconnected = false;
                profileImage.color = Color.white;
            }
        }

        public void GiveAmountToPot(float amount)
        {
            UpdateBetAmtText(amount);
            // playerAmount.text = myPlayerState.playerData.money.ToString();
            betContainer.transform.localPosition = localStartPosision.localPosition;
            if (MasterAudioController.instance.CheckSoundToggle())
                MasterAudioController.instance.PlayAudio(AudioEnum.CHIPSOUND);
            //betContainer.SetActive(true);
            betContainer.transform.DOScale(1f, 0.5f);
            betContainer.GetComponent<Transform>().DOLocalMoveX(localEndPosision.localPosition.x, .4f).SetDelay(1f).SetEase(xEase);
            betContainer.GetComponent<Transform>().DOLocalMoveY(localEndPosision.localPosition.y, .4f).SetDelay(1f).SetEase(yEase).OnComplete(() =>
            {

                if (myPlayerState.playerData.money == 0)
                    playerAmount.text = "All-IN";
                else
                    UpdatePlayerAmtTxt(myPlayerState.playerData.money);

             
            }); 

        }
        public void GetAmountFromPot(double amount)
        {
            StartCoroutine(GetAmount(amount));
        }
        IEnumerator GetAmount(double amount)
        {
            yield return new WaitForSeconds(0.01f);
            UpdateBetAmtText(amount);
            betContainer.transform.localPosition = localEndPosision.localPosition;
            betContainer.SetActive(true);
            
            DOTween.Kill(betContainer.GetComponent<RectTransform>());
            //Debug.Log("PotAmount3");
            if (MasterAudioController.instance.CheckSoundToggle())
                MasterAudioController.instance.PlayAudio(AudioEnum.CHIPSOUND);
            //GameManager.instance.gameHUD.potAmount.text = "0";
           
            betContainer.transform.DOScale(1.25f, 1.25f);
            betContainer.GetComponent<RectTransform>().DOLocalMove(localStartPosision.localPosition, 1.25f, false).OnComplete(() => {
                UpdatePlayerAmtTxt(myPlayerState.playerData.money);
                betContainer.SetActive(false);
            });
        }
        public void SetInfoText()
        {

           // Debug.Log(myPlayerManager.GetPlayerState().isSpectator+myPlayerState.playerData.playerName);

            //return;

            //if (myPlayerManager.GetPlayerState().hasFolded)
            //{
            //    statusText.text = "FOLD";
            //}
            //if (myPlayerManager.GetPlayerState().isAllIn)
            //{
            //    statusText.text = "All in";
            //}

  
            {
                playerProfile.SetActive(true);
                if (isMine)
                {                   
                    sitButton.SetActive(false);
                }
                else
                {
                    invitebutton.SetActive(false);                 
                }
            }                    
        }

        public void ResetCard()
        {
           
        }

        


        public void DealCard(int numCard)
        {
            int tempIndex = numCard;
            tempIndex = GameController.Instance.CurrentGameMode == GameMode.OMAHAPOKER ? tempIndex : (tempIndex + 1);


            //MasterAudioController.instance.PlayAudio(AudioEnum.CARDDEALTEENPATTI);
        


         

        }

        public IEnumerator SetCard()
        {
            //Debug.LogError("test card open");
            int numberOfCard = GameController.Instance.CurrentGameMode == GameMode.OMAHAPOKER ? 4 : 2;

            if (!isMine)
            {
                for (int i = 0; i < numberOfCard; i++)
                {
                    //            myPlayerManager.GetPlayerState().playerData.currentCards[i].isClose = false;
                    //CardData data = new CardData();
                    //Debug.Log(myPlayerState.playerData.currentCards[i].cardIndex+ "CardIndexxxxxxxxxxxx");
                    SeeCard(myPlayerState.playerData.currentCards[i], i);
                    yield return new WaitForSeconds(0.1f);
                }
            }
        }

        //void ApplyCardComponents(string)
        //{
        //    foreach (var item in cardComponent)
        //    {
        //        foreach (var rank in item.cardRankTxt)
        //        {
        //            rank.text = "J";
        //        }
        //        foreach (var cImage in item.courtImage)
        //        {
        //            cImage.sprite = GameHUD.instance.jack;
        //        }

        //    }
        //}



        public void SeeCard(CardData card, int numCard)
        {
            if (GameController.Instance.CurrentGameMode != GameMode.OMAHAPOKER)
                numCard++;

            Debug.Log(numCard + "Card____________Card" + card.rankCard +"_______"+ card.suitCard + "_________"+ card.cardIndex);


           
            

            //if (!isMine)
            //{
            //    cards[numCard].gameObject.GetComponent<Animator>().ResetTrigger("Card" + (numCard + 1) + "Deal");
            //    if (!myPlayerState.hasFolded)
            //        cards[numCard].gameObject.GetComponent<Animator>().SetTrigger("Card" + (numCard + 1) + "Reveal");
            //}

          


        }

        

 



    }




    public enum BetType
    {
        Call,
        Check,
        Raise,
        Fold,
        AllIn,
        Bet,
        Blind
    }
}

[System.Serializable]
public class Card
{
    [NonReorderable]
    public List<TextMeshProUGUI> cardRankTxt;
    [NonReorderable]
    [Space(1)]
    public List<Image> suitImageSmall;
    [NonReorderable]
    [Space(1)]
    public List<Image> suitImageBig;
    [NonReorderable]
    [Space(1)]
    public List<Image> courtImage;

    public Image backImage;
    public Image packedImage;
    public GameObject glowImage;

    [HideInInspector]
    public int cardIndex;

    public void Intialize(int index)
    {
        //packedImage.gameObject.SetActive(false);
        //backImage.gameObject.SetActive(true);
        cardIndex = index;
    }

    public void PackCard()
    {
        glowImage.gameObject.SetActive(false);
        packedImage.gameObject.SetActive(true);
    }
    public void UnPack()
    {
        packedImage.gameObject.SetActive(false);
        glowImage.gameObject.SetActive(true);
    }
}