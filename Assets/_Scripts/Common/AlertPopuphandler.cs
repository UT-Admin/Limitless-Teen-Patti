using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
namespace TP
{
	public class AlertPopuphandler : MonoBehaviour
	{
		public TextMeshProUGUI alertMessage, okBtnTxt, positiveBtnTxt, negativeBtnTxt, header;
		public GameObject singleBtnGO, dualBtnGO;
		Action onClickAction;
		Action onPostiveClickAction;

		public Button homeBtn;


	Color defaultColor = Color.white;



		public Image Box;
		public Button single;
		private void OnEnable()
		{

#if RealTPG

			if (UIController.Instance.mainmenuPage.gameObject.activeSelf || UIController.Instance.teenPattiGameUIPanel.gameObject.activeSelf)
			{
				Box.transform.localScale = new Vector3(1, 1f, 1f);
				single.transform.localScale = new Vector3(1f, 1f, 1f);
			}
			else
			{
				Box.transform.localScale = new Vector3(1.6f, 1.6f, 1.6f);
				single.transform.localScale = new Vector3(1.7f, 1.7f, 1.7f);
			}
#elif GOP
			defaultColor = alertMessage.color;
#endif

		}

        private void Start()
        {
			homeBtn.onClick.AddListener(() => { OnClickHomeButton(); });
        }

		public void ShowAlertPopup()
		{
            if (!this.gameObject.activeInHierarchy)
            {
                this.gameObject.SetActive(true);
            }
        }


        public void ShowMessage(string message, string header = "ALERT WINDOW")
		{
			

			singleBtnGO.SetActive(true);
			dualBtnGO.SetActive(false);
			alertMessage.text = message;
			gameObject.SetActive(true);
			okBtnTxt.text = "Ok";
			Invoke("CloseAlert", 5);


		}

		public void ShowErroMessage(string message, string header = "ALERT WINDOW")
		{
			singleBtnGO.SetActive(true);
			dualBtnGO.SetActive(false);
			alertMessage.text = message;
			gameObject.SetActive(true);
			okBtnTxt.text = "Ok";
			Invoke("CloseAlert", 3);

			alertMessage.color = Color.red;
		}

		public void ShowMessageWithAction(string message, Action onClickAction, string header = "ALERT WINDOW", string positiveBtnMsg = "Ok")
		{

			singleBtnGO.SetActive(true);
			dualBtnGO.SetActive(false);
			alertMessage.text = message;
			okBtnTxt.text = positiveBtnMsg;
			gameObject.SetActive(true);
			this.onPostiveClickAction = null;
			this.onClickAction = null;
			okBtnTxt.text = positiveBtnMsg;
			this.onClickAction = onClickAction;


		}

		public void ShowMessageWithDualAction(string message, Action onClickPositiveAction, Action onClickNegativeAction, string header = "ALERT WINDOW", string positiveBtnMsg = "Ok", string negativeBtnMsg = "Cancel")
		{


			singleBtnGO.SetActive(false);
			dualBtnGO.SetActive(true);
			alertMessage.text = message;
			positiveBtnTxt.text = positiveBtnMsg;
			negativeBtnTxt.text = negativeBtnMsg;
			gameObject.SetActive(true);
			this.onPostiveClickAction = null;
			this.onClickAction = null;
			this.onPostiveClickAction = onClickPositiveAction;
			this.onClickAction = onClickNegativeAction;

		}

		public void OnPositiveBtnClicked()
		{
			if (onPostiveClickAction != null)
			{
				onPostiveClickAction.Invoke();
			}

			OnNegativeBtnClicked();
		}
		public void OnNegativeBtnClicked()
		{
			gameObject.SetActive(false);
			alertMessage.text = "";
			positiveBtnTxt.text = "Ok";
			negativeBtnTxt.text = "Cancel";
			okBtnTxt.text = "Ok";
#if TPF

#else
	alertMessage.color = defaultColor;
#endif
		
			if (onPostiveClickAction != null)
			{
				onPostiveClickAction = null;
			}
			if (onClickAction != null)
			{
				onClickAction.Invoke();
				onClickAction = null;
			}
		}

		public void CloseAlert()
		{
			CancelInvoke("CloseAlert");
			gameObject.SetActive(false);
			alertMessage.text = "";
			positiveBtnTxt.text = "Ok";
			negativeBtnTxt.text = "Cancel";
			okBtnTxt.text = "Ok";

			
#if TPF

#else
	alertMessage.color = defaultColor;
#endif
			if (onClickAction != null)
			{
				onClickAction.Invoke();
				onClickAction = null;
			}
		}

		private void OnClickHomeButton()
		{
			if (this.gameObject.activeInHierarchy)
			{
				this.gameObject.SetActive(false);
			}
#if UNITY_WEBGL
			UIController.CloseWindow();
#endif
        }

        /*private void Update()
		{

			if (Input.GetKeyDown(KeyCode.Escape))
				gameObject.SetActive(false);

		}*/
    }
}











