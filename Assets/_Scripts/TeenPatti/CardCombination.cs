using System;
using System.Linq;
using UnityEngine;

namespace TP
{
    public static class CardCombination
    {
        private static int player1CardIndex, player2CardIndex;
        public static CardData[] sortcardsstart(CardData[] cards)
        {

            Array.Sort(cards, delegate (CardData card1, CardData card2)
            {
                return card1.rankCard.CompareTo(card2.rankCard); // (user1.Age - user2.Age)
            });

            if (cards[0].rankCard == 2 && cards[1].rankCard == 3 && cards[2].rankCard == 14)
            {
                CardData tempcard;
                tempcard = cards[0];
                cards[0] = cards[2];
                cards[2] = cards[1];
                cards[1] = tempcard;
            }
            else if (cards[0].rankCard == 2 && cards[1].rankCard == 14 && cards[2].rankCard == 15)
            {
                CardData tempcard;
                tempcard = cards[0];
                cards[0] = cards[1];
                cards[1] = tempcard;
            }


            return cards;

        }

        public static CardsCombination GetCombinationFromCardCustom(CardData[] cardsPlayer, string gameMode, int a, int b, int c)
        {
            cardsPlayer = sortcardsstart(cardsPlayer);

            CardData[] tempCard = cardsPlayer.ToArray();
            CardsCombination currentCombination = CardsCombination.HighCard;

            if (gameMode == "Zandu" || gameMode == "Ak47")
            {
                bool[] iszanducard = new bool[3];
                if (gameMode == "Zandu")
                {
                    for (int i = 0; i < tempCard.Length; i++)
                    {
                        if (tempCard[i].rankCard == a || tempCard[i].rankCard == b || tempCard[i].rankCard == c)
                        {
                            //      iszanducard[i] = true;
                            UnityEngine.Debug.Log("Card cangedxxx");
                            tempCard[i].rankCard = 15;
                        }
                        else
                        {
                            //        iszanducard[i] = false;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < tempCard.Length; i++)
                    {
                        if (tempCard[i].rankCard == 14 || tempCard[i].rankCard == 13 || tempCard[i].rankCard == 4 || tempCard[i].rankCard == 7)
                        {
                            UnityEngine.Debug.Log("Card cangedxxx");
                            //   iszanducard[i] = true;
                            tempCard[i].rankCard = 15;
                        }
                        else
                        {
                            // iszanducard[i] = false;
                        }
                    }
                }

                tempCard = sortcardsstart(tempCard);

                for (int i = 0; i < tempCard.Length; i++)
                {
                    if (tempCard[i].rankCard == 15)
                    {
                        iszanducard[i] = true;
                        // tempCard[i].rankCard = 15;
                    }
                    else
                    {
                        iszanducard[i] = false;
                    }
                }


                if (!iszanducard[0] && !iszanducard[1] && !iszanducard[2])
                {
                    //no-joker normal
                    currentCombination = GetCombinationFromCard(tempCard);
                }
                else if (!iszanducard[0] && !iszanducard[1] && iszanducard[2])
                {
                    //third-only-zandu

                    //tempCard[2].rankCard = 15;
                    //pair-in-general
                    if (tempCard[0].rankCard > tempCard[1].rankCard)
                    {
                        tempCard[2].rankCard = tempCard[0].rankCard;
                    }
                    else
                    {
                        tempCard[2].rankCard = tempCard[1].rankCard;
                    }
                    currentCombination = CardsCombination.Pair;

                    //flush -possibilty
                    if (tempCard[0].suitCard == tempCard[1].suitCard)
                    {
                        tempCard[2].suitCard = tempCard[0].suitCard;
                        currentCombination = CardsCombination.Flush;
                    }

                    //in case of ace
                    if (tempCard[0].rankCard == 14 || tempCard[1].rankCard == 14)
                    {
                        if (tempCard[0].rankCard == 14)
                        {

                            //reversesequence (A - K)
                            if (tempCard[0].rankCard == tempCard[1].rankCard + 1)
                            {
                                tempCard[2].rankCard = tempCard[1].rankCard - 1;
                                currentCombination = CardsCombination.Sequence;
                            }


                            //sequence (A - 2)
                            tempCard[0].rankCard = 1;
                            if (tempCard[0].rankCard == tempCard[1].rankCard - 1)
                            {
                                tempCard[2].rankCard = tempCard[1].rankCard + 1;
                                currentCombination = CardsCombination.Sequence;
                            }
                            tempCard[0].rankCard = 14;

                            //reverse sequence with 2 difference (A - Q)       
                            if (tempCard[0].rankCard == tempCard[1].rankCard + 2)
                            {
                                tempCard[2].rankCard = tempCard[1].rankCard + 1;
                                currentCombination = CardsCombination.Sequence;
                            }


                            //sequence with 2 difference (A - 3)
                            tempCard[0].rankCard = 1;
                            if (tempCard[0].rankCard == tempCard[1].rankCard - 2)
                            {
                                tempCard[2].rankCard = tempCard[1].rankCard - 1;
                                currentCombination = CardsCombination.Sequence;
                            }
                            tempCard[0].rankCard = 14;


                            //pure reverse sequence ( A - K)
                            if (tempCard[0].rankCard == tempCard[1].rankCard + 1 && tempCard[0].suitCard == tempCard[1].suitCard)
                            {
                                tempCard[2].rankCard = tempCard[1].rankCard - 1;
                                tempCard[2].suitCard = tempCard[0].suitCard;
                                currentCombination = CardsCombination.StraightFlush;
                            }

                            //pure sequence(A - 2)
                            tempCard[0].rankCard = 1;
                            if (tempCard[0].rankCard == tempCard[1].rankCard - 1 && tempCard[0].suitCard == tempCard[1].suitCard)
                            {
                                tempCard[2].rankCard = tempCard[1].rankCard + 1;
                                tempCard[2].suitCard = tempCard[0].suitCard;
                                currentCombination = CardsCombination.StraightFlush;
                            }
                            tempCard[0].rankCard = 14;

                            //pure-sequence with 2 difference (A - Q)                
                            if (tempCard[0].rankCard == tempCard[1].rankCard + 2 && tempCard[0].suitCard == tempCard[1].suitCard)
                            {
                                tempCard[2].rankCard = tempCard[1].rankCard + 1;
                                tempCard[2].suitCard = tempCard[0].suitCard;
                                currentCombination = CardsCombination.StraightFlush;
                            }

                            //pure reverse sequence with 2 difference(A - 3)
                            tempCard[0].rankCard = 1;
                            if (tempCard[0].rankCard == tempCard[1].rankCard - 2 && tempCard[0].suitCard == tempCard[1].suitCard)
                            {
                                tempCard[2].rankCard = tempCard[1].rankCard - 1;
                                tempCard[2].suitCard = tempCard[0].suitCard;
                                currentCombination = CardsCombination.StraightFlush;
                            }
                            tempCard[0].rankCard = 14;
                        }
                        if (tempCard[1].rankCard == 14)
                        {
                            //reverse sequence (2 - A)
                            tempCard[1].rankCard = 1;
                            if (tempCard[0].rankCard == tempCard[1].rankCard + 1)
                            {
                                tempCard[2].rankCard = tempCard[0].rankCard + 1;
                                currentCombination = CardsCombination.Sequence;
                            }
                            tempCard[1].rankCard = 14;

                            //sequence (K - A)
                            if (tempCard[0].rankCard == tempCard[1].rankCard - 1)
                            {
                                tempCard[2].rankCard = tempCard[0].rankCard - 1;
                                currentCombination = CardsCombination.Sequence;
                            }

                            //reverse sequence with 2 difference (3 - A)
                            tempCard[1].rankCard = 1;
                            if (tempCard[0].rankCard == tempCard[1].rankCard + 2)
                            {
                                tempCard[2].rankCard = tempCard[0].rankCard - 1;
                                currentCombination = CardsCombination.Sequence;
                            }
                            tempCard[1].rankCard = 14;

                            //sequence with 2 difference (Q - A)
                            if (tempCard[0].rankCard == tempCard[1].rankCard - 2)
                            {
                                tempCard[2].rankCard = tempCard[0].rankCard + 1;
                                currentCombination = CardsCombination.Sequence;
                            }

                            //pure sequence (K - A)
                            if (tempCard[0].rankCard == tempCard[1].rankCard - 1 && tempCard[0].suitCard == tempCard[1].suitCard)
                            {
                                tempCard[2].rankCard = tempCard[0].rankCard - 1;
                                tempCard[2].suitCard = tempCard[0].suitCard;
                                currentCombination = CardsCombination.StraightFlush;
                            }

                            //reverse pure sequence (2 - A)
                            tempCard[1].rankCard = 1;
                            if (tempCard[0].rankCard == tempCard[1].rankCard + 1 && tempCard[0].suitCard == tempCard[1].suitCard)
                            {
                                tempCard[2].rankCard = tempCard[0].rankCard + 1;
                                tempCard[2].suitCard = tempCard[0].suitCard;
                                currentCombination = CardsCombination.StraightFlush;
                            }
                            tempCard[1].rankCard = 14;

                            //pure reverse sequence with 2 difference (Q - A)
                            if (tempCard[0].rankCard == tempCard[1].rankCard - 2 && tempCard[0].suitCard == tempCard[1].suitCard)
                            {
                                tempCard[2].rankCard = tempCard[0].rankCard + 1;
                                tempCard[2].suitCard = tempCard[0].suitCard;
                                currentCombination = CardsCombination.StraightFlush;
                            }

                            //pure-sequence with 2 difference (3 - A)     
                            tempCard[1].rankCard = 1;
                            if (tempCard[0].rankCard == tempCard[1].rankCard + 2 && tempCard[0].suitCard == tempCard[1].suitCard)
                            {
                                tempCard[2].rankCard = tempCard[0].rankCard - 1;
                                tempCard[2].suitCard = tempCard[0].suitCard;
                                currentCombination = CardsCombination.StraightFlush;
                            }
                            tempCard[1].rankCard = 14;


                        }

                    }
                    else
                    {
                        //sequence
                        if (tempCard[0].rankCard == tempCard[1].rankCard + 1)
                        {
                            tempCard[2].rankCard = tempCard[0].rankCard + 1;
                            currentCombination = CardsCombination.Sequence;
                        }

                        //reverse sequence
                        if (tempCard[0].rankCard == tempCard[1].rankCard - 1)
                        {
                            tempCard[2].rankCard = tempCard[1].rankCard + 1;
                            currentCombination = CardsCombination.Sequence;
                        }

                        //sequence difference of 2 sequence
                        if (tempCard[0].rankCard == tempCard[1].rankCard + 2)
                        {
                            tempCard[2].rankCard = tempCard[0].rankCard + 1;
                            currentCombination = CardsCombination.Sequence;
                        }

                        //reverse difference of 2 sequence
                        if (tempCard[0].rankCard == tempCard[1].rankCard - 2)
                        {
                            tempCard[2].rankCard = tempCard[0].rankCard - 1;
                            currentCombination = CardsCombination.Sequence;
                        }

                        //pure sequence
                        if (tempCard[0].rankCard == tempCard[1].rankCard - 1 && tempCard[0].suitCard == tempCard[1].suitCard)
                        {
                            tempCard[2].rankCard = tempCard[1].rankCard + 1;
                            tempCard[2].suitCard = tempCard[1].suitCard;
                            currentCombination = CardsCombination.StraightFlush;
                        }

                        //reverse pure-sequence
                        if (tempCard[1].rankCard == tempCard[0].rankCard + 1 && tempCard[0].suitCard == tempCard[1].suitCard)
                        {
                            tempCard[2].rankCard = tempCard[0].rankCard + 1;
                            tempCard[2].suitCard = tempCard[0].suitCard;
                            currentCombination = CardsCombination.StraightFlush;
                        }

                        //sequence difference of 2 sequence
                        if (tempCard[0].rankCard == tempCard[1].rankCard + 2 && tempCard[0].suitCard == tempCard[1].suitCard)
                        {
                            tempCard[2].rankCard = tempCard[0].rankCard + 1;
                            tempCard[2].suitCard = tempCard[0].suitCard;
                            currentCombination = CardsCombination.StraightFlush;
                        }

                        //reverse difference of 2 sequence
                        if (tempCard[0].rankCard == tempCard[1].rankCard - 2 && tempCard[0].suitCard == tempCard[1].suitCard)
                        {
                            tempCard[2].rankCard = tempCard[0].rankCard - 1;
                            tempCard[2].suitCard = tempCard[0].suitCard;
                            currentCombination = CardsCombination.StraightFlush;
                        }




                    }
                    if (tempCard[0].rankCard == tempCard[1].rankCard)
                    {
                        tempCard[2].rankCard = tempCard[0].rankCard;
                        currentCombination = CardsCombination.Trail;
                    }
                }
                else if (!iszanducard[0] && iszanducard[1] && iszanducard[2])
                {
                    //second-third-only zandu
                    tempCard[1].rankCard = tempCard[0].rankCard;
                    tempCard[2].rankCard = tempCard[0].rankCard;

                    currentCombination = CardsCombination.Trail;
                }
                else if (iszanducard[0] && iszanducard[1] && iszanducard[2])
                {
                    tempCard[0].rankCard = 14;
                    tempCard[1].rankCard = 14;
                    tempCard[2].rankCard = 14;


                    //all zandu

                    currentCombination = CardsCombination.Trail;
                }

            }

            return currentCombination;

        }


        public static CardsCombination GetCombinationFromCard(CardData[] cardsPlayer)
        {
            cardsPlayer = sortcardsstart(cardsPlayer);
            CardsCombination currentCombination = CardsCombination.HighCard;
            if (cardsPlayer[2].suitCard != CardSuit.Joker)
            {
                //pair cards
                if (cardsPlayer[0].rankCard == cardsPlayer[1].rankCard || cardsPlayer[1].rankCard == cardsPlayer[2].rankCard || cardsPlayer[0].rankCard == cardsPlayer[2].rankCard)
                {
                    currentCombination = CardsCombination.Pair;
                }

                //flush
                if (cardsPlayer[0].suitCard == cardsPlayer[1].suitCard && cardsPlayer[1].suitCard == cardsPlayer[2].suitCard && cardsPlayer[0].suitCard == cardsPlayer[2].suitCard)
                {
                    currentCombination = CardsCombination.Flush;
                }

                //if-ace
                if (cardsPlayer[0].rankCard == 14 || cardsPlayer[1].rankCard == 14 || cardsPlayer[2].rankCard == 14)
                {
                    if (cardsPlayer[0].rankCard == 14)
                    {


                        //reverse sequence (A - K - Q)
                        if (cardsPlayer[0].rankCard == cardsPlayer[1].rankCard + 1 && cardsPlayer[0].rankCard == cardsPlayer[2].rankCard + 2)
                        {
                            currentCombination = CardsCombination.Sequence;
                        }

                        //sequence(A - 2 - 3)
                        cardsPlayer[0].rankCard = 1;
                        if (cardsPlayer[0].rankCard == cardsPlayer[1].rankCard - 1 && cardsPlayer[0].rankCard == cardsPlayer[2].rankCard - 2)
                        {
                            currentCombination = CardsCombination.Sequence;
                        }
                        cardsPlayer[0].rankCard = 14;


                        //straightflush - puresequence (A - K - Q)
                        if (cardsPlayer[0].rankCard == cardsPlayer[1].rankCard + 1 && cardsPlayer[0].rankCard == cardsPlayer[2].rankCard + 2 &&
                           cardsPlayer[0].suitCard == cardsPlayer[1].suitCard && cardsPlayer[1].suitCard == cardsPlayer[2].suitCard && cardsPlayer[0].suitCard == cardsPlayer[2].suitCard)
                        {
                            currentCombination = CardsCombination.StraightFlush;
                        }

                        //straightflush - puresequence (A - 2 - 3)
                        cardsPlayer[0].rankCard = 1;
                        if (cardsPlayer[0].rankCard == cardsPlayer[1].rankCard - 1 && cardsPlayer[0].rankCard == cardsPlayer[2].rankCard - 2 &&
                            cardsPlayer[0].suitCard == cardsPlayer[1].suitCard && cardsPlayer[1].suitCard == cardsPlayer[2].suitCard && cardsPlayer[0].suitCard == cardsPlayer[2].suitCard)
                        {
                            currentCombination = CardsCombination.StraightFlush;
                        }
                        cardsPlayer[0].rankCard = 14;
                    }
                    if (cardsPlayer[2].rankCard == 14)
                    {


                        //sequencereverse (3-2-A)
                        cardsPlayer[2].rankCard = 1;
                        if (cardsPlayer[0].rankCard == cardsPlayer[1].rankCard + 1 && cardsPlayer[0].rankCard == cardsPlayer[2].rankCard + 2)
                        {
                            currentCombination = CardsCombination.Sequence;
                        }
                        cardsPlayer[2].rankCard = 14;

                        //sequence(Q-K-A)
                        if (cardsPlayer[0].rankCard == cardsPlayer[1].rankCard - 1 && cardsPlayer[0].rankCard == cardsPlayer[2].rankCard - 2)
                            currentCombination = CardsCombination.Sequence;

                        //straightflush - puresequence(3-2-A)
                        cardsPlayer[2].rankCard = 1;
                        if (cardsPlayer[0].rankCard == cardsPlayer[1].rankCard + 1 && cardsPlayer[0].rankCard == cardsPlayer[2].rankCard + 2 &&
                           cardsPlayer[0].suitCard == cardsPlayer[1].suitCard && cardsPlayer[1].suitCard == cardsPlayer[2].suitCard && cardsPlayer[0].suitCard == cardsPlayer[2].suitCard)
                        {
                            currentCombination = CardsCombination.StraightFlush;
                        }
                        cardsPlayer[2].rankCard = 14;

                        //straightflush - puresequence(Q-K-A)              
                        if (cardsPlayer[0].rankCard == cardsPlayer[1].rankCard - 1 && cardsPlayer[0].rankCard == cardsPlayer[2].rankCard - 2 &&
                            cardsPlayer[0].suitCard == cardsPlayer[1].suitCard && cardsPlayer[1].suitCard == cardsPlayer[2].suitCard && cardsPlayer[0].suitCard == cardsPlayer[2].suitCard)
                        {
                            currentCombination = CardsCombination.StraightFlush;
                        }

                    }
                }
                //if no ace
                else
                {

                    //sequence

                    if (cardsPlayer[0].rankCard == cardsPlayer[1].rankCard + 1 && cardsPlayer[0].rankCard == cardsPlayer[2].rankCard + 2)
                    {
                        currentCombination = CardsCombination.Sequence;
                    }

                    //sequencereverse
                    if (cardsPlayer[0].rankCard == cardsPlayer[1].rankCard - 1 && cardsPlayer[0].rankCard == cardsPlayer[2].rankCard - 2)
                    {
                        currentCombination = CardsCombination.Sequence;
                    }

                    //straightflush - puresequence

                    if (cardsPlayer[0].rankCard == cardsPlayer[1].rankCard - 1 && cardsPlayer[0].rankCard == cardsPlayer[2].rankCard - 2 &&
                        cardsPlayer[0].suitCard == cardsPlayer[1].suitCard && cardsPlayer[1].suitCard == cardsPlayer[2].suitCard && cardsPlayer[0].suitCard == cardsPlayer[2].suitCard)
                    {
                        currentCombination = CardsCombination.StraightFlush;
                    }


                    //straightflush - puresequence
                    if (cardsPlayer[0].rankCard == cardsPlayer[1].rankCard + 1 && cardsPlayer[0].rankCard == cardsPlayer[2].rankCard + 2 &&
                       cardsPlayer[0].suitCard == cardsPlayer[1].suitCard && cardsPlayer[1].suitCard == cardsPlayer[2].suitCard && cardsPlayer[0].suitCard == cardsPlayer[2].suitCard)
                    {
                        currentCombination = CardsCombination.StraightFlush;
                    }
                }


                //trail cards
                if (cardsPlayer[0].rankCard == cardsPlayer[1].rankCard && cardsPlayer[1].rankCard == cardsPlayer[2].rankCard && cardsPlayer[0].rankCard == cardsPlayer[2].rankCard)
                    currentCombination = CardsCombination.Trail;

            }
            else
            {
                //pair-in-general
                currentCombination = CardsCombination.Pair;

                //flush
                if (cardsPlayer[0].suitCard == cardsPlayer[1].suitCard)
                {
                    currentCombination = CardsCombination.Flush;
                }

                //in case of ace
                if (cardsPlayer[0].rankCard == 14 || cardsPlayer[1].rankCard == 14)
                {
                    if (cardsPlayer[0].rankCard == 14)
                    {

                        //reversesequence (A - K)
                        if (cardsPlayer[0].rankCard == cardsPlayer[1].rankCard + 1)
                        {
                            currentCombination = CardsCombination.Sequence;
                        }


                        //sequence (A - 2)
                        cardsPlayer[0].rankCard = 1;
                        if (cardsPlayer[0].rankCard == cardsPlayer[1].rankCard - 1)
                        {
                            currentCombination = CardsCombination.Sequence;
                        }
                        cardsPlayer[0].rankCard = 14;

                        //reverse sequence with 2 difference (A - Q)       
                        if (cardsPlayer[0].rankCard == cardsPlayer[1].rankCard + 2)
                        {
                            currentCombination = CardsCombination.Sequence;
                        }


                        //sequence with 2 difference (A - 3)
                        cardsPlayer[0].rankCard = 1;
                        if (cardsPlayer[0].rankCard == cardsPlayer[1].rankCard - 2)
                        {
                            currentCombination = CardsCombination.Sequence;
                        }
                        cardsPlayer[0].rankCard = 14;


                        //pure reverse sequence ( A - K)
                        if (cardsPlayer[0].rankCard == cardsPlayer[1].rankCard + 1 && cardsPlayer[0].suitCard == cardsPlayer[1].suitCard)
                        {
                            currentCombination = CardsCombination.StraightFlush;
                        }

                        //pure sequence(A - 2)
                        cardsPlayer[0].rankCard = 1;
                        if (cardsPlayer[0].rankCard == cardsPlayer[1].rankCard - 1 && cardsPlayer[0].suitCard == cardsPlayer[1].suitCard)
                        {
                            currentCombination = CardsCombination.StraightFlush;
                        }
                        cardsPlayer[0].rankCard = 14;

                        //pure-sequence with 2 difference (A - Q)                
                        if (cardsPlayer[0].rankCard == cardsPlayer[1].rankCard + 2 && cardsPlayer[0].suitCard == cardsPlayer[1].suitCard)
                        {
                            currentCombination = CardsCombination.StraightFlush;
                        }

                        //pure reverse sequence with 2 difference(A - 3)
                        cardsPlayer[0].rankCard = 1;
                        if (cardsPlayer[0].rankCard == cardsPlayer[1].rankCard - 2 && cardsPlayer[0].suitCard == cardsPlayer[1].suitCard)
                        {
                            currentCombination = CardsCombination.StraightFlush;
                        }
                        cardsPlayer[0].rankCard = 14;
                    }
                    if (cardsPlayer[1].rankCard == 14)
                    {
                        //reverse sequence (2 - A)
                        cardsPlayer[1].rankCard = 1;
                        if (cardsPlayer[0].rankCard == cardsPlayer[1].rankCard + 1)
                        {
                            currentCombination = CardsCombination.Sequence;
                        }
                        cardsPlayer[1].rankCard = 14;

                        //sequence (K - A)
                        if (cardsPlayer[0].rankCard == cardsPlayer[1].rankCard - 1)
                        {
                            currentCombination = CardsCombination.Sequence;
                        }

                        //reverse sequence with 2 difference (3 - A)
                        cardsPlayer[1].rankCard = 1;
                        if (cardsPlayer[0].rankCard == cardsPlayer[1].rankCard + 2)
                        {
                            currentCombination = CardsCombination.Sequence;
                        }
                        cardsPlayer[1].rankCard = 14;

                        //sequence with 2 difference (Q - A)
                        if (cardsPlayer[0].rankCard == cardsPlayer[1].rankCard - 2)
                        {
                            currentCombination = CardsCombination.Sequence;
                        }

                        //pure sequence (K - A)
                        if (cardsPlayer[0].rankCard == cardsPlayer[1].rankCard - 1 && cardsPlayer[0].suitCard == cardsPlayer[1].suitCard)
                        {
                            currentCombination = CardsCombination.StraightFlush;
                        }

                        //reverse pure sequence (2 - A)
                        cardsPlayer[1].rankCard = 1;
                        if (cardsPlayer[0].rankCard == cardsPlayer[1].rankCard + 1 && cardsPlayer[0].suitCard == cardsPlayer[1].suitCard)
                        {
                            currentCombination = CardsCombination.StraightFlush;
                        }
                        cardsPlayer[1].rankCard = 14;

                        //pure reverse sequence with 2 difference (Q - A)
                        if (cardsPlayer[0].rankCard == cardsPlayer[1].rankCard - 2 && cardsPlayer[0].suitCard == cardsPlayer[1].suitCard)
                        {
                            currentCombination = CardsCombination.StraightFlush;
                        }

                        //pure-sequence with 2 difference (3 - A)     
                        cardsPlayer[1].rankCard = 1;
                        if (cardsPlayer[0].rankCard == cardsPlayer[1].rankCard + 2 && cardsPlayer[0].suitCard == cardsPlayer[1].suitCard)
                        {
                            currentCombination = CardsCombination.StraightFlush;
                        }
                        cardsPlayer[1].rankCard = 14;


                    }

                }
                else
                {
                    //sequence
                    if (cardsPlayer[0].rankCard == cardsPlayer[1].rankCard + 1)
                    {
                        currentCombination = CardsCombination.Sequence;
                    }

                    //reverse sequence
                    if (cardsPlayer[0].rankCard == cardsPlayer[1].rankCard - 1)
                    {
                        currentCombination = CardsCombination.Sequence;
                    }

                    //sequence difference of 2 sequence
                    if (cardsPlayer[0].rankCard == cardsPlayer[1].rankCard + 2)
                    {
                        currentCombination = CardsCombination.Sequence;
                    }

                    //reverse difference of 2 sequence
                    if (cardsPlayer[0].rankCard == cardsPlayer[1].rankCard - 2)
                    {
                        currentCombination = CardsCombination.Sequence;
                    }

                    //pure sequence
                    if (cardsPlayer[0].rankCard == cardsPlayer[1].rankCard - 1 && cardsPlayer[0].suitCard == cardsPlayer[1].suitCard)
                    {
                        currentCombination = CardsCombination.StraightFlush;
                    }

                    //reverse pure-sequence
                    if (cardsPlayer[0].rankCard == cardsPlayer[1].rankCard + 1 && cardsPlayer[0].suitCard == cardsPlayer[1].suitCard)
                    {
                        currentCombination = CardsCombination.StraightFlush;
                    }

                    //sequence difference of 2 sequence
                    if (cardsPlayer[0].rankCard == cardsPlayer[1].rankCard + 2 && cardsPlayer[0].suitCard == cardsPlayer[1].suitCard)
                    {
                        currentCombination = CardsCombination.StraightFlush;
                    }

                    //reverse difference of 2 sequence
                    if (cardsPlayer[0].rankCard == cardsPlayer[1].rankCard - 2 && cardsPlayer[0].suitCard == cardsPlayer[1].suitCard)
                    {
                        currentCombination = CardsCombination.StraightFlush;
                    }


                }

                //trail
                if (cardsPlayer[0].rankCard == cardsPlayer[1].rankCard)
                {
                    currentCombination = CardsCombination.Trail;
                }
            }
            return currentCombination;
        }

        public static int GetHighCard(CardData[] cards, int defcard, int pos = 0)
        {
            int numCard = 0;
            Debug.Log("Get High Card  1 --- > " + numCard);

            if (defcard == 0)
            {
                for (int i = 0; i < cards.Length; i++)
                {
                    if (cards[i].rankCard > numCard)
                        numCard = cards[i].rankCard;

                }
                Debug.Log("Get High Card 2  --- > " + numCard);
                return numCard;
            }
            else
            {
                int count = 0;
                for (int i = 0; i < cards.Length; i++)
                {
                    if ((cards[i].rankCard > numCard) && (cards[i].rankCard < defcard))
                    {

                        numCard = cards[i].rankCard;
                        Debug.Log("Get High Card 3 --- > " + numCard);
                    }
                    else if(cards[i].rankCard == defcard)
                    {
                        if (count == pos)
                            numCard = cards[i].rankCard;
                        count += 1;

                        Debug.Log("Get High Card 4 --- > " + numCard);
                    }

                }

                Debug.Log("Get High Card 5 --- > " + numCard);
                return numCard;
            }
        }


        public static int GetHighCardposition(CardData[] cards)
        {
            int numCard = 0;
            for (int i = 0; i < cards.Length; i++)
            {
                if (cards[i].rankCard > numCard && cards[i].suitCard != CardSuit.Joker)
                    numCard = i;
            }
            return numCard;
        }

        public static PlayerState FindWinnerFromAllState(PlayerState[] players)
        {

            CardsCombination[] playerInGamCardsCombination = new CardsCombination[players.Length];
            int[] playerInGameHigh = new int[players.Length];

            for (int i = 0; i < players.Length; i++)
            {
                if (!players[i].hasPacked)
                {
                    playerInGamCardsCombination[i] = GetCombinationFromCard(players[i].GetCurrentCards());
                    playerInGameHigh[i] = GetHighCard(players[i].GetCurrentCards(), 0);
                }
            }

            int maxCombinaion = 0;
            int numWinPlayer = 0;
            for (int i = 0; i < players.Length; i++)
            {
                if (maxCombinaion < (int)playerInGamCardsCombination[i])
                {
                    maxCombinaion = (int)playerInGamCardsCombination[i];
                    numWinPlayer = i;
                }
                else if (maxCombinaion == (int)playerInGamCardsCombination[i])
                {
                    if (GetHighCard(players[numWinPlayer].GetCurrentCards(), 0) < playerInGameHigh[i])
                    {
                        maxCombinaion = (int)playerInGamCardsCombination[i];
                        numWinPlayer = i;
                    }
                }
            }
            return players[numWinPlayer];
        }

        public static PlayerState FindWinnerFromAllCustomState(PlayerState[] players, string gameMode, int a, int b, int c)
        {
            CardsCombination[] playerInGamCardsCombination = new CardsCombination[players.Length];
            int[] playerInGameHigh = new int[players.Length];

            for (int i = 0; i < players.Length; i++)
            {
                if (!players[i].hasPacked)
                {
                    playerInGamCardsCombination[i] = GetCombinationFromCardCustom(players[i].GetCurrentCards(), gameMode, a, b, c);
                    playerInGameHigh[i] = GetHighCard(players[i].GetCurrentCards(), 0);
                }
            }

            int maxCombinaion = 0;
            int numWinPlayer = 0;
            int minCombination = 6;

            if (gameMode == "Muflis")
            {
                for (int i = 0; i < players.Length; i++)
                {
                    if (minCombination > (int)playerInGamCardsCombination[i])
                    {
                        minCombination = (int)playerInGamCardsCombination[i];
                        numWinPlayer = i;
                    }
                    else if (minCombination == (int)playerInGamCardsCombination[i])
                    {
                        if (GetHighCard(players[numWinPlayer].GetCurrentCards(), 0) > playerInGameHigh[i])
                        {
                            minCombination = (int)playerInGamCardsCombination[i];
                            numWinPlayer = i;
                        }
                    }
                }
                return players[numWinPlayer];
            }

            else
            {
                for (int i = 0; i < players.Length; i++)
                {
                    if (maxCombinaion < (int)playerInGamCardsCombination[i])
                    {
                        maxCombinaion = (int)playerInGamCardsCombination[i];
                        numWinPlayer = i;
                    }
                    else if (maxCombinaion == (int)playerInGamCardsCombination[i])
                    {
                        if (GetHighCard(players[numWinPlayer].GetCurrentCards(), 0) < playerInGameHigh[i])
                        {
                            maxCombinaion = (int)playerInGamCardsCombination[i];
                            numWinPlayer = i;
                        }
                    }
                }
                return players[numWinPlayer];

            }

        }


        public static int GetPairCardHighest(CardData[] cardsPlayer1)
        {
            int Value;
            var repeatedValues = cardsPlayer1.GroupBy(x => x.rankCard).Where(g => g.Count() > 1).Select(g => g.Key);
            foreach (var value in repeatedValues)
            {
                Value = value;
                return Value;
            }

            return 0;
        }

        public static int GetPairCardHighestKicker(CardData[] cardsPlayer1)
        {
            int Value;
            var repeatedValues = cardsPlayer1.GroupBy(x => x.rankCard).Where(g => g.Count() == 1).Select(g => g.Key);

            foreach (var value in repeatedValues)
            {
                Value = value;
                return Value;
            }

            return 0;
        }
        public static int RankValues1;
        public static int GetPairCardHighestSUITP1(CardData[] cardsPlayer1)
        {
            Debug.Log(cardsPlayer1[0].suitCard + " ********" + cardsPlayer1[1].suitCard + " ************ " + cardsPlayer1[2].suitCard);
            var repeatedValues = cardsPlayer1.GroupBy(x => x.rankCard).Where(g => g.Count() == 1).Select(g => g.Key);

            foreach (var value in repeatedValues)
            {
                RankValues1 = value;
                Debug.Log("SUIT 1 " + RankValues1);
            }

            for (int i = 0; i < cardsPlayer1.Length; i++)
            {

                if (RankValues1 == cardsPlayer1[i].rankCard)
                {
                    Debug.Log("SUIT 1 " + cardsPlayer1[i].suitCard);
                    return (int)cardsPlayer1[i].suitCard;
                }


            }
            return (int)CardSuit.None;
        }

        public static int RankValues2;
        public static int GetPairCardHighestSUITP2(CardData[] cardsPlayer2)
        {
            Debug.Log(cardsPlayer2[0].suitCard + " ********" + cardsPlayer2[1].suitCard + " ************ " + cardsPlayer2[2].suitCard);
            var repeatedValues = cardsPlayer2.GroupBy(x => x.rankCard).Where(g => g.Count() == 1).Select(g => g.Key);

            foreach (var value in repeatedValues)
            {
                RankValues2 = value;
                Debug.Log("SUIT 2 " + RankValues2);
            }

            for (int i = 0; i < cardsPlayer2.Length; i++)
            {

                if (RankValues2 == cardsPlayer2[i].rankCard)
                {
                    Debug.Log("SUIT 2 " + cardsPlayer2[i].suitCard);
                    return (int)cardsPlayer2[i].suitCard;
                }

            }
            return (int)CardSuit.None;
        }


        public static bool CompareCards(CardData[] cardsPlayer1, CardData[] cardsPlayer2, string gameMode, int a, int b, int c)
        {

            CardsCombination player1;
            CardsCombination player2;            
            if (gameMode == "Zandu")
            {
                player1 = GetCombinationFromCardCustom(cardsPlayer1, "Zandu", a, b, c);
                player2 = GetCombinationFromCardCustom(cardsPlayer2, "Zandu", a, b, c);
            }
            else if (gameMode == "Ak47")
            {
                player1 = GetCombinationFromCardCustom(cardsPlayer1, "Ak47", a, b, c);
                player2 = GetCombinationFromCardCustom(cardsPlayer2, "Ak47", a, b, c);
            }
            else
            {
                Debug.Log("WINNER TEENPATTI CHECK HERE --- >");
                player1 = GetCombinationFromCard(cardsPlayer1);
                player2 = GetCombinationFromCard(cardsPlayer2);
            }
            if (gameMode == "Muflis")
            {
                if ((int)player1 > (int)player2)
                {
                    return false;
                }
                else if ((int)player1 == (int)player2)
                {
                    int highCard1 = GetHighCard(cardsPlayer1, 0);
                    int highCard2 = GetHighCard(cardsPlayer2, 0);
                    Debug.LogError(highCard1 + "----" + highCard2);
                    if (highCard1 > highCard2)
                    {
                        return false;
                    }
                    else if (highCard1 == highCard2)
                    {
                        int highCard1b = GetHighCard(cardsPlayer1, highCard1,1);
                        int highCard2b = GetHighCard(cardsPlayer2, highCard2,1);
                        if (highCard1b > highCard2b)
                        {
                            return false;
                        }
                        else if (highCard1b == highCard2b)
                        {
                            int highCard1c = GetHighCard(cardsPlayer1, highCard1b,2);
                            int highCard2c = GetHighCard(cardsPlayer2, highCard2b,2);
                            if (highCard1c > highCard2c)
                            {
                                return false;
                            }
                            else if (highCard1c == highCard2c)
                            {

                                UnityEngine.Debug.Log("Verify position " + highCard2c);
                                player1CardIndex = GetHighCardposition(cardsPlayer1);
                                player2CardIndex = GetHighCardposition(cardsPlayer2);

                                if ((int)cardsPlayer1[player1CardIndex].suitCard > (int)cardsPlayer2[player1CardIndex].suitCard)
                                {
                                    return false;
                                }
                                else
                                {
                                    return true;
                                }

                            }
                            else
                            {
                                return true;
                            }
                        }
                        else
                        {
                            return true;
                        }

                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
            else
            {

                if (player1 == CardsCombination.Pair && player2 == CardsCombination.Pair)
                {
                    Debug.Log("WINNER TEENPATTI CHECK  PAIR 1  >" + player1 + " ************** " + player2);
                    int HighestPair1 = GetPairCardHighest(cardsPlayer1);
                    int HighestPair2 = GetPairCardHighest(cardsPlayer2);

                    if (HighestPair1 > HighestPair2)
                    {
                        Debug.Log("WINNER TEENPATTI CHECK  PAIR 2 >" + HighestPair1 + " ************** " + HighestPair2);
                        return true;
                    }
                    else
                    {
                        if (HighestPair2 > HighestPair1)
                        {
                            Debug.Log("WINNER TEENPATTI CHECK  PAIR 3 >" + HighestPair2 + " ************** " + HighestPair1);
                            return false;
                        }
                        else
                        {
                            if (HighestPair1 == HighestPair2)
                            {
                                int HighestPair1Kicker = GetPairCardHighestKicker(cardsPlayer1);
                                int HighestPair2Kicker = GetPairCardHighestKicker(cardsPlayer2);

                                if (HighestPair1Kicker > HighestPair2Kicker)
                                {
                                    return true;
                                }
                                else
                                {
                                    if (HighestPair2Kicker > HighestPair1Kicker)
                                    {
                                        return false;
                                    }
                                    else
                                    {
                                        if (HighestPair2Kicker == HighestPair1Kicker)
                                        {
                                            int SuitPlayer1 = GetPairCardHighestSUITP1(cardsPlayer1);
                                            int SuitPlayer2 = GetPairCardHighestSUITP2(cardsPlayer2);
                                            Debug.Log("WINNER TEENPATTI CHECK  PAIR 4 >" + SuitPlayer1 + " ************** " + SuitPlayer2);
                                            if (SuitPlayer1 > SuitPlayer2)
                                            {
                                                return true;
                                            }
                                            else
                                            {
                                                if (SuitPlayer2 > SuitPlayer1)
                                                {
                                                    return false;
                                                }
                                                else
                                                {
                                                    return true;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                return true;
                            }
                        }
                    }
                }



                Debug.Log("WINNER TEENPATTI CHECK HERE ---  1 >" + player1 + " ************** " + player2);
                if ((int)player1 > (int)player2)
                {
                    Debug.Log("WINNER TEENPATTI CHECK HERE --- 2 >" + player1 + " ************** " + player2);
                    return true;
                }
                else if ((int)player1 == (int)player2)
                {
                    Debug.Log("WINNER TEENPATTI CHECK HERE --- 3 >" + player1 + " ************** " + player2);
                    int highCard1 = GetHighCard(cardsPlayer1, 0);
                    int highCard2 = GetHighCard(cardsPlayer2, 0);
                    if (highCard1 > highCard2)
                    {
                        Debug.Log("WINNER TEENPATTI CHECK HERE --- 4 >" + highCard1 + " ************** " + highCard2);
                        return true;
                    }
                    else if (highCard1 == highCard2)
                    {
                        Debug.Log("WINNER TEENPATTI CHECK HERE --- 5 >" + player1 + " ************** " + player2);
                        int highCard1b = GetHighCard(cardsPlayer1, highCard1,1);
                        int highCard2b = GetHighCard(cardsPlayer2, highCard2,1);
                        if (highCard1b > highCard2b)
                        {
                            Debug.Log("WINNER TEENPATTI CHECK HERE --- 6 >" + player1 + " ************** " + player2);
                            return true;
                        }
                        else if (highCard1b == highCard2b)
                        {
                            Debug.Log("WINNER TEENPATTI CHECK HERE --- 7 >" + player1 + " ************** " + player2);
                            int highCard1c = GetHighCard(cardsPlayer1, highCard1b,2);
                            int highCard2c = GetHighCard(cardsPlayer2, highCard2b,2);
                            if (highCard1c > highCard2c)
                            {
                                Debug.Log("WINNER TEENPATTI CHECK HERE --- 8 >" + player1 + " ************** " + player2);
                                return true;
                            }
                            else if (highCard1c == highCard2c)
                            {
                                Debug.Log("WINNER TEENPATTI CHECK HERE --- 9 >" + player1 + " ************** " + player2);
                                player1CardIndex = GetHighCardposition(cardsPlayer1);
                                player2CardIndex = GetHighCardposition(cardsPlayer2);

                                if ((int)cardsPlayer1[player1CardIndex].suitCard < (int)cardsPlayer2[player1CardIndex].suitCard)
                                {
                                    Debug.Log("WINNER TEENPATTI CHECK HERE --- 10 >" + player1 + " ************** " + player2);
                                    return false;
                                }
                                else
                                {
                                    Debug.Log("WINNER TEENPATTI CHECK HERE --- 11 >" + player1 + " ************** " + player2);
                                    return true;
                                }


                            }
                            else
                            {
                                Debug.Log("WINNER TEENPATTI CHECK HERE --- 12 >" + player1 + " ************** " + player2);
                                return false;
                            }
                        }
                        else
                        {
                            Debug.Log("WINNER TEENPATTI CHECK HERE --- 13 >" + player1 + " ************** " + player2);
                            return false;
                        }

                    }
                    else
                    {
                        Debug.Log("WINNER TEENPATTI CHECK HERE --- 14 >" + player1 + " ************** " + player2);
                        return false;
                    }
                }
                else
                {
                    Debug.Log("WINNER TEENPATTI CHECK HERE --- 15 >" + player1 + " ************** " + player2);
                    return false;
                }
            }
        }
    }

}