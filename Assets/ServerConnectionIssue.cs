using System.Collections;
using System.Collections.Generic;
using TP;
using UnityEngine;
using UnityEngine.UI;

public class ServerConnectionIssue : MonoBehaviour
{

    public Button Close;

    private void Awake()
    {

        Close.onClick.AddListener(() => { OnClickClose(); });
    }

 

    public void OnClickClose()
    {
#if !UNITY_SERVER
        APIController.instance.CheckInternetForButtonClick((Success) =>
        {

            if (Success)
            {
                UIController.Instance.ExitGame();
            }
            else
            {

            }


        });
#endif
    }



}
