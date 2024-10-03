using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if TPG_ADS
using UnityEngine.Advertisements;
#endif
using UnityEngine.UI;

namespace TP
{


#if TPG_ADS
    public class AdsManager : SingletonMonoBehaviour<AdsManager>, IUnityAdsListener
    {
        string gameID = "4242373";
        bool testmode = true;

        string videoAdID = "Interstitial_Android";
        string RewardableVideoAdID = "Rewarded_Android";
        Action<bool> onCompleteAction;

        private void Start()
        {
            Advertisement.AddListener(this);

            if (!Advertisement.IsReady())
            {
                Advertisement.Initialize(gameID, testmode);
            }

            else
            {
                Debug.Log("No ads available now"); //failure action comes here
            }
        }

        public void DisplayVideoAds()
        {
            Advertisement.Show(videoAdID);
        }

        public void ShowRewardedVideo(Action<bool> onCompleteAction)
        {
            this.onCompleteAction = onCompleteAction;
            // Check if UnityAds ready before calling Show method:
            if (Advertisement.IsReady(RewardableVideoAdID))
            {
                Advertisement.Show(RewardableVideoAdID);
            }
            else
            {
                UIController.Instance.adsFailedPopup.ShowMe();
                AdsFailedPoupu.instance.message("No Ads to view currently check again in sometime");
            }
        }

        // Implement IUnityAdsListener interface methods:
        public void OnUnityAdsDidFinish(string surfacingId, ShowResult showResult)
        {
           /* if (showResult == ShowResult.Finished)
            {
               
                if (surfacingId == RewardableVideoAdID)
                {
                    Advertisement.Load(RewardableVideoAdID);
                }
            }
            if (onCompleteAction != null)
            {
                onCompleteAction.Invoke(showResult == ShowResult.Finished);
                onCompleteAction = null;
            }*/


            // Define conditional logic for each ad completion status:
            if (showResult == ShowResult.Finished)
            {
               
                if (surfacingId == RewardableVideoAdID)
                {
                    // Optional actions to take when theAd Unit or legacy Placement becomes ready (for example, enable the rewarded ads button)
                    if(onCompleteAction!= null)
                    {
                        onCompleteAction.Invoke(true);
                        onCompleteAction = null;
                    }
                    Advertisement.Load(RewardableVideoAdID);
                }
            }
            else if (showResult == ShowResult.Skipped)
            {
                UIController.Instance.adsFailedPopup.ShowMe();
                AdsFailedPoupu.instance.message("Please watch the complete video to get rewarded");
                if (onCompleteAction != null)
                {
                    onCompleteAction.Invoke(false);
                    onCompleteAction = null;
                }
                // Debug.LogWarning("Please watch the complete video to get rewarded");//a pop up can be added here instead of this
            }
            else if (showResult == ShowResult.Failed)
            {
                UIController.Instance.adsFailedPopup.ShowMe();
                AdsFailedPoupu.instance.message("Try again later.");
                if (onCompleteAction != null)
                {
                    onCompleteAction.Invoke(false);
                    onCompleteAction = null;
                }
                //  GameController.Instance.alertWindow.ShowMessage("Try again later.");
            }
        }

        public void OnUnityAdsReady(string surfacingId)
        {
            // If the ready Ad Unit or legacy Placement is rewarded, show the ad:
            if (surfacingId == RewardableVideoAdID)
            {
                // Optional actions to take when theAd Unit or legacy Placement becomes ready (for example, enable the rewarded ads button)
            }
        }

        public void OnUnityAdsDidError(string message)
        {
            UIController.Instance.adsFailedPopup.ShowMe();
            AdsFailedPoupu.instance.message("No Ads to view currently check again in sometime");
            if (onCompleteAction != null)
            {
                onCompleteAction.Invoke(false);
                onCompleteAction = null;
            }
        }

        public void OnUnityAdsDidStart(string surfacingId)
        {
            // Optional actions to take when the end-users triggers an ad.
        }

        // When the object that subscribes to ad events is destroyed, remove the listener:
        protected override void OnDestroy()
        {
            Advertisement.RemoveListener(this);
        }
    }
#endif
}


