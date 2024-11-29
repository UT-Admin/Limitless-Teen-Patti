using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TP;
using DG.Tweening;
public class TotalPotFiller : MonoBehaviour
{
    public List<GameObject> PlayerMe = new();
    public List<GameObject> Player1 = new();
    public List<GameObject> Player2 = new();
    public List<GameObject> Player4 = new();
    public List<GameObject> Player5 = new();
    public GameObject PlayerMePostionForCoin;
    public GameObject Player1PostionForCoin;
    public GameObject Player2PostionForCoin;
    public GameObject Player4PostionForCoin;
    public GameObject Player5PostionForCoin;
    public PlayerUI playerMEUI;
    public PlayerUI player1UI;
    public PlayerUI player2UI;
    public PlayerUI player4UI;
    public PlayerUI player5UI;
    public Transform PlayerHolder;
    public GameObject Parent1;
    public GameObject Parent2; 
    public GameObject Parent3;
    public GameObject Parent4;
    public GameObject Parent5;


    public void SetGameObjectToThisList(GameObject Val)
    {
        if (PlayerMe.Count == 1)
        {
            for (int i = 0; i < PlayerMe.Count; i++)
            {
                PlayerMe[i].gameObject.SetActive(false);
                PlayerMe[i].gameObject.transform.SetParent(PlayerMePostionForCoin.transform, true);
                PlayerMe[i].gameObject.transform.SetAsFirstSibling();
                PlayerMe[i].gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f,36f,0f);
               // playerMEUI.CoinAnimation.Add(PlayerMe[PlayerMe.Count - 1]);
                PlayerMe.RemoveAt(PlayerMe.Count - 1);
            }
        }
        PlayerMe.Add(Val);
        check(Val.GetComponent<Image>());

    }

    public void check(Image SpriteCoin)
    {
        double totalPot = GameManager.localInstance.gameState.totalPot;

        if (totalPot <= 10)
        {
            SpriteCoin.sprite = GamePlayUI.instance.Coins[0];
        }
        else if (totalPot <= 30)
        {
            SpriteCoin.sprite = GamePlayUI.instance.Coins[0];
        }
        else if (totalPot <= 50)
        {
            SpriteCoin.sprite = GamePlayUI.instance.Coins[1];
        }
        else if (totalPot <= 100)
        {
            SpriteCoin.sprite = GamePlayUI.instance.Coins[2];
        }
        else if (totalPot <= 150)
        {
            SpriteCoin.sprite = GamePlayUI.instance.Coins[3];
        }
        else if (totalPot <= 200)
        {
            SpriteCoin.sprite = GamePlayUI.instance.Coins[4];
        }
        else if (totalPot <= 250)
        {
            SpriteCoin.sprite = GamePlayUI.instance.Coins[5];
        }
        else if (totalPot <= 300)
        {
            SpriteCoin.sprite = GamePlayUI.instance.Coins[6];
        }
        else if (totalPot <= 360)
        {
            SpriteCoin.sprite = GamePlayUI.instance.Coins[7];
        }
        else if (totalPot <= 450)
        {
            SpriteCoin.sprite = GamePlayUI.instance.Coins[8];
        }
        else if (totalPot <= 500)
        {
            SpriteCoin.sprite = GamePlayUI.instance.Coins[7];
        }
        else if (totalPot <= 650)
        {
            SpriteCoin.sprite = GamePlayUI.instance.Coins[8];
        }
        else if (totalPot <= 700)
        {
            SpriteCoin.sprite = GamePlayUI.instance.Coins[6];
        }
        else if (totalPot <= 860)
        {
            SpriteCoin.sprite = GamePlayUI.instance.Coins[7];
        }
        else if (totalPot <= 1000)
        {
            SpriteCoin.sprite = GamePlayUI.instance.Coins[8];
        }

    }


    public void SetGameObjectToThisListplayer1(GameObject Val)
    {
        if (Player1.Count == 1)
        {
            for (int i = 0; i < Player1.Count; i++)
            {
                Player1[i].gameObject.SetActive(false);
                Player1[i].gameObject.transform.SetParent(Player1PostionForCoin.transform, true);
                Player1[i].gameObject.transform.SetAsFirstSibling();
                Player1[i].gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 36f, 0f);
               // player1UI.CoinAnimation.Add(Player1[Player1.Count - 1]);
                Player1.RemoveAt(Player1.Count - 1);
            }
        }
        Player1.Add(Val);
        check(Val.GetComponent<Image>());
    }

    public void SetGameObjectToThisListplayer2(GameObject Val)
    {
        if (Player2.Count == 1)
        {
            for (int i = 0; i < Player2.Count; i++)
            {
                Player2[i].gameObject.SetActive(false);
                Player2[i].gameObject.transform.SetParent(Player2PostionForCoin.transform, true);
                Player2[i].gameObject.transform.SetAsFirstSibling();
                Player2[i].gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 36f, 0f);
               // player2UI.CoinAnimation.Add(Player2[Player2.Count - 1]);
                Player2.RemoveAt(Player2.Count - 1);
            }
        }
        Player2.Add(Val);
        check(Val.GetComponent<Image>());

    }

    public void SetGameObjectToThisListplayer4(GameObject Val)
    {
        if (Player4.Count == 1)
        {
            for (int i = 0; i < Player4.Count; i++)
            {
                Player4[i].gameObject.SetActive(false);
                Player4[i].gameObject.transform.SetParent(Player4PostionForCoin.transform, true);
                Player4[i].gameObject.transform.SetAsFirstSibling();
                Player4[i].gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 36f, 0f);
                //player4UI.CoinAnimation.Add(Player4[Player4.Count - 1]);
                Player4.RemoveAt(Player4.Count - 1);
            }
        }
        Player4.Add(Val);
        check(Val.GetComponent<Image>());

    }

    public void SetGameObjectToThisListplayer5(GameObject Val)
    {
        if (Player5.Count == 1)
        {
            for (int i = 0; i < Player5.Count; i++)
            {
                Player5[i].gameObject.SetActive(false);
                Player5[i].gameObject.transform.SetParent(Player5PostionForCoin.transform, true);
                Player5[i].gameObject.transform.SetAsFirstSibling();
                Player5[i].gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 36f, 0f);
               // player5UI.CoinAnimation.Add(Player5[Player5.Count - 1]);
                Player5.RemoveAt(Player5.Count - 1);
            }
        }
        Player5.Add(Val);
        check(Val.GetComponent<Image>());

    }



    public void ResetBackAllData()
    {


        for (int i = PlayerMe.Count - 1; i >= 0; i--)
        {
            PlayerMe[i].gameObject?.SetActive(false);
            PlayerMe[i].gameObject?.transform.SetParent(PlayerMePostionForCoin.transform, true);
            PlayerMe[i].gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 36f, 0f);
           /* if (playerMEUI.CoinAnimation.Count < 6)
                playerMEUI.CoinAnimation?.Add(PlayerMe[i]);*/
            PlayerMe.RemoveAt(i);
        }

    }

    public void ResetBackAllData1()
    {

        for (int i = Player1.Count - 1; i >= 0; i--)
        {
            Player1[i].gameObject?.SetActive(false);
            Player1[i].gameObject?.transform.SetParent(Player1PostionForCoin.transform, true);
            Player1[i].gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 36f, 0f);
            /*if (player1UI.CoinAnimation.Count < 6)
                player1UI.CoinAnimation?.Add(Player1[i]);*/
            Player1.RemoveAt(i);
        }
    }

    public void ResetBackAllData2()
    {



        for (int i = Player2.Count - 1; i >= 0; i--)
        {
            Player2[i].gameObject?.SetActive(false);
            Player2[i].gameObject?.transform.SetParent(Player2PostionForCoin.transform, true);
            Player2[i].gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 36f, 0f);
           /* if (player2UI.CoinAnimation.Count < 6)
                player2UI.CoinAnimation?.Add(Player2[i]);*/
            Player2.RemoveAt(i);
        }


    }

    public void ResetBackAllData4()
    {

        for (int i = Player4.Count - 1; i >= 0; i--)
        {
            Player4[i].gameObject?.SetActive(false);
            Player4[i].gameObject?.transform.SetParent(Player4PostionForCoin.transform, true);
            Player4[i].gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 36f, 0f);
            /*if (player4UI.CoinAnimation.Count < 6)
                player4UI.CoinAnimation?.Add(Player4[i]);*/
            Player4.RemoveAt(i);
        }
    }

    public void ResetBackAllData5()
    {
        for (int i = Player5.Count - 1; i >= 0; i--)
        {
            Player5[i].gameObject?.SetActive(false);
            Player5[i].gameObject?.transform.SetParent(Player5PostionForCoin.transform, true);
            Player5[i].gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 36f, 0f);
            /*if (player5UI.CoinAnimation.Count < 6)
                player5UI.CoinAnimation?.Add(Player5[i]);*/
            Player5.RemoveAt(i);
        }
    }

}
