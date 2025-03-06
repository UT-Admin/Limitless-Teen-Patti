using System.Collections;
using System.Collections.Generic;
using TP;
using UnityEngine;
using UnityEngine.UI;

public class JoinNewGame : MonoBehaviour
{
    public Button AcceptButton;
    public Button CancelButton;

    private void Awake()
    {
        AcceptButton.onClick.AddListener(() => { AcceptButtonClick(); });
        CancelButton.onClick.AddListener(() => { RejectButtonClick(); });
    }

    private void OnEnable()
    {
        UIController.Instance.Connecting.SetActive(false);
        AcceptButton.interactable = true;
    }

    public void AcceptButtonClick()
    {
#if !UNITY_SERVER
        APIController.instance.CheckInternetForButtonClick((Success) =>
        {

            AcceptButton.interactable = false;

            if (Success)
            {
                if (PlayerManager.localPlayer != null)
                {

                    GameController.Instance.StartGameOnButtonClick();
                    this.gameObject.SetActive(false);
                }
                else
                {
                    UIController.Instance.FindGameWEBGL();
                    GameController.Instance.SearchOnInternetCheck = true;
                    this.gameObject.SetActive(false);
                }
            }
            else
            {

                AcceptButton.interactable = true;
            }


        });
#endif

    }

    public void RejectButtonClick()
    {

#if !UNITY_SERVER
        APIController.instance.CheckInternetForButtonClick((Success) =>
        {

            if (Success)
            {
                ExitGame();

            }
            else
            {

            }


        });

#endif
    }

    public void ExitGame()
    {
        Invoke(nameof(DelayExit), 0.5f);
    }

    public void DelayExit()
    {
#if UNITY_WEBGL
        UIController.Instance.BackToMainMenuLoading.SetActive(true);
        //APIController.CloseWindow();
#endif
    }

}


