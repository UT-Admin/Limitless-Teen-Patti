using System;
using TMPro;
using UnityEngine;

namespace TP
{

    public class AlertDialogPopupAction : MonoBehaviour
    {
        public TextMeshProUGUI alertMessageTxt, headerTxt, hintTxt, positiveTxt, negativeTxt;
        Action onPositiveClickAction, onNegativeClickAction;


        public void ShowMessageWithAction(string dialogMsg, Action onPositiveClickAction, Action onNegativeClickAction, string headerMsg = "ALERT WINDOW", string hintInfoMsg = "", string positiveBtnMsg = "Yes", string negativeBtnMsg = "Close")
        {
            alertMessageTxt.text = dialogMsg;
            headerTxt.text = headerMsg;
            hintTxt.text = hintInfoMsg;
            positiveTxt.text = positiveBtnMsg;
            negativeTxt.text = negativeBtnMsg;
            gameObject.SetActive(true);
            this.onPositiveClickAction = onPositiveClickAction;
            this.onNegativeClickAction = onNegativeClickAction;

        }

        public void OnPositiveBtnClicked()
        {
            if (onPositiveClickAction != null)
            {
                onPositiveClickAction.Invoke();
                onPositiveClickAction = null;
            }

            CloseAlert();
        }

        public void OnNegativeBtnClicked()
        {
            CloseAlert();
        }

        public void CloseAlert()
        {
            alertMessageTxt.text = "";
            headerTxt.text = "";
            hintTxt.text = "";
            positiveTxt.text = "";
            negativeTxt.text = "";

            if (onNegativeClickAction != null)
            {
                onNegativeClickAction.Invoke();
                onNegativeClickAction = null;
            }
            gameObject.SetActive(false);
        }
    }

}