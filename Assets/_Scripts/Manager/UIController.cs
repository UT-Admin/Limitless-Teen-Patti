using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using Mirror;
using System.Runtime.InteropServices;
using TMPro;
namespace TP
{
	public class UIController : SingletonMonoBehaviour<UIController>
	{

        [SerializeField] private List<UIHandler> openedPages;
        public TMP_Text CurrenyType;
        public TMP_Text Type;
        public TMP_Text AmountType;
        public GameObject Connecting, Loading, PlayAgain, Insufficient, InternetPopNew, InsufficientDemo, ConnectionIssue, InternetPopInSufficient, NewGamePopUp, ByInPage, BackToMainMenuLoading, matchClosed;
        public SettingsPanelHandler SettingsPanel;
        public ServerKickMessagePopUp serverKick;
        public UIHandler teenPattiGameUIPanel;
        public string Message;

        public bool IsNewUser = false;

        public void ResetNewUser()
		{
			IsNewUser = false;
		}

		private void Start()
		{
			
			Application.targetFrameRate = 300;

		

		}
#if UNITY_WEBGL
		[DllImport("__Internal")]
		public static extern void CloseWindow();


	
#endif

		public 	IEnumerator FindGame()
        {
			DebugHelper.Log("Find Game Called");
			yield return new WaitForSeconds(0f);

#if UNITY_WEBGL
			FindGameWEBGL();
#endif
		}

		public void ExitGame()
        {
#if UNITY_WEBGL
			UIController.CloseWindow();
#endif
		}

        public void AddMoney()
        {
#if UNITY_WEBGL
            APIController.instance.CheckInternetForButtonClick((Success) =>
            {
                if (Success)
                {

                    APIController.instance.OnClickDepositBtn();

                }
                else
                {

                    UIController.Instance.InternetPopInSufficient.SetActive(true);

                }

            });
#endif

        }


        public void NetworkShutDown()
		{
		  NetworkClient.Shutdown();
		}


		private void OnSceneUnloaded(Scene current)
		{
			DebugHelper.Log("OnSceneUnloaded: " + current.name);
			Resources.UnloadUnusedAssets();
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape) && openedPages.Count > 0 && openedPages.Count != 0)
				openedPages[openedPages.Count - 1].OnBack();
		}

		/// <summary>
		/// Add the current page to the list of opened pages to handle back button
		/// </summary>
		/// <param name="handler"></param>
		public void AddToOpenPages(UIHandler handler)
		{
			openedPages.Add(handler);
		}

		/// <summary>
		/// Removes the current page from the list of opened pages to handle back button
		/// </summary>
		/// <param name="handler"></param>
		public void RemoveFromOpenPages(UIHandler handler)
		{
			if (openedPages.Contains(handler))
				openedPages.Remove(handler);
		}

		private void OnLoginError(PlayFabControllerError error)
		{
			HideLoadingScreen();
		
			DebugHelper.Log(error.errorMessage);
			if(error.errorMessage == "Cannot resolve destination host")
			{
				
			}
		}

		
	

		public void ShowLoadingScreen()
		{
		
		  
		}

		public void HideLoadingScreen()
		{
			
		  
		}
		public bool isdulex;

		public void FindGamePrivate(int gameModeIndex)
		{
			isdulex = false;
			GameController.Instance.CurrentGameType = GameType.PRIVATE;
			GameController.Instance.CurrentGameMode = (GameMode)gameModeIndex;


		}

		public void FindGameWEBGL()
        {
			DebugHelper.Log("Find wbegl game Called");
			//Loading.SetActive(true);
			Connecting.SetActive(true);
			if(TeenpattiGameUIHandler.instance.HowToplay.activeSelf)
            {
				TeenpattiGameUIHandler.instance.HowToplay.SetActive(false);
			}
			if (TeenpattiGameUIHandler.instance.teenpattiInfoPanel.gameObject.activeSelf)
			{
				TeenpattiGameUIHandler.instance.teenpattiInfoPanel.gameObject.SetActive(false);

			}

			JoinTeenpatti(1);


		}

		public bool IsRejoin;

		public void RejoinManual()
		{
            GameController.Instance.isREconnectonce = true;
        }


        public void FindGameWEBGLRejoin()
        {
			DebugHelper.Log("=================> REJOIN CALLED");
           // Loading.SetActive(true);
			Connecting.SetActive(true) ;
			StartGameAfterShutDown();

//#if !UNITY_SERVER
//            APIController.instance.CheckInternetForButtonClick((Success) =>
//			{
//				if (Success)
//				{
//					NetworkClient.Shutdown();
//					if (GameController.Instance.isREconnectonce)
//					{
//                        GameController.Instance.isREconnectonce = false;
//                        Invoke(nameof(StartGameAfterShutDown),5);
//					}
//                }
//                else
//				{
//                    Loading.SetActive(false);
//                    UIController.Instance.InternetPopNew.gameObject.SetActive(true);
//                }
//			});
//#endif
		}


		public void StrtGameAfterDeposit()
		{
			DebugHelper.Log("=================>  Startgame AfterDeposit");
            JoinTeenpatti(1);
        }

		public void StartGameAfterShutDown()
		{
            UIController.Instance.InternetPopNew.gameObject.SetActive(false);
            if (TeenpattiGameUIHandler.instance.HowToplay.activeSelf)
            {
                TeenpattiGameUIHandler.instance.HowToplay.SetActive(false);
            }
            if (TeenpattiGameUIHandler.instance.teenpattiInfoPanel.gameObject.activeSelf)
            {
                TeenpattiGameUIHandler.instance.teenpattiInfoPanel.gameObject.SetActive(false);
            }
            AudioListener.volume = 1;
            IsRejoin = true;
			GameController.Instance.CanRejoin = true;
            JoinTeenpatti(1);
        }



        public void FindGame(int gameModeIndex)
		{

			GameController.Instance.CurrentGameType = GameType.ONLINE;
			GameController.Instance.CurrentGameMode = (GameMode)gameModeIndex;
				isdulex = false;

		}
		
		public void JoinTeenpattiTournaments()
		{
			double minimumBalanceForPlayGame = 11;
			if (GameController.Instance.CheckAmountForPlay(minimumBalanceForPlayGame,true))
			{
				DebugHelper.Log("You need minimum " + minimumBalanceForPlayGame + " for play this game.");
				HideLoadingScreen();
			}
			else
			{

			
				StartCoroutine(WaitAndEnterGameMirror());
			}


		}

		public IEnumerator CheckPreviousGame()
		{
			yield return new WaitForSeconds(.1f);
			StartCoroutine(CheckAndConnectMirrorNetworkForceCheck());
		}

		public IEnumerator CheckAndConnectMirrorNetworkForceCheck()
		{
			ShowLoadingScreen();
			yield return StartCoroutine(InitializeGame(false));

			
		}
		IEnumerator InitializeGame(bool isforce = true)
		{
		
			if (!NetworkClient.isConnected)
			{
				LoggerUtils.LogError("Starting Client");
				NetworkManager.singleton.StartClient();
			}

			float count = 0;

			while (!NetworkClient.isConnected)
			{
				count += Time.deltaTime;

				yield return new WaitForEndOfFrame();

				if (count > 5)
				{
					
					yield break;
				}
			}


			LoggerUtils.LogError("Connected to server");
			if (isforce)
			{
				NetworkClient.Shutdown();
				yield return new WaitForSeconds(.5f);
				StartCoroutine(InitializeGame(false));
				yield break;
			}

			if (!PlayerManager.localPlayer)
				NetworkClient.AddPlayer();
			count = 0;
			while (!PlayerManager.localPlayer)
			{
				LoggerUtils.LogError("Connected to server 0");
				yield return new WaitForSeconds(1);
				count += 1;
				if (count > 5)
				{
					
					NetworkClient.Shutdown();
					yield break;
				}

			}
			LoggerUtils.LogError("Connected to server1");


			count = 0;
			bool isGoodInternet = true;
			DebugHelper.Log("Connected to server1.1 "+ MirrorManager.instance.GetPing());
			while (MirrorManager.instance.GetPing() <= 1)
			{
				yield return null;
				count += Time.deltaTime;

				if (count > 5)
				{
					isGoodInternet = false;
					DebugHelper.Log("Connected to server1.2 " + MirrorManager.instance.GetPing());
					break;
				}
			}
			if (isGoodInternet)
			{
				float ping = MirrorManager.instance.GetPing();
			}
			DebugHelper.Log("Connected to server1.3 " + MirrorManager.instance.GetPing());
			if (!isGoodInternet)
			{
				NetworkClient.Shutdown();
				
				yield break;
			}
			GameController.Instance.isForceJoin = true;
			PlayerManager.localPlayer.SearchPreviousGame();
		}


		

		public void JoinTeenpatti(int id = 0)
		{
            GameController.Instance.Dulex = 0;
            if (id == 0)
            {
                GameController.Instance.CurrentAmountType = CashType.SILVER;

            }
            else
            {
                GameController.Instance.CurrentAmountType = CashType.CASH;
            }

            double minimumBalanceForPlayGame = 11;
			if (GameController.Instance.CheckAmountForPlay(minimumBalanceForPlayGame))
			{
                if (APIController.instance.userDetails.isBlockApiConnection)
                {
                    if (!UIController.Instance.InsufficientDemo.activeSelf)
					{
						UIController.Instance.InsufficientDemo.SetActive(true);
                        UIController.Instance.Connecting.SetActive(false);
                        UIController.Instance.Loading.SetActive(false);
					}
                }
                else
                {

                    if (!UIController.Instance.Insufficient.activeSelf)
					{
						UIController.Instance.Insufficient.SetActive(true);
                        UIController.Instance.Connecting.SetActive(false);
                        UIController.Instance.Loading.SetActive(false);

					}

                }
              
			}
			else
			{
				StartCoroutine(WaitAndEnterGameMirror());
			}

		}

		public void ShowGameHUD()
		{
			teenPattiGameUIPanel.ShowMe();
			
		}
		public void ShowMainMenu()
		{
            //Loading.SetActive(false);
            UIController.Instance.Connecting.SetActive(false);

        }
        public void ShowRoomClosed()
		{

		}
		
		IEnumerator WaitAndEnterGameMirror()
		{
			DebugHelper.Log("CHECK STATUS =========> 1");
			ShowLoadingScreen();
            string data;
			GameController.Instance.totalEarnings = 0;
			GameController.Instance.totalTrophyEarned = 0;
			string lobbyName = GameController.Instance.CurrentGameMode.ToString() + "_" + GameController.Instance.CurrentGameTable.ToString() + "_" + GameController.Instance.CurrentGameModelTable.ToString() + "_" + GameController.Instance.CurrentAmountType.ToString() + "_" + APIController.instance.userDetails.isBlockApiConnection;
			if (GameController.Instance.CurrentGameType == GameType.PRIVATE)
			{
				lobbyName = "Private"+ GameController.Instance.privateRoomCode;
				GameController.Instance.privateRoomName = ("Private_" + GameController.Instance.CurrentGameMode.ToString() + "_" + GameController.Instance.CurrentGameTable.ToString() + "_" + GameController.Instance.CurrentGameModelTable.ToString() + "_" + GameController.Instance.privateRoomCode).ToUpper() + "_" + GameController.Instance.privateRoomControl;
			}
			else
			{

				lobbyName += "_" + "TPG";

			}
			DebugHelper.Log("CHECK STATUS =========> 3");
			GameController.Instance.isInGame = false;
            GameController.Instance.currentLobbyName = lobbyName + APIController.instance.userDetails.game_Id + APIController.instance.userDetails.isBlockApiConnection + APIController.instance.userDetails.bootAmount;
            GameController.Instance.StartGame((int)GameController.Instance.CurrentGameMode, GameController.Instance.currentLobbyName);
			yield return new WaitForSeconds(0);
		}
		
		public DeluxeTeenPattiTableModel GetDeluxeTeenPattiTableMode(double playerAmount)
		{
			DeluxeTeenPattiTableModel table = GameController.Instance.GameModeModels.DeluxeTeenPattiData[3];
			return table;
		}
	}


}
