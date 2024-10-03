using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TP;
public class testCards : MonoBehaviour
{
    [System.Serializable]
    public struct TestProfile
    {
        public int id;
        public TMP_Dropdown[] suit;
        public TMP_Dropdown[] rank;
        public TextMeshProUGUI result;
        public TextMeshProUGUI cardCombination;
        public CardData[] Currentcards;
    }
    public List<TestProfile> userData;
    public GameObject[] JockerCards;
    public TMP_Dropdown gameModes;
    private void OnEnable()
    {
        ResetCards();
    }
    public void ResetCards()
    {
        for (int i = 0; i < userData.Count; i++)
        {
            userData[i].result.text = "";
            userData[i].cardCombination.text = "";
            gameModes.options.Clear();
            gameModes.options.Add(new TMP_Dropdown.OptionData("Normal"));
            gameModes.options.Add(new TMP_Dropdown.OptionData("AK47"));
            gameModes.options.Add(new TMP_Dropdown.OptionData("MUFLIS"));
            gameModes.options.Add(new TMP_Dropdown.OptionData("ZANDU"));
            gameModes.options.Add(new TMP_Dropdown.OptionData("HUKAM"));
            gameModes.value = 0;
            ModeChange();
            for (int j =0;j<3;j++)
            { 
            userData[i].suit[j].options.Clear();
            userData[i].rank[j].options.Clear();
            userData[i].rank[j].options.Add(new TMP_Dropdown.OptionData("NONE"));
            userData[i].rank[j].options.Add(new TMP_Dropdown.OptionData("A"));
            userData[i].rank[j].options.Add(new TMP_Dropdown.OptionData("2"));
            userData[i].rank[j].options.Add(new TMP_Dropdown.OptionData("3"));
            userData[i].rank[j].options.Add(new TMP_Dropdown.OptionData("4"));
            userData[i].rank[j].options.Add(new TMP_Dropdown.OptionData("5"));
            userData[i].rank[j].options.Add(new TMP_Dropdown.OptionData("6"));
            userData[i].rank[j].options.Add(new TMP_Dropdown.OptionData("7"));
            userData[i].rank[j].options.Add(new TMP_Dropdown.OptionData("8"));
            userData[i].rank[j].options.Add(new TMP_Dropdown.OptionData("9"));
            userData[i].rank[j].options.Add(new TMP_Dropdown.OptionData("10"));
            userData[i].rank[j].options.Add(new TMP_Dropdown.OptionData("J"));
            userData[i].rank[j].options.Add(new TMP_Dropdown.OptionData("Q"));
            userData[i].rank[j].options.Add(new TMP_Dropdown.OptionData("K"));
            userData[i].rank[j].options.Add(new TMP_Dropdown.OptionData("A"));
            userData[i].rank[j].options.Add(new TMP_Dropdown.OptionData("JOCKER"));
            userData[i].suit[j].options.Add(new TMP_Dropdown.OptionData(CardSuit.None.ToString()));
            userData[i].suit[j].options.Add(new TMP_Dropdown.OptionData(CardSuit.Clubs.ToString()));
            userData[i].suit[j].options.Add(new TMP_Dropdown.OptionData(CardSuit.Diamonds.ToString()));
            userData[i].suit[j].options.Add(new TMP_Dropdown.OptionData(CardSuit.Hearts.ToString()));
            userData[i].suit[j].options.Add(new TMP_Dropdown.OptionData(CardSuit.Spade.ToString()));
            userData[i].suit[j].options.Add(new TMP_Dropdown.OptionData(CardSuit.Joker.ToString()));
            userData[i].suit[j].value = 0;
            userData[i].rank[j].value = 0;
            }
        }
    }

    public void ModeChange()
    {
        if(gameModes.value <= 2)
        {
            for(int i=0;i<3;i++)
            {
                JockerCards[i].SetActive(false);
            }
        }
        else if(gameModes.value == 3)
        {
            for (int i = 0; i < 3; i++)
            {
                JockerCards[i].SetActive(true);
            }
        }
        else if (gameModes.value == 4)
        {
            for (int i = 0; i < 3; i++)
            {
                JockerCards[i].SetActive(false);
            }
            JockerCards[1].SetActive(true);
        }
    }
    public void CalculateData()
    {
        bool check = true;
        int id = 0;
        List<TestProfile> selectedUser = new List<TestProfile>();
        foreach(TestProfile data in userData)
        {
            check = true;
            if(id < 5)
            {
                data.result.text = "";
                data.cardCombination.text = "";
                for (int i = 0; i < 3; i++)
                {
                    if (data.rank[i].value == 0)
                    {
                        check = false;
                        break;
                    }
                    if (data.suit[i].value == 0)
                    {
                        check = false;
                        break;
                    }
                    data.Currentcards[i] = new CardData(data.suit[i].value, data.rank[i].value == 1 ? 14 : data.rank[i].value, false);
                }
                if (!check)
                {
                    id += 1;
                    continue;
                }
                selectedUser.Add(data);
            }
            else
            {
                data.result.text = "";
                data.cardCombination.text = "";
                for (int i = 0; i < 3; i++)
                {
                    data.Currentcards[i] = new CardData(data.suit[i].value, data.rank[i].value == 1 ? 14 : data.rank[i].value, false);
                }
            }
            id = id + 1;
        }

        GenerateRankwinnew(selectedUser);
    }

    public void GenerateRankwinnew(List<TestProfile> selectedUser)
    {
        List<int> rankDetails1 = new List<int>();
        rankDetails1.Clear();
        if(selectedUser.Count > 1)
        {
            for (int i = 0; i < selectedUser.Count - 1; i++)
            {
                for (int j = i + 1; j < selectedUser.Count; j++)
                {

                    if (gameModes.value == 3)
                    {
                        if (CardCombination.CompareCards(selectedUser[i].Currentcards, selectedUser[j].Currentcards, "Zandu", userData[5].Currentcards[0].rankCard, userData[5].Currentcards[1].rankCard, userData[5].Currentcards[2].rankCard))
                        {
                            //                        rankDetails1.Add(selectedUser[i].id);
                        }
                        else
                        {
                            //   rankDetails1.Add(selectedUser[j].id);

                            string temp = JsonUtility.ToJson(selectedUser[i]);
                            string temp1 = JsonUtility.ToJson(selectedUser[j]);
                            selectedUser[i] = JsonUtility.FromJson<TestProfile>(temp1);
                            selectedUser[j] = JsonUtility.FromJson<TestProfile>(temp);

                        }
                    }
                    else if (gameModes.value == 4)
                    {
                        if (CardCombination.CompareCards(selectedUser[i].Currentcards, selectedUser[j].Currentcards, "Zandu", userData[5].Currentcards[1].rankCard, userData[5].Currentcards[1].rankCard, userData[5].Currentcards[1].rankCard))
                        {
                            //                        rankDetails1.Add(selectedUser[i].id);
                        }
                        else
                        {
                            //                       rankDetails1.Add(selectedUser[j].id);
                            string temp = JsonUtility.ToJson(selectedUser[i]);
                            string temp1 = JsonUtility.ToJson(selectedUser[j]);
                            selectedUser[i] = JsonUtility.FromJson<TestProfile>(temp1);
                            selectedUser[j] = JsonUtility.FromJson<TestProfile>(temp);

                        }
                    }
                    else if (gameModes.value == 1)
                    {
                        if (CardCombination.CompareCards(selectedUser[i].Currentcards, selectedUser[j].Currentcards, "Ak47", 0, 0, 0))
                        {
                            //                        rankDetails1.Add(selectedUser[i].id);
                        }
                        else
                        {
                            //                        rankDetails1.Add(selectedUser[j].id);
                            string temp = JsonUtility.ToJson(selectedUser[i]);
                            string temp1 = JsonUtility.ToJson(selectedUser[j]);
                            selectedUser[i] = JsonUtility.FromJson<TestProfile>(temp1);
                            selectedUser[j] = JsonUtility.FromJson<TestProfile>(temp);

                        }
                    }
                    else if (gameModes.value == 2)
                    {
                        if (CardCombination.CompareCards(selectedUser[i].Currentcards, selectedUser[j].Currentcards, "Muflis", 0, 0, 0))
                        {
                            //                        rankDetails1.Add(selectedUser[i].id);
                        }
                        else
                        {
                            //                        rankDetails1.Add(selectedUser[j].id);
                            string temp = JsonUtility.ToJson(selectedUser[i]);
                            string temp1 = JsonUtility.ToJson(selectedUser[j]);
                            selectedUser[i] = JsonUtility.FromJson<TestProfile>(temp1);
                            selectedUser[j] = JsonUtility.FromJson<TestProfile>(temp);

                        }
                    }
                    else
                    {
                        if (CardCombination.CompareCards(selectedUser[i].Currentcards, selectedUser[j].Currentcards, "0", 0, 0, 0))
                        {
                            //                        rankDetails1.Add(selectedUser[i].id);
                            Debug.LogError(selectedUser[j].Currentcards[0].suitCard + " / " + selectedUser[j].Currentcards[0].rankCard + " ---- " + selectedUser[j].Currentcards[1].suitCard + " / " + selectedUser[j].Currentcards[1].rankCard + " ---- " + selectedUser[j].Currentcards[2].suitCard + " / " + selectedUser[j].Currentcards[2].rankCard + " validate card 1 "+ selectedUser[i].Currentcards[0].suitCard + " / "+ selectedUser[i].Currentcards[0].rankCard + " ---- " + selectedUser[i].Currentcards[1].suitCard + " / " + selectedUser[i].Currentcards[1].rankCard + " ---- " + selectedUser[i].Currentcards[2].suitCard + " / " + selectedUser[i].Currentcards[2].rankCard);
                        }
                        else
                        {
                            //                        rankDetails1.Add(selectedUser[j].id);
                            Debug.LogError(selectedUser[j].Currentcards[0].suitCard + " / " + selectedUser[j].Currentcards[0].rankCard + " ---- " + selectedUser[j].Currentcards[1].suitCard + " / " + selectedUser[j].Currentcards[1].rankCard + " ---- " + selectedUser[j].Currentcards[2].suitCard + " / " + selectedUser[j].Currentcards[2].rankCard + " validate card 2 " + selectedUser[i].Currentcards[0].suitCard + " / " + selectedUser[i].Currentcards[0].rankCard + " ---- " + selectedUser[i].Currentcards[1].suitCard + " / " + selectedUser[i].Currentcards[1].rankCard + " ---- " + selectedUser[i].Currentcards[2].suitCard + " / " + selectedUser[i].Currentcards[2].rankCard);
                            string temp = JsonUtility.ToJson(selectedUser[i]);
                            string temp1 = JsonUtility.ToJson(selectedUser[j]);
                            selectedUser[i] = JsonUtility.FromJson<TestProfile>(temp1);
                            selectedUser[j] = JsonUtility.FromJson<TestProfile>(temp);

                        }
                    }
                }
                rankDetails1.Add(selectedUser[i].id);
                Debug.LogError(" selected card " + selectedUser[i].Currentcards[0].suitCard + " / " + selectedUser[i].Currentcards[0].rankCard + " ---- " + selectedUser[i].Currentcards[1].suitCard + " / " + selectedUser[i].Currentcards[1].rankCard + " ---- " + selectedUser[i].Currentcards[2].suitCard + " / " + selectedUser[i].Currentcards[2].rankCard);

                if (i == selectedUser.Count - 2)
                {
                    rankDetails1.Add(selectedUser[i+1].id);
                }
            }
        }
        int count = 1;
        foreach (int id in rankDetails1)
        {
            TestProfile data = userData.Find(x=>x.id == id);
            data.result.text = "Rank - " + count;
            data.cardCombination.text = CardCombination.GetCombinationFromCard(data.Currentcards).ToString();
            count += 1;
        }
    }

}
