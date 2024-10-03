using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace TP
{

    [System.Serializable]
    public class MirrorRoomInfo
    {
        public string roomName;
        public string lobbyName;
        public bool isClosed = false;
        public bool isRoomFull = false;
        public bool isVisible = true;
        public GameMode gameType;
        public List<MirrorPlayer> players = new List<MirrorPlayer>();
        public GameState gameState;
        public int maxPlayerCount = 5;
        public NetworkIdentity gameManagerID;

        public int GetPlayerCount()
        {
            int playerCount = 0;
            foreach(MirrorPlayer mp in players)
            {
                //if (!mp.playerManager.isBot)
                    playerCount++;
            }
            return playerCount;
        }

        public int GetRealPlayerCount()
        {
            int playerCount = 0;
            foreach (MirrorPlayer mp in players)
            {
            if (!mp.playerManager.isBot)
                playerCount++;
            }
            return playerCount;
        }


        public int GetBotCount()
        {
            int BotCount = 0;
            foreach (MirrorPlayer mp in players)
            {
                if (mp.playerManager.isBot)
                    BotCount++;
            }
            return BotCount;
        }

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


