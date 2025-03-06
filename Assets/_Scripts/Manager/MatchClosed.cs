using System.Collections;
using System.Collections.Generic;
using TMPro;
using TP;
using UnityEngine;
using UnityEngine.UI;

public class MatchClosed : MonoBehaviour
{
    public Button CancelButton;
    public TMP_Text message;


    private void OnEnable()
    {
        message.text = UIController.Instance.Message;
    }

    private void Awake()
    {
        CancelButton.onClick.AddListener(() => { RejectButtonClick(); });
    }

    public void RejectButtonClick()
    {

#if !UNITY_SERVER
        APIController.instance.CheckInternetForButtonClick((Success) =>
        {

            if (Success)
            {
                Exit();

            }
            else
            {

            }


        });

#endif
    }

    public void Exit()
    {
#if UNITY_WEBGL
        UIController.Instance.BackToMainMenuLoading.SetActive(true);
#endif
    }
}
