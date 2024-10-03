using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TP
{
    [System.Serializable]
    public class PlayerStateDetails
    {
        public List<PlayerState> waitingPlayers = new List<PlayerState>();
        public List<string> removedPlayers = new List<string>();
    }
}