using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using System.Net;
using Newtonsoft.Json;

namespace TP
{
    public class RemoveEmetyRoom
    {
        public Coroutine coroutine;
        public string matchId;
    }

    public class MirrorManager : NetworkBehaviour
    {
        public static MirrorManager instance;
        public bool isConnectedToServer;
        public SyncList<string> roomIDs = new SyncList<string>();
        [SerializeField]
        public List<MirrorRoomInfo> roomList = new List<MirrorRoomInfo>();
        public List<string> emptyRoomList = new List<string>();
        [SerializeField]
        public List<MirrorPlayer> LobbyPlayers = new List<MirrorPlayer>();
        public List<RemoveEmetyRoom> emetyRooms = new List<RemoveEmetyRoom>();
        public SyncList<string> LobbyPlayersList = new SyncList<string>();
        NetworkMatch networkMatch;
        public bool isRunLiveServer;
        string roomDataStr = "";
        public RoomData roomData = new RoomData();
        private void Awake()
        {
            instance = this;
            networkMatch = GetComponent<NetworkMatch>();

        }

        /// <summary>
        /// called when the script is enabled
        /// </summary>
        private void Start()
        {
            if (isServer)
            {
                StartCoroutine(nameof(UpdateRoomInfo));

            }
        }



        /// <summary>
        ///  The Room ID and Room list are updated below for each match created
        /// </summary>

        IEnumerator UpdateRoomInfo()
        {
#if UNITY_EDITOR
            yield break;
#endif
            if (!isRunLiveServer)
                yield break;
            string serverIPAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList.First(f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString();
            while (true)
            {
                yield return new WaitForSeconds(5);
                roomData.roomData.Clear();
                roomData.serverAddress = serverIPAddress;
                roomData.gameName = "TEENPATTI";
                roomData.playerCount = 0;
                roomData.botCount = 0;
                roomData.roomCount = 0;
                foreach (MirrorRoomInfo rooms in roomList)
                {
                    RoomInfoData infoData = new RoomInfoData();
                    infoData.botCount = rooms.GetBotCount();
                    infoData.playerCount = rooms.GetRealPlayerCount();
                    infoData.isClosed = rooms.isClosed;
                    infoData.isRoomFull = rooms.isRoomFull;
                    infoData.isVisible = rooms.isVisible;
                    infoData.lobbyName = rooms.lobbyName;
                    infoData.roomName = rooms.roomName;
                    roomData.botCount += infoData.botCount;
                    roomData.playerCount += infoData.playerCount;
                    roomData.roomCount += 1;
                    roomData.roomData.Add(infoData);
                }
                if (roomDataStr != JsonUtility.ToJson(roomData))
                {
                    roomDataStr = JsonUtility.ToJson(roomData);
                    ApiRequest apiRequest = new ApiRequest();
                    apiRequest.action = (status, error, body) =>
                    {
                    };
                    apiRequest.callType = NetworkCallType.GET_METHOD;
                    apiRequest.url = "https://faa3fxxjp475nyajp53ljl4yve0ddudp.lambda-url.ap-south-1.on.aws/";
                    List<KeyValuePojo> param = new List<KeyValuePojo>();
                    param.Add(new KeyValuePojo { keyId = "requestType", value = "UpdateRoomInfo" });
                    param.Add(new KeyValuePojo { keyId = "server_id", value = roomData.serverAddress });
                    param.Add(new KeyValuePojo { keyId = "player_count", value = roomData.playerCount.ToString() });
                    param.Add(new KeyValuePojo { keyId = "bot_count", value = roomData.botCount.ToString() });
                    param.Add(new KeyValuePojo { keyId = "room_count", value = roomData.roomCount.ToString() });
                    param.Add(new KeyValuePojo { keyId = "game_name", value = roomData.gameName });
                    param.Add(new KeyValuePojo { keyId = "data", value = JsonConvert.SerializeObject(roomData.roomData) });
                    apiRequest.param = param;
                    APIController.instance.ExecuteAPI(apiRequest);
                }
            }
        }



        private void OnEnable()
        {
            isConnectedToServer = true;
            //UIController.Instance.InternetPop.SetActive(false);
        }

        private void OnDisable()
        {
            isConnectedToServer = false;
            Debug.Log("Mirror Check ==============> " + GameController.Instance.isInGame);
            if (GameController.Instance.isInGame  /*UIController.Instance.mainmenuPage.gameObject.activeSelf*/)
            {
                if (!GameController.Instance.isInGame && GameController.Instance.isForceJoinLobby)
                {
                    GameController.Instance.isForceJoinLobby = false;
                    return;
                }
                //UIController.Instance.InternetPop.SetActive(true);
                AudioListener.volume = 0;
                GameController.Instance.isREconnectonce = true;
                UIController.Instance.InternetPopNew.SetActive(true);
                //GameController.Instance.Reconnect();
            }
        }
        public PlayerManager GetLobbyPlayerManager(string playerID)
        {
            if (LobbyPlayers.Exists(x => x.playerID == playerID))
                return LobbyPlayers.Find(x => x.playerID == playerID).playerManager;
            return null;
        }
        public int GetPing()
        {
            if (NetworkClient.isConnected)
            {
                return (int)(NetworkTime.rtt * 1000);
            }
            else
                return 0;
        }

        public bool SearchPreviousGame(GameObject _playerObj, string _playerID, out int _gameType, out string lobbyName, out string matchID, out NetworkIdentity gameManagerNetID)
        {
            lock (roomList)
            {
                matchID = "";
                gameManagerNetID = GetComponent<NetworkIdentity>();
                _gameType = 0;
                lobbyName = "";
                for (int i = 0; i < roomList.Count; i++)
                {
                    if (roomList[i].gameState.players.Exists(x => x.playerData.playerID == _playerID && x.currentState == 1))
                    {
                        if (JoinGame(roomList[i].roomName, _playerID, _playerObj, out gameManagerNetID))
                        {
                            matchID = roomList[i].roomName;
                            gameManagerNetID = roomList[i].gameManagerID;
                            _gameType = (int)roomList[i].gameType;
                            lobbyName = roomList[i].lobbyName;
                            return true;
                        }
                    }
                }

                return false;

            }
        }

        public void JoinLobby(GameObject gameObject, string playerId)
        {
            if (!LobbyPlayers.Exists(x => x.playerID == playerId))
            {
                MirrorPlayer player = new MirrorPlayer();
                player.playerID = playerId;
                player.playerManager = gameObject.GetComponent<PlayerManager>();
                LobbyPlayers.Add(player);
                LobbyPlayersList.Add(playerId);
            }
            else
            {
                ExitLobby(playerId);
                JoinLobby(gameObject, playerId);
            }
        }
        public void ExitLobby(string playerId)
        {
            if (LobbyPlayers.Exists(x => x.playerID == playerId))
            {
                LobbyPlayers.RemoveAll(x => x.playerID == playerId);
                LobbyPlayersList.RemoveAll(x => x == playerId);
            }

        }

        [SerializeField] GameObject gameManagerPrefab, botPlayerManager, lobbyManager;

        public void AddBot(PlayerState ps, GameManager gm)
        {
            GameObject go = Instantiate(botPlayerManager);
            NetworkServer.Spawn(go);
            PlayerManager pm = go.GetComponent<PlayerManager>();
            go.GetComponent<NetworkMatch>().matchId = gm.roomInfo.roomName.ToGuid();
            pm.playerID = ps.playerData.playerID;

            pm.myPlayerData = ps.playerData;
            pm.myPlayerState = ps;
            pm.roomName = gm.roomInfo.roomName;
            pm.myPlayerStateJson = JsonUtility.ToJson(ps);
            pm.isBot = true;
            pm.gameManager = gm;
            pm.gameManagerNetID = gm.GetComponent<NetworkIdentity>();

            MirrorPlayer newPlayer = new MirrorPlayer();
            newPlayer.playerID = ps.playerData.playerID;

            newPlayer.playerManager = pm;
            MirrorRoomInfo room = roomList.Find(x => (x.roomName == gm.roomInfo.roomName && x.lobbyName == gm.roomInfo.lobbyName));
            room.players.Add(newPlayer);
            gm.roomInfo = room;
            gm.botPlayers.Add(go);
            gm.CheckBotStatus(pm);
        }
        public void RemoveBot(GameObject go)
        {
            NetworkServer.Destroy(go);


        }

        public bool HostGame(string _matchID, string _playerID, int _GameMode, string lobbyName, GameObject _playerObj, string gameData, out NetworkIdentity gameManagerNetID)
        {
            gameManagerNetID = GetComponent<NetworkIdentity>();
            if (!roomIDs.Contains(_matchID))
            {
                GameObject go = Instantiate(gameManagerPrefab);
                NetworkServer.Spawn(go);

                go.GetComponent<GameManager>().networkMatch.matchId = _matchID.ToGuid();
                TeenPattiGameData data = JsonUtility.FromJson<TeenPattiGameData>(gameData);



                MirrorRoomInfo roomInfo = new MirrorRoomInfo();
                roomInfo.roomName = _matchID;
                roomInfo.gameManagerID = go.GetComponent<NetworkIdentity>();
                roomInfo.gameType = (GameMode)_GameMode;
                roomInfo.lobbyName = lobbyName;
                MirrorPlayer newPlayer = new MirrorPlayer();
                newPlayer.playerID = _playerID;
                newPlayer.playerManager = _playerObj.GetComponent<PlayerManager>();
                roomInfo.players.Add(newPlayer);

                roomList.Add(roomInfo);
                roomIDs.Add(_matchID);

                gameManagerNetID = roomInfo.gameManagerID;
                go.GetComponent<GameManager>().roomInfo = roomInfo;
                return true;
            }
            else
            {
                Debug.Log($"Match ID already exists");
                return false;
            }
        }

        public bool JoinGame(string _matchID, string _playerID, GameObject _playerObj , out NetworkIdentity gameManagerNetID, bool isRejoin = false)
        {
            gameManagerNetID = GetComponent<NetworkIdentity>();
            if (roomIDs.Contains(_matchID))
            {

                for (int i = 0; i < roomList.Count; i++)
                {
                    if (roomList[i].roomName == _matchID)
                    {

                        GameManager gm = roomList[i].gameManagerID.gameObject.GetComponent<GameManager>();
                        Debug.Log("Join Game Current Room  max player Count " + gm.gameState.players.Count +"    "+ gm.gameState.waitingPlayers.Count);
                        if ((gm.gameState.players.Count + gm.gameState.waitingPlayers.Count) < roomList[i].maxPlayerCount ||   (isRejoin && (gm.gameState.players.Count + gm.gameState.waitingPlayers.Count) <= roomList[i].maxPlayerCount))
                        {
                            Debug.Log("Join Game Current Room is not Full");
                        }
                        else
                        {
                            Debug.Log("Join Game Current Room is Full");
                            return false;
                        }

                        if (roomList[i].players.Exists(x => x.playerID == _playerID) || (!roomList[i].isClosed && !roomList[i].isRoomFull))
                        {
                            if (roomList[i].players.Exists(x => x.playerID == _playerID))
                            {
                                try
                                {
                                    roomList[i].players.Find(x => x.playerID == _playerID).playerManager.ServerKick("");
                                }
                                catch
                                {
                                    roomList[i].players.RemoveAll(x => x.playerID == _playerID);
                                }
                            }
                            MirrorPlayer newPlayer = new MirrorPlayer();
                            newPlayer.playerID = _playerID;
                            newPlayer.playerManager = _playerObj.GetComponent<PlayerManager>();
                            gameManagerNetID = roomList[i].gameManagerID;
                            roomList[i].players.Add(newPlayer);



                            if (roomList[i].players.Count == roomList[i].maxPlayerCount)
                            {
                                roomList[i].isRoomFull = true;
                                roomList[i].isClosed = true;
                            }

                            break;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }

                Debug.Log($"Match joined");
                StopClearMatchCoroutine(_matchID);
                return true;
            }
            else
            {
                Debug.Log($"Match ID does not exist");
                return false;
            }
        }

        public bool SearchGame(GameObject _playerObj, string _playerID, int _GameMode, string lobbyName, out string matchID, out NetworkIdentity gameManagerNetID, bool canReJoin)
        {

            lock (roomList)
            {
                matchID = "";
                gameManagerNetID = GetComponent<NetworkIdentity>();
                Debug.Log("Can rejoin 4: " + canReJoin);

                for (int i = 0; i < roomList.Count; i++)
                {
                    if (!canReJoin)
                    {
                        Debug.Log("Can rejoin 5: " + canReJoin);
                        if (roomList[i].players.Exists(x => x.playerID == _playerID))
                        {
                            Debug.Log("Can rejoin 6: " + canReJoin + roomList[i].players.Exists(x => x.playerID == _playerID));
                            roomList[i].gameManagerID.gameObject.GetComponent<GameManager>().CheckForReJoin(roomList[i].players.Find(x => x.playerID == _playerID).playerManager, _playerID, roomList[i].lobbyName);
                            return false;
                        }

                    }

                    //Debug.Log($"Join Game ===>  Checking match {roomList[i].players.Count}");
                    //Debug.Log($"Join Game ===>  Checking match {_GameMode}");
                    //Debug.Log($"Join Game ===>  Checking match {(int)roomList[i].gameType}");
                    //Debug.Log($"Join Game ===>  Checking match {roomList[i].lobbyName}");
                    //Debug.Log($"Join Game ===>  Checking match {lobbyName}");
                    Debug.Log($"Join Game ===>  Checking match {roomList[i].players.Exists(x => x.playerID == _playerID)} ////////////////  {roomList[i].players.Count}");


                     

                    if ((int)roomList[i].gameType == _GameMode && roomList[i].lobbyName == lobbyName && roomList[i].players.Count < 5)
                    {
                        Debug.Log($"Join Game =========> Checking match {roomList[i].roomName} | inMatch {roomList[i].isClosed} | matchFull {roomList[i].isRoomFull} count {roomList[i].players.Count} / {roomList[i].maxPlayerCount}");
                        if (roomList[i].players.Exists(x => x.playerID == _playerID) || (!roomList[i].isClosed && !roomList[i].isRoomFull && roomList[i].players.Count < roomList[i].maxPlayerCount))
                        {
                            Debug.Log("Join Game =======> 1" + canReJoin);
                            if (JoinGame(roomList[i].roomName, _playerID, _playerObj, out gameManagerNetID, canReJoin))
                            {
                                Debug.Log("Join Game =======> 2");
                                matchID = roomList[i].roomName;
                                gameManagerNetID = roomList[i].gameManagerID;
                                return true;
                            }
                        }
                    }
                }
                Debug.Log("Join Game =======> 3");
                return false;
            }
        }

        public void PlayerDisconnected(PlayerManager player, string _roomName)
        {
            for (int i = 0; i < roomList.Count; i++)
            {
                if (roomList[i].roomName == _roomName)
                {
                    roomList[i].players.RemoveAll(x => x.playerManager == player);
                    roomList[i].isRoomFull = false;
                    if (!isServer) return;

                    Debug.Log($"Join Game Player disconnected from match {_roomName} | {roomList[i].players.Count} players remaining");
                    if (roomList[i].players.Count == 4 && roomList[i].players.Exists(x => x.playerManager.myPlayerData.isBot))
                    {
                        roomList[i].isClosed = false;
                    }

                    if (roomList[i].GetPlayerCount() == 0)
                    {
                        Debug.Log($"No more players in Match. Attempting Terminating {_roomName}");
                        StopClearMatchCoroutine(_roomName);
                        StartCoroutinForClearMatch(roomList[i]);

                    }
                    break;
                }
            }
        }
        void StopClearMatchCoroutine(string matchID)
        {
            if (emetyRooms.Exists(x => x.matchId == matchID))
            {
                if (emetyRooms.Find(x => x.matchId == matchID).coroutine != null)
                    StopCoroutine(emetyRooms.Find(x => x.matchId == matchID).coroutine);
                emetyRooms.RemoveAll(x => x.matchId == matchID);
            }
        }

        void StartCoroutinForClearMatch(MirrorRoomInfo room)
        {

            Coroutine coroutine = StartCoroutine(WaitAndClearMatch(room));
            RemoveEmetyRoom emetyRoom = new RemoveEmetyRoom();
            emetyRoom.coroutine = coroutine;
            emetyRoom.matchId = room.roomName;
            emetyRooms.Add(emetyRoom);
        }
        IEnumerator WaitAndClearMatch(MirrorRoomInfo room)
        {
            if (!emptyRoomList.Contains(room.roomName))
                emptyRoomList.Add(room.roomName);
            //else
            //    yield break;

            Debug.LogError("Empty room: " + room.roomName);
            yield return new WaitForSeconds(15);
            if (room.GetPlayerCount() == 0)
            {
                GameManager gm = room.gameManagerID.GetComponent<GameManager>();
                string matchId = gm.gameState.currentMatchToken;
                bool isTournament = gm.gameController.IsTournament;

                //gm.playerManagersList.Where(x => x != null).ToList();
                //while (gm.playerManagersList.Count > 0)
                //{
                //    if(gm.playerManagersList[0].gameObject)
                //    NetworkServer.Destroy(gm.playerManagersList[0].gameObject);
                //    gm.playerManagersList.Where(x => x != null).ToList();
                //}
                Debug.LogError("Empty room cleared: " + room.roomName);
                roomList.Remove(room);
                roomIDs.Remove(room.roomName);
                if (room.gameManagerID.gameObject)
                    NetworkServer.Destroy(room.gameManagerID.gameObject);
            }
            else
                Debug.LogError("Room clear cancelled, Player count: " + room.players.Count);
            emptyRoomList.Remove(room.roomName);
        }




        #region inviteFriends
        public void InvitePlayerToPrivateTable(string playerID, string senderID)
        {
            if (LobbyPlayers.Exists(x => x.playerID == playerID))
            {
                MirrorPlayer mirrorPlayer = LobbyPlayers.Find(x => x.playerID == playerID);
                if (mirrorPlayer.playerManager)
                {
                    mirrorPlayer.playerManager.InviteToGame(senderID);
                }
            }
        }
        #endregion
    }

    /// <summary>
    ///  This helper class holds the RoomData
    /// </summary>
    [System.Serializable]
    public class RoomData
    {
        public string serverAddress;
        public string gameName;
        public int playerCount;
        public int botCount;
        public int roomCount;
        public List<RoomInfoData> roomData = new List<RoomInfoData>();
    }

    /// <summary>
    ///  This helper class holds the RoomInfoData
    /// </summary>
    [System.Serializable]
    public class RoomInfoData
    {
        public string roomName;
        public string lobbyName;
        public bool isClosed;
        public bool isRoomFull;
        public bool isVisible;
        public int playerCount;
        public int botCount;
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


}

