using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Mirror;
using DG.Tweening;
using UnityEditor;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using System.Text;

namespace TP
{

    public class GameManager : NetworkBehaviour
    {
        [Header("======================================================================")]

        [Header("Bools")]
        public bool isBotIncressBet;
        public bool isWinnerDisplayed = false;
        public bool IsSideShowChecked = false;
        public bool isSideshow = false;
        public bool isdistributCard = false;
        public bool isdistributCardEnd = false;
        public bool isAutoSee = false;
        public bool isBotWin = true;
        public bool isPlayerstateInitialized;


        bool isShowNoti = false;
        public bool isrejoin;
        bool istournamentend;
        bool IsFirstBet = false;
        bool gameStart = false;
        bool isShowWinner = false;
        bool isBotNotInGame = false;
        bool isAddingBot = false;
        bool BackGroundMusic = false;
        bool IsSideShowWinnerAnimationDisplay = false;
        bool IsShowWinnerAnimationDisplay = false;

        [Header("======================================================================")]

        [Header("Integers")]
        public int betIndex;
        public int betIndexBot;
        public int botLimit = 1;
        public int botCount;
        public int CountTemp;

        int playingPlayerIndex;
        int playercount = 0;
        int currentChaalAmount;
        int botid = 0;


        [Header("======================================================================")]

        [Header("String")]
        public string myPlayerID;
        public string previousPlayerId;
        public string[] roomplayerid;

        string coinSpriteAsset = " <sprite index= 5> ";
        string betcoinImage = "";



        [Header("======================================================================")]

        [Header("Double")]
        public double refreshTime = 0;
        public double refreshGameTime = 0;


        [Header("======================================================================")]

        [Header("Singleton")]
        public static GameManager localInstance;

        [Header("======================================================================")]

        [Header("List")]
        public List<GameObject> botPlayers;
        public List<PlayerManager> playerManagersList = new List<PlayerManager>();
        [SerializeField] List<SWEvent> requestList = new List<SWEvent>();

        [Header("======================================================================")]

        [Header("Struct")]
        public PlayerData myPlayerData;
        public PlayerData botAPI = new();

        [Header("======================================================================")]

        [Header("Call_Referance_Variables")]
        public BotBehaviour botPlayer = new BotBehaviour();
        public TeenPattiTableModel currentTableModel;
        public DeluxeTeenPattiTableModel currentTableModelD;
        public MirrorRoomInfo roomInfo;
        public GameState gameState = new GameState();
        public PlayerState myPlayerState;
        public PlayerManager myPlayer;
        PlayerState playingPlayerState;


        [Header("======================================================================")]

        [Header("Coroutine")]

        [SerializeField] Coroutine playerTurnTimerRoutine;
        [SerializeField] Coroutine botPlay;


        public delegate void Callback();

        [Header("======================================================================")]

        [Header("Structs")]
        public UIDETAILS[] playerUiPosition;


        public GameController UIGameController;


        [Header("======================================================================")]

        [Header("NetworkVariables")]
        public NetworkMatch networkMatch;
        [SyncVar(hook = nameof(OnGameStateChanged))] public string gameStateJson;
        [SyncVar] public List<string> playerRankDetails = new List<string>();
        [SyncVar] public TeenPattiGameData gameController = new TeenPattiGameData();
        [SyncVar] public int AutoGamePlayCount;
        [SyncVar(hook = nameof(EnableTimerAnimation))] public int GameStartCountDown;





        [Serializable]
        public struct UIDETAILS
        {
            public int[] playerIndex;
        }

        [System.Serializable]
        public struct CardDetails
        {
            public CardData[] currentCards;
            public CardsCombination currentCombination;
        }

        [System.Serializable]
        public struct TableInfo
        {
            public int blindLimit, chalLimit, potLimit, startBoot;
        }

        private void Awake()
        {
            localInstance = this;
        }

        private void Start()
        {
            UIGameController = GameController.Instance;

        }
        void OnGameStateChanged(string oldStr, string newStr)
        {
            gameState = JsonUtility.FromJson<GameState>(newStr);

            if (isClient)
            {
                UpdateNetworkGame();
            }

        }

        public void InitCommon()
        {
            currentTableModel = gameController.CurrentGameModelTable;
        }

        public void StartPrivateGame()
        {
            foreach (PlayerState ps in gameState.players)
            {
                ps.currentState = 1;
                if (ps.cardStrength == -1)
                {
                    ps.playerData.currentCards = GetCards();
                    ps.playerData.currentCombination = CardCombination.GetCombinationFromCard(ps.playerData.currentCards);
                    ps.cardStrength = (int)ps.playerData.currentCombination;
                }
            }
            gameState.gameStartTime = NetworkTime.time;
            DebugHelper.Log("start game timer");
            gameState.currentState = 1;
            UpdateGameStateToServer();
        }

        public void StartGame()
        {
            DebugHelper.LogError("start game timer");
            gameState.isDealCard = true;
            gameState.SetCurrentStake(currentTableModel.BootAmount);
            foreach (PlayerState ps in gameState.players)
            {
                if (!ps.isSpectator)
                {
                    CardDetails currentCards;
                    currentCards.currentCards = ps.playerData.currentCards;
                    currentCards.currentCombination = ps.playerData.currentCombination;
                }
            }

            if (gameController.IsTournament)
            {
                if (gameController.CurrentTournamentModel.IsAllIn)
                {
                    DebugHelper.LogError("game state update 4");
                    gameState.currentState = 4;
                    return;
                }
            }

            if (gameState.currentState != 3)
            {
                gameState.currentState = 2;
            }
            InitPlayerTurn();
        }

        public void EnterGame()
        {
            CommonFunctions.Instance.SetLastEnteredRoom(roomInfo.roomName, roomInfo.lobbyName);
        }

        public void CheckJoinStatus(bool isRejoin)
        {

            isrejoin = isRejoin;
            refreshGameTime = NetworkTime.time;
            InitGame();

            if (roomInfo.roomName == null)
                Exit();



            if (gameState.currentState == 2)
            {

                GamePlayUI.instance.potAmount.text = CommonFunctions.Instance.TpvAmountSeparator(gameState.totalPot) + " " + $"<size=25>{APIController.instance.authentication.currency_type}</size>";


                if (GetCurrentPlayingPlayerID() == UIGameController.CurrentPlayerData.GetPlayfabID())
                    GamePlayUI.instance.ShowHud();
            }
            if (gameState.currentState == 2 || gameState.currentState == 3)
            {
                GamePlayUI.instance.potAmountPannel.SetActive(true);

                GamePlayUI.instance.potAmount.text = CommonFunctions.Instance.TpvAmountSeparator(gameState.totalPot) + " " + $"<size=25>{APIController.instance.authentication.currency_type}</size>";


            }

            StartCoroutine(GamePlayUI.instance.setStrengthMeter(GetPlayerState(myPlayerID).cardStrength));
            StopCoroutine(nameof(RefreshGame));
            StartCoroutine(nameof(RefreshGame));
            UpdateGameWithDelay();

            GameManager.localInstance.InitTableData();

            isShowNoti = true;
        }

        IEnumerator RefreshGame()
        {
            yield return new WaitForSeconds(2);
            if (gameState.currentState == 2 && gameState.players.Exists(x => x.playerData.playerID == UIGameController.CurrentPlayerData.GetPlayfabID() && !x.hasPacked && !x.isSpectator))
            {
                if (GamePlayUI.instance.bottomTemp.active)
                    UpdateNetworkGame();
            }

        }

        public void TableValues()
        {
            GamePlayUI.instance.TeenPattiInfoBootAmount.text = (currentTableModel.BootAmount) == -1 ? "No Limits" : currentTableModel.BootAmount.ToString();
            GamePlayUI.instance.TeenPattiInfoBlindLimit.text = currentTableModel.BlindLimit.ToString();
            GamePlayUI.instance.TeenPattiInfoChallLimit.text = (currentTableModel.ChaalLimit) == -1 ? "No Limits" : (currentTableModel.ChaalLimit * 2).ToString();
            GamePlayUI.instance.TeenPattiInfoPotLimit.text = (currentTableModel.PotLimit) == -1 ? "No Limits" : currentTableModel.PotLimit.ToString();

        }

        public void InitTableData()
        {




            currentTableModel = (TeenPattiTableModel)UIGameController.CurrentGameModelTable;

            switch (UIGameController.CurrentGameMode)
            {

                /* case GameMode.AK47:
                     TableValues();
                     break;*/
                case GameMode.NOLIMITS:
                    if (UIGameController.CurrentGameModelTable is DeluxeTeenPattiTableModel)
                    {
                        GamePlayUI.instance.TeenPattiInfoBootAmount.text = CommonFunctions.Instance.GetAmountDecimalSeparator(currentTableModel.BootAmount);
                        GamePlayUI.instance.TeenPattiInfoBlindLimit.text = "No Limits";
                        GamePlayUI.instance.TeenPattiInfoChallLimit.text = "No Limits";
                        GamePlayUI.instance.TeenPattiInfoPotLimit.text = "No Limits";
                    }
                    else
                    {

                        GamePlayUI.instance.TeenPattiInfoBootAmount.text = CommonFunctions.Instance.GetAmountDecimalSeparator(currentTableModel.BootAmount);
                        GamePlayUI.instance.TeenPattiInfoBlindLimit.text = "No Limits";
                        GamePlayUI.instance.TeenPattiInfoChallLimit.text = "No Limits";
                        GamePlayUI.instance.TeenPattiInfoPotLimit.text = "No Limits";

                    }
                    break;
                    /* case GameMode.POTBLIND:
                         GamePlayUI.instance.TeenPattiInfoBootAmount.text = CommonFunctions.Instance.GetAmountDecimalSeparator(currentTableModel.BootAmount);
                         GamePlayUI.instance.TeenPattiInfoBlindLimit.text = "Always Blind";
                         GamePlayUI.instance.TeenPattiInfoChallLimit.text = "Always Chaal";
                         GamePlayUI.instance.TeenPattiInfoPotLimit.text = CommonFunctions.Instance.GetAmountDecimalSeparator(currentTableModel.PotLimit, false, 0);
                         break;*/
                    /* case GameMode.MUFLIS:
                         if (UIGameController.CurrentGameModelTable is DeluxeTeenPattiTableModel)
                         {
                             TableValues();
                         }
                         else
                         {
                             TableValues();
                         }
                         break;*/
                    /*case GameMode.JOKER:
                        if (UIGameController.CurrentGameModelTable is DeluxeTeenPattiTableModel)
                        {
                            TableValues();
                        }
                        else
                        {
                            TableValues();


                        }
                        break;
                    case GameMode.HUKAM:
                        if (UIGameController.CurrentGameModelTable is DeluxeTeenPattiTableModel)
                        {
                            TableValues();
                        }
                        else
                        {
                            TableValues();

                        }
                        break;
                    case GameMode.NOLIMITS:
                        if (UIGameController.CurrentGameModelTable is DeluxeTeenPattiTableModel)
                        {
                            GamePlayUI.instance.TeenPattiInfoBootAmount.text = CommonFunctions.Instance.GetAmountDecimalSeparator(currentTableModel.BootAmount);
                            GamePlayUI.instance.TeenPattiInfoBlindLimit.text = "No Limits";
                            GamePlayUI.instance.TeenPattiInfoChallLimit.text = "No Limits";
                            GamePlayUI.instance.TeenPattiInfoPotLimit.text = "No Limits";
                        }
                        else
                        {

                            GamePlayUI.instance.TeenPattiInfoBootAmount.text = CommonFunctions.Instance.GetAmountDecimalSeparator(currentTableModel.BootAmount);
                            GamePlayUI.instance.TeenPattiInfoBlindLimit.text = "No Limits";
                            GamePlayUI.instance.TeenPattiInfoChallLimit.text = "No Limits";
                            GamePlayUI.instance.TeenPattiInfoPotLimit.text = "No Limits";

                        }
                        break;
                    case GameMode.ZANDU:
                        if (UIGameController.CurrentGameModelTable is DeluxeTeenPattiTableModel)
                        {
                            TableValues();
                        }
                        else
                        {
                            TableValues();
                        }
                        break;

                    default:
                    case GameMode.CLASSIC:
                        if (UIGameController.CurrentGameModelTable is DeluxeTeenPattiTableModel)
                        {
                            TableValues();
                        }
                        else
                        {


                                TableValues();

                        }
                        break;
    */
            }

        }

        public void LinkPlayerManagerToUI(bool repositionUI = false)
        {
            if (repositionUI)
            {
                foreach (PlayerUI ui in GamePlayUI.instance.playerUIDetails)
                {
                    if (string.IsNullOrWhiteSpace(ui.playerID))
                        ui.ClearUI();
                    else if (ui.playerID == GetPlayerState(ui.playerID).playerData.playerID)
                    {
                        ui.ClearUI(true);
                    }
                    else
                    {
                        ui.ClearUI();
                    }
                }
            }

            foreach (PlayerManager player in playerManagersList)
            {
                StartCoroutine(player.InitPlayerManager(repositionUI));
            }
            if (!string.IsNullOrWhiteSpace(GetPlayerState(UIGameController.CurrentPlayerData.GetPlayfabID()).playerData.playerID))
            {
                foreach (PlayerState ps in gameState.players)
                {
                    if (!playerManagersList.Exists(x => x.playerID == ps.playerData.playerID))
                    {
                        PlayerUI ui = GetUI(ps.playerData.playerID);
                        ui.UpdateStatus(ps);
                    }
                }
                foreach (PlayerState ps in gameState.waitingPlayers)
                {
                    if (!playerManagersList.Exists(x => x.playerID == ps.playerData.playerID))
                    {
                        PlayerUI ui = GetUI(ps.playerData.playerID);
                        ui.UpdateStatus(ps);
                    }
                }
            }
            else
            {

            }
        }

        public void AddPlayersToWaitingList(PlayerState newPlayerState)
        {
            DebugHelper.LogError("Player added rpc");
            newPlayerState.playerData.currentCards = new CardData[3];
            if (!gameState.waitingPlayers.Exists(x => x.playerData.playerID == newPlayerState.playerData.playerID) && !gameState.players.Exists(x => x.playerData.playerID == newPlayerState.playerData.playerID))
            {
                newPlayerState.ui = getUiIndex();
                gameState.waitingPlayers.Add(newPlayerState);
            }
            else
            {
                DebugHelper.LogError("Gamestate already added");
                DebugHelper.LogError(GetPlayerState(newPlayerState.playerData.playerID) + "     " + newPlayerState);
            }
            gameState.removedPlayers.Remove(newPlayerState.playerData.playerID);
            UpdateGameStateToServer();
        }



        

        public void AddPlayerManager(PlayerManager pm)
        {
            if (pm.networkMatch.matchId != networkMatch.matchId)
                return;
            playerManagersList = playerManagersList.Where(x => x != null).ToList();
            if (!playerManagersList.Contains(pm))
                playerManagersList.Add(pm);
        }



        public void SubtractFromPotAmount()
        {
            gameState.totalPot -= currentTableModel.BootAmount;
            UpdateGameStateToServer();
        }


        public void RemovePlayerManager(PlayerManager pm)
        {
            if (pm.networkMatch.matchId != networkMatch.matchId)
                return;
            playerManagersList = playerManagersList.Where(x => x != null).ToList();
            if (playerManagersList.Contains(pm))
                playerManagersList.Remove(pm);
        }

        public void ClearGameUI()
        {
            GamePlayUI.instance.sideShowAlertPannel.SetActive(false);
            GamePlayUI.instance.sideShowPannel.SetActive(false);
        }

        public void Exit(bool isforce = true)
        {
            if (roomInfo.roomName == null)
            {
                UIGameController.isInGame = false;
                gameState = new GameState();
                ClearGameUI();
            }
            if (GetPlayerState(myPlayerID).playerData.playerID != myPlayerID)
            {
                UIGameController.isInGame = false;
                gameState = new GameState();
                ClearGameUI();

            }
            //DebugHelper.LogError("game exit *********");

            //if ((gameState.currentState == 1 || (!gameState.isDealCard && gameState.currentState != 0)))
            //    return;
            OnExitLeaveRoomToMenu(isforce);

        }

        async void OnExitLeaveRoomToMenu(bool isforce = true)
        {

            PlayerUI playerUi = GetPlayerUI(myPlayerID);
            if (playerUi != null)
                playerUi.SetSpectator(false);
            RemoveMeFromGame(isforce);

        }

        public void RejoinGame(string playerID)
        {
            SetPlayerReconnected(playerID);
            UpdateGameStateToServer();
        }

        public void SetWinner(bool isWinner)
        {
            isBotWin = isWinner;
            DebugHelper.Log("init bet response :::::::----::: Server >>>>  " + isBotWin);
        }

        public void SetPlayerReconnected(string playerID)
        {
            gameState.removedPlayers.Remove(playerID);
            GetPlayerState(playerID).disconnectTime = -1;
        }

        public void RefreshMyPlayerState()
        {
            if (gameState.players.Exists(x => x.playerData.playerID == myPlayerID))
                myPlayerState = gameState.players.Find(x => string.Equals(x.playerData.playerID, myPlayerData.playerID));
            else if (gameState.waitingPlayers.Exists(x => x.playerData.playerID == myPlayerID))
                myPlayerState = gameState.waitingPlayers.Find(x => string.Equals(x.playerData.playerID, myPlayerData.playerID));
            else if (myPlayer)
                StartCoroutine(myPlayer.WaitForPlayerInGameState());
        }

        void RemoveCompletedEvents()
        {
            foreach (string reqID in gameState.completedRequests)
                RemoveServerEvent(reqID);

        }

        public void ToggleSpectatorUsers()
        {
            gameState.waitingPlayers.AddRange(gameState.players.Where(x => x.isSpectator).ToList());
            gameState.players.RemoveAll(x => x.isSpectator);
        }

        public bool FindPlayerInGame(string id)
        {
            foreach (PlayerState ps in gameState.players)
                if (ps.playerData.playerID == id)
                    return true;
            foreach (PlayerState ps in gameState.waitingPlayers)
                if (ps.playerData.playerID == id)
                    return true;

            return false;
        }

        IEnumerator CheckForAddExtraBot()
        {
            while (true)
            {
                yield return new WaitForSeconds(UnityEngine.Random.Range(1, 3));
                if (GetBotCountInGame() < botLimit && (gameState.players.Count + gameState.waitingPlayers.Count) <= 4 && UnityEngine.Random.Range(1, 3) == 2)
                {
                    Debug.Log("Chek The Bot added Concept======>1");
                    StartCoroutine(AddBotIfRequired());
                }
                if (gameState.currentState != 0 && gameState.currentState != 1)
                {
                    yield return new WaitForSeconds(UnityEngine.Random.Range(5, 10));
                }
            }
        }

        public IEnumerator UpdateGameWithDelay()
        {
            GamePlayUI.instance.isHUDShown = false;
            yield return new WaitForSeconds(1);
            UpdateNetworkGame();
        }
        double lastDealingTime = 0;

        IEnumerator callShowWinnerWithDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            StartCoroutine(nameof(ShowWinner));
        }

        public void StartPrivateTable()
        {
            if (gameState.GameCreatePlayerID == myPlayerID)
            {
                if (gameState.players.Count > 1)
                {

                    myPlayer.StartPrivateGame();
                }
            }
            else
            {
                return;
            }

        }

        void ForceCheckTurn()
        {
            bool turnAssigned = false;
            for (int i = 0; i < gameState.players.Count; i++)
            {
                if (gameState.players[i].isMyTurn)
                {
                    turnAssigned = true;
                    continue;
                }
                if (turnAssigned && gameState.players[i].playerData.playerID == myPlayerID && GetCurrentGameState() == 2)
                    CheckMyTurn();
            }

        }

        void AutoSee()
        {
            isAutoSee = true;
            if (gameState.currentState == 2 && !myPlayerState.isSpectator && !myPlayerState.hasPacked)
            {
                if (myPlayer)
                {
                    GamePlayUI.instance.SeeButtonActive(false);
                    StartCoroutine(myPlayer.myUI.SetCard());
                }
            }


        }

        public void InactiveUserKick(string playerid, string message)
        {
            PlayerState ps = GetPlayerState(playerid);
            RemoveFromGame(playerid, false);
            UpdateGameStateToServer();
            DebugHelper.Log(playerid + "---- kicked by server " + message);
            DebugHelper.Log(GetPlayerPUN(playerid).playerID + "----");

            if (GetPlayerPUN(playerid).playerID == playerid)
                GetPlayerPUN(playerid).ServerKick(message);

        }

        public void OnStandUpUser()
        {
            if (myPlayerState.isSpectator) return;
            GetPlayerUI(myPlayerID).ResetCard();
            GamePlayUI.instance.ResetZanthuCards();
            GamePlayUI.instance.HideHud();
            myPlayer.StandUpCMD(myPlayerID);
            myPlayer.myUI.UpdateStatus(myPlayerState);
            myPlayer.myUI.SetSpectator(true);
            myPlayer.myUI.SetInfoText();
        }

        public void OnPlayerSit()
        {
            myPlayer.SitCMD(myPlayerID);
            myPlayer.myUI.SetSpectator(false);
        }

        public void GlobalMessage(string _globalInfo)
        {
            if (isClient)
                GlobalMessageServer(_globalInfo);
            else
                GlobalMessageClient(_globalInfo);
        }

        public void CheckAutoSee()
        {
            if (gameState.forceSee == true && isAutoSee == false && gameState.isDealCard)
            {
                GamePlayUI.instance.GlobalMessage("Blind limit is reached");
                AutoSee();
            }

        }

        void UpdatePlayingPlayer()
        {
            for (int i = 0; i < gameState.players.Count; i++)
            {
                if (gameState.players[i].isMyTurn)
                {
                    playingPlayerIndex = i;
                    playingPlayerState = gameState.players[i];
                }
            }

        }



        public void StartBubbleLoop()
        {


            /*float moveDuration = 2f;
            float fadeDuration = 0.5f;
            Vector3 startPosition = new Vector3(0, 0, 0);
            Vector3 targetPosition = new Vector3(0, 50f, 0);

            // Reference the Image component
            Image bubbleImage = GamePlayUI.instance.ChaalBubbles.GetComponent<Image>();

            GamePlayUI.instance.ChaalBubbles.transform.DOLocalMove(targetPosition, moveDuration).OnComplete(() =>
            {
                Debug.Log("Bubbles===========>1");

                // Fade out by changing the alpha of the Image color
                bubbleImage.DOFade(0, fadeDuration).OnComplete(() =>
                {
                    Debug.Log("Bubbles===========>2");
                    // Set inactive
                    GamePlayUI.instance.ChaalBubbles.SetActive(false);
                    Debug.Log("Bubbles===========>3");
                    // Reset local position
                    GamePlayUI.instance.ChaalBubbles.transform.localPosition = startPosition;
                    Debug.Log("Bubbles===========>4");
                    // Set active
                    GamePlayUI.instance.ChaalBubbles.SetActive(true);
                    Debug.Log("Bubbles===========>5");
                    // Fade in by restoring the alpha of the Image color
                    bubbleImage.DOFade(1, fadeDuration).OnComplete(() =>
                    {
                        Debug.Log("Bubbles===========>6");
                        // Restart the loop
                        StartBubbleLoop();
                    });
                });
            });*/




        }



        public void CheckMyTurn()
        {
            if (!gameState.players.Exists(x => x.playerData.playerID == myPlayerID))
                return;

            PlayerState mystate = GetPlayerState(myPlayerID);
            if (GetPlayerUI(myPlayerID).playerID == myPlayerID)
                GetPlayerUI(myPlayerID).UpdateStatus(mystate);

            if (mystate.isMyTurn)
            {
                if (GetPlayerUI(mystate.playerData.playerID).isMine)
                {
                    GamePlayUI.instance.ButtonHolder.SetActive(true);

                    StartBubbleLoop();


                }

                Debug.Log("CheckMyTurn======================> " + GamePlayUI.instance.SoundCheck);
                if (GamePlayUI.instance.SoundCheck == true)
                {
                    //if (MasterAudioController.instance.CheckSoundToggle())
                    //    MasterAudioController.instance.PlayAudio(AudioEnum.TURN);
                }
                else
                {
                    GamePlayUI.instance.SoundCheck = true;
                }


                if (mystate.turnCount > 3 && !mystate.hasSeenCard)
                {
                    mystate.hasSeenCard = true;
                    Debug.Log($"Check the Auto Trun Cards");
                    GamePlayUI.instance.OnSeeClicked();
                }

                gameState.isSideShowRequestSend = false;
                GamePlayUI.instance.UpdateChaalText(mystate.currentBoot);

               
                    if (mystate.hasSeenCard)
                    {

                    GamePlayUI.instance.challText.text = "CHAAL";
                    }
                    else
                    {
                    GamePlayUI.instance.challText.text = "BLIND";
                    }
                






                GamePlayUI.instance.ShowHud();


            }
            else
            {
                GamePlayUI.instance.ButtonHolder.SetActive(false);
                GamePlayUI.instance.HideHud();
               
                    //GamePlayUI.instance.MyProfileGlow.gameObject.SetActive(false);
              
            }
        }

        public void OpenCardForShow()
        {
            foreach (PlayerState ps in gameState.players)
            {
                ps.SetCardsSeen();
            }
        }

        public void CheckPlayerShowCard()
        {
            foreach (PlayerState ps in gameState.players)
            {
                if (ps.isCardShow && ps.currentState == 3 && GetPlayerState(myPlayerID).currentState == 3)
                {
                    DebugHelper.Log("--vs--" + ps.playerData.playerName);
                    StartCoroutine(GamePlayUI.instance.playerUIDetails.Find(x => x.playerID == ps.playerData.playerID).SetCard());
                }
                else if (ps.isCardShow && ps.currentState == 3)
                {
                    //GamePlayUI.instance.sideShowAlertPannel.SetActive(false);
                }
            }
        }

        void CheckSideShowRequest()
        {
            if (IsSideShowChecked == false)
            {
                IsSideShowChecked = true;
                if (gameState.players[gameState.Receiverval].playerData.playerID == myPlayerID)
                {
                    GamePlayUI.instance.StartSideShow();
                }
                else
                {
                    GamePlayUI.instance.StartSideShowUser(gameState.Senderval, gameState.Receiverval);
                }
            }
        }

        public void NextPlayerTurn(int currentPlayer, bool update = true)
        {
            if (gameState.currentState != 2)
                return;
            isSideshow = false;

            DebugHelper.Log("check nextplayer turn");
            double delay = 0;
            if (GetActivePlayedPlayerCount() <= 1)
            {
                DebugHelper.LogError("game state update 4");

                gameState.currentState = 4;
                UpdateGameStateToServer();
                return;
            }

            PlayerState ps = gameState.players[GetCurrentPlayingPlayerIndex()];
            if (ps.playerData.isBot)
            {
                delay = NetworkTime.time - ps.myTurnTime - 10;
                if (delay < 0)
                    delay = 0;
                if (delay > 15)
                    delay = 15;
            }
            int index = NextPlayerIndex(currentPlayer);
            gameState.players[index].SetMyTurn(true);
            if (update)
                UpdateGameStateToServer();
            if (playerTurnTimerRoutine != null) StopCoroutine(playerTurnTimerRoutine);
            playerTurnTimerRoutine = StartCoroutine(StartPlayerTimer(gameState.players[index].playerData.playerID));
        }

        public void OnDeclineSideShow(int currentPlayer, int myPlayerId)
        {
            myPlayer.DenailSideShowCMD(currentPlayer, myPlayerId);

        }

        public void OnAcceptSideShow(int firstPlayerId, int botPlayerId)
        {
            if (!isSideshow)
            {
                myPlayer.AcceptSideShowCMD(firstPlayerId, botPlayerId);
            }
        }

        public void PackPlayerServer(string playerID)
        {
            PlayerState ps = GetPlayerState(playerID);
            ps.hasPacked = true;
            DebugHelper.LogError("player packed check");
            DebugHelper.LogError("player packed check 1");
            GlobalMessage((CommonFunctions.Instance.GetTruncatedPlayerName(ps.playerData.playerName) + " has Packed."));
            NextPlayerTurn(true);
        }

        public void PackPlayerServer(string playerID, bool isUpdate)
        {
            PlayerState ps = GetPlayerState(playerID);
            ps.hasPacked = true;
            DebugHelper.LogError("player packed");
            GlobalMessage((CommonFunctions.Instance.GetTruncatedPlayerName(ps.playerData.playerName) + " has Packed."));
            if (GetActivePlayedPlayerCount() <= 1)
            {

                gameState.currentState = 4;
                if (isUpdate)
                    UpdateGameStateToServer();
                return;
            }
            else if (GetCurrentPlayingPlayerID() == playerID)
                NextPlayerTurn(isUpdate);
            else if (isUpdate)
                UpdateGameStateToServer();
        }

        void PackPlayerBot()
        {
            PackPlayerServer(GetCurrentPlayingPlayerID());
        }

        void PackPlayer()
        {

            if (myPlayer)
            {
                GamePlayUI.instance.strengthMeter.SetActive(false);
                GamePlayUI.instance.SeeButtonActive(false);
                GamePlayUI.instance.HideHud();
            }

            myPlayer.PackCMD(myPlayerID);
        }

        void PackPlayer(int currentPlayerIndex)
        {
            if (isServer)
                PackPlayerServer(gameState.players[currentPlayerIndex].playerData.playerID);
            else
                myPlayer.PackCMD(gameState.players[currentPlayerIndex].playerData.playerID);
        }

        public IEnumerator OnAcceptSideShowDelayCall(int firstPlayerId, int secondPlayerId)
        {

            gameState.SideShowWinnerEffectAfterAccept = true;
            UpdateGameStateToServer();

            gameState.players[firstPlayerId].isMyTurn = false;
            gameState.isSideShowRequestSend = false;
            gameState.sideShowRequestReceiver = -1;
            gameState.sideShowRequestSender = -1;
            gameState.sideShowRequestTime = -1;
            if (CardCombination.CompareCards(gameState.players[secondPlayerId].GetCurrentCards(), gameState.players[firstPlayerId].GetCurrentCards(), "0", 0, 0, 0))
            {
                gameState.players[firstPlayerId].currentState = 1;
                gameState.players[secondPlayerId].currentState = 1;
                yield return new WaitForSeconds(7);
                gameState.currentState = 2;
                gameState.SideShowWinnerEffectAfterAccept = false;
                PackPlayer(firstPlayerId);
            }
            else
            {
                gameState.players[secondPlayerId].currentState = 1;
                gameState.players[firstPlayerId].currentState = 1;
                yield return new WaitForSeconds(7);
                gameState.currentState = 2;
                gameState.SideShowWinnerEffectAfterAccept = false;
                PackPlayer(secondPlayerId);
            }



            DebugHelper.LogError(gameState.players[secondPlayerId].playerData.playerName + "Change game state 2" + gameState.players[firstPlayerId].playerData.playerName);


            //if (gameController.CurrentGameMode == GameMode.ZANDU)
            //{
            //    if (CardCombination.CompareCards(gameState.players[secondPlayerId].GetCurrentCards(), gameState.players[firstPlayerId].GetCurrentCards(), "Zandu", gameState.zanducards[0].rankCard, gameState.zanducards[1].rankCard, gameState.zanducards[2].rankCard))
            //    {
            //        gameState.players[firstPlayerId].currentState = 1;
            //        gameState.players[secondPlayerId].currentState = 1;
            //        PackPlayer(firstPlayerId);
            //    }
            //    else
            //    {
            //        gameState.players[firstPlayerId].currentState = 1;
            //        gameState.players[secondPlayerId].currentState = 1;
            //        PackPlayer(secondPlayerId);
            //    }
            //}
            //else if (gameController.CurrentGameMode == GameMode.HUKAM)
            //{
            //    if (CardCombination.CompareCards(gameState.players[secondPlayerId].GetCurrentCards(), gameState.players[firstPlayerId].GetCurrentCards(), "Zandu", gameState.zanducards[0].rankCard, gameState.zanducards[0].rankCard, gameState.zanducards[0].rankCard))
            //    {
            //        gameState.players[firstPlayerId].currentState = 1;
            //        gameState.players[secondPlayerId].currentState = 1;
            //        PackPlayer(firstPlayerId);
            //    }
            //    else
            //    {
            //        gameState.players[firstPlayerId].currentState = 1;
            //        gameState.players[secondPlayerId].currentState = 1;
            //        PackPlayer(secondPlayerId);
            //    }
            //}

            //else if (gameController.CurrentGameMode == GameMode.AK47)
            //{
            //    if (CardCombination.CompareCards(gameState.players[secondPlayerId].GetCurrentCards(), gameState.players[firstPlayerId].GetCurrentCards(), "Ak47", 0, 0, 0))
            //    {
            //        gameState.players[secondPlayerId].currentState = 1;
            //        gameState.players[firstPlayerId].currentState = 1;
            //        PackPlayer(firstPlayerId);
            //    }
            //    else
            //    {
            //        gameState.players[secondPlayerId].currentState = 1;
            //        gameState.players[firstPlayerId].currentState = 1;
            //        PackPlayer(secondPlayerId);
            //    }
            //}
            //else if (gameController.CurrentGameMode == GameMode.MUFLIS)
            //{
            //    if (CardCombination.CompareCards(gameState.players[secondPlayerId].GetCurrentCards(), gameState.players[firstPlayerId].GetCurrentCards(), "Muflis", 0, 0, 0))
            //    {
            //        gameState.players[secondPlayerId].currentState = 1;
            //        gameState.players[firstPlayerId].currentState = 1;
            //        PackPlayer(secondPlayerId);
            //    }
            //    else
            //    {
            //        gameState.players[secondPlayerId].currentState = 1;
            //        gameState.players[firstPlayerId].currentState = 1;
            //        PackPlayer(secondPlayerId);
            //    }
            //}
            //else
            //{

            //}

            yield return null;
        }




        public void UpdateUI()
        {
            foreach (PlayerUI ui in GamePlayUI.instance.playerUIDetails)
            {
                if (ui.isMine)
                {
                    Debug.Log("*** UpdateUI ----->" + APIController.instance.userDetails.balance);
                    ui.playerAmount.text = CommonFunctions.Instance.TpvAmountSeparator(APIController.instance.userDetails.balance) + " " + $"<size=15>{APIController.instance.authentication.currency_type}</size>"; ;
                }

                if (ui.IsFull() && gameState.removedPlayers.Exists(x => x == ui.playerID))
                {
                    Debug.Log("Check this =============> " + ui.IsFull() + "***************" + gameState.removedPlayers.Exists(x => x == ui.playerID));
                    gameState.removedPlayers.Remove(ui.playerID);
                    ui.ClearUI();
                    continue;
                }

                if (ui.IsFull())
                {
                    PlayerState tempState = new PlayerState();
                    if (gameState.players.Exists(x => x.playerData.playerID == ui.playerID))
                        tempState = gameState.players.Find(x => x.playerData.playerID == ui.playerID);
                    else if (gameState.waitingPlayers.Exists(x => x.playerData.playerID == ui.playerID))
                        tempState = gameState.waitingPlayers.Find(x => x.playerData.playerID == ui.playerID);
                    else
                        tempState = null;
                    if (!string.IsNullOrEmpty(GetPlayerPUN(ui.playerID).GetPlayerID()))
                    {
                        if (tempState != null)
                            GetPlayerPUN(ui.playerID).UpdatePlayerState(tempState);
                        else
                        {
                            LoggerUtils.Log("Player missing in Gamestate");
                            StartCoroutine(GetPlayerPUN(ui.playerID).WaitForPlayerInGameState());
                            StartCoroutine(CheckPlayerManagerAndClearUI(ui));
                        }

                    }
                    else if (tempState != null)
                    {
                        ui.UpdateStatus(tempState);
                        ui.SetDisconnected();
                    }
                    else if (tempState == null)
                        ui.ClearUI();
                }

            }

            if (gameState.removedPlayers.Count > 0)
            {
                gameState.removedPlayers.Clear();
            }

        }

        void RemoveUnusedPlayerUI()
        {
            if (gameState.removedPlayers.Count == 0)
                return;

            foreach (PlayerUI ui in GamePlayUI.instance.playerUIDetails)
            {
                DebugHelper.LogError("attempting to remove player UI: " + gameState.removedPlayers.Count);
                if (ui.IsFull() && gameState.removedPlayers.Exists(x => x == ui.playerID))
                {
                    DebugHelper.LogError("attempting to remove player UI");
                    gameState.removedPlayers.Remove(ui.playerID);
                    ui.ClearUI();
                    continue;
                }
            }
            DebugHelper.LogError(JsonUtility.ToJson(gameState));
        }

        IEnumerator CheckPlayerManagerAndClearUI(PlayerUI playerUI)
        {
            yield return new WaitForSeconds(1);
            if (!string.IsNullOrEmpty(GetPlayerPUN(playerUI.playerID).GetPlayerID()))
            {
                playerUI.ClearUI();

            }
        }

        void SetGameStartUI()
        {
            gameState.isSideShowRequestSend = false;


            StartCoroutine(DistributeCards());


            GamePlayUI.instance.SeeButtonActive(true);
        }

        IEnumerator StartGameTimer()
        {
            isAutoSee = false;
            GamePlayUI.instance.SeeButtonActive(false);
            GamePlayUI.instance.ResetHud();
            isSideshow = false;
            int countDown = 10;
            GamePlayUI.instance.gameTimer.SetActive(true);

            ResetCards();

            /*if (UIGameController.CurrentGameMode == GameMode.ZANDU)
                GamePlayUI.instance.ResetZanthuCards();*/
            ///First attemp to get match token

            yield return null;



        }

        IEnumerator initGameClient()
        {
            GamePlayUI.instance.isBootCollected = true;
            yield return new WaitForSeconds(1);
            GamePlayUI.instance.gameTimer.SetActive(false);
            if (!isrejoin)
            {
                GamePlayUI.instance.GlobalMessage("Card Dealing");
            }
            LinkPlayerManagerToUI(true);
            SetGameStartUI();
            yield return new WaitForSeconds(1.5f);
            GamePlayUI.instance.gameTimer.SetActive(false);

        }

        public void checkForStartgameMaster()
        {
            if (!gameState.isDealCard)
            {
                StartGame();
                UpdateGameStateToServer();
            }
        }

        void InitPlayerTurn()
        {
            foreach (PlayerState ps in gameState.players)
            {
                if (!ps.hasPacked && !ps.hasAllin)
                {

                    ps.SetMyTurn(true);
                    previousPlayerId = ps.playerData.playerID;
                    if (playerTurnTimerRoutine != null) StopCoroutine(playerTurnTimerRoutine);
                    playerTurnTimerRoutine = StartCoroutine(StartPlayerTimer(ps.playerData.playerID));
                    DebugHelper.LogError("start game timer set turn " + ps.playerData.playerName);
                    return;
                }
            }

        }

        public bool VerifyMyPlayerStateInGame()
        {
            if (gameState.players.Exists(x => x.playerData.playerID == myPlayerID))
                return true;
            else
                return false;
        }

        public void LostGameBotServer(double Amount, string PlayerId)
        {
            Debug.Log("LostGameBotServer Called");
            PlayerState ps = GetPlayerState(PlayerId);

            if (gameController.isBlockedAPI)
            {
                if (GetActivePlayerManager() != null)
                {

                    GetActivePlayerManager().SetBotWinnerAPI(ps.BetIndex, 0, Amount, "LossGameBot", ps.playerData.playerID, gameState.totalPot);
                }

            }
            else
            {
                TransactionMetaData val = new();
                val.Amount = 0;
                val.Info = "LossGameBot";

                if (!OnlyBotExsist)
                {
                    APIController.instance.WinningsBetMultiplayerAPI(ps.BetIndex, ps.BetId, 0, ps.CurrentGameSpend, gameState.totalPot, val, null, ps.playerData.playerID, true, true, gameController.gameName, gameController.operatorName, gameController.gameId, gameController.Commission, gameState.currentMatchToken, gameController.domainURL, ps.playerData.session_token, ps.playerData.currency_type, ps.playerData.platform, ps.playerData.token, gameController.environment, ps.playerData.money.ToString());
                }

            }



        }

        public void LossGamePlayerServer(double Amount, string PlayerId)
        {

            Debug.Log("LossGamePlayerServer Called");
            PlayerState ps = GetPlayerState(PlayerId);

            if (gameController.isBlockedAPI)
            {
                CheckWinnerAndLooser(ps.BetIndex, 0, ps.CurrentGameSpend, "LossGamePlayer", ps.playerData.playerID, "", gameState.totalPot);
            }
            else
            {
                TransactionMetaData val = new();
                val.Amount = 0;
                val.Info = "LossGamePlayer";

                // ShowServerAnimation(0, ps.playerData.money, ps.playerData.playerID);
                APIController.instance.WinningsBetMultiplayerAPI(ps.BetIndex, ps.BetId, 0, ps.CurrentGameSpend, gameState.totalPot, val, (x, c, y) => {
                    if (x)
                    {
                        JObject jsonObject = JObject.Parse(y);
                        Debug.Log("Ludo Game Balance ==========> " + jsonObject["balance"].ToString());
                        ClientUpdateBalance(jsonObject["balance"].ToString(), ps.playerData.playerID);


                    }
                    else
                    {
                         HandleErrorCode(c, ps.playerData.playerID, y);
                        UpdateGameStateToServer();
                    }
                }
                   , ps.playerData.playerID, false, true, gameController.gameName, gameController.operatorName, gameController.gameId, gameController.Commission, gameState.currentMatchToken, gameController.domainURL, ps.playerData.session_token, ps.playerData.currency_type, ps.playerData.platform, ps.playerData.token, gameController.environment, ps.playerData.money.ToString());
            }


        }

        public void LostGame()
        {

            string val = CommonFunctions.Instance.TpvAmountSeparator(myPlayerState.playerData.money, true);
            UIGameController.totalEarnings -= myPlayerState.CurrentGameSpend;
        }

        public void ChallAmount(double amount, string playerid)
        {
            if (gameController.IsTournament)
                return;
        }

        public void ChallAmountBot(double amount, double balance, string playerid, bool isinitialize, bool check = false)
        {
            if (GetActivePlayerManager() != null)
            {
                GetActivePlayerManager().SetBotInializeAPI(playerid, amount, balance, isinitialize, check);
            }
            else
            {

            }
            if (gameController.IsTournament)
                return;
        }

        private IEnumerator GlobalMessageCallWithDelay(string _name, string _wonCombination)
        {
            yield return new WaitForSeconds(1f);
            GamePlayUI.instance.GlobalMessage(_name + " Win with a " + _wonCombination);
        }

        public void InitNewRoundState()
        {
            if (!string.IsNullOrWhiteSpace(gameState.currentMatchToken))
            {
                APIController.instance.TerminateMatch(gameState.currentMatchToken, gameController.operatorName, gameController.environment);
            }
            DebugHelper.LogError("init new round");
            if (gameController.IsTournament && gameState.CurrentGameCount > 0)
                gameState.currentState = 1;
            else
                gameState.currentState = 0;
            gameState.gameStartTime = 0;
            gameState.zanthuCount = 0;
            gameState.forceSee = false;
            gameState.totalPot = gameState.currentStake = 0;
            gameState.sideShowRequestReceiver = -1;
            gameState.sideShowRequestSender = -1;
            gameState.sideShowRequestTime = -1;
            isWinnerDisplayed = false;
            gameState.isSideShowRequestSend = false;
            gameState.isDealCard = false;
            Debug.Log("NewDeck===========>1");
            gameState.NewDeck();
            /*if (gameController.CurrentGameMode.ToString() == "ZANDU")
                gameState.InitZanduMode();*/
            /* else if (gameController.CurrentGameMode == GameMode.HUKAM)
                 gameState.InitHUKAMMode();*/

            foreach (PlayerState ps in gameState.players)
            {
                ps.ResetState();
            }
            if ((gameState.players.Count + gameState.waitingPlayers.Count) == 1)
            {
                if (gameState.players.Count == 1)
                {
                    gameState.waitingPlayers.Add(gameState.players[0]);
                    gameState.players.Clear();
                }
            }
            gameState.currentMatchToken = string.Empty;
        }

        IEnumerator ResetGame()
        {

            yield return new WaitForSeconds(7);
            isWinnerDisplayed = true;
            GamePlayUI.instance.HideHud();
            GamePlayUI.instance.hukamCards.SetActive(false);
            gameState.totalPot = 0;
            UpdateNetworkGame();
            if (UIGameController.CurrentGameType == GameType.PRIVATE)
            {
                yield return new WaitForSeconds(1);
                isAutoSee = false;
                GamePlayUI.instance.SeeButtonActive(false);
                GamePlayUI.instance.ResetHud();
                isSideshow = false;
                ResetCards();

            }
        }

        public void showTournamentBanner()
        {

        }

        public void ReduceLife()
        {
            gameState.players[GetCurrentPlayingPlayerIndex()].lives--;
        }

        public int NextPlayerIndex(int currentPlayerIndex)
        {
            int nextPlayerIndex = currentPlayerIndex;
            gameState.players[currentPlayerIndex].turnCount++;
            gameState.players[currentPlayerIndex].SetMyTurn(false);
            bool isNextPlayerFound = false;

            while (!isNextPlayerFound)
            {
                if (nextPlayerIndex + 1 == gameState.players.Count)
                {
                    DebugHelper.Log("new round");
                    nextPlayerIndex = 0;
                }
                else
                    nextPlayerIndex += 1;

                if (gameState.players[nextPlayerIndex].currentState == 1 && !gameState.players[nextPlayerIndex].hasPacked && !gameState.players[nextPlayerIndex].hasAllin)
                {
                    isNextPlayerFound = true;
                    if (gameState.players[nextPlayerIndex].turnCount >= currentTableModel.BlindLimit /*&& gameController.CurrentGameMode != GameMode.NOLIMITS*/ /*&& gameController.CurrentGameMode != GameMode.POTBLIND*/)
                    {
                        if (isAutoSee == false)
                            AutoSeeUpdate();
                    }
                    /*if (gameController.CurrentGameMode == GameMode.ZANDU)
                    {
                        UpdateZanthuStatus(nextPlayerIndex);
                    }*/
                }
            }

            var playersNotPacked = gameState.players.FindAll(x => !x.hasPacked);
            if (playersNotPacked.Any())
            {
                int firstTurnCount = playersNotPacked.First().turnCount;
                bool allTurnCountsSame = playersNotPacked.All(x => x.turnCount == firstTurnCount);
                if (allTurnCountsSame)
                {
                    gameState.RoundCount++;
                }
            }

            return nextPlayerIndex;
        }

        public void NextPlayerTurn()
        {

            if (GetCurrentGameState() != 2)
                return;
            double delay = 0;
            if (GetActivePlayedPlayerCount() <= 1)
            {

                DebugHelper.LogError("game state update 4");
                gameState.currentState = 4;
                UpdateGameStateToServer();
                return;
            }
            PlayerState ps = gameState.players[GetCurrentPlayingPlayerIndex()];
            if (ps.playerData.isBot)
            {
                delay = NetworkTime.time - ps.myTurnTime - 10;
                if (delay < 0)
                    delay = 0;
                if (delay > 15)
                    delay = 15;
            }
            LoggerUtils.Log("Next player turn with update");
            int currentPlayerIndex = GetCurrentPlayingPlayerIndex();
            int index = NextPlayerIndex(currentPlayerIndex);
            gameState.players[index].SetMyTurn(true);
            UpdateGameStateToServer();
            if (playerTurnTimerRoutine != null) StopCoroutine(playerTurnTimerRoutine);
            playerTurnTimerRoutine = StartCoroutine(StartPlayerTimer(gameState.players[index].playerData.playerID));
        }
        IEnumerator StartPlayerTimer(string playerID)
        {
            DebugHelper.Log("check next timer ");
            yield return new WaitForSeconds(15.5f);
            PlayerState tempState = new PlayerState();
            if (gameState.players.Exists(x => x.playerData.playerID == playerID))
                tempState = gameState.players.Find(x => x.playerData.playerID == playerID);
            else if (string.IsNullOrEmpty(GetCurrentPlayingPlayerID()))
                tempState = null;
            else
                yield break;
            if (tempState == null)
                NextPlayerTurn(true);
            else if (tempState.isMyTurn && gameState.currentState == 2)
            {
                while (gameState.currentState == 2 && tempState.myTurnTime + 15.2f >= NetworkTime.time)
                {
                    yield return new WaitForSeconds(0.2f);
                }
                if (tempState.isMyTurn && gameState.currentState == 2)
                {
                    NextPlayerTurn(false);
                    InactiveUserKick(playerID, StaticStrings.InactiveServerKick);

                }
            }


        }



        void NextPlayerTurn(bool shouldUpdateServer)
        {

            if (GetCurrentGameState() != 2)
                return;
            DebugHelper.Log("check nextplayer turn");
            if (GetActivePlayedPlayerCount() <= 1)
            {
                DebugHelper.LogError("game state update 4");
                gameState.allPacked = true;
                gameState.currentState = 4;
                UpdateGameStateToServer();
                return;
            }
            botPlay = null;

            PlayerState ps = gameState.players[GetCurrentPlayingPlayerIndex()];

            LoggerUtils.Log("Next player turn with update");
            int currentPlayerIndex = GetCurrentPlayingPlayerIndex();
            int index = NextPlayerIndex(currentPlayerIndex);
            gameState.players[index].SetMyTurn(true);
            previousPlayerId = gameState.players[index].playerData.playerID;
            if (shouldUpdateServer)
                UpdateGameStateToServer();
            if (playerTurnTimerRoutine != null) StopCoroutine(playerTurnTimerRoutine);
            playerTurnTimerRoutine = StartCoroutine(StartPlayerTimer(gameState.players[index].playerData.playerID));
        }

        public void RemoveFromGame(string playerID, bool isforce)
        {
            if (playerID == gameState.GameCreatePlayerID)
            {
                if (GetPlayerCount() > 1)
                {
                    foreach (PlayerState ps in gameState.players)
                    {
                        if (ps.playerData.playerID != playerID)
                        {
                            gameState.GameCreatePlayerID = ps.playerData.playerID;
                            break;
                        }
                    }
                    if (gameState.GameCreatePlayerID == playerID)
                    {
                        foreach (PlayerState ps in gameState.waitingPlayers)
                        {
                            if (ps.playerData.playerID != playerID)
                            {
                                gameState.GameCreatePlayerID = ps.playerData.playerID;
                                break;
                            }
                        }

                    }
                }
            }
            if (!isforce && gameController.IsTournament)
            {
                if (gameState.players.Exists(x => x.playerData.playerID == playerID))
                {
                    GetPlayerState(playerID).playerData.money = 0;
                    gameState.UpdateCurrentTournamentPlayers();
                }
            }
            if (gameState.players.Exists(x => x.playerData.playerID == playerID))
            {
                if (gameState.currentState == 2 || gameState.currentState == 3)
                    PackPlayerServer(playerID, false);
                gameState.removedPlayers.Add(playerID);

                gameState.players.RemoveAll(x => x.playerData.playerID == playerID);
                if (gameState.players.Count == 1 && gameState.currentState != 0 && gameState.currentState != 1)
                    gameState.currentState = 4;


            }
            else if (gameState.waitingPlayers.Exists(x => x.playerData.playerID == playerID))
            {
                gameState.removedPlayers.Add(playerID);
                gameState.waitingPlayers.RemoveAll(x => x.playerData.playerID == playerID);
            }

            UpdateGameStateToServer();
        }

        public void RemoveMeFromGame(bool isforce = true)
        {
            myPlayer.CmdDisconnectGame();
#if UNITY_WEBGL
            UIController.CloseWindow();
#endif
            Debug.Log("Exit Game ===============> ");

        }

        public void ForcePackCurrentPlayer(bool isMine)
        {
            if (isMine && myPlayer != null && !gameController.IsTournament)
            {
                if (myPlayerState.isSpectator) return;
                GamePlayUI.instance.ResetZanthuCards();
                GamePlayUI.instance.HideHud();
                myPlayer.myUI.UpdateStatus(myPlayerState);
                myPlayer.myUI.SetSpectator(true);
                myPlayer.myUI.SetInfoText();
            }
        }

        public void ChallWithoutNextturnMaster(string playerId, bool isStakeDoubled, bool ShowMasterCheck = false)
        {
            Debug.Log("===================> Show Card 5");
            int currentPlayerIndex = GetPlayerIndex(playerId);
            if (gameState.players[currentPlayerIndex].playerData.money < gameState.currentStake)
            {
                Debug.Log("===================> Show Card 6");
                double myMoney = (gameState.players[currentPlayerIndex].playerData.money);

                Debug.Log($"TotalPot =>>>>> Before --- {gameState.totalPot}");

                gameState.totalPot += myMoney;

                Debug.Log($"TotalPot =>>>>> After --- {gameState.totalPot}");

                gameState.players[currentPlayerIndex].playerData.money = 0;
                ReduceAmount(myMoney, gameState.players[currentPlayerIndex].playerData.playerID);
                StartCoroutine(WaitAndForcedShow());
                if (gameState.players[currentPlayerIndex].playerData.isBot)
                {
                    Debug.Log($"SendChall 1 --- ");

                    if (gameController.isBlockedAPI)
                    {
                        // SendChall(playerId, myMoney, gameState.players[currentPlayerIndex].playerData.money, false);
                        ChallAmountBot(myMoney, gameState.players[currentPlayerIndex].playerData.money, gameState.players[currentPlayerIndex].playerData.playerID, false);
                    }
                    else
                    {
                        Debug.Log($"ADD BET**** ONE   " + myMoney);
                        TransactionMetaData _metaData = new TransactionMetaData();
                        _metaData.Amount = myMoney;
                        _metaData.Info = "Second Bet";
                        PlayerState ps = gameState.players[currentPlayerIndex];
                        ShowServerAnimation(myMoney, gameState.players[currentPlayerIndex].playerData.money, gameState.players[currentPlayerIndex].playerData.playerID, false);

                        if (!OnlyBotExsist)
                        {
                            APIController.instance.AddBetMultiplayerAPI(gameState.players[currentPlayerIndex].BetIndex, gameState.players[currentPlayerIndex].BetId, _metaData, myMoney, (x, c, y) =>
                            {

                                if (x)
                                {
                                    if (c != 200)
                                    {
                                         SubtractAmountforAddBetOnSessionFailed(myMoney);
                                        HandleErrorCode(c, ps.playerData.playerID, y);
                                        UpdateGameStateToServer();
                                        return;
                                    }
                                    JObject jsonObject = JObject.Parse(y);
                                    Debug.Log("Ludo Game Balance ==========> " + jsonObject["balance"].ToString());
                                    ClientUpdateBalance(jsonObject["balance"].ToString(), ps.playerData.playerID);
                                    if (ShowMasterCheck)
                                    {
                                        ShowAndEndGame();
                                    }

                                    DebugHelper.Log("CHECK >>>>>>>>>>>>>  ***************" + gameState.totalPot + " ****************** " + GetPlayerState(playerId).playerData.playerName);

                                }
                                else
                                {
                                    SubtractAmountforAddBetOnSessionFailed(myMoney);
                                    DebugHelper.Log("KICK ALL PLAYERS >>>>>>>>>>>>>  ***************");
                                    HandleErrorCode(c, ps.playerData.playerID, y);
                                    UpdateGameStateToServer();
                                    return;
                                }

                            }, playerId, true, gameController.gameName, gameController.operatorName, gameController.gameId, gameState.currentMatchToken, gameController.domainURL, ps.playerData.session_token, ps.playerData.currency_type, ps.playerData.platform, ps.playerData.token, gameController.environment, ps.playerData.money.ToString());
                        }
                        else
                        {
                            ShowAndEndGame();
                        }

                    }


                }
                else
                {
                    Debug.Log($"SendChall 1 --- ");
                    if (gameController.isBlockedAPI)
                    {
                        SendChall(playerId, myMoney, gameState.players[currentPlayerIndex].playerData.money, false, false);
                        ChallAmount(myMoney, playerId);
                    }
                    else
                    {
                        TransactionMetaData _metaData = new TransactionMetaData();
                        _metaData.Amount = myMoney;
                        _metaData.Info = "Second Bet";
                        PlayerState ps = gameState.players[currentPlayerIndex];
                        ShowServerAnimation(myMoney, gameState.players[currentPlayerIndex].playerData.money, gameState.players[currentPlayerIndex].playerData.playerID, false);
                        APIController.instance.AddBetMultiplayerAPI(gameState.players[currentPlayerIndex].BetIndex, gameState.players[currentPlayerIndex].BetId, _metaData, myMoney, (x, c, y) =>
                        {
                            if (x)
                            {
                                if (c != 200)
                                {
                                    SubtractAmountforAddBetOnSessionFailed(myMoney);
                                     HandleErrorCode(c, ps.playerData.playerID, y);
                                    UpdateGameStateToServer();
                                    return;
                                }
                                JObject jsonObject = JObject.Parse(y);
                                Debug.Log("Ludo Game Balance ==========> " + jsonObject["balance"].ToString());
                                ClientUpdateBalance(jsonObject["balance"].ToString(), ps.playerData.playerID);

                                if (ShowMasterCheck)
                                {
                                    ShowAndEndGame();
                                }
                                DebugHelper.Log("CHECK >>>>>>>>>>>>>  ***************" + gameState.totalPot + " ****************** " + GetPlayerState(playerId).playerData.playerName);
                            }
                            else
                            {
                                 SubtractAmountforAddBetOnSessionFailed(myMoney);
                                DebugHelper.Log("KICK ALL PLAYERS >>>>>>>>>>>>>  ***************");
                                HandleErrorCode(c, ps.playerData.playerID, y);
                                UpdateGameStateToServer();
                                return;
                            }

                        }, playerId, false, gameController.gameName, gameController.operatorName, gameController.gameId, gameState.currentMatchToken, gameController.domainURL, ps.playerData.session_token, ps.playerData.currency_type, ps.playerData.platform, ps.playerData.token, gameController.environment, ps.playerData.money.ToString());
                    }

                }
            }
            else
            {
                Debug.Log("===================> Show Card 7");
                double newStake = 0;
                if (isStakeDoubled)
                {
                    newStake = gameState.currentStake * 2;
                }
                else
                    newStake = gameState.currentStake;
                gameState.SetCurrentStake((int)newStake);
                if (!GetPlayerState(playerId).hasSeenCard)
                    newStake /= 2;
                gameState.players[currentPlayerIndex].playerData.money -= newStake;

                Debug.Log($"TotalPot =>>>>> Before --- {gameState.totalPot}");

                gameState.totalPot += newStake;

                Debug.Log($"TotalPot =>>>>> After --- {gameState.totalPot}");


                if (currentTableModel.PotLimit <= gameState.totalPot)
                {

                    Debug.Log("SET WINNER FROM ChallWithoutNextturnMaster");
                    Debug.Log($"Pot Limit =>>>>> --- {gameState.totalPot}");

                    StartCoroutine(WaitAndForcedShow());
                }
                else
                {
                    GetPlayerState(playerId).isMyTurn = false;
                }
                if (gameState.players[currentPlayerIndex].playerData.isBot)
                {
                    Debug.Log($"case for Bot 2 SendChall 2 --- ");

                    if (gameController.isBlockedAPI)
                    {
                        //SendChall(playerId, newStake, gameState.players[currentPlayerIndex].playerData.money, false);
                        ChallAmountBot(newStake, gameState.players[currentPlayerIndex].playerData.money, gameState.players[currentPlayerIndex].playerData.playerID, false);
                    }
                    else
                    {

                        Debug.Log($"ADD BET**** TWO  " + newStake);
                        TransactionMetaData _metaData = new TransactionMetaData();
                        _metaData.Amount = newStake;
                        _metaData.Info = "Second Bet";
                        PlayerState ps = gameState.players[currentPlayerIndex];
                        ShowServerAnimation(newStake, gameState.players[currentPlayerIndex].playerData.money, gameState.players[currentPlayerIndex].playerData.playerID, false);

                        if (!OnlyBotExsist)
                        {
                            APIController.instance.AddBetMultiplayerAPI(gameState.players[currentPlayerIndex].BetIndex, gameState.players[currentPlayerIndex].BetId, _metaData, newStake, (x, c, y) =>
                            {
                                if (x)
                                {
                                    if (c != 200)
                                    {
                                        SubtractAmountforAddBetOnSessionFailed(newStake);
                                        HandleErrorCode(c, ps.playerData.playerID, y);
                                        UpdateGameStateToServer();
                                        return;
                                    }
                                    JObject jsonObject = JObject.Parse(y);
                                    Debug.Log("Ludo Game Balance ==========> " + jsonObject["balance"].ToString());
                                    ClientUpdateBalance(jsonObject["balance"].ToString(), ps.playerData.playerID);
                                    if (ShowMasterCheck)
                                    {
                                        ShowAndEndGame();
                                    }
                                    DebugHelper.Log("CHECK >>>>>>>>>>>>>  ***************" + gameState.totalPot + " ****************** " + GetPlayerState(playerId).CurrentGameSpend);
                                }
                                else
                                {
                                    SubtractAmountforAddBetOnSessionFailed(newStake);
                                    DebugHelper.Log("KICK ALL PLAYERS >>>>>>>>>>>>>  ***************");
                                    HandleErrorCode(c, ps.playerData.playerID, y);
                                    UpdateGameStateToServer();
                                    return;

                                }

                            }, playerId, true, gameController.gameName, gameController.operatorName, gameController.gameId, gameState.currentMatchToken, gameController.domainURL, ps.playerData.session_token, ps.playerData.currency_type, ps.playerData.platform, ps.playerData.token, gameController.environment, ps.playerData.money.ToString());

                        }
                        else
                        {
                            ShowAndEndGame();
                        }

                    }

                }
                else
                {
                    DebugHelper.Log($"SendChall 2 --- ");

                    if (gameController.isBlockedAPI)
                    {
                        SendChall(playerId, newStake, gameState.players[currentPlayerIndex].playerData.money, false, false);
                        ChallAmount(newStake, playerId);
                    }
                    else
                    {
                        DebugHelper.Log($"===================> &&&&&&&&&&&&&&&&&&&&&&& ");
                        TransactionMetaData _metaData = new TransactionMetaData();
                        _metaData.Amount = newStake;
                        _metaData.Info = "Second Bet";
                        PlayerState ps = gameState.players[currentPlayerIndex];
                        ShowServerAnimation(newStake, gameState.players[currentPlayerIndex].playerData.money, gameState.players[currentPlayerIndex].playerData.playerID, false);
                        APIController.instance.AddBetMultiplayerAPI(gameState.players[currentPlayerIndex].BetIndex, gameState.players[currentPlayerIndex].BetId, _metaData, newStake, (x, c, y) =>
                        {
                            if (x)
                            {
                                if (c != 200)
                                {
                                    SubtractAmountforAddBetOnSessionFailed(newStake);
                                    HandleErrorCode(c, ps.playerData.playerID, y);
                                    UpdateGameStateToServer();
                                    return;
                                }
                                JObject jsonObject = JObject.Parse(y);
                                Debug.Log("Ludo Game Balance ==========> " + jsonObject["balance"].ToString());
                                ClientUpdateBalance(jsonObject["balance"].ToString(), ps.playerData.playerID);
                                if (ShowMasterCheck)
                                {
                                    ShowAndEndGame();
                                }
                                DebugHelper.Log("CHECK >>>>>>>>>>>>>  ***************" + gameState.totalPot + " ****************** " + GetPlayerState(playerId).CurrentGameSpend);
                            }
                            else
                            {
                                SubtractAmountforAddBetOnSessionFailed(newStake);
                                DebugHelper.Log("KICK ALL PLAYERS >>>>>>>>>>>>>  ***************");
                                HandleErrorCode(c, ps.playerData.playerID, y);
                                UpdateGameStateToServer();
                                return;
                            }

                        }, playerId, false, gameController.gameName, gameController.operatorName, gameController.gameId, gameState.currentMatchToken, gameController.domainURL
                            , ps.playerData.session_token, ps.playerData.currency_type, ps.playerData.platform, ps.playerData.token, gameController.environment, ps.playerData.money.ToString());
                    }

                }
                gameState.players[currentPlayerIndex].playerData.money -= newStake;
            }
            UpdateGameStateToServer();
        }

        bool isWinnerForUpdateClient;


        [ClientRpc]
        private void ClientUpdateBalance(string amount, string playerID)
        {
            if (APIController.instance.authentication.Id == playerID)
            {
                APIController.instance.UpdateAmount(amount);
            }
        }



        /* [ClientRpc]
         private void ClientUpdateBalance()
         {

             if (!APIController.instance.userDetails.isBlockApiConnection)
             {
 #if UNITY_WEBGL && !UNITY_EDITOR
         APIController.GetUpdatedBalance();
 #endif
             }

         }*/

        void ChaalWithoutNextTurn(string playerid)
        {
            ChallWithoutNextturnMaster(playerid, false);
            GamePlayUI.instance.HideHud();
        }

        public void ReduceAmount(double amount, string playerId)
        {
            if (!gameController.IsTournament)
            {
                return;

            }

        }

        public void ReduceBootAmountForTournament(string playerid)
        {
            if (!gameController.IsTournament)
                return;

        }

        void AutoSeeUpdate()
        {
            if (gameState.currentState == 2)
            {
                gameState.forceSee = true;

                foreach (PlayerState ps in gameState.players)
                {
                    ps.SetCardsSeen();
                }
            }
        }

        void ForceShowCards()
        {

            OpenCardForShow();
            gameState.forceSee = false;
            DebugHelper.LogError("game state update 4");
            gameState.currentState = 4;
            UpdateGameStateToServer();
            if (myPlayer)
            {
                GamePlayUI.instance.HideHud();
            }
        }

        public IEnumerator WaitAndForcedShow()
        {
            GlobalMessage(("Pot limit is reached."));
            yield return new WaitForSeconds(2);
            ForceShowCards();
        }

        public int AllinPerformCount()
        {
            int count = 1;
            foreach (PlayerState ps in gameState.players)
                if (ps.hasAllin)
                    count += 1;
            return count;
        }

        public IEnumerator fetchData()
        {
            Debug.Log("FETCH DATA CALLED");
            yield return new WaitForSeconds(0f);

            if (!GetPlayerState().isSpectator)
                StartCoroutine(GamePlayUI.instance.setStrengthMeter(GetPlayerState(myPlayerID).cardStrength));
        }

        public IEnumerator CheckInternetAndProceed(Callback OnSuccess)
        {
            GamePlayUI.instance.isWaitingForRequest = true;
            yield return new WaitForSeconds(0);
            GamePlayUI.instance.isWaitingForRequest = false;
        }

        public void SetDisconnectedPlayer(string playerID)
        {
            DebugHelper.LogError("SetDisconnectedPlayer");
            if (gameState.players.Exists(x => x.playerData.playerID == playerID))
            {
                PlayerState disconnectedPlayerState = gameState.players.Find(x => x.playerData.playerID == playerID);
                if (disconnectedPlayerState.hasPacked && !gameController.IsTournament)
                {
                    DebugHelper.LogError("player " + playerID);
                    gameState.players.RemoveAll(x => x.playerData.playerID == playerID);
                }
                else
                {
                    DebugHelper.LogError("disconnect");
                    disconnectedPlayerState.disconnectTime = NetworkTime.time;
                }
            }
            if (gameState.waitingPlayers.Exists(x => x.playerData.playerID == playerID))
            {
                DebugHelper.LogError("waiting player " + playerID);
                gameState.waitingPlayers.RemoveAll(x => x.playerData.playerID == playerID);
            }
            UpdateGameStateToServer();
        }

        #region ZANTHU_MODE_LOGIC

        public void UpdateZanthuStatus(int id)
        {
            if (gameState.players[id].turnCount == 1)
            {
                gameState.zanthuCount = 1;
                gameState.zanducards[0].isClose = false;
            }
            else if (gameState.players[id].turnCount == 2)
            {
                gameState.zanthuCount = 2;
                gameState.zanducards[0].isClose = false;
                gameState.zanducards[1].isClose = false;
            }
            else if (gameState.players[id].turnCount >= 3)
            {
                gameState.zanthuCount = 3;
                gameState.zanducards[0].isClose = false;
                gameState.zanducards[1].isClose = false;
                gameState.zanducards[2].isClose = false;
            }
        }

        /*  public void CheckZanthuStatus()
          {
              if (UIGameController.CurrentGameMode == GameMode.ZANDU)
              {
                  StartCoroutine(ZanthuCardAinmination());
              }
          }*/
        IEnumerator ZanthuCardAinmination()
        {
            if (gameState.zanthuCount == 1)
            {
                GamePlayUI.instance.ZanduSetCard(gameState.zanducards[0], 0);
            }
            else if (gameState.zanthuCount == 2)
            {
                GamePlayUI.instance.ZanduSetCard(gameState.zanducards[0], 0);
                yield return new WaitForSecondsRealtime(.5f);
                GamePlayUI.instance.ZanduSetCard(gameState.zanducards[1], 1);
            }
            else if (gameState.zanthuCount == 3)
            {

                GamePlayUI.instance.ZanduSetCard(gameState.zanducards[0], 0);
                yield return new WaitForSecondsRealtime(.5f);
                GamePlayUI.instance.ZanduSetCard(gameState.zanducards[1], 1);
                yield return new WaitForSecondsRealtime(.5f);
                GamePlayUI.instance.ZanduSetCard(gameState.zanducards[2], 2);
            }
            yield return 0;
        }

        IEnumerator ShowZanthuCard()
        {
            if (gameState.zanducards[0].isClose)
            {
                GamePlayUI.instance.ZanduSetCard(gameState.zanducards[0], 0);
                yield return new WaitForSecondsRealtime(.5f);
            }
            if (gameState.zanducards[1].isClose)
            {
                GamePlayUI.instance.ZanduSetCard(gameState.zanducards[1], 1);
                yield return new WaitForSecondsRealtime(.5f);
            }
            if (gameState.zanducards[2].isClose)
            {
                GamePlayUI.instance.ZanduSetCard(gameState.zanducards[2], 2);
            }
            yield return 0;
        }

        #endregion

        #region HELPER FUNCTIONS

        public IEnumerator GetBotFromAPI()
        {
            yield return null;
            PlayerData botDataAPI = new PlayerData();
            bool issuccess = false;
            APIController.instance.GetBot((botData) =>
            {
                issuccess = true;

                botDataAPI.playerID = botData.userId;
                botDataAPI.playerName = botData.name;
                botDataAPI.silver = botData.balance;
                botDataAPI.gold = botData.balance;
                try
                {
                    if (gameController.CurrentAmountType == CashType.CASH)
                        botDataAPI.money = botData.balance;
                    else
                        botDataAPI.money = botData.balance;
                }
                catch (Exception e)
                {
                    botDataAPI.money = 0;
                }
                botDataAPI.isBot = true;
                botDataAPI.avatarIndex = 1;
                botDataAPI.profilePicURL = UnityEngine.Random.Range(0, 9).ToString();

            });
            int trycount = 0;
            while (!issuccess)
            {
                yield return new WaitForSeconds(.2f);
                trycount++;
                if (trycount > 50)
                {
                    break;
                }
            }
            String JsonValToBot = JsonUtility.ToJson(botDataAPI);
            if (playerManagersList.Find(x => x.playerID == UIGameController.CurrentPlayerData.GetPlayfabID()).isBot == false && playerManagersList.FindIndex(x => x.playerID == UIGameController.CurrentPlayerData.GetPlayfabID()) == 0)
            {
                PlayerManager.localPlayer.CmdAddBot(APIController.instance.winningStatus.WinProbablity <= 0, JsonValToBot);
            }
        }

        public int GetCurrentPlayingPlayerIndex()
        {

            if (gameState.players.Exists(x => x.isMyTurn == true))
                return gameState.players.FindIndex(x => x.isMyTurn == true);
            else if (!string.IsNullOrEmpty(previousPlayerId))
                return gameState.players.FindIndex(x => x.playerData.playerID == previousPlayerId);
            return -1;
        }
        public int GetPlayerIndex(string id)
        {

            int index = gameState.players.FindIndex(x => x.playerData.playerID == id);
            return (index == -1 ? playingPlayerIndex : index);
        }

        public string GetCurrentPlayingPlayerID()
        {
            if (gameState.players.Exists(x => x.isMyTurn == true))
                return gameState.players.Find(x => x.isMyTurn == true).playerData.playerID;
            else if (!string.IsNullOrEmpty(previousPlayerId))
                return previousPlayerId;
            return "";
        }

        public int GetSideShowIndex()
        {
            int currentPlayerIndex = GetMyPlayerIndex();
            int sideShowPlayerIndex = currentPlayerIndex;
            int nextPlayerIndex = currentPlayerIndex + 1;
            if (nextPlayerIndex >= gameState.players.Count)
                nextPlayerIndex = 0;
            bool isNextPlayerFound = false;

            while (!isNextPlayerFound && nextPlayerIndex != currentPlayerIndex)
            {
                if (gameState.players[nextPlayerIndex].currentState == 1 && !gameState.players[nextPlayerIndex].hasPacked && gameState.players[nextPlayerIndex].hasSeenCard && !gameState.players[nextPlayerIndex].hasAllin)
                {
                    isNextPlayerFound = true;
                    sideShowPlayerIndex = nextPlayerIndex;
                }

                if (nextPlayerIndex + 1 == gameState.players.Count)
                    nextPlayerIndex = 0;
                else
                    nextPlayerIndex += 1;

            }

            return sideShowPlayerIndex;
        }

        public List<PlayerState> GetContestingPlayers()
        {
            List<PlayerState> contestingPlayers = new List<PlayerState>();
            foreach (PlayerState ps in gameState.players)
            {
                if (!ps.hasPacked && !ps.hasAllin)
                    contestingPlayers.Add(ps);
            }
            return contestingPlayers;
        }
        public int GetSeenPlayerCount()
        {
            int count = 0;
            foreach (PlayerState ps in gameState.players)
            {
                if (ps.hasSeenCard && !ps.hasPacked && !ps.hasAllin)
                    count = count + 1;

            }
            return count;

        }

        public PlayerManager GetPlayerPUN(string playerid)
        {
            playerManagersList = playerManagersList.Where(x => x != null).ToList();
            PlayerManager pm = new PlayerManager();
            if (playerManagersList.Exists(x => x.GetPlayerID() == playerid))
            {
                pm = playerManagersList.FirstOrDefault(x => x.GetPlayerID() == playerid);
            }
            return pm;
        }


        public PlayerState GetPlayerState(string playerID = "")
        {
            PlayerState playerState;
            if (playerID == "")
                playerID = myPlayerID;
            if (gameState.players.Exists(x => x.playerData.playerID == playerID))
                playerState = gameState.players.Find(x => x.playerData.playerID == playerID);
            else if (gameState.waitingPlayers.Exists(x => x.playerData.playerID == playerID))
                playerState = gameState.waitingPlayers.Find(x => x.playerData.playerID == playerID);
            else
                playerState = new PlayerState();
            return playerState;
        }

       

        public int GetMyPlayerIndex()
        {
            return gameState.players.FindIndex(x => string.Equals(x.playerData.playerID, myPlayerID));
        }

        public int GetCurrentGameState()
        {
            return gameState.currentState;
        }


        public int GetActivePlayedPlayerCount()
        {
            int i = 0;
            foreach (PlayerState ps in gameState.players)
            {
                if (!ps.hasPacked && !ps.hasAllin)
                    i++;
            }
            return i;
        }

        public int GetActivePlayersCount()
        {
            int i = 0;
            foreach (PlayerState ps in gameState.players)
            {
                if (!ps.playerData.isBot)
                    i++;
            }
            return i;
        }

        public int GetNotDisconnectedPlayerCount()
        {
            int i = 0;
            foreach (PlayerState ps in gameState.players)
            {
                if (ps.disconnectTime < 0)
                    i++;
            }
            return i;
        }


        public int getUiIndex()
        {
            List<int> index = new List<int>();
            foreach (PlayerState ps in gameState.players)
            {
                index.Add(ps.ui);
            }
            foreach (PlayerState ps in gameState.waitingPlayers)
            {
                index.Add(ps.ui);
            }
            for (int i = 0; i < 5; i++)
            {
                if (!index.Contains(i))
                {
                    return i;
                }
            }
            return -1;
        }

        public PlayerUI GetUI(string targetPlayfabID)
        {
            PlayerUI assignedUI;
            int myPlayerIndex = GetPlayerState(myPlayerID).ui;
            int targetPlayerIndex = GetPlayerState(targetPlayfabID).ui;

            Debug.Log("Finding Error  + myplayerIndex" + myPlayerIndex + "TargetrPlayerIndex" + targetPlayerIndex);

            assignedUI = GamePlayUI.instance.playerUIDetails[playerUiPosition[myPlayerIndex].playerIndex[targetPlayerIndex]];



            return assignedUI;
        }


        public PlayerUI GetPlayerUI(string playerID)
        {
            PlayerUI assignedUI = new PlayerUI();
            foreach (PlayerUI playerUI in GamePlayUI.instance.playerUIDetails)
            {
                if (playerUI.playerID == playerID)
                    return playerUI;
            }
            return assignedUI;
        }

        public int GetPlayerCount()
        {
            int count = 0;
            foreach (PlayerState ps in gameState.players)
            {
                if (!ps.playerData.isBot && ps.disconnectTime < 0)
                {
                    count += 1;
                }
            }
            foreach (PlayerState ps in gameState.waitingPlayers)
            {
                if (!ps.playerData.isBot && ps.disconnectTime < 0)
                {
                    count += 1;
                }
            }
            return count;
        }



        #endregion

        #region CLIENT_SIDE_LOGIC


        [Client]
        public void InitGame()
        {
            if (UIGameController.isForceJoin)
            {
                UIGameController.CurrentAmountType = gameState.gameController.CurrentAmountType;
                UIGameController.CurrentGameMode = gameState.gameController.CurrentGameMode;
                UIGameController.CurrentGameModelTable = gameState.gameController.CurrentGameModelTable;
                UIGameController.CurrentGameType = gameState.gameController.CurrentGameType;
                UIGameController.isBotWin = APIController.instance.winningStatus.WinProbablity <= 0 ? true : false;
            }
            InitCommon();
            ResetCards();
            myPlayerID = UIGameController.CurrentPlayerData.GetPlayfabID();
            gameState.dealersprite();
            GamePlayUI.instance.GlowMinusButton.SetActive(false);
            // GamePlayUI.instance. decreaseBet.image.sprite = GamePlayUI.instance.InActivePlusMinus;
            GamePlayUI.instance.Minus.color = GamePlayUI.instance.Inactive;
            GamePlayUI.instance.decreaseBet.interactable = false;
            GamePlayUI.instance.hukamCards.SetActive(false);
            UIGameController.isInGame = true;
            EnterGame();
            GamePlayUI.instance.InitDetails();

        }
        public bool showOnceCointStack;
        public bool PackSound;
        public bool Allpacked;
        public void UpdateNetworkGame()
        {
            if (UIGameController == null)
            {
                UIGameController = GameController.Instance;
            }

            refreshGameTime = NetworkTime.time;
            myPlayerID = UIGameController.CurrentPlayerData.GetPlayfabID();
            myPlayerState = GetPlayerState();
            myPlayerData = myPlayerState.playerData;
            myPlayer = GetPlayerPUN(myPlayerID);

            double value = 0;

            string potVal = CommonFunctions.Instance.TpvAmountSeparator(value, true);





            switch (gameState.currentState)
            {
                case 0:
                    GamePlayUI.instance.PotLimitReched.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 440, 0);
                    GamePlayUI.instance.PotLimitReched.SetActive(false);
                    // GamePlayUI.instance.ParentCoinsStack.gameObject.SetActive(true);
                    PackSound = false;
                    BackGroundMusic = true;
                    IsFirstBet = false;
                    showOnceCointStack = true;
                    GamePlayUI.instance.globalPannel.SetActive(false);
                    GamePlayUI.instance.EnableBottomNotificationPannel("PLEASE WAIT FOR PLAYERS TO JOIN");
                    Allpacked = false;
                    if (playerManagersList.Exists(x => x.playerID == myPlayerID))
                    {
                        if (playerManagersList.Find(x => x.playerID == myPlayerID))
                        {
                            if (APIController.instance.userDetails.balance < 11)
                            {
                                if (APIController.instance.userDetails.isBlockApiConnection)
                                {
                                    if (!UIController.Instance.InsufficientDemo.activeSelf)
                                    {
                                        UIController.Instance.InsufficientDemo.SetActive(true);
                                        UIController.Instance.Loading.SetActive(false);
                                    }
                                }
                                else
                                {

                                    if (!UIController.Instance.Insufficient.activeSelf)
                                    {
                                        UIController.Instance.Insufficient.SetActive(true);
                                        UIController.Instance.Loading.SetActive(false);
                                    }

                                }
                            }
                        }

                    }

                    isdistributCard = false;

                    GamePlayUI.instance.logo.SetActive(true);
                    GamePlayUI.instance.potAmountHolder.SetActive(true);
                    isWinnerDisplayed = false;
                    GamePlayUI.instance.SeeButtonActive(false);

                    GamePlayUI.instance.strengthMeter.SetActive(false);
                    GamePlayUI.instance.potAmount.text = potVal + " " + $"<size=25>{APIController.instance.authentication.currency_type}</size>";
                    Debug.Log(potVal + "potValpotVal");
                    GamePlayUI.instance.bottomTemp.gameObject.SetActive(false);
                    UpdateUI();
                    LinkPlayerManagerToUI();
                    GamePlayUI.instance.HideHud();
                    //GamePlayUI.instance.ParentCoinsStack.SetActive(false);
                    break;

                case 1:
                    GamePlayUI.instance.PotLimitReched.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 440, 0);
                    GamePlayUI.instance.PotLimitReched.SetActive(false);
                    GamePlayUI.instance.bottomTemp.gameObject.SetActive(false);
                    GamePlayUI.instance.ResetCoins.ResetBackAllData();
                    GamePlayUI.instance.ResetCoins.ResetBackAllData1();
                    GamePlayUI.instance.ResetCoins.ResetBackAllData2();
                    GamePlayUI.instance.ResetCoins.ResetBackAllData4();
                    GamePlayUI.instance.ResetCoins.ResetBackAllData5();
                    //GamePlayUI.instance.ParentCoinsStack.gameObject.SetActive(true);
                    GamePlayUI.instance.SoundCheck = true;

                    GamePlayUI.instance.challText.text = "blind";
                    
                    GamePlayUI.instance.challAmountText.text = "0.00";
                    CountTemp = 0;
                    PackSound = false;
                    BackGroundMusic = true;
                    showOnceCointStack = true;
                    GamePlayUI.instance.strengthMeterText.text = "";
                    GamePlayUI.instance.strengthMeterList.fillAmount = 0;
                    IsFirstBet = false;
                    // GamePlayUI.instance.ParentCoinsStack.SetActive(false);




                    if (playerManagersList.Exists(x => x.playerID == myPlayerID))
                    {
                        if (playerManagersList.Find(x => x.playerID == myPlayerID))
                        {
                            if (APIController.instance.userDetails.balance < 11)
                            {
                                if (APIController.instance.userDetails.isBlockApiConnection)
                                {
                                    if (!UIController.Instance.InsufficientDemo.activeSelf)
                                    {
                                        UIController.Instance.InsufficientDemo.SetActive(true);
                                        UIController.Instance.Loading.SetActive(false);
                                    }
                                }
                                else
                                {

                                    if (!UIController.Instance.Insufficient.activeSelf)
                                    {
                                        UIController.Instance.Insufficient.SetActive(true);
                                        UIController.Instance.Loading.SetActive(false);

                                    }

                                }
                            }
                        }

                    }
                    Allpacked = false;

                    if (myPlayerState.isSpectator)
                    {

                        GamePlayUI.instance.DisableBottomNotificationPannel();


                    }
                    else
                    {
                        GamePlayUI.instance.DisableBottomNotificationPannel();
                    }

                    GamePlayUI.instance.potAmount.text = potVal + " " + $"<size=25>{APIController.instance.authentication.currency_type}</size>";
                    GamePlayUI.instance.SeeButtonActive(false);
                    StopCoroutine(nameof(ResetGame));

                    GamePlayUI.instance.logo.SetActive(true);
                    GamePlayUI.instance.potAmountHolder.SetActive(true);



                    GamePlayUI.instance.strengthMeter.SetActive(false);
                    UpdateUI();
                    LinkPlayerManagerToUI();

                    ResetCards();
                    GamePlayUI.instance.ResetZanthuCards();
                    GamePlayUI.instance.hukamCards.SetActive(false);
                    for (int i = 0; i < GamePlayUI.instance.playerUIDetails.Count; i++)
                    {
                        if (!GamePlayUI.instance.playerUIDetails[i].isMine)
                        {
                            GamePlayUI.instance.playerUIDetails[i].playerBalance.SetActive(false);
                            GamePlayUI.instance.playerUIDetails[i].invite.SetActive(false);
                        }

                        //GamePlayUI.instance.playerUIDetails[i].PlayerAmountSpritePic.sprite = ChangeChipSprite.Instance.Coins[0];
                        //GamePlayUI.instance.playerUIDetails[i].BetAmountSpritePic.sprite = ChangeChipSprite.Instance.Coins[0];
                        //GamePlayUI.instance.playerUIDetails[i].BetAmountSpritePic.SetNativeSize();
                    }

                    if (myPlayer != null)
                    {
                        myPlayer.myUI.profilebg.SetActive(true);
                        myPlayer.myUI.invite.SetActive(false);
                    }


                    //StopCoroutine(nameof(GetBotFromAPI));
                    //StartCoroutine(nameof(GetBotFromAPI));

                    StopCoroutine(nameof(StartGameTimer));
                    StartCoroutine(nameof(StartGameTimer));
                    GamePlayUI.instance.HideHud();
                    break;

                case 2:
                    if (showOnceCointStack)
                    {
                        // GamePlayUI.instance.ParentCoinsStack.SetActive(true);
                        showOnceCointStack = false;
                    }

                    GamePlayUI.instance.HandStrengthMeter();
                    if (!isdistributCard && myPlayerState.currentState == 1 && gameState.players.Exists(x => x.playerData.playerID == myPlayerID) && myPlayerState.GetCurrentCards()[0].rankCard != 0)
                    {
                        if (lastDealingTime == 0 || (lastDealingTime + 4 < NetworkTime.time))
                        {
                            lastDealingTime = NetworkTime.time;
                            StartCoroutine(initGameClient());
                        }

                    }
                    else
                    {
                        if (myPlayerState.currentState == 1 && gameState.players.Exists(x => x.playerData.playerID == myPlayerID) && myPlayerState.GetCurrentCards()[0].rankCard != 0)
                            CheckAutoSee();

                    }
                    Allpacked = true;

                    if (myPlayerState.currentState == 0)
                        if (myPlayerState.isSpectator)
                        {

                            GamePlayUI.instance.DisableBottomNotificationPannel();
                        }

                        else
                        {
                            GamePlayUI.instance.EnableBottomNotificationPannel("PLEASE WAIT FOR NEXT ROUND TO START");
                        }

                    else
                        GamePlayUI.instance.DisableBottomNotificationPannel();

                    if (UIGameController.CurrentGameType == GameType.PRIVATE)
                    {
                        GamePlayUI.instance.logo.SetActive(true);
                        GamePlayUI.instance.potAmountHolder.SetActive(true);

                    }


                    GamePlayUI.instance.sideShowWinnerEffect.DisableSideShowEffect();
                    UpdateUI();
                    isWinnerDisplayed = false;
                    IsSideShowChecked = false;
                    IsSideShowWinnerAnimationDisplay = false;
                    IsShowWinnerAnimationDisplay = false;
                    UpdatePlayingPlayer();
                    CheckMyTurn();
                    if (GetCurrentGameState() == 2)
                    {
                        /*  CheckZanthuStatus();*/

                    }
                    else
                        GamePlayUI.instance.HideHud();
                    break;
                case 3:

                    IsShowWinnerAnimationDisplay = false;
                    UpdateUI();
                    if (UIGameController.CurrentGameType == GameType.PRIVATE)
                    {
                        GamePlayUI.instance.logo.SetActive(true);
                        GamePlayUI.instance.potAmountHolder.SetActive(true);

                    }
                    CheckSideShowRequest();
                    CheckPlayerShowCard();
                    if (gameState.SideShowWinnerEffectAfterAccept)
                    {
                        Debug.Log("===============> CheckSideShow WinnerEffect");
                        ShowSideShowWinnerEffect();
                    }
                    break;
                case 4:

                    UpdateUI();

                    if (gameState.allPacked && Allpacked)
                    {
                        Allpacked = false;
                        GamePlayUI.instance.AllPacked.SetActive(true);
                        Invoke(nameof(CloseAllPackEffect), 1.3f);
                    }

                    GamePlayUI.instance.sideShowWinnerEffect.DisableSideShowEffect();
                    isdistributCard = false;
                    if (UIGameController.CurrentGameType == GameType.PRIVATE)
                    {
                        GamePlayUI.instance.logo.SetActive(true);
                        GamePlayUI.instance.potAmountHolder.SetActive(true);

                    }
                    gameState.forceSee = false;
                    StartCoroutine(callShowWinnerWithDelay(1.5f));

                    GamePlayUI.instance.HideHud();
                    GamePlayUI.instance.SeeButtonActive(false);
                    break;
                case 5:
                    UpdateUI();
                    if (gameState.ShowAnimationEffect)
                    {
                        Debug.Log("===============> Show WinnerEffect");
                        ShowWinnerEffect();

                    }
                    break;
            }
        }

        public void CloseAllPackEffect()
        {
            GamePlayUI.instance.AllPacked.SetActive(false);
        }

        public void ShowSideShowWinnerEffect()
        {

            if (!IsSideShowWinnerAnimationDisplay)
            {
                IsSideShowWinnerAnimationDisplay = true;

                if ((GameManager.localInstance.gameState.players[GameManager.localInstance.gameState.Senderval].playerData.playerID == GameManager.localInstance.myPlayerID && !GameManager.localInstance.gameState.players[GameManager.localInstance.gameState.Senderval].playerData.isBot) ||
                  (GameManager.localInstance.gameState.players[GameManager.localInstance.gameState.Receiverval].playerData.playerID == GameManager.localInstance.myPlayerID && !GameManager.localInstance.gameState.players[GameManager.localInstance.gameState.Receiverval].playerData.isBot))
                {
                    GamePlayUI.instance.sideShowWinnerEffect.EnbleThisForEffect();
                }
                else
                {
                    GamePlayUI.instance.sideShowWinnerEffect.EnbleThisForEffectOtherPlayers();
                }

            }

        }

        public void ShowWinnerEffect()
        {

            if (!IsShowWinnerAnimationDisplay)
            {
                IsShowWinnerAnimationDisplay = true;
                PlayerState val = GetPlayerState(gameState.SenderShowval);
                PlayerState val1 = GetPlayerState(gameState.ReceiverShowval);

                if ((val.playerData.playerID == GameManager.localInstance.myPlayerID && !val.playerData.isBot) ||
                  (val1.playerData.playerID == GameManager.localInstance.myPlayerID && !val1.playerData.isBot))
                {
                    GamePlayUI.instance.sideShowWinnerEffect.EnbleThisForShowEffect();
                }
                else
                {
                    GamePlayUI.instance.sideShowWinnerEffect.EnbleThisForEffectOtherPlayersShow();
                }

            }

        }

        PlayerManager GetActivePlayerManager()
        {
            PlayerManager pm = new PlayerManager();
            pm.playerID = "";
            foreach (PlayerManager playerManager in playerManagersList)
            {
                if (playerManager != null)
                {
                    if ((playerManager.playerID != "") && !playerManager.isBot)
                    {
                        pm = playerManager;
                        break;
                    }
                }
            }
            return pm;
        }


        [ClientRpc]
        public void SendChall(string playerID, double amount, double balance, bool isinitialize, bool BootColection)
        {
            string ValID;
            if (playerManagersList.Exists(x => x.playerID == playerID))
            {
                if (playerManagersList.Find(x => x.playerID == playerID).isBot)
                {
                    ValID = playerID;
                }
                else
                {
                    ValID = "";
                }

            }
            else
            {
                ValID = "";
            }







            if (playerID == myPlayerID)
            {

                if (isinitialize)
                {
                    TransactionMetaData val = new();
                    val.Amount = amount;
                    val.Info = "FirstBet";



                    betIndex = APIController.instance.InitlizeBet(amount, val, false, (success) =>
                    {

                        if (success)
                        {

                            int ValBot = gameState.players.FindAll(x => x.playerData.isBot).Count;
                            if (ValBot != 0)
                            {
                                if (APIController.instance.winningStatus.WinProbablity <= 0)
                                {
                                    DebugHelper.Log($"RNG Calculation:\n==============\n{"Bot will win the round"}\n==============\n");

                                }
                                else
                                {
                                    DebugHelper.Log($"RNG Calculation:\n==============\n{"Bot will play fair"}\n==============\n");

                                }


                                PlayerManager.localPlayer.setBoolIsWinnerPlayer(APIController.instance.winningStatus.WinProbablity <= 0);

                            }



                        }
                        else
                        {

                        }

                    }, "", false, (Betid) => {


                        PlayerManager.localPlayer.SetBetId(Betid, playerID);

                    });


                    PlayerManager.localPlayer.SetBetIndex(betIndex, playerID);

                }
                else
                {
                    TransactionMetaData val = new();
                    val.Amount = amount;
                    val.Info = "Second Bet";


                    APIController.instance.AddBet(GetPlayerState(playerID).BetIndex, GetPlayerState(playerID).BetId, val, amount, (success) =>
                    {

                        if (success)
                        {

                        }
                        else
                        {

                        }

                    }, "", false);
                }
            }




            PlayerUI myUI = GetPlayerUI(playerID);
            if (myUI != null)
                myUI.GiveAmountToPot(amount, balance, BootColection);
            if (MasterAudioController.instance.CheckSoundToggle())
                MasterAudioController.instance.PlayAudio(AudioEnum.CHIPSOUND);//2
        }


        [ClientRpc]
        public void GlobalMessageClient(string _globalInfo)
        {

            GamePlayUI.instance.GlobalMessage(_globalInfo);
        }

        [ClientRpc]
        public void CheckWinnerAndLooser(int _index, double winAmount, double amountSpend, string metaData, string playerID, string BotID, double totalPot)
        {

            TransactionMetaData val = new();
            val.Amount = winAmount;
            val.Info = metaData;
            string ValID = playerID;


            if (PlayerManager.localPlayer != null && PlayerManager.localPlayer.myPlayerData.playerID.Equals(ValID))
            {

                APIController.instance.WinningsBetMultiplayer(_index, GetPlayerState(playerID).BetId, winAmount, amountSpend, totalPot, val, (success) =>
                {

                    if (success)
                    {

                    }
                    else
                    {

                    }

                }, "", false, true);

            }



        }

        #endregion

        #region WINNER_LOGIC

        public IEnumerator ShowWinner()
        {
            if (isWinnerDisplayed == false && gameState.currentState == 4)
            {

                if (gameState.totalPot >= gameState.gameController.CurrentGameModelTable.PotLimit)
                {
                    TeenpattiGameUIHandler.instance.EnableTotalPot();
                    GamePlayUI.instance.GlobalMessage("Pot limit is reached");
                }


                isAutoSee = false;

                isWinnerDisplayed = true;
                GamePlayUI.instance.ResetHud();



                string val = CommonFunctions.Instance.TpvAmountSeparator(gameState.totalPot, true);


                GamePlayUI.instance.potAmount.text = val + " " + $"<size=25>{APIController.instance.authentication.currency_type}</size>";
                foreach (PlayerUI ui in GamePlayUI.instance.playerUIDetails)
                {
                    if (ui.IsFull() && gameState.players.Exists(x => x.playerData.playerID == ui.playerID))
                        StartCoroutine(ui.SetCard());
                }
                List<PlayerState> winerps = WinnerList();
                /*  CheckZanthuStatus();*/
                foreach (PlayerUI ui in GamePlayUI.instance.playerUIDetails)
                    ui.EndTurn();
                if (winerps.Count > 0 && !winerps[0].hasAllin)
                {
                    PlayerState winnerStateFinal = winerps[0];

                    string winningPlayerID = winnerStateFinal.playerData.playerID;
                    if (GamePlayUI.instance.playerUIDetails.Exists(x => x.playerID == winningPlayerID))
                    {

                        StopCoroutine(GlobalMessageCallWithDelay("", ""));
                        StartCoroutine(GlobalMessageCallWithDelay(CommonFunctions.Instance.GetTruncatedPlayerName(winnerStateFinal.playerData.playerName), winnerStateFinal.playerData.currentCombination.ToString()));




                        if (myPlayerState.playerData.playerID == winningPlayerID)
                        {

                            WinningGame(gameState.totalPot);
                        }
                        else
                        {
                            if (VerifyMyPlayerStateInGame())
                            {
                                LostGame();
                            }
                        }


                        float totalBet = (float)(gameState.totalPot);
                        if (GamePlayUI.instance.playerUIDetails.Exists(x => x.playerID == winningPlayerID))
                        {


                            PlayerUI winnerUI = GamePlayUI.instance.playerUIDetails.First(x => x.playerID == winningPlayerID);

                            if (winnerUI)
                            {
                                winnerUI.SetWinText();
                                if (!APIController.instance.userDetails.isBlockApiConnection)
                                {
                                    winnerUI.GetAmountFromPot((totalBet - (totalBet * APIController.instance.userDetails.commission)));

                                }
                                else
                                {
                                    winnerUI.GetAmountFromPot((totalBet));
                                }


                                if (!APIController.instance.userDetails.isBlockApiConnection)
                                {
                                    try
                                    {
                                        TeenpattiGameUIHandler.instance.Check("-" + ((totalBet * APIController.instance.userDetails.commission)).ToString("F2") + " " + APIController.instance.userDetails.currency_type);
                                    }
                                    catch
                                    {

                                    }

                                }
                            }


                        }
                    }

                }
                else
                {
                    yield return new WaitForSeconds(0f);
                    //yield return new WaitForSeconds(5f);
                    int currentid = 0;
                    bool finalwinner = false;
                    double allinAmount = 0;
                    double totalAmount = gameState.totalPot;
                    List<string> winnerdata = new List<string>();
                    foreach (PlayerState ps in winerps)
                    {

                        string winningPlayerID = ps.playerData.playerID;
                        double amountAdd = (float)ps.allinAmount - allinAmount;
                        if (ps.hasAllin && ps.allinPosition > currentid && !finalwinner && amountAdd > 0)
                        {
                            if (GamePlayUI.instance.playerUIDetails.Exists(x => x.playerID == winningPlayerID))
                            {
                                PlayerUI winnerUI = GamePlayUI.instance.playerUIDetails.First(x => x.playerID == winningPlayerID);
                                winnerdata.Add(winningPlayerID);
                                allinAmount += amountAdd;

                                if (winnerUI)
                                {
                                    winnerUI.SetWinText();
                                    if (!APIController.instance.userDetails.isBlockApiConnection)
                                    {
                                        winnerUI.GetAmountFromPot(((float)amountAdd) - ((float)amountAdd * APIController.instance.userDetails.commission));
                                    }
                                    else
                                    {
                                        winnerUI.GetAmountFromPot(((float)amountAdd));
                                    }


                                    if (!APIController.instance.userDetails.isBlockApiConnection)
                                    {
                                        try
                                        {
                                            TeenpattiGameUIHandler.instance.Check("-" + (((float)amountAdd * APIController.instance.userDetails.commission)).ToString("F2") + " " + APIController.instance.userDetails.currency_type);
                                        }
                                        catch
                                        {

                                        }

                                    }

                                    totalAmount -= amountAdd;

                                    //   winnerUI.GetAmountFromPot((float)ps.playerData.money);
                                }
                            }

                            currentid = ps.allinPosition;
                            if (myPlayerState.playerData.playerID == winningPlayerID)
                            {

                                WinningGame(amountAdd);
                            }
                            if (totalAmount == 0)
                                finalwinner = true;
                        }
                        else if (!ps.hasAllin && totalAmount > 0 && !finalwinner)
                        {
                            finalwinner = true;
                            if (GamePlayUI.instance.playerUIDetails.Exists(x => x.playerID == winningPlayerID))
                            {
                                PlayerUI winnerUI = GamePlayUI.instance.playerUIDetails.First(x => x.playerID == winningPlayerID);
                                winnerdata.Add(winningPlayerID);
                                if (winnerUI)
                                {
                                    winnerUI.SetWinText();
                                    if (!APIController.instance.userDetails.isBlockApiConnection)
                                    {
                                        winnerUI.GetAmountFromPot((float)totalAmount - ((float)totalAmount * APIController.instance.userDetails.commission));
                                    }
                                    else
                                    {
                                        winnerUI.GetAmountFromPot((float)totalAmount);
                                    }

                                    if (!APIController.instance.userDetails.isBlockApiConnection)
                                    {
                                        try
                                        {
                                            TeenpattiGameUIHandler.instance.Check("-" + (((float)totalAmount * APIController.instance.userDetails.commission)).ToString("F2") + " " + APIController.instance.userDetails.currency_type);
                                        }
                                        catch
                                        {

                                        }

                                    }
                                    //                                winnerUI.GetAmountFromPot((float)ps.playerData.money);
                                }
                                if (myPlayerState.playerData.playerID == winningPlayerID)
                                {

                                    WinningGame(gameState.totalPot);
                                }
                            }
                        }
                        if (!winnerdata.Contains(myPlayerState.playerData.playerID))
                        {
                            if (VerifyMyPlayerStateInGame())
                            {
                                LostGame();
                            }
                        }
                    }
                }

                if (!APIController.instance.userDetails.isBlockApiConnection)
                {
                    try
                    {
                        TeenpattiGameUIHandler.instance.Check("-" + (((float)gameState.totalPot * APIController.instance.userDetails.commission)).ToString("F2") + " " + APIController.instance.userDetails.currency_type);
                    }
                    catch
                    {

                    }

                }


            }

            StartCoroutine(nameof(ResetGame));
        }

        public void WinningGame(double amount)
        {
            UIGameController.totalEarnings += (amount - myPlayerState.CurrentGameSpend);
        }

        public void WinningGamePlayerServer(double amount, string playerid)
        {


            double val = ((gameController.CurrentAmountType == (CashType.CASH) && !gameController.IsTournament) ? ((amount)) : amount);
            PlayerState ps = GetPlayerState(playerid);
            Debug.Log("WinningGamePlayerServer Called" + ps.playerData.currency_type + " " + ps.playerData.platform + " " + ps.playerData.session_token);
            if (!gameController.IsTournament)
            {
                ps.playerData.currentTableChipsWon += val;
                ps.playerData.currentTableGamesWon += 1;
            }
            ps.playerData.money = ps.playerData.money + val;


            if (gameController.isBlockedAPI)
            {
                CheckWinnerAndLooser(ps.BetIndex, val, ps.CurrentGameSpend, "WinningGamePlayer", ps.playerData.playerID, "", gameState.totalPot);
            }
            else
            {
                TransactionMetaData val1 = new();
                val1.Amount = val;
                val1.Info = "WinningGamePlayer";
                //ShowServerAnimation(val, ps.CurrentGameSpend, ps.playerData.playerID);
                APIController.instance.WinningsBetMultiplayerAPI(ps.BetIndex, ps.BetId, val, ps.CurrentGameSpend, gameState.totalPot, val1, (x, c, y) =>
                {

                    if (x)
                    {

                        JObject jsonObject = JObject.Parse(y);
                        Debug.Log("Ludo Game Balance ==========> " + jsonObject["balance"].ToString());
                        ClientUpdateBalance(jsonObject["balance"].ToString(), ps.playerData.playerID);

                    }
                    else
                    {
                        HandleErrorCode(c, ps.playerData.playerID, y);
                        UpdateGameStateToServer();
                    }



                }
                , ps.playerData.playerID, false, true, gameController.gameName, gameController.operatorName, gameController.gameId, gameController.Commission, gameState.currentMatchToken, gameController.domainURL, ps.playerData.session_token, ps.playerData.currency_type, ps.playerData.platform, ps.playerData.token, gameController.environment, ps.playerData.money.ToString());
            }
        }




        public List<PlayerState> WinnerList()
        {
            gameState.zanthuCount = 3;
            List<PlayerState> rankDetails = new List<PlayerState>();
            string gamestateData = JsonUtility.ToJson(gameState);
            GameState tempState = JsonUtility.FromJson<GameState>(gamestateData);
            tempState.players.RemoveAll(x => x.hasPacked);
            if (tempState.players.Count > 1)
            {
                for (int i = 0; i < tempState.players.Count - 1; i++)
                {
                    for (int j = i + 1; j < tempState.players.Count; j++)
                    {

                        if (CardCombination.CompareCards(tempState.players[i].GetCurrentCards(), tempState.players[j].GetCurrentCards(), "0", 0, 0, 0))
                        {

                        }
                        else
                        {

                            string temp1 = JsonUtility.ToJson(tempState.players[i]);
                            string temp2 = JsonUtility.ToJson(tempState.players[j]);
                            tempState.players[i] = JsonUtility.FromJson<PlayerState>(temp2);
                            tempState.players[j] = JsonUtility.FromJson<PlayerState>(temp1);
                        }






                        /*if (gameController.CurrentGameMode == GameMode.ZANDU)
                        {
                            if (CardCombination.CompareCards(tempState.players[i].GetCurrentCards(), tempState.players[j].GetCurrentCards(), "Zandu", tempState.zanducards[0].rankCard, tempState.zanducards[1].rankCard, tempState.zanducards[2].rankCard))
                            {

                            }
                            else
                            {


                                string temp1 = JsonUtility.ToJson(tempState.players[i]);
                                string temp2 = JsonUtility.ToJson(tempState.players[j]);
                                tempState.players[i] = JsonUtility.FromJson<PlayerState>(temp2);
                                tempState.players[j] = JsonUtility.FromJson<PlayerState>(temp1);

                            }
                        }
                        else if (gameController.CurrentGameMode == GameMode.HUKAM)
                        {
                            if (CardCombination.CompareCards(tempState.players[i].GetCurrentCards(), tempState.players[j].GetCurrentCards(), "Zandu", tempState.zanducards[0].rankCard, tempState.zanducards[0].rankCard, tempState.zanducards[0].rankCard))
                            {

                            }
                            else
                            {

                                string temp1 = JsonUtility.ToJson(tempState.players[i]);
                                string temp2 = JsonUtility.ToJson(tempState.players[j]);
                                tempState.players[i] = JsonUtility.FromJson<PlayerState>(temp2);
                                tempState.players[j] = JsonUtility.FromJson<PlayerState>(temp1);
                            }
                        }
                        else if (gameController.CurrentGameMode == GameMode.AK47)
                        {
                            if (CardCombination.CompareCards(tempState.players[i].GetCurrentCards(), tempState.players[j].GetCurrentCards(), "Ak47", 0, 0, 0))
                            {

                            }
                            else
                            {

                                string temp1 = JsonUtility.ToJson(tempState.players[i]);
                                string temp2 = JsonUtility.ToJson(tempState.players[j]);
                                tempState.players[i] = JsonUtility.FromJson<PlayerState>(temp2);
                                tempState.players[j] = JsonUtility.FromJson<PlayerState>(temp1);
                            }
                        }
                        else if (gameController.CurrentGameMode == GameMode.MUFLIS)
                        {
                            if (CardCombination.CompareCards(tempState.players[i].GetCurrentCards(), tempState.players[j].GetCurrentCards(), "Muflis", 0, 0, 0))
                            {

                            }
                            else
                            {

                                string temp1 = JsonUtility.ToJson(tempState.players[i]);
                                string temp2 = JsonUtility.ToJson(tempState.players[j]);
                                tempState.players[i] = JsonUtility.FromJson<PlayerState>(temp2);
                                tempState.players[j] = JsonUtility.FromJson<PlayerState>(temp1);
                            }
                        }
                        else
                        {
                            
                        }*/
                    }
                    rankDetails.Add(tempState.players[i]);
                    if (i == tempState.players.Count - 2)
                        rankDetails.Add(tempState.players[i + 1]);
                }
            }
            else
            {
                if (tempState.players.Count > 0)
                    rankDetails.Add(tempState.players[0]);
            }
            return rankDetails;
        }

        #endregion

        #region USER_INTERACTION_LOGIC


        public void VerifyAndChaal(bool isBot)
        {

            if (isBot)
            {
                BotChaal();
            }
            else
            {
                Chaal();
            }
        }


        public void SideShowMaster(int myIndex, int secondPlayerIndex)
        {
            GlobalMessage((CommonFunctions.Instance.GetTruncatedPlayerName(gameState.players[GetCurrentPlayingPlayerIndex()].playerData.playerName) + " requesting SideShow with " + CommonFunctions.Instance.GetTruncatedPlayerName(gameState.players[secondPlayerIndex].playerData.playerName) + "."));
            gameState.isSideShowRequestSend = true;
            Debug.Log("send sideshow request");
            gameState.sideShowRequestSender = myIndex;
            gameState.sideShowRequestReceiver = secondPlayerIndex;
            gameState.Senderval = myIndex;
            gameState.Receiverval = secondPlayerIndex;
            gameState.sideShowRequestTime = NetworkTime.time;
            gameState.currentState = 3;
            ChallWithoutNextturnMaster(gameState.players[myIndex].playerData.playerID, false);
            UpdateGameStateToServer();
            Debug.Log("******************> " + myIndex + " ================> " + secondPlayerIndex);
            Debug.Log("******************> " + gameState.Senderval + " ================> " + gameState.Receiverval);
            Debug.Log("******************> " + gameState.sideShowRequestSender + " ================> " + gameState.sideShowRequestReceiver);

        }
        void SideShow()
        {

            List<PlayerState> contestingPlayers = GetContestingPlayers();
            StartCoroutine(fetchData());
            if (contestingPlayers.Count > 2)
            {
                if (gameState.isSideShowRequestSend == false)
                {
                    int secondPlayerIndex = GetSideShowIndex();
                    int myIndex = GetCurrentPlayingPlayerIndex();
                    if (myIndex == secondPlayerIndex)
                    {
                    }
                    else
                    {
                        Debug.Log("=============> " + myIndex + " ================> " + secondPlayerIndex);
                        myPlayer.SideShowCMD(myIndex, secondPlayerIndex);
                        GamePlayUI.instance.HideHud();
                    }
                }
            }
            else
            {
                if (myPlayer)
                {
                    GamePlayUI.instance.HideHud();
                }
                myPlayer.ShowCMD(myPlayerID);
            }
        }

        Coroutine checkValShow;
        public void ShowCardsMaster(string playerId)
        {
            GlobalMessage(((GetPlayerState(playerId).playerData.playerName) + " Shows"));
            gameState.ShowAnimationEffect = true;
            gameState.Senderval = GetPlayerIndex(playerId);
            gameState.Receiverval = GetPlayerIndex(gameState.players.Find(x => x.playerData.playerID != playerId && x.hasPacked == false).playerData.playerID);
            gameState.SenderShowval = playerId;
            gameState.ReceiverShowval = gameState.players.Find(x => x.playerData.playerID != playerId && x.hasPacked == false).playerData.playerID;
            gameState.currentState = 5;
            UpdateGameStateToServer();
            Debug.Log("===================> Show Card 1");
            if (checkValShow != null)
            {
                StopCoroutine(checkValShow);
            }
            checkValShow = StartCoroutine(DelcareWinenrShowAfterAnimation(playerId));
            Debug.Log("===================> Show Card 2");

        }

        public IEnumerator DelcareWinenrShowAfterAnimation(string playerId)
        {
            Debug.Log("===================> Show Card 3");
            yield return new WaitForSeconds(7);
            if (gameController.isBlockedAPI)
            {
                ShowAndEndGame();
            }
            Debug.Log("===================> Show Card 4");
            ChallWithoutNextturnMaster(playerId, false, true);
        }

        public void ShowAndEndGame()
        {
            Debug.Log("Show End Game Called ===================> ");
            OpenCardForShow();
            gameState.forceSee = false;
            gameState.currentState = 4;

        }

        public void OnDecreaseBet()
        {
            GamePlayUI.instance.GlowPlusButton.SetActive(true);
            GamePlayUI.instance.GlowMinusButton.SetActive(false);
            GamePlayUI.instance.increaseBet.image.color = GamePlayUI.instance.activeButtonColor;
            GamePlayUI.instance.increaseBet.image.sprite = GamePlayUI.instance.ActivePlusMinus;
            GamePlayUI.instance.Plus.color = GamePlayUI.instance.PlusMinus;
            GamePlayUI.instance.decreaseBet.image.color = GamePlayUI.instance.InactiveButtonColor;
            //  GamePlayUI.instance.decreaseBet.image.sprite = GamePlayUI.instance.InActivePlusMinus;
            GamePlayUI.instance.Minus.color = GamePlayUI.instance.Inactive;
            GamePlayUI.instance.increaseBet.interactable = true;
            GamePlayUI.instance.decreaseBet.interactable = false;
            GamePlayUI.instance.isStakeDoubled = false;
            if (myPlayerState.hasSeenCard)
                myPlayerState.currentBoot = gameState.currentStake;
            else
                myPlayerState.currentBoot = gameState.currentStake / 2;
            float val = myPlayerState.currentBoot;
            GamePlayUI.instance.UpdateChaalText(val);
        }

        public void OnIncreaseBet()
        {
            if (CanIncreaseBoot(myPlayerID))
            {
                GamePlayUI.instance.GlowPlusButton.SetActive(false);
                GamePlayUI.instance.GlowMinusButton.SetActive(true);
                GamePlayUI.instance.increaseBet.image.color = GamePlayUI.instance.InactiveButtonColor;
                //GamePlayUI.instance.increaseBet.image.sprite = GamePlayUI.instance.InActivePlusMinus;
                GamePlayUI.instance.Plus.color = new Color(0.5f, 1f, 0.4f, 0.2f);
                GamePlayUI.instance.decreaseBet.image.color = GamePlayUI.instance.activeButtonColor;
                GamePlayUI.instance.decreaseBet.image.sprite = GamePlayUI.instance.ActivePlusMinus;
                GamePlayUI.instance.Minus.color = GamePlayUI.instance.PlusMinus;
                GamePlayUI.instance.increaseBet.interactable = false;
                //GamePlayUI.instance.Plus.color = new Color(0.5f, 1f, 0.4f, 0.2f);
                GamePlayUI.instance.decreaseBet.interactable = true;
                GamePlayUI.instance.isStakeDoubled = true;
                GamePlayUI.instance.UpdateChaalText(GameManager.localInstance.myPlayerState.currentBoot);
                GamePlayUI.instance.isRaisedBet = true;
                if (myPlayerState.hasSeenCard)
                    myPlayerState.currentBoot = gameState.currentStake;
                else
                    myPlayerState.currentBoot = gameState.currentStake / 2;
                float val = myPlayerState.currentBoot;
                GamePlayUI.instance.UpdateChaalText(val);
            }
            else
            {
                GamePlayUI.instance.GlowPlusButton.SetActive(false);
                GamePlayUI.instance.GlowMinusButton.SetActive(false);
                GamePlayUI.instance.decreaseBet.image.color = GamePlayUI.instance.InactiveButtonColor;
                //    GamePlayUI.instance.decreaseBet.image.sprite = GamePlayUI.instance.InActivePlusMinus;
                GamePlayUI.instance.Minus.color = GamePlayUI.instance.Inactive;
                GamePlayUI.instance.increaseBet.image.color = GamePlayUI.instance.InactiveButtonColor;
                //   GamePlayUI.instance.increaseBet.image.sprite = GamePlayUI.instance.InActivePlusMinus;
                GamePlayUI.instance.Plus.color = new Color(0.5f, 1f, 0.4f, 0.2f);
                GamePlayUI.instance.increaseBet.interactable = false;
                GamePlayUI.instance.decreaseBet.interactable = false;
            }
        }

        public bool CanIncreaseBoot(string playerid = "")
        {
            if (playerid == "")
                playerid = myPlayerID;

            float newBoot = GameManager.localInstance.myPlayerState.currentBoot * 2;
            if (((myPlayerState.hasSeenCard ? currentTableModel.ChaalLimit : currentTableModel.ChaalLimit / 2) >= newBoot /*|| *//*UIGameController.CurrentGameMode == GameMode.NOLIMITS*/) && APIController.instance.userDetails.balance >= newBoot)
            {
                return true;
            }
            else
                return false;
        }

        public void VerifyAndSeeCards()
        {
            SeeCards();
        }

        void SeeCards()
        {

            Debug.Log("Check See cards data In Room");

            /* if (gameController.CurrentGameMode == GameMode.POTBLIND)
                 return;*/
            if (gameState.currentState == 2)
            {
                if (myPlayer)
                {
                    GlobalMessage((CommonFunctions.Instance.GetTruncatedPlayerName(myPlayerData.playerName) + " has Seen cards."));
                    GamePlayUI.instance.SeeButtonActive(false);
                    myPlayer.ShowCards(true);
                    GamePlayUI.instance.HandStrengthMeter();
                    GamePlayUI.instance.UpdateChaalText(myPlayerState.currentBoot);
                    foreach (var item in myPlayer.myUI.BackCardGlow)
                    {
                        Debug.Log(item.gameObject.activeSelf + "Back Card Glow");
                        item.SetActive(false);
                        myPlayer.myUI.isSeen = true;
                        Debug.Log(item.gameObject.activeSelf + "Back Card Glow2");

                    }

                }
                myPlayer.SeeCMD(myPlayerID);
            }
            else
            {
                GamePlayUI.instance.SeeButtonActive(false);
            }

        }
        public void SeeCardMaster(string playerid)
        {
            /* if (gameController.CurrentGameMode == GameMode.POTBLIND)
                 return;*/
            int currentPlayerIndex = GetPlayerIndex(playerid);
            gameState.players[currentPlayerIndex].SetCardsSeen();
            UpdateGameStateToServer();

        }

        public void VerifyAndPack()
        {
            PackPlayer();
        }
        public void VerifyAndPackBot()
        {
            DebugHelper.LogError("player packed bot");
            StartCoroutine(BotActivityAfterPack(GetCurrentPlayingPlayerID()));
            PackPlayerBot();
        }

        public void VerifyAndShow()
        {
            SideShow();
        }

        public void VerifyAndAllin()
        {
            Allin();
        }

        public void Allin()
        {

            StartCoroutine(fetchData());

            myPlayer.AllInCMD(myPlayerID);
            GamePlayUI.instance.HideHud();
        }
        public void Chaal()
        {

            StartCoroutine(fetchData());

            myPlayer.ChallCMD(myPlayerID, GamePlayUI.instance.isStakeDoubled);
            GamePlayUI.instance.HideHud();
        }

        public void AllinMaster(string playerID)
        {
            if (playerID != GetCurrentPlayingPlayerID()) return;
            gameState.isSideShowRequestSend = false;
            int currentPlayerIndex = GetCurrentPlayingPlayerIndex();
            Debug.Log(" ALL IN MASTER --- >  " + gameState.players[currentPlayerIndex].playerData.playerName);
            Debug.Log(" ALL IN MASTER --- >  " + gameState.players[currentPlayerIndex].playerData.money);

            if (gameState.players[currentPlayerIndex].playerData.money <= (gameState.players[currentPlayerIndex].hasSeenCard ? gameState.currentStake : (gameState.currentStake / 2)))
            {
                double myMoney = gameState.players[currentPlayerIndex].playerData.money;
                double newStake = myMoney;
                gameState.players[currentPlayerIndex].playerData.money -= newStake;
                gameState.players[currentPlayerIndex].CurrentGameSpend += newStake;

                gameState.players[currentPlayerIndex].hasAllin = true;

                DebugHelper.Log($"TotalPot =>>>>> Before --- {gameState.totalPot}");

                gameState.totalPot += newStake;

                DebugHelper.Log($"TotalPot =>>>>> After --- {gameState.totalPot}");

                gameState.players[currentPlayerIndex].allinAmount = gameState.totalPot - (gameState.currentStake - newStake);
                gameState.players[currentPlayerIndex].allinPosition = AllinPerformCount();


                if (currentTableModel.PotLimit <= gameState.totalPot)
                {

                    if (gameController.isBlockedAPI)
                    {
                        if (currentTableModel.PotLimit <= gameState.totalPot)
                        {
                            Debug.Log("SET WINNER FROM CHALL");
                            Debug.Log($"Pot Limit =>>>>> --- {gameState.totalPot}");
                            Debug.Log("AllinMaster ==================> STATE SET 4");
                            gameState.forceSee = false;
                            gameState.currentState = 4;
                            Debug.Log(" GAME STATE SET TO FOUR");

                        }
                    }


                }
                else
                {
                    PlayerState currentPlayerState = gameState.players[currentPlayerIndex];
                    GlobalMessage((CommonFunctions.Instance.GetTruncatedPlayerName(currentPlayerState.playerData.playerName) + " played a " + (currentPlayerState.hasSeenCard ? "chaal" : "blind") + " of " + CommonFunctions.Instance.GetAmountDecimalSeparator(newStake)));
                    NextPlayerTurn(false);
                }

                if (gameState.players[currentPlayerIndex].playerData.isBot)
                {
                    //Debug.Log($"SendChall 3 --- ");

                    if (gameController.isBlockedAPI)
                    {

                        ChallAmountBot(newStake, gameState.players[currentPlayerIndex].playerData.money, gameState.players[currentPlayerIndex].playerData.playerID, false);
                    }
                    else
                    {

                        Debug.Log($"ADD BET**** THREE" + myMoney);
                        TransactionMetaData _metaData = new TransactionMetaData();
                        _metaData.Amount = myMoney;
                        _metaData.Info = "Second Bet";
                        ShowServerAnimation(myMoney, gameState.players[currentPlayerIndex].playerData.money, gameState.players[currentPlayerIndex].playerData.playerID, false);

                        if (!OnlyBotExsist)
                        {
                            PlayerState ps = gameState.players[currentPlayerIndex];
                            APIController.instance.AddBetMultiplayerAPI(gameState.players[currentPlayerIndex].BetIndex, gameState.players[currentPlayerIndex].BetId, _metaData, myMoney, (x, c, y) =>
                            {
                                if (x)
                                {
                                    if (c != 200)
                                    {
                                         SubtractAmountforAddBetOnSessionFailed(newStake);
                                        HandleErrorCode(c, ps.playerData.playerID, y);
                                        UpdateGameStateToServer();
                                        return;
                                    }
                                    JObject jsonObject = JObject.Parse(y);
                                    Debug.Log("Teenpatti Game Balance ==========> " + jsonObject["balance"].ToString());
                                    ClientUpdateBalance(jsonObject["balance"].ToString(), ps.playerData.playerID);
                                    DebugHelper.Log("CHECK >>>>>>>>>>>>>  ***************" + gameState.totalPot + " ****************** " + GetPlayerState(playerID).CurrentGameSpend); SetTostateFour();

                                }
                                else
                                {
                                    SubtractAmountforAddBetOnSessionFailed(newStake);
                                    DebugHelper.Log("KICK ALL PLAYERS >>>>>>>>>>>>>  ***************");
                                    HandleErrorCode(c, ps.playerData.playerID, y);
                                    UpdateGameStateToServer();
                                    return;
                                }

                            }, gameState.players[currentPlayerIndex].playerData.playerID, true, gameController.gameName, gameController.operatorName, gameController.gameId, gameState.currentMatchToken, gameController.domainURL, ps.playerData.session_token, ps.playerData.currency_type, ps.playerData.platform, ps.playerData.token, gameController.environment, ps.playerData.money.ToString());
                        }
                        else
                        {
                            SetTostateFour();
                        }

                    }

                }
                else
                {
                    Debug.Log($"SendChall 3 --- ");
                    if (gameController.isBlockedAPI)
                    {
                        SendChall(playerID, newStake, gameState.players[currentPlayerIndex].playerData.money, false, false);
                    }
                    else
                    {
                        Debug.Log("ALL IN MASTER ======================== > ");
                        TransactionMetaData _metaData = new TransactionMetaData();
                        _metaData.Amount = myMoney;
                        _metaData.Info = "Second Bet";
                        ShowServerAnimation(myMoney, gameState.players[currentPlayerIndex].playerData.money, gameState.players[currentPlayerIndex].playerData.playerID, false);
                        PlayerState ps = gameState.players[currentPlayerIndex];
                        APIController.instance.AddBetMultiplayerAPI(gameState.players[currentPlayerIndex].BetIndex, gameState.players[currentPlayerIndex].BetId, _metaData, myMoney, (x, c, y) => {

                            if (x)
                            {
                                if (c != 200)
                                {
                                    SubtractAmountforAddBetOnSessionFailed(newStake);
                                    HandleErrorCode(c, ps.playerData.playerID, y);
                                    UpdateGameStateToServer();
                                    return;
                                }
                                JObject jsonObject = JObject.Parse(y);
                                Debug.Log("Ludo Game Balance ==========> " + jsonObject["balance"].ToString());
                                ClientUpdateBalance(jsonObject["balance"].ToString(), ps.playerData.playerID);
                                DebugHelper.Log("CHECK >>>>>>>>>>>>>  ***************" + gameState.totalPot + " ****************** " + GetPlayerState(playerID).CurrentGameSpend);
                                SetTostateFour();

                            }
                            else
                            {
                                SubtractAmountforAddBetOnSessionFailed(newStake);
                                DebugHelper.Log("KICK ALL PLAYERS >>>>>>>>>>>>>  ***************");
                                HandleErrorCode(c, ps.playerData.playerID, y);
                                UpdateGameStateToServer();
                                return;
                            }

                        }, gameState.players[currentPlayerIndex].playerData.playerID, false, gameController.gameName, gameController.operatorName, gameController.gameId, gameState.currentMatchToken, gameController.domainURL, ps.playerData.session_token, ps.playerData.currency_type, ps.playerData.platform, ps.playerData.token, gameController.environment, ps.playerData.money.ToString());
                    }


                }
                Debug.Log("ALL IN MASTER ======================== > " + gameState.players[currentPlayerIndex].playerData.playerName + " ************** " + gameState.players[currentPlayerIndex].playerData.money);
                GlobalMessage((CommonFunctions.Instance.GetTruncatedPlayerName(gameState.players[currentPlayerIndex].playerData.playerName) + " played All-In of " + CommonFunctions.Instance.GetAmountDecimalSeparator(myMoney)));

            }
            UpdateGameStateToServer();
        }

        public void SubtractAmountforAddBetOnSessionFailed(double newStake)
        {
            gameState.totalPot -= newStake;

        }

        public void SetTostateFour()
        {
            if (currentTableModel.PotLimit <= gameState.totalPot)
            {
                Debug.Log("SET WINNER FROM CHALL");
                Debug.Log($"Pot Limit =>>>>> --- {gameState.totalPot}");
                gameState.forceSee = false;
                gameState.currentState = 4;
                Debug.Log(" GAME STATE SET TO FOUR");

            }
        }
        bool BetCheck;
        public void Chaal(string playerID, bool isIncreaseBet)
        {
            if (playerID != GetCurrentPlayingPlayerID()) return;

            gameState.isSideShowRequestSend = false;
            int currentPlayerIndex = GetCurrentPlayingPlayerIndex();
            DebugHelper.Log("check current player index " + currentPlayerIndex);
            if (gameState.players[currentPlayerIndex].playerData.money <= (gameState.players[currentPlayerIndex].hasSeenCard ? gameState.currentStake : (gameState.currentStake / 2)))
            {
                AllinMaster(playerID);
            }
            else
            {
                double newStake = 0;
                if (gameState.players[currentPlayerIndex].playerData.isBot)
                {
                    if (CanIncreaseBoot(GetCurrentPlayingPlayerID()))
                    {
                        if (isIncreaseBet)
                        {
                            BetCheck = true;
                            Debug.Log(" **************** " + gameState.currentStake);
                            newStake = gameState.currentStake < 320 ? gameState.currentStake * 2 : gameState.currentStake;
                            Debug.Log("&&&&&**************** " + newStake);
                        }
                        else
                        {
                            BetCheck = false;
                            newStake = gameState.currentStake;
                            Debug.Log("&&&&&**************** " + newStake);
                        }
                    }
                    else
                    {
                        BetCheck = false;
                        newStake = gameState.currentStake;
                        Debug.Log("&&&&&**************** " + newStake);
                    }
                }
                else
                {
                    if (isIncreaseBet)
                    {
                        BetCheck = true;
                        Debug.Log(" **************** Player " + newStake);
                        newStake = gameState.currentStake * 2;
                    }
                    else
                    {
                        BetCheck = false;
                        Debug.Log(" **************** Player " + newStake);
                        newStake = gameState.currentStake;
                    }
                }
                Debug.Log(" **************** " + newStake);
                gameState.SetCurrentStake((int)newStake);

                if (!gameState.players[currentPlayerIndex].hasSeenCard)
                {
                    newStake /= 2;

                }

                gameState.players[currentPlayerIndex].playerData.money -= newStake;
                gameState.players[currentPlayerIndex].CurrentGameSpend += newStake;

                DebugHelper.Log($"TotalPot =>>>>> Before --- {gameState.totalPot}");

                gameState.totalPot += newStake;

                DebugHelper.Log($"TotalPot =>>>>> After --- {gameState.totalPot}");
                if (GetPlayerState(playerID).playerData.isBot)
                {

                    if (gameController.isBlockedAPI)
                    {

                        ChallAmountBot(newStake, gameState.players[currentPlayerIndex].playerData.money, gameState.players[currentPlayerIndex].playerData.playerID, false, BetCheck);
                    }
                    else
                    {
                        Debug.Log("AddBetCAlled BOT" + newStake + " *********************************** " + " *********************************************################################################################################### " + playerID);
                        TransactionMetaData _metaData = new TransactionMetaData();
                        _metaData.Amount = newStake;
                        _metaData.Info = "Second Bet";
                        ShowServerAnimation(newStake, gameState.players[currentPlayerIndex].playerData.money, gameState.players[currentPlayerIndex].playerData.playerID, BetCheck);

                        if (!OnlyBotExsist)
                        {
                            PlayerState ps = gameState.players[currentPlayerIndex];
                            APIController.instance.AddBetMultiplayerAPI(gameState.players[currentPlayerIndex].BetIndex, gameState.players[currentPlayerIndex].BetId, _metaData, newStake, (x, c, y) => {
                                if (x)
                                {
                                    if (c != 200)
                                    {
                                        SubtractAmountforAddBetOnSessionFailed(newStake);
                                        HandleErrorCode(c, ps.playerData.playerID, y);
                                        UpdateGameStateToServer();
                                        return;
                                    }
                                    JObject jsonObject = JObject.Parse(y);
                                    Debug.Log("Ludo Game Balance ==========> " + jsonObject["balance"].ToString());
                                    ClientUpdateBalance(jsonObject["balance"].ToString(), ps.playerData.playerID);
                                    AddWinnerAfterChaal();
                                    DebugHelper.Log("CHECK >>>>>>>>>>>>>  ***************" + gameState.totalPot + " ****************** " + GetPlayerState(playerID).playerData.playerName);

                                }
                                else
                                {
                                    SubtractAmountforAddBetOnSessionFailed(newStake);
                                    DebugHelper.Log("KICK ALL PLAYERS >>>>>>>>>>>>>  ***************");
                                    HandleErrorCode(c, ps.playerData.playerID, y);
                                    UpdateGameStateToServer();
                                    return;
                                }

                            }, gameState.players[currentPlayerIndex].playerData.playerID, true, gameController.gameName, gameController.operatorName, gameController.gameId, gameState.currentMatchToken, gameController.domainURL, ps.playerData.session_token, ps.playerData.currency_type, ps.playerData.platform, ps.playerData.token, gameController.environment, ps.playerData.money.ToString());
                        }
                        else
                        {
                            AddWinnerAfterChaal();
                        }

                    }
                }
                else
                {
                    ChallAmount(newStake, playerID);
                }

                PlayerState currentPlayerState = gameState.players[GetCurrentPlayingPlayerIndex()];
                GlobalMessage((CommonFunctions.Instance.GetTruncatedPlayerName(currentPlayerState.playerData.playerName) + " played a " + (currentPlayerState.hasSeenCard ? "chaal" : "blind") + " of " + CommonFunctions.Instance.GetAmountDecimalSeparator(newStake)));
                if (currentTableModel.PotLimit <= gameState.totalPot)
                {
                    if (gameController.isBlockedAPI)
                    {
                        Debug.Log("SET WINNER FROM CHALL BUTTON LOOTRIX ");
                        Debug.Log($"Pot Limit CHALL BUTTON =>>>>> LOOTRIX --- {gameState.totalPot}");
                        Debug.Log("Chaal ==================> STATE SET 4");
                        gameState.forceSee = false;
                        gameState.currentState = 4;
                    }

                }
                else
                {

                    NextPlayerTurn(false);
                }

                if (!GetPlayerState(playerID).playerData.isBot)
                {

                    if (gameController.isBlockedAPI)
                    {
                        SendChall(playerID, newStake, gameState.players[currentPlayerIndex].playerData.money, false, BetCheck);

                    }
                    else
                    {
                        TransactionMetaData _metaData = new TransactionMetaData();
                        _metaData.Amount = newStake;
                        _metaData.Info = "Second Bet";
                        Debug.Log("AddBetCAlled Player " + newStake + " *********************************** " + " *********************************************################################################################################### " + playerID);
                       // ShowServerAnimation(newStake, gameState.players[currentPlayerIndex].playerData.money, gameState.players[currentPlayerIndex].playerData.playerID, BetCheck);
                        PlayerState ps = gameState.players[currentPlayerIndex];
                        ShowServerAnimation(newStake, gameState.players[currentPlayerIndex].playerData.money, gameState.players[currentPlayerIndex].playerData.playerID, BetCheck);
                        APIController.instance.AddBetMultiplayerAPI(gameState.players[currentPlayerIndex].BetIndex, gameState.players[currentPlayerIndex].BetId, _metaData, newStake, (x, c, y) => {

                            if (x)
                            {
                                if (c != 200)
                                {
                                     SubtractAmountforAddBetOnSessionFailed(newStake);
                                    HandleErrorCode(c, ps.playerData.playerID, y);
                                    UpdateGameStateToServer();
                                    return;
                                }
                                JObject jsonObject = JObject.Parse(y);
                                Debug.Log("Ludo Game Balance ==========> " + jsonObject["balance"].ToString());
                                ClientUpdateBalance(jsonObject["balance"].ToString(), ps.playerData.playerID);
                                AddWinnerAfterChaal();
                                DebugHelper.Log(" CHECK >>>>>>>>>>>>>  ***************" + gameState.totalPot + " ****************** " + GetPlayerState(playerID).playerData.playerName);
                            }
                            else
                            {
                                SubtractAmountforAddBetOnSessionFailed(newStake);
                                DebugHelper.Log("KICK ALL PLAYERS >>>>>>>>>>>>>  ***************");
                                HandleErrorCode(c, ps.playerData.playerID, y);
                                UpdateGameStateToServer();
                                return;
                            }

                        }, gameState.players[currentPlayerIndex].playerData.playerID, false, gameController.gameName, gameController.operatorName, gameController.gameId, gameState.currentMatchToken, gameController.domainURL, ps.playerData.session_token, ps.playerData.currency_type, ps.playerData.platform, ps.playerData.token, gameController.environment, ps.playerData.money.ToString());
                    }
                }

                Debug.Log("CHALL --- >  " + gameState.players[currentPlayerIndex].playerData.playerName);
                Debug.Log("CHALL --- >  " + gameState.players[currentPlayerIndex].playerData.money);

            }
            UpdateGameStateToServer();
        }

        public void AddWinnerAfterChaal()
        {
            if (currentTableModel.PotLimit <= gameState.totalPot)
            {
                Debug.Log("SET WINNER FROM CHALL BUTTON");
                Debug.Log($"Pot Limit CHALL BUTTON =>>>>> --- {gameState.totalPot}");
                gameState.forceSee = false;
                gameState.currentState = 4;
                UpdateGameStateToServer();

            }

        }


        #endregion

        #region CARD

        public void ResetCards()
        {

            if ((gameState.currentState == 2 || gameState.currentState == 3) && (!myPlayerState.isSpectator || !myPlayerState.hasPacked))
            {

                return;
            }

            if (gameState.currentState != 0 && gameState.currentState != 1)
            {
                if (MasterAudioController.instance.CheckSoundToggle())
                    MasterAudioController.instance.PlayAudio(AudioEnum.PACK);
            }

            for (int j = 0; j < GamePlayUI.instance.playerUIDetails.Count; j++)
            {
                GamePlayUI.instance.playerUIDetails[j].ResetCard();
                GamePlayUI.instance.playerUIDetails[j].CloseWinnerBanner();
            }
            GamePlayUI.instance.HandStrengthMeter();

        }

        public CardData[] GetCards()
        {

            CardData[] newCardData = new CardData[3];
            for (int i = 0; i < 3; i++)
            {
                CardData randomCard = gameState.GetRandomCard();
                if (randomCard.rankCard == 1)
                    randomCard.rankCard = 14;
                /*if (gameController.CurrentGameMode == GameMode.JOKER && i == 2)
                {
                    randomCard.suitCard = CardSuit.Joker;
                    randomCard.rankCard = 15;
                }*/
                newCardData[i] = randomCard;
            }
            newCardData = SortedCards(newCardData);
            return newCardData;
        }


        public CardData[] SortedCards(CardData[] cards)
        {
            cards = cards.OrderBy(x => x.rankCard).ToArray();

            Array.Sort(cards, delegate (CardData card1, CardData card2)
            {
                return card1.rankCard.CompareTo(card2.rankCard);
            });

            if (cards[0].rankCard == 2 && cards[1].rankCard == 3 && cards[2].rankCard == 14)
            {
                CardData tempcard;
                tempcard = cards[0];
                cards[0] = cards[2];
                cards[2] = cards[1];
                cards[1] = tempcard;
            }
            else if (cards[0].rankCard == 2 && cards[1].rankCard == 14 && cards[2].rankCard == 15)
            {
                CardData tempcard;
                tempcard = cards[0];
                cards[0] = cards[1];
                cards[1] = tempcard;
            }

            return cards;
        }


        public IEnumerator DistributeCards()
        {

            isdistributCardEnd = false;
            isdistributCard = true;
            for (int j = 0; j < GamePlayUI.instance.playerUIDetails.Count; j++)
            {
                GamePlayUI.instance.playerUIDetails[j].CloseWinnerBanner();
            }

            for (int i = 0; i < 3; i++)
            {
                foreach (PlayerState ps in gameState.players)
                {
                    if (ps.hasPacked)
                        continue;
                    PlayerUI playerUI = GetPlayerUI(ps.playerData.playerID);
                    while (playerUI.playerID != ps.playerData.playerID)
                    {

                        yield return new WaitForSeconds(0.1f);
                        playerUI = GetPlayerUI(ps.playerData.playerID);
                        DebugHelper.LogError("One card error");



                    }


                    playerUI.DealCard(i);


                }


                yield return new WaitForSeconds(0.3f);



            }


            yield return new WaitForSeconds(1.5f);
            /* if (UIGameController.CurrentGameMode == GameMode.HUKAM)
             {
                 GamePlayUI.instance.HukamSetCard(gameState.zanducards[0]);
             }
             else
             {
                 GamePlayUI.instance.hukamCards.SetActive(false);
             }*/
            GamePlayUI.instance.hukamCards.SetActive(false);
            if (myPlayerState.hasSeenCard)
            {
                AutoSee();
            }
            isdistributCardEnd = true;
            GamePlayUI.instance.SeeButtonActive(true);
            UpdateNetworkGame();
        }

        public void playwithSomeDelay()
        {
            if (MasterAudioController.instance.CheckSoundToggle())
                MasterAudioController.instance.PlayAudio(AudioEnum.CHIPSOUND);
        }

        #endregion

        #region GENERATE_RANK

        public void GenerateRank()
        {
            PlayerState[] rankDetails = new PlayerState[gameState.players.Count];
            botPlayer.rankDetails.Clear();
            PlayerState winner = new PlayerState();
            List<PlayerState> botList = new List<PlayerState>();
            string gamestateData = JsonUtility.ToJson(gameState);
            GameState tempState = JsonUtility.FromJson<GameState>(gamestateData);
            for (int i = 0; i < tempState.players.Count - 1; i++)
            {
                for (int j = i + 1; j < tempState.players.Count; j++)
                {


                    if (CardCombination.CompareCards(tempState.players[i].GetCurrentCards(), tempState.players[j].GetCurrentCards(), "0", 0, 0, 0))
                    {

                    }
                    else
                    {

                        string temp1 = JsonUtility.ToJson(tempState.players[i]);
                        string temp2 = JsonUtility.ToJson(tempState.players[j]);
                        tempState.players[i] = JsonUtility.FromJson<PlayerState>(temp2);
                        tempState.players[j] = JsonUtility.FromJson<PlayerState>(temp1);
                    }



                    /*if (gameController.CurrentGameMode == GameMode.ZANDU)
                    {
                        if (CardCombination.CompareCards(tempState.players[i].GetCurrentCards(), tempState.players[j].GetCurrentCards(), "Zandu", tempState.zanducards[0].rankCard, tempState.zanducards[1].rankCard, tempState.zanducards[2].rankCard))
                        {

                        }
                        else
                        {


                            string temp1 = JsonUtility.ToJson(tempState.players[i]);
                            string temp2 = JsonUtility.ToJson(tempState.players[j]);
                            tempState.players[i] = JsonUtility.FromJson<PlayerState>(temp2);
                            tempState.players[j] = JsonUtility.FromJson<PlayerState>(temp1);

                        }
                    }
                    else if (gameController.CurrentGameMode == GameMode.HUKAM)
                    {
                        if (CardCombination.CompareCards(tempState.players[i].GetCurrentCards(), tempState.players[j].GetCurrentCards(), "Zandu", tempState.zanducards[0].rankCard, tempState.zanducards[0].rankCard, tempState.zanducards[0].rankCard))
                        {

                        }
                        else
                        {

                            string temp1 = JsonUtility.ToJson(tempState.players[i]);
                            string temp2 = JsonUtility.ToJson(tempState.players[j]);
                            tempState.players[i] = JsonUtility.FromJson<PlayerState>(temp2);
                            tempState.players[j] = JsonUtility.FromJson<PlayerState>(temp1);
                        }
                    }
                    else if (gameController.CurrentGameMode == GameMode.AK47)
                    {
                        if (CardCombination.CompareCards(tempState.players[i].GetCurrentCards(), tempState.players[j].GetCurrentCards(), "Ak47", 0, 0, 0))
                        {

                        }
                        else
                        {

                            string temp1 = JsonUtility.ToJson(tempState.players[i]);
                            string temp2 = JsonUtility.ToJson(tempState.players[j]);
                            tempState.players[i] = JsonUtility.FromJson<PlayerState>(temp2);
                            tempState.players[j] = JsonUtility.FromJson<PlayerState>(temp1);
                        }
                    }
                    else if (gameController.CurrentGameMode == GameMode.MUFLIS)
                    {
                        if (CardCombination.CompareCards(tempState.players[i].GetCurrentCards(), tempState.players[j].GetCurrentCards(), "Muflis", 0, 0, 0))
                        {

                        }
                        else
                        {

                            string temp1 = JsonUtility.ToJson(tempState.players[i]);
                            string temp2 = JsonUtility.ToJson(tempState.players[j]);
                            tempState.players[i] = JsonUtility.FromJson<PlayerState>(temp2);
                            tempState.players[j] = JsonUtility.FromJson<PlayerState>(temp1);
                        }
                    }
                    else
                    {
                        if (CardCombination.CompareCards(tempState.players[i].GetCurrentCards(), tempState.players[j].GetCurrentCards(), "0", 0, 0, 0))
                        {

                        }
                        else
                        {

                            string temp1 = JsonUtility.ToJson(tempState.players[i]);
                            string temp2 = JsonUtility.ToJson(tempState.players[j]);
                            tempState.players[i] = JsonUtility.FromJson<PlayerState>(temp2);
                            tempState.players[j] = JsonUtility.FromJson<PlayerState>(temp1);
                        }
                    }*/
                }
                botPlayer.rankDetails.Add(tempState.players[i].playerData.playerID);
                if (i == tempState.players.Count - 2)
                    botPlayer.rankDetails.Add(tempState.players[i + 1].playerData.playerID);

                if (tempState.players[i].playerData.isBot)
                {
                    botList.Add(tempState.players[i]);

                }
                else if (tempState.players[i + 1].playerData.isBot)
                {
                    botList.Add(tempState.players[i + 1]);
                }

            }
            foreach (PlayerState ps in gameState.players)
            {
                if (!botPlayer.rankDetails.Contains(ps.playerData.playerID))
                {
                    botPlayer.rankDetails.Add(ps.playerData.playerID);
                }
            }
            if (isBotWin)
            {
                botList.OrderBy(x => x.winnintSteak);
                PlayerState bot = botList[0];
                winner = GetPlayerState(botPlayer.rankDetails[0]);
                int botindex = 0;
                foreach (string data in botPlayer.rankDetails)
                {
                    if (GetPlayerState(data).playerData.isBot)
                    {
                        bot = GetPlayerState(data);
                        break;
                    }
                    else
                    {
                        botindex += 1;
                    }
                }
                if (!winner.playerData.isBot)
                {
                    DebugHelper.LogError(bot.playerData.currentCards[0].rankCard + " enter boot " + winner.playerData.currentCards[0].rankCard);
                    CardData[] winnercards = new CardData[3];
                    CardData[] botcards = new CardData[3];
                    for (int i1 = 0; i1 < 3; i1++)
                    {
                        winnercards[i1] = new CardData(winner.playerData.currentCards[i1].suitCard, winner.playerData.currentCards[i1].rankCard, winner.playerData.currentCards[i1].isClose);
                        botcards[i1] = new CardData(bot.playerData.currentCards[i1].suitCard, bot.playerData.currentCards[i1].rankCard, bot.playerData.currentCards[i1].isClose);
                    }
                    winner.playerData.currentCards = botcards;
                    bot.playerData.currentCards = winnercards;
                    DebugHelper.LogError(bot.playerData.currentCards[0].rankCard + " enter boot exit " + winner.playerData.currentCards[0].rankCard);
                    botPlayer.rankDetails[0] = bot.playerData.playerID;
                    botPlayer.rankDetails[botindex] = winner.playerData.playerID;
                }
            }
            foreach (PlayerState ps in gameState.players)
            {
                ps.playerData.currentCombination = CardCombination.GetCombinationFromCard(ps.playerData.currentCards);
                ps.cardStrength = (int)ps.playerData.currentCombination;
            }
            playerRankDetails = botPlayer.rankDetails;
        }

        #endregion

        [ClientRpc]
        public void SendErrorMessageRPC(byte[] jsonBytes)
        {
            string json = Encoding.UTF8.GetString(jsonBytes);
            DebugHelper.Log("ServerKick ========> " + json);
            var data = JsonUtility.FromJson<ShowErrorMessage>(json);
            if (APIController.instance.userDetails.Id == data.PlayerID)
            {
                UIController.Instance.serverKick.ShowPopup(data.Message, data.Code);
            }
        }




        #region SERVER_SIDE_LOGIC
        bool StateSeven;
        bool StateEight;
        bool checkOnce;
        bool showResultOnce;
        [Server]
        public void CheckActionRequired()
        {
            foreach (SWEvent request in requestList)
            {
                request.eventAction.Invoke();
                gameState.completedRequests.Add(request.GetEventID());
            }

            switch (gameState.currentState)
            {
                case 0:

                    if (gameState.players.Count > 0)
                    {

                        var playersSnapshot = gameState.players.ToArray();
                        for (int i = 0; i < playersSnapshot.Length; i++)
                        {
                            if (playersSnapshot[i].currentState == 7)
                            {
                                string json = JsonUtility.ToJson(new ShowErrorMessage
                                {
                                    PlayerID = playersSnapshot[i].playerData.playerID,
                                    Message = gameState.MessageToDisplay,
                                    Code = gameState.Code
                                });

                                byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
                                SendErrorMessageRPC(jsonBytes);
                                DebugHelper.Log("Before Cancel Bet Called ===============>" + playersSnapshot[i].playerData.playerName);
                                APIController.instance.CancelBetMultiplayerAPI((x, y, z) => { }, playersSnapshot[i].playerData.playerID, gameController.gameName, gameController.operatorName, gameController.gameId, gameState.currentMatchToken, playersSnapshot[i].playerData.session_token, playersSnapshot[i].playerData.currency_type
                                , gameController.environment);
                                DebugHelper.Log("After Cancel Bet Called ===============>" + playersSnapshot[i].playerData.playerName);
                                GetPlayerPUN(playersSnapshot[i].playerData.playerID).ServerKick("Kick");
                                RemovePlayerByServer(playersSnapshot[i]);
                                gameState.currentMatchToken = string.Empty;
                                playersList.Clear();
                            }
                            if (GetRealPlayersCountInGame() < 1)
                            {

                                if (playersSnapshot[i].playerData.isBot)
                                {
                                    APIController.instance.CancelMatchAPI((x, y, z) => { }, gameController.gameName, gameController.operatorName, gameController.gameId, gameState.currentMatchToken, playersSnapshot[i].playerData.session_token, gameController.currency, gameController.environment);
                                    RemovePlayerByServer(playersSnapshot[i]);
                                    DebugHelper.Log("Kick Player ============= 1> " + GetRealPlayersCountInGame());
                                }


                            }
                            DebugHelper.Log("Kick Player ============= 2> " + GetRealPlayersCountInGame());
                        }
                        DebugHelper.Log("Kick Player ============= 3> " + GetRealPlayersCountInGame());
                    }


                    GameTimerOnce = false;
                    FirstInitBetSuccess = false;
                    IsSucces = false;
                    CallThisOnce = false;
                    StateEight = false;
                    StateSeven = false;
                    showResultOnce = false;
                    gameState.RoundCount = 0;
                    gameStart = false;
                    StopCoroutine(nameof(StartGameTimerServer));
                    CheckForRemoveAllBots();
                    AddPlayersToGame(false);

                    if (GetRealPlayersCountInGame() < 3)
                    {

                        Debug.Log("CheckActionRequired==");
                        AddBotFromAPI();
                        Debug.Log("**** Went in Add Bot");
                    }
                    else
                    {
                        Debug.Log("Chek The Bot added Concept======>2");
                        StopCoroutine(nameof(AddBotIfRequired));
                        Debug.Log("****  NOT Went in Add Bot");
                    }
                    isrejoin = false;
                    checkOnce = true;
                    gameState.ShowAnimationEffect = false;
                    gameState.allPacked = false;
                    break;
                case 1:
                    gameState.RoundCount = 0;
                    ToggleSpectatorUsers();
                    AddPlayersToGame(true);
                    if (GetRealPlayersCountInGame() < 3)
                    {
                        AddBotFromAPI();
                        Debug.Log("**** Went in Add Bot");
                    }
                    else
                    {
                        Debug.Log("Chek The Bot added Concept======>3");
                        StopCoroutine(nameof(AddBotIfRequired));
                        Debug.Log("**** NOT Went in Add Bot");
                    }
                    gameState.ShowAnimationEffect = false;
                    StopCoroutine(nameof(StartGameTimerServer));
                    StartCoroutine(nameof(StartGameTimerServer));
                    isrejoin = false;
                    isWinnerForUpdateClient = false;
                    gameState.allPacked = false;
                    break;
                case 2:
                    StateEight = false;
                    StateSeven = false;
                    ToggleSpectatorUsers();
                    if (GetActivePlayedPlayerCount() <= 1)
                    {
                        DebugHelper.LogError("game state update 4");
                        DebugHelper.Log("CheckActionRequired 1 ==================> STATE SET 4");
                        gameState.currentState = 4;

                    }

                    if (GetNotDisconnectedPlayerCount() == 0)
                    {
                        DebugHelper.Log("###################  Player has Disconnected");
                        DebugHelper.Log("CheckActionRequired 2 ==================> STATE SET 4");
                        gameState.currentState = 4;

                    }
                    IsSideShowChecked = false;
                    CheckBotTurn();
                    /*if(!isWinnerForUpdateClient && gameState.currentState != 4)
                    {
                        if (GetActivePlayersCount() > 0)
                        {
                            Debug.Log("Client Update Server For UI -------------------------> ");
                            //ClientUpdateBalance();
                        }
                     
                    }*/


                    break;
                case 3:
                    StateEight = false;
                    StateSeven = false;
                    showResultOnce = false;
                    gameState.ShowAnimationEffect = false;
                    CheckSideShowRequestServer();
                    break;
                    
                case 4:
                    CallThisOnce = false;
                    StateEight = false;
                    StateSeven = false;
                    gameState.ShowAnimationEffect = false;
                    isWinnerForUpdateClient = true;
                    isrejoin = false;
                    gameStart = false;
                    DebugHelper.Log("State 4 set");
                    if (!showResultOnce)
                    {
                        showResultOnce = true;
                        GameTimerOnce = false;
                    }
                    StartCoroutine(ShowWinnerServer());
                    break;
                case 5:
                    StateEight = false;
                    StateSeven = false;
                    showResultOnce = false;
                    break;
                case 7:
                    if (!StateSeven)
                    {
                        StateSeven = true;
                        string json = JsonUtility.ToJson(new ShowErrorMessage
                        {
                            PlayerID = "",
                            Message = gameState.MessageToDisplay,
                            Code = 0
                        });
                        byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
                        showStateSevenPopUp(jsonBytes);
                        foreach (var item in roomInfo.players.ToArray())
                        {

                            RemovePlayerByServer(item.playerManager.myPlayerState);

                        }
                    }

                    DebugHelper.Log("===============> Show Emergency PopUp");
                    break;
                case 8:
                    if (!StateEight)
                    {
                        StateEight = true;
                        foreach (var item in roomInfo.players.ToArray())
                        {
                            RemovePlayerByServer(item.playerManager.myPlayerState);
                        }
                    }
                    DebugHelper.Log("===============> Show Emergency PopUp");
                    break;
            }
            UpdateGameStateToServer();
        }



        [ClientRpc]
        public void showStateSevenPopUp(byte[] jsonBytes)
        {
            string json = Encoding.UTF8.GetString(jsonBytes);
            DebugHelper.Log("State7 ========> " + json);
            var data = JsonUtility.FromJson<ShowErrorMessage>(json);

            if (!UIController.Instance.matchClosed.activeSelf)
            {

                UIController.Instance.Message = data.Message;
                UIController.Instance.matchClosed.SetActive(transform);
                DebugHelper.Log("===============> Show Emergency PopUp");
            }
        }






        [Server]
        public void AddPlayersToGame(bool isGameStarted)
        {
            if ((gameState.waitingPlayers.Count + gameState.players.Count > 1))
            {

                foreach (PlayerState ps in gameState.waitingPlayers)
                {
                    if (!ps.isSpectator && !gameState.players.Exists(x => x.playerData.playerID == ps.playerData.playerID))
                    {
                        gameState.players.Add(ps);
                        DebugHelper.LogError("new player added...");
                    }
                }
                gameState.waitingPlayers.RemoveAll(x => !x.isSpectator);

            }

            if (gameController.CurrentGameType == GameType.PRIVATE)
            {

                return;
            }
            if (gameController.IsTournament)
            {
                if (gameState.players.Count >= 5)
                {
                    gameState.UpdateCurrentTournamentPlayers();

                    foreach (PlayerState ps in gameState.players)
                    {
                        ps.currentState = 1;
                        if (ps.cardStrength == -1)
                        {
                            ps.playerData.currentCards = GetCards();
                            ps.playerData.currentCombination = CardCombination.GetCombinationFromCard(ps.playerData.currentCards);
                            ps.cardStrength = (int)ps.playerData.currentCombination;
                        }
                    }
                    if (!isGameStarted)
                    {
                        gameState.currentTournamentResultModel.tournamentId = gameController.CurrentTournamentModel.TournamemtId;
                        gameState.currentTournamentResultModel.roundNo = gameController.CurrentTournamentModel.currentPlayingRound;
                        gameState.currentTournamentResultModel.isPoker = gameController.CurrentTournamentModel is PokerTournamentModel;
                        roomInfo.isClosed = true;
                        roomInfo.isVisible = false;
                        DebugHelper.Log("start game timer");
                        gameState.gameStartTime = NetworkTime.time;
                        gameState.currentState = 1;
                    }
                }
                return;
            }
            if (gameState.players.Count > 1)
            {
                foreach (PlayerState ps in gameState.players)
                {
                    ps.currentState = 1;
                    if (ps.cardStrength == -1)
                    {
                        ps.playerData.currentCards = GetCards();
                        ps.playerData.currentCombination = CardCombination.GetCombinationFromCard(ps.playerData.currentCards);
                        ps.cardStrength = (int)ps.playerData.currentCombination;
                    }
                }
                if (!isGameStarted)
                {
                    DebugHelper.Log("start game timer");
                    gameState.gameStartTime = NetworkTime.time;
                    gameState.currentState = 1;
                }
            }
        }

        public void UpdateGameStateToServer()
        {
            if (gameState.currentState == 4)
                DebugHelper.LogError("gamestate update to 4 showwinner");
            gameState.gameController = gameController;
            foreach (MirrorPlayer mp in roomInfo.players)
            {
                PlayerManager pm = mp.playerManager;
                if (pm != null)
                {
                    if (gameState.players.Exists(x => x.playerData.playerID == pm.playerID))
                    {
                        pm.myPlayerStateJson = JsonUtility.ToJson(GetPlayerState(pm.playerID));
                    }
                    else if (gameState.waitingPlayers.Exists(x => x.playerData.playerID == pm.playerID))
                    {
                        pm.myPlayerStateJson = JsonUtility.ToJson(GetPlayerState(pm.playerID));
                    }
                }
            }
            string newGameStateJson = JsonUtility.ToJson(gameState);
            if (gameStateJson != newGameStateJson)
            {
                if (playercount != (gameState.players.Count + gameState.waitingPlayers.Count))
                    DebugHelper.Log("update in players.....");
                gameStateJson = newGameStateJson;
                roomInfo.gameState = gameState;
                RemoveCompletedEvents();
                CheckActionRequired();

            }

        }

        void RemovePlayerByServer(PlayerState ps)
        {
            RemoveFromGame(ps.playerData.playerID, false);
            GameObject go = GetBotObject(ps.playerData.playerID);
            botPlayers.Remove(go);
            MirrorManager.instance.RemoveBot(go);
        }

        IEnumerator ResetServer(int timer)
        {
            yield return new WaitForSeconds(3f);
            botPlay = null;
            if (gameState.players.FindAll(x => x.playerData.money < currentTableModel.BootAmount).Count != 0)
                yield return new WaitForSeconds(2.5f);
            while (gameState.players.FindAll(x => x.playerData.money < currentTableModel.BootAmount).Count != 0)
            {
                PlayerState ps = gameState.players.Find(x => x.playerData.money < currentTableModel.BootAmount);
                if (ps != null && !String.IsNullOrEmpty(ps.playerData.playerID))
                {
                    InactiveUserKick(ps.playerData.playerID, StaticStrings.EnoughBalanceServerKick + "" + (gameController.CurrentAmountType == CashType.SILVER ? StaticStrings.Chip : StaticStrings.Cash));
                }
            }


            if (gameState.players.Exists(x => x.playerData.money < (currentTableModel.BootAmount + 1)))
            {
                PlayerState psVal = gameState.players.Find(x => x.playerData.money < (currentTableModel.BootAmount + 1));
                if (psVal != null && !String.IsNullOrEmpty(psVal.playerData.playerID))
                {
                    InactiveUserKick(psVal.playerData.playerID, StaticStrings.EnoughBalanceServerKick + "" + (gameController.CurrentAmountType == CashType.SILVER ? StaticStrings.Chip : StaticStrings.Cash));
                }
            }


            int count = 0;
            foreach (PlayerState ps in gameState.waitingPlayers)
            {
                if (ps.isSpectator)
                {
                    if (ps.playerData.isBot && (UnityEngine.Random.Range(1, 3) == 2 || count >= UnityEngine.Random.Range(0, 1)))
                    {
                        ps.isSpectator = false;
                        ps.ResetState();
                    }
                    else
                    {
                        count += 1;
                    }
                }
            }

            count = 0;
            yield return new WaitForSeconds(2);
            if (GetRealPlayersCountInGame() == 0)
            {
                foreach (var item in roomInfo.players.ToArray())
                {
                    if (item.playerManager.myPlayerData.isBot)
                    {
                        RemovePlayerByServer(item.playerManager.myPlayerState);
                    }
                }
            }
            else
            {
                foreach (var item in roomInfo.players.ToArray())
                {
                    if (item.playerManager.myPlayerData.isBot && item.playerManager.myPlayerData.money <= 100)
                    {

                        DebugHelper.Log("Bot Removed Due to Low Balance <================================>" + item.playerManager.myPlayerData.money);
                        RemovePlayerByServer(item.playerManager.myPlayerState);
                    }
                }
            }



            gameState.removedPlayers.Clear();
            yield return new WaitForSeconds(1.5f);
            CallThisOnce = false;
            InitNewRoundState();
            UpdateGameStateToServer();

        }


        IEnumerator ShowWinnerServer()
        {


            isAddingBot = false;
            if (gameController.IsTournament)
            {
                if (gameController.CurrentTournamentModel.IsAllIn)
                {
                    yield return new WaitForSeconds(2.5f);
                }
            }
            DebugHelper.LogError("game state update 4");
            gameState.currentState = 4;

            if (isWinnerDisplayed == false)
            {
                Debug.Log("############################################ SHOW WINNER ======================== > ");
                isWinnerDisplayed = true;
                gameState.forceSee = false;

                if (GetNotDisconnectedPlayerCount() == 0)
                {



                    double maxval = gameState.players.Max(x => x.disconnectTime);
                    Debug.Log("############################################ Disconnected Time ===>   max====================> " + maxval);
                    for (int i = 0; i < gameState.players.Count; i++)
                    {
                        if (gameState.players[i].disconnectTime == maxval)
                        {
                            Debug.Log("############################################ Check Winner " + gameState.players[i].playerData.playerName + " ======= > " + gameState.players[i].disconnectTime);
                            WinningGamePlayerServer(gameState.totalPot, gameState.players[i].playerData.playerID);
                        }
                        else
                        {
                            Debug.Log("############################################ Check Losser " + gameState.players[i].playerData.playerName + " ======= > " + gameState.players[i].disconnectTime);
                            LossGamePlayerServer(gameState.totalPot, gameState.players[i].playerData.playerID);
                        }
                    }



                    UpdateGameStateToServer();
                    Debug.Log("############################################ ALL PLAYER DISCONNECTED =================>");
                    yield break;



                    //var groupsWithSameDisconnectTime = gameState.players.GroupBy(player => player.disconnectTime).Where(group => group.Count() == 5);
                    //foreach (var group in groupsWithSameDisconnectTime)
                    //{
                    //    foreach (var player in group)
                    //    {
                    //        WinningGamePlayerServer((gameState.totalPot / gameState.players.Count ) , player.playerData.playerID);
                    //    }
                    //}

                    //var groupsWithSameDisconnectTime1 = gameState.players.GroupBy(player => player.disconnectTime).Where(group => group.Count() == 4);
                    //foreach (var group in groupsWithSameDisconnectTime1)
                    //{
                    //    foreach (var player in group)
                    //    {
                    //        WinningGamePlayerServer((gameState.totalPot / gameState.players.Count), player.playerData.playerID);
                    //    }
                    //}
                    //var groupsWithSameDisconnectTime2 = gameState.players.GroupBy(player => player.disconnectTime).Where(group => group.Count() == 3);
                    //foreach (var group in groupsWithSameDisconnectTime2)
                    //{
                    //    foreach (var player in group)
                    //    {
                    //        WinningGamePlayerServer((gameState.totalPot / gameState.players.Count), player.playerData.playerID);
                    //    }
                    //}
                    //var groupsWithSameDisconnectTime3 = gameState.players.GroupBy(player => player.disconnectTime).Where(group => group.Count() == 2);
                    //foreach (var group in groupsWithSameDisconnectTime3)
                    //{
                    //    foreach (var player in group)
                    //    {
                    //        WinningGamePlayerServer((gameState.totalPot / gameState.players.Count), player.playerData.playerID);
                    //    }
                    //}

                }


                if (gameController.IsTournament)
                {
                    gameState.CurrentGameCount += 1;
                    gameState.UpdateCurrentTournamentPlayers();
                }
                List<PlayerState> winerps = WinnerList();
                if (gameController.IsTournament)
                {
                    if (gameController.CurrentTournamentModel.IsAllIn)
                        gameState.setRankForAllInTournament(winerps);
                }
                if (!winerps[0].hasAllin)
                {
                    PlayerState winnerStateFinal = winerps[0];

                    string winningPlayerID = winnerStateFinal.playerData.playerID;

                    if (winnerStateFinal.playerData.isBot)
                    {
                        WinningGameBotServer(gameState.totalPot, winnerStateFinal);
                    }
                    else
                    {
                        WinningGamePlayerServer(gameState.totalPot, winnerStateFinal.playerData.playerID);
                    }

                    float totalBet = (float)(gameState.totalPot);
                    for (int i = 1; i < winerps.Count; i++)
                    {
                        PlayerState ps = winerps[i];
                        if (ps.playerData.isBot)
                        {
                            LostGameBotServer(gameState.totalPot, ps.playerData.playerID);
                        }
                        else
                        {
                            LossGamePlayerServer(gameState.totalPot, ps.playerData.playerID);
                        }
                    }

                }
                else
                {
                    Debug.Log("SHOW ALL IN WINNER AFTER DELAY =============> ");
                    yield return new WaitForSeconds(3f);
                    Debug.Log("SHOW ALL IN WINNER AFTER DELAY =============> 1 ");
                    int currentid = 0;
                    bool finalwinner = false;
                    double allinAmount = 0;
                    double totalAmount = gameState.totalPot;
                    List<string> winnerdata = new List<string>();
                    foreach (PlayerState ps in winerps)
                    {
                        string winningPlayerID = ps.playerData.playerID;
                        double amountAdd = (float)ps.allinAmount - allinAmount;
                        if (ps.hasAllin && ps.allinPosition > currentid && !finalwinner && amountAdd > 0)
                        {
                            winnerdata.Add(winningPlayerID);
                            allinAmount += amountAdd;

                            currentid = ps.allinPosition;

                            if (ps.playerData.isBot)
                            {

                                WinningGameBotServer(amountAdd, ps);
                                totalAmount -= amountAdd;

                            }
                            else
                            {
                                WinningGamePlayerServer(amountAdd, ps.playerData.playerID);
                                totalAmount -= amountAdd;

                            }
                            if (totalAmount == 0)
                                finalwinner = true;
                        }
                        else if (!ps.hasAllin && totalAmount > 0 && !finalwinner)
                        {
                            finalwinner = true;
                            winnerdata.Add(winningPlayerID);
                            if (ps.playerData.isBot)
                            {

                                WinningGameBotServer(totalAmount, ps);



                            }
                            else
                            {
                                WinningGamePlayerServer(totalAmount, ps.playerData.playerID);

                            }
                        }
                    }
                    foreach (PlayerState ps in winerps)
                    {
                        if (!winnerdata.Contains(ps.playerData.playerID))
                        {
                            if (ps.playerData.isBot)
                            {

                                LostGameBotServer(totalAmount, ps.playerData.playerID);



                            }
                            else
                            {
                                LossGamePlayerServer(totalAmount, ps.playerData.playerID);

                            }

                        }
                    }
                }



                UpdateGameStateToServer();
                StartCoroutine(ResetServer(5));
            }
        }
        [ClientRpc]
        public void ShowServerAnimation(double amount, double balance, string PlayerID, bool BootCollection)
        {
            PlayerUI myUI = GetPlayerUI(PlayerID);
            if (myUI != null)
                myUI.GiveAmountToPot(amount, balance, BootCollection);
            Invoke(nameof(checkSound), 0.5f);
        }
        public void checkSound()
        {
            if (MasterAudioController.instance.CheckSoundToggle())
                MasterAudioController.instance.PlayAudio(AudioEnum.CHIPSOUND);
        }

        [ClientRpc]
        public void ShowServerAnimationResult(double amount, double balance, string PlayerID)
        {
            PlayerUI myUI = GetPlayerUI(PlayerID);
            if (myUI != null)
                myUI.GetAmountFromPot((float)amount);
        }

        bool OnlyBotExsist;


        public void EnableTimerAnimation(int oldStr, int newStr)
        {

            switch (GameStartCountDown)
            {
                case 5:
                    GamePlayUI.instance.StartTimerObjects[4].SetActive(true);
                    break;
                case 4:
                    GamePlayUI.instance.StartTimerObjects[3].SetActive(true);
                    break;
                case 3:
                    GamePlayUI.instance.StartTimerObjects[2].SetActive(true);
                    break;
                case 2:
                    GamePlayUI.instance.StartTimerObjects[1].SetActive(true);
                    break;
                case 1:
                    GamePlayUI.instance.StartTimerObjects[0].SetActive(true);
                    break;
            }
        }
        bool CallThisOnce = false;
        bool IsSucces = false;
        public List<string> playersList = new List<string>();
        bool FirstInitBetSuccess;
        bool GameTimerOnce = false;

        [Server]
        IEnumerator StartGameTimerServer()
        {

            DebugHelper.Log("StartGameTimerServer ===========> 1" + CallThisOnce);
            IsSucces = false;

            if (gameState.players.Count >= 3 && /*!CallThisOnce &&*/ !gameController.isBlockedAPI)
            {
                if ((!gameStart))
                {
                    GetMatchToken(gameState.players[0].playerData.playerID);

                }

                DebugHelper.Log("Waiting to Get Match Token ============ Before>");
                yield return new WaitUntil(() => !string.IsNullOrEmpty(gameState.currentMatchToken));
                DebugHelper.Log("Waiting to Get Match Token ============ After>");

                PerformInitBetForAllPLayerOnNextGame();

                yield return new WaitUntil(() => FirstInitBetSuccess);

            }

            //AddPlayersToGame(false);

            if (!gameController.isBlockedAPI)
            {

                bool check = gameState.players.FindAll(x => x.hasInitBet == true).Count == gameState.players.Count;
                gameState.BetCompleted = check;
                UpdateGameStateToServer();
                yield return new WaitUntil(() => check);
                roomInfo.isClosed = true;
                if (!GameTimerOnce)
                {
                    GameTimerOnce = true;
                    gameState.gameStartTime = NetworkTime.time;
                }

                int countDown = 10;
                isAutoSee = false;

                botPlay = null;
                while (gameState.gameStartTime + 6 > NetworkTime.time && gameState.players.Count > 1 && (!gameController.IsTournament || gameState.CurrentGameCount == 0))
                {
                    countDown = (int)(gameState.gameStartTime + 6 - NetworkTime.time);
                    GameStartCountDown = countDown;
                    if (countDown > 0)
                    {
                        GlobalMessage("GAME STARTING IN");
                        // EnableTimerAnimation(countDown);
                    }

                    yield return new WaitForSeconds(0.2f);
                }
            }
            else
            {

                if (!GameTimerOnce)
                {
                    roomInfo.isClosed = true;
                    GameTimerOnce = true;
                    gameState.gameStartTime = NetworkTime.time;
                }

                int countDown = 10;
                isAutoSee = false;

                botPlay = null;
                while (gameState.gameStartTime + 6 > NetworkTime.time && gameState.players.Count > 1 && (!gameController.IsTournament || gameState.CurrentGameCount == 0))
                {
                    countDown = (int)(gameState.gameStartTime + 6 - NetworkTime.time);
                    GameStartCountDown = countDown;
                    if (countDown > 0)
                    {
                        GlobalMessage("GAME STARTING IN");

                        // EnableTimerAnimation(countDown);
                    }

                    yield return new WaitForSeconds(0.2f);
                }
            }
            if (GetRealPlayersCountInGame() >= 1)
            {
                if (gameState.players.Count + gameState.waitingPlayers.Count == GetBotCountInGame())
                {
                    AutoGamePlayCount += 1;
                }
                else
                {
                    AutoGamePlayCount = 0;
                }
                int count = 0;
                foreach (PlayerState ps in gameState.players)
                {
                    if (gameController.isBlockedAPI)
                    {
                        gameState.totalPot += currentTableModel.BootAmount;

                    }
                    if (ps.isSpectator)
                    {
                        ps.currentState = 0;
                        count += 1;
                        continue;
                    }
                    ps.playerData.currentCards = GetCards();
                    ps.playerData.money = ps.playerData.money - currentTableModel.BootAmount;
                    ps.playerData.currentCombination = CardCombination.GetCombinationFromCard(ps.playerData.currentCards);
                    ps.cardStrength = (int)ps.playerData.currentCombination;
                    if (gameController.IsTournament && gameState.CurrentGameCount == 0)
                    {
                        if (gameController.CurrentTournamentModel.currentPlayingRound == 0)
                            ReduceBootAmountForTournament(ps.playerData.playerID);
                    }
                    ReduceAmount(currentTableModel.BootAmount, ps.playerData.playerID);
                    count += 1;
                    for (int i = 0; i < 3; i++)
                    {
                        ps.playerData.currentCards[i].isClose = true;
                    }
                    // gameState.totalPot = gameState.totalPot + currentTableModel.BootAmount;
                    ps.currentState = 1;
                }
                foreach (PlayerState ps in gameState.players)
                {
                    if (gameController.isBlockedAPI)
                    {

                        SendChall(ps.playerData.playerID, Convert.ToDouble(currentTableModel.BootAmount), ps.playerData.money, true, false);
                    }
                    else
                    {
                        // ClientUpdateBalance(ps.InitBetAmount, ps.playerData.playerID);
                        ShowServerAnimation(Convert.ToDouble(currentTableModel.BootAmount), ps.playerData.money, ps.playerData.playerID, false);
                    }
                }
                gameState.currentState = 6;
                UpdateGameStateToServer();
                gameState.NewDeck();
                if (gameController.CurrentGameMode.ToString() == "ZANDU")
                    gameState.InitZanduMode();
                /*else if (gameController.CurrentGameMode == GameMode.HUKAM)
                    gameState.InitHUKAMMode();*/
                gameState.players = gameState.players.OrderBy(x => x.ui).ToList();
                gameState.currentState = 2;
                GlobalMessage("Collecting Boot amount");
                yield return new WaitForSeconds(2);
                UpdateGameStateToServer();
                gameState.isSideShowRequestSend = false;

                if (IsBotInGame())
                {
                    botPlayer = new BotBehaviour();

                    GenerateRank();
                }
                yield return new WaitForSeconds(1.5f);
                StartGame();
                //for (int i = 0; i < gameState.players.Count; i++)
                //{
                //    if (i == 0)
                //    {
                //        gameState.players[i].playerData.currentCards[0].rankCard = 7;
                //        gameState.players[i].playerData.currentCards[0].suitCard = CardSuit.Diamonds;
                //        gameState.players[i].playerData.currentCards[1].rankCard = 8;
                //        gameState.players[i].playerData.currentCards[1].suitCard = CardSuit.Hearts;
                //        gameState.players[i].playerData.currentCards[2].rankCard = 10;
                //        gameState.players[i].playerData.currentCards[2].suitCard = CardSuit.Hearts;
                //        gameState.players[i].playerData.currentCombination = CardsCombination.HighCard;
                //    }
                //    if (i == 1)
                //    {
                //        gameState.players[i].playerData.currentCards[0].rankCard = 6;
                //        gameState.players[i].playerData.currentCards[0].suitCard = CardSuit.Hearts;
                //        gameState.players[i].playerData.currentCards[1].rankCard = 10;
                //        gameState.players[i].playerData.currentCards[1].suitCard = CardSuit.Diamonds;
                //        gameState.players[i].playerData.currentCards[2].rankCard = 14;
                //        gameState.players[i].playerData.currentCards[2].suitCard = CardSuit.Spade;
                //        gameState.players[i].playerData.currentCombination = CardsCombination.HighCard;
                //    }

                //    if (i == 2)
                //    {
                //        gameState.players[i].playerData.currentCards[0].rankCard = 3;
                //        gameState.players[i].playerData.currentCards[0].suitCard = CardSuit.Diamonds;
                //        gameState.players[i].playerData.currentCards[1].rankCard = 11;
                //        gameState.players[i].playerData.currentCards[1].suitCard = CardSuit.Clubs;
                //        gameState.players[i].playerData.currentCards[2].rankCard = 13;
                //        gameState.players[i].playerData.currentCards[2].suitCard = CardSuit.Hearts;
                //        gameState.players[i].playerData.currentCombination = CardsCombination.HighCard;
                //    }
                //}

                roomInfo.isClosed = false;
                yield return new WaitForSeconds(1f);
                UpdateGameStateToServer();
                yield return new WaitForSeconds(.5f);
                checkForStartgameMaster();
                APIController.instance.AddPlayers(gameState.currentMatchToken, playersList, gameController.operatorName, gameController.environment);

            }
            else
            {
                if (GetRealPlayersCountInGame() == 0)
                {
                    for (int i = 0; i < gameState.players.Count; i++)
                    {
                        APIController.instance.CancelBetMultiplayerAPI((x, y, z) => { }, gameState.players[i].playerData.playerID, gameController.gameName, gameController.operatorName, gameController.gameId, gameState.currentMatchToken, gameState.players[i].playerData.session_token, gameState.players[i].playerData.currency_type
                               , gameController.environment);
                    }

                }
                gameState.currentState = 0;
                UpdateGameStateToServer();
            }
        }



        public void PerformInitBetForAllPLayerOnNextGame()
        {

            Debug.Log($"=====>PerformInitBetForAllPLayerOnNextGame");

            CallThisOnce = true;
            foreach (var ps in gameState.players)
            {
                if (!ps.hasInitBet && !ps.CalledInitBet)
                {
                    int IndexToSet = /*gameState.players.IndexOf(ps);*/UnityEngine.Random.Range(0, 100000);


                    DebugHelper.Log(gameController.gameName + " **********  " + gameState.currentMatchToken);
                    if (gameController.isBlockedAPI)
                    {

                    }
                    else
                    {
                        TransactionMetaData _metaData = new TransactionMetaData();
                        _metaData.Amount = Convert.ToDouble(currentTableModel.BootAmount);
                        _metaData.Info = "Initialize Bet";

                        betIndex = APIController.instance.InitBetMultiplayerAPI(IndexToSet, Convert.ToDouble(currentTableModel.BootAmount), _metaData, false, (success, message, Code) =>
                        {

                            if (success)
                            {

                                int ValBot = gameState.players.FindAll(x => x.playerData.isBot).Count;
                                if (ValBot != 0)
                                {
                                    if (isBotWin)
                                    {
                                        DebugHelper.Log(" ********** " + "Success Called WIN");
                                        RpcSendMessage($"RNG Calculation:\n==============\n{"Bot will win the round"}\n==============\n", ps.playerData.playerID);
                                    }
                                    else
                                    {
                                        DebugHelper.Log(" ********** " + "Success Called Loss");
                                        RpcSendMessage($"RNG Calculation:\n==============\n{"Bot will play fair"}\n==============\n", ps.playerData.playerID);
                                    }
                                }
                                gameState.totalPot += currentTableModel.BootAmount;
                                ps.hasInitBet = true;
                                IsSucces = true;

                                playersList.Add(ps.playerData.playerID);
                                /*  JObject jsonObject = JObject.Parse(message);
                                  ps.InitBetAmount = jsonObject["balance"].ToString();*/
                               // ClientUpdateBalance(ParseBalanceMessage(message), ps.playerData.playerID);
                               // ps.InitBetAmount = ParseBalanceMessage(message).ToString();
                                UpdateGameStateToServer();


                            }
                            else
                            {
                                DebugHelper.Log("KICK ALL PLAYERS >>>>>>>>>>>>>  ***************");

                                IsSucces = false;
                                ps.hasInitBet = false;
                                ps.currentState = 7;
                                 HandleErrorCode(Code, ps.playerData.playerID, message);
                                UpdateGameStateToServer();
                                return;
                            }

                        }, ps.playerData.playerID, ps.playerData.playerName, ps.playerData.isBot, (Betid) =>
                        {
                            DebugHelper.Log("Init Bet =========> " + ps.playerData.playerID + " ======  " + ps.playerData.playerName + " ========== " + Betid);
                            ps.BetId = Betid;
                            UpdateGameStateToServer();

                        }, gameController.gameName, gameController.operatorName, gameController.gameId, gameState.currentMatchToken, gameController.domainURL, ps.playerData.session_token, ps.playerData.currency_type, ps.playerData.platform, ps.playerData.token, gameController.environment, ps.playerData.money.ToString());


                        DebugHelper.Log("CHECK >>>>>>>>>>>>>  ***************" + gameState.totalPot + " ****************** " + ps.playerData.playerName);
                        ps.BetIndex = betIndex;
                        DebugHelper.Log("***TESTCHECK  PlayerInitBet ==================> " + ps.BetIndex + " ================ " + ps.playerData.playerID + " =============== " + ps.playerData.playerName);

                    }
                    ps.CurrentGameSpend += currentTableModel.BootAmount;
                    ps.CalledInitBet = true;
                    UpdateGameStateToServer();
                }
            }
            FirstInitBetSuccess = true;
        }

        [ClientRpc]
        public void RpcSendMessage(string val, string playerID)
        {

            if (myPlayerID == playerID)
            {
                Debug.Log(val);
            }

        }


        [Command]
        public void GlobalMessageServer(string _globalInfo)
        {

            GlobalMessageClient(_globalInfo);
        }



        public void AddServerEvent(SWEvent newEvent)
        {
            if (gameState.players.Count + gameState.waitingPlayers.Count == 0)
                return;
            requestList.Add(newEvent);
            CheckActionRequired();
        }
        public void RemoveServerEvent(string eventID)
        {
            requestList.RemoveAll(x => x.GetEventID() == eventID);

        }


        public void StandUpUserServer(string playerid)
        {
            DebugHelper.LogError("standup called");
            PlayerState ps = GetPlayerState(playerid);
            if (ps.isSpectator) return;

            if (gameState.players.Exists(x => x.playerData.playerID == playerid))
            {
                if (gameState.currentState == 2)
                {
                    ps.hasPacked = true;

                    if (ps.isMyTurn)
                    {
                        if (GetActivePlayedPlayerCount() <= 1)
                        {
                            if (playerTurnTimerRoutine != null) StopCoroutine(playerTurnTimerRoutine);
                            DebugHelper.LogError("game state update 4");
                            gameState.currentState = 4;

                        }
                        else
                        {
                            int index = NextPlayerIndex(GetPlayerIndex(playerid));
                            gameState.players[index].SetMyTurn(true);
                            if (playerTurnTimerRoutine != null) StopCoroutine(playerTurnTimerRoutine);
                            playerTurnTimerRoutine = StartCoroutine(StartPlayerTimer(gameState.players[index].playerData.playerID));

                        }
                    }
                }
                ps.isSpectator = true;
                ps.currentState = 0;
            }
            else
            {
                ps.currentState = 0;
                ps.isSpectator = true;
            }
            UpdateGameStateToServer();
        }

        public void OnPlayerSitServer(string playerid)
        {
            PlayerState ps = GetPlayerState(playerid);

            ps.ResetState();
            ps.isSpectator = false;
            ps.currentState = 0;
            UpdateGameStateToServer();
        }


        public void OnDeclineSideShowServer(int currentPlayer, int myPlayerId)
        {
            StopCoroutine("ResetSideShowServer");
            gameState.players[currentPlayer].isMyTurn = false;
            gameState.isSideShowRequestSend = false;
            gameState.sideShowRequestReceiver = -1;
            gameState.sideShowRequestSender = -1;
            gameState.sideShowRequestTime = -1;
            gameState.currentState = 2;
            DebugHelper.LogError("side show 2 Change game state 4");
            string data = (CommonFunctions.Instance.GetTruncatedPlayerName(gameState.players[myPlayerId].playerData.playerName) + " has rejected SideShow request from " + gameState.players[currentPlayer].playerData.playerName);
            GlobalMessage(data);
            NextPlayerTurn(currentPlayer);
        }

        void CheckSideShowRequestServer()
        {
            if (IsSideShowChecked == false)
            {
                IsSideShowChecked = true;

                if (gameState.players[gameState.sideShowRequestReceiver].playerData.isBot)
                {
                    if (UnityEngine.Random.Range(1, 2) == 1)
                    {
                        OnAcceptSideShowServer(gameState.sideShowRequestSender, gameState.sideShowRequestReceiver);
                    }
                    else
                    {
                        OnDeclineSideShowServer(gameState.sideShowRequestSender, gameState.sideShowRequestReceiver);
                    }
                    return;
                }

                StartCoroutine("ResetSideShowServer");
            }
        }

        IEnumerator ResetSideShowServer()
        {
            double currCountdownValue = (gameState.sideShowRequestTime + 10) - NetworkTime.time;
            yield return new WaitForSeconds((float)currCountdownValue);
            if (gameState.currentState == 3)
            {
                int sideshowsender = gameState.sideShowRequestSender;
                gameState.sideShowRequestReceiver = -1;
                gameState.sideShowRequestSender = -1;
                gameState.sideShowRequestTime = -1;
                gameState.isSideShowRequestSend = false;
                gameState.currentState = 2;
                NextPlayerTurn(true);
                UpdateGameStateToServer();
            }

        }

        public void OnAcceptSideShowServer(int firstPlayerId, int botPlayerId)
        {
            StopCoroutine("ResetSideShowServer");
            isSideshow = true;
            DebugHelper.Log("sideshow::: change state:::" + firstPlayerId);
            string data = (CommonFunctions.Instance.GetTruncatedPlayerName(gameState.players[botPlayerId].playerData.playerName) + " has accept SideShow request from " + gameState.players[firstPlayerId].playerData.playerName + ".");
            CloseAlertPopUp();
            GlobalMessage(data);
            gameState.players[firstPlayerId].isCardShow = true;
            gameState.players[botPlayerId].isCardShow = true;
            gameState.players[firstPlayerId].currentState = 3;
            gameState.players[botPlayerId].currentState = 3;
            UpdateGameStateToServer();
            DebugHelper.Log("SideShow error check 1");
            StartCoroutine(OnAcceptSideShowDelayCall(firstPlayerId, botPlayerId));
        }




        [ClientRpc]
        public void CloseAlertPopUp()
        {
            GamePlayUI.instance.sideShowRequest.DisableSideshow();
        }

        #endregion

        #region BOT_LOGIC

        public bool IsBotInGame()
        {
            foreach (PlayerState ps in gameState.players)
            {
                if (ps.playerData.isBot)
                {
                    return true;
                }
            }
            return false;
        }

        public int GetRealPlayersCountInGame()
        {
            int count = 0;
            Debug.Log(gameState.players.Count + "GetRealPlayersCountInGame==>1");
            foreach (PlayerState ps in gameState.players)
            {
                if (!ps.playerData.isBot)
                {
                    count += 1;
                    Debug.Log("players count : " + count + "GetRealPlayersCountInGame====2");
                }
            }
            Debug.Log(gameState.waitingPlayers.Count + "GetRealPlayersCountInGame==>3");
            foreach (PlayerState ps in gameState.waitingPlayers)
            {
                if (!ps.playerData.isBot)
                {
                    count += 1;
                    Debug.Log("waitingPlayers count : " + count + "GetRealPlayersCountInGame====4");
                }
            }

            Debug.Log("Retrun count" + count + "GetRealPlayersCountInGame====5");
            return count;
        }

        public int GetBotCountInGame()
        {
            int count = 0;
            foreach (PlayerState ps in gameState.players)
            {
                if (ps.playerData.isBot)
                {
                    count += 1;
                }
            }
            foreach (PlayerState ps in gameState.waitingPlayers)
            {
                if (ps.playerData.isBot)
                {
                    count += 1;
                }
            }
            return count;
        }
        public PlayerState GetBotInGame()
        {
            foreach (PlayerState ps in gameState.players)
            {
                if (ps.playerData.isBot)
                {
                    return ps;
                }
            }
            foreach (PlayerState ps in gameState.waitingPlayers)
            {
                if (ps.playerData.isBot)
                {
                    return ps;
                }
            }
            return new PlayerState();
        }


        public GameObject GetBotObject(string id)
        {
            foreach (GameObject go in botPlayers)
            {
                if (go.GetComponent<PlayerManager>().playerID == id)
                    return go;
            }
            return null;
        }

        public void BotSeeCards()
        {
            /*if (gameController.CurrentGameMode == GameMode.POTBLIND)
                return;*/
            int currentPlayerIndex = GetCurrentPlayingPlayerIndex();
            gameState.players[currentPlayerIndex].SetCardsSeen();
            GlobalMessage((CommonFunctions.Instance.GetTruncatedPlayerName(gameState.players[currentPlayerIndex].playerData.playerName) + " has Seen cards."));
            UpdateGameStateToServer();
        }

        IEnumerator CheckBotCleanUp()
        {
            yield return new WaitForSeconds(2);
            if (gameState.hasbot)
            {
                int playersCount = gameState.players.Count + gameState.waitingPlayers.Count;
                gameState.players.RemoveAll(x => x.playerData.isBot);
                gameState.waitingPlayers.RemoveAll(x => x.playerData.isBot);
                if (playersCount != gameState.players.Count + gameState.waitingPlayers.Count)
                {
                    DebugHelper.LogError("Bot Cleaned Up..........");
                    gameState.hasbot = false;
                    UpdateGameStateToServer();
                }
            }
        }


        IEnumerator BotActivityAfterPack(string playerid)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(3, 7));
            if (gameState.currentState == 2)
            {
                if (UnityEngine.Random.Range(1, 5) == 2)
                {
                    RemovePlayerByServer(GetPlayerState(playerid));
                }
                else if (UnityEngine.Random.Range(1, 5) == 2)
                {
                    StandUpUserServer(playerid);
                }
            }
        }

        public void BotChaal()
        {
            Chaal(GetCurrentPlayingPlayerID(), isBotIncressBet);
        }

        public void AddBotFromAPI()
        {
            DebugHelper.LogError("BOT API CALLED >>>>>>> " + GetBotCountInGame() + "   <<<<<<<<<<<<<<  ");
            if (GetBotCountInGame() < botLimit)
            {
                Debug.Log("Chek The Bot added Concept======>4");
                StopCoroutine(nameof(AddBotIfRequired));
                StartCoroutine(nameof(AddBotIfRequired));
            }
        }

        string[] BotIDs = { "0176b48c-1e68-4347-89c2-bc9ffcf4b8ef", "040db418-c1ef-4533-be32-680487b76e3a", "0468f798-0224-479f-bb9d-3c747299ad9c", "06480966-fb90-4213-a675-ba5b285a409e", "0b31dc13-6952-45f1-8e64-f58f5cfd61ca", "09bc6f6d-d450-412e-8762-bba470532974" };

        int val = 0;
        public IEnumerator AddBotIfRequired()
        {

            val++;
            Debug.Log(val + "Check AddBot if Requied");
            Debug.Log("BOT >>>>>>>>>>>> ADD BOT IF REQUIRED CALLED *************" + GetRealPlayersCountInGame());



            Debug.LogError("bot check 0");
            if (isBotNotInGame)
                yield break;
            Debug.LogError("bot check 1");
            Debug.Log($"AddBot the check Data Bot Count {gameState.players.Count}======>{gameState.waitingPlayers.Count}");
            if (gameState.players.Count + gameState.waitingPlayers.Count == 0)
                yield break;
            if ((gameController.CurrentGameType == GameType.PRIVATE) || gameController.IsTournament)
            {
                DebugHelper.LogError("Privete/Tournament. No bots allowed");
                yield break;
            }
            Debug.LogError("bot check 2");


            if (((GetBotCountInGame() >= botLimit || (gameState.players.Count + gameState.waitingPlayers.Count) >= 3)))
                yield break;
            if (GetBotCountInGame() == (gameState.players.Count + gameState.waitingPlayers.Count))
                yield break;

            Debug.LogError("bot check 3");

            Debug.LogError("bot check 4");

            //int sec = UnityEngine.Random.Range(2, 7);
            Debug.Log("BOT >>>>>>>>>>>> ADD BOT IF REQUIRED CALLED ************* %%%%%%%%%%%%%%%%" + GetRealPlayersCountInGame());
            yield return new WaitForSeconds(2);
            if (isAddingBot) yield break;
            Debug.LogError("bot check 5");
            if (((GetBotCountInGame() >= botLimit || (gameState.players.Count + gameState.waitingPlayers.Count) >= 3)) || gameController.IsTournament)
                yield break;
            isAddingBot = true;
            gameState.hasbot = true;
            botCount++;
            Debug.Log("bot result request send" + botLimit + "--" + GetBotCountInGame());
            Debug.LogError("bot check 6");

            if (gameController.isBlockedAPI)
            {
                Debug.Log("NORMAL BOT API");

                PlayerData botData = new();
                botid += 1;
                botData.playerID = FetchBotID();
                botData.playerName = GetRandomBotName();
                DebugHelper.LogError(botData.playerName + "************playerName**************");
                botData.silver = UnityEngine.Random.Range(10000, 50000);
                botData.gold = UnityEngine.Random.Range(10000, 50000);
                try
                {
                    if (gameController.CurrentAmountType == CashType.CASH)
                        botData.money = botData.gold;
                    else
                        botData.money = botData.silver;
                }
                catch (Exception e)
                {
                    botData.money = 0;
                }
                botData.isBot = true;
                botData.avatarIndex = 1;
                botData.profilePicURL = UnityEngine.Random.Range(0, 9).ToString();
                PlayerState ps = new PlayerState();
                ps.ResetState();
                ps.playerData = botData;
                if (GetRealPlayersCountInGame() < 3)
                {
                    Debug.Log("Add Bot API >>>>>>>>>>>> BLOCKED API : " + GetRealPlayersCountInGame());
                    MirrorManager.instance.AddBot(ps, this);
                }
                isAddingBot = false;
                UpdateGameStateToServer();
                if (GetBotCountInGame() < botLimit)
                {
                    Debug.Log("Chek The Bot added Concept======>5");
                    StopCoroutine(nameof(AddBotIfRequired));
                    StartCoroutine(nameof(AddBotIfRequired));
                }

            }
            else
            {
                DebugHelper.Log(" BOT API >>>>>>>>>>>> BEFORE : " + GetBotCountInGame());
                PlayerData botDataAPI = new PlayerData();
                APIController.instance.GetABotAPI(FetchExsistingBotID(), (botData) =>
                {

                    botDataAPI.playerID = botData.userId;
                    botDataAPI.playerName = botData.name;
                    botDataAPI.silver = botData.balance;
                    botDataAPI.gold = botData.balance;
                    try
                    {
                        if (gameController.CurrentAmountType == CashType.CASH)
                            botDataAPI.money = botData.balance;
                        else
                            botDataAPI.money = botData.balance;
                    }
                    catch (Exception e)
                    {
                        botDataAPI.money = 0;
                    }
                    botDataAPI.isBot = true;
                    botDataAPI.avatarIndex = 1;
                    botDataAPI.profilePicURL = UnityEngine.Random.Range(0, 9).ToString();

                    PlayerState ps = new PlayerState();
                    ps.ResetState();

                    ps.playerData = botDataAPI;
                    ps.playerData.session_token = "BotTokenFromTeenpattiSetManually";
                    ps.playerData.currency_type = "BotUniversal";
                    ps.playerData.platform = "BOTplatform";

                    if (GetRealPlayersCountInGame() < 3)
                    {
                        DebugHelper.Log("Add Bot API >>>>>>>>>>>> NOT REAL GAME BLOCKED API : " + GetRealPlayersCountInGame());
                        MirrorManager.instance.AddBot(ps, this);
                        OnlyBotExsist = !gameState.players.Exists(x => !x.playerData.isBot);
                        DebugHelper.Log("Check only Bot Exsist" + OnlyBotExsist);
                        if ((!gameStart))
                        {
                            GetMatchToken(gameState.players[0].playerData.playerID);

                        }
                        StartCoroutine(InitializeBetBot(ps));
                    }


                    isAddingBot = false;
                    DebugHelper.Log(" BOT API >>>>>>>>>>>> AFTER : " + GetBotCountInGame());
                    UpdateGameStateToServer();
                    if (GetBotCountInGame() < botLimit)
                    {
                        Debug.Log("Chek The Bot added Concept======>6");
                        StopCoroutine(nameof(AddBotIfRequired));
                        StartCoroutine(nameof(AddBotIfRequired));
                    }
                }, StaticStrings.GetLambdaUrl(gameController.environment), gameController.operatorName);
            }
        }


        public IEnumerator InitializeBetBot(PlayerState ps)
        {
            Debug.Log($"=====>InitializeBetBot");
            yield return new WaitUntil(() => !string.IsNullOrEmpty(gameState.currentMatchToken));
            int IndexToSet = /*gameState.players.IndexOf(ps);*/ UnityEngine.Random.Range(0, 100000);
            if (ps.playerData.isBot && !ps.hasInitBet && !ps.CalledInitBet)
            {
                if (gameController.isBlockedAPI)
                {

                }
                else
                {
                    TransactionMetaData _metaData = new TransactionMetaData();
                    _metaData.Amount = Convert.ToDouble(currentTableModel.BootAmount);
                    _metaData.Info = "Initialize Bet Bot";

                    if (!OnlyBotExsist && gameState.currentState < 2)
                    {
                        betIndex = APIController.instance.InitBetMultiplayerAPI(IndexToSet, Convert.ToDouble(currentTableModel.BootAmount), _metaData, false, (success, message, code) =>
                        {

                            if (success)
                            {
                                DebugHelper.Log("CHECK >>>>>>>>>>>>>  ***************" + gameState.totalPot + " ****************** " + ps.playerData.playerName);
                                gameState.totalPot += currentTableModel.BootAmount;
                                ps.hasInitBet = true;
                                IsSucces = true;

                                playersList.Add(ps.playerData.playerID);

                               // ClientUpdateBalance(ParseBalanceMessage(message), ps.playerData.playerID);
                               // ps.InitBetAmount = ParseBalanceMessage(message).ToString();
                                UpdateGameStateToServer();
                            }
                            else
                            {

                            }

                        }, ps.playerData.playerID, ps.playerData.playerName, true, (Betid) =>
                        {
                            DebugHelper.Log("Init Bet =========> " + ps.playerData.playerID + " ======  " + ps.playerData.playerName + " ========== " + Betid);
                            ps.BetId = Betid;
                            UpdateGameStateToServer();

                        }, gameController.gameName, gameController.operatorName, gameController.gameId, gameState.currentMatchToken, gameController.domainURL, ps.playerData.session_token, ps.playerData.currency_type, ps.playerData.platform, ps.playerData.token, gameController.environment, ps.playerData.money.ToString());

                        DebugHelper.Log("CHECK >>>>>>>>>>>>>  ***************" + gameState.totalPot + " ****************** " + ps.playerData.playerName);
                        ps.BetIndex = betIndex;
                        DebugHelper.Log("***TESTCHECK  BotInitBet ==================> " + ps.BetIndex + " ================ " + ps.playerData.playerID + " =============== " + ps.playerData.playerName);
                        ps.CalledInitBet = true;
                        UpdateGameStateToServer();
                    }
                    ps.CurrentGameSpend += currentTableModel.BootAmount;
                }
            }
        }


        private List<string> FetchExsistingBotID()
        {
            List<string> BotIDs = new();


            foreach (var Player in gameState.waitingPlayers)
            {
                if (Player.playerData.isBot)
                {


                    BotIDs.Add(Player.playerData.playerID);


                }
            }


            foreach (var Player in gameState.players)
            {
                if (Player.playerData.isBot)
                {


                    BotIDs.Add(Player.playerData.playerID);


                }
            }


            return BotIDs;
        }

        private string FetchBotID()
        {
            foreach (string id in BotIDs)
            {
                if (!gameState.players.Exists(x => x.playerData.playerID.Equals(id)) && !gameState.players.Exists(x => x.playerData.playerID.Equals(id)))
                {
                    return id;
                }
            }

            return BotIDs[0];
        }

        public void CheckForRemoveAllBots()
        {



            if (gameState.players.Exists(x => x.playerData.isBot))
            {
                for (int i = 0; i < gameState.players.Count; i++)
                {
                    if (gameState.players[i].playerData.money <= 1000)
                    {
                        RemovePlayerByServer(GetBotInGame());
                    }
                }
            }


           /* if ((gameState.players.Count + gameState.waitingPlayers.Count == GetBotCountInGame() && (AutoGamePlayCount > 2)) || ((gameState.players.Count + gameState.waitingPlayers.Count) - GetBotCountInGame()) >= 3)
            {
                while (GetBotCountInGame() != 0)
                {
                    RemovePlayerByServer(GetBotInGame());
                }
            }*/


        }



        void CheckBotTurn()
        {
            if (gameState.isDealCard)
                if (botPlay == null && gameState.players[GetCurrentPlayingPlayerIndex()].playerData.isBot)
                {

                    StopCoroutine(nameof(BotTurn));
                    botPlay = StartCoroutine(nameof(BotTurn));
                }

        }
        public void CheckBotStatus(PlayerManager botManager)
        {
            foreach (PlayerState playerState in gameState.players)
            {
                if (playerState.playerData.playerID == botManager.GetPlayerState().playerData.playerID)
                {
                    DebugHelper.LogError("Already in game. Rejoining");
                    botManager.SetPlayerState(playerState);
                    return;
                }
            }
            botManager.GetPlayerState().ResetState();
            botManager.GetPlayerState().disconnectTime = -1;
            botManager.GetPlayerState().ui = getUiIndex();

            gameState.waitingPlayers.Add(botManager.GetPlayerState());
            UpdateGameStateToServer();
        }



        public List<string> GetBotList()
        {
            List<string> bot = new List<string>();
            foreach (PlayerState player in gameState.players)
            {
                if (player.playerData.isBot)
                {
                    bot.Add(player.playerData.playerID);
                }
            }

            foreach (PlayerState player in gameState.waitingPlayers)
            {
                if (player.playerData.isBot)
                {
                    bot.Add(player.playerData.playerID);
                }
            }

            return bot;
        }



        private string GetRandomBotName()
        {
            return "User_" + UnityEngine.Random.Range(100, 999);
        }


        public void WinningGameBotServer(double amount, PlayerState ps)
        {

            Debug.Log("WinningGameBotServer Called");
            double val = ((gameController.CurrentAmountType == (CashType.CASH) && !gameController.IsTournament) ? ((amount)) : amount);
            ps.playerData.money = ps.playerData.money + val;
            //UIGameController.SetMatchLogs(gameState.currentMatchToken, "Transaction", ps.playerData.playerID, "" + gameState.totalPot, ps.playerData.isBot, ps.playerData.playerName + " Won the game");
            //UIGameController.SetMatchLogs(gameState.currentMatchToken, "Match Won", ps.playerData.playerID, "0", ps.playerData.isBot, (ps.playerData.playerName) + " winning this game");

            if (gameController.isBlockedAPI)
            {
                if (GetActivePlayerManager() != null)
                {

                    GetActivePlayerManager().SetBotWinnerAPI(ps.BetIndex, val, ps.CurrentGameSpend, "WinningGameBotPlayer", ps.playerData.playerID, gameState.totalPot);
                }

            }
            else
            {
                TransactionMetaData val1 = new();
                val1.Amount = 0;
                val1.Info = "WinningGameBotPlayer";

                if (!OnlyBotExsist)
                {
                    APIController.instance.WinningsBetMultiplayerAPI(ps.BetIndex, ps.BetId, val, ps.CurrentGameSpend, gameState.totalPot, val1, null, ps.playerData.playerID, true, true, gameController.gameName, gameController.operatorName, gameController.gameId, gameController.Commission, gameState.currentMatchToken, gameController.domainURL, ps.playerData.session_token, ps.playerData.currency_type, ps.playerData.platform, ps.playerData.token, gameController.environment, ps.playerData.money.ToString());
                }

            }

        }

        IEnumerator BotTurn()
        {
            int cppi = GetCurrentPlayingPlayerIndex();
            if (cppi == -1)
            {
                botPlay = null;
                yield break;
            }
            DebugHelper.Log("bot turn start " + gameState.players[cppi].playerData.playerName);
            if (gameState.players[cppi].isMyTurn && gameState.players[cppi].playerData.isBot)
            {

                int control = botPlayer.SetBotActionMirror(this);
                if (control == 1)
                {
                    yield return new WaitForSeconds(UnityEngine.Random.Range(2, 6));
                    if (gameState.players[GetCurrentPlayingPlayerIndex()].playerData.isBot)
                        VerifyAndPackBot();
                }
                else
                {
                    yield return new WaitForSeconds(UnityEngine.Random.Range(2, 4));
                    try
                    {
                        if (gameState.players[GetCurrentPlayingPlayerIndex()].playerData.isBot)
                            VerifyAndChaal(true);
                    }
                    catch
                    {

                    }

                }
            }
            if (GetActivePlayedPlayerCount() == 1)
            {
                if (gameState.currentState == 2)
                {
                    OpenCardForShow();
                    gameState.forceSee = false;
                    DebugHelper.LogError("game state update 4");
                    gameState.currentState = 4;

                }
            }
        }


        public void CheckForReJoin(PlayerManager pm, string _playerID, string roomName)
        {
            Debug.Log("Check for Rejoin called ******** " + _playerID);
            if (pm != null)
            {
                pm.ServerDisconnect();
                RemovePlayerManager(pm);
            }
            Debug.Log("Check for Rejoin called ******** Success " + _playerID);
            RemoveFromGame(_playerID, true);
        }

        public void GetMatchToken(string ID)
        {
            DebugHelper.Log("Get Match TokenCalled ======================> " + gameState.currentMatchToken);
            gameStart = true;
            if (string.IsNullOrEmpty(gameState.currentMatchToken))
            {
                APIController.instance.CreateMatch(roomInfo.lobbyName, (Responce, code, message) =>
                {
                    gameState.currentMatchToken = Responce.MatchToken;
                    if (!gameController.isBlockedAPI)
                    {
                        isBotWin = Responce.WinChance <= 0 ? true : false;
                        DebugHelper.Log("BOT WILL WIN THIS ROUND ------ >" + isBotWin + " *************** " + Responce.WinChance);
                    }
                    else
                    {
                        int val = UnityEngine.Random.Range(0, 10);
                        isBotWin = val <= 7;
                        DebugHelper.Log("BOT WILL WIN THIS ROUND LOOTRIX  ------ >" + isBotWin + " *************** " + val);
                    }
                    HandleErrorCode(code, ID, message);
                    UpdateGameStateToServer();

                }, gameController.gameName, gameController.operatorName, ID, gameController.isBlockedAPI, gameController.serverInfo, gameController.environment, GetPlayerState(ID).playerData.money.ToString());
            }
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
                    if (gameState.currentState <= 1 && !fromStartAuth)
                    {
                        gameState.currentState = 0;
                        gameState.Code = Code;
                        gameState.MessageToDisplay = Message;
                    }
                    else if (gameState.currentState >= 2 || fromStartAuth)
                    {
                        gameState.currentState = 7;
                        gameState.MessageToDisplay = Message;
                    }
                    break;
                case 402:
                    InsufficientPopup(playerID);
                    PackPlayerServer(playerID);
                    break;
                case 409:
                    break;
                case 504:
                    gameState.currentState = 8;
                    break;
            }

        }
        [ClientRpc]
        public void InsufficientPopup(string playerID)
        {
            if (playerID == APIController.instance.authentication.Id)
            {
                UIController.Instance.Insufficient.gameObject.SetActive(true);
            }
        }





        #endregion





    }





}




[Serializable]
public class ShowErrorMessage
{
    public string PlayerID;
    public string Message;
    public int Code;
}

