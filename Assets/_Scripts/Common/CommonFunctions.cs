using TP;
using System;
using System.Globalization;
using System.Linq;
using UnityEngine;
using DG.Tweening;

using System.Net;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Networking;

#if UNITY_ANDROID && !UNITY_EDITOR
using UnityEngine.Android;
#endif

namespace TP
{
    public class CommonFunctions : SingletonMonoBehaviour<CommonFunctions>
    {
        #region PlayerPrefs
        public bool CheckHasKey(string key)

        {
#if UNITY_EDITOR
            if (ParrelSync.ClonesManager.IsClone()) return false;
            else return PlayerPrefs.HasKey(key);
#else
            return PlayerPrefs.HasKey(key);
#endif
        }

        #region Settings Panel

        public void ClearSavedSettings()
        {
            PlayerPrefs.DeleteKey(StaticStrings.Pref_SoundToggleKey);
            PlayerPrefs.DeleteKey(StaticStrings.Pref_VibrateToggleKey);
            PlayerPrefs.DeleteKey(StaticStrings.Pref_PortraitToggleKey);
            PlayerPrefs.DeleteKey(StaticStrings.Pref_OrientationKey);
            PlayerPrefs.Save();
        }


 
        /// <summary>
        ///  Check wether app installed for First Time
        /// </summary>
        /// 

      


        public  int GetGullakIntroShowedOnce()
        {
            return PlayerPrefs.GetInt(StaticStrings.Pref_GullakIntoScreenKey,1);
        }
      

#endregion Settings Panel


        public void ClearSavedLogin()
        {
            PlayerPrefs.DeleteKey(StaticStrings.Pref_LoginTypeKey);
            PlayerPrefs.DeleteKey(StaticStrings.Pref_SavedUserNameKey);
            PlayerPrefs.DeleteKey(StaticStrings.Pref_SavedPasswordKey);
            PlayerPrefs.Save();
        }







        #region LastGameRoom
        public string GetLastEnteredLobby()
        {
#if UNITY_EDITOR
            if (ParrelSync.ClonesManager.IsClone()) return string.Empty;
#endif
            return PlayerPrefs.GetString(StaticStrings.Pref_LastGameLobbyKey, string.Empty);
        }
        public string GetLastEnteredRoom()
        {
#if UNITY_EDITOR
            if (ParrelSync.ClonesManager.IsClone()) return string.Empty;
#endif
            return PlayerPrefs.GetString(StaticStrings.Pref_LastGameRoomKey, string.Empty);
        }

        public void SetLastEnteredRoom(string roomName, string lobbyName)
        {
#if UNITY_EDITOR
            if (ParrelSync.ClonesManager.IsClone()) return;
#endif
            PlayerPrefs.SetString(StaticStrings.Pref_LastGameRoomKey, roomName);
            PlayerPrefs.SetString(StaticStrings.Pref_LastGameLobbyKey, lobbyName);
            PlayerPrefs.Save();
        }

        public void ClearLastEnteredRoom()
        {
            PlayerPrefs.DeleteKey(StaticStrings.Pref_LastGameLobbyKey);
            PlayerPrefs.DeleteKey(StaticStrings.Pref_LastGameRoomKey);
            PlayerPrefs.Save();
        }
#endregion

#endregion


        public void sendInviteViaWP()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        //     Share on WhatsApp only, if installed (Android only)
            if( NativeShare.TargetExists( "com.whatsapp" ))
            	new NativeShare().AddTarget( "com.whatsapp" ).SetText(GameController.Instance.CurrentPlayerData.GetPrivateRoomMsgTemplate().Replace("$$", Environment.NewLine)
                    .Replace("XXXX", GameController.Instance.CurrentGameMode.ToString().ToLower())
                    .Replace("YYYY", GameController.Instance.privateRoomCode)
                    .Replace("ZZZZ", "https://tpgc.com/?ly="+ (int)GameController.Instance.CurrentGameType + "&rc="+ GameController.Instance.privateRoomCode)
                    ).Share();

#endif
        }
        public Sprite GetProfilePicture(int playerProfile, string url)
        {
                return GameController.Instance.avatharPicture[int.Parse(url)];
        }

        public string GetAmountDecimalSeparator(double value, bool isIndianFormat = true, int spriteindex = 5)
        {
            if (value < 100000000)
            {

#if GOP
                    return string.Format("<sprite index={0}>", spriteindex) + value.ToString("0.00");
#else
                return value.ToString("0.00");
#endif

            }
            if (isIndianFormat)
            {
                var currencyFormat = (NumberFormatInfo)CultureInfo.GetCultureInfo("en-IN").NumberFormat.Clone();
                currencyFormat.CurrencySymbol = "";
                if (GameController.Instance.CurrentAmountType == CashType.CASH)
                {

#if GOP
                        currencyFormat.CurrencySymbol = string.Format("<sprite index={0}>", spriteindex);
#else
                    currencyFormat.CurrencySymbol = string.Format("<sprite index=5>", spriteindex);
#endif
                }
                else
                {
#if GOP
                        currencyFormat.CurrencySymbol = "<sprite index=5>";
#else
                    currencyFormat.CurrencySymbol = "<sprite index=5>";
#endif  //Added by gibson Rupee symbol was Missing in non Cash Games
                }

                currencyFormat.CurrencyDecimalDigits = 0;
                return value.ToString("C", currencyFormat);
            }
            else
            {
                var currencyFormat = (NumberFormatInfo)CultureInfo.GetCultureInfo("en").NumberFormat.Clone();
                currencyFormat.CurrencySymbol = "$";
                currencyFormat.CurrencyDecimalDigits = 0;
                return value.ToString("C", currencyFormat);
            }
        }
        public double GetTime()
        {
     return (DateTime.UtcNow.ToUniversalTime() - new DateTime(2022, 9, 9)).TotalSeconds;
        }

#region added by Babu Vignesh kumar
        public string TpvAmountSeparator(double value ,bool isIndianCurrency = true)
        {
            
                return value.ToString("0.00");
            
            if (isIndianCurrency)
            {
                var currencyformat = (NumberFormatInfo)CultureInfo.GetCultureInfo("en-IN").NumberFormat.Clone();
                currencyformat.ToString();
                currencyformat.CurrencySymbol = string.Empty;


                currencyformat.CurrencyDecimalDigits = 0;
                return value.ToString("C",currencyformat);
            }
            else
            {
                var currencyformat = (NumberFormatInfo)CultureInfo.GetCultureInfo("en").NumberFormat.Clone();
                currencyformat.ToString();
                currencyformat.CurrencyDecimalDigits = 0;
                return value.ToString("C", currencyformat);
            }
        }
        #endregion
        public IEnumerator CheckInternetConnection(Action<bool> connectionAction)
        {
            //gameHUD.isWaitingForRequest = true;
            //UnityWebRequest www = UnityWebRequest.Get("https://clients3.google.com/generate_204");
            //www.timeout = 3;
            //yield return www.SendWebRequest();

            // Check if there is any internet connectivity
            //if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError || www.responseCode == 0)
            //{
            //    connectionAction(false);
            //    yield break;
            //}

            //if (www.error != null)
            //{
            //    connectionAction(false);
            //}
            //else
            //{
            //    connectionAction(www.responseCode.ToString() == "204");
            //}

            yield return  new WaitForSeconds(0);
        }


        public void CopyReferralCode()
        {
            CopyToClipBoard(GameController.Instance.CurrentPlayerData.GetReferralCode());
        }
        public void CopyToClipBoard(string value)
        {
            TextEditor Text = new TextEditor();
            Text.text = value;
            Text.SelectAll();
            Text.Copy();

        }
        public void Transform_Object(Vector2 startPos, Vector2 endPos, Transform trans, float delay)
        {
            trans.localPosition = startPos;
            trans.DOLocalMove(endPos,delay,false);
        }
        public string GenRandomAlphaNum(int length)
        {
            //millisecond seed
            var random = new System.Random(DateTime.Now.Millisecond); //requires seed
            return new string(Enumerable.Repeat(StaticStrings.AvailChars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public string GetAmountAbreviation(double value, bool isShortForm = true, bool isIndianFormat = true)
        {
            if (isIndianFormat)
            {
                return GetIndianAbreviation(value, isShortForm).Trim();
            }
            else
            {
                return GetWesternAbreviation(value, isShortForm).Trim();
            }
        }
        public int ExcludeNumbersFromRange(HashSet<int> valuetoExclude,int min ,int max)
        {
            var exclude = valuetoExclude; 
            var range = Enumerable.Range(min, max).Where(e => !exclude.Contains(e));
            var rand = new System.Random();
            int index = rand.Next(min, max - exclude.Count);
            return range.ElementAt(index);
        }

        List<int> usedValues = new List<int>();   
        public int UniqueRandomInt(int min, int max)
        {
            int val = UnityEngine.Random.Range(min, max);
            while (usedValues.Contains(val))
            {
                val = UnityEngine.Random.Range(min, max);
            }
            return val;

        }
        private string GetIndianAbreviation(double value, bool isShortForm)
        {
            if (value < 100000) return Math.Round(value,2).ToString();

            // Ensure number has max 3 significant digits (no rounding up can happen)
            double i = Math.Pow(10, (int)Math.Max(0, Math.Log10(value) - 2));
            value = value / i * i;

            if (value >= 10000000)
                return (value / 10000000D).ToString("0.##") + (isShortForm ? " Cr" : " Crore");
            if (value >= 100000)
                return (value / 100000D).ToString("0.##") + (isShortForm ? " L" : " Lakh");
            if (value >= 1000)
                return (value / 1000D).ToString("0.##") + (isShortForm ? " K" : " ");

            return value.ToString("#,0");
        }

        private string GetWesternAbreviation(double value, bool isShortForm)
        {
            if (value < 1000) return value.ToString();

            // Ensure number has max 3 significant digits (no rounding up can happen)
            long i = (long)Math.Pow(10, (int)Math.Max(0, Math.Log10(value) - 2));
            value = value / i * i;

            if (value >= 1000000000)
                return (value / 1000000000D).ToString("0.##") + (isShortForm ? " B" : " Billion");
            if (value >= 1000000)
                return (value / 1000000D).ToString("0.##") + (isShortForm ? " M" : " Million");
            if (value >= 1000)
                return (value / 1000D).ToString("0.##") + (isShortForm ? " K" : " ");

            return value.ToString("#,0");
        }

        public void SendEmail()
        {
            string email = "teenpattigold@gmail.com";
            string subject = MyEscapeURL("Contact Us");
            string body = MyEscapeURL("User ID -" + " " + GameController.Instance.CurrentPlayerData.GetPlayfabID().ToString() + " \n Version -" + " " + Application.version + " \n Device -" + "" + SystemInfo.deviceUniqueIdentifier + " \n Install OS -" + " " + SystemInfo.operatingSystem + " \n <br> ---- Please write below this line ------ ");
            Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
        }
        string MyEscapeURL(string url)
        {

            return WWW.EscapeURL(url).Replace("+", "%20");
        }

        public float ReMapFloat(float Imin, float Imax, float oMin, float Omax, float v)
        {
            float t = InverseLerpFloat(Imin, Imax, v);
            return LerpFloat(oMin, Omax, t);
        }

        public Color ReMapColor(float Imin, float Imax, Color oMin, Color Omax, float v)
        {
            float t = InverseLerpFloat(Imin, Imax, v);
            return LerpColor(oMin, Omax, t);
        }

        public float InverseLerpFloat(float a, float b, float v)
        {
            return (v - a) / (b - a);
        }

        public float LerpFloat(float a, float b, float t)
        {
            return (1.0f - t) * a + b * t;
        }

        public Color LerpColor(Color a, Color b, float t)
        {
            return Color.Lerp(a, b, t);
        }

        public string GetTruncatedPlayerName(string palyerName, int limit = 12)
        {
            //return palyerName;
            if (string.IsNullOrEmpty(palyerName) || palyerName.Length <= limit) return palyerName;
            return palyerName.Substring(0, limit) + "...";
        }

        public int GetBonusAtNextLevel(int levelVal)
        {
            return (GetXpAtLevel(levelVal + 1) * 20);
        }

        public int GetXpAtLevel(int levelVal)
        {
            //return (int)(25 * Mathf.Pow(levelVal -1, 2));
            return 25 * levelVal * levelVal - 25 * levelVal;
            //return (int) Mathf.Pow((levelVal / 0.22f) , 2);
        }

        public int GetLevelAtXp(int xpVal)
        {
            //return  (int)Mathf.Floor(Mathf.Log((xpVal/25+1), 2));
            return (int)Mathf.Floor(25 + Mathf.Sqrt(625 + 100 * xpVal)) / 50;
            //return (int)(0.22f * Mathf.Sqrt(xpVal));
        }

        public void DoTransForm(Transform objectToMove, Transform targetToMove, float time, Action OnTweenComplete, Ease easeType = Ease.Linear)
        {
            objectToMove.transform.DOMove(targetToMove.transform.position, time).SetEase(easeType);
            objectToMove.transform.DORotateQuaternion(targetToMove.transform.rotation, time).SetEase(easeType);
            objectToMove.transform.DOScale(targetToMove.transform.localScale, time).SetEase(easeType).OnComplete(() => OnTweenComplete!.Invoke());
        }


        public int GetRequiredXpForNextLevel(int levelVal, int xpVal)
        {
            return GetXpAtLevel(levelVal + 1) - xpVal;
        }

       /* public string GetGameNameFromEnum(GameMode val)
        {
            string returnVal = string.Empty;
            switch (val)
            {
                case GameMode.POKER:
                    returnVal = "Poker";
                    break;
                case GameMode.POINTRUMMY:
                    returnVal = "Point Poker";
                    break;
                case GameMode.POOLRUMMY:
                    returnVal = "Pool Rummy";
                    break;
                case GameMode.DEALRUMMY:
                    returnVal = "Deal Rummy";
                    break;
                case GameMode.OMAHAPOKER:
                    returnVal = "Omaha Poker";
                    break;
                case GameMode.NOLIMITPOKER:
                    returnVal = "No Limit Poker";
                    break;
                case GameMode.NOLIMITS:
                    returnVal = "No Limits";
                    break;
                case GameMode.ANTHARBAHAR:
                    returnVal = "Andar Bahar";
                    break;
                default:

                    returnVal = val.ToString();
                    break;
            }
            return returnVal;
        }
*/
        public char SetToUpper(char c)
        {
            return char.ToUpper(c);
        }

        public char OnProofValidateInput(string input, int charIndex, char addedChar)
        {
            if ((addedChar >= '0' && addedChar <= '9') || (addedChar >= 'a' && addedChar <= 'z') || (addedChar >= 'A' && addedChar <= 'Z'))
            {
                return SetToUpper(addedChar);
            }
            return '\0';
        }

        public Int32 GetAgeTillToday(DateTime dateOfBirth)
        {
            var today = DateTime.Today;

            var a = (today.Year * 100 + today.Month) * 100 + today.Day;
            var b = (dateOfBirth.Year * 100 + dateOfBirth.Month) * 100 + dateOfBirth.Day;

            return (a - b) / 10000;
        }

        public void RequestAnyPermission(string PermissionType, Action<string> OnPermissionDenied, Action<string> OnPermissionGranted, Action<string> OnPermissionDeniedAndDontAskAgain)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (!Permission.HasUserAuthorizedPermission(PermissionType))
            {
                var callbacks = new PermissionCallbacks();
                callbacks.PermissionDenied += OnPermissionDenied;
                callbacks.PermissionGranted += OnPermissionGranted;
                callbacks.PermissionDeniedAndDontAskAgain += OnPermissionDeniedAndDontAskAgain;
                Permission.RequestUserPermission(PermissionType, callbacks);
            }
#endif
        }

        public string ChangeDateFormat(string inputDate, string[] specificFormats)
        {
            string[] formats = { "dd/MM/yyyy", "dd/M/yyyy", "d/M/yyyy", "d/MM/yyyy",
                        "dd/MM/yy", "dd/M/yy", "d/M/yy", "d/MM/yy", "MM/dd/yyyy","yyyy/MM/dd","dd/MM/yyyy","MM-dd-yyyy","yyyyMMdd"};
            DateTime.TryParseExact(inputDate, specificFormats == null ? formats : specificFormats,
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None, out DateTime outputDate);
            return outputDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        }

        [ContextMenu("Find Sequences")]
        public void FindSequences()
        {
            Debug.Log("GetXpAtLevel:0:" + GetXpAtLevel(0));
            Debug.Log("GetXpAtLevel:1:" + GetXpAtLevel(1));
            Debug.Log("GetXpAtLevel:2:" + GetXpAtLevel(2));
            Debug.Log("GetXpAtLevel:3:" + GetXpAtLevel(3));
            Debug.Log("GetXpAtLevel:4:" + GetXpAtLevel(4));
            Debug.Log("GetXpAtLevel:5:" + GetXpAtLevel(5));

            Debug.Log("GetLevelAtXp:0:" + GetLevelAtXp(0));
            Debug.Log("GetLevelAtXp:20:" + GetLevelAtXp(20));
            Debug.Log("GetLevelAtXp:80:" + GetLevelAtXp(80));
            Debug.Log("GetLevelAtXp:85:" + GetLevelAtXp(85));
            Debug.Log("GetLevelAtXp:120:" + GetLevelAtXp(120));
            Debug.Log("GetLevelAtXp:185:" + GetLevelAtXp(185));
            Debug.Log("GetLevelAtXp:200:" + GetLevelAtXp(200));
            Debug.Log("GetLevelAtXp:300:" + GetLevelAtXp(300));
            Debug.Log("GetLevelAtXp:360:" + GetLevelAtXp(360));
            Debug.Log("GetLevelAtXp:480:" + GetLevelAtXp(480));

            Debug.Log("GetBonusAtNextLevel:0:" + GetBonusAtNextLevel(0));
            Debug.Log("GetBonusAtNextLevel:1:" + GetBonusAtNextLevel(1));
            Debug.Log("GetBonusAtNextLevel:2:" + GetBonusAtNextLevel(2));
            Debug.Log("GetBonusAtNextLevel:3:" + GetBonusAtNextLevel(3));
            Debug.Log("GetBonusAtNextLevel:4:" + GetBonusAtNextLevel(4));
            Debug.Log("GetBonusAtNextLevel:5:" + GetBonusAtNextLevel(5));

        }
    }
}
