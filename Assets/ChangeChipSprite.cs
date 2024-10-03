using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeChipSprite : MonoBehaviour
{
    public static ChangeChipSprite Instance;

    private void Awake()
    {
        Instance = this;
    }


    public Sprite[] Coins;
    public Image CoinPics;

   public void SetPicRound(int Rounds)
    {
        CoinPics.sprite = Coins[Rounds];
    }
}
