using Mirror;
using System.Collections;
using System.Collections.Generic;
using TP;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class RejoinPopUp : MonoBehaviour
{
    public GameObject seeButton;
    public GameObject Bottom;

    private void OnEnable()
    {
        UIController.Instance.Connecting.SetActive(false);
        AudioListener.volume = 0;
    }

    private void OnDisable()
    {
        AudioListener.volume = 1;
    }

     public void Rejoin()
    {
        if (APIController.instance.userDetails.isBlockApiConnection)
        {
            this.gameObject.SetActive(false);
            seeButton.SetActive(false);
            Bottom.SetActive(false);
            UIController.Instance.ByInPage.SetActive(true);
        }
        else
        {

            GameController.Instance.isInGame = false;
            NetworkClient.Shutdown();
            this.gameObject.SetActive(false);
            UIController.Instance.FindGameWEBGLRejoin();
        }
    }
}
