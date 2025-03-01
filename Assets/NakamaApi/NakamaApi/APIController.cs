using Cysharp.Threading.Tasks;
using Mirror;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using TP;
using UnityEngine;
using static WebApiManager;

[Serializable]
public class BetRequest
{
    public string BetId;
    public string PlayerId;
    public string MatchToken;
    public int betId;
}

//[Serializable]
//public class APIResponse
//{
//    public string url;
//    public string code;
//    public string message;
//}

[Serializable]
public class APIRequestList
{
    public string url;
    public ReqCallback callback;
}
[System.Serializable]
public class ServerInfo
{
    public string instance_id;
    public string game_name;
    public string server_host;
    public string server_scheme;
    public int server_port;
    public float server_cpu_usage;
    public float server_memory_usage;
    public DateTime create_at;
}
public class APIController : MonoBehaviour
{

    #region REST_API_VARIABLES
    public static APIController instance;
    [Header("==============================================")]

    #endregion
    [Header("StartAuthentication")]
    public string DummyData;
    public bool IsLiveGame;
    public Action OnUserDetailsUpdate;
    public Action OnUserBalanceUpdate;
    public Action OnUserDeposit;
    public Action<bool> OnCancelDepositPopup;
    public Action<NetworkStatus> OnInternetStatusChange;
    public Action<bool> OnSwitchingTab;
    public bool isWin = false;
    public bool IsBotInGame = true;
    public GameWinningStatus winningStatus;
    public UserGameData userDetails;
    public List<BetDetails> betDetails = new List<BetDetails>();
    public List<BetRequest> betRequest = new List<BetRequest>();
    public bool isPlayByDummyData;
    public double maxWinAmount;
    public bool isClickDeopsit = false;
    public string testJson;
    public string defaultGameName;
    public int defaultBootAmount;
    public string DefaultHostAddress;
    public string DefaultHostSchema;
    public int DefaultHostPort;
    public List<APIRequestList> apiRequestList;
    public BackendAPI backendAPIURL = new();

    //public List<ApiResponse> apiResponse;
    //    public List<ApiRequest> apis = new List<ApiRequest>();

    public List<string> PlayerIDs = new() { "f0255647-61d5-4807-b700-352ce052c791", "f24c7429-3946-4567-87be-e258571f704f", "72b183bc-fcbb-4ed4-9061-775d3e908731", "72d43cf3-7bbb-4b88-b443-0669b1390d5e", "cb282b1f-d52f-4958-a28c-9c4a31610877", "bd004aed-2912-46ed-b30b-216e861376e4", "4b2921b6-275b-4454-b61a-d120b57933f3" };

#if UNITY_WEBGL
    #region WebGl Events

    [DllImport("__Internal")]
    public static extern void GetLoginData();
    [DllImport("__Internal")]
    public static extern void DisconnectGame(string message);
    [DllImport("__Internal")]
    public static extern void ExternalApiResponse(string message);
    [DllImport("__Internal")]
    public static extern void UpdateBalance();
    [DllImport("__Internal")]
    public static extern void FullScreen();
    [DllImport("__Internal")]
    private static extern void ShowDeposit();

    [DllImport("__Internal")]
    public static extern void CloseWindow();

    [DllImport("__Internal")]
    public static extern void CheckOnlineStatus();

    private Action<BotDetails> GetABotAction;
    [DllImport("__Internal")]
    private static extern void GetABot();

    [DllImport("__Internal")]
    private static extern void InitPlayerBet(string type, int index, string game_user_Id, string game_Id, string metaData, string isAbleToCancel, double bet_amount, int isBot);

    [DllImport("__Internal")]
    private static extern void AddPlayerBet(string type, int index, string id, string metaData, string game_user_Id, string game_Id, double bet_amount, int isBot);
    [DllImport("__Internal")]
    private static extern void CancelPlayerBet(string type, string id, string metaData, string game_user_Id, string game_Id, double amount, int isBot);
    [DllImport("__Internal")]
    private static extern void FinilizePlayerBet(string type, string id, string metadata, string game_user_Id, string game_Id, int isBot);
    [DllImport("__Internal")]
    private static extern void WinningsPlayerBet(string type, string id, string metadata, string game_user_Id, string game_Id, double win_amount, double spend_amount, int isBot);
    [DllImport("__Internal")]
    private static extern void MultiplayerWinningsPlayerBet(string type, string id, string metadata, string game_user_Id, string game_Id, double win_amount, double spend_amount, double pot_amount, int isBot, int isWinner);
    [DllImport("__Internal")]
    private static extern void GetRandomPrediction(string type, int rowCount, int columnCount, int predectedCount);
    private Action<string, bool> GetPredictionAction;

    [DllImport("__Internal")]
    public static extern void ExecuteExternalUrl(string url, int timout);


    [DllImport("__Internal")]
    public static extern void SetAudio(int sound, int music);

    [DllImport("__Internal")]
    public static extern void InternetCheckResponse();


    #endregion

    #region WebGl Response
    [ContextMenu("check json")]
    public void CheckJson()
    {
        InitPlayerBetResponse(testJson);
    }

    public void GetABotResponse(string data)
    {
        DebugHelper.Log("get bot response :::::::----::: " + data);

        BotDetails bot = new BotDetails();
        bot = JsonUtility.FromJson<BotDetails>(data);
        GetABotAction?.Invoke(bot);
        GetABotAction = null;
        DebugHelper.Log("get bot response :::::::----::: after response " + data);
    }

    public void UpdateBalanceResponse(double data)
    {
        if (string.IsNullOrEmpty(userDetails.Id)) return;
        if (userDetails.isBlockApiConnection) return;
        DebugHelper.Log("Balance Updated response  :::::::----::: " + data);
        userDetails.balance = data;
        OnUserBalanceUpdate?.Invoke();
        if (isClickDeopsit)
        {
            OnUserDeposit?.Invoke();

        }
    }

    [HideInInspector] public bool isOnline = true;
    public bool isInFocus = true;
    public bool isNeedToPauseWhileSwitchingTab = false;
    public void GetNetworkStatus(string data)
    {
        Time.timeScale = 1;
        isOnline = data == "true" ? true : false;
        DebugHelper.Log($"Calleeedddd check internet {data}   -   {isOnline}   -   {isInFocus}");
        if (isNeedToPauseWhileSwitchingTab)
        {
            if (isInFocus && isOnline)
            {
                Time.timeScale = 1;
            }
            else
            {
                Time.timeScale = 0;

            }
        }
        if (isOnline)
        {
            OnInternetStatusChange?.Invoke(NetworkStatus.Active);
        }
        else
        {


            string val = IsLiveGame ? APIController.instance.userDetails.serverInfo.server_host : "test.gameservers.utwebapps.com";
            CheckInternetForButtonClick((status) =>
            {
                if (status)
                {
                    OnInternetStatusChange?.Invoke(NetworkStatus.ServerIssue);
                }
                else
                    OnInternetStatusChange?.Invoke(NetworkStatus.NetworkIssue);
            });
        }

    }

    public void InternetCheckResponse(string status)
    {
        Debug.Log($"Received internet check response ::: {status}, Is Action null {InternetCheckAction == null}");
        InternetCheckAction?.Invoke(status.ToLower().Equals("true"));
    }

    Action<bool> InternetCheckAction = null;


    public async void CheckInternetForButtonClick(Action<bool> action)
    {
#if !UNITY_EDITOR && !UNITY_SERVER
        InternetCheckAction = action;
        InternetCheckResponse(); 
#else
        action?.Invoke(true);
        DebugHelper.Log($"Received internet check response ::: , Is Action null");
#endif
    }

    public void OnSwitchingTabs(string data)
    {
        Time.timeScale = 1;
        isInFocus = data == "true" ? true : false;
        DebugHelper.Log($"Calleeedddd switching tab {data}   -   {isOnline}   -   {isInFocus}");
        if (isNeedToPauseWhileSwitchingTab)
        {
            if (isInFocus && isOnline)
            {
                Time.timeScale = 1;
            }
            else
            {
                Time.timeScale = 0;
            }
        }
        OnSwitchingTab?.Invoke(data.ToLower() == "true");
    }

    public void UpdateAudioSettings()
    {


#if !UNITY_EDITOR && !UNITY_SERVER
        SetAudio(APIController.instance.authentication.sound ? 1 : 0, APIController.instance.authentication.music ? 1 : 0);
        DebugHelper.Log("UpdateAudioSettings Called");
#endif
    }

    public void InitPlayerBetResponse(string data)
    {
        DebugHelper.Log("init bet response :::::::----::: " + data);
        InitBetDetails response = JsonUtility.FromJson<InitBetDetails>(data);
        BetDetails bet = betDetails.Find(x => x.index == response.index);
        if (response.status)
        {
            winningStatus = response.message;
            DebugHelper.Log("init bet response :::::::----::: " + response.message);
            DebugHelper.Log("init bet response :::::::----::: " + winningStatus.Id);
            bet.betID = winningStatus.Id;
            bet.Status = BetProcess.Success;
            bet.betIdAction?.Invoke(winningStatus.Id);
            bet.action?.Invoke(true);
        }
        else
        {
            bet.action?.Invoke(false);
            betDetails.RemoveAll(x => x.index == response.index);
        }
        bet.action = null;
    }

    public void CancelPlayerBetResponse(string data)
    {
        DebugHelper.Log("cancel bet response :::::::----::: " + data);
        BetResponse response = JsonUtility.FromJson<BetResponse>(data);
        if (betDetails.Exists(x => x.index == response.index))
        {
            BetDetails bet = betDetails.Find(x => x.index == response.index);
            if (response.status)
            {
                bet.Status = response.status ? BetProcess.Success : BetProcess.Failed;
                bet.action?.Invoke(true);
            }
            else
            {
                bet.action?.Invoke(false);
            }
            bet.action = null;
        }
    }

    public void DepositCancelResponse(string data)
    {
        DebugHelper.Log("DepositCancelResponse  :::::::----::: " + data);
        OnCancelDepositPopup?.Invoke(data.ToLower() == "true");
    }

    public void CheckInternet()
    {
#if !UNITY_EDITOR
        CheckOnlineStatus();
#endif
    }

    public void AddPlayerBetResponse(string data)
    {
        DebugHelper.Log("add bet response :::::::----::: " + data);
        BetResponse response = JsonUtility.FromJson<BetResponse>(data);
        if (betDetails.Exists(x => x.index == response.index))
        {
            BetDetails bet = betDetails.Find(x => x.index == response.index);
            if (response.status)
            {
                bet.Status = response.status ? BetProcess.Success : BetProcess.Failed;
                bet.action?.Invoke(true);
            }
            else
            {
                bet.action?.Invoke(false);
            }
            bet.action = null;
        }
    }

    public void FinilizePlayerBetResponse(string data)
    {
        BetResponse response = JsonUtility.FromJson<BetResponse>(data);
        if (betDetails.Exists(x => x.index == response.index))
        {
            BetDetails bet = betDetails.Find(x => x.index == response.index);
            if (response.status)
            {
                bet.Status = response.status ? BetProcess.Success : BetProcess.Failed;
                bet.action?.Invoke(true);
            }
            else
            {
                bet.action?.Invoke(false);
            }
            bet.action = null;
        }
    }

    public void WinningsPlayerBetResponse(string data)
    {
        DebugHelper.Log("winning bet response :::::::----::: " + data);
        BetResponse response = JsonUtility.FromJson<BetResponse>(data);
        if (betDetails.Exists(x => x.index == response.index))
        {
            BetDetails bet = betDetails.Find(x => x.index == response.index);
            if (response.status)
            {
                bet.Status = response.status ? BetProcess.Success : BetProcess.Failed;
                bet.action?.Invoke(true);
            }
            else
            {
                bet.action?.Invoke(false);
            }
            bet.action = null;
        }
    }
    #endregion

    public void ExecuteExternalAPI(string data)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(data);
        string encryptedData = Convert.ToBase64String(bytes);
        DebugHelper.Log("unity :: base64 is :: " + encryptedData);
        encryptedData = Encryptbase64String(encryptedData);
        DebugHelper.Log("unity :: encoded base64 is :: " + encryptedData);
        ExternalApiResponse(encryptedData);
        return;
    }

    public static string Encryptbase64String(string plainText)
    {
        int shift = 0;
        char[] buffer = plainText.ToCharArray();
        for (int i = 0; i < buffer.Length; i++)
        {
            char c = buffer[i];
            if (cryptocharacters.Contains(c))
            {
                shift = key[i % key.Length];
                int value = (cryptocharacters.IndexOf(c) + cryptocharacters.IndexOf((char)shift));
                if (value >= cryptocharacters.Count)
                {
                    value = ((value - cryptocharacters.Count));
                }
                c = cryptocharacters[value];
                buffer[i] = c;
            }
            else
            {
                DebugHelper.Log("notfound" + c);
            }
        }
        return new string(buffer);
    }

#endif

    public static List<char> cryptocharacters = new List<char>();
    private static readonly string key = "Hs9INfoebjwQwtrGRMD1hPaNAMrvGXxX"; // Replace with your key
    private void Awake()
    {
        instance = this;

        for (int i = 0; i < 26; i++)
        {
            cryptocharacters.Add((char)('a' + i));
        }
        for (int i = 0; i < 26; i++)
        {
            cryptocharacters.Add((char)('A' + i));
        }
        for (int i = 0; i < 10; i++)
        {
            cryptocharacters.Add((char)('0' + i));
        }

    }


    public void OnClickDepositBtn()
    {
        isClickDeopsit = true;
#if UNITY_WEBGL
        ShowDeposit();
#endif
    }


    public void GetServerDetails(Action<ServerInfo, string> action)
    {
        ApiRequest apiRequest = new ApiRequest();///?requestType=GetGameServer&game_name=carrom
        apiRequest.url = "https://sijnoejayed2n2hodo3cot5st40ndsxd.lambda-url.ap-south-1.on.aws";
        List<KeyValuePojo> param = new List<KeyValuePojo>();
        param.Add(new KeyValuePojo { keyId = "requestType", value = "GetGameServer" });
        DebugHelper.Log(userDetails.game_Id);
        param.Add(new KeyValuePojo { keyId = "&game_name", value = userDetails.game_Id.Split("_")[1] });
        apiRequest.param = param;
        apiRequest.action = (success, error, body) =>
        {
            if (success)
            {
                ApiResponse response = JsonUtility.FromJson<ApiResponse>(body);
                if (response.code == 200)
                {
                    action.Invoke(JsonUtility.FromJson<ServerInfo>(response.message), "success");
                }
                else
                {
                    action.Invoke(new ServerInfo(), response.message);
                }
                DebugHelper.Log(response.message);
            }
            else
            {
                DebugHelper.Log(error);
                action.Invoke(new ServerInfo(), error);
            }
        };

        ExecuteAPI(apiRequest);


    }

    public void SendApiRequest(string url, ReqCallback callback)
    {

        byte[] bytesToEncode = Encoding.UTF8.GetBytes(url);

        string base64EncodedString = Convert.ToBase64String(bytesToEncode);
        apiRequestList.Add(new APIRequestList() { url = base64EncodedString, callback = callback });
        //DebugHelper.Log($"Sending Api Request URl :-" + base64EncodedString);
#if UNITY_WEBGL
        ExecuteExternalUrl(base64EncodedString, 5);
#endif

        CheckAPICallBack(base64EncodedString);

    }

    public void ReloadPage(string data)
    {
        DebugHelper.Log("Mirror Check Reload ==============> " + GameController.Instance.isInGame);
        GameController.Instance.isInGame = false;
    }


    public bool isCheckOnline;
    public async void CheckOnline()
    {
#if !UNITY_SERVER
        while (true)
        {
            isCheckOnline = false;
            if (PlayerManager.localPlayer != null)
            {
                isCheckOnline = true;
                PlayerManager.localPlayer.CMD_CheckOnline();
            }
            else
            {
                isOnline = false;
                await UniTask.Delay(2000);
                GetNetworkStatus(isOnline.ToString());
                continue;
            }
            await UniTask.Delay(2000);
            isOnline = !isCheckOnline;
            GetNetworkStatus(isOnline.ToString());

        }
#endif
    }

    int defaultDelay = 3;
    string PlayNextGameMsg;
    public void ValidateSessionResponce(string body)
    {


#if !UNITY_SERVER
        isValidateSession = false;
        bool success = false;
        JObject jsonObject = JObject.Parse(body);
        DebugHelper.Log("ValidateSessionResponce ================>   " + (int)(jsonObject["code"]));
        if ((int)(jsonObject["code"]) == 200)
        {
            DebugHelper.Log("online true 1");
            defaultDelay = (int)(jsonObject["data"]);
            isOnline = true;
            PlayNextGameMsg = "";
            success = true;
        }
        else if ((int)(jsonObject["code"]) == 203)
        {
            DebugHelper.Log("online true 1.5");
            isOnline = true;
            PlayNextGameMsg = (string)jsonObject["message"];
        }
        else if ((int)(jsonObject["code"]) != 200)
        {
            DebugHelper.Log("online true 2");
            isOnline = true;
            PlayNextGameMsg = "";
            DisconnectGame((string)jsonObject["message"]);
        }
        else
        {
            DebugHelper.Log("online false 1");
            isOnline = false;
        }


        DebugHelper.Log("TARGET valide Session Called ===================> " + success);

#endif
        //        if (success)
        //            return;
        //#if UNITY_WEBGL && !UNITY_EDITOR
        //            APIController.DisconnectGame("Session expired. Account active in another device.");
        //            DebugHelper.Log("Invalide Session ===================> ");
        //#endif

    }

    public bool IsAbleToPlayGame()
    {
        if (string.IsNullOrEmpty(PlayNextGameMsg))
        {
            return true;
        }
        else
        {
#if !UNITY_SERVER
            DisconnectGame(PlayNextGameMsg);
#endif
            return false;
        }
    }


    public void ValidateSession(string playerID, string token, string gamename, string operatorname, string session_token, Action<string> action, string environment)
    {
        List<KeyValuePojo> param = new List<KeyValuePojo>();
        param.Add(new KeyValuePojo { keyId = "user_id", value = playerID });
        param.Add(new KeyValuePojo { keyId = "user_token", value = token });
        param.Add(new KeyValuePojo { keyId = "game_name", value = gamename });
        param.Add(new KeyValuePojo { keyId = "operator", value = operatorname });
        param.Add(new KeyValuePojo { keyId = "session_token", value = session_token });
        param.Add(new KeyValuePojo { keyId = "request_type", value = "validateSession" });
        Debug.Log(" Check The CallType In the Start Game======>1");
        WebApiManager.Instance.GetNetWorkCall(NetworkCallType.POST_METHOD_USING_JSONDATA, StaticStrings.GetLambdaUrl(environment), param, (success, error, body) =>
        {

            action.Invoke(body);

        });
    }


    public void CheckMirrorGameAvaliblity(string host, string port, Action<bool, string> action)
    {

        action?.Invoke(true, "BACK END TEAM ISSUE");
        return;
        DebugHelper.Log("Mirror Server check ==========> Check Once " + host + port);
        var param = new List<KeyValuePojo>();
        param.Add(new KeyValuePojo { keyId = "request_type", value = "CheckMirrorServer" });
        param.Add(new KeyValuePojo { keyId = "host", value = host });
        param.Add(new KeyValuePojo { keyId = "port", value = port });
        param.Add(new KeyValuePojo { keyId = "game_name", value = authentication.gamename });


        if (host.ToLower().Contains("localhost"))
        {
            action?.Invoke(true, "Connected to localHost");
            return;
        }

        Debug.Log(" Check The CallType In the Start Game======>2");
        WebApiManager.Instance.GetNetWorkCall(NetworkCallType.POST_METHOD_USING_JSONDATA, StaticStrings.GetLambdaUrl(authentication.environment), param, (success, error, body) =>
        {
            DebugHelper.Log("Mirror Server check ==========> BODY " + body);
            if (success)
            {

                ApiResponse response = JsonUtility.FromJson<ApiResponse>(body);
                DebugHelper.Log("Mirror Server check ==========> IsACTIVE " + response.code + "**************" + response.message);


                if (response.code != 200)
                {
                    action.Invoke(false, response.message);

                }
                else
                {
                    action.Invoke(true, response.message);
                }

            }
            else
            {
                DebugHelper.Log("Mirror Server check ==========> " + " Not IsACTIVE");
                if (body.Contains("timeout"))
                {
                    UIController.Instance.InternetPopNew.SetActive(true);
                }
                else
                {
                    action.Invoke(false, "error");
                }
            }
        }, 1);
    }


    public bool isValidateSession = false;

    public async void ValidateSession()
    {

#if !UNITY_SERVER
        while (true)
        {
            isValidateSession = false;
            if (PlayerManager.localPlayer != null && NetworkClient.isConnected)
            {
                isValidateSession = true;
                DebugHelper.Log("Validate Session called CLIENT PLAYERMANAGER ===================> ");
                PlayerManager.localPlayer.ValidateSession(userDetails.Id, userDetails.token, userDetails.game_Id.Split("_")[1], userDetails.game_Id.Split("_")[0], userDetails.session_token, userDetails.isBlockApiConnection);
                await UniTask.Delay(defaultDelay * 1000);
                DebugHelper.Log("Need to show disconnect popup " + isValidateSession.ToString());
                if (isValidateSession)
                {

                }
                continue;
            }
            await UniTask.Delay(defaultDelay * 1000);
        }
#endif
    }

    public async void VerifyInternetAndProcess(Action<bool> action)
    {
#if !UNITY_SERVER
        isCheckOnline = false;
        if (PlayerManager.localPlayer != null)
        {
            isCheckOnline = true;
            PlayerManager.localPlayer.CMD_CheckOnline();
        }
        else
        {
            isOnline = false;
            await UniTask.Delay(2000);
            GetNetworkStatus(isOnline.ToString());
        }
        await UniTask.Delay(2000);
        isOnline = !isCheckOnline;
        GetNetworkStatus(isOnline.ToString());
        action.Invoke(isOnline);

#endif
    }

    public class ServerData
    {
        public string domainName;
        public string instanceIP;
        public double cpuUsage;
        public double memoryUsage;
    }

    public void GetLoginDataResponseFromWebGL(string data)
    {
    }


    void Start()
    {
        //   GetLambdaURL(IsLiveGame);
#if UNITY_WEBGL && !UNITY_EDITOR
         GetLoginData();
        //CheckInternetStatus();
#elif UNITY_EDITOR && !UNITY_SERVER
        StartAuthentication(DummyData);
#endif
    }

    public PlayerData GeneratePlayerDataForBot(string botAccountData)
    {
        BotData account = JsonUtility.FromJson<BotData>(botAccountData);
        PlayerWalletData walletData = JsonUtility.FromJson<PlayerWalletData>(account.wallet);
        PlayerData player = new();
        player.playerID = account.user.id;
        player.playerName = account.user.display_name;
        player.isBot = true;
        ProfilePicture profile = JsonUtility.FromJson<ProfilePicture>(account.user.avatar_url);
        player.profilePicURL = profile.ProfileUrl;
        player.avatarIndex = (int)profile.ProfileType;
        player.gold = (walletData.CashDepositVal / 100) + (walletData.CashDepositVal / 1000);
        player.silver = (walletData.SilverVal / 100);
        player.money = player.gold;
        player.totalWinnings = walletData.NetWinning;
        return player;
    }
    public void RandomPrediction_Response(string data)
    {
        DebugHelper.Log("get Prediction response :::::::----::: " + data);
#if UNITY_WEBGL
        GetPredictionAction?.Invoke(data, true);
        GetPredictionAction = null;
#endif
    }
    public void GetRandomPredictionIndex(int rowCount, int columnCount, int predectedCount, Action<string, bool> OnScucces = null)
    {
#if UNITY_WEBGL
        GetPredictionAction = OnScucces;
        GetRandomPrediction("RandomPrediction", rowCount, columnCount, predectedCount);
#endif
    }


    #region API
    int id = 0;

    public void GetBot(Action<BotDetails> action)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        GetABotAction = action;
        GetABot();
#endif
    }

    public void TerminateMatch(string matchToken, string operatorName, string enviroment)
    {
        List<KeyValuePojo> param = new List<KeyValuePojo>();
        param.Add(new KeyValuePojo { keyId = "id", value = matchToken });
        param.Add(new KeyValuePojo { keyId = "operator", value = operatorName });
        param.Add(new KeyValuePojo { keyId = "request_type", value = "TerminateMatch" });
        Debug.Log(" Check The CallType In the Start Game======>3");
        WebApiManager.Instance.GetNetWorkCall(NetworkCallType.POST_METHOD_USING_JSONDATA, StaticStrings.GetLambdaUrl(enviroment), param, (success, error, body) =>
        {
            DebugHelper.Log($"TerminateMatch => Body : {body}, Status : {success}, Error : {error}");
        });
    }

    public int InitlizeBet(double amount, TransactionMetaData metadata, bool isAbleToCancel = false, Action<bool> action = null, string playerId = "", bool isBot = false, Action<string> betIdAction = null)
    {
        if (isPlayByDummyData)
        {
            DebugHelper.Log("" + amount);
            if (string.IsNullOrWhiteSpace(playerId) || playerId == userDetails.Id)
            {
                DebugHelper.Log("Dummy Data" + amount);
                userDetails.balance -= amount;
                OnUserBalanceUpdate.Invoke();
            }
            else
            {
                DebugHelper.Log(playerId + " __ " + userDetails.Id + "__ Dummy Data ---1" + amount);
            }
            action?.Invoke(true);
            return 0;
        }
        DebugHelper.Log("Dummy Data ---2" + amount);

        id += 1;
        if (id > 1000)
            id = 1;
        //var data = new
        //{
        //    type = "InitlizeBet",
        //    Index = id,
        //    Game_user_Id = userDetails.Id,
        //    Game_Id = userDetails.game_Id,
        //    MetaData = metadata,
        //    IsAbleToCancel = isAbleToCancel,
        //    Bet_amount = amount
        //};
        BetDetails bet = new BetDetails();
        bet.index = id;
        bet.betID = id.ToString();
        bet.Status = BetProcess.Processing;
        bet.IsAbleToCancel = isAbleToCancel ? "true" : "false";
        betDetails.Add(bet);
#if UNITY_WEBGL
        DebugHelper.Log("Init Bet Data" + amount);
        bet.action = action;
        bet.betIdAction = betIdAction;
        InitPlayerBet("InitlizeBet", id, userDetails.Id, playerId == "" ? userDetails.Id : playerId, JsonUtility.ToJson(metadata), bet.IsAbleToCancel, amount, isBot ? 1 : 0);
#endif
        return id;
    }

    public void AddBet(int index, string BetId, TransactionMetaData metadata, double amount, Action<bool> action = null, string playerId = "", bool isBot = false)
    {
        if (isPlayByDummyData)
        {
            if (playerId == "" || playerId == userDetails.Id)
            {
                userDetails.balance -= amount;
                OnUserBalanceUpdate.Invoke();
            }
            action?.Invoke(true);
            return;
        }

        foreach (var item in betDetails)
        {
            DebugHelper.Log(item.betID);
        }

        if (betDetails.Exists(x => x.betID == BetId))
        {
            BetDetails bet = betDetails.Find(x => x.betID == BetId);

#if UNITY_WEBGL
            DebugHelper.Log("Add Bet Data");
            bet.action = action;
            AddPlayerBet("AddBet", index, BetId, JsonUtility.ToJson(metadata), playerId == "" ? userDetails.Id : playerId, userDetails.game_Id, amount, isBot ? 1 : 0);
#endif
        }
        else
        {
#if UNITY_WEBGL
            BetDetails bet = new BetDetails();
            id += 1;
            bet.index = id;
            bet.betID = BetId;
            bet.Status = BetProcess.Processing;
            betDetails.Add(bet);
            DebugHelper.Log("Winning Bet Data");
            AddPlayerBet("AddBet", index, BetId, JsonUtility.ToJson(metadata), playerId == "" ? userDetails.Id : playerId, userDetails.game_Id, amount, isBot ? 1 : 0);
#endif

        }
    }

    public void CancelBet(int index, string metadata, double amount, Action<bool> action = null, string playerId = "", bool isBot = false)
    {
        if (isPlayByDummyData)
        {
            if (playerId == "" || playerId == userDetails.Id)
            {
                userDetails.balance += amount;
                OnUserBalanceUpdate.Invoke();
            }
            action?.Invoke(true);
            return;
        }

        if (betDetails.Exists(x => x.index == index))
        {
            BetDetails bet = betDetails.Find(x => x.index == index);
            bet.action = action;

            if (bet.IsAbleToCancel == "false")
            {
                bet.Status = BetProcess.Failed;
                return;
            }
            bet.Status = BetProcess.Processing;
#if UNITY_WEBGL
            DebugHelper.Log("Cancel Bet Data");
            CancelPlayerBet("CancelBet", bet.betID, metadata, playerId == "" ? userDetails.Id : playerId, userDetails.game_Id, amount, isBot ? 1 : 0);
#endif
        }
    }

    public void FinilizeBet(int index, TransactionMetaData metadata, Action<bool> action = null, string playerId = "", bool isBot = false)
    {
        if (isPlayByDummyData)
        {
            action?.Invoke(true);
            return;
        }

        if (betDetails.Exists(x => x.index == index))
        {
            BetDetails bet = betDetails.Find(x => x.index == index);
            bet.action = action;
            if (bet.IsAbleToCancel == "false")
            {
                bet.Status = BetProcess.Failed;
                return;
            }
            bet.Status = BetProcess.Processing;
            //var data = new
            //{
            //    type = "FinilizeBet",
            //    Id = bet.betID,
            //    MetaData = metadata,
            //    Game_user_Id = userDetails.Id,
            //    Game_Id = userDetails.game_Id,
            //};
#if UNITY_WEBGL
            DebugHelper.Log("Finalize Bet Data");
            FinilizePlayerBet("FinilizeBet", bet.betID, JsonUtility.ToJson(metadata), playerId == "" ? userDetails.Id : playerId, userDetails.game_Id, isBot ? 1 : 0);
#endif
        }

    }

    public void CreateMatch(string lobbyName, Action<CreateMatchResponse, int, string> action, string gamename, string operatorname, string playerId, bool isBlockAPI, string server, string environment, string Balance)
    {
        CreateMatchResponse matchResponse = new CreateMatchResponse();
        string serverIPAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList.First(f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString();
        List<KeyValuePojo> param = new List<KeyValuePojo>();
        param.Add(new KeyValuePojo { keyId = "created_by", value = serverIPAddress });
        param.Add(new KeyValuePojo { keyId = "game_name", value = gamename == "" ? userDetails.game_Id.Split()[1] : gamename });
        param.Add(new KeyValuePojo { keyId = "operator", value = operatorname == "" ? userDetails.game_Id.Split()[0] : operatorname });
        param.Add(new KeyValuePojo { keyId = "request_type", value = "CreateMatch" });
        param.Add(new KeyValuePojo { keyId = "balance", value = Balance });
        param.Add(new KeyValuePojo { keyId = "lobby_name", value = lobbyName });

        Debug.Log(" Check The CallType In the Start Game======>4");
        WebApiManager.Instance.GetNetWorkCall(NetworkCallType.POST_METHOD_USING_JSONDATA, StaticStrings.GetLambdaUrl(environment), param, (success, error, body) =>
        {
            if (success)
            {
                DebugHelper.Log(server + " ::: match created " + body);
                JObject jsonObject = JObject.Parse(body);
                if ((int)(jsonObject["code"]) == 200)
                {
                    matchResponse = JsonUtility.FromJson<CreateMatchResponse>(jsonObject["data"].ToString());
                    matchResponse.status = true;
                    action.Invoke(matchResponse, (int)(jsonObject["code"]), jsonObject["message"].ToString());

                }
                else
                {
                    matchResponse.status = false;
                    action.Invoke(matchResponse, (int)(jsonObject["code"]), jsonObject["message"].ToString());

                }

            }
            else
            {
                matchResponse.status = false;
                action.Invoke(matchResponse, 0, "");
            }
        });
    }


    public void ApiCallBackDebugger(string data)
    {
        //DebugHelper.Log("API Call Back Debug :- " + data);

        //byte[] bytesToEncode = Encoding.UTF8.GetBytes(url);
        byte[] bytesToEncode = Convert.FromBase64String(data);

        string base64EncodedString = Encoding.UTF8.GetString(bytesToEncode);

        DebugHelper.Log(base64EncodedString + "inpuT");

        JObject OBJ = JObject.Parse(base64EncodedString);
        string url = OBJ["url"].ToString();
        int code = int.Parse(OBJ["status"].ToString());
        string body = OBJ["body"].ToString();
        string error = OBJ["error"].ToString();
        //DebugHelper.Log("===========================");
        //DebugHelper.Log(url);
        //DebugHelper.Log(body);
        //DebugHelper.Log(code);
        //DebugHelper.Log(error);
        //DebugHelper.Log("===========================");
        APICallBack(url, code, body, error);
    }

    public void APICallBack(string url, int code, string body, string error)
    {
        DebugHelper.Log($"API_ Response :-  URL{url} -- Code {code} -- Body {body} -- Error {error}");
        foreach (var item in apiRequestList)
        {
            if (item.url == url)
            {
                if (code == 200)
                {
                    DebugHelper.Log("api response :::::: true");
                    item.callback(true, error, body);
#if UNITY_WEBGL
                    //if (!userDetails.isBlockApiConnection)
                    //    GetUpdatedBalance();
#endif
                }
                else
                {
                    DebugHelper.Log("api response :::::: false");
                    item.callback(false, error, body);
                }
            }
        }
        apiRequestList.RemoveAll(x => x.url == url);
    }

    public async void CheckAPICallBack(string url)
    {

        await UniTask.Delay(7000);
        DebugHelper.Log($"API_ Response :-  URL{url} ");
        foreach (var item in apiRequestList)
        {
            if (item.url == url)
            {
                item.callback(false, "timeout", "timeout");
            }
        }
        apiRequestList.RemoveAll(x => x.url == url);
    }



    public void AddMatchLog(string matchToken, string action, string metadata, string PlayerId = "")
    {
        if (userDetails.isBlockApiConnection)
        {
            return;
        }
        List<KeyValuePojo> param = new List<KeyValuePojo>();
        param.Add(new KeyValuePojo { keyId = "Token", value = matchToken });
        param.Add(new KeyValuePojo { keyId = "Action", value = action });
        param.Add(new KeyValuePojo { keyId = "Metadata", value = metadata });
        param.Add(new KeyValuePojo { keyId = "PlayerId", value = PlayerId == "" ? userDetails.Id : PlayerId });
        param.Add(new KeyValuePojo { keyId = "requestType", value = "AddMatchLog" });


    }

    public void AddUnclaimAmount(string matchToken, double amount)
    {
        if (APIController.instance.userDetails.isBlockApiConnection)
        {
            return;
        }
        List<KeyValuePojo> param = new List<KeyValuePojo>();
        param.Add(new KeyValuePojo { keyId = "Id", value = matchToken });
        param.Add(new KeyValuePojo { keyId = "unclaim_amount", value = amount.ToString() });
        param.Add(new KeyValuePojo { keyId = "requestType", value = "AddUnclaimAmount" });
        Debug.Log(" Check The CallType In the Start Game======>5");
        WebApiManager.Instance.GetNetWorkCall(NetworkCallType.GET_METHOD, backendAPIURL.LootrixMatchAPI, param, (success, error, body) =>
        {
            if (success)
            {
                JObject jsonObject = JObject.Parse(body);
                if ((int)(jsonObject["code"]) == 200)
                {

                }
                else
                {
                }
            }
        });
    }

    public void AddPlayers(string matchToken, List<string> players, string Operatorname, string environment)
    {
        if (APIController.instance.userDetails.isBlockApiConnection)
        {
            return;
        }
        List<KeyValuePojo> param = new List<KeyValuePojo>();
        param.Add(new KeyValuePojo { keyId = "id", value = matchToken });
        param.Add(new KeyValuePojo { keyId = "players", value = JsonConvert.SerializeObject(players) });
        param.Add(new KeyValuePojo { keyId = "player", value = JsonConvert.SerializeObject(players) });
        param.Add(new KeyValuePojo { keyId = "request_type", value = "AddPlayers" });
        param.Add(new KeyValuePojo { keyId = "operator", value = Operatorname });
        Debug.Log(" Check The CallType In the Start Game======>6");
        WebApiManager.Instance.GetNetWorkCall(NetworkCallType.POST_METHOD_USING_JSONDATA, StaticStrings.GetLambdaUrl(environment), param, (success, error, body) =>
        {
            if (success)
            {
                JObject jsonObject = JObject.Parse(body);
                if ((int)(jsonObject["code"]) == 200)
                {

                }
                else
                {
                }
            }
        });
    }


    public void ExecuteAPI(ApiRequest api, int timeout = 0, bool check = false)
    {
        if (!check)
        {
            DebugHelper.Log("ExecuteAPI =====> " + api);
        }

        Debug.Log("Check The CallType In the Start Game====>7"+ api.callType);
        WebApiManager.Instance.GetNetWorkCall(api.callType, api.url, api.param, (success, error, body) =>
        {

            if (!check)
            {
                DebugHelper.Log($"<color=orange>Success is set to {success}, error is set to {error} and body is set to {body}\nURL is : {api.url}</color>");
            }
            if (success)
            {
                if (!check)
                {
                    DebugHelper.Log($"<color=orange>API sent to success</color>");
                }
                api.action?.Invoke(success, error, body);
            }
            else
            {
                if (timeout >= 3)
                {
                    api.action?.Invoke(success, error, body);
                    if (!check)
                    {
                        DebugHelper.Log($"<color=orange>API run failed with timeout {timeout}</color>");
                    }
                }
                else
                {
                    if (!check)
                    {
                        DebugHelper.Log($"<color=orange>API recalled with timeout set to {timeout}</color>");
                    }
                    ExecuteAPI(api, ++timeout);
                }
            }
        }, 15);
    }

    //string LootrixMatchAPIPath = "https://aezmien2odhv7hkrbmdzy2csoe0dagnw.lambda-url.ap-south-1.on.aws/";
    //string RumbleBetsAPIPath = "https://rumblebets.utwebapps.com:7350/v2/rpc/";
    //string LootrixAPIPath = "https://xpxpmhpjldqvjulexwx34z7jca0kdxzf.lambda-url.ap-south-1.on.aws/";
    bool isRunApi = false;

    /// API url Live Create Match, Add Player, create and join game
    string LiveLootrixAPIPathCreateAddJoinGame = "https://56ebdif5s4aqltb74xhkvnnrai0sxaey.lambda-url.ap-south-1.on.aws/";

    /// API url Dev Create Match, Add Player, create and join game
    string DevLootrixAPIPathCreateAddJoinGame = "https://b465kezbry2ssew2w6w56x3jhi0ennyi.lambda-url.ap-south-1.on.aws/";

    /// API url Live Init Bet, add Bet ,Winning Bet
    string LiveLootrixAPIPathInitAddWinningBet = "https://xpxpmhpjldqvjulexwx34z7jca0kdxzf.lambda-url.ap-south-1.on.aws/";

    /// API url Dev Init Bet, add Bet ,Winning Bet
    string DevLootrixAPIPathInitAddWinningBet = "https://vbklyx2pq3nh6xf3vr55vmrwem0ldawq.lambda-url.ap-south-1.on.aws/";

    /// API url Live getserver 
    string LiveLootrixAPIPathgetserver = "https://ahw3tps3cf6a7xjmykrdlaq5ta0xggdq.lambda-url.ap-south-1.on.aws/";

    /// API url Dev getserver
    string DevLootrixAPIPathgetserver = "https://4stgpkclxjbxwkx6xyqkgguwj40etlbm.lambda-url.ap-south-1.on.aws/";




    //public void GetABotAPI(List<string> botId, Action<BotDetails> action, string domainURL)
    //{
    //    List<KeyValuePojo> param = new List<KeyValuePojo>();
    //    param.Add(new KeyValuePojo { keyId = "player", value = JsonConvert.SerializeObject(botId) });
    //    param.Add(new KeyValuePojo { keyId = "request_type", value = "getabot" });
    //    string url = domainURL;
    //    ApiRequest apiRequest = new ApiRequest();
    //    apiRequest.action = (success, error, body) =>
    //    {
    //        if (success)
    //        {
    //            JObject json = JObject.Parse(body);
    //            BotDetails bot = new BotDetails();
    //            bot = JsonUtility.FromJson<BotDetails>(json["data"].ToString());
    //            action?.Invoke(bot);
    //        }
    //    };
    //    apiRequest.url = url;
    //    apiRequest.param = param;
    //    apiRequest.callType = NetworkCallType.POST_METHOD_USING_JSONDATA;
    //    ExecuteAPI(apiRequest);
    //}


    public void GetABotAPI(List<string> botId, Action<BotDetails> action, string domainURL, string operatorName)
    {
        List<KeyValuePojo> param = new List<KeyValuePojo>();
        param.Add(new KeyValuePojo { keyId = "player", value = JsonConvert.SerializeObject(botId) });
        param.Add(new KeyValuePojo { keyId = "request_type", value = "getabot" });
        param.Add(new KeyValuePojo { keyId = "operator", value = operatorName });
        string url = domainURL;
        ApiRequest apiRequest = new ApiRequest();
        apiRequest.action = (success, error, body) =>
        {
            if (success)
            {
                JObject json = JObject.Parse(body);
                BotDetails bot = new BotDetails();
                bot = JsonUtility.FromJson<BotDetails>(json["data"].ToString());
                action?.Invoke(bot);
            }
        };
        apiRequest.url = url;
        apiRequest.param = param;
        apiRequest.callType = NetworkCallType.POST_METHOD_USING_JSONDATA;
        ExecuteAPI(apiRequest);
    }


    #region StartAuthentication

    public AuthenticationData authentication = new AuthenticationData();
    public async void StartAuthentication(string data)
    {
        authentication = !string.IsNullOrWhiteSpace(data) ? JsonUtility.FromJson<AuthenticationData>(data) : new AuthenticationData();
        /*if (string.IsNullOrWhiteSpace(authentication.environment))
        {
            IsLiveGame = true;
        }
        else
        {
            IsLiveGame = authentication.environment.ToLower() == "live";
        }*/
        // NetworkManager.singleton.networkAddress = IsLiveGame ? "gameserver.utwebapps.com" : "dev.test.gameservers.utwebapps.com";
        GameController.Instance.CurrentServerHost = NetworkManager.singleton.networkAddress;
       
        GamePlayUI.instance.AssignCurrency(authentication.currency_type.ToString(), "0.00");

#if UNITY_WEBGL
        CheckMirrorGameAvaliblity(GameController.Instance.CurrentServerHost, (7777).ToString(), async (success, message) =>
        {
            DebugHelper.Log($"CheckMirrorGameAvaliblity => Success : {success} ....... Message : {message}");
            if (!success)
            {
                UIController.Instance.ConnectionIssue.SetActive(true);
                return;
            }
            else
            {
                InitPlayerManager();

                while (!NetworkClient.isConnected)
                {
                    await UniTask.Delay(50);
                }

                while (!PlayerManager.localPlayer)
                {
                    await UniTask.Delay(50);
                }
                PlayerManager.localPlayer.StartGameAuthentication(data);
            }
        });

#endif
    }

    int Timer;

    public async void InitPlayerManager()
    {
        DebugHelper.Log("==========> " + "InitializePlayerManager ===========>");
        if (!NetworkClient.isConnected && !NetworkClient.isConnecting)
        {
            DebugHelper.Log("Starting Client");
            NetworkManager.singleton.StartClient();
        }
        else
        {
            DebugHelper.LogWarning("Already connected or connecting!");
        }
        DebugHelper.Log("try to Connect Server");
        while (!NetworkClient.isConnected)
        {
            if (!NetworkClient.isConnecting)
                NetworkManager.singleton.StartClient();
            Timer++;
            DebugHelper.Log("==========> " + Timer);
            if (Timer > 20)
            {
                Timer = 0;
                UIController.Instance.ConnectionIssue.SetActive(true);
                break;

            }
            await UniTask.Delay(50);
        }
        DebugHelper.Log("Connected to Server");
        if (!PlayerManager.localPlayer)
            NetworkClient.AddPlayer();
        while (!PlayerManager.localPlayer)
        {
            await UniTask.Delay(50);
        }
        DebugHelper.Log($"Added Local Player{PlayerManager.localPlayer}");
        while (!NetworkClient.isConnected)
            await UniTask.Delay(50);
    }



    public void StartAuthenticationClientSideCall(string body, string error, bool success)
    {

        if (!success)
        {
            DebugHelper.Log("StartAuthenticationFailed in Server Side Check Server for Further Details ?  " + body);
            return;
        }

        if (string.IsNullOrEmpty(body))
        {
            DebugHelper.Log("Body ===========> IsNull ?  " + body);
        }
        else
        {
            DebugHelper.Log("Body ===========>   " + body);
        }

        JObject apiResponse = JObject.Parse(body);
        if ((int)apiResponse["code"] == 200)
        {
            try
            {
                if ((int)apiResponse["code"] == 200)
                {

                    JObject data = JObject.Parse(apiResponse["data"].ToString());
                    JObject output = JObject.Parse(apiResponse["output"].ToString());
                    authentication.session_token = (string)output["session_token"];
                    authentication.name = (string)data["username"];
                    authentication.balance = (float)data["balance"];
                    DebugHelper.Log("authentication  is : " + JsonUtility.ToJson(authentication));
                    userDetails.Id = authentication.Id;
                    userDetails.game_Id = authentication.operatorname + "_" + authentication.gamename;
                    userDetails.isBlockApiConnection = authentication.operatorname == "demo";
                    userDetails.name = authentication.name;
                    userDetails.session_token = authentication.session_token;
                    userDetails.token = authentication.token;
                    userDetails.UserDevice = authentication.platform;
                    userDetails.operatorDomainUrl = authentication.operatorDomainUrl;
                    userDetails.currency_type = authentication.currency_type;
                    userDetails.gameId = (string)output["gameid"];
                    userDetails.hasBot = true;
                    userDetails.balance = authentication.balance;
                    userDetails.maxWin = 0;
                    userDetails.isWin = true;
                    userDetails.bootAmount = 10;
                    IsBotInGame = userDetails.hasBot;
                    if (authentication.potLimit != 0)
                    {
                        userDetails.potLimit = authentication.potLimit;
                        userDetails.challLimit = authentication.challLimit;
                        userDetails.bootAmount = authentication.bootAmount;
                    }
                    else
                    {
                        userDetails.potLimit = 1000;
                        userDetails.challLimit = 320;
                        userDetails.bootAmount = 10;
                    }
                    userDetails.commission = authentication.game_data.comission;
                    authentication.comission = authentication.game_data.comission;
                    if (string.IsNullOrWhiteSpace(userDetails.gameId))
                        userDetails.gameId = "ecd5c5ce-e0a1-4732-82a0-099ec7d180be";
                    DebugHelper.Log("Check this once !!!!!!!!!!!!!" + JsonUtility.ToJson(userDetails));
                    GameController.Instance.CurrentPlayerData.token = authentication.token;
                    GameController.Instance.CurrentPlayerData.session_token = authentication.session_token;
                    GameController.Instance.CurrentPlayerData.currency_type = authentication.currency_type;
                    GameController.Instance.CurrentPlayerData.gamename = authentication.gamename;
                    GameController.Instance.CurrentPlayerData.operatorname = authentication.operatorname;
                    GameController.Instance.CurrentPlayerData.operatorDomainUrl = authentication.operatorDomainUrl;
                    GameController.Instance.CurrentPlayerData.platform = authentication.platform;
                    GameController.Instance.CurrentPlayerData.comission = authentication.comission;
                    GameController.Instance.CurrentPlayerData.environment = authentication.environment;
                    GameController.Instance.CurrentPlayerData.balance = authentication.balance;
                    // GamePlayUI.instance.AssignCurrency(userDetails.currency_type.ToString(), authentication.balance.ToString());

                    OnUserDetailsUpdate?.Invoke();
                    OnUserBalanceUpdate?.Invoke();


                    //                    GetAutoscaleServerDetail(true, () =>
                    //                    {

                    //#if UNITY_WEBGL && !UNITY_EDITOR
                    //                  //    ValidateSession();
                    //               //     CheckOnline();
                    //#endif
                    //                    }, 3);
                }
                else
                {
#if UNITY_WEBGL

                    DisconnectGame((string)apiResponse["message"]);
#else
                    DebugHelper.Log((string)apiResponse["message"]);
#endif
                }
            }
            catch (Exception ex)
            {
                DebugHelper.Log("check 1" + ex.Message);
#if UNITY_WEBGL

                DisconnectGame("Illigal Access");
#else
                DebugHelper.Log("Illigal Access");
#endif
            }
        }
        else
        {
            DebugHelper.Log("check 1.1" + (int)apiResponse["code"]);
        }
    }



    #endregion

    public void UpdateBalanceTrigger()
    {
        DebugHelper.Log("UpdateBalanceTrigger ==>  ");
        isClickDeopsit = false;
        GetUpdatedBalance();
    }

    #region GetUpdatedBalance

    public async void GetUpdatedBalance()
    {


        if (!NetworkClient.isConnected)
        {
            CheckMirrorGameAvaliblity(GameController.Instance.CurrentServerHost, (7777).ToString(), async (success, message) =>
            {
                DebugHelper.Log($"CheckMirrorGameAvaliblity => Success : {success} ....... Message : {message}");
                if (!success)
                {
                    UIController.Instance.ConnectionIssue.SetActive(true);
                    return;
                }
                else
                {
                    InitPlayerManager();
                }
            });
        }

        while (!NetworkClient.isConnected)
        {
            await UniTask.Delay(100);
        }

        while (!PlayerManager.localPlayer)
        {
            await UniTask.Delay(100);
        }

        PlayerManager.localPlayer.GetUpdatedBalance(APIController.instance.authentication.Id, APIController.instance.authentication.session_token, APIController.instance.authentication.currency_type, APIController.instance.authentication.token, APIController.instance.authentication.gamename, APIController.instance.authentication.operatorname, APIController.instance.authentication.environment);

    }


    public void UpdateAmount(string Amount)
    {

#if UNITY_WEBGL && !UNITY_EDITOR
        UpdateBalance();
#endif
        authentication.balance = float.Parse(Amount);
        userDetails.balance = authentication.balance;
        DebugHelper.Log("UpdateAmount ==============> " + authentication.balance);
        OnUserBalanceUpdate?.Invoke();
    }

    public void GetUpdatedBalanceClientSideCall(string body, string error, bool success)
    {
        DebugHelper.Log("GetUpdateBalance Body ==============> " + body);
        ApiResponse apiResponse = JsonUtility.FromJson<ApiResponse>(body);
        DebugHelper.Log("authentication response is : " + body);
        DebugHelper.Log("authentication response is : " + apiResponse.code);
        if (apiResponse.code == 200)
        {
            try
            {
                JObject json = JObject.Parse(apiResponse.data);
#if UNITY_WEBGL
                UpdateBalance();
#endif
                authentication.balance = (float)json["balance"];
                userDetails.balance = authentication.balance;
                DebugHelper.Log("GetUpdateBalance Body Success ==============> " + authentication.balance);
                OnUserBalanceUpdate?.Invoke();


            }
            catch (Exception ex)
            {
#if UNITY_WEBGL
                DebugHelper.Log("check 1" + ex.Message);
                DisconnectGame("Illigal Access");
#endif
            }
        }
        else
        {
            DebugHelper.Log("check 1.1" + apiResponse.code);
#if UNITY_WEBGL
            DisconnectGame(apiResponse.message);
#endif
        }
        DebugHelper.Log("check 2");
    }


    #endregion



    /// <summary>
    /// /
    /// </summary>
    /// <param name="betId"></param>
    /// <param name="win_amount_with_comission"></param>
    /// <param name="spend_amount"></param>
    /// <param name="pot_amount"></param>
    /// <param name="metadata"></param>
    /// <param name="action"></param>
    /// <param name="playerId"></param>
    /// <param name="isBot"></param>
    /// <param name="isWinner"></param>
    /// <param name="gameName"></param> <param name="Operator"></param> game name must be APIController.instance.userDetails.game_Id. Get that from client side and stored that into server side. (or) manualy give that in serverside. ex : APIController.instance.userDetails.game_Id = rumbblebets_aviator
    /// <param name="gameId"></param> Get that from client side and stored that into server side. APIController.instance.userDetails.gameId


    public async void WinningsBetMultiplayerAPI(int betIndex, string betId, double win_amount_with_comission, double spend_amount, double pot_amount, TransactionMetaData metadata, Action<bool, int, string> action, string playerId, bool isBot, bool isWinner, string gameName, string operatorName, string gameId, float commission, string matchToken, string domainUrl, string session_token, string currency, string platform, string userToken, string environment, string Balance)
    {
        await UniTask.Delay(3000);
        string AmountToset = win_amount_with_comission.ToString();     //win_amount_with_comission % 1 == 0 ? win_amount_with_comission.ToString("F0") : win_amount_with_comission.ToString("F2");
        DebugHelper.Log($"WinningsBetMultiplayerAPI  WinAmount : {AmountToset}, playerId: {playerId}, matchToken: {matchToken}");
        BetRequest request = betRequest.Find(x => x.betId == betIndex && x.PlayerId == playerId && x.MatchToken.Equals(matchToken));
        DebugHelper.Log($"Request data is {JsonUtility.ToJson(request)}");
        while (request.BetId != betId)
        {
            await UniTask.Delay(200);
        }

        DebugHelper.Log($"<color=orange>WinningsBetMultiplayerAPI called with comission set to {commission}</color>");
        if (!isWinner)
        {
            commission = 0.0f;
        }


        List<KeyValuePojo> param1 = new List<KeyValuePojo>
        {
            new KeyValuePojo { keyId = "user_id", value = playerId },
            new KeyValuePojo { keyId = "user_token", value = userToken },
            new KeyValuePojo { keyId = "game_name", value = gameName },
            new KeyValuePojo { keyId = "operator", value = operatorName },
            new KeyValuePojo { keyId = "session_token", value = session_token },
            new KeyValuePojo { keyId = "id", value = betId },
            new KeyValuePojo { keyId = "game_id", value = gameId },
            new KeyValuePojo { keyId = "is_bot", value = isBot ? "1" : "0" },
            new KeyValuePojo { keyId = "win_amount", value = AmountToset.ToString() },
            new KeyValuePojo { keyId = "amount_spend", value = spend_amount.ToString() },
            new KeyValuePojo { keyId = "comission", value = commission.ToString() },
            new KeyValuePojo { keyId = "is_win", value = isWinner ? "1" : "0" },
            new KeyValuePojo { keyId = "request_type", value = "winningBet" },
            new KeyValuePojo { keyId = "currency", value = currency },
            new KeyValuePojo { keyId = "provider", value = gameName+"_Lootrix" },
            new KeyValuePojo { keyId = "action", value = "winningbet" },
            new KeyValuePojo { keyId = "action_id", value = gameName+"_winningbet" },
            new KeyValuePojo { keyId = "url", value = domainUrl+"api/deposit/" },
            new KeyValuePojo { keyId = "balance", value = Balance },
            new KeyValuePojo { keyId = "platform", value = platform },

        };

        string url1 = StaticStrings.GetLambdaUrl(environment);
        ApiRequest apiRequest1 = new ApiRequest();
        DebugHelper.Log($"need to check external api ::::  winning bet request is   ::: player id is {playerId} :: amount {win_amount_with_comission}   ::: bet id {betId}");

        apiRequest1.action = (success, error, body) =>
        {
            DebugHelper.Log($"need to check external api ::::  winning bet response is   ::: player id is {playerId} :: amount {win_amount_with_comission}  ::: bet id {betId}  :::: response is {body}");
            if (success)
            {
                ApiResponse response = JsonUtility.FromJson<ApiResponse>(body);
                action?.Invoke(response != null && response.code == 200, response.code, response.message);
                ClearBetResponse(request.BetId);
            }
            else
            {
                action?.Invoke(false, 0, "");
            }
        };
        apiRequest1.url = url1;
        apiRequest1.param = param1;
        apiRequest1.callType = NetworkCallType.POST_METHOD_USING_JSONDATA;
        ExecuteAPI(apiRequest1);



    }

    public async void CancelMatchAPI(Action<bool, int, string> action, string gameName, string operatorName, string gameId, string matchToken, string session_token, string currency, string environment)
    {

        List<KeyValuePojo> param1 = new List<KeyValuePojo>();
        param1.Add(new KeyValuePojo { keyId = "match_token", value = matchToken });
        param1.Add(new KeyValuePojo { keyId = "game_name", value = gameName });
        param1.Add(new KeyValuePojo { keyId = "operator", value = operatorName });
        param1.Add(new KeyValuePojo { keyId = "game_id", value = gameId });
        param1.Add(new KeyValuePojo { keyId = "request_type", value = "cancelMatch" });
        param1.Add(new KeyValuePojo { keyId = "currency", value = currency });

        Debug.Log("CancelMatchAPI Params =====>  " + JsonConvert.SerializeObject(param1));

        string url1 = StaticStrings.GetLambdaUrl(environment);

        ApiRequest apiRequest = new ApiRequest();
        apiRequest.action = (success, error, body) =>
        {
            Debug.Log("CancelMatchAPI " + body);
            ApiResponse response = JsonUtility.FromJson<ApiResponse>(body);
            if (success)
            {
                Debug.Log("CancelMatchAPI Sucess" + response.code + " " + response.message);
                action?.Invoke((response != null && response.code == 200), response.code, response.message);
            }
            else
            {
                Debug.Log("CancelMatchAPI Failure" + response.code + " " + response.message);
                action?.Invoke(false, response.code, response.message);
            }
        };
        apiRequest.url = url1;
        apiRequest.param = param1;
        apiRequest.callType = NetworkCallType.POST_METHOD_USING_JSONDATA;
        ExecuteAPI(apiRequest);
    }



    public async void CancelBetMultiplayerAPI(Action<bool, int, string> action, string playerId, string gameName, string operatorName, string gameId, string matchToken, string session_token, string currency, string environment)
    {

        List<KeyValuePojo> param1 = new List<KeyValuePojo>();
        param1.Add(new KeyValuePojo { keyId = "match_token", value = matchToken });
        param1.Add(new KeyValuePojo { keyId = "game_name", value = gameName });
        param1.Add(new KeyValuePojo { keyId = "operator", value = operatorName });
        param1.Add(new KeyValuePojo { keyId = "game_id", value = gameId });
        param1.Add(new KeyValuePojo { keyId = "session_token", value = session_token });
        param1.Add(new KeyValuePojo { keyId = "user_id", value = playerId });
        param1.Add(new KeyValuePojo { keyId = "request_type", value = "cancelBets" });
        param1.Add(new KeyValuePojo { keyId = "currency", value = currency });

        Debug.Log("CancelBetMultiplayerAPI Params =====>  " + JsonConvert.SerializeObject(param1));

        string url1 = StaticStrings.GetLambdaUrl(environment);

        ApiRequest apiRequest = new ApiRequest();
        apiRequest.action = (success, error, body) =>
        {
            Debug.Log("CancelBetMultiplayerAPI " + body);
            ApiResponse response = JsonUtility.FromJson<ApiResponse>(body);
            if (success)
            {
                Debug.Log("CancelBetMultiplayerAPI Sucess" + response.code + " " + response.message);
                action?.Invoke((response != null && response.code == 200), response.code, response.message);
            }
            else
            {
                Debug.Log("CancelBetMultiplayerAPI Failure" + response.code + " " + response.message);
                action?.Invoke(false, response.code, response.message);
            }
        };
        apiRequest.url = url1;
        apiRequest.param = param1;
        apiRequest.callType = NetworkCallType.POST_METHOD_USING_JSONDATA;
        ExecuteAPI(apiRequest);
    }

    public async void CancelSingleBetMultiplayerAPI(int betIndex, string betId, double betamount, TransactionMetaData metadata, Action<bool, int, string> action, string playerId, bool isBot, bool isWinner, string gameName, string operatorName, string gameId, string matchToken, string domainURL, string session_token, string currency, string platform, string userToken, string environment, string Balance)
    {
        BetRequest request = betRequest.Find(x => x.betId == betIndex && x.PlayerId == playerId && x.MatchToken.Equals(matchToken));
        while (request.BetId != betId)
        {
            await UniTask.Delay(200);
        }
        Debug.Log($"CancelBetMultiplayerAPI : {betIndex}, BetId : {betId}, PlayerID : {playerId},");

        List<KeyValuePojo> param = new List<KeyValuePojo>
 {
     new KeyValuePojo { keyId = "user_id", value = playerId },
     new KeyValuePojo { keyId = "user_token", value = userToken },
     new KeyValuePojo { keyId = "game_name", value = gameName },
     new KeyValuePojo { keyId = "operator", value = operatorName },
     new KeyValuePojo { keyId = "session_token", value = session_token },
     new KeyValuePojo { keyId = "id", value = betId },
     new KeyValuePojo { keyId = "game_id", value = gameId },
     new KeyValuePojo { keyId = "is_bot", value = isBot ? "1" : "0" },
     new KeyValuePojo { keyId = "bet_amount", value = betamount.ToString() },
     new KeyValuePojo { keyId = "request_type", value = "cancelBet" },
     new KeyValuePojo { keyId = "currency", value = currency },
     new KeyValuePojo { keyId = "provider", value = gameName+"_lootrix" },
     new KeyValuePojo { keyId = "action", value = "cancelBet" },
     new KeyValuePojo { keyId = "action_id", value = gameName+"_cancelBet" },
     new KeyValuePojo { keyId = "balance", value = Balance },
     new KeyValuePojo { keyId = "platform", value = platform },

 };

        string url1 = StaticStrings.GetLambdaUrl(environment);

        ApiRequest apiRequest = new ApiRequest();
        apiRequest.action = (success, error, body) =>
        {
            Debug.Log("Cancel body " + body);
            ApiResponse response = JsonUtility.FromJson<ApiResponse>(body);
            if (success)
            {
                action?.Invoke((response != null && response.code == 200), response.code, response.message);
            }
            else
            {
                action?.Invoke(false, response.code, response.message);
            }
        };
        apiRequest.url = url1;
        apiRequest.param = param;
        apiRequest.callType = NetworkCallType.POST_METHOD_USING_JSONDATA;
        ExecuteAPI(apiRequest);
    }




    public async void AddBetMultiplayerAPI(int index, string BetId, TransactionMetaData metadata, double amount, Action<bool, int, string> action, string playerId, bool isBot, string gameName, string operatorName, string gameId, string matchToken, string domainURL, string session_token, string currency, string platform, string userToken, string enviroment, string Balance)
    {
        DebugHelper.Log("Server side check ============> " + session_token + " " + userToken);
        string AmountToset = amount % 1 == 0 ? amount.ToString("F0") : amount.ToString("F2");
        DebugHelper.Log($"AddBetMultiplayerAPI  Betamount : {AmountToset}, playerId: {playerId}, matchToken: {matchToken}");
        BetRequest request = betRequest.Find(x => x.betId == index && x.PlayerId == playerId && x.MatchToken.Equals(matchToken));
        DebugHelper.Log($"AddBetMultiplayerAPI Y==================> " + BetId + " *************** " + request.BetId);
        while (request.BetId != BetId)
        {
            await UniTask.Delay(200);
        }
        DebugHelper.Log($"AddBetMultiplayerAPI X==================> ");
        List<KeyValuePojo> param1 = new List<KeyValuePojo>()
        {

             new KeyValuePojo { keyId = "user_id", value = playerId },
             new KeyValuePojo { keyId = "user_token", value = userToken },
             new KeyValuePojo { keyId = "game_name", value = gameName },
             new KeyValuePojo { keyId = "operator", value = operatorName },
             new KeyValuePojo { keyId = "session_token", value = session_token },
             new KeyValuePojo { keyId = "game_id", value = gameId },
             new KeyValuePojo { keyId = "is_bot", value = isBot ? "1" : "0" },
             new KeyValuePojo { keyId = "id", value = BetId },
             new KeyValuePojo { keyId = "bet_amount", value = AmountToset },
             new KeyValuePojo { keyId = "request_type", value = "addBet" },
             new KeyValuePojo { keyId = "currency", value = currency },
             new KeyValuePojo { keyId = "provider", value = gameName + "_Lootrix" },
             new KeyValuePojo { keyId = "action", value = "addBet" },
             new KeyValuePojo { keyId = "action_id", value = gameName + "_addBet" },
             new KeyValuePojo { keyId = "platform", value = platform },
             new KeyValuePojo { keyId = "balance", value = Balance },
             new KeyValuePojo { keyId = "meta_data", value = JsonUtility.ToJson(metadata) },
    };



        DebugHelper.Log($"AddBetMultiplayerAPI ==================> 3" + JsonConvert.SerializeObject(param1));
        string url1 = StaticStrings.GetLambdaUrl(enviroment);
        ApiRequest apiRequest1 = new ApiRequest();
        DebugHelper.Log($"AddBetMultiplayerAPI ==================> 4");
        DebugHelper.Log($"need to check external api ::::  add bet request is   ::: player id is {playerId} :: amount {AmountToset}  ::: bet id {BetId} ::: player");

        apiRequest1.action = (success, error, body) =>
        {
            DebugHelper.Log($"need to check external api ::::  add bet response is   ::: player id is {playerId} :: amount {amount}    ::: bet id {BetId}  :::: response is {body}");

            if (success)
            {
                ApiResponse response = JsonUtility.FromJson<ApiResponse>(body);
                DebugHelper.Log("ADD Bet Success Case  Check ==========>" + body + " ===== " + response.message + " ====== " + response.code);
                action?.Invoke(response != null && response.code == 200, response.code, response.message);
            }
            else
            {

                ApiResponse response = JsonUtility.FromJson<ApiResponse>(body);
                DebugHelper.Log("ADD Bet Success Case  Check Failure  ==========>" + body + " ===== " + response.message + " ====== " + response.code);
                action?.Invoke(false, 0, response.message);
            }
        };
        apiRequest1.url = url1;
        apiRequest1.param = param1;
        apiRequest1.callType = NetworkCallType.POST_METHOD_USING_JSONDATA;
        ExecuteAPI(apiRequest1);

    }


    private async void ClearBetResponse(string betID)
    {
        await UniTask.Delay(100);
        try
        {
            if (betRequest != null && betRequest.Count > 0 && betRequest.Exists(x => x.BetId.Equals(betID)))
            {
                BetRequest request = betRequest.Find(x => x.BetId.Equals(betID));
                DebugHelper.Log($"BetID is {request.BetId}\n Bet Index is {request.betId}\nPlayer Id is {request.PlayerId}\nMatch Token is {request.MatchToken}");

                try
                {
                    if (request != null)
                        betRequest.RemoveAll(x => x.BetId == (betID));
                }
                catch (Exception e)
                {
                    DebugHelper.Log($"Catch ======>  : {e}");
                    if (request != null)
                        betRequest.Remove(request);
                }
            }
        }
        catch (Exception e)
        {
            DebugHelper.Log($"Error: {e}");
        }
    }

    public int InitBetMultiplayerAPI(int index, double amount, TransactionMetaData metadata, bool isAbleToCancel, Action<bool, string, int> action, string playerId, string playerName, bool isBot, Action<string> betIdAction, string gameName, string operatorName, string gameID, string matchToken, string domainURL, string session_token, string currency, string platform, string userToken, string environment, string Balance)
    {
        DebugHelper.Log($"<color=orange>Initializing bet for player {playerId}, index is {index}</color> , check session  {session_token} ,  currency {currency} , platform {platform}");
        string AmountToset = amount % 1 == 0 ? amount.ToString("F0") : amount.ToString("F2");
        BetRequest bet = new BetRequest();
        bet.MatchToken = matchToken;
        bet.PlayerId = playerId;
        bet.betId = index;

        betRequest.Add(bet);
        DebugHelper.Log("Check this in Init Bet ==============> " + matchToken + " ========= " + playerId + " =================    " + index + " ================= amount " + AmountToset);

        List<KeyValuePojo> param1 = new List<KeyValuePojo>
             {
                 new KeyValuePojo { keyId = "game_name", value = gameName },
                 new KeyValuePojo { keyId = "user_id", value = playerId },
                 new KeyValuePojo { keyId = "user_token", value = userToken },
                 new KeyValuePojo { keyId = "game_id", value = gameID},
                 new KeyValuePojo { keyId = "operator", value = operatorName },
                 new KeyValuePojo { keyId = "player_name", value = playerName },
                 new KeyValuePojo { keyId = "session_token", value = session_token },
                 new KeyValuePojo { keyId = "is_bot", value = isBot ? "1" : "0" },
                 new KeyValuePojo { keyId = "bet_amount", value = AmountToset },
                 new KeyValuePojo { keyId = "is_able_to_cancel", value = isAbleToCancel.ToString() },
                 new KeyValuePojo { keyId = "index", value = index.ToString() },
                 new KeyValuePojo { keyId = "request_type", value = "initbet" },
                 new KeyValuePojo { keyId = "match_token", value = matchToken },
                 new KeyValuePojo { keyId = "balance", value = Balance },
                 new KeyValuePojo { keyId = "meta_data", value = JsonUtility.ToJson(metadata) }
        };

        DebugHelper.Log("InitBet Params ==============> " + JsonConvert.SerializeObject(param1));

        GameWinningStatus _winningStatus;
        string url1 = StaticStrings.GetLambdaUrl(environment);
        DebugHelper.Log("InitBet Params ==============> " + url1 + "    ::name is " + playerName);

        ApiRequest apiRequest1 = new ApiRequest();
        DebugHelper.Log($"need to check external api ::::  init bet request is   ::: player id is {playerId}  :: name is {playerName}  :: amount {amount}");
        apiRequest1.action = (success, error, body) =>
        {
            if (success)
            {
                ApiResponse response = JsonUtility.FromJson<ApiResponse>(body);
                action?.Invoke(response != null && response.code == 200, response.message, response.code);
                if (response.code == 200)
                {
                    DebugHelper.Log($"<color=aqua>Response message is : {response.data}</color>");
                    _winningStatus = JsonUtility.FromJson<GameWinningStatus>(response.data);
                    DebugHelper.Log("Check this In INIT BET ==============>  " + _winningStatus.Id + " ==== " + playerName + " ==== " + index);
                    DebugHelper.Log($"need to check external api ::::  init bet response is   ::: player id is {playerId}  :: name is {playerName}  :: amount {amount}  :::: response is {_winningStatus.Id}");
                    bet.BetId = _winningStatus.Id;
                    betIdAction.Invoke(_winningStatus.Id);
                }
            }
            else
            {

                action?.Invoke(false, "Cant Reach Server INT", 366);
            }
        };
        apiRequest1.url = url1;
        apiRequest1.param = param1;
        apiRequest1.callType = NetworkCallType.POST_METHOD_USING_JSONDATA;
        ExecuteAPI(apiRequest1);

        return index;
    }



    public void WinningsBetMultiplayer(int betIndex, string betId, double win_amount_with_comission, double spend_amount, double pot_amount, TransactionMetaData metadata, Action<bool> action = null, string playerId = "", bool isBot = false, bool isWinner = true)
    {

        if (isPlayByDummyData)
        {
            if (playerId == "" || playerId == userDetails.Id)
            {
                DebugHelper.Log("Winning Bet Data **********");
                userDetails.balance += win_amount_with_comission;
                OnUserBalanceUpdate.Invoke();
            }
            action?.Invoke(true);
            return;
        }

        if (betDetails.Exists(x => x.betID == betId))
        {
            BetDetails bet = betDetails.Find(x => x.betID == betId);
            bet.Status = BetProcess.Processing;
            bet.action = action;
            //var data = new
            //{
            //    type = "WinningsBet",
            //    Id = bet.betID,
            //    MetaData = metadata,
            //    Game_user_Id = userDetails.Id,
            //    Game_Id = userDetails.game_Id,
            //    Win_amount = amount,
            //    Spend_amount = amount,
            //};

#if UNITY_WEBGL
            DebugHelper.Log("Winning Bet Data");
            MultiplayerWinningsPlayerBet("WinningsBet", betId, JsonUtility.ToJson(metadata), playerId == "" ? userDetails.Id : playerId, userDetails.game_Id, win_amount_with_comission, spend_amount, pot_amount, isBot ? 1 : 0, isWinner ? 1 : 0);
#endif
        }
        else
        {
#if UNITY_WEBGL
            BetDetails bet = new BetDetails();
            id += 1;
            bet.index = id;
            bet.betID = betId;
            bet.Status = BetProcess.Processing;
            betDetails.Add(bet);
            DebugHelper.Log("Winning Bet Data");
            MultiplayerWinningsPlayerBet("WinningsBet", betId, JsonUtility.ToJson(metadata), playerId == "" ? userDetails.Id : playerId, userDetails.game_Id, win_amount_with_comission, spend_amount, pot_amount, isBot ? 1 : 0, isWinner ? 1 : 0);
#endif

        }
    }

    public void WinningsBet(int index, double amount, double spend_amount, TransactionMetaData metadata, Action<bool> action = null, string playerId = "", bool isBot = false)
    {


        if (isPlayByDummyData)
        {
            if (playerId == "" || playerId == userDetails.Id)
            {
                DebugHelper.Log("Winning Bet Data **********");
                userDetails.balance += amount;
                OnUserBalanceUpdate.Invoke();
            }
            action?.Invoke(true);
            return;
        }

        if (betDetails.Exists(x => x.index == index))
        {
            BetDetails bet = betDetails.Find(x => x.index == index);
            bet.Status = BetProcess.Processing;
            bet.action = action;
            //var data = new
            //{
            //    type = "WinningsBet",
            //    Id = bet.betID,
            //    MetaData = metadata,
            //    Game_user_Id = userDetails.Id,
            //    Game_Id = userDetails.game_Id,
            //    Win_amount = amount,
            //    Spend_amount = amount,
            //};
#if UNITY_WEBGL
            DebugHelper.Log("Winning Bet Data");
            WinningsPlayerBet("WinningsBet", bet.betID, JsonUtility.ToJson(metadata), playerId == "" ? userDetails.Id : playerId, userDetails.game_Id, amount, spend_amount, isBot ? 1 : 0);
#endif
        }
    }

    public BetProcess CheckBetStatus(int index)
    {
        if (isPlayByDummyData)
            return BetProcess.Success;
        if (betDetails.Exists(x => x.index == index))
        {
            BetDetails bet = betDetails.Find(x => x.index == index);
            return bet.Status;
        }
        return BetProcess.Failed;
    }

    public void GetLambdaURL(bool isLive)
    {
        backendAPIURL.IsDataReceived = false;
        DebugHelper.Log("GetLambdaURL Is Live Game========> " + isLive);
        ApiRequest apiRequest = new();
        apiRequest.url = "https://qllb52jc5pxturffykekbtewn40osanl.lambda-url.ap-south-1.on.aws/";
        List<KeyValuePojo> param = new List<KeyValuePojo>();
        param.Add(new KeyValuePojo { keyId = "LoginType", value = isLive ? "1" : "0" });
        param.Add(new KeyValuePojo { keyId = "GameName", value = defaultGameName });
        apiRequest.param = param;
        apiRequest.action = (success, error, body) =>
        {
            if (success)
            {
                ApiResponse response = JsonUtility.FromJson<ApiResponse>(body);
                if (response.code == 200)
                {

                    backendAPIURL = JsonUtility.FromJson<BackendAPI>(response.message);
                    DebugHelper.Log("LootrixMatchAPI===========> " + backendAPIURL.LootrixMatchAPI);
                    DebugHelper.Log("LootrixTransactionAPI===========> " + backendAPIURL.LootrixTransactionAPI);
                    DebugHelper.Log("LootrixGetServerAPI===========> " + backendAPIURL.LootrixGetServerAPI);
                    DebugHelper.Log("LootrixInternetCheckAPI===========> " + backendAPIURL.LootrixInternetCheckAPI);
                    DebugHelper.Log("LootrixValidateServerAPI===========> " + backendAPIURL.LootrixValidateServerAPI);
                    DebugHelper.Log("LootrixServerInactiveAPI===========> " + backendAPIURL.LootrixServerInactiveAPI);
                    DebugHelper.Log("LootrixGetABot===========> " + backendAPIURL.LootrixGetABot);
                    DebugHelper.Log("LootrixAuthenticationAPI===========> " + backendAPIURL.LootrixAuthenticationAPI);
                    DebugHelper.Log("LootrixAudioUpdateAPI==============>" + backendAPIURL.LootrixAudioUpdate);
                    backendAPIURL.IsDataReceived = true;
                }
                else
                {
                    backendAPIURL.IsDataReceived = false;
                }
            }
        };
        ExecuteAPI(apiRequest);
    }



    #endregion



}



[System.Serializable]
public class BackendAPI
{
    //dev https://b465kezbry2ssew2w6w56x3jhi0ennyi.lambda-url.ap-south-1.on.aws/
    //live https://56ebdif5s4aqltb74xhkvnnrai0sxaey.lambda-url.ap-south-1.on.aws/
    public string LootrixMatchAPI = "https://b465kezbry2ssew2w6w56x3jhi0ennyi.lambda-url.ap-south-1.on.aws/";
    //live https://xpxpmhpjldqvjulexwx34z7jca0kdxzf.lambda-url.ap-south-1.on.aws/
    //dev https://vbklyx2pq3nh6xf3vr55vmrwem0ldawq.lambda-url.ap-south-1.on.aws/
    public string LootrixTransactionAPI = "https://vbklyx2pq3nh6xf3vr55vmrwem0ldawq.lambda-url.ap-south-1.on.aws/";
    //live https://htatuqjumsb7qhv3yvmfx6jlza0pdwyg.lambda-url.ap-south-1.on.aws/
    public string LootrixGetServerAPI = "https://htatuqjumsb7qhv3yvmfx6jlza0pdwyg.lambda-url.ap-south-1.on.aws/";
    // https://6rugffwb323fkm7j7umild4vjm0hfcfm.lambda-url.ap-south-1.on.aws/
    public string LootrixInternetCheckAPI = "https://6rugffwb323fkm7j7umild4vjm0hfcfm.lambda-url.ap-south-1.on.aws/";
    // https://vbklyx2pq3nh6xf3vr55vmrwem0ldawq.lambda-url.ap-south-1.on.aws/
    public string LootrixValidateServerAPI = "https://vbklyx2pq3nh6xf3vr55vmrwem0ldawq.lambda-url.ap-south-1.on.aws/";
    // https://uyqcil3sz6qt6pus23vizk2k5i0dlfcx.lambda-url.ap-south-1.on.aws/
    public string LootrixServerInactiveAPI = "https://uyqcil3sz6qt6pus23vizk2k5i0dlfcx.lambda-url.ap-south-1.on.aws/";
    public string LootrixAudioUpdate = "https://lfu76bhfx3dif4sglqij4qu6ou0hghmx.lambda-url.ap-south-1.on.aws/";
    public string LootrixGetABot = "";
    public string LootrixAuthenticationAPI = "";
    public bool IsDataReceived;

}

[System.Serializable]
public class ApiRequest
{
    public NetworkCallType callType;
    public string url;
    public List<KeyValuePojo> param;
    public Action<bool, string, string> action;
}

public class InitBetDetails
{
    public bool status;
    public GameWinningStatus message;
    public int index;
}

[System.Serializable]
public class MinMaxOffest
{
    public float min;
    public float max;
}

[System.Serializable]
public class GameWinningStatus
{
    public string Id;
    public double Amount;
    public MinMaxOffest WinCutOff;
    public float WinProbablity;
    public string Game_Id;
    public string Operator;
    public DateTime create_at;
}

[System.Serializable]
public class UserGameData
{
    public string Id;
    public string name;
    public string token;
    public string session_token;
    public double balance;
    public string currency_type;
    public string game_Id;
    public string gameId;
    public bool isBlockApiConnection;
    public double bootAmount;
    public bool isWin;
    public bool hasBot;
    public float commission;
    public float maxWin;
    public ServerInfo serverInfo;
    public string operatorDomainUrl;
    public string UserDevice;
    public double potLimit;
    public double challLimit;
}

[System.Serializable]
public class TransactionMetaData
{
    public double Amount;
    public string Info;
}
[System.Serializable]
public class BetDetails
{
    public string betID;
    public int index;
    public BetProcess Status;
    public string IsAbleToCancel;
    public Action<bool> action;
    public Action<string> betIdAction;
}

public enum BetProcess
{
    Processing = 0,
    Success = 1,
    Failed = 2,
    None = 3
}

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class ApiResponse
{
    public int code;
    public string message;
    public string data;
    public string output;
}

public class NakamaApiResponse
{
    public int Code;
    public string Message;
}

public class BetResponse
{
    public bool status;
    public string message;
    public int index;
}

[System.Serializable]
public class BotDetails
{
    public string userId;
    public string name;
    public double balance;
}

public static class MatchExtensions
{
    public static Guid ToGuid(this string id)
    {
        MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
        byte[] inputBytes = Encoding.Default.GetBytes(id);
        byte[] hashBytes = provider.ComputeHash(inputBytes);

        return new Guid(hashBytes);
    }
}
public class CreateMatchResponse
{
    public bool status;
    public string MatchToken;
    public int MatchCount;
    public double WinChance;
}

public enum NetworkStatus
{
    Active = 0,
    NetworkIssue = 1,
    ServerIssue = 2
}


[System.Serializable]
public class AuthenticationData
{
    public string Id;
    public string name;
    public string token;
    public string session_token;
    public float balance;
    public string currency_type;
    public string gamename;
    public string operatorname;
    public string operatorDomainUrl;
    public string platform;
    public string returnurl;
    public float comission;
    public double potLimit;
    public double challLimit;
    public double bootAmount;
    public bool music = true;
    public bool sound = true;
    public string environment;
    public EntryAmountDetails game_data = new();
}


[System.Serializable]
public class EntryAmountDetails
{
    public List<int> betValues = new List<int>();
    public int maxBetValue;
    public int minBetValue;
    public int incrementValue;
    public int decrementValue;
    public List<int> entryAmounts = new List<int>();
    public List<int> smallBlinds = new List<int>();
    public List<int> potLimits = new List<int>();
    public List<int> chaalLimits = new List<int>();
    public float comission = 0;
    public int player_count = 0;
}