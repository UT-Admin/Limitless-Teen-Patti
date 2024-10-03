
namespace TP
{

    public enum PFErrorCode
    {
        General_Http_Error = -1,
        Terms_NotAccepted = -2,
        AgeLimit_NotAccepted = -3,
        Photon_Custom_Auth_Failed = -4,
        EmailId_Empty = 99000,
        EmailId_Invalid = 99001,
        EmailId_NotVerified = 99002,
        Password_Empty = 99003,
        Password_Invalid_Len = 99004,
        NickName_Invalid_Len = 99005,
        NickName_Empty = 99006,
        MobileNo_Empty = 99007,
        MobileNo_Invalid_Len = 99008,
        Referral_Empty = 99009,
        Referral_Invalid_Len = 99010,
        Referral_Invalid = 99011,
        Facebook_ID_Invalid = 99012,
        FB_ProfileUserId_Empty = 99013,
        OTP_Empty = 99014,
        OTP_Invalid = 99015,
        OTP_Expired = 99016,
        PlayFabID_Empty = 99017,
        Bank_Account_Name_Empty = 99018,
        Bank_Account_No_Empty = 99019,
        Bank_Name_Empty = 99020,
        Bank_Branch_Empty = 99021,
        Bank_IFSC_Empty = 99022,
        Google_ID_Invalid = 99023,

    }

    public enum ProfilePicType
    {
        None = 0,
        Index,
        Url,
        Facebook,
        FBIndex
    }

    public enum OnOffOption
    {
        OFF = 0,
        ON,
    }

    public enum LoginType
    {
        None = 0,
        MobileNo,
        EmailId,
        Facebook,
        DeviceId,
        Google
    }
   /* public enum CoinType
    {
        GOLD = 0,
        SILVER,
    }*/
    public enum LeaderboardType
    {

        WeeklyChipWon,
        WeeklyHandsWon,
        WeeklyHighPot,
        MonthlyChipWon,
        MonthlyHandsWon,
        MonthlyHighPot,
        TotalChipWon,
        TotalHandsWon,
        TotalHighPot
    }


    // The States a reward can have
    public enum DailyRewardState
    {
        CLAIMED = -1,
        UNCLAIMED_AVAILABLE = 0,
        UNCLAIMED_UNAVAILABLE = 1
    }

    public enum  FirstTimeAppInstalledShownPannel
    {
        yes =0,
        no
    }

    public enum KYCStatusState
    {
        NOT_UPLOADED = 0,
        PENDING_FOR_APPROVAL,
        APPROVED,
        REUPLOAD
    }

    public enum KYCProofType
    {
        PAN_CARD = 0,
        AADHAAR_CARD,
        DRIVING_LICENSE
    }
    public enum PFFriendIdType { PlayFabId, Username, Email, DisplayName };
}