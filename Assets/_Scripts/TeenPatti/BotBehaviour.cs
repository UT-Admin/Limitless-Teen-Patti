using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace TP
{


    [System.Serializable]
    public class BotBehaviour : MonoBehaviour
    {
        public static BotBehaviour instance;
        public List<string> rankDetails = new List<string>();
        public CardsCombination botCardCombination;
        public static bool isTurnCompleted;
        public double lastActive;
        GameManager gm;
        private void Awake()
        {

            instance = this;
        }
        #region BotMirror
        public int SetBotActionMirror(GameManager manager)
        {
            gm = manager;
            gm.isBotIncressBet = false;
            rankDetails = gm.playerRankDetails;
            return BotActivities();
        }
        int BotActivities()
        {

            if (!gm.isBotWin)
            {
                bool isMultipleBot = gm.GetBotCountInGame() > 1;
                int minTurnCountForSeeCard = 2;
                int minTurnCountForPlay = 3;
                if (isMultipleBot)
                {
                    foreach (string playerId in rankDetails)
                    {
                        if (gm.GetPlayerState(playerId).playerData.isBot)
                        {
                            if (playerId == gm.GetCurrentPlayingPlayerID())
                            {
                                minTurnCountForPlay = 2;
                                minTurnCountForSeeCard = 1;
                            }
                            else
                            {
                                minTurnCountForPlay = 4;
                                minTurnCountForSeeCard = 3;
                            }
                            break;
                        }
                    }
                }
                if (gm.gameState.players[gm.GetCurrentPlayingPlayerIndex()].hasSeenCard)

                {
                    if (gm.gameState.players[gm.GetCurrentPlayingPlayerIndex()].turnCount >= minTurnCountForPlay)
                    {
                        return 1;
                    }
                    else if (UnityEngine.Random.Range(1, 3) == 3)
                    {
                        return 1;
                    }
                    else
                    {
                        return 2;
                    }
                }
                else
                {
                    if (gm.gameState.players[gm.GetCurrentPlayingPlayerIndex()].turnCount >= minTurnCountForSeeCard)
                    {
                        gm.BotSeeCards();
                        if (UnityEngine.Random.Range(1, 3) == 3)
                        {
                            return 1;
                        }
                        else
                        {
                            return 2;
                        }
                    }
                    else
                    {
                        if (UnityEngine.Random.Range(1, 5) == 3)
                        {
                            gm.BotSeeCards();
                            if (UnityEngine.Random.Range(1, 3) == 3)
                            {
                                return 1;
                            }
                            else
                            {
                                return 2;
                            }
                        }
                        else
                        {
                            return 2;
                        }
                    }
                }

            }

            else if (gm.gameState.players[gm.GetCurrentPlayingPlayerIndex()].playerData.isBot)
            {

                if (!gm.gameState.players[gm.GetCurrentPlayingPlayerIndex()].hasSeenCard && gm.gameState.currentState == 2)
                {
                    int rand = UnityEngine.Random.Range(1, 5);
                    if (rand == 3 || (gm.gameState.players[gm.GetCurrentPlayingPlayerIndex()].turnCount > 3 && (gm.gameState.players[gm.GetCurrentPlayingPlayerIndex()].playerData.playerID != rankDetails[0])))
                    {
                        gm.BotSeeCards();
                        DebugHelper.LogError("bot play 4");
                        return PackOrChallBot();
                    }
                    else
                    {
                        DebugHelper.LogError("bot play 5");
                        return 2;
                    }
                }
                else
                {
                    if (/*gm.gameController.CurrentGameMode == GameMode.ZANDU && */gm.gameState.players[gm.GetCurrentPlayingPlayerIndex()].turnCount <= 3)
                    {
                        return 2;
                    }
                    else
                    {
                        DebugHelper.LogError("bot play 7");
                        return PackOrChallBot();
                    }
                }
            }
            DebugHelper.LogError("bot play 8");
            return 1;
        }
        int PackOrChallBot()
        {
            if (gm.gameState.players[gm.GetCurrentPlayingPlayerIndex()].playerData.playerID != gm.botPlayer.rankDetails[0])
            {
                if ((/*gm.gameController.CurrentGameMode == GameMode.MUFLIS && */botCardCombination == CardsCombination.HighCard || botCardCombination == CardsCombination.Pair) || /*(gm.gameController.CurrentGameMode != GameMode.MUFLIS && */(botCardCombination == CardsCombination.Trail || botCardCombination == CardsCombination.StraightFlush))
                {
                    if (UnityEngine.Random.Range(1, 3) == 2)
                    {
                        if (UnityEngine.Random.Range(1, 5) == 3)
                            gm.isBotIncressBet = true;
                        return 2;
                    }
                    else
                        return 1;
                }
                else
                {
                    int rand = UnityEngine.Random.Range(1, 5);
                    if (rand != 3)
                    {
                        if (UnityEngine.Random.Range(1, 4) == 2)
                            gm.isBotIncressBet = true;
                        return 2;
                    }
                    else
                    {
                        return 1;
                    }
                }
            }
            else
            {
                if (UnityEngine.Random.Range(1, 3) == 2)
                    gm.isBotIncressBet = true;
                return 2;
            }
        }

        #endregion

        public void SetBotAction(bool isforce)
        {
            //Debug.LogError("bot action");
            //if ((isTurnCompleted || lastActive + 7 < PhotonNetwork.Time) && gm.myBot.GetPlayerID() == gm.GetCurrentPlayingPlayerID())
            //{
            //    lastActive = PhotonNetwork.Time;
            //    isTurnCompleted = false;

            //    StartCoroutine(BotActivities(isforce));

            //}
            //else if (isforce && gm.myBot.GetPlayerID() == gm.GetCurrentPlayingPlayerID())
            //{
            //    StopBotAction();
            //    lastActive = PhotonNetwork.Time;
            //    isTurnCompleted = false;
            //    StartCoroutine(BotActivities(isforce));
            //}
        }
        bool isOnlyChall;


        //IEnumerator BotActivities(bool isforce)
        //{
        //    isOnlyChall = false;
        //    if (!isforce)
        //        yield return new WaitForSeconds(Random.Range(2, 4));
        //    else
        //    {
        //        double delay = 0;
        //        PlayerState ps = gm.gameState.players[gm.GetCurrentPlayingPlayerIndex()];
        //        if (ps.playerData.isBot)
        //        {
        //            delay = NetworkTime.time - ps.myTurnTime - 10;
        //            if (delay < 0)
        //                delay = 0;
        //            if (delay > 15)
        //                delay = 15;
        //        }
        //        if (delay > 10)
        //            isOnlyChall = true;
        //    }
        //    if (gm.GetCurrentGameState() != 2)
        //        yield break;
        //    if (isOnlyChall)
        //    {
        //        StartCoroutine(OnChaal());
        //    }
        //    else if (gm.gameState.players[gm.GetCurrentPlayingPlayerIndex()].playerData.isBot)
        //    {
        //        if (!gm.gameState.players[gm.GetCurrentPlayingPlayerIndex()].hasSeenCard && gm.gameState.currentState == 2)
        //        {
        //            int rand = Random.Range(1, 5);
        //            if (rand == 3 || (gm.gameState.players[gm.GetCurrentPlayingPlayerIndex()].turnCount > 3 && (gm.gameState.players[gm.GetCurrentPlayingPlayerIndex()].playerData.playerID != rankDetails[0])))
        //            {
        //                gm.BotSeeCards();
        //                PackOrChall();
        //            }
        //            else
        //            {
        //                StartCoroutine(OnChaal());
        //            }
        //        }
        //        else
        //        {
        //            if (GameController.Instance.CurrentGameMode == GameMode.ZANDU && gm.gameState.players[gm.GetCurrentPlayingPlayerIndex()].turnCount <= 3)
        //            {
        //                StartCoroutine(OnChaal());
        //            }
        //            else
        //            {
        //                PackOrChall();
        //            }
        //        }
        //    }
        //}

        //public void PackOrChall()
        //{
        //    if (gm.gameState.players[gm.GetCurrentPlayingPlayerIndex()].playerData.playerID != rankDetails[0])
        //    {
        //        if ((GameController.Instance.CurrentGameMode == GameMode.MUFLIS && botCardCombination == CardsCombination.Trail || botCardCombination == CardsCombination.StraightFlush) || (GameController.Instance.CurrentGameMode != GameMode.MUFLIS && botCardCombination == CardsCombination.Pair || botCardCombination == CardsCombination.HighCard))
        //        {
        //            StartCoroutine(OnPack());
        //        }
        //        else
        //        {
        //            int rand = Random.Range(1, 3);
        //            if (rand != 3)
        //            {
        //                StartCoroutine(OnPack());
        //            }
        //            else
        //            {
        //                StartCoroutine(OnChaal());
        //            }
        //        }
        //    }
        //    else
        //    {
        //        StartCoroutine(OnChaal());
        //    }
        //}
        //IEnumerator OnChaal()
        //{
        //    while (!isTurnCompleted)
        //    {
        //        if (PhotonNetwork.IsMasterClient)
        //        {
        //           DebugHelper.Log("bot chaal");
        //            gm.VerifyAndChaal(true);
        //        }
        //        yield return new WaitForSeconds(5);
        //    }
        //}
        //IEnumerator OnPack()
        //{
        //    //Debug.Log("bot pack");
        //    //gm.VerifyAndPack();
        //    //yield return null;
        //    while (!isTurnCompleted)
        //    {
        //        if (PhotonNetwork.IsMasterClient)
        //        {
        //           DebugHelper.Log("bot chaal");
        //            gm.VerifyAndPackBot();
        //        }
        //        yield return new WaitForSeconds(5);
        //    }
        //}
        public void StopBotAction()
        {
            StopAllCoroutines();
        }
        //private void OnApplicationPause(bool pause)
        //{
        //    if (pause)
        //        Time.timeScale = 0; 
        //    else
        //        Time.timeScale = 1;
        //}
    }

}