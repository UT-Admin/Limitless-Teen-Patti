using System;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;
using Cysharp.Threading.Tasks;
using Random = UnityEngine.Random;
using static WebApiManager;
using TP;
using Mirror;
using UnityEngine.UI;
using System.Net;
using System.Linq;

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
    public static extern void GetUpdatedBalance();
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
        Debug.Log($"Calleeedddd check internet {data}   -   {isOnline}   -   {isInFocus}");
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
            CheckInternetForButtonClick((status) => {
                if (status)
                {
                    WebApiManager.Instance.GetNetWorkCall(NetworkCallType.POST_METHOD_USING_FORMDATA
                 ,
                 "https://waekhvdxviqdmzdzo6hisjqsli0bvajw.lambda-url.ap-south-1.on.aws/?requestType=ServerInactive&Id&Message",
                 new List<KeyValuePojo>() { new KeyValuePojo { keyId = "requestType", value = "ServerInactive" }, new KeyValuePojo { keyId = "Id", value = userDetails.gameId }, new KeyValuePojo { keyId = "Message", value = "Server connection issue " + val } },
                 (bool isSuccess, string error, string body) =>
                 {
                 }, 2);
                    OnInternetStatusChange?.Invoke(NetworkStatus.ServerIssue);
                }
                else
                    OnInternetStatusChange?.Invoke(NetworkStatus.NetworkIssue);
            });
        }

    }

    public async void CheckInternetForButtonClick(Action<bool> action)
    {
        WebApiManager.Instance.GetNetWorkCall(NetworkCallType.POST_METHOD_USING_FORMDATA
                 , backendAPIURL.LootrixInternetCheckAPI,
                 new List<KeyValuePojo>(),
                 (bool isSuccess, string error, string body) =>
                 {
                     action.Invoke(isSuccess);
                 });
    }

    public void OnSwitchingTabs(string data)
    {
        Time.timeScale = 1;
        isInFocus = data == "true" ? true : false;
        Debug.Log($"Calleeedddd switching tab {data}   -   {isOnline}   -   {isInFocus}");
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
        Debug.Log("DepositCancelResponse  :::::::----::: " + data);
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
        Debug.Log("unity :: base64 is :: " + encryptedData);
        encryptedData = Encryptbase64String(encryptedData);
        Debug.Log("unity :: encoded base64 is :: " + encryptedData);
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
                Debug.Log("notfound" + c);
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

    private async void ClearBetResponse(string betID)
    {
        await UniTask.Delay(100);
        try
        {
            if (betRequest != null && betRequest.Count > 0 && betRequest.Exists(x => x.BetId.Equals(betID)))
            {
                BetRequest request = betRequest.Find(x => x.BetId.Equals(betID));
                Debug.Log($"BetID is {request.BetId}\n Bet Index is {request.betId}\nPlayer Id is {request.PlayerId}\nMatch Token is {request.MatchToken}");
                betRequest.RemoveAll(x => x.BetId.Equals(betID));
            }
        }
        catch (Exception e)
        {
            Debug.Log($"Error: {e}");
        }
    }

    public void GetServerDetails(Action<ServerInfo, string> action)
    {
        ApiRequest apiRequest = new ApiRequest();///?requestType=GetGameServer&game_name=carrom
        apiRequest.url = "https://sijnoejayed2n2hodo3cot5st40ndsxd.lambda-url.ap-south-1.on.aws";
        List<KeyValuePojo> param = new List<KeyValuePojo>();
        param.Add(new KeyValuePojo { keyId = "requestType", value = "GetGameServer" });
        Debug.Log(userDetails.game_Id);
        param.Add(new KeyValuePojo { keyId = "&game_name", value = userDetails.game_Id.Split("_")[1] });
        apiRequest.param = param;
        apiRequest.action = (success, error, body) => {
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
                Debug.Log(response.message);
            }
            else
            {
                Debug.Log(error);
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
        //Debug.Log($"Sending Api Request URl :-" + base64EncodedString);
#if UNITY_WEBGL
        ExecuteExternalUrl(base64EncodedString, 5);
#endif

        CheckAPICallBack(base64EncodedString);

    }

    public void ReloadPage(string data)
    {
        Debug.Log("Mirror Check Reload ==============> " + GameController.Instance.isInGame);
        GameController.Instance.isInGame = false;
    }


    public void SetUserData(string data)
    {
        DebugHelper.Log("Response from webgl ::::: " + data);
        if (data.Length < 30)
        {
            userDetails = new UserGameData();
            userDetails.balance = 5000;
            // userDetails.balance = amount;

            userDetails.currency_type = "USD";
            userDetails.Id = DateTime.UtcNow.ToString() + "----" + Random.Range(1, 100000000);
            // userDetails.Id = playerId;

            userDetails.token = UnityEngine.Random.Range(5000, 500000) + SystemInfo.deviceUniqueIdentifier.ToGuid().ToString();
            //userDetails.name = SystemInfo.deviceName + SystemInfo.deviceModel;
            userDetails.name = "User_" + UnityEngine.Random.Range(100, 999);
            isPlayByDummyData = true;
            userDetails.hasBot = true;
            userDetails.game_Id = "demo_" + defaultGameName;
            userDetails.isBlockApiConnection = true;
            //userDetails.commission = 0.2f;

        }
        else
        {
            userDetails = JsonUtility.FromJson<UserGameData>(data);
            isPlayByDummyData = userDetails.isBlockApiConnection;
            isWin = userDetails.isWin;
            maxWinAmount = userDetails.maxWin;
        }
        IsBotInGame = userDetails.hasBot;
        if (userDetails.bootAmount == 0)
            userDetails.bootAmount = defaultBootAmount;
        if (string.IsNullOrWhiteSpace(userDetails.gameId))
            userDetails.gameId = "ecd5c5ce-e0a1-4732-82a0-099ec7d180be";
        DebugHelper.Log(JsonUtility.ToJson(userDetails));


        GetAutoscaleServerDetail(true, () => {
            OnUserDetailsUpdate?.Invoke();
            OnUserBalanceUpdate?.Invoke();
#if UNITY_WEBGL && !UNITY_EDITOR
                      ValidateSession();
               //     CheckOnline();
#endif
        }, 3);

        //GetServerDetails((serverinfo, info) =>
        //{
        //    Debug.Log($"server details is host {serverinfo.server_host} ,  port {serverinfo.server_port}  ,  scheme   {serverinfo.server_scheme}");
        //    userDetails.serverInfo = serverinfo;

        //    if (string.IsNullOrWhiteSpace(serverinfo.server_host))
        //    {
        //        userDetails.serverInfo.server_host = DefaultHostAddress;
        //        userDetails.serverInfo.server_port = DefaultHostPort;
        //        userDetails.serverInfo.server_scheme = DefaultHostSchema;
        //    }
        //    OnUserDetailsUpdate?.Invoke();
        //    OnUserBalanceUpdate?.Invoke();

        //});
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
        if ((int)(jsonObject["code"]) == 200)
        {
            Debug.Log("online true 1");
            defaultDelay = (int)(jsonObject["data"]);
            isOnline = true;
            PlayNextGameMsg = "";
            success = true;
        }
        else if ((int)(jsonObject["code"]) == 203)
        {
            Debug.Log("online true 1.5");
            isOnline = true;
            PlayNextGameMsg = (string)jsonObject["message"];
        }
        else if ((int)(jsonObject["code"]) != 200)
        {
            Debug.Log("online true 2");
            isOnline = true;
            PlayNextGameMsg = "";
            DisconnectGame((string)jsonObject["message"]);
        }
        else
        {
            Debug.Log("online false 1");
            isOnline = false;
        }


        Debug.Log("TARGET valide Session Called ===================> " + success);

#endif
        //        if (success)
        //            return;
        //#if UNITY_WEBGL && !UNITY_EDITOR
        //            APIController.DisconnectGame("Session expired. Account active in another device.");
        //            Debug.Log("Invalide Session ===================> ");
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



    public void ValidateSession(string playerID, string token, string gamename, string operatorname, string session_token, bool isBlocked, Action<string> action)
    {



        if (isBlocked)
        {
            // action.Invoke(true);
            return;
        }
        List<KeyValuePojo> param = new List<KeyValuePojo>();
        param.Add(new KeyValuePojo { keyId = "PlayerId", value = playerID });
        param.Add(new KeyValuePojo { keyId = "token", value = token });
        param.Add(new KeyValuePojo { keyId = "GameName", value = gamename });
        param.Add(new KeyValuePojo { keyId = "Operator", value = operatorname });
        param.Add(new KeyValuePojo { keyId = "session_token", value = session_token });
        param.Add(new KeyValuePojo { keyId = "requestType", value = "validateSession" });



        WebApiManager.Instance.GetNetWorkCall(NetworkCallType.GET_METHOD, backendAPIURL.LootrixTransactionAPI, param, (success, error, body) =>
        {

            action.Invoke(body);

        });
    }

    public void CheckMirrorGameAvaliblity(string host, string port, Action<bool, string> action)
    {
        Debug.Log("Mirror Server check ==========> Check Once " + host + port);
        var param = new List<KeyValuePojo>();
        param.Add(new KeyValuePojo { keyId = "requestType", value = "CheckMirrorServer" });
        param.Add(new KeyValuePojo { keyId = "host", value = host });
        param.Add(new KeyValuePojo { keyId = "port", value = port });

        WebApiManager.Instance.GetNetWorkCall(NetworkCallType.GET_METHOD, backendAPIURL.LootrixValidateServerAPI, param, (success, error, body) =>
        {
            Debug.Log("Mirror Server check ==========> BODY " + body);
            if (success)
            {

                ApiResponse response = JsonUtility.FromJson<ApiResponse>(body);
                Debug.Log("Mirror Server check ==========> IsACTIVE " + response.code + "**************" + response.message);
                action.Invoke(response.code == 200, response.message);
            }
            else
            {
                Debug.Log("Mirror Server check ==========> " + " Not IsACTIVE");
                if (body.Contains("timeout"))
                {
                    UIController.Instance.ConnectionIssue.SetActive(true);
                }
                else
                {
                    action.Invoke(false, "error");
                }

                CheckMirrorInActive(GameController.Instance.CurrentServerHost, userDetails.serverInfo.instance_id, userDetails.gameId);
            }
        }, 1);
    }

    public void CheckMirrorInActive(string host, string ip, string gameid)
    {
        Debug.Log("CheckMirrorInActive ==========> Check Once " + host + ip);
        var param = new List<KeyValuePojo>();
        param.Add(new KeyValuePojo { keyId = "requestType", value = "ServerInactive" });
        param.Add(new KeyValuePojo { keyId = "Id", value = gameid });
        param.Add(new KeyValuePojo { keyId = "Host", value = host });
        param.Add(new KeyValuePojo { keyId = "Ip", value = ip });
        WebApiManager.Instance.GetNetWorkCall(NetworkCallType.POST_METHOD_USING_FORMDATA, backendAPIURL.LootrixServerInactiveAPI, param, (success, error, body) =>
        {
            Debug.Log("CheckMirrorInActive ==========> BODY " + body);
            if (success)
            {
                Debug.Log("CheckMirrorInActive ====>");
            }
        });
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
                Debug.Log("Validate Session called CLIENT PLAYERMANAGER ===================> ");
                PlayerManager.localPlayer.ValidateSession(userDetails.Id, userDetails.token, userDetails.game_Id.Split("_")[1], userDetails.game_Id.Split("_")[0], userDetails.session_token, userDetails.isBlockApiConnection);
                await UniTask.Delay(defaultDelay * 1000);
                Debug.Log("Need to show disconnect popup " + isValidateSession.ToString());
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


    public void GetAutoscaleServerDetail(bool isRejoin, Action action, int Count)
    {
        var param = new List<KeyValuePojo>();
        param.Add(new KeyValuePojo { keyId = "isRejoin", value = isRejoin ? "1" : "0" });
        param.Add(new KeyValuePojo { keyId = "gameName", value = userDetails.game_Id.Split("_")[1] });
        param.Add(new KeyValuePojo { keyId = "operator", value = userDetails.game_Id.Split("_")[0] });
        param.Add(new KeyValuePojo { keyId = "playerId", value = userDetails.Id });

        WebApiManager.Instance.GetNetWorkCall(NetworkCallType.GET_METHOD, backendAPIURL.LootrixGetServerAPI, param, (success, error, body) =>
        {

            if (success)
            {

                Debug.Log(body); ApiResponse response = JsonUtility.FromJson<ApiResponse>(body);

                if (response.code != 200)
                {
                    if (Count > 0)
                    {
                        Count--;
                        GetAutoscaleServerDetail(isRejoin, action, Count);

                    }
                    else
                    {
                        APIController.instance.CheckMirrorGameAvaliblity(GameController.Instance.CurrentServerHost, GameController.Instance.CurrentServerPort, (success, message) =>
                        {
                            if (!success)
                            {
                                UIController.Instance.ConnectionIssue.SetActive(true);
                            }

                        });
                    }


                }
                else
                {
                    ServerData server = JsonUtility.FromJson<ServerData>(response.message);
                    userDetails.serverInfo = new()
                    {
                        instance_id = server.instanceIP,
                        server_host = server.domainName,
                        server_port = 7777,
                        server_scheme = "https",
                        server_cpu_usage = (float)server.cpuUsage,
                    };
                    if (APIController.instance.IsLiveGame)
                    {
                        Mirror.NetworkManager.singleton.networkAddress = server.domainName;
                    }

                    Debug.Log(JsonUtility.ToJson(userDetails.serverInfo));
                }

                Debug.Log(body);
            }
            else
            {
                if (Count > 0)
                {
                    Count--;
                    GetAutoscaleServerDetail(isRejoin, action, Count);

                }
                else
                {
                    APIController.instance.CheckMirrorGameAvaliblity(GameController.Instance.CurrentServerHost, GameController.Instance.CurrentServerPort, (success, message) =>
                    {
                        if (!success)
                        {
                            UIController.Instance.ConnectionIssue.SetActive(true);
                        }

                    });
                }

            }
            action.Invoke();
        });
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
        GetLambdaURL(IsLiveGame);

#if UNITY_WEBGL && !UNITY_EDITOR
        GetLoginData();
        //CheckInternetStatus();
#elif UNITY_EDITOR && !UNITY_SERVER
        SetUserData("");
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

    public void TerminateMatch(string matchToken)
    {
        List<KeyValuePojo> param = new List<KeyValuePojo>();
        param.Add(new KeyValuePojo { keyId = "Id", value = matchToken });
        param.Add(new KeyValuePojo { keyId = "requestType", value = "TerminateMatch" });

        WebApiManager.Instance.GetNetWorkCall(NetworkCallType.GET_METHOD, backendAPIURL.LootrixMatchAPI, param, (success, error, body) =>
        {
            if (success)
            {
                JObject jsonObject = JObject.Parse(body);
                Debug.Log($"TerminateMatch: {jsonObject}");
                if ((int)(jsonObject["code"]) == 200)
                {


                }
                else
                {

                }
            }
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

    public void CreateMatch(string lobbyName, Action<CreateMatchResponse> action, string gamename, string operatorname, string playerId, bool isBlockAPI, string server)
    {
        CreateMatchResponse matchResponse = new CreateMatchResponse();
        //if (isBlockAPI)
        //{
        //    matchResponse.status = true;
        //    matchResponse.MatchToken = DateTime.UtcNow.ToString().ToGuid().ToString();
        //    action.Invoke(matchResponse);
        //    return;
        //}
        string serverIPAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList.First(f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString();
        List<KeyValuePojo> param = new List<KeyValuePojo>();
        param.Add(new KeyValuePojo { keyId = "Created_By", value = serverIPAddress });
        param.Add(new KeyValuePojo { keyId = "Game_Name", value = gamename == "" ? userDetails.game_Id.Split()[1] : gamename });
        param.Add(new KeyValuePojo { keyId = "Operator", value = operatorname == "" ? userDetails.game_Id.Split()[0] : operatorname });
        param.Add(new KeyValuePojo { keyId = "requestType", value = "CreateMatch" });
        param.Add(new KeyValuePojo { keyId = "Lobby_Name", value = lobbyName });
        param.Add(new KeyValuePojo { keyId = "GameName", value = gamename == "" ? userDetails.game_Id.Split()[1] : gamename });


        WebApiManager.Instance.GetNetWorkCall(NetworkCallType.GET_METHOD, backendAPIURL.LootrixMatchAPI, param, (success, error, body) =>
        {
            if (success)
            {
                Debug.Log(server + " ::: match created " + body);
                JObject jsonObject = JObject.Parse(body);
                if ((int)(jsonObject["code"]) == 200)
                {
                    JObject jsonObject1 = JObject.Parse(jsonObject["data"].ToString());
                    matchResponse = JsonUtility.FromJson<CreateMatchResponse>(jsonObject["data"].ToString());
                    matchResponse.status = true;
                    action.Invoke(matchResponse);

                }
                else
                {
                    matchResponse.status = false;
                    action.Invoke(matchResponse);

                }

            }
            else
            {
                matchResponse.status = false;
                action.Invoke(matchResponse);
            }
        });
    }
    [ContextMenu("RandomPrediction")]
    public void GetRandomPrediction()
    {
        GetRandomPredictionIndexApi(9, 5, 1, (data, status) => { Debug.Log(data); }, "tower");
    }



    public void ApiCallBackDebugger(string data)
    {
        //Debug.Log("API Call Back Debug :- " + data);

        //byte[] bytesToEncode = Encoding.UTF8.GetBytes(url);
        byte[] bytesToEncode = Convert.FromBase64String(data);

        string base64EncodedString = Encoding.UTF8.GetString(bytesToEncode);

        Debug.Log(base64EncodedString + "inpuT");

        JObject OBJ = JObject.Parse(base64EncodedString);
        string url = OBJ["url"].ToString();
        int code = int.Parse(OBJ["status"].ToString());
        string body = OBJ["body"].ToString();
        string error = OBJ["error"].ToString();
        //Debug.Log("===========================");
        //Debug.Log(url);
        //Debug.Log(body);
        //Debug.Log(code);
        //Debug.Log(error);
        //Debug.Log("===========================");
        APICallBack(url, code, body, error);
    }

    public void APICallBack(string url, int code, string body, string error)
    {
        Debug.Log($"API_ Response :-  URL{url} -- Code {code} -- Body {body} -- Error {error}");
        foreach (var item in apiRequestList)
        {
            if (item.url == url)
            {
                if (code == 200)
                {
                    Debug.Log("api response :::::: true");
                    item.callback(true, error, body);
#if UNITY_WEBGL
                    if (!userDetails.isBlockApiConnection)
                        GetUpdatedBalance();
#endif
                }
                else
                {
                    Debug.Log("api response :::::: false");
                    item.callback(false, error, body);
                }
            }
        }
        apiRequestList.RemoveAll(x => x.url == url);
    }

    public async void CheckAPICallBack(string url)
    {

        await UniTask.Delay(7000);
        Debug.Log($"API_ Response :-  URL{url} ");
        foreach (var item in apiRequestList)
        {
            if (item.url == url)
            {
                item.callback(false, "timeout", "timeout");
            }
        }
        apiRequestList.RemoveAll(x => x.url == url);
    }

    public void GetRandomPredictionIndexApi(int rowCount, int columnCount, int predectedCount, Action<string, bool> OnScucces = null, string gamename = "")
    {
        List<KeyValuePojo> param = new List<KeyValuePojo>();
        param.Add(new KeyValuePojo { keyId = "rowCount", value = rowCount.ToString() });
        param.Add(new KeyValuePojo { keyId = "columnCount", value = columnCount.ToString() });
        param.Add(new KeyValuePojo { keyId = "predictionCount", value = predectedCount.ToString() });
        param.Add(new KeyValuePojo { keyId = "requestType", value = "RandomPrediction" });
        param.Add(new KeyValuePojo { keyId = "gameName", value = gamename == "" ? userDetails.game_Id.Split()[1] : gamename });

        WebApiManager.Instance.GetNetWorkCall(NetworkCallType.GET_METHOD, backendAPIURL.LootrixMatchAPI, param, (success, error, body) =>
        {
            if (success)
            {
                JObject jsonObject = JObject.Parse(body);
                ApiResponse response = JsonUtility.FromJson<ApiResponse>(body);
                if (response.code == 200)
                {
                    OnScucces?.Invoke(response.message, true);
                }
                else
                {
                    OnScucces?.Invoke(body, false);
                }
            }
            else
            {
                OnScucces?.Invoke("error", false);
            }
        });
    }

    public void CheckInternetStatus(int tryCount = 0)
    {
        return;
        //#if !UNITY_SERVER
        //        WebApiManager.Instance.GetNetWorkCall(NetworkCallType.GET_METHOD, "https://google.com/", new List<KeyValuePojo>(), async (success, error, body) =>
        //        {
        //            if (success)
        //            {
        //                OnInternetStatusChange?.Invoke(success.ToString());
        //                await UniTask.Delay(2500);
        //                CheckInternetStatus();
        //            }
        //            else
        //            {
        //                tryCount += 1;
        //                if (tryCount > 4)
        //                {
        //                    OnInternetStatusChange?.Invoke(success.ToString());
        //                    await UniTask.Delay(500);
        //                    CheckInternetStatus(tryCount);
        //                }
        //                else
        //                {
        //                    CheckInternetStatus(tryCount);
        //                }

        //            }
        //        }, 2);
        //#endif
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

    public void AddPlayers(string matchToken, List<string> players)
    {
        if (APIController.instance.userDetails.isBlockApiConnection)
        {
            return;
        }
        List<KeyValuePojo> param = new List<KeyValuePojo>();
        param.Add(new KeyValuePojo { keyId = "Id", value = matchToken });
        param.Add(new KeyValuePojo { keyId = "Players", value = JsonConvert.SerializeObject(players) });
        param.Add(new KeyValuePojo { keyId = "requestType", value = "AddPlayers" });

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

    public void ExecuteAPI(ApiRequest api, int timeout = 0)
    {
        WebApiManager.Instance.GetNetWorkCall(api.callType, api.url, api.param, (success, error, body) =>
        {
            Debug.Log($"<color=orange>Success is set to {success}, error is set to {error} and body is set to {body}\nURL is : {api.url}</color>");
            if (success)
            {
                Debug.Log($"<color=orange>API sent to success</color>");
                api.action?.Invoke(success, error, body);
            }
            else
            {
                if (timeout >= 3)
                {
                    api.action?.Invoke(success, error, body);
                    Debug.Log($"<color=orange>API run failed with timeout {timeout}</color>");
                }
                else
                {
                    Debug.Log($"<color=orange>API recalled with timeout set to {timeout}</color>");
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









    public void GetABotAPI(List<string> botId, Action<BotDetails> action, string domainURL)
    {
        List<KeyValuePojo> param = new List<KeyValuePojo>();
        param.Add(new KeyValuePojo { keyId = "player", value = JsonConvert.SerializeObject(botId) });
        string url = domainURL + "api/getbot";
        ApiRequest apiRequest = new ApiRequest();
        apiRequest.action = (success, error, body) =>
        {
            if (success)
            {
                NakamaApiResponse nakamaApi = JsonUtility.FromJson<NakamaApiResponse>(body);
                BotDetails bot = new BotDetails();
                bot = JsonUtility.FromJson<BotDetails>(body);
                action?.Invoke(bot);
            }
        };
        apiRequest.url = url;
        apiRequest.param = param;
        apiRequest.callType = NetworkCallType.GET_METHOD;
        ExecuteAPI(apiRequest);
    }

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


    public async void WinningsBetMultiplayerAPI(int betIndex, string betId, double win_amount_with_comission, double spend_amount, double pot_amount, TransactionMetaData metadata, Action<bool> action, string playerId, bool isBot, bool isWinner, string gameName, string operatorName, string gameId, float commission, string matchToken, string domainUrl)
    {
        await UniTask.Delay(3000);
        Debug.Log($"BetIndex: {betIndex}, playerId: {playerId}, matchToken: {matchToken}");
        BetRequest request = betRequest.Find(x => x.betId == betIndex && x.PlayerId == playerId && x.MatchToken.Equals(matchToken));
        Debug.Log($"Request data is {JsonUtility.ToJson(request)}");
        while (request.BetId != betId)
        {
            await UniTask.Delay(200);
        }

        Debug.Log($"<color=orange>WinningsBetMultiplayerAPI called with commission set to {commission}</color>");
        if (!isWinner)
        {
            commission = 0.0f;
        }
        List<KeyValuePojo> param = new List<KeyValuePojo>();

        param.Add(new KeyValuePojo { keyId = "playerID", value = playerId });
        param.Add(new KeyValuePojo { keyId = "amount", value = win_amount_with_comission.ToString() });
        param.Add(new KeyValuePojo { keyId = "amountSpend", value = spend_amount.ToString() });
        param.Add(new KeyValuePojo { keyId = "cashType", value = 1.ToString() });
        param.Add(new KeyValuePojo { keyId = "game_name", value = gameName });
        param.Add(new KeyValuePojo { keyId = "metadata", value = JsonUtility.ToJson(metadata) });
        param.Add(new KeyValuePojo { keyId = "isBot", value = isBot ? "1" : "0" });
        param.Add(new KeyValuePojo { keyId = "matchToken", value = playerId });
        param.Add(new KeyValuePojo { keyId = "comission", value = commission.ToString() });
        //        string url = APIController.instance.userDetails.operatorDomainUrl + "api/winningbet";
        string url = domainUrl + "api/winningbet";
        int timeout = 0;
        ApiRequest apiRequest = new ApiRequest();
        apiRequest.action = (success, error, body) =>
        {
            if (success)
            {
                NakamaApiResponse nakamaApi = JsonUtility.FromJson<NakamaApiResponse>(body);
                if (nakamaApi.Code == 200)
                {

                    List<KeyValuePojo> param1 = new List<KeyValuePojo>
            {
                    new KeyValuePojo { keyId = "Id", value = betId },
                    new KeyValuePojo { keyId = "GameName", value = gameName },
                    new KeyValuePojo { keyId = "Operator", value = operatorName },
                    new KeyValuePojo { keyId = "Game_Id", value = gameId },
                    new KeyValuePojo { keyId = "isBot", value = isBot ? "1" : "0" },
                    new KeyValuePojo { keyId = "Win_amount", value = win_amount_with_comission.ToString() },
                    new KeyValuePojo { keyId = "amountSpend", value = spend_amount.ToString() },
                    new KeyValuePojo { keyId = "potamount", value = pot_amount.ToString() },
                    new KeyValuePojo { keyId = "Comission", value = commission.ToString() },
                    new KeyValuePojo { keyId = "isWin", value = isWinner ? "1" : "0" },
                    new KeyValuePojo { keyId = "requestType", value = "winningBet" }
            };

                    string url1 = backendAPIURL.LootrixTransactionAPI;
                    ApiRequest apiRequest1 = new ApiRequest();
                    apiRequest1.action = (success, error, body) =>
                    {
                        if (success)
                        {
                            ApiResponse response = JsonUtility.FromJson<ApiResponse>(body);
                            action?.Invoke(response != null && response.code == 200);
                            ClearBetResponse(request.BetId);
                        }
                        else
                        {
                            action?.Invoke(false);
                        }
                    };
                    apiRequest1.url = url1;
                    apiRequest1.param = param1;
                    apiRequest1.callType = NetworkCallType.GET_METHOD;
                    ExecuteAPI(apiRequest1);
                }
                else
                {
                    action?.Invoke(false);
                }
            }
            else
            {
                action?.Invoke(false);
            }
        };
        apiRequest.url = url;
        apiRequest.param = param;
        apiRequest.callType = NetworkCallType.GET_METHOD;
        ExecuteAPI(apiRequest);


    }
    public async void CancelBetMultiplayerAPI(int betIndex, string betId, double amount, TransactionMetaData metadata, Action<bool> action, string playerId, bool isBot, bool isWinner, string gameName, string operatorName, string gameId, string matchToken, string domainURL)
    {
        BetRequest request = betRequest.Find(x => x.betId == betIndex && x.PlayerId == playerId && x.MatchToken.Equals(matchToken));
        while (request.BetId != betId)
        {
            await UniTask.Delay(200);
        }
        List<KeyValuePojo> param = new List<KeyValuePojo>();
        param.Add(new KeyValuePojo { keyId = "playerID", value = string.IsNullOrEmpty(playerId) ? userDetails.Id : playerId });
        param.Add(new KeyValuePojo { keyId = "amount", value = amount.ToString() });
        param.Add(new KeyValuePojo { keyId = "cashType", value = 1.ToString() });
        param.Add(new KeyValuePojo { keyId = "metadata", value = JsonUtility.ToJson(metadata) });
        param.Add(new KeyValuePojo { keyId = "isBot", value = isBot ? "1" : "0" });
        param.Add(new KeyValuePojo { keyId = "matchToken", value = string.IsNullOrEmpty(playerId) ? userDetails.Id : playerId });
        string url = domainURL + "api/cancelbet";
        ApiRequest apiRequest = new ApiRequest();
        apiRequest.action = (success, error, body) =>
        {
            if (success)
            {
                NakamaApiResponse nakamaApi = JsonUtility.FromJson<NakamaApiResponse>(body);
                if (nakamaApi.Code == 200)
                {
                    List<KeyValuePojo> param1 = new List<KeyValuePojo>
                {
                    new KeyValuePojo { keyId = "Id", value = betId },
                    new KeyValuePojo { keyId = "GameName", value = gameName },
                    new KeyValuePojo { keyId = "Operator", value = operatorName },
                    new KeyValuePojo { keyId = "Game_Id", value = gameId },
                    new KeyValuePojo { keyId = "isBot", value = isBot ? "1" : "0" },
                    new KeyValuePojo { keyId = "requestType", value = "cancelBet" }
                };
                    string url1 = backendAPIURL.LootrixTransactionAPI;
                    ApiRequest apiRequest1 = new ApiRequest();
                    apiRequest1.action = (success, error, body) =>
                    {
                        if (success)
                        {
                            ApiResponse response = JsonUtility.FromJson<ApiResponse>(body);
                            action?.Invoke(response != null && response.code == 200);
                        }
                        else
                        {
                            action?.Invoke(false);
                        }
                    };
                    apiRequest1.url = url1;
                    apiRequest1.param = param1;
                    apiRequest1.callType = NetworkCallType.GET_METHOD;
                    ExecuteAPI(apiRequest1);

                }
                else
                {
                    action?.Invoke(false);
                }
            }
            else
            {
                action?.Invoke(false);
            }
        };
        apiRequest.url = url;
        apiRequest.param = param;
        apiRequest.callType = NetworkCallType.GET_METHOD;
        ExecuteAPI(apiRequest);
    }

    public async void AddBetMultiplayerAPI(int index, string BetId, TransactionMetaData metadata, double amount, Action<bool> action, string playerId, bool isBot, string gameName, string operatorName, string gameId, string matchToken, string domainURL)
    {

        Debug.Log($"ADD BET MULTIPLAYER API ===>  BetIndex: {index}, playerId: {playerId}, matchToken: {matchToken} , betRequestCount: {betRequest.Count}");
        BetRequest request = betRequest.Find(x => x.betId == index && x.PlayerId == playerId && x.MatchToken.Equals(matchToken));
        while (request.BetId != BetId)
        {
            await UniTask.Delay(200);
        }
        List<KeyValuePojo> param = new List<KeyValuePojo>();
        param.Add(new KeyValuePojo { keyId = "playerID", value = string.IsNullOrEmpty(playerId) ? userDetails.Id : playerId });
        param.Add(new KeyValuePojo { keyId = "amount", value = amount.ToString() });
        param.Add(new KeyValuePojo { keyId = "cashType", value = 1.ToString() });
        param.Add(new KeyValuePojo { keyId = "metadata", value = JsonUtility.ToJson(metadata) });
        param.Add(new KeyValuePojo { keyId = "isBot", value = isBot ? "1" : "0" });
        param.Add(new KeyValuePojo { keyId = "matchToken", value = matchToken });
        string url = domainURL + "api/addbet";
        ApiRequest apiRequest = new ApiRequest();
        apiRequest.action = (success, error, body) =>
        {
            if (success)
            {
                NakamaApiResponse nakamaApi = JsonUtility.FromJson<NakamaApiResponse>(body);
                if (nakamaApi.Code == 200)
                {
                    id += 1;
                    List<KeyValuePojo> param1 = new List<KeyValuePojo>();

                    param1.Add(new KeyValuePojo { keyId = "Game_Id", value = gameId });
                    param1.Add(new KeyValuePojo { keyId = "GameName", value = gameName });
                    param1.Add(new KeyValuePojo { keyId = "Operator", value = operatorName });
                    param1.Add(new KeyValuePojo { keyId = "isBot", value = isBot ? "1" : "0" });
                    param1.Add(new KeyValuePojo { keyId = "Id", value = BetId });
                    param1.Add(new KeyValuePojo { keyId = "Bet_amount", value = amount.ToString() });
                    param1.Add(new KeyValuePojo { keyId = "requestType", value = "addBet" });
                    param1.Add(new KeyValuePojo { keyId = "MetaData", value = JsonUtility.ToJson(metadata) });

                    string url1 = backendAPIURL.LootrixTransactionAPI;
                    ApiRequest apiRequest1 = new ApiRequest();
                    apiRequest1.action = (success, error, body) =>
                    {
                        if (success)
                        {
                            ApiResponse response = JsonUtility.FromJson<ApiResponse>(body);
                            action?.Invoke(response != null && response.code == 200);
                        }
                        else
                        {
                            action?.Invoke(false);
                        }
                    };
                    apiRequest1.url = url1;
                    apiRequest1.param = param1;
                    apiRequest1.callType = NetworkCallType.GET_METHOD;
                    ExecuteAPI(apiRequest1);
                }
                else
                {
                    action?.Invoke(false);
                }
            }
            else
            {
                Debug.Log("AddBetCAlled   Failure" + amount + " *********************************** " + isBot + " *********************************************################################################################################### " + playerId);
                action?.Invoke(false);
            }
        };
        apiRequest.url = url;
        apiRequest.param = param;
        apiRequest.callType = NetworkCallType.GET_METHOD;
        ExecuteAPI(apiRequest);
    }



    public int InitBetMultiplayerAPI(int index, double amount, TransactionMetaData metadata, bool isAbleToCancel, Action<bool> action, string playerId, string playerName, bool isBot, Action<string> betIdAction, string gameName, string operatorName, string gameID, string matchToken, string domainURL)
    {
        Debug.Log($"<color=orange>Initializing bet for player {playerId}, index is {index}</color>");

        List<KeyValuePojo> param = new List<KeyValuePojo>();
        BetRequest bet = new BetRequest();
        bet.MatchToken = matchToken;
        bet.PlayerId = playerId;
        bet.betId = index;
        betRequest.Add(bet);
        param.Add(new KeyValuePojo { keyId = "playerID", value = string.IsNullOrEmpty(playerId) ? userDetails.Id : playerId });
        param.Add(new KeyValuePojo { keyId = "amount", value = amount.ToString() });
        param.Add(new KeyValuePojo { keyId = "cashType", value = 1.ToString() });
        param.Add(new KeyValuePojo { keyId = "metadata", value = JsonUtility.ToJson(metadata) });
        param.Add(new KeyValuePojo { keyId = "isBot", value = isBot ? "1" : "0" });
        param.Add(new KeyValuePojo { keyId = "matchToken", value = matchToken });
        string url = domainURL + "api/initbet";
        ApiRequest apiRequest = new ApiRequest();
        apiRequest.action = (success, error, body) =>
        {
            if (success)
            {
                NakamaApiResponse nakamaApi = JsonUtility.FromJson<NakamaApiResponse>(body);
                if (nakamaApi.Code == 200)
                {
                    List<KeyValuePojo> param1 = new List<KeyValuePojo>
             {
                 new KeyValuePojo { keyId = "Game_Id", value = gameID},
                 new KeyValuePojo { keyId = "GameName", value = gameName },
                 new KeyValuePojo { keyId = "Operator", value = operatorName },
                 new KeyValuePojo { keyId = "PlayerName", value = playerName },
                 new KeyValuePojo { keyId = "Game_user_Id", value = playerId },
                 new KeyValuePojo { keyId = "Status", value = isAbleToCancel.ToString() },
                 new KeyValuePojo { keyId = "IsAbleToCancel", value = isAbleToCancel.ToString() },
                 new KeyValuePojo { keyId = "isBot", value = isBot ? "1" : "0" },
                 new KeyValuePojo { keyId = "Index", value = index.ToString() },
                 new KeyValuePojo { keyId = "Bet_amount", value = amount.ToString() },
                 new KeyValuePojo { keyId = "requestType", value = "initBet" },
                 new KeyValuePojo { keyId = "MatchToken", value = matchToken },
                 new KeyValuePojo { keyId = "MetaData", value = JsonUtility.ToJson(metadata) }
             };
                    GameWinningStatus _winningStatus;
                    string url1 = backendAPIURL.LootrixTransactionAPI;
                    ApiRequest apiRequest1 = new ApiRequest();
                    apiRequest1.action = (success, error, body) =>
                    {
                        if (success)
                        {
                            ApiResponse response = JsonUtility.FromJson<ApiResponse>(body);
                            action?.Invoke(response != null && response.code == 200);
                            if (response.code == 200)
                            {
                                Debug.Log($"<color=aqua>Response message is : {response.data}</color>");
                                _winningStatus = JsonUtility.FromJson<GameWinningStatus>(response.data);
                                bet.BetId = _winningStatus.Id;
                                betIdAction.Invoke(_winningStatus.Id);
                            }
                        }
                        else
                        {
                            action?.Invoke(false);
                        }
                    };
                    apiRequest1.url = url1;
                    apiRequest1.param = param1;
                    apiRequest1.callType = NetworkCallType.GET_METHOD;
                    ExecuteAPI(apiRequest1);
                }
                else
                {
                    action?.Invoke(false);
                }
            }
            else
            {
                action?.Invoke(false);
            }
        };
        apiRequest.url = url;
        apiRequest.param = param;
        apiRequest.callType = NetworkCallType.GET_METHOD;
        ExecuteAPI(apiRequest);
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
        Debug.Log("GetLambdaURL Is Live Game========> " + isLive);
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
                    Debug.Log("LootrixMatchAPI===========> " + backendAPIURL.LootrixMatchAPI);
                    Debug.Log("LootrixTransactionAPI===========> " + backendAPIURL.LootrixTransactionAPI);
                    Debug.Log("LootrixGetServerAPI===========> " + backendAPIURL.LootrixGetServerAPI);
                    Debug.Log("LootrixInternetCheckAPI===========> " + backendAPIURL.LootrixInternetCheckAPI);
                    Debug.Log("LootrixValidateServerAPI===========> " + backendAPIURL.LootrixValidateServerAPI);
                    Debug.Log("LootrixServerInactiveAPI===========> " + backendAPIURL.LootrixServerInactiveAPI);
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
    public object output;
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