using System;
using UnityEngine;

namespace TP
{

    [Serializable]
    public struct RegisterDetailsRequest
    {
        public string emailId;
        public string passWord;
        public string rePassWord;
        public string userName;
        public string mobileNo;
        public string referralCode;
        public bool termsToggle;
        public bool ageLimitToggle;
    }

    public struct PlayFabControllerError
    {
        public int errorCode;
        public string errorMessage;
    }

    [Serializable]
    public struct FacebookDetailsRequest
    {
        public string accessToken;
        public string profileUserId;
        public string profileName;
        public string profileFirstName;
        public string profilePicUrl;
    }

    [Serializable]
    public struct GoogleDetailsRequest
    {
        public string AuthCode;
        public string IdToken;
        public string profileUserId;
        public string profileName;
        public string profileFamilyName;
        public string profileGivenName;
        public string profileEmail;
        public string profilePicUrl;
    }


    [Serializable]
    public class LeaderboardPlayer
    {
       
        public Sprite profilePic;
        public LeaderboardType leaderboardType;
    }

    [Serializable]
    public class RewardsModelCollection
    {
        public RewardsModel[] Data;
    }


    [Serializable]
    public struct RewardsModel
    {
        public string Name;
        public int Day;
        public int Rewards;
        public DailyRewardState State;
    }

    [Serializable]
    public class ChiptStoreCollection
    {
        public ChipStoreModel[] Data;
    }


    [Serializable]
    public struct ChipStoreModel
    {
        public int IconId;
        public bool Popular;
        public string Chips;
        public string Bonus;
        public string Price;
        public int OldPrice;
    }
    [Serializable]
    public class BankDetailsModelCollection
    {
        public BankDetailsModel[] Data;
    }
    [Serializable]
    public class BankDetailsModel
    {
        public string BankName;
        public string AccName;
        public string AccNumber;
        public string IfscCode;
        public int IsDefault;
        public string PassbookUrl;
        public long AddedOn;
        public bool BankStatus = false;
    }

    [Serializable]
    public class TournamentDetailsModelCollection
    {
        public TournamentDetailsModel[] Items;
    }
    [Serializable]
    public class TournamentDetailsModel
    {
        public string name;
        public int tournamentId;
        public int currentRound;
    }

    [Serializable]
    public class KYCDetailsModelCollection
    {
        public KYCDetailsModel[] Data;
    }
    [Serializable]
    public class KYCDetailsModel
    {
        public string CardNumber;
        public KYCProofType CardType;
        public KYCImageModel[] KYCImageModels;
        public string CardMessage;
        public KYCStatusState CardStatus;
    }

    [Serializable]
    public class KYCImageModel
    {
        public string CardDisplayText;
        public string PhotoPicUrl;
    }

    [Serializable]
    public class HotTableModelCollection
    {
        public HotTableModel[] Data;
    }

    [Serializable]
    public class HotTableModel
    {
        public GameMode GM;
        public CashType CT;
        public int DM;
    }

    [Serializable]
    public class ExtraBonusModel
    {
        public int FBLogin;
        public int KYCVerified;
        public int ProfileVerified;
        public int SignUpBonus;
        public int TutorialCompleted;
    }


    [Serializable]
    public class ExtraBonusModelCollection
    {
        public ExtraBonusModel[] Data;
    }

   
}