using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using TP;
using UnityEngine;
using UnityEngine.UI;

public class SideShowWinnerEffect : MonoBehaviour
{
    bool wasActiveEarlier;
    bool wasActiveEarlieri;
    public static SideShowWinnerEffect Instance;
    public GameObject[] BlastEffect;
    public GameObject[] LostEffect;
    public Image ProfileA;
    public Image ProfileB;
    public Sprite[] CardList;
    public TMP_Text playerAName;
    public TMP_Text playerBName;
    public Animator Effect1;
    public Animator Effect2;
    public Animator Blast;
    public Animator Blast1;
    public Image PlayerACardStrengthImage;
    public Image PlayerBCardStrengthImage;
    public Card[] cardComponent;
    public Card[] cardComponentB;
    public GameObject[] ArrowIdicators;
    public GameObject[] ShowSideShowText;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {

        if (TeenpattiGameUIHandler.instance.HowToplay.activeSelf)
        {
            wasActiveEarlier = true;
            TeenpattiGameUIHandler.instance.HowToplay.SetActive(false);
        }
        else
        {
            wasActiveEarlier = false;
        }

        if (TeenpattiGameUIHandler.instance.teenpattiInfoPanel.gameObject.activeSelf)
        {
            wasActiveEarlieri = true;
            TeenpattiGameUIHandler.instance.teenpattiInfoPanel.gameObject.SetActive(false);
        }
        else
        {
            wasActiveEarlieri = false;
        }



        BlastEffect[0].SetActive(false);
        BlastEffect[1].SetActive(false);
        LostEffect[0].SetActive(false);
        LostEffect[1].SetActive(false);
        ArrowIdicators[0].SetActive(false);
        ArrowIdicators[1].SetActive(false);
    }


    private void OnDisable()
    {
        //if (wasActiveEarlier)
        //{
        //    TeenpattiGameUIHandler.instance.HowToplay.SetActive(true);
        //}

        //if (wasActiveEarlieri)
        //{
        //    TeenpattiGameUIHandler.instance.teenpattiInfoPanel.gameObject.SetActive(true);
        //}
    }

    // SideShowBlueCardIdle
    // SideShowWinnerRedCardIdle

    public void DisableSideShowEffect()
    {
        if (gameObject.activeSelf)
        {
            Effect1.Play("SideShowWinnerRedReturn");
            Effect2.Play("SideShowWinnerReturn");
            Blast.Play("SideshowBlastReverseEffect");
            Blast1.Play("ShowWinnerEffectReverse");
            Invoke(nameof(DisableThisObjectAfterSomeTime), 1f);
        }

    }
    public void DisableThisObjectAfterSomeTime()
    {
        ShowSideShowText[0].SetActive(false);
        ShowSideShowText[1].SetActive(false);
        this.gameObject.SetActive(false);
    }

    public void EnbleThisForEffect()
    {
        ShowSideShowText[0].SetActive(true);
        ShowSideShowText[1].SetActive(false);
        Invoke(nameof(EnableCardAnimationEffects), 0.8f);
    }

    public void EnableCardAnimationEffects()
    {
        this.gameObject.SetActive(true);
        ProfileA.sprite = GameController.Instance.avatharPicture[GameManager.localInstance.gameState.players[GameManager.localInstance.gameState.Senderval].ui];
        ProfileB.sprite = GameController.Instance.avatharPicture[GameManager.localInstance.gameState.players[GameManager.localInstance.gameState.Receiverval].ui];
        if (APIController.instance.userDetails.isBlockApiConnection)
        {
            playerAName.text = GameManager.localInstance.myPlayerID == GameManager.localInstance.gameState.players[GameManager.localInstance.gameState.Senderval].playerData.playerID ? "You" : GameManager.localInstance.gameState.players[GameManager.localInstance.gameState.Senderval].playerData.playerName;
            playerBName.text = GameManager.localInstance.myPlayerID == GameManager.localInstance.gameState.players[GameManager.localInstance.gameState.Receiverval].playerData.playerID ? "You" : GameManager.localInstance.gameState.players[GameManager.localInstance.gameState.Receiverval].playerData.playerName;
        }
        else
        {
            playerAName.text = GameManager.localInstance.myPlayerID == GameManager.localInstance.gameState.players[GameManager.localInstance.gameState.Senderval].playerData.playerID ? "You" : GameManager.localInstance.gameState.players[GameManager.localInstance.gameState.Senderval].playerData.playerName.Substring(0, 3) + "*****";
            playerBName.text = GameManager.localInstance.myPlayerID == GameManager.localInstance.gameState.players[GameManager.localInstance.gameState.Receiverval].playerData.playerID ? "You" : GameManager.localInstance.gameState.players[GameManager.localInstance.gameState.Receiverval].playerData.playerName.Substring(0, 3) + "*****";
        }
        ArrowIdicators[1].SetActive(GameManager.localInstance.myPlayerID == GameManager.localInstance.gameState.players[GameManager.localInstance.gameState.Senderval].playerData.playerID);
        ArrowIdicators[0].SetActive(GameManager.localInstance.myPlayerID == GameManager.localInstance.gameState.players[GameManager.localInstance.gameState.Receiverval].playerData.playerID);
        Effect1.Play("SideShowRedWinner");
        Effect2.Play("SideShowBlueWinner");
        Invoke(nameof(EnableCardSlowly), 2f);
    }

    public void EnableCardSlowly()
    {
        for (int i = 0; i < 3; i++)
        {
            ShowCardA(GameManager.localInstance.gameState.players[GameManager.localInstance.gameState.Senderval].playerData.currentCards[i], i);
        }

        for (int i = 0; i < 3; i++)
        {
            ShowCardB(GameManager.localInstance.gameState.players[GameManager.localInstance.gameState.Receiverval].playerData.currentCards[i], i);
        }
        PlayerACardStrengthImage.sprite = CardList[GameManager.localInstance.gameState.players[GameManager.localInstance.gameState.Senderval].cardStrength];
        PlayerBCardStrengthImage.sprite = CardList[GameManager.localInstance.gameState.players[GameManager.localInstance.gameState.Receiverval].cardStrength];

        Effect1.Play("SideShowRedCardFlip");
        Effect2.Play("SideShowBlastCardFlip");

        Invoke(nameof(BlastEffectSlow), 2);
    }

    public void BlastEffectSlow()
    {
        if (CardCombination.CompareCards(GameManager.localInstance.gameState.players[GameManager.localInstance.gameState.Senderval].GetCurrentCards(), GameManager.localInstance.gameState.players[GameManager.localInstance.gameState.Receiverval].GetCurrentCards(), "0", 0, 0, 0))
        {
            BlastEffect[0].SetActive(false);
            BlastEffect[1].SetActive(true);
            LostEffect[0].SetActive(false);
            LostEffect[1].SetActive(true);
        }
        else
        {
            BlastEffect[1].SetActive(false);
            BlastEffect[0].SetActive(true);
            LostEffect[0].SetActive(true);
            LostEffect[1].SetActive(false);
        }
    }




    public void ShowCardA(CardData card, int numCard)
    {
        Card cardToChange = cardComponentB[numCard];
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
        cardComponentB[numCard].Intialize(card.cardIndex);
    }

    public void ShowCardB(CardData card, int numCard)
    {
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
    }


    public void SetCardsDetails(Card cardToChange, string rankText, Sprite smallSuitSprite, Sprite bigSpriteImage, Sprite coutSprite, Color cardColor, Color fontColor)
    {

        cardToChange.cardRankTxt.ConvertAll(x => x.text = rankText);
        cardToChange.suitImageSmall.ConvertAll(x => x.sprite = smallSuitSprite);
        cardToChange.cardRankTxt.ConvertAll(x => x.color = fontColor);
        cardToChange.suitImageSmall.ConvertAll(x => x.color = cardColor);
        cardToChange.suitImageBig.ConvertAll(x => x.color = cardColor);
    }





    public void EnbleThisForEffectOtherPlayers()
    {
        ShowSideShowText[0].SetActive(true);
        ShowSideShowText[1].SetActive(false);
        Invoke(nameof(EnableCardAnimationEffectsOtherPlayers), 0.8f);
    }

    public void EnableCardAnimationEffectsOtherPlayers()
    {
        this.gameObject.SetActive(true);
        ProfileA.sprite = GameController.Instance.avatharPicture[GameManager.localInstance.gameState.players[GameManager.localInstance.gameState.Senderval].ui];
        ProfileB.sprite = GameController.Instance.avatharPicture[GameManager.localInstance.gameState.players[GameManager.localInstance.gameState.Receiverval].ui];
        if (APIController.instance.userDetails.isBlockApiConnection)
        {
            playerAName.text = GameManager.localInstance.myPlayerID == GameManager.localInstance.gameState.players[GameManager.localInstance.gameState.Senderval].playerData.playerID ? "You" : GameManager.localInstance.gameState.players[GameManager.localInstance.gameState.Senderval].playerData.playerName;
            playerBName.text = GameManager.localInstance.myPlayerID == GameManager.localInstance.gameState.players[GameManager.localInstance.gameState.Receiverval].playerData.playerID ? "You" : GameManager.localInstance.gameState.players[GameManager.localInstance.gameState.Receiverval].playerData.playerName;
        }
        else
        {
            playerAName.text = GameManager.localInstance.myPlayerID == GameManager.localInstance.gameState.players[GameManager.localInstance.gameState.Senderval].playerData.playerID ? "You" : GameManager.localInstance.gameState.players[GameManager.localInstance.gameState.Senderval].playerData.playerName.Substring(0, 3) + "*****";
            playerBName.text = GameManager.localInstance.myPlayerID == GameManager.localInstance.gameState.players[GameManager.localInstance.gameState.Receiverval].playerData.playerID ? "You" : GameManager.localInstance.gameState.players[GameManager.localInstance.gameState.Receiverval].playerData.playerName.Substring(0, 3) + "*****";
        }
        ArrowIdicators[1].SetActive(false);
        ArrowIdicators[0].SetActive(false);
        Effect1.Play("SideShowRedWinner");
        Effect2.Play("SideShowBlueWinner");
        Invoke(nameof(EnableCardSlowlyOtherPlayers), 2f);
    }

    public void EnableCardSlowlyOtherPlayers()
    {
        Effect1.Play("SideShowWinnerRedCardIdle");
        Effect2.Play("SideShowBlueCardIdle");
        Invoke(nameof(BlastEffectSlow), 2);
    }



    PlayerState val;
    PlayerState val1;


    //////////////////////////////////////////////////////////////////////////////////////////////
    public void EnbleThisForShowEffect()
    {

        ShowSideShowText[0].SetActive(false);
        ShowSideShowText[1].SetActive(true);
        val = GameManager.localInstance.GetPlayerState(GameManager.localInstance.gameState.SenderShowval);
        val1 = GameManager.localInstance.GetPlayerState(GameManager.localInstance.gameState.ReceiverShowval);
        Invoke(nameof(EnableCardAnimationShowEffects), 0.8f);
    }

    public void EnableCardAnimationShowEffects()
    {
        this.gameObject.SetActive(true);
        ProfileA.sprite = GameController.Instance.avatharPicture[val.ui];
        ProfileB.sprite = GameController.Instance.avatharPicture[val1.ui];
        if (APIController.instance.userDetails.isBlockApiConnection)
        {
            playerAName.text = GameManager.localInstance.myPlayerID == val.playerData.playerID ? "You" : val.playerData.playerName;
            playerBName.text = GameManager.localInstance.myPlayerID == val1.playerData.playerID ? "You" : val1.playerData.playerName;
        }
        else
        {
            playerAName.text = GameManager.localInstance.myPlayerID == val.playerData.playerID ? "You" : val.playerData.playerName.Substring(0, 3) + "*****";
            playerBName.text = GameManager.localInstance.myPlayerID == val1.playerData.playerID ? "You" : val1.playerData.playerName.Substring(0, 3) + "*****";
        }
        ArrowIdicators[1].SetActive(GameManager.localInstance.myPlayerID == val.playerData.playerID);
        ArrowIdicators[0].SetActive(GameManager.localInstance.myPlayerID == val1.playerData.playerID);
        Effect1.Play("SideShowRedWinner");
        Effect2.Play("SideShowBlueWinner");
        Invoke(nameof(EnableCardSlowlyShow), 2f);
    }

    public void EnableCardSlowlyShow()
    {
        for (int i = 0; i < 3; i++)
        {
            ShowCardA(val.playerData.currentCards[i], i);
        }

        for (int i = 0; i < 3; i++)
        {
            ShowCardB(val1.playerData.currentCards[i], i);
        }
        PlayerACardStrengthImage.sprite = CardList[val.cardStrength];
        PlayerBCardStrengthImage.sprite = CardList[val1.cardStrength];

        Effect1.Play("SideShowRedCardFlip");
        Effect2.Play("SideShowBlastCardFlip");

        Invoke(nameof(BlastEffectSlowShow), 2);
    }

    public void BlastEffectSlowShow()
    {
        if (CardCombination.CompareCards(val.GetCurrentCards(), val1.GetCurrentCards(), "0", 0, 0, 0))
        {
            BlastEffect[0].SetActive(false);
            BlastEffect[1].SetActive(true);
            LostEffect[0].SetActive(false);
            LostEffect[1].SetActive(true);
        }
        else
        {
            BlastEffect[1].SetActive(false);
            BlastEffect[0].SetActive(true);
            LostEffect[0].SetActive(true);
            LostEffect[1].SetActive(false);
        }
    }



    ///////////////////////////////////////////////////////////////


    public void EnbleThisForEffectOtherPlayersShow()
    {
        ShowSideShowText[0].SetActive(false);
        ShowSideShowText[1].SetActive(true);
        val = GameManager.localInstance.GetPlayerState(GameManager.localInstance.gameState.SenderShowval);
        val1 = GameManager.localInstance.GetPlayerState(GameManager.localInstance.gameState.ReceiverShowval);
        Invoke(nameof(EnableCardAnimationEffectsOtherPlayersShow), 0.8f);
    }

    public void EnableCardAnimationEffectsOtherPlayersShow()
    {
        this.gameObject.SetActive(true);
        ProfileA.sprite = GameController.Instance.avatharPicture[val.ui];
        ProfileB.sprite = GameController.Instance.avatharPicture[val1.ui];
        playerAName.text = GameManager.localInstance.myPlayerID == val.playerData.playerID ? "You" : val.playerData.playerName.Substring(0, 3) + "*****";
        playerBName.text = GameManager.localInstance.myPlayerID == val1.playerData.playerID ? "You" : val1.playerData.playerName.Substring(0, 3) + "*****";
        ArrowIdicators[1].SetActive(false);
        ArrowIdicators[0].SetActive(false);
        Effect1.Play("SideShowRedWinner");
        Effect2.Play("SideShowBlueWinner");
        Invoke(nameof(EnableCardSlowlyOtherPlayersShow), 2f);
    }

    public void EnableCardSlowlyOtherPlayersShow()
    {
        Effect1.Play("SideShowWinnerRedCardIdle");
        Effect2.Play("SideShowBlueCardIdle");
        Invoke(nameof(BlastEffectSlow), 2);
    }

}
