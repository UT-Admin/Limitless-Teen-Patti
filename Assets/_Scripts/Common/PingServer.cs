
using TP;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PingServer : MonoBehaviour
{
	[Header("PING SERVER")]
	[SerializeField] private TMP_Text ping;
	[SerializeField] private Image wifiIcon;

	public bool isMirror;
	int pingvalue;
	void Start()
	{

	}

	void Update()
	{
		if (isMirror && MirrorManager.instance)
			pingvalue = MirrorManager.instance.GetPing();

		DisplayTempPing();
	}
	public void DisplayTempPing()
	{

			ping.text = "Ping:" + " " + pingvalue.ToString() + "" + "ms";
	
			if (pingvalue < 70 && NetworkClient.isConnected)
			{
				wifiIcon.color = new Color32(8, 255, 0, 255);
				wifiIcon.fillAmount = 1f;
			}
			else if (pingvalue > 70 && pingvalue < 100 && NetworkClient.isConnected )
			{
				wifiIcon.color = new Color32(8, 255, 0, 255);
				wifiIcon.fillAmount = 0.7f;
			}
			else if (pingvalue > 100 && pingvalue < 150 && NetworkClient.isConnected )
			{
				wifiIcon.color = new Color32(255, 145, 0, 255);
				wifiIcon.fillAmount = 0.45f;
			}
			else if (pingvalue > 150 && NetworkClient.isConnected )
			{
				wifiIcon.color = new Color32(255, 0, 18, 255);
				wifiIcon.fillAmount = 0.25f;
			}
			else if (!NetworkClient.isConnected )
			{
				wifiIcon.color = new Color32(0, 0, 0, 0);
				wifiIcon.fillAmount = 1f;
				ping.text = "Ping:" + " " + "---";
			}
		
	}
}
