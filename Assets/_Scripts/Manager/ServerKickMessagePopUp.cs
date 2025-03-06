using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TP;
using UnityEngine;
using UnityEngine.UI;

public class ServerKickMessagePopUp : MonoBehaviour
{
    public RectTransform PopupBG;
    public TMP_Text MsgTxt;
    public int Code;
    public string Message = "";


    public void ShowPopup(string msg, int code)
    {
        MsgTxt.text = msg;
        Code = code;
        gameObject.SetActive(true);
        foreach (Image img in PopupBG.GetComponentsInChildren<Image>())
        {
            img.DOFade(1f, 0.2f).From(0);
        }
        MsgTxt.DOFade(1f, 0.2f).From(0);
        CancelInvoke(nameof(HidePopup));
        Invoke(nameof(HidePopup), 1.5f);
    }

    private void HidePopup()
    {

        foreach (Image img in PopupBG.GetComponentsInChildren<Image>())
        {
            img.DOFade(0, 0.2f).From(1);
        }
        MsgTxt.DOFade(0, 0.2f).From(1).OnComplete(() =>
        {
            gameObject.SetActive(false);
            if (Code == 401 || Code == 403 || Code == 412 || Code == 413 || Code == 501)
            {

                MsgTxt.text = Message;
                Invoke(nameof(RedirectToMainMenu), 0f);
            }
            else
            {
                DebugHelper.Log("TEST================>");
                MsgTxt.text = Message;
                Invoke(nameof(RedirectToGame), 0f);
            }
        });
    }



    public void RedirectToMainMenu()
    {
#if UNITY_WEBGL
        UIController.Instance.BackToMainMenuLoading.SetActive(true);
#endif
    }

    public void RedirectToGame()
    {
        GameController.Instance.CanRejoin = false;
        GameController.Instance.toCheckIfPlayerHasReJoined = false;
        GameController.Instance.isInGame = false;
        this.gameObject.SetActive(false);
        GameController.Instance.StartGameOnButtonClick();
    }
}
