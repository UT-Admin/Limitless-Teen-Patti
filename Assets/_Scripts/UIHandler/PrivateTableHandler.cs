using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;

namespace TP
{
	public class PrivateTableHandler : UIHandler
	{
		[SerializeField] private GameObject pokerPanel;
		[SerializeField] private GameObject rummyPanel;
		[SerializeField] private GameObject teenPattiPanel;
		[SerializeField] private GameObject holder1, holder2;
		[SerializeField] private GameObject invalid;
		[SerializeField] private TMP_Text boot, blind, chaal, pot, error_Txt;
		private int _gameMode = 0;
		public TMP_InputField roomCode;
		public override void HideMe()
		{
			PlayClickCloseButton();
			UIController.Instance.RemoveFromOpenPages(this);
			gameObject.SetActive(false);
			invalid.SetActive(false);
		}

		public override void OnBack()
		{
		   
	 
		   HideMe();
		  
		}

		public override void ShowMe()
		{
			PlayClickButton();
			UIController.Instance.AddToOpenPages(this);
			gameObject.SetActive(true);
			holder1.SetActive(true);
	#if GOP
			panel1.SetActive(false);
			panel2.SetActive(false);
	#else
				panel1.SetActive(false);
	#endif
			#if GOP
			#else
				panel2.SetActive(false);
			#endif
			OnClickTeenPatti();
			ResetPosition();
		}

		public void OnClickPoker()
		{
			PlayClickButton();
			_gameMode = 2;
			pokerPanel.SetActive(true);
			rummyPanel.SetActive(false);
			teenPattiPanel.SetActive(false);
		}
		public void OnClickRummy()
		{
			PlayClickButton();
			_gameMode = 3;
			pokerPanel.SetActive(false);
			rummyPanel.SetActive(true);
			teenPattiPanel.SetActive(false);
		}

		public void OnClickTeenPatti()
		{
			PlayClickButton();
			_gameMode = 1;
			pokerPanel.SetActive(false);
			rummyPanel.SetActive(false);
			teenPattiPanel.SetActive(true);
			SetInfoValuesTeenpatti();
		}

		public void SetInfoValuesTeenpatti()
		{
		  
		   
				//boot.text = CommonFunctions.Instance.GetAmountDecimalSeparator(GameManager.instance.currentTableModel.BootAmount);
				//blind.text = CommonFunctions.Instance.GetAmountDecimalSeparator(GameManager.instance.currentTableModel.BlindLimit);
				//chaal.text = CommonFunctions.Instance.GetAmountDecimalSeparator(GameManager.instance.currentTableModel.ChaalLimit);
				//pot.text = CommonFunctions.Instance.GetAmountDecimalSeparator(GameManager.instance.currentTableModel.PotLimit);
			

		}
		public void OnStartClick()
		{
		
			GameController.Instance.CurrentGameType = GameType.PRIVATE;
			GameController.Instance.privateRoomCode = _gameMode + ""+CommonFunctions.Instance.GenRandomAlphaNum(Random.Range(4, 5)).ToUpper();
			GameController.Instance.IsprivateRoomCreate = true;
			Debug.LogError(_gameMode);
			if (_gameMode == 1)
			{
				GameController.Instance.privateRoomCode = "";
				UIController.Instance.FindGamePrivate(1);
			}
			else if (_gameMode == 3)
			{
				//UIController.Instance.JoinRummy();
			}
			else
			{
				//UIController.Instance.JoinPoker();
			}
			HideMe();
		}
		public void OnJoinClick()
		{

			if (roomCode.text.Length >= 5)
			{
				GameController.Instance.privateRoomCode = roomCode.text.ToUpper();
				invalid.SetActive(false);

				#if GOP
				#else
					StartCoroutine(CloseAllPanels());
				#endif

				roomCode.text = "";
			}
			else
			{
				#if GOP
				#else
					invalid.SetActive(true);
				#endif
				if(string.IsNullOrWhiteSpace(roomCode.text))
					#if GOP
					ErrorMessageHandler.Instance.ShowErrorBanner("Room Code cannot be empty");
					#else
						error_Txt.text = "Room Code Cannot be empty.";
					#endif
				else
					#if GOP
					ErrorMessageHandler.Instance.ShowErrorBanner("Please enter full Room Code");
					#else
						error_Txt.text = "Invalid Room Code.";
					#endif
				StopCoroutine("HideNotification");
				StartCoroutine("HideNotification");
				GameController.Instance.CurrentGameType = GameType.ONLINE;
			}
		   // HideMe();
		}

		IEnumerator HideNotification()
		{
			yield return new WaitForSeconds(2);
			roomCode.text = "";
			invalid.SetActive(false);
		}
		public void Closebutton()
		{
			holder1.SetActive(false);
			holder2.SetActive(false);
			HideMe();
		}

		#region AudioClipPlay


		[ContextMenu("Test")]
		public void PlayClickButton()
        {
            if (MasterAudioController.instance.CheckSoundToggle())
                MasterAudioController.instance.PlayAudio(AudioEnum.BUTTONCLICK);
		}
		public void PlayClickCloseButton()
        {
            if (MasterAudioController.instance.CheckSoundToggle())
                MasterAudioController.instance.PlayAudio(AudioEnum.BUTTONCLICKCLOSE);
		}

		#endregion AudioClipPlay


		#region Animation

		[SerializeField] private Transform pokerPanel1;
		[SerializeField] private Transform RummyPanel1;
		[SerializeField] private Transform TeenpattiPanel1;
		[SerializeField] private GameObject poker, rummy, teenpatti;
		[SerializeField] private GameObject panel1, panel2;
		[SerializeField] private Toggle _pokerToggle, _rummyToggle, _teenpattiToggle;
		// Start is called before the first frame update


		public void OnClickPokerButton()
		{
			pokerPanel1.DOScale(new Vector3(1f, 1f, 1f), 0.5f);
			poker.GetComponent<Transform>().DOLocalMove(new Vector3(-656f, 222f, 0), 0.5f);
			teenpatti.GetComponent<Transform>().DOLocalMove(new Vector3(40f, 218f, 0), 0.5f);
			rummy.GetComponent<Transform>().DOLocalMove(new Vector3(742f, 229f, 0), 0.5f);
			_pokerToggle.isOn = true;
			_teenpattiToggle.isOn = false;
			_rummyToggle.isOn = false;
			OnClickPoker();
			StartCoroutine(Closepanels());


		}

		public void OnClickRummyButton()
		{
			RummyPanel1.DOScale(new Vector3(1f, 1f, 1f), 0.5f);
			rummy.GetComponent<Transform>().DOLocalMove(new Vector3(742f, 229f, 0), 0.5f);
			poker.GetComponent<Transform>().DOLocalMove(new Vector3(-656f, 222f, 0), 0.5f);
			teenpatti.GetComponent<Transform>().DOLocalMove(new Vector3(40f, 218f, 0), 0.5f);
			_rummyToggle.isOn = true;
			_pokerToggle.isOn = false;
			_teenpattiToggle.isOn = false;
			OnClickRummy();
			StartCoroutine(Closepanels());

		}
		public void OnClickTeenpattiButton()
		{
			TeenpattiPanel1.DOScale(new Vector3(1f, 1f, 1f), 0.5f);
			teenpatti.GetComponent<Transform>().DOLocalMove(new Vector3(40f, 218f, 0), 0.5f);
			rummy.GetComponent<Transform>().DOLocalMove(new Vector3(742f, 229f, 0), 0.5f);
			poker.GetComponent<Transform>().DOLocalMove(new Vector3(-656f, 222f, 0), 0.5f);
			_teenpattiToggle.isOn = true;
			_rummyToggle.isOn = false;
			_pokerToggle.isOn = false;
			OnClickTeenPatti();
			StartCoroutine(Closepanels());
	   
		}

		public void ResetPosition()
		{
			pokerPanel1.localScale = new Vector3(1, 0, 1);
			RummyPanel1.localScale = new Vector3(1, 0, 1);
			TeenpattiPanel1.localScale = new Vector3(1, 0, 1);
			teenpatti.GetComponent<Transform>().localPosition = new Vector3(40, 0, 0);
			rummy.GetComponent<Transform>().localPosition = new Vector3(742, 0, 0);
			poker.GetComponent<Transform>().localPosition = new Vector3(-656, 0, 0);
		}

		public IEnumerator Closepanels()
		{
			yield return new WaitForSeconds(0.5f);
			panel1.SetActive(true);
			#if GOP
			#else
				panel2.SetActive(false);
			#endif
		}


		public IEnumerator CloseAllPanels()
		{
			yield return new WaitForSeconds(1f);
			Closebutton();
		}

		public void oncloseButtonclick()
		{
			holder1.SetActive(false);
			panel2.SetActive(false);
			HideMe();
		}


		#endregion Animation
	}

}











