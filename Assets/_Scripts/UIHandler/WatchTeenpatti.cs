using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TP
{
	public class WatchTeenpatti : UIHandler
	{
		[SerializeField] private int JoiningIndex = 0;
		public List<Toggle> PlayToggles;
		
		[Header("GOP Variables")]
		public Image TitleImg;
		public List<GameModeData> ModeData;
		
		public void SetIndex(int index)
		{
			foreach	(Toggle toggle in PlayToggles)
			{
				if (toggle.isOn)
				{
					JoiningIndex = PlayToggles.IndexOf(toggle);
				}
			}
		}
		
		public void JoinTeenpatti()
		{
			UIController.Instance.JoinTeenpatti(JoiningIndex);
			HideMe();
		}
		
		public override void HideMe()
        {
            if (MasterAudioController.instance.CheckSoundToggle())
                MasterAudioController.instance.PlayAudio(AudioEnum.BUTTONCLICKCLOSE);
			UIController.Instance.RemoveFromOpenPages(this);
			if (PlayToggles.Count != 0)
				PlayToggles[0].isOn = true;
			gameObject.SetActive(false);
		}

		public override void OnBack()
		{
			HideMe();
		}

		public override void ShowMe()
		{
			UIController.Instance.AddToOpenPages(this);
			gameObject.SetActive(true);
		
			#if GOP || TPV
			SetTitle();
			#endif
		}
		
		private void SetTitle()
		{
			foreach	(GameModeData data in ModeData)
			{
				if (data.Mode == GameController.Instance.CurrentGameMode)
				{
					TitleImg.sprite = data.ModeSprite;
					break;
				}
			}
		}
	}
	
	[System.Serializable]
	public class GameModeData
	{
		public GameMode Mode;
		public Sprite ModeSprite;
	}
}