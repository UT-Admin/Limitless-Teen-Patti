using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace TP.Mirror
{

    [System.Serializable]
    public class RoomInfo
    {
        public string roomName;
        public string lobbyName;
        public bool isClosed = false;
        public bool isRoomFull = false;
        public bool isVisible = true;
        public GameType gameType;
        public List<MirrorPlayer> players = new List<MirrorPlayer>();
        public int maxPlayerCount = 3;
        public NetworkIdentity gameManagerID;
    }

    [System.Serializable]
    public class SyncListGameObject : SyncList<GameObject> { }

    [System.Serializable]
    public class SyncListString : SyncList<string> { }

    //public enum GameType
    //{
    //    Teenpatti,
    //    Poker,
    //    Rummy
    //}

    [System.Serializable]
    public class MirrorPlayer
    {
        public string playerID;
        public PlayerManager playerManager;
    }

}


