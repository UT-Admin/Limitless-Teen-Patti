using System;

namespace TP
{
    [Serializable]
    public class CardData
    {
        public CardSuit suitCard;
        public int rankCard;
        public bool isClose;
        public bool isCutJoker;
        public string playerid;

        #region Poker
        public int cardIndex;
        #endregion

        public CardData()
        {
            suitCard = CardSuit.None;
            rankCard = 0;
            isClose = true;
            cardIndex = 0;
            isCutJoker = false;
        }

        public CardData(int suit, int rank, bool close)
        {
            suitCard = (CardSuit)suit;
            rankCard = rank;
            isClose = close;
            cardIndex = 0;
            isCutJoker = false;
        }
        public CardData(CardSuit suit, int rank, bool close)
        {
            suitCard = suit;
            rankCard = rank;
            isClose = close;
            cardIndex = 0;
            isCutJoker = false;
        }

        public CardData(int suit, int rank, bool close,int index = 0)
        {
            suitCard = (CardSuit)suit;
            rankCard = rank;
            isClose = close;
            cardIndex = index;
            isCutJoker = false;
        }


        public CardData(int suit, int rank, bool close, bool cutJoker = false)
        {
            suitCard = (CardSuit)suit;
            rankCard = rank;
            isClose = close;
            isCutJoker = cutJoker;
            cardIndex = 0;
        }

        public bool IsJokerCard()
        {
            return (isCutJoker ? true : (suitCard == CardSuit.Joker));
        }
    }

    [Serializable]
    public struct DeckInfo
    {
        public CardData[] DeckData;
    }


}