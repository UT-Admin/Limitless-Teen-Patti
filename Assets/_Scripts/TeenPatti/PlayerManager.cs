using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using TP;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace TP

{
    public class InvitePlayerDetails
    {
        public string playerId;
        public bool isCash;
        public double amount;
        public string playerName;
        public string gameName;
        public int avatharIndex;
        public string avatharUrl;
        public string roomCode;
    }

    public class FriendRequest
    {
        public string playerId;
        public string playerName;
        public int avatharIndex;
        public string avatharUrl;
    }

    public class PlayerManager : NetworkBehaviour
    {

        [Header("======================================================================")]

        [Header("NetworkVariables")]
        public NetworkMatch networkMatch;
        public NetworkIdentity gameManagerNetID;
        [SyncVar] public PlayerData myPlayerData;
        [SyncVar(hook = nameof(OnPlayerStateChanged))] public string myPlayerStateJson;
        [SyncVar] public string playerID;
        [SyncVar] public string roomName;
        [SyncVar] public int playerIndex;
        [SyncVar] public bool isBot;
        [SyncVar] public bool isInLobby;


        [Header("======================================================================")]

        [Header("Bools")]
        public bool isLocalPlayer;
        bool hasRandomUI;
        bool isDuplicate = false;

        [Header("======================================================================")]

        [Header("Referance")]
        public PlayerState myPlayerState = new PlayerState();
        public PlayerUI myUI;
        public GameManager gameManager;

        [Header("======================================================================")]

        [Header("Singleton")]
        public static PlayerManager localPlayer;

        public GameController UIGameController;

        void Awake()
        {
            networkMatch = GetComponent<NetworkMatch>();
            if (isBot)
                InitilizeBot();

        }

        public PlayerState GetPlayerState()
        {
            return myPlayerState;
        }

        private void OnEnable()
        {
            if (isClientOnly)
                StartCoroutine(AddPlayerManager());
        }

        IEnumerator AddPlayerManager()
        {
            while (String.IsNullOrEmpty(myPlayerData.playerID))
                yield return new WaitForSeconds(.5f);
            while (GameManager.localInstance == null)
                yield return new WaitForSeconds(.5f);
            GameManager.localInstance.AddPlayerManager(this);
            GameManager.localInstance.LinkPlayerManagerToUI();

        }

        public void SetPlayerState(PlayerState ps)
        {
            myPlayerData = ps.playerData;
            myPlayerState = ps;
        }

        public void ShowCards(bool isForce = false)
        {
            GamePlayUI.instance.SeeButtonActive(false);
            StartCoroutine(myUI.SetCard(isForce));
        }

        public string GetPlayerID()
        {
            return playerID;
        }

        public IEnumerator InitPlayerManager(bool reset = false)
        {


            if (string.IsNullOrEmpty(playerID))
            {

                yield break;
            }
            while (!GameManager.localInstance.FindPlayerInGame(playerID))
            {
                yield return new WaitForSeconds(.5f);
            }
            while (!GameManager.localInstance.FindPlayerInGame(GameController.Instance.CurrentPlayerData.GetPlayfabID()))
            {
                yield return new WaitForSeconds(.5f);
            }
            bool isActionSuccess;
            PlayerState playerState = GameManager.localInstance.GetPlayerState(playerID);
            if (hasRandomUI || !myUI || reset)
            {
                if (hasRandomUI)
                {
                    myUI.ClearUI();
                    hasRandomUI = false;
                }
                if (playerState.playerData.playerID != "")
                {
                    PlayerUI newUI = GameManager.localInstance.GetUI(playerID);
                    myUI = newUI;
                }
            }
            isActionSuccess = true;

            if (isActionSuccess)
                UpdatePlayerState(playerState);

        }

        public void UpdatePlayerState(PlayerState ps)
        {
            if (ps.playerData.playerID == "")
                return;
            myPlayerState = ps;
            myPlayerData = ps.playerData;
            if (myUI == null)
                myUI = GameManager.localInstance.GetUI(ps.playerData.playerID);
            myUI.UpdatePlayer(this);
        }

        void OnPlayerStateChanged(string oldStr, string newStr)
        {
            myPlayerState = JsonUtility.FromJson<PlayerState>(newStr);


            if (myPlayerState.playerData.playerID == GameController.Instance.CurrentPlayerData.GetPlayfabID())
            {

                localPlayer = this;
                isLocalPlayer = true;
            }
            else
            {
                isLocalPlayer = false;
            }
        }

        public override void OnStartClient()
        {



            if (isLocalPlayer)
                localPlayer = this;
            else
                StartCoroutine(WaitAndCheckJoinStatus(false));
        }

        public override void OnStartLocalPlayer()
        {
            localPlayer = this;

            UpdatePlayerData(JsonUtility.ToJson(GameController.Instance.CurrentPlayerData), GameController.Instance.CurrentAmountType == CashType.CASH, GameController.Instance.CurrentPlayerData.GetPlayfabID());
        }

        public override void OnStopClient()
        {



            ClientDisconnect();
        }

        public override void OnStopServer()
        {
            DebugHelper.LogError(isDuplicate.ToString());
            if (gameManager == null)
                DebugHelper.LogError("gm null " + playerID);
            if (gameManager && !isDuplicate)
                gameManager.SetDisconnectedPlayer(playerID);
            DebugHelper.Log($"Client Stopped on Server " + myPlayerState.playerData.isBot);
            ServerDisconnect();
        }

        public void SearchPreviousGame()
        {
            LoggerUtils.LogError("search previous game");
            CmdSearchPreviousGame(GameController.Instance.CurrentPlayerData.GetPlayfabID());
        }

        public void SearchGame(int _gameType, string lobbyName, bool canReJoin)
        {
            DebugHelper.Log("Can rejoin 2: " + canReJoin);
            DebugHelper.Log(" Check this level 1 ");
            GameController.Instance.isForceJoin = false;
            DebugHelper.Log(" Check this level 2");
            try
            {
                CmdSearchGame(_gameType, lobbyName, canReJoin);
            }
            catch
            {
                UIController.Instance.Loading.SetActive(false);
                DebugHelper.Log(" Check this level 3a ");
            }

            DebugHelper.Log(" Check this level 3");
        }

        public void AddFriend(string playerId)
        {
            if (GameManager.localInstance.GetPlayerState(playerId).playerData.isBot)
            {
                return;
            }
            CmdAddFriend(playerId);
        }

        public void AddSenderFriend(string playerId, string senderId)
        {
            CmdAddSenderFriend(playerId, senderId);
        }

        public void InvitePlayerToGame(string playerId, string roomCode)
        {
            DebugHelper.Log(playerID + this.GetComponent<NetworkIdentity>().hasAuthority);
            CmdInviteToGame(playerId, roomCode);
        }

        public void JoinLobby()
        {
            CmdJoinLobby();
        }

        public void ExitLobby()
        {
            isInLobby = false;
            MirrorManager.instance.ExitLobby(playerID);
        }

        public void HostGame(int _gameType, string lobbyName)
        {
            string matchID = "Room_" + DateTime.UtcNow;
            CmdHostGame(matchID, _gameType, lobbyName, JsonUtility.ToJson(GameController.Instance.GetCurrentGameData()));
        }

        public void JoinGame(string _matchID)
        {
            DebugHelper.Log(_matchID);
            CmdJoinGame(_matchID);

        }

        public void RemovePlayerFromGame()
        {
            ServerDisconnect();
            gameManager.RemovePlayerManager(this);
            gameManager.RemoveFromGame(playerID, true);

        }

        public void ServerDisconnect(bool forceKick = false)
        {
            try
            {
                StopAllCoroutines();
            }
            catch
            {

            }

            if (isInLobby)
                MirrorManager.instance.ExitLobby(playerID);
            else
                MirrorManager.instance.PlayerDisconnected(this, roomName);
            RpcDisconnectGame(forceKick);
            networkMatch.matchId = string.Empty.ToGuid();
            roomName = "";
        }

        public void ServerKick(string message)
        {
            StopAllCoroutines();
            if (isInLobby)
                MirrorManager.instance.ExitLobby(playerID);
            else
                MirrorManager.instance.PlayerDisconnected(this, roomName);
            DebugHelper.Log("server kick start: 1");
            RpcKickedOutGame(message);
        }

        void ClientDisconnect()
        {
            if (gameManager)
                gameManager.RemovePlayerManager(this);
        }

        IEnumerator WaitAndCheckJoinStatus(bool isMine)
        {
            while (!gameManager)
            {
                if (GameManager.localInstance)
                {
                    gameManager = GameManager.localInstance;
                }
                yield return new WaitForSeconds(1);
            }
            if (isMine)
            {
                UIController.Instance.teenPattiGameUIPanel.ShowMe();
                UIController.Instance.Loading.SetActive(false);
            }
            if (isMine)
            {

                UIController.Instance.ShowGameHUD();
            }
            while (GamePlayUI.instance == null)
            {
                yield return new WaitForSeconds(1);
            }

            gameManager.AddPlayerManager(this);

            if (isMine)
            {
                localPlayer = this;

                CheckJoinStatus();
            }
            else
            {

            }

            StartCoroutine(WaitForPlayerInGameState());
        }

        public IEnumerator WaitForPlayerInGameState()
        {
            bool isAddedToGameState = false;
            int tryCount = 0;
            while (!isAddedToGameState)
            {

                if (gameManager.gameState.players.Exists(x => x.playerData.playerID == playerID))
                    isAddedToGameState = true;
                else if (gameManager.gameState.waitingPlayers.Exists(x => x.playerData.playerID == playerID))
                    isAddedToGameState = true;
                else
                {
                    yield return new WaitForSeconds(0.2f);
                    tryCount++;
                    if (tryCount > 5 && isLocalPlayer)
                    {
                        CheckJoinStatus();
                    }
                }

                if (gameManager.gameState.players.Exists(x => x.playerData.playerID == GameController.Instance.CurrentPlayerData.GetPlayfabID()))
                {

                }
                else if (gameManager.gameState.waitingPlayers.Exists(x => x.playerData.playerID == GameController.Instance.CurrentPlayerData.GetPlayfabID()))
                {

                }
                else
                {
                    isAddedToGameState = false;
                    yield return new WaitForSeconds(0.2f);
                }
            }
            StartCoroutine(InitPlayerManager());
        }

        public void CheckJoinStatus()
        {


            if (!isLocalPlayer) return;


            GameManager.localInstance.myPlayerState = myPlayerState;
            GameManager.localInstance.myPlayerData = myPlayerData;
            GameManager.localInstance.myPlayer = this;
            GameManager.localInstance.myPlayerID = playerID;

            GameManager.localInstance.CheckJoinStatus(gameManager.gameState.players.Exists(x => x.playerData.playerID == myPlayerState.playerData.playerID) || gameManager.gameState.waitingPlayers.Exists(x => x.playerData.playerID == myPlayerState.playerData.playerID));

            foreach (PlayerState playerState in gameManager.gameState.players)
            {
                if (playerState.playerData.playerID == myPlayerState.playerData.playerID)
                {
                    myPlayerState = playerState;

                    CmdRejoinGame(playerID);

                    GameManager.localInstance.AddPlayerManager(this);
                    GameManager.localInstance.LinkPlayerManagerToUI(true);

                    StartCoroutine(GameManager.localInstance.UpdateGameWithDelay());
                    return;
                }
            }

            foreach (PlayerState playerState in gameManager.gameState.waitingPlayers)
            {
                if (playerState.playerData.playerID == myPlayerState.playerData.playerID)
                {
                    myPlayerState = playerState;
                    CmdRejoinGame(playerID);
                    GameManager.localInstance.AddPlayerManager(this);
                    GameManager.localInstance.LinkPlayerManagerToUI(true);
                    StartCoroutine(GameManager.localInstance.UpdateGameWithDelay());
                    return;
                }
            }

            CmdAddPlayersToWaitingList(myPlayerData.playerID, NetworkTime.time, JsonUtility.ToJson(myPlayerState));

        }

        void InitializePlayerData()
        {
            playerID = GameController.Instance.CurrentPlayerData.GetPlayfabID();
            myPlayerData.playerID = GameController.Instance.CurrentPlayerData.GetPlayfabID();
            myPlayerData.playerName = GameController.Instance.CurrentPlayerData.GetNickName();
            myPlayerData.currentCards = new CardData[3];
            myPlayerData.silver = GameController.Instance.CurrentPlayerData.GetSilverVal();
            myPlayerData.gold = GameController.Instance.CurrentPlayerData.GetGoldVal();

            if (GameController.Instance.CurrentAmountType == CashType.SILVER)
                myPlayerData.money = GameController.Instance.CurrentPlayerData.GetSilverVal();
            else
                myPlayerData.money = GameController.Instance.CurrentPlayerData.GetGoldVal();
            myPlayerData.profilePicURL = GameController.Instance.CurrentPlayerData.GetAvatarUrl();
            myPlayerData.avatarIndex = (int)GameController.Instance.CurrentPlayerData.GetAvatarIndex();
            myPlayerData.playerLevel = GameController.Instance.CurrentPlayerData.GetPlayerLevel();
            myPlayerData.handsWonCount = GameController.Instance.CurrentPlayerData.GetWeeklyHandsWonVal();
            myPlayerData.currentTableGamesWon = 0;
            myPlayerData.currentTableChipsWon = 0;
            myPlayerState = new PlayerState();
            myPlayerState.playerData = myPlayerData;

        }

        void InitializePlayerData(string playerDataRaw, bool isCash, string playerId)
        {
            PlayfabPlayerData playerData = JsonUtility.FromJson<PlayfabPlayerData>(playerDataRaw);
            isBot = false;
            playerID = playerId;
            myPlayerData.playerID = playerId;
            myPlayerData.playerName = playerData.GetNickName();
            myPlayerData.currentCards = new CardData[3];
            myPlayerData.silver = playerData.GetSilverVal();
            myPlayerData.gold = playerData.GetGoldVal();
            myPlayerData.token = playerData.token;
            myPlayerData.session_token = playerData.session_token;
            myPlayerData.currency_type = playerData.currency_type;
            myPlayerData.gamename = playerData.gamename;
            myPlayerData.operatorname = playerData.operatorname;
            myPlayerData.operatorDomainUrl = playerData.operatorDomainUrl;
            myPlayerData.platform = playerData.platform;
            myPlayerData.comission = playerData.comission;
            myPlayerData.environment = playerData.environment;
            myPlayerData.balance = playerData.balance;



            if (!isCash)
                myPlayerData.money = playerData.GetSilverVal();
            else
                myPlayerData.money = playerData.GetGoldVal();
            myPlayerData.profilePicURL = playerData.GetAvatarUrl();
            myPlayerData.avatarIndex = (int)playerData.GetAvatarIndex();

            myPlayerData.playerLevel = playerData.GetPlayerLevel();
            myPlayerData.handsWonCount = playerData.GetWeeklyHandsWonVal();
            myPlayerData.currentTableGamesWon = 0;
            myPlayerData.currentTableChipsWon = 0;
            myPlayerState = new PlayerState();
            myPlayerState.playerData = myPlayerData;


        }

        public void NextPlayerTurnCommand()
        {
            CmdNextPlayerTurn(playerID, NetworkTime.time);
        }

        #region TARGET_RPC

        [TargetRpc]
        public void Target_CheckOnline()
        {
            APIController.instance.isCheckOnline = false;
        }
        [TargetRpc]
        public void Target_ValidateSession(bool suceess)
        {
            if (suceess)
            {
                APIController.instance.isCheckOnline = false;
            }
            else
            {
#if UNITY_WEBGL && !UNITY_EDITOR
                APIController.DisconnectGame("Session expired. Account active in another device.");
#endif
            }

            DebugHelper.Log("invalide session");


        }

        [TargetRpc]
        public void RPCCardStrength()
        {
            StartCoroutine(gameManager.fetchData());
            GamePlayUI.instance.strengthMeterActive(true);
            myUI.SetSeeBool();
        }



        [TargetRpc]
        public void RPCPrivateMessage(string message, string senderID)
        {
            DebugHelper.LogError("RPCPrivateMessage" + message + senderID);

        }


        [TargetRpc]
        public void TargetRPCRemovePlayerManager()
        {
            gameManager.RemovePlayerManager(this);
            NetworkClient.Disconnect();
            DebugHelper.Log("PlayerManager destroyed");
            StopAllCoroutines();


            DebugHelper.Log("Destroyed PUN:" + playerID);
            try
            {
                GameController.Instance.isInGame = false;
                GameManager.localInstance.isPlayerstateInitialized = false;
                GamePlayUI.instance.ClearAllUI();
                CommonFunctions.Instance.ClearLastEnteredRoom();

                DebugHelper.LogError("Exiting game:" + GameController.Instance.multipleDeviceLogged);
                if (GameController.Instance.multipleDeviceLogged)
                {
                    GameController.Instance.multipleDeviceLogged = false;
                }
                else
                {
                    CommonFunctions.Instance.ClearLastEnteredRoom();
                }
            }
            catch { }
            Destroy(this.gameObject);
        }


        [TargetRpc]
        void TargetJoinGame(bool success, string _matchID)
        {
            if (success)
            {
                roomName = _matchID;
                GameController.Instance.currentRoomName = _matchID;

                PlayerPrefs.SetString("LastEnteredRoom", _matchID);
                StartCoroutine(WaitAndCheckJoinStatus(true));
            }
            else
            {
                DebugHelper.Log("Room Closed Searching New Game");
                // GameController.Instance.StartGameOnButtonClick();
                UIController.Instance.FindGameWEBGL();


                //UIController.Instance.ShowRoomClosed();
                //GameController.Instance.isInGame = false;
                //NetworkClient.Disconnect();
                //UIController.Instance.ShowMainMenu();
            }

        }

        [TargetRpc]
        void TargetHostGame(bool success, string _matchID, int _gameType, string lobbyName)
        {
            if (success)
            {
                roomName = _matchID;

                GameController.Instance.currentRoomName = _matchID;

                PlayerPrefs.SetString("LastEnteredRoom", _matchID);
                StartCoroutine(WaitAndCheckJoinStatus(true));
            }
            else
                HostGame(_gameType, lobbyName);
        }

        [TargetRpc]
        public void InviteToGame(string message)
        {
            DebugHelper.Log(message);

        }



        [TargetRpc]
        void ForceSearchGameFailed()
        {
            DebugHelper.Log("force search game failed");

            StopAllCoroutines();
            GameController.Instance.isForceJoinLobby = true;

            NetworkClient.Shutdown();
        }

        [TargetRpc]
        void TargetSearchGame(bool success, string _matchID, int _gameType, string lobbyName)
        {
            roomName = _matchID;
            if (!success)
            {
                HostGame(_gameType, lobbyName);
            }
            else
            {
                float val = float.Parse(APIController.instance.userDetails.balance.ToString("F2"));
                GamePlayUI.instance.allinAmountText.text = CommonFunctions.Instance.GetAmountDecimalSeparator(APIController.instance.userDetails.balance);
                GamePlayUI.instance.UpdateChaalText(val, true);
                GamePlayUI.instance.isRaisedBet = false;
                UIController.Instance.Connecting.SetActive(false);
                GameController.Instance.currentRoomName = _matchID;
                PlayerPrefs.SetString("LastEnteredRoom", _matchID);
                StartCoroutine(WaitAndCheckJoinStatus(true));
            }

        }

        #endregion

        #region CLIENT_RPC && CLIENT

        [ClientRpc]
        public void RPCMessage(string message)
        {
            DebugHelper.LogError("Message" + message);
        }

        [ClientRpc]
        void RpcDisconnectGame(bool forceKick)
        {
            if (isLocalPlayer)
            {
                if (forceKick)
                {
                    UIController.Instance.serverKick.ShowPopup("Session Token Expired", 401);
                }
                GamePlayUI.instance.ClearAllUI();
                GameController.Instance.DisconnectClient();
                UIController.Instance.BackToMainMenuLoading.gameObject.SetActive(true);
            }
            ClientDisconnect();
        }

        [ClientRpc]
        public void RefreshRPCData(string id)
        {
            if (id == playerID)
                SwRPCManager.Instance.RunAllRPCWithDelay();
        }

        [ClientRpc]
        public void AddAsFriend(string message)
        {


        }

        [ClientRpc]
        void RpcKickedOutGame(string message)
        {

            if (isLocalPlayer)
            {


                if (message == StaticStrings.EnoughBalanceServerKick + (GameController.Instance.CurrentAmountType == CashType.SILVER ? StaticStrings.Chip : StaticStrings.Cash))
                {

                    GameController.Instance.isInGame = false;
                    if (APIController.instance.userDetails.isBlockApiConnection)
                    {
                        UIController.Instance.InsufficientDemo.SetActive(true);
                        UIController.Instance.Loading.SetActive(false);
                    }
                    else
                    {
                        UIController.Instance.Insufficient.SetActive(true);
                        UIController.Instance.Loading.SetActive(false);
                    }

                    GamePlayUI.instance.ClearAllUI();
                    GameController.Instance.DisconnectClient();
                    return;

                }
                GamePlayUI.instance.ClearAllUI();
                UIController.Instance.PlayAgain.SetActive(true);
                GameController.Instance.DisconnectClient();

            }
            else
                ClientDisconnect();
        }

        [Client]
        void InitilizeBot()
        {
            StartCoroutine(InitPlayerManager());

        }

        [ClientRpc]
        public void SetBotInializeAPI(string playerID, double Amount, double balance, bool isinitialize, bool check)
        {
            if (isLocalPlayer)
            {

                if (isinitialize)
                {
                    TransactionMetaData val = new();
                    val.Amount = Amount;
                    val.Info = "FirstBet";

                    int botval;

                    botval = APIController.instance.InitlizeBet(Amount, val, false, (success) =>
                    {

                        if (success)
                        {




                        }
                        else
                        {

                        }

                    }, playerID, true, (BetId) => {


                        SetBetId(BetId, playerID);


                    });


                    SetBetIndex(botval, playerID);

                }
                else
                {
                    TransactionMetaData val = new();
                    val.Amount = Amount;
                    val.Info = "Second Bet";


                    APIController.instance.AddBet(GameManager.localInstance.GetPlayerState(playerID).BetIndex, GameManager.localInstance.GetPlayerState(playerID).BetId, val, Amount, (success) =>
                    {

                        if (success)
                        {

                        }
                        else
                        {

                        }

                    }, playerID, true);
                }
            }

            PlayerUI myUI = GameManager.localInstance.GetPlayerUI(playerID);
            if (myUI != null)
                myUI.GiveAmountToPot(Amount, balance, check);
            Invoke(nameof(checkSound), 0.5f);//1
        }

        public void checkSound()
        {
            if (MasterAudioController.instance.CheckSoundToggle())
                MasterAudioController.instance.PlayAudio(AudioEnum.CHIPSOUND);
        }

        [ClientRpc]
        public void SetBotWinnerAPI(int _index, double winAmount, double amountSpend, string metaData, string BotID, double totalPot)
        {

            TransactionMetaData val = new();
            val.Amount = winAmount;
            val.Info = metaData;


            if (isLocalPlayer)
            {
                APIController.instance.WinningsBetMultiplayer(_index, GameManager.localInstance.GetPlayerState(BotID).BetId, winAmount, amountSpend, totalPot, val, (success) =>
                {


                    if (success)
                    {

                    }
                    else
                    {

                    }

                }, BotID, true, true);

            }

        }


        #endregion

        #region CMD_FUNCTIONS

        [Command]
        public void CMD_CheckOnline()
        {
            Target_CheckOnline();
        }




        [Command]
        public void CmdSetAmountFormAPI(double Val, string PlayerID)
        {

            if (PlayerID == playerID)
            {
                PlayerState playerState = gameManager.GetPlayerState(PlayerID);
                playerState.playerData.money = Val;
                UpdatePlayerState(playerState);
                gameManager.UpdateGameStateToServer();
            }

        }



        [Command]
        public void StartPrivateGame()
        {
            gameManager.StartPrivateGame();
        }
        [Command]
        void CmdRejoinGame(string playerID)
        {
            gameManager.RejoinGame(playerID);
            gameManager.AddPlayerManager(this);
        }

        [Command]
        public void ChallCMD(string playerID, bool isIncreaseBet)
        {
            gameManager.Chaal(playerID, isIncreaseBet);

        }
        [Command]
        public void AllInCMD(string playerID)
        {
            gameManager.AllinMaster(playerID);
        }
        [Command]
        public void PackCMD(string playerID)
        {
            gameManager.PackPlayerServer(playerID);
        }
        [Command]
        public void SideShowCMD(int myIndex, int secondPlayerIndex)
        {
            gameManager.SideShowMaster(myIndex, secondPlayerIndex);
        }
        [Command]
        public void ShowCMD(string playerID)
        {
            gameManager.ShowCardsMaster(playerID);
        }
        [Command]
        public void StandUpCMD(string playerID)
        {
            gameManager.StandUpUserServer(playerID);
        }
        [Command]
        public void SeeCMD(string playerID)
        {
            Debug.Log("SEE CARD  CALLED =========> SeeCMD ");
            gameManager.SeeCardMaster(playerID);
            RPCCardStrength();
        }
        [Command]
        public void SitCMD(string playerID)
        {
            gameManager.OnPlayerSitServer(playerID);
        }
        [Command]
        public void AcceptSideShowCMD(int firstPlayerId, int botPlayerId)
        {
            gameManager.OnAcceptSideShowServer(firstPlayerId, botPlayerId);
        }
        [Command]
        public void DenailSideShowCMD(int firstPlayerId, int botPlayerId)
        {
            gameManager.OnDeclineSideShowServer(firstPlayerId, botPlayerId);
        }

        [Command]
        public void CmdAddFriend(string playerId)
        {
            FriendRequest addFriendRequest = new FriendRequest();
            addFriendRequest.playerId = playerID;
            addFriendRequest.playerName = myPlayerData.playerName;
            addFriendRequest.avatharIndex = myPlayerData.avatarIndex;
            addFriendRequest.avatharUrl = myPlayerData.profilePicURL;
            PlayerManager pm = gameManager.GetPlayerPUN(playerId);
            if (!string.IsNullOrWhiteSpace(pm.playerID))
                pm.AddAsFriend(JsonUtility.ToJson(addFriendRequest));


        }

        [Command]
        void CmdSearchGame(int _gameType, string lobbyName, bool canReJoin)
        {
            isInLobby = false;
            ExitLobby();
            DebugHelper.Log("Can rejoin 3: " + canReJoin);

            if (MirrorManager.instance.SearchGame(gameObject, playerID, _gameType, lobbyName, out roomName, out gameManagerNetID, canReJoin))
            {
                DebugHelper.Log($"Join Game =======> Game Found Successfully</color>");
                networkMatch.matchId = roomName.ToGuid();
                gameManager = gameManagerNetID.GetComponent<GameManager>();
                TargetSearchGame(true, roomName, _gameType, lobbyName);
            }
            else
            {
                DebugHelper.Log($"Join Game =======> Game Search Failed</color>");
                TargetSearchGame(false, roomName, _gameType, lobbyName);
            }
        }


        [Command]
        public void UpdatePlayerData(string playerDataRaw, bool iscash, string playerId)
        {

            InitializePlayerData(playerDataRaw, iscash, playerId);
            myPlayerState.ResetState();
            myPlayerState.disconnectTime = -1;
            myPlayerData = myPlayerState.playerData;
            playerID = myPlayerData.playerID;
            myPlayerStateJson = JsonUtility.ToJson(myPlayerState);
        }

        [Command]
        public void CmdSendPrivateChatMessageMirror(string PlayerIDReciver, string message, string playerIDSender)
        {
            PlayerManager pm = gameManager.GetPlayerPUN(PlayerIDReciver);
            if (!string.IsNullOrWhiteSpace(pm.playerID))
                pm.RPCPrivateMessage(message, playerIDSender);
        }

        [Command]
        void CmdNextPlayerTurn(string userID, double serverTime)
        {
            SWEvent request = new SWEvent();
            request.playerID = userID;
            request.reqTime = serverTime;
            request.eventAction = new Action(() => gameManager.NextPlayerTurn());
            gameManager.AddServerEvent(request);
        }

        [Command]
        public void CmdSendChatMessageMirror(string message)
        {

            DebugHelper.LogError("Went in Chat CMD");
            RPCMessage(message);
        }


        [Command]
        void CmdAddPlayersToWaitingList(string userID, double serverTime, string newPlayerDetails)
        {

            DebugHelper.LogError("WaitAndCheckJoinStatus 0");
            PlayerState newPlayerState = JsonUtility.FromJson<PlayerState>(newPlayerDetails);
            if (gameManager.GetPlayerCount() == 0)
            {
                gameManager.gameState.GameCreatePlayerID = newPlayerState.playerData.playerID;
            }
            if (gameManager.gameController.IsTournament)
            {
                myPlayerData.money = gameManager.gameController.CurrentTournamentModel.currentStartingChips;
                newPlayerState.playerData.money = gameManager.gameController.CurrentTournamentModel.currentStartingChips;
            }

            gameManager.AddPlayersToWaitingList(newPlayerState);
            gameManager.AddPlayerManager(this);


        }


        [Command]
        public void CmdAddBot(bool Win, string val)
        {
            //DebugHelper.Log("Bot Called From API Server  Top Called >>>>>>>>>>>>>>" + val + " >>>>>>>>>>>>>>>>>>  "  + Win);
            //gameManager.gameController.isBotWin = Win;
            //PlayerData APIBotData = JsonUtility.FromJson<PlayerData>(val);
            //gameManager.botAPI = APIBotData;
            //gameManager.AddBotFromAPI();
            //DebugHelper.Log("Bot Called From API Server");

        }

        [Command]
        public void SetBetIndex(int Index, string playeridval)
        {
            DebugHelper.Log(" Bet Index >>>>>>>>>>>>>>" + Index + playeridval);

            gameManager.GetPlayerState(playeridval).BetIndex = Index;
            UpdatePlayerState(gameManager.GetPlayerState(playeridval));



        }

        [Command]
        public void SetBetId(string Index, string playeridval)
        {
            DebugHelper.Log(" Bet Index >>>>>>>>>>>>>>" + Index + playeridval);

            gameManager.GetPlayerState(playeridval).BetId = Index;
            UpdatePlayerState(gameManager.GetPlayerState(playeridval));



        }


        [Command]
        void CmdSearchPreviousGame(string id)
        {
            int _gameType;
            string lobbyName;
            if (MirrorManager.instance.SearchPreviousGame(gameObject, id, out _gameType, out lobbyName, out roomName, out gameManagerNetID))
            {
                LoggerUtils.Log($"<color=green>Game Found Successfully</color>");
                networkMatch.matchId = roomName.ToGuid();
                gameManager = gameManagerNetID.GetComponent<GameManager>();

                TargetSearchGame(true, roomName, _gameType, lobbyName);
            }
            else
            {
                LoggerUtils.LogError("previous game not found");
                ForceSearchGameFailed();
            }

        }

        [Command]
        public void CmdDisconnectGame()
        {

            if (gameManager.gameState.currentState == 1)
            {
                gameManager.playersList.Remove(myPlayerData.playerID);
                gameManager.SubtractFromPotAmount();
                APIController.instance.CancelBetMultiplayerAPI((x, y, z) => { }, myPlayerState.playerData.playerID, gameManager.gameController.gameName, gameManager.gameController.operatorName, gameManager.gameController.gameId, gameManager.gameState.currentMatchToken, myPlayerState.playerData.session_token, myPlayerState.playerData.currency_type
                    , gameManager.gameController.environment);
            }


            RemovePlayerFromGame();
            TargetRPCRemovePlayerManager();
        }

        [Command]
        void CmdJoinGame(string _matchID)
        {
            isInLobby = false;
            ExitLobby();
            roomName = _matchID;
            if (MirrorManager.instance.JoinGame(_matchID, playerID, gameObject, out gameManagerNetID))
            {
                networkMatch.matchId = _matchID.ToGuid();
                gameManager = gameManagerNetID.GetComponent<GameManager>();
                gameManager.AddPlayerManager(this);
                TargetJoinGame(true, _matchID);
            }
            else
            {
                TargetJoinGame(false, _matchID);
            }
        }

        [Command]
        public void CmdJoinLobby()
        {
            isInLobby = true;
            MirrorManager.instance.JoinLobby(gameObject, playerID);
        }
        [Command]
        public void CmdExitLobby()
        {
            ExitLobby();
        }

        [Command(requiresAuthority = false)]
        public void CmdInviteToGame(string playerId, string roomCode)
        {
            InvitePlayerDetails invitePlayer = new InvitePlayerDetails();
            invitePlayer.playerId = playerID;
            invitePlayer.isCash = gameManager.gameController.CurrentAmountType == CashType.CASH;
            invitePlayer.amount = myPlayerData.money;
            invitePlayer.playerName = myPlayerData.playerName;
            invitePlayer.gameName = gameManager.gameController.CurrentGameMode.ToString();
            invitePlayer.avatharIndex = myPlayerData.avatarIndex;
            invitePlayer.avatharUrl = myPlayerData.profilePicURL;
            invitePlayer.roomCode = roomCode;
            MirrorManager.instance.InvitePlayerToPrivateTable(playerId, JsonUtility.ToJson(invitePlayer));
        }




        [Command]
        void CmdHostGame(string _matchID, int _gameType, string lobbyName, string gameData)
        {
            roomName = _matchID;
            DebugHelper.LogError("cmd host");
            if (MirrorManager.instance.HostGame(_matchID, playerID, _gameType, lobbyName, gameObject, gameData, out gameManagerNetID))
            {
                DebugHelper.Log($"<color=green>Game hosted successfully</color>");
                networkMatch.matchId = _matchID.ToGuid();

                gameManager = gameManagerNetID.GetComponent<GameManager>();
                DebugHelper.Log("$$$$$$$$" + JsonUtility.FromJson<TeenPattiGameData>(gameData).ToString());
                gameManager.gameController = JsonUtility.FromJson<TeenPattiGameData>(gameData);
                gameManager.gameState.gameController = gameManager.gameController;
                gameManager.InitCommon();
                gameManager.GetMatchToken(playerID);
                TargetHostGame(true, _matchID, _gameType, lobbyName);
                DebugHelper.Log("$$$$$$$$" + "passed");
            }
            else
            {
                DebugHelper.Log($"<color=red>Game hosted failed</color>");
                TargetHostGame(false, _matchID, _gameType, lobbyName);
            }
        }

        [Command]
        public void CmdAddSenderFriend(string playerId, string senderId)
        {

            SwRPCManager.Instance.SendRPC(senderId, "AddFriendToFriendList", playerId);
            PlayerManager pm = gameManager.GetPlayerPUN(senderId);
            if (!string.IsNullOrWhiteSpace(pm.playerID))
                pm.RefreshRPCData(senderId);
        }


        [Command]
        public void CmdRemoveFriend(string playerId, string senderId)
        {
            SwRPCManager.Instance.SendRPC(playerId, "RemoveFriendFromFriendList", senderId);
            PlayerManager pm = MirrorManager.instance.GetLobbyPlayerManager(playerId);
            if (pm != null)
                pm.RefreshRPCData(playerId);

        }

        [Command]
        public void setBoolIsWinnerPlayer(bool isWin)
        {

            //gameManager.SetWinner(isWin);
            //DebugHelper.Log("init bet response :::::::----:::  " + gameManager.isBotWin);
        }


        #endregion


        public void ValidateSession(string playerID, string token, string gamename, string operatorname, string session_token, bool isBlock)
        {

            CmdValidateSession(playerID, token, gamename, operatorname, session_token, isBlock);
        }


        public void StartGameAuthentication(string data)
        {
            CmdStartGameAuthentication(data);
        }


        [Command]
        private void CmdStartGameAuthentication(string data)
        {
            LoggerUtils.Log("StartAuthenticationServerSideCall  data============> " + data);
            AuthenticationData authentication = JsonUtility.FromJson<AuthenticationData>(data);
            JObject json = JObject.Parse(data);
            List<KeyValuePojo> param = new List<KeyValuePojo>();
            param.Add(new KeyValuePojo { keyId = "request_type", value = "auth" });
            param.Add(new KeyValuePojo { keyId = "user_token", value = authentication.token });
            param.Add(new KeyValuePojo { keyId = "platform", value = authentication.platform });
            param.Add(new KeyValuePojo { keyId = "currency", value = authentication.currency_type });
            param.Add(new KeyValuePojo { keyId = "game_name", value = authentication.gamename });
            param.Add(new KeyValuePojo { keyId = "operator", value = authentication.operatorname });

            string url1 = StaticStrings.GetLambdaUrl(authentication.environment);
            ApiRequest apiRequest1 = new ApiRequest();
            apiRequest1.action = (success, error, body) =>
            {
                
                if (success)
                {
                    DebugHelper.Log("StartAuthenticationServerSideCall success ================> " + success);
                    JObject apiResponse = JObject.Parse(body);
                    HandleErrorCode((int)apiResponse["code"], playerID, apiResponse["message"].ToString());
                }
                else
                {
                    LoggerUtils.Log("StartAuthenticationServerSideCall error ================> " + error);
                }
                TargetStartAuthenticationClientSideCall(body, error, success);
            };
            apiRequest1.url = url1;
            apiRequest1.param = param;
            apiRequest1.callType = NetworkCallType.POST_METHOD_USING_JSONDATA;
            APIController.instance.ExecuteAPI(apiRequest1);
        }


        [TargetRpc]
        public void TargetStartAuthenticationClientSideCall(string body, string error, bool success)
        {
            APIController.instance.StartAuthenticationClientSideCall(body, error, success);
        }


        /*[Command]
        private void CmdValidateSession(string playerID, string token, string gamename, string operatorname, string session_token, bool isBlock)
        {
            

            if (isBlock)
            {
                TargetValidateSession(true);
                return;
            }
           
            APIController.instance.ValidateSession(playerID, token, gamename, operatorname, session_token,isBlock, (success) => TargetValidateSession(success));
        }
        [TargetRpc]
        private void TargetValidateSession(bool success)
        {
            DebugHelper.Log("TARGET valide Session Called ===================> " + success);
            APIController.instance.isValidateSession = false;
            if (success)
                return;
#if UNITY_WEBGL && !UNITY_EDITOR
            APIController.DisconnectGame("Session expired. Account active in another device.");
            DebugHelper.Log("Invalide Session ===================> ");
#endif
        }*/
        [Command]
        private void CmdValidateSession(string playerID, string token, string gamename, string operatorname, string session_token, bool isBlock)
        {

            if (isBlock)
            {
                //TargetValidateSession("");
                return;
            }

            //  APIController.instance.ValidateSession(playerID, token, gamename, operatorname, session_token, isBlock, (jsonBody) => TargetValidateSession(jsonBody));
        }
        [TargetRpc]
        private void TargetValidateSession(string jsonBody)
        {

            APIController.instance.ValidateSessionResponce(jsonBody);
        }


        public void GetUpdatedBalance(string Id, string SessionToken, string CurrencyType, string UserToken, string gameName, string Operator, string environment)
        {
            CmdGetUpdatedBalance(Id, SessionToken, CurrencyType, UserToken, gameName, Operator, environment);
        }
        [Command]
        private void CmdGetUpdatedBalance(string Id, string SessionToken, string CurrencyType, string UserToken, string gameName, string Operator, string environment)
        {
            List<KeyValuePojo> param = new List<KeyValuePojo>();
            param.Add(new KeyValuePojo { keyId = "user_token", value = UserToken });
            param.Add(new KeyValuePojo { keyId = "game_name", value = gameName });
            param.Add(new KeyValuePojo { keyId = "operator", value = Operator });
            param.Add(new KeyValuePojo { keyId = "request_type", value = "info" });
            param.Add(new KeyValuePojo { keyId = "user_id", value = Id });
            param.Add(new KeyValuePojo { keyId = "session_token", value = SessionToken });
            param.Add(new KeyValuePojo { keyId = "balance", value = myPlayerState.playerData.money.ToString() });
            param.Add(new KeyValuePojo { keyId = "currency", value = CurrencyType });


            string url1 = StaticStrings.GetLambdaUrl(environment);
            ApiRequest apiRequest1 = new ApiRequest();
            apiRequest1.action = (success, error, body) =>
            {
                if (success)
                {
                    DebugHelper.Log("CmdGetUpdatedBalance success ================> " + success);
                    ApiResponse apiResponse = JsonUtility.FromJson<ApiResponse>(body);
                    // HandleErrorCode(apiResponse.code, playerID, apiResponse.message);
                }
                else
                {

                    DebugHelper.Log("CmdGetUpdatedBalance error ================> " + error);
                }
                TargetGetUpdatedBalanceClientSideCall(body, error, success);
            };
            apiRequest1.url = url1;
            apiRequest1.param = param;
            apiRequest1.callType = NetworkCallType.POST_METHOD_USING_JSONDATA;
            APIController.instance.ExecuteAPI(apiRequest1);
        }

        [TargetRpc]
        public void TargetGetUpdatedBalanceClientSideCall(string body, string error, bool success)
        {
            APIController.instance.GetUpdatedBalanceClientSideCall(body, error, success);
        }


        public void HandleErrorCode(int Code, string playerID, string Message, bool fromStartAuth = false)
        {
            switch (Code)
            {
                case 200:
                    break;
                case 401:
                case 403:
                case 405:
                case 412:
                case 500:
                case 413:
                case 501:
                case 408:
                case 502:
                    ShowErrorPopup(playerID, Message, Code);
                    break;
                case 402:
                    InsufficientPopup(playerID);
                    break;
                case 409:
                    break;
                case 504:
                    ServerMaintance(playerID);
                    break;
            }

        }


        [TargetRpc]
        public void InsufficientPopup(string playerID)
        {
            UIController.Instance.Insufficient.SetActive(true);
        }


        [TargetRpc]
        public void ServerMaintance(string playerID)
        {
            //UIController.Instance.ConnectionIssue.SetActive(true);
        }




        [TargetRpc]
        public void ShowErrorPopup(string playerID, string Message, int code)
        {
            UIController.Instance.serverKick.ShowPopup(Message, code);
        }

    }
}