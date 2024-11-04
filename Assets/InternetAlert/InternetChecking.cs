using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class InternetChecking : MonoBehaviour
{
    public GameObject connectionPanel;
    public static InternetChecking instance;

    private void Start()
    {
        /*APIController.instance.OnInternetStatusChange += GetNetworkStatus;*/

    }
    //IEnumerator Start()
    //{
    //    APIController.instance.OnInternetStatusChange += GetNetworkStatusActionCall;
    //    while (true)
    //    {
    //        yield return new WaitForSeconds(2f);

    //       APIController.instance.CheckInternet();


    //    }
    //}

   /* public void GetNetworkStatus(string data)
    {
        connectionPanel.SetActive(data == "true" ? false:true);
    }*/
}

