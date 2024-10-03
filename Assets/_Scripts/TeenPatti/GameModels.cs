
using System;
using System.Collections.Generic;
using UnityEngine;

namespace TP
{
    [Serializable]
    public class GameModelModelCollection
    {
        public GameModeModel Data;
    }

    [Serializable]
    public class GameModeModel 
    {
        public bool IsTeenPattiAvailable;
        public bool IsDeluxTeenPattiAvailable;
        public bool IsPokerAvailable;
        public bool IsRummyAvailable;
        public bool IsAndharBaharAvailable;
        public PokerTableModel[] PokerGameData;
        public TeenPattiTableModel[] TeenPattiData;
        public DeluxeTeenPattiTableModel[] DeluxeTeenPattiData;
        public RummyTableModel[] RummyData;
        public AndharBaharTableModel[] AndharBaharData;
        public TournamentModel TournamentData;
    }

    [Serializable]
    public class TournamentModel
    {
        public PokerTournamentModel[] PokerGameData;
        public TeenPattiTournamentModel[] TeenPattiData;
    }

    [Serializable]
    public class GameTableModel
    {
        public string Name;
        public GameTable Id;
        public bool IsEnable;
        public bool AddBot;
        //public GameMode GameVal;
        //public CashType CashVal;

        public virtual string GetConcatString()
        {
            return Name + "_" + Id;
        }
    }

    [Serializable]
    public class TRoundConfigModel
    {
        public int NoOfPlayer;
        public double StartingChips;
        public int BootIncreaseRate;
    }

    [Serializable]
    public class TeenPattiTRoundConfigModel : TRoundConfigModel
    {
        public int BootAmount;
        public int ChaalLimit;
        public int PotLimit;
        public int BlindLimit;
    }

    [Serializable]
    public class PokerTRoundConfigModel : TRoundConfigModel
    {
        public int SmallBlindAmount;
        public int MinBuyInAmount;
        public int MaxButInAmount;
    }

    [Serializable]
    public class TeenPattiTRoundModel
    {
        public string Name;
        public double FirstPrize;
        public double SecondPrize;
        public TeenPattiTRoundConfigModel RoundConfig;
    }
    
    [Serializable]
    public class PokerTRoundModel
    {
        public string Name;
        public double FirstPrize;
        public double SecondPrize;
        public PokerTRoundConfigModel RoundConfig;
    }


    [Serializable]
    public class TournamentTableModel : GameTableModel
    {
        public int TournamemtId;
        public double EntryFee;
        public double WinAmount;
        public int NoOfRound;
        public int IconNo;
        public GameMode GameModeVal;
        public CashType CashVal;
        public bool IsAllIn;
        public int RequiredLevel;
        public int currentPlayingRound;
        public double currentStartingChips;
    }

    [Serializable]
    public class PokerTableModel : GameTableModel
    {
        public double SmallBlindAmount;
        public double MinBuyInAmount;
        public double MaxButInAmount;
        public override string GetConcatString()
        {
            return "_" + SmallBlindAmount + "_" + MinBuyInAmount + "_" + MaxButInAmount;
        }
    }

    [Serializable]
    public class TeenPattiTableModel : GameTableModel
    {
        public int BlindLimit; //5
        public int ChaalLimit; //8000
        public int BootAmount; //500
        public int PotLimit; // 50000

        public override string GetConcatString()
        {
            return "_"+ BootAmount + "_" + ChaalLimit + "_" + BootAmount + "_" + PotLimit;
        }
    }

    [Serializable]
    public class DeluxeTeenPattiTableModel : TeenPattiTableModel
    {
    }

    [Serializable]
    public class PokerTournamentModel : TournamentTableModel
    {
        public PokerTRoundModel[] RoundData;
    }
    [Serializable]
    public class TeenPattiTournamentModel : TournamentTableModel
    {
        public TeenPattiTRoundModel[] RoundData;
    }


    [Serializable]
    public class RummyTableModel : GameTableModel
    {
        public int PointValue;
        public override string GetConcatString()
        {
            return "_" + PointValue;
        }

    }

    [Serializable]
    public class AndharBaharTableModel : GameTableModel
    {
        public int BootAmount;
        public override string GetConcatString()
        {
            return "_" + BootAmount;
        }
    }


    [Serializable]
    public struct StoreChipValue
    {
        public int amountToPay;
        public double silverToGet;
    }

    [Serializable]
    public class TournamentResultModel
    {
        public int tournamentId;
        public int roundNo;
        public bool isPoker;
        public string firstWinnerId;
        public string secondWinnerId;
        public List<string> loosersIdList = new List<string>();
        public List<TournamentPlayers> players = new List<TournamentPlayers>();
    }
    [Serializable]
    public class TournamentPlayers
    {
        public string playerId;
        public string playerName;
        public double chipsInHand;
        public int lastPlayedRound;
        public int rank;
    }


    [Serializable]
    public struct AddAmountToOpponentECSStruct
    {
        public int cashType;
        public string playerID;
        public double winAmount;
        public double spentAmount;
        public string matchToken;
        public int isBot;
    }

    [Serializable]
    public struct SubtractAmountFromOpponentECSStruct
    {
        public int cashType;
        public string playerID;
        public double amount;
        public string matchToken;
        public int isBot;
    }
    [Serializable]
    public struct UpdateStaticsECSStruct
    {
        public string player_id;
        public int cashType;
        public double winAmount;
        public double spendAmount;
        public int isBot;
    }
    [Serializable]
    public struct MatchLogsECSStruct
    {
        public string created_by;
        public string token;
        public string action;
        public string action_body;
        public string date_time;
        public int is_master;
        public int isBot;
    }
    [Serializable]
    public struct MatchCreateECStruct
    {
        public string created_by;
        public string match_type;
        public string match_date_time;
        public string game_type;
        public int cash_type;
    }
    [Serializable]
    public struct MatchPlayersECStruct
    {
        public string created_by;
        public string token;
        public string[] players;
        public string date_time;
    }
    [Serializable]
    public struct GetABotECStruct
    {
        public string currentPlayer;
        public double potLimit;
        public string gameName;
        public List<string> botListInGame;
    }


}