using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System;
using TMPro;

namespace TP
{

    [Serializable]
    public struct SplitDesignSuit
    {
        public CardSuit cardSuit;
        public Sprite cardImage;
    }
    [Serializable]
    public struct SplitDesignRank
    {
        public CardRankDesignEnum cardRankDesign;
        public Sprite cardImage;
    }

    [CreateAssetMenu(fileName = "CardAssets", menuName = "CustomObjects/CardAssets", order = 0)]
    public class CardAssets : ScriptableObject
    {
        //scriptable object to get the sprite of respective card

        [SerializeField] public Sprite cardEmptyImage;
        [SerializeField] public Sprite cardBackImage;
        [SerializeField] public Sprite cardJokerImage;
        //public List<CardData> Cards;

        [SerializeField] public bool isSuitColoredDesign = false;
        [SerializeField] private List<SplitDesignSuit> splitDesignSuits;
        [SerializeField] private List<SplitDesignRank> splitDesignRanks;
        [SerializeField] public string charcTenAltValue = "10";

        [SerializeField] public TMP_FontAsset tmpFont;
        [SerializeField] public Font normalFont;

        [SerializeField] private Color blackColor = Color.black;
        [SerializeField] private Color redColor = Color.red;

        public Sprite GetCardSuitSplitDesign(CardSuit cardSuit)
        {
            return splitDesignSuits
                    .Where(x => x.cardSuit == cardSuit)
                    .FirstOrDefault()
                    .cardImage;
        }

        public Sprite GetCardRankSplitDesign(int cardRank)
        {
            return splitDesignRanks
                    .Where(x => x.cardRankDesign == (CardRankDesignEnum)cardRank)
                    .FirstOrDefault()
                    .cardImage;
        }

        public string GetCardRankCustomText(int cardRank)
        {
            if(Enum.IsDefined(typeof(CardRankDesignEnum), cardRank))
            {
                return ((CardRankDesignEnum)cardRank).ToString().Substring(0, 1);
            }
            else
            {
                if(cardRank == 10)
                {
#if TPF
                    return cardRank.ToString();

#else
                    return (!string.IsNullOrWhiteSpace(charcTenAltValue) ? charcTenAltValue : cardRank.ToString());

#endif
                    return (!string.IsNullOrWhiteSpace(charcTenAltValue) ? charcTenAltValue : cardRank.ToString());
                }
                else if (cardRank == 1 || cardRank == 14)
                {
                    return ("A");
                }
                else
                {
                    return cardRank.ToString();
                }
            }
            //return (Enum.IsDefined(typeof(CardRankDesignEnum), cardRank) ? ((CardRankDesignEnum)cardRank).ToString().Substring(0, 1) : (cardRank == 10 && !string.IsNullOrEmpty(charcTenAltValue) ? charcTenAltValue : cardRank.ToString()));//cardRank.ToString()
        }

        public string GetCardRankVal(int cardRank)
        {
            if (Enum.IsDefined(typeof(CardRankDesignEnum), cardRank))
            {
                return ((CardRankDesignEnum)cardRank).ToString().Substring(0, 1);
            }
            else
            {
                if (cardRank == 1 || cardRank == 14)
                {
                    return ("A");
                }
                else
                {
                    return cardRank.ToString();
                }
            }
            //return (Enum.IsDefined(typeof(CardRankDesignEnum), cardRank) ? ((CardRankDesignEnum)cardRank).ToString().Substring(0, 1) : cardRank.ToString());//cardRank.ToString()
        }

        public Color GetCardSuitColor(CardSuit cardSuit)
        {
            return isSuitColoredDesign ? Color.white : (cardSuit == CardSuit.Clubs || cardSuit == CardSuit.Spade ? blackColor : redColor);
        }

        public Color GetCardSuitFontColor(CardSuit cardSuit)
        {
            return (cardSuit == CardSuit.Clubs || cardSuit == CardSuit.Spade ? blackColor : redColor);
        }

        public TMP_FontAsset GetTMPFont()
        {
            return tmpFont;
        }

        public Font GetFont()
        {
            return normalFont;
        }
    }


}