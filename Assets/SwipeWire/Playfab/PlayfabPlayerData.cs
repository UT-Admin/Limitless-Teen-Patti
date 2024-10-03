
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Globalization;
using SimpleJSON;
using System.Text;

namespace TP
{
	[Serializable]
	public class PlayfabPlayerData
	{
		#region private variables

		#region profile title data
		[SerializeField] private string PlayfabID;
		[SerializeField] private string NickName;
		[SerializeField] private string FullName;
		[SerializeField] private string FacebookUserId;
		[SerializeField] private ProfilePicType AvatarIndex = ProfilePicType.None;
		[SerializeField] private int avatarIndex;
		[SerializeField] private string AvatarUrl;
		[SerializeField] private Sprite AvatarSprite;
		[SerializeField] private string ReferralCode;
		[SerializeField] private int ReferralCount;
		[SerializeField] private string ReferredBy;
		[SerializeField] private string DailyRewards;
		[SerializeField] private string CreatedDate;
		[SerializeField] private int PlayerLevel;
		[SerializeField] private int GamesPlayed;
		[SerializeField] private int GamesWon;
		[SerializeField] private int NetWinning;
		[SerializeField] private string MobileNo;
		[SerializeField] private string EmailId;
		[SerializeField] private string DOB;
		[SerializeField] private bool PlayerVerified;
		[SerializeField] private bool IsClaimedChipOffer = false;
		[SerializeField] private bool IsClaimedReferal = false;
		[SerializeField] private string LatestPaymentStatus;
		[SerializeField] private List<TournamentDetailsModel> TournamentDetails;
		[SerializeField] private List<HotTableModel> HotGamesList;

		[SerializeField] private string GoldVal;
		[SerializeField] private string GoldWinningVal;
		[SerializeField] private string GoldDepositVal;
		[SerializeField] private string SilverVal;
		[SerializeField] private int TrophyVal;
		[SerializeField] private double TotalDeposit;
		[SerializeField] private int PlayerXP;
		[SerializeField] private int RewardedVideoCount;
		[SerializeField] private int WeeklyHandsWon;

		[SerializeField] private KYCStatusState KYCStatus;
		[SerializeField] private BankDetailsModel[] BankDetails;
		[SerializeField] private KYCDetailsModel[] KYCDetails;
		[SerializeField] private ExtraBonusModel ExtraBonus;
		#endregion

		#region title data
		[SerializeField] private float AppVersion;
		[SerializeField] private string AndroidAppUrl;
		[SerializeField] private string SupportEmail;
		[SerializeField] private string WebsiteUrl;
		[SerializeField] private double ReferralBonus;
		[SerializeField] private double SignUpBonus;
		[SerializeField] private string ShareMsgTemplate;
		[SerializeField] private string PrivateRoomMsgTemplate;
		[SerializeField] private string RoomSummaryMsgTemplate;
		[SerializeField] private string RefferalMsgTemplate;
		[SerializeField] private string PaymentCollectUrl;
		[SerializeField] private string PaymentValidateUrl;
		[SerializeField] private string PaymentTransactionUrl;
		[SerializeField] private string PaymentWithdrawUrl;
		[SerializeField] private string KYCDetailsTemplate;
		[SerializeField] private string BankDetailsTemplate;
		[SerializeField] private string TermsUrl;
		[SerializeField] private string PrivacyPolicyUrl;
		[SerializeField] private string FileUploadUrl;
		[SerializeField] private string GetOtpUrl;
		[SerializeField] private string ValidateOtpUrl;
		[SerializeField] private string ValidateMobileUrl;
		[SerializeField] private string GetCurrentIpUrl;
		[SerializeField] private float TDSPercentage;
		[SerializeField] private float WinCommissionPercentage;
		//[SerializeField] private string ValidateMobileNoUrl;
		#endregion

		#endregion


		#region Title Data Getter

		public string GetBankDetailsTemplate()
		{
			return BankDetailsTemplate;
		}
		public string GetKYCDetailsTemplate()
		{
			return KYCDetailsTemplate;
		}
		public float GetAppVersion()
		{
			return AppVersion;
		}
		public string GetAndroidAppUrl()
		{
			return AndroidAppUrl;
		}
		public string GetSupportEmail()
		{
			return SupportEmail;
		}
		public string GetWebsiteUrl()
		{
			return WebsiteUrl;
		}
		public double GetReferralBonus()
		{
			return ReferralBonus;
		}
		public double GetSignUpBonus()
		{
			return SignUpBonus;
		}
		public string GetRefferalMsgTemplate()
		{
			return RefferalMsgTemplate;
		}
		public string GetPrivateRoomMsgTemplate()
		{
			return PrivateRoomMsgTemplate;
		}
		public string GetShareMsgTemplate()
		{
			return ShareMsgTemplate;
		}
		public string GetRoomSummaryMsgTemplate()
		{
			return RoomSummaryMsgTemplate;
		}

		public string GetPaymentCollectUrl()
		{
			return PaymentCollectUrl;
		}

		public string GetPaymentValidateUrl()
		{
			return PaymentValidateUrl;
		}

		public string GetPaymentTransactionUrl()
		{
			return PaymentTransactionUrl;
		}
		public string GetPaymentWithdrawUrl()
		{
			return PaymentWithdrawUrl;
		}

		public string GetTermsUrl()
		{
			return TermsUrl;
		}

		public string GetPrivacyPolicyUrl()
		{
			return PrivacyPolicyUrl;
		}

		public string GetFileUploadUrl()
		{
			return FileUploadUrl;
		}

		public double GetTDSPercentage()
		{
			return TDSPercentage;
		}
		public double GetWinCommissionPercentage()
		{
			return WinCommissionPercentage;
		}

		public string GetGetOtpUrl()
		{
			return GetOtpUrl;
		}

		public string GetValidateOtpUrl()
		{
			return ValidateOtpUrl;
		}

		public string GetValidateMobileUrl()
		{
			return ValidateMobileUrl;
		}

		public string GetGetCurrentIpUrl()
		{
			return GetCurrentIpUrl;
		}
		#endregion

		#region Details Setter

		public void SetAvatarIndex(int value)
		{

			AvatarUrl = value.ToString();
			AvatarIndex = ProfilePicType.Index;
		
			
		}
		public string SetGold(double val)
        {
			return GoldDepositVal = val.ToString();
        }

		public void SetAvatarSprite()
		{
			if (AvatarIndex == ProfilePicType.None)
			{
				SetAvatarIndex(UnityEngine.Random.Range(0, GameController.Instance.avatharPicture.Length));
			}
			else
			if (AvatarIndex == ProfilePicType.Url)
			{
				Sprite fallbackImage = GameController.Instance.avatharPicture[UnityEngine.Random.Range(0, GameController.Instance.avatharPicture.Length)];

				ImageCacheUtils.Instance.LoadFromCacheOrDownload(AvatarUrl, PlayfabID, fallbackImage, cacheImage =>
				{
					AvatarSprite = cacheImage;

					if (GameController.Instance.onPlayerDataChanged != null)
						GameController.Instance.onPlayerDataChanged.Invoke();
				});
			}
			else if (AvatarIndex == ProfilePicType.Index)
			{
				int picPos = 0;
				if (int.TryParse(AvatarUrl, out picPos))
					AvatarSprite = GameController.Instance.avatharPicture[picPos];
				else
					SetAvatarIndex(UnityEngine.Random.Range(0, GameController.Instance.avatharPicture.Length));
			}
			else if (AvatarIndex == ProfilePicType.FBIndex)
			{
				int picPos = 0;
				if (int.TryParse(GameController.Instance.CurrentPlayerData.GetAvatarUrl(), out picPos))
					AvatarSprite = GameController.Instance.fbAvatharPicture[picPos];
			}
			else if (AvatarIndex == ProfilePicType.Facebook)
			{


			}
		}


		public void SetEmail(string value)
		{
			EmailId = value;
		}

		public void SetNickName(string value)
		{
			NickName = value;

		}




		
		public void SetExtraBonusDetails(ExtraBonusModel value)
		{
			ExtraBonus = value;
		}

		public ExtraBonusModel GetExtraBonus()
		{
			return ExtraBonus;
		}
		public string GetMobileNumber()
		{
			return MobileNo;
		}
		public string GetEmailID()
		{
			return EmailId;
		}
		public string GetDOB()
		{
			return DOB;
		}
		public bool GetVerifiedPlayer()
		{

			return PlayerVerified;
		  
		}
		public List<TournamentDetailsModel> GetTournamentDetails()
		{
			return TournamentDetails;
		}
		#endregion

		#region Stats Setter

		public static string Base64Encode(string plainText)
		{
			var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
			return System.Convert.ToBase64String(plainTextBytes);
		}

		public void AddWinningAmountToOpponent(CashType cash, string playerid, double amount, double spendAmount, string token, int hasBot)
		{
			AddAmountToOpponentECSStruct addWinning = new AddAmountToOpponentECSStruct();
			addWinning.cashType = (int)cash;
			addWinning.playerID = playerid;
			addWinning.spentAmount = spendAmount;
			addWinning.winAmount = amount;
			addWinning.matchToken = token;
			addWinning.isBot = hasBot;
		}
		public void SubractAmountFromOpponent(CashType cash, string playerid, double amount, string token, int hasBot)
		{
			SubtractAmountFromOpponentECSStruct subAmount = new SubtractAmountFromOpponentECSStruct();
			subAmount.amount = amount;
			subAmount.cashType = (int)cash;
			subAmount.matchToken = token;
			subAmount.isBot = hasBot;
			subAmount.playerID = playerid;
		}

		public void SetMatchLogs(string matchToken, string action,string actionBody,int hasBot)
		{
			MatchLogsECSStruct log = new MatchLogsECSStruct();
			log.action_body =  actionBody;
			log.created_by = GetPlayfabID();
			log.date_time = DateTime.Now.ToString(StaticStrings.DateTimeFormat);
			log.is_master = 1;
			log.token = matchToken;
			log.action = action;
			log.isBot = hasBot;
		}
		public void MatchCreateECS(CashType cash, string gameType, string matchType,Action<string, bool> successAction, Action failureAction,bool botCount)
		{



			MatchCreateECStruct match = new MatchCreateECStruct();
			match.created_by = GetPlayfabID();
			match.cash_type = (int)cash;
			match.game_type = gameType;
			match.match_type = matchType;
			match.match_date_time = DateTime.Now.ToString(StaticStrings.DateTimeFormat);
		
		}

		public void AddMatchPlayersECS(string tokens,string[] playerList)
		{
			MatchPlayersECStruct players = new MatchPlayersECStruct();
			players.players = playerList;
			players.token = tokens;
			players.date_time = DateTime.Now.ToString(StaticStrings.DateTimeFormat);
			players.created_by = GetPlayfabID();
		}
		public void GetABotECS(double pot, string gameName, List<string> botList, Action<string> success)
		{
			GetABotECStruct bot = new GetABotECStruct();
			bot.currentPlayer = "";
			bot.gameName = gameName;
			bot.potLimit = pot;
			bot.botListInGame = botList;     
		}


		public void UpdateMatchStatices(CashType cash, string playerid, double winAmount, double spendAmount, string token, int hasBot)
		{
			UpdateStaticsECSStruct statices = new UpdateStaticsECSStruct();
			statices.isBot = hasBot;
			statices.player_id = playerid;
			statices.spendAmount = spendAmount;
			statices.winAmount = winAmount;
			statices.cashType = (int)cash;
		}

		
	
		public int GetTournamentCurrentRound(TournamentDetailsModel value)
		{
			int data = 0;
			if(TournamentDetails.Exists(x => x.tournamentId == value.tournamentId))
			{
				data = TournamentDetails.Find(x => x.tournamentId == value.tournamentId).currentRound;
			}
			return data;
		}

		public void RemoveTournamentData(TournamentDetailsModel value)
		{
			TournamentDetails.RemoveAll(x=>x.tournamentId == value.tournamentId);
		}
		public void SetTournamentDetails(TournamentDetailsModel value)
		{
			if(TournamentDetails.Exists(x=>x.tournamentId == value.tournamentId))
			{
				TournamentDetailsModel data = TournamentDetails.Find(x => x.tournamentId == value.tournamentId);
				data = value;
			}
			else
			{
				TournamentDetails.Add(value);
			}
		}
		public void SetTournamentDetails(List<TournamentDetailsModel> _value)
		{
			this.TournamentDetails = _value;
		}

		public void SetHotGamesList(List<HotTableModel> _value)
		{
			this.HotGamesList = _value;
		}
		public List<HotTableModel> GetHotGamesList()
		{
			return this.HotGamesList;
		}
		#endregion


		private System.Random random = new System.Random();

		private string generatedID = null;
		private const string allowedChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

		private string GenerateRandomID()
		{
			if (generatedID == null)
			{
				StringBuilder idBuilder = new StringBuilder(30);


				for (int i = 0; i < 30; i++)
				{
					int randomIndex = random.Next(0, allowedChars.Length);
					char randomChar = allowedChars[randomIndex];
					idBuilder.Append(randomChar);
				}

				generatedID = idBuilder.ToString();
			}

			return generatedID;
		}
		#region Details Getter



		public string GetPlayfabID()
		{
			PlayfabID = APIController.instance.userDetails.Id;
			return PlayfabID;
		}

		private List<string> names = new List<string>
	{
		"Alice", "Bob", "Charlie", "David", "Eve", "Frank"
	};

	
		private string GetRandomName()
		{
			return names[UnityEngine.Random.Range(0, names.Count)];
		}


		public string GetNickName()
		{
			return NickName;
		}
		public string GetFullName()
		{
			return FullName;
		}
		public ProfilePicType GetAvatarIndex()
		{
			return AvatarIndex;
		}
		public string GetAvatarUrl()
		{
			return AvatarUrl;
		}
		public Sprite GetAvatarSprite()
		{
			return AvatarSprite;
		}
		public string GetReferralCode()
		{
			return ReferralCode;
		}
		public int GetReferralCount()
		{
			return ReferralCount;
		}
		public string GetReferredBy()
		{
			return ReferredBy;
		}
		public string GetDailyRewards()
		{
			return DailyRewards;
		}
		public DateTime GetCreatedDate()
		{
			Debug.Log("CreatedDate:::" + CreatedDate);

			DateTime parsedDateTime = DateTime.Now;
			if(!DateTime.TryParseExact(CreatedDate,
									   "yyyy-MM-dd'T'HH:mm:ss.ff'Z'",
									   CultureInfo.InvariantCulture,
									   DateTimeStyles.AssumeUniversal |
									   DateTimeStyles.AdjustToUniversal, out parsedDateTime))
			{
				var dateOnly = CreatedDate.Substring(0, CreatedDate.IndexOf('T'));
				parsedDateTime = DateTime.ParseExact(dateOnly,
													"yyyy-MM-dd",
													CultureInfo.InvariantCulture);
			}
			return parsedDateTime;
		}

		public string GetLatestPaymentStatus()
		{
			return LatestPaymentStatus;
		}
		#endregion

		#region Stats Getter

	
		public int GetPlayerLevel()
		{
			return PlayerLevel;
		}
		public int GetGamesPlayed()
		{
			return GamesPlayed;
		}
		public int GetGamesWon()
		{
			return GamesWon;
		}
		public int GetNetWinning()
		{
			return NetWinning;
		}
		public double GetGoldVal()
		{
			if (string.IsNullOrEmpty(GoldDepositVal) && string.IsNullOrEmpty(GoldWinningVal)) return 0.0;
			else if (string.IsNullOrEmpty(GoldDepositVal)) return double.Parse(GoldWinningVal);
			else if (string.IsNullOrEmpty(GoldWinningVal)) return double.Parse(GoldDepositVal);
			return double.Parse(GoldDepositVal) + double.Parse(GoldWinningVal);

		}
		public int GetXPVal()
		{
			return PlayerXP;
		}
		public double GetGoldWinVal()
		{
			//if (string.IsNullOrEmpty(SilverVal)) return 0.0;
			return double.Parse(GoldWinningVal);
		}
		public double GetGoldDepositVal()
		{
			//if (string.IsNullOrEmpty(SilverVal)) return 0.0;
			return double.Parse(GoldDepositVal);
		}
		public double GetSilverVal()
		{
			if (string.IsNullOrEmpty(SilverVal)) return 0.0;
			return double.Parse(SilverVal);
		}
		public int GetTrophyVal()
		{
			return TrophyVal;
		}
		public int GetPlayerXPVal()
		{
			return PlayerXP;
		}
		public int GetWeeklyHandsWonVal()
		{
			return WeeklyHandsWon;
		}
		public int GetRewardedVideoCountVal()
		{
			return RewardedVideoCount;
		}
		public string GetFacebookUserId()
		{
			return FacebookUserId;
		}
		public bool IsFBLoggedInLinked()
		{
			return !string.IsNullOrEmpty(FacebookUserId);
		}
		public bool GetClaimedOffer()
		{
			return IsClaimedChipOffer;
		}
		public bool GetClaimedReferal()
		{
			return IsClaimedReferal;
		}
		#endregion


		#region Bank Details Getter
		public void SetBankDetails(BankDetailsModel[] value)
		{
			BankDetails = value;
		}
		public BankDetailsModel[] GetBankDetails()
		{
			return BankDetails;
		}

		public double GetTotalDeposit()
		{
			return TotalDeposit;
		}
		/*public double GetTotalWithDraw()
		{
			return TotalWithDraw;
		}*/
		#endregion

		#region KYC Details Getter
		public void SetKYCDetails(KYCDetailsModel[] value)
		{
			KYCDetails = value;
		}
		public KYCDetailsModel[] GetKYCDetails()
		{
			return KYCDetails;
		}
		public KYCStatusState GetKYCStatus()
		{
			return KYCStatus;
		}
		#endregion

		#region Helper

	

		public void LoadFromJson(string jsonData, Action onComplete)
		{
			JsonUtility.FromJsonOverwrite(jsonData, this);
			onComplete.Invoke();
		}

		public void SetPlayerDetailsData(string jsonData, Action onComplete)
		{
			var _cacheUrl = AvatarUrl;
			var _cacheLevel = PlayerLevel;

			JsonUtility.FromJsonOverwrite(jsonData, this);
			if (PlayerLevel > _cacheLevel && _cacheLevel >= 1)
			{
				if (GameController.Instance.onPlayerLeveledUp != null)
					GameController.Instance.onPlayerLeveledUp.Invoke();
			}

			Debug.Log("AvatarSprite == null::" + AvatarSprite == null);
			Debug.Log(" _cacheUrl != AvatarUrl::" + _cacheUrl != AvatarUrl);
			if (AvatarSprite == null || ((AvatarIndex == ProfilePicType.Url || AvatarIndex == ProfilePicType.Facebook) && _cacheUrl != AvatarUrl))
			{
				SetAvatarSprite();
			}
			else
			{
				if (GameController.Instance.onPlayerDataChanged != null)
					GameController.Instance.onPlayerDataChanged.Invoke();
			}

		   

			onComplete.Invoke();

		}

		public void ClearDetails()
		{
			GameController.Instance.CurrentPlayerData = new PlayfabPlayerData();

			if (GameController.Instance.onPlayerDataChanged != null)
				GameController.Instance.onPlayerDataChanged.Invoke();

		}
		#endregion

	}
}
