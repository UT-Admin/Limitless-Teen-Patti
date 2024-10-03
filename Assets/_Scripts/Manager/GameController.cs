using Mirror;
using Mirror.SimpleWeb;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace TP
{
    /// <summary>
    /// Controls the entire game Logic flow
    /// </summary>
   
    public class GameController : MonoBehaviour
    {
        #region VARIABLES
#pragma warning disable 0414
        bool isVibrateReady = true;
#pragma warning restore 0414

        //============================================================================================//
        [Header("=======BOOL=========")]
        public bool isLocalServer;
        public bool isServerBuild;
        public bool isForceJoin = false;
        public bool isForceJoinLobby = false;
        public bool isBotWin = true;
        public bool IsprivateRoomCreate;
        public bool isLoggedIn;
        public bool isInGame;
        public bool isCreateRoom;
        public bool isTutorial = false;
        public bool isSound = true;
        public bool isVibrate = true;
        public bool isGullakGoldOn = true;
        public bool isHandStrengthMeterOn = true;
        public bool isAllin;
        public bool CheckForceClose;
        public bool isAudioPlayStarted;
        public bool isConnectedtoInternet = true;
        public bool isAttemptingToReconnect;
        public bool SearchOnInternetCheck;
        public bool isREconnectonce;
        public bool CanRejoin = false;
        public bool OnStartButtonClick = false;
        public bool multipleDeviceLogged = false;


        //============================================================================================//
        [Header("=======DOUBLE=========")]
        public double tournamentEntry;
        public double totalEarnings;

        //============================================================================================//
        [Header("=======INT=========")]
        public int Dulex = 0;
        public int DaysCountSinceCreated;
        public int HandstrengthMeterFreeDays;
        public int profileControl = 1;
        public int totalTrophyEarned;
        public int privateRoomControl;
        public int poolPoint;
        public int CurrentGamePlayerCount;
        int count = 0;

        //============================================================================================//
        [Header("========GAMEOBJECT=========")]
        public GameObject[] gameList;
        public GameObject DemoText;
        public GameObject Info;
        public GameObject StartPopUp;

        //============================================================================================//
        [Header("=======ENUMS=========")]
        public GameType CurrentGameType;
        public GameMode CurrentGameMode;
        public CashType CurrentAmountType;
        [SerializeField] private GameTable _GameTable;


        //============================================================================================//
        [Header("=======SPRITE=======")]
        public Sprite[] avatharPicture;
        public Sprite[] fbAvatharPicture;


        //============================================================================================//
        [Header("=======STRING=========")]
        public string currentLobbyName;
        public string currentRoomName;
        public string privateRoomName;
        public string privateRoomCode;
        public string deeplinkURL;
        string DeviceId;
        string processArgs = string.Empty;

        //============================================================================================//
        [Header("=======ACTION=========")]
        public Action onPlayerDataChanged;
        public Action onPlayerLeveledUp;
        public Action onLocationGranted;
        private Action<string> onComplete;


        //============================================================================================//
        [Header("=========REFERANCED_CLASSES==========")]
        public PlayfabPlayerData CurrentPlayerData;
        public CardAssets currentPlayingCards;
        public PlayerUI profileData;
        public UIHandler updateAppPopup;
        public List<StoreChipValue> storeChipValues;
        public DateTime currentServerDate;
        public NetworkManager NetMirrorWork;
        [SerializeField] private GameTableModel _GameModeModel;
        [SerializeField] private GameModeModel _gameModeModels;
        [SerializeField] private TournamentTableModel _tournamentTableModel;

        public TextMeshProUGUI Val;
        public static GameController Instance;
       
        #endregion

        #region UNITY_FUNCTIONS
        /// <summary>
        ///  Runs on Awake of this Application
        /// </summary>
        public void Awake()
        {
            Debug.Log("<=========== BUILD VERSION ===========> 0.3");
            Instance = this;
#if UNITY_EDITOR
            LoggerUtils.ToogleLogOnDevice(true);
            LoggerUtils.SetLogProfile(LogProfile.UnityDebug);
#elif UNITY_ANDROID
            LoggerUtils.ToogleLogOnDevice(true);
            LoggerUtils.SetLogProfile(LogProfile.UnityDebug);
#endif
            Input.multiTouchEnabled = false;
            UnityThread.initUnityThread();
        }


        /// <summary>
        /// called when the script is enabled
        /// </summary>
        private void Start()
        {
            isAudioPlayStarted = false;
            APIController.instance.OnUserDetailsUpdate += SetDataActionCall;
            APIController.instance.OnUserBalanceUpdate += SetPlayerAmountOnUpdateActionCall;
            APIController.instance.OnCancelDepositPopup += OnCancelDepositAmountActionCall;
            APIController.instance.OnUserDeposit += OnDepositActionCall;
            APIController.instance.OnSwitchingTab += OnSwitchTabActionCall;
            APIController.instance.OnInternetStatusChange += GetNetworkStatusActionCall;
            AudioListener.volume = 0;
            PlayerPrefs.DeleteKey("SoundActive");
            MasterAudioController.instance.PlayAudio(AudioEnum.BG, true);
            UIController.Instance.Loading.SetActive(true);

        }

        #endregion

        #region SUBSCRIBED_FUNCTIONS

        /// <summary>
        ///  This Function will be called when an userdetails gets updated and has its Subcription in start
        /// </summary>
        public void SetDataActionCall()
        {
            Debug.Log("API Controller ======> " + APIController.instance.userDetails.serverInfo.server_host + " <==================> " + ushort.Parse(APIController.instance.userDetails.serverInfo.server_port.ToString()));
#if !UNITY_SERVER
           /* NetMirrorWork.networkAddress = string.IsNullOrWhiteSpace(APIController.instance.userDetails.serverInfo.server_host) ? "gameserver.utwebapps.com" : APIController.instance.userDetails.serverInfo.server_host;
            NetMirrorWork.GetComponent<SimpleWebTransport>().port = ushort.Parse(APIController.instance.userDetails.serverInfo.server_port.ToString());*/
#endif
            Val.text = "10.00 " + APIController.instance.userDetails.currency_type + " will be taken as bet for this round.";
            CurrentPlayerData.SetNickName(APIController.instance.userDetails.name);
            if (APIController.instance.userDetails.isBlockApiConnection)
            {
                DemoText.SetActive(true);
                Info.SetActive(true);
            }
            else
            {
                DemoText.SetActive(false);
                Info.SetActive(true);
            }
            while (string.IsNullOrWhiteSpace(APIController.instance.userDetails.currency_type))
            {
                if (!UIController.Instance.Loading.activeSelf)
                {
                    UIController.Instance.Loading.SetActive(true);
                }
                return;
            }

            if (GameManager.localInstance == null && !UIController.Instance.PlayAgain.activeSelf)
            {

#if !UNITY_SERVER
                StopCoroutine(nameof(InitializePlayerManager));
                StartCoroutine(nameof(InitializePlayerManager));
#endif
                Invoke(nameof(EnableStartPopupAfterDelay), 2);
            }
        }



        /// <summary>
        /// Initialize Player manager at the start to check the server session
        /// </summary>
        /// <returns></returns>
        public IEnumerator InitializePlayerManager()
        {
            if (!NetworkClient.isConnected && !NetworkClient.isConnecting)
            {
                DebugHelper.Log("Starting Client");
                NetworkManager.singleton.StartClient();
            }
            else
            {
                Debug.LogWarning("Already connected or connecting!");
            }
            DebugHelper.Log("try to Connect Server");
            while (!NetworkClient.isConnected)
            {
                if (!NetworkClient.isConnecting)
                    NetworkManager.singleton.StartClient();
                yield return null;
            }
            DebugHelper.Log("Connected to Server");
            if (!PlayerManager.localPlayer)
                NetworkClient.AddPlayer();
            while (!PlayerManager.localPlayer)
            {
                yield return null;
            }
            DebugHelper.Log($"Added Local Player{PlayerManager.localPlayer}");
            while (!NetworkClient.isConnected)
                yield return null;
        }


        /// <summary>
        ///  This Function will be called when an userbalance gets updated and has its Subcription in start
        /// </summary>
        public void SetPlayerAmountOnUpdateActionCall()
        {
            CurrentPlayerData.SetGold(APIController.instance.userDetails.balance);
            UIController.Instance.CurrenyType.text = APIController.instance.userDetails.currency_type;
            UIController.Instance.Type.text = APIController.instance.userDetails.currency_type;
            GamePlayUI.instance.profileAmount.text = APIController.instance.userDetails.balance.ToString("F2");
            if (GameManager.localInstance != null)
            {
                TeenpattiGameUIHandler.instance.MyPlayerAmount.text = APIController.instance.userDetails.balance.ToString("F2");
                PlayerManager.localPlayer.CmdSetAmountFormAPI(APIController.instance.userDetails.balance, CurrentPlayerData.GetPlayfabID());

            }

        }

        /// <summary>
        ///  This Function will be called when an user deposits amount in game and has its Subcription in start
        /// </summary>
        public void OnDepositActionCall()
        {
            if (APIController.instance.isClickDeopsit)
            {
                if (GameManager.localInstance == null && !UIController.Instance.PlayAgain.activeSelf)
                {
                    if (GameController.Instance.CheckAmountForPlay(11))
                    {
                        UIController.Instance.PlayAgain.SetActive(false);
                        StartPopUp.SetActive(false);
                        UIController.Instance.Insufficient.SetActive(true);
                        UIController.Instance.Loading.SetActive(false);
                    }
                    else
                    {
                        UIController.Instance.IsRejoin = true;
                        CanRejoin = true;
                        StartPopUp.SetActive(true);
                        UIController.Instance.Loading.SetActive(false);
                    }
                }
                APIController.instance.isClickDeopsit = false;
            }
        }

        /// <summary>
        ///    This Function will be called when an user cancels deposit in game and has its Subcription in start
        /// </summary>
        public void OnCancelDepositAmountActionCall(bool Success)
        {
            if (Success)
            {
                Invoke(nameof(OnPaymentPageSuceessorFail), 1);
            }
        }

        /// <summary>
        ///  Runs every time a Tab is switched and has its Subcription in start
        /// </summary>
        private void OnSwitchTabActionCall(bool istrue)
        {
#if UNITY_WEBGL
            if (istrue && APIController.instance.isInFocus && isAudioPlayStarted)
            {
                AudioListener.volume = 1;
            }
            else
            {
                AudioListener.volume = 0;
            }
#endif
        }

        /// <summary>
        ///  Runs based on network status ( offline / online ) and has its Subcription in start
        /// </summary>
        public void GetNetworkStatusActionCall(string data)
        {
            if (data == "true")
            {
                isConnectedtoInternet = true;
                if (!APIController.instance.userDetails.isBlockApiConnection && isAudioPlayStarted)
                {

                }
                StopCoroutine(nameof(CheckInternet));
            }
            else
            {
                StartCoroutine(nameof(CheckInternet));
            }
        }


        public IEnumerator CheckInternet()
        {
            isConnectedtoInternet = false;
            yield return new WaitForSeconds(5);
        }

        #endregion

        #region HELPER_FUNCTIONS 

        /// <summary>
        ///  An API Check to verify the payment process
        /// </summary>
        public void OnPaymentPageSuceessorFail()
        {
            if (GameController.Instance.CheckAmountForPlay(11))
            {
                UIController.Instance.PlayAgain.SetActive(false);
                StartPopUp.SetActive(false);
                UIController.Instance.Insufficient.SetActive(true);
                UIController.Instance.Loading.SetActive(false);

            }
            else
            {
                if (GameManager.localInstance == null && GameManager.localInstance.gameState.currentState >= 2)
                {
                    UIController.Instance.PlayAgain.SetActive(false);
                    UIController.Instance.Insufficient.SetActive(false);
                    UIController.Instance.Loading.SetActive(false);
                    UIController.Instance.IsRejoin = true;
                    CanRejoin = true;
                    StartPopUp.SetActive(true);
                }
                else if (GameManager.localInstance != null && GameManager.localInstance.gameState.currentState == 2)
                {
                    if (GameController.Instance.CheckAmountForPlay(11))
                    {

                    }
                }
            }
        }

        /// <summary>
        ///  A method to enable startgame popupafter delay
        /// </summary>
        public void EnableStartPopupAfterDelay()
        {
            UIController.Instance.Loading.SetActive(false);
            StartPopUp.SetActive(true);
        }


        /// <summary>
        ///  This method allows user to join that particular table with a unique mode
        /// </summary>
        [SerializeField]
        public GameTableModel CurrentGameModelTable
        {
            get { return CurrentGameTableData(); }
            set
            {
                CurrentGameTable = GameTable.ROOKIE;

            }
        }

        /// <summary>
        /// This method returns the current table datat the user has joined
        /// </summary>
        public GameTable CurrentGameTable
        {
            get { return _GameTable; }
            set
            {
                _GameTable = value;

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public TeenPattiGameData GetCurrentGameData()
        {
            string GameNameandOperator = APIController.instance.userDetails.game_Id;
            string[] val = GameNameandOperator.Split('_');
            TeenPattiGameData data = new TeenPattiGameData();
            data.CurrentGameMode = CurrentGameMode;
            data.CurrentGameType = CurrentGameType;
            data.Dulex = Dulex;
            data.CurrentAmountType = CurrentAmountType;
            data.CurrentGameModelTable = ((TeenPattiTableModel)CurrentGameModelTable);
            data.isBotWin = APIController.instance.winningStatus.WinProbablity <= 0;
            data.Commission = APIController.instance.userDetails.commission;
            data.gameId = APIController.instance.userDetails.gameId;
            data.gameName = val[1];
            data.operatorName = val[0];
            data.WinProbability = APIController.instance.winningStatus.WinProbablity;
            data.isBlockedAPI = APIController.instance.userDetails.isBlockApiConnection;
            data.serverInfo = APIController.instance.userDetails.serverInfo.instance_id;
            data.domainURL = APIController.instance.userDetails.operatorDomainUrl;
            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        public GameModeModel GameModeModels
        {
            get { return _gameModeModels; }
            private set { _gameModeModels = value; }
        }



        /// <summary>
        ///  This method stores the current game table data 
        /// </summary>
        /// <returns></returns>
        public GameTableModel CurrentGameTableData()
        {
            TeenPattiTableModel _gameModelTable = new TeenPattiTableModel();
            _gameModelTable.BlindLimit = 4;
            _gameModelTable.PotLimit = 1000;
            _gameModelTable.ChaalLimit = 320;
            _gameModelTable.BootAmount = 10;
            _gameModelTable.Id = 0;
            _gameModelTable.Name = "TPG";
            _gameModelTable.IsEnable = true;
            return _gameModelTable;
        }

        #endregion

        #region START_GAME_TEENPATTI

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameType"></param>
        /// <param name="lobbyName"></param>
        public void StartGame(int gameType, string lobbyName)
        {
            StartCoroutine(CheckAndConnectMirrorNetwork(gameType, lobbyName));
        }


        /// <summary>
        /// 
        /// </summary>
        public void StartGameOnButtonClick()
        {
            UIController.Instance.Loading.SetActive(true);
            CanRejoin = false;
            if (UIController.Instance.IsRejoin)
            {
                UIController.Instance.StrtGameAfterDeposit();
                return;
            }
            OnStartButtonClick = true;
            UIController.Instance.FindGameWEBGL();
        }

        #endregion

        #region START_MIRROR_SERVER


        /// <summary>
        ///  This method is called on client when we have successfully connected 
        /// </summary>
        public void OnConnectedToServer()
        {
            NetworkManager.singleton.StartServer();
        }



        /// <summary>
        /// This method used to verify and allow user to join the mirror server 
        /// </summary>
        /// <param name="gameType"></param>
        /// <param name="lobbyName"></param>
        /// <returns></returns>
        IEnumerator CheckAndConnectMirrorNetwork(int gameType, string lobbyName)
        {
            if (!UIController.Instance.IsRejoin)
            {
                if (isAttemptingToReconnect)
                    yield break;
                isAttemptingToReconnect = true;
            }
            yield return StartCoroutine(SearchAndJoinMirrorServer(gameType, lobbyName));
            isAttemptingToReconnect = false;
        }


        /// <summary>
        /// This  method is used to check and verify player balance before allowing to join the game server
        /// </summary>
        /// <param name="minimumBalanceForPlayGame"></param>
        /// <param name="isenterTournament"></param>
        /// <returns></returns>
        public bool CheckAmountForPlay(double minimumBalanceForPlayGame, bool isenterTournament = false)
        {
            minimumBalanceForPlayGame = 11;
            double currentAmount = 0;
            currentAmount = APIController.instance.userDetails.balance;
            float balanceCheckTime = 0;
            while (balanceCheckTime < 3 && APIController.instance.userDetails.balance <= 0)
            {
                balanceCheckTime += Time.deltaTime;
            }
            return currentAmount < minimumBalanceForPlayGame;
        }


        /// <summary>
        ///  This method is used to join and Search game in mirror server
        /// </summary>
        /// <param name="gameType"></param>
        /// <param name="lobbyName"></param>
        /// <returns></returns>
        IEnumerator SearchAndJoinMirrorServer(int gameType, string lobbyName)
        {
            yield return new WaitForSeconds(.5f);
            if (!NetworkClient.isConnected && !NetworkClient.isConnecting)
            {
                NetworkManager.singleton.StartClient();
            }

            while (!NetworkClient.isConnected)
            {
                if (!NetworkClient.isConnecting)
                    NetworkManager.singleton.StartClient();
                yield return null;
            }
            if (!PlayerManager.localPlayer)
                NetworkClient.AddPlayer();
            while (!PlayerManager.localPlayer)
            {
                yield return null;
            }
            while (!NetworkClient.isConnected)
                yield return null;


            PlayerManager.localPlayer.UpdatePlayerData(JsonUtility.ToJson(CurrentPlayerData), CurrentAmountType == CashType.CASH, CurrentPlayerData.GetPlayfabID());

            if (isInGame || UIController.Instance.IsRejoin)
            {
                Debug.Log("Rejoin ================>");
                UIController.Instance.IsRejoin = false;
                PlayerManager.localPlayer.SearchGame((int)CurrentGameMode, currentLobbyName, true);
            
               // PlayerManager.localPlayer.JoinGame(currentRoomName);
            }

            if (GameController.Instance.SearchOnInternetCheck)
            {
                Debug.Log("Start Game ================>");
                PlayerManager.localPlayer.SearchGame((int)CurrentGameMode, currentLobbyName, false);
                GameController.Instance.SearchOnInternetCheck = false;
            }

            if (OnStartButtonClick)
            {
                Debug.Log("Start Game ================>  2");
                OnStartButtonClick = false;
                PlayerManager.localPlayer.SearchGame((int)CurrentGameMode, currentLobbyName, false);
            }

        }

        #endregion

        #region DISCONNECT_FROM_MIRROR_SERVER

        /// <summary>
        /// This method is used to shut down mirror server completly 
        /// </summary>
        public void DisconnectClient()
        {
            isInGame = false;
            NetworkClient.Shutdown();
            UIController.Instance.ShowMainMenu();
        }
        #endregion

        #region RECONNECT_TO_MIRROR_SERVER

        /// <summary>
        /// This method is used for Reconnection
        /// </summary>
        public void Reconnect()
        {
            if (isInGame)
                StartCoroutine(CheckAndConnectMirrorNetwork(0, currentLobbyName));

        }
        #endregion

    }
    #region HELPER_CLASS
    /// <summary>
    ///  Sends All API related data from Client to Server
    /// </summary>

    [System.Serializable]
    public class TeenPattiGameData
    {
        public float Commission;
        public GameMode CurrentGameMode;
        public CashType CurrentAmountType;
        public GameType CurrentGameType;
        public bool IsTournament;
        public int Dulex = 0;
        public TeenPattiTableModel CurrentGameModelTable;
        public TournamentTableModel CurrentTournamentModel;
        public bool isBotWin;
        public string gameName;
        public string operatorName;
        public string gameId;
        public float WinProbability;
        public bool isBlockedAPI;
        public string serverInfo;
        public string domainURL;

    }

    #endregion
}
