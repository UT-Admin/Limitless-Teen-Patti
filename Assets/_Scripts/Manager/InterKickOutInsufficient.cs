using System.Collections;
using System.Collections.Generic;
using TP;
using UnityEngine;
using UnityEngine.UI;

public class InterKickOutInsufficient : MonoBehaviour
{

    public Button Close;
    private void Awake()
    {
        Close.onClick.AddListener(() => { OnClickClose(); });
    }

    private void OnEnable()
    {
        AudioListener.volume = 0;
    }

    private void OnDisable()
    {
        AudioListener.volume = 1;
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
