using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TP
{
    [System.Serializable]
    public class GameState
    {

        // note before add new variable  ::: if any one add new variable need to add initilize value in ClearGameState function also.
        public List<PlayerState> players = new List<PlayerState>();
        public List<PlayerState> waitingPlayers = new List<PlayerState>();
        public List<string> completedRequests = new List<string>();
        public List<string> removedPlayers = new List<string>();
        /// <summary> 0 - Not Enough Players, 1 - Countdown, 2 - Game InProgress, 3 - Result </summary>
        public bool hasbot;
        public bool allPacked;
        public int currentState = 0;
        public double gameStartTime = 0;
        public int zanthuCount = 0;
        public string GameCreatePlayerID;
        public bool forceSee = false;
        public double totalPot = 0;
        public float currentStake = 0;
        public CardData[] zanducards = new CardData[3];
        public int gameStartPlayerIndex;
        public List<CardData> currentDeck = new List<CardData>();
        private const int deckCount = 52;
        public int sideShowRequestSender = -1;
        public int sideShowRequestReceiver = -1;
        public int Senderval = -1;
        public int Receiverval = -1;
        public double sideShowRequestTime = -1;
        public bool isSideShowRequestSend = false;
        public bool SideShowWinnerEffectAfterAccept;
        public bool ShowAnimationEffect;
        public string SenderShowval;
        public string ReceiverShowval;
        public bool isDealCard = false;
        public int dealerindex;
        public string botPlayerId = "";
        public string currentMatchToken = "";
        public int CurrentGameCount = 0;
        public TournamentResultModel currentTournamentResultModel;
        public TeenPattiGameData gameController = new TeenPattiGameData();
        public int RoundCount;

        public void ClearGameState()
        {
            players.Clear();
            waitingPlayers.Clear();
            completedRequests.Clear();
            removedPlayers.Clear();
            hasbot = false;
            currentState = 0;
            gameStartTime = 0;
            zanthuCount = 0;
            forceSee = false;
            totalPot = 0;
            currentStake = 0;
            gameStartPlayerIndex = 0;
            currentDeck.Clear();
            sideShowRequestSender = -1;
            sideShowRequestReceiver = -1;
            sideShowRequestTime = -1;
            Senderval = -1;
            Receiverval = -1;
            isSideShowRequestSend = false;
            isDealCard = false;
            ShowAnimationEffect = false;
            SenderShowval ="";
            ReceiverShowval ="";
            dealerindex = 0;
            botPlayerId = "";
            currentMatchToken = "";
            CurrentGameCount = 0;

        }
    public GameState()
        {
            currentTournamentResultModel = new TournamentResultModel();
        }
        public void UpdateCurrentTournamentPlayers()
        {
            foreach(PlayerState ps in players)
            {
                double chip = ps.playerData.money;
                string playerid = ps.playerData.playerID;
                if (currentTournamentResultModel.players.Exists(x => x.playerId == playerid))
                {
                    TournamentPlayers players = currentTournamentResultModel.players.Find(x => x.playerId == playerid);
                    players.lastPlayedRound = CurrentGameCount;
                    players.chipsInHand = chip;
                }
                else
                {
                    TournamentPlayers players = new TournamentPlayers();
                    players.playerId = playerid;
                    players.playerName = ps.playerData.playerName;
                    players.lastPlayedRound = CurrentGameCount;
                    players.chipsInHand = chip;
                    currentTournamentResultModel.players.Add(players);
                }
            }
            var  orderedList = currentTournamentResultModel.players.OrderBy(x => x.rank);
            if (!gameController.CurrentTournamentModel.IsAllIn)
                orderedList = currentTournamentResultModel.players.OrderByDescending(x => x.lastPlayedRound).ThenByDescending(x => x.chipsInHand);
            
            //get sorted list of IDs
            var SortedIds = orderedList.Select(x => x.playerId).ToList();
            var updatedList = orderedList.ToList();
            if(!gameController.CurrentTournamentModel.IsAllIn)
            {
                updatedList.ForEach(x => x.rank = SortedIds.IndexOf(x.playerId));
            }
            currentTournamentResultModel.players = updatedList;

            currentTournamentResultModel.loosersIdList.Clear();

            int countOfWinners = 1;
            bool hasSecondPlayerWinner = false;
            if (gameController.CurrentTournamentModel is PokerTournamentModel)
            {
                var model = ((PokerTournamentModel)gameController.CurrentTournamentModel);
                hasSecondPlayerWinner = model.RoundData[model.currentPlayingRound].SecondPrize > 0;
                countOfWinners = hasSecondPlayerWinner ? 2 : 1;
            }
            else
            {
                List<TeenPattiTournamentModel> tournament_data = GameController.Instance.GameModeModels.TournamentData.TeenPattiData.ToList();
                var model = tournament_data.Find(x=>x.Name== gameController.CurrentTournamentModel.Name);
                hasSecondPlayerWinner = model.RoundData[model.currentPlayingRound].SecondPrize > 0;
                countOfWinners = hasSecondPlayerWinner ? 2 : 1;
            }

            currentTournamentResultModel.firstWinnerId = currentTournamentResultModel.players[0].playerId;
            if(hasSecondPlayerWinner)
                currentTournamentResultModel.secondWinnerId = currentTournamentResultModel.players[1].playerId;
            for (int i = countOfWinners; i< currentTournamentResultModel.players.Count;i++)
            {
                currentTournamentResultModel.loosersIdList.Add(currentTournamentResultModel.players[i].playerId);
            }
        }
        public void setRankForAllInTournament(List<PlayerState> winerps)
        {
            int rank = 0;
            foreach (PlayerState ps in winerps)
            {
                foreach(TournamentPlayers tp in currentTournamentResultModel.players)
                {
                    if(tp.playerId == ps.playerData.playerID)
                    {
                        tp.rank = rank;
                        break;
                    }
                }
                rank += 1;
            }
        }
        bool RoyalMode = false;
        public void NewDeck()
        {
            currentDeck = new List<CardData>();
            int suit = 1;
            int rank = 1;

            for (int i = 0; i < deckCount; i++)
            {
                if (rank % 14 == 0)
                {
                    suit++;
                    rank = 1;
                }
                currentDeck.Add(new CardData(suit, rank, true));
                rank++;
            }

           /* if (gameController.CurrentGameMode == GameMode.ROYAL)
            {
               
                for (int j=1;j<=4;j++)
                {
                   
                    currentDeck.Add(new CardData(j, 1, true));
                    for (int i = 10; i < 14; i++)
                    {
                       *//* RoyalMode = true;*//*
                        Debug.Log("Print the log");
                        currentDeck.Add(new CardData(j, i, true));
                       
                    }
                }

            }
            else
            {
                for (int i = 0; i < deckCount; i++)
                {
                    if (rank % 14 == 0)
                    {
                        suit++;
                        rank = 1;
                    }
                    currentDeck.Add(new CardData(suit, rank, true));
                    rank++;
                }
            }*/
        }
        public int GetSeenPlayerCount()
        {
            int count = 0;
            foreach (PlayerState ps in players)
            {
                if (!ps.hasPacked && ps.hasSeenCard)
                    count = count + 1;
            }
            return count;
        }
        public CardData GetRandomCard()
        {
            if (currentDeck == null) 
            {
                Debug.Log("NewDeck===========>3");
                NewDeck();
            
            }
            if (currentDeck.Count < 2) 
            {
                Debug.Log("NewDeck===========>4");
                NewDeck();
            
            }

            int randomNum = UnityEngine.Random.Range(0, currentDeck.Count);
            CardData cardGive = currentDeck[randomNum];
            currentDeck.RemoveAt(randomNum);
            return cardGive;
        }

        public void dealersprite()
        {
            dealerindex = CommonFunctions.Instance.UniqueRandomInt(CommonFunctions.Instance.ExcludeNumbersFromRange(new HashSet<int> { 1, 6, 9 }, 0, 11), CommonFunctions.Instance.ExcludeNumbersFromRange(new HashSet<int> { 1, 6, 9 }, 0, 11));

        }


        public void InitNewRoundState()
        {
            if (gameController.IsTournament && CurrentGameCount > 0)
                currentState = 1;
            else
                currentState = 0;
            gameStartTime = 0;
            totalPot = currentStake = 0;
            sideShowRequestReceiver = -1;
            sideShowRequestSender = -1;
            sideShowRequestTime = -1;
            isSideShowRequestSend = false;
            isDealCard = false;
            forceSee = false;
            Debug.Log("NewDeck===========>5");
            NewDeck();
            if (gameController.CurrentGameMode.ToString() == "ZANDU")
                InitZanduMode();
            /*else if (gameController.CurrentGameMode == GameMode.HUKAM)
                InitHUKAMMode();*/

            players.RemoveAll(x => x.disconnectTime > 0);
            foreach (PlayerState ps in players)
            {
                ps.ResetState();
            }
            if ((players.Count + waitingPlayers.Count) == 1)
            {
                if(players.Count == 1)
                {
                    waitingPlayers.Add(players[0]);
                    players.Clear();
                }
            }
            currentMatchToken = string.Empty;
        }

        public void CloseAllCards()
        {
            foreach (PlayerState ps in players)
                ps.CloseCards();
            foreach (PlayerState ps in waitingPlayers)
                ps.CloseCards();
        }

        public void InitZanduMode()
        {
            zanducards = new CardData[3];
            zanthuCount = 0;
            for (int i = 0; i < 3; i++)
            {
                zanducards[i] = GetRandomCard();
            }
        }
        public void InitHUKAMMode()
        {
            zanducards = new CardData[1];
            zanthuCount = 0;
            zanducards[0] = GetRandomCard();
        }
        public void SetCurrentStake(float amnt)
        {
            currentStake = amnt;
            foreach (PlayerState ps in players)
            {
                if (ps.hasSeenCard)
                    ps.currentBoot = currentStake;
                else
                    ps.currentBoot = currentStake / 2;
            }
        }

    }
    [System.Serializable]
    public struct PlayerData
    {
        public string playerID;
        public string playerName;
        public CardData[] currentCards;
        public CardsCombination currentCombination;
        public bool isBot;
        public string profilePicURL;
        public int avatarIndex;
        public double gold;
        public double silver;
        public double money;
        public float currentBootPlayer;
        public int experience;
        public bool isDoubleBoot;
        public int playerLevel;
        public int playerFriendsCount;
        public int handsWonCount;
        public int currentTableGamesWon;
        public double currentTableChipsWon;
        public AuthenticationData auth;
    }

    [System.Serializable]
    public class PlayerState
    {
        public PlayerData playerData = new PlayerData();
        public int lives = 1;
        public bool isMyTurn = false;
        public int ui;
        public double CurrentGameSpend = 0;
        public int winnintSteak;
        public bool hasSeenCard = false;
        public bool hasSeenCardBoolCheck = false;
        public bool hasPacked = false;
        /// <summary>  0 - InWaiting List, 1 - Playing, 2 - Packed, 3 - Side Show, 4 - Show, 5 - Waiting to Join, 6 - Spectate </summary>
        public int currentState = 0;
        public int cardStrength = -1;
        public double myTurnTime = -1;
        public double disconnectTime = -1;
        public float currentBoot = 0;
        public int turnCount;
        public bool hasAllin;
        public int allinPosition;
        public double allinAmount;
        public bool isCardShow = false;
        public bool isSpectator = false;
        public int BetIndex;
        public string BetId;
        public void ResetState()
        {
            playerData.currentCards = new CardData[3];
            allinPosition = 0;
            lives = 1;
            allinAmount = 0;
            hasAllin = false;
            isMyTurn = false;
            hasSeenCard = false;
            hasPacked = false;
            currentState = 0;
            cardStrength = -1;
            myTurnTime = -1;
            isCardShow = false;
            disconnectTime = -1;
            currentBoot = 0;
            turnCount = 0;
            CurrentGameSpend = 0;
            BetIndex = 0;
        }
        public CardData[] GetCurrentCards()
        {
            return playerData.currentCards;
        }
        public void SetCardsSeen()
        {
            if (hasSeenCard == false)
            {
                hasSeenCardBoolCheck = true;
                hasSeenCard = true;
                currentBoot *= 2;
            }
        }
        public void CloseCards()
        {
            if (playerData.currentCards == null)
                playerData.currentCards = new CardData[3];

            for (int i = 0; i < 3; i++)
            {
                playerData.currentCards[i].isClose = true;
            }
        }

        public void SetMyTurn(bool val)
        {
            isMyTurn = val;
            if (val)
            {
                myTurnTime = NetworkTime.time;
            }
            else
            {
                hasSeenCardBoolCheck = false;
                myTurnTime = -1;
            }
               
        }
    }

}