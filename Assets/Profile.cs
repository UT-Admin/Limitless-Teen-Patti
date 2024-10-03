using TP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Profile : MonoBehaviour
{
    public GameObject playerBalance;
    private void OnEnable()
    {
        if(playerBalance != null)
           playerBalance.SetActive(true);
    }

    private void OnDisable()
    {
       // GamePlayUI.instance.strengthMeter.SetActive(false);
        if (playerBalance != null)
            playerBalance.SetActive(false);
    }
}
