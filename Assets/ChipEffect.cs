using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChipEffect : MonoBehaviour
{
    public GameObject[] EnabeCoins;

    public static ChipEffect Instance;

    private void Awake()
    {
        Instance = this;
    }




}
