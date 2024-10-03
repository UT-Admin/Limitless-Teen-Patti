
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using Mirror;
using DG.Tweening;

namespace TP
{
	public class TeenpattiGameUIHandler : UIHandler
	{
		public UIHandler teenpattiInfoPanel;
		public TextMeshProUGUI MyPlayerAmount;
		public TextMeshProUGUI CommissionAmount;
		public Image Commission;
		public Image Dummy;
		public Image DummyStart;
        public Image Dummy1;
        public Image DummyStart1;
        public static TeenpattiGameUIHandler instance;
		public GameObject HowToplay;
		public override void HideMe()
		{
			UIController.Instance.RemoveFromOpenPages(this);
			if (NetworkClient.isConnected)
				NetworkClient.Shutdown();
			this.gameObject.SetActive(false);
		}
		public override void OnBack()
        {

        }

		private void Awake()
		{
			instance = this;
		}

        private void OnEnable()
        {
            ShowInfoPanel();
        }

		public override void ShowMe()
		{
			UIController.Instance.AddToOpenPages(this);
			this.gameObject.SetActive(true);
		}

	
		IEnumerator updateRulesScreenAs(int bgRendVal, int sRulesVal, int headPicsVal, int tabBgVal, string modeVal)
		{

			yield return new WaitForSeconds(4f);
			teenpattiInfoPanel.HideMe();

		}

        public void RemoveListenr()
        {
            Button button = teenpattiInfoPanel.GetComponentInChildren<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => { teenpattiInfoPanel.HideMe(); OnClickInfoPrivatTable(); });
        }

        void  ShowInfoPanel()
		{

			StartCoroutine(updateRulesScreenAs(0, 14, 0, 0, "Regular"));
		}

		public void OnClickInfoPrivatTable()
		{
	        teenpattiInfoPanel.ShowMe();	
		}

		public void Check(string val)
        {
			CommissionAmount.text = val;
			Commission.GetComponent<RectTransform>().DOMove(Dummy.GetComponent<RectTransform>().position, 2f, false).OnStepComplete(() => { Commission.DOFade(0.5f, .05f); }).OnStart(()=> { Commission.gameObject.SetActive(true); Commission.DOFade(1f, .01f); }).OnComplete(()=> { Commission.gameObject.SetActive(false); Commission.DOFade(0f, .5f); }).From(DummyStart.transform.position);
		}

		public void EnableTotalPot()
		{
         GamePlayUI.instance.PotLimitReched.GetComponent<RectTransform>().DOMove(Dummy1.GetComponent<RectTransform>().position, 2f, false).OnStepComplete(() => { GamePlayUI.instance.PotLimitReched.GetComponent<Image>().DOFade(0.5f, .05f); }).OnStart(() => { GamePlayUI.instance.PotLimitReched.SetActive(true); GamePlayUI.instance.PotLimitReched.GetComponent<Image>().DOFade(1f, .01f); }).OnComplete(() => { GamePlayUI.instance.PotLimitReched.SetActive(false); GamePlayUI.instance.PotLimitReched.GetComponent<Image>().DOFade(0f, .5f); }).From(DummyStart1.transform.position);
        }

	}

}