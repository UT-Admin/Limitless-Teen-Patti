using Mirror;
using Mirror.SimpleWeb;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
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
        public bool toCheckIfPlayerHasReJoined;
        public bool OnCancelDepositPopupBool = false;
        public bool MoneyFlowCheck;
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
       /* public GameObject StartPopUp;*/

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
        public string CurrentServerHost;
        public string CurrentServerPort;
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


        [Header("=========LIST==========")]
        public BuyInScreenDataHolder BuyScreenData = new();

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
            CurrentGameMode = GameMode.NOLIMITS;
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
            Debug.Log("SetDataActionCall ===> 1");
            Val.text = "10.00 " + APIController.instance.userDetails.currency_type + " will be taken as bet for this round.";
            CurrentPlayerData.SetNickName(APIController.instance.userDetails.name);
            Debug.Log("SetDataActionCall ===> 1.1");
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
            Debug.Log("SetDataActionCall ===> 2");
            while (string.IsNullOrWhiteSpace(APIController.instance.userDetails.currency_type))
            {
                if (!UIController.Instance.Loading.activeSelf)
                {
                    UIController.Instance.Loading.SetActive(true);
                }
                return;
            }
            Debug.Log("SetDataActionCall ===> 3");
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
            DebugHelper.Log("SetPlayerAmountOnUpdateActionCall ==============>  " + GameController.Instance.isInGame + " =========== " + (GameManager.localInstance != null));
            CurrentPlayerData.SetGold(APIController.instance.userDetails.balance);
            UIController.Instance.CurrenyType.text = APIController.instance.userDetails.currency_type;
            UIController.Instance.Type.text = APIController.instance.userDetails.currency_type;
            GamePlayUI.instance.profileAmount.text = APIController.instance.userDetails.balance.ToString("F2");

            if (GameManager.localInstance != null)
            {
                TeenpattiGameUIHandler.instance.MyPlayerAmount.text = APIController.instance.userDetails.balance.ToString("F2");
                PlayerManager.localPlayer.CmdSetAmountFormAPI(APIController.instance.userDetails.balance, CurrentPlayerData.GetPlayfabID());
                OnPaymentPageSuceessorFail();
            }
            else
            {

                if (!GameController.Instance.isInGame && !MoneyFlowCheck && !GameController.Instance.CheckAmountForPlay((APIController.instance.userDetails.bootAmount + 1)))
                {
                    OnStartButtonClick = true;
                    UIController.Instance.NewGamePopUp.SetActive(true);
                    UIController.Instance.PlayAgain.SetActive(false);
                    UIController.Instance.Insufficient.SetActive(false);
                    UIController.Instance.Loading.SetActive(false);
                    UIController.Instance.IsRejoin = false;
                    //StartPopUp.SetActive(false);
                    UIController.Instance.ByInPage.SetActive(false);
                    DebugHelper.Log("SetPlayerAmountOnUpdateActionCall ==============> 1");
                }

                if (MoneyFlowCheck)
                {
                    MoneyFlowCheck = false;
                }

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
                        /*StartPopUp.SetActive(false);*/
                        UIController.Instance.Insufficient.SetActive(true);
                        UIController.Instance.Loading.SetActive(false);
                    }
                    else
                    {
                        UIController.Instance.IsRejoin = true;
                        CanRejoin = true;
                      /*  StartPopUp.SetActive(true);*/
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
                OnCancelDepositPopupBool = true;
                UIController.Instance.Loading.SetActive(true);
                APIController.instance.GetUpdatedBalance();

                //Invoke(nameof(OnPaymentPageSuceessorFail), 1);

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
        public void GetNetworkStatusActionCall(NetworkStatus data)
        {
            /*if (data == NetworkStatus.Active)
            {
                isConnectedtoInternet = true;
                if (!APIController.instance.userDetails.isBlockApiConnection && isAudioPlayStarted)
                {

                }
                StopCoroutine(nameof(CheckInternet));
            }
            else if (data == NetworkStatus.NetworkIssue)
            {
                if (NetworkClient.isConnected)
                {
                    NetworkClient.Disconnect();
                }
                MirrorManager.instance.gameObject.SetActive(false);
                StartCoroutine(nameof(CheckInternet));

            }
            else if (data == NetworkStatus.ServerIssue)
            {
                APIController.instance.CheckMirrorGameAvaliblity(CurrentServerHost, CurrentServerPort, (success, message) =>
                {
                    if (!success)
                    {
                       
                    }

                });


            }*/
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
            DebugHelper.Log("OnCancelDepositPopupBool case 1  ==============> " + APIController.instance.userDetails.balance);
            if (GameController.Instance.CheckAmountForPlay((APIController.instance.userDetails.bootAmount + 1)) && GameManager.localInstance.gameState.currentState <= 0)
            {
                UIController.Instance.PlayAgain.SetActive(false);
                //StartPopUp.SetActive(false);
                UIController.Instance.ByInPage.SetActive(false);
                UIController.Instance.Insufficient.SetActive(true);
                UIController.Instance.Loading.SetActive(false);
                UIController.Instance.NewGamePopUp.SetActive(false);
                DebugHelper.Log("OnCancelDepositPopupBool case 2 ==============> " + APIController.instance.userDetails.balance);
            }
            else
            {
                if (GameManager.localInstance.gameState.currentState == 2)
                {
                    if (GameController.Instance.CheckAmountForPlay((APIController.instance.userDetails.bootAmount + 1)))
                    {
                        DebugHelper.Log("Amount Check Success ==============> ");
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
            //StartPopUp.SetActive(true);
            if (APIController.instance.authentication.game_data.entryAmounts.Count == 1)
            {
                APIController.instance.userDetails.bootAmount = APIController.instance.authentication.game_data.entryAmounts[0];
                APIController.instance.userDetails.potLimit = APIController.instance.authentication.game_data.potLimits[0];
                APIController.instance.userDetails.challLimit = APIController.instance.authentication.game_data.chaalLimits[0];
                APIController.instance.authentication.challLimit = APIController.instance.authentication.game_data.chaalLimits[0];
                APIController.instance.authentication.bootAmount = APIController.instance.authentication.game_data.entryAmounts[0];

                if (!UIController.Instance.NewGamePopUp.activeSelf)
                {
                    StartGameOnButtonClick();
                    isAudioPlayStarted = true;
                    AudioListener.volume = 1;
                }

            }
            else
            {
                UIController.Instance.Connecting.SetActive(false);
                UIController.Instance.ByInPage.SetActive(true);
            }

            UIController.Instance.SettingsPanel.CheckPlayerprefs();
        }





        public void GameStartUp()
        {
#if !UNITY_SERVER
            APIController.instance.CheckInternetForButtonClick((Success) =>
            {

                if (Success)
                {
                    if (PlayerManager.localPlayer != null)
                    {
                        SetVolumeOn();
                        GameController.Instance.StartGameOnButtonClick();
                        /*this.gameObject.SetActive(false);*/
                    }
                    else
                    {
                        UIController.Instance.FindGameWEBGL();
                        GameController.Instance.SearchOnInternetCheck = true;
                        /*this.gameObject.SetActive(false);*/
                    }


                }
                else
                {

                }


            });


#endif
        }


        public void SetVolumeOn()
        {
            Debug.Log("AUDIO SOUND ============>");
            GameController.Instance.isAudioPlayStarted = true;
            AudioListener.volume = 1;
            /* if (SoundToggle.isOn)
                 MasterAudioController.instance.PlayAudio(AudioEnum.BUTTONCLICK);*/

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
            data.environment = APIController.instance.authentication.environment;
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
            DebugHelper.Log("Test this  ===============> " + APIController.instance.userDetails.potLimit);
            _gameModelTable.BlindLimit = 4;
            _gameModelTable.PotLimit = (int)APIController.instance.userDetails.potLimit;
            _gameModelTable.ChaalLimit = (int)APIController.instance.userDetails.challLimit;
            _gameModelTable.BootAmount = (int)APIController.instance.userDetails.bootAmount;
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

        int Timer1;
        /// <summary>
        ///  This method is used to join and Search game in mirror server
        /// </summary>
        /// <param name="gameType"></param>
        /// <param name="lobbyName"></param>
        /// <returns></returns>
        IEnumerator SearchAndJoinMirrorServer(int gameType, string lobbyName)
        {
            DebugHelper.Log("SearchAndJoinMirrorServer1 ================>");


            yield return new WaitForSeconds(.1f);
            if (!NetworkClient.isConnected && !NetworkClient.isConnecting)
            {
                NetworkManager.singleton.StartClient();
            }
            DebugHelper.Log("SearchAndJoinMirrorServer2 ================>");

                    //UIController.Instance.ConnectionIssue.SetActive(true);
            /*APIController.instance.CheckMirror(async (success) =>
            {
                if (!success)
                {
                    return;
                }
            });*/

            DebugHelper.Log("SearchAndJoinMirrorServer3 ================>");

            while (!NetworkClient.isConnected)
            {
                if (!NetworkClient.isConnected && !NetworkClient.isConnecting)
                {
                    DebugHelper.Log("==========> " + Timer1);
                    NetworkManager.singleton.StartClient();
                }

                Timer1++;

                if (Timer1 > 20)
                {
                    Timer1 = 0;


                    /*APIController.instance.CheckMirror(async (success) =>
                    {
                        if (!success)
                        {
                            UIController.Instance.ConnectionIssue.SetActive(true);
                        }
                        else
                        {
                            UIController.Instance.ConnectionIssue.SetActive(false);
                            UIController.Instance.InternetPopNew.SetActive(true);
                        }
                    });
                    yield break;*/

                }
                yield return new WaitForSeconds(0.1f);
            }
            DebugHelper.Log("SearchAndJoinMirrorServer4 ================>");

            if (!PlayerManager.localPlayer)
                NetworkClient.AddPlayer();

            DebugHelper.Log("SearchAndJoinMirrorServer5 ================>");

            while (!PlayerManager.localPlayer)
            {
                yield return null;
            }
            while (!NetworkClient.isConnected)
                yield return null;

            DebugHelper.Log("AuthValue ================>" + APIController.instance.authentication.currency_type);

            PlayerManager.localPlayer.UpdatePlayerData(JsonUtility.ToJson(CurrentPlayerData), CurrentAmountType == CashType.CASH, CurrentPlayerData.GetPlayfabID());

            if (isInGame || UIController.Instance.IsRejoin)
            {
                DebugHelper.Log("Rejoin ================>");

                UIController.Instance.IsRejoin = false;
                PlayerManager.localPlayer.SearchGame((int)CurrentGameMode, currentLobbyName, true);

            }

            if (GameController.Instance.SearchOnInternetCheck)
            {
                DebugHelper.Log("Start Game ================>");
                PlayerManager.localPlayer.SearchGame((int)CurrentGameMode, currentLobbyName, false);
                GameController.Instance.SearchOnInternetCheck = false;
            }

            if (OnStartButtonClick)
            {
                DebugHelper.Log("Start Game ================>  2");
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
        public double chaalLimit;
        public double potLimit;
        public string environment;
        public string currency;

    }


    [Serializable]
    public class BuyInScreenDataHolder
    {
        public List<SelectedAmountData> selectedAmountData = new List<SelectedAmountData>();
    }

    [Serializable]
    public class SelectedAmountData
    {
        public float Commission;
        public float entryFee;
        public float ChaalAmount;
        public float potAmount;
        public string Currency;


    }

    [Serializable]
    public class Card
    {
        [NonReorderable]
        public List<TextMeshProUGUI> cardRankTxt;
        [NonReorderable]
        [Space(1)]
        public List<Image> suitImageSmall;
        [NonReorderable]
        [Space(1)]
        public List<Image> suitImageBig;
        [NonReorderable]
        [Space(1)]
        public List<Image> courtImage;

        public Image backImage;
        public Image packedImage;
        public GameObject glowImage;

        [HideInInspector]
        public int cardIndex;

        public void Intialize(int index)
        {
            cardIndex = index;
        }
    }
    #endregion



}
