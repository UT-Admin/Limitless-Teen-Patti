using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TP;

public class InternetKickOut : MonoBehaviour
{
    public Button Ok;
    public Button Close;

    private void Awake()
    {
        Ok.onClick.AddListener(() => { OnClickOk(); });
        Close.onClick.AddListener(() => { OnClickClose(); });
    }

    public void OnClickOk()
    {
#if !UNITY_SERVER
        APIController.instance.CheckInternetForButtonClick((Success) =>
        {


            if (Success)
            {
                UIController.Instance.FindGameWEBGLRejoin();
                gameObject.SetActive(false);
            }
            else
            {

            }


        });
#endif
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
