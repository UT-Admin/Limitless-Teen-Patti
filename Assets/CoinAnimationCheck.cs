using System.Collections;
using System.Collections.Generic;
using TP;
using UnityEngine;
using UnityEngine.UI;

public class CoinAnimationCheck : MonoBehaviour
{
    //public Slider Val1;
    public Image SpriteCoin;

    public void SliderValueBasedOnAmount(float totalAmount)
    {
        Debug.Log("===============> " + totalAmount);

        if (GamePlayUI.instance == null)
        {
            Debug.LogError("GamePlayUI.instance is null");
            return;
        }

        if (GamePlayUI.instance.Coins == null || GamePlayUI.instance.Coins.Length == 0)
        {
            Debug.LogError("GamePlayUI.instance.Coins is not properly initialized");
            return;
        }

        if (SpriteCoin == null)
        {
            return;
        }

        if (totalAmount <= 10)
        {
            SpriteCoin.sprite = GamePlayUI.instance.Coins[0];
        }
        else if (totalAmount <= 30)
        {
            SpriteCoin.sprite = GamePlayUI.instance.Coins[0];
        }
        else if (totalAmount <= 50)
        {
            SpriteCoin.sprite = GamePlayUI.instance.Coins[1];
        }
        else if (totalAmount <= 100)
        {
            SpriteCoin.sprite = GamePlayUI.instance.Coins[2];
        }
        else if (totalAmount <= 150)
        {
            SpriteCoin.sprite = GamePlayUI.instance.Coins[3];
        }
        else if (totalAmount <= 200)
        {
            SpriteCoin.sprite = GamePlayUI.instance.Coins[4];
        }
        else if (totalAmount <= 250)
        {
            SpriteCoin.sprite = GamePlayUI.instance.Coins[5];
        }
        else if (totalAmount <= 300)
        {
            SpriteCoin.sprite = GamePlayUI.instance.Coins[6];
        }
        else if (totalAmount <= 360)
        {
            SpriteCoin.sprite = GamePlayUI.instance.Coins[7];
        }
    }

    public void SliderReset()
    {
        //Val1.value =  0;
    }
}
