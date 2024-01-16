using UnityEngine;
using GoogleMobileAds.Api;
using System;
using System.Collections;
using UnityEngine.UI;
using System.Net;
using System.IO;
using UnityEngine.AI;

public class Admob : MonoBehaviour
{
    [Header("BANNER")]
    BannerView adBanner;
    [Header("interstitial")]
    public InterstitialAd interstitial;
    public RewardedAd rewardedAd;
    //bool TestMode = false;
    public bool TestMode = false;

#if UNITY_IPHONE || UNITY_IOS
    //IOS Rewarded
    const string RewardedAD_IOS_TEST_adUnitId = "ca-app-pub-3940256099942544/1712485313";
    const string RewardedAD_IOS_adUnitId = "ca-app-pub-4028019648626827/8697951787";
    // IOS Banner
    const string Banner_IOS_TEST_adUnitId = "ca-app-pub-3940256099942544/2934735716";
    const string Banner_IOS_adUnitId = "ca-app-pub-4028019648626827/1196179365";
    //Interstitial
    const string Interstitial_IOS_TEST_adUnitId = "ca-app-pub-3940256099942544/4411468910";
    const string Interstitial_IOS_adUnitId = "ca-app-pub-4028019648626827/5257826225";
#else
    //ANDRROID Rewarded
    const string RewardedAD_ANDROID_TEST_adUnitId = "ca-app-pub-3940256099942544/5224354917";
    const string RewardedAD_ANDROID_adUnitId = "ca-app-pub-4028019648626827/5153081809";
    //ANDRROID Banner
    const string Banner_ANDROID_TEST_adUnitId = "ca-app-pub-3940256099942544/6300978111";
    const string Banner_ANDROID_adUnitId = "ca-app-pub-4028019648626827/7423023472";
    //Interstitial
    const string Interstitial_TEST_ANDROID_adUnitId = "ca-app-pub-3940256099942544/5354046379";
    const string Interstitial_ANDROID_adUnitId = "ca-app-pub-4028019648626827/3673281816";
#endif


    void Start()
    {
        MobileAds.Initialize(initStatus => { });
    }

    

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~Banner~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public void RequestBannerAd(AdPosition adPosition)
    {
#if UNITY_ANDROID || UNITY_EDITOR
        if (Application.platform == RuntimePlatform.IPhonePlayer)
            return;

        if (adBanner != null) DestroyBannerAd();
        if (adBanner == null)
            {
                CreateBannerView(adPosition);
            }
            // create our request used to load the ad.
            var adRequest = new AdRequest();
            adRequest.Keywords.Add("unity-admob-sample");

            // send the request to load the ad.
            Debug.Log("Loading banner ad.");
            adBanner.LoadAd(adRequest);
#endif

    }
    void CreateBannerView(AdPosition adPosition)
    {
        int Ads_width = 320, Ads_Height = 50;
        //int start_X = Mathf.RoundToInt(0);
        //int start_Y = Mathf.RoundToInt( 50);
        AdSize adSize = new AdSize(Ads_width, Ads_Height);
        string adUnitId;
#if UNITY_ANDROID
        adUnitId = TestMode ? Banner_ANDROID_TEST_adUnitId : Banner_ANDROID_adUnitId;
#elif UNITY_IPHONE
        adUnitId = TestMode? Banner_IOS_TEST_adUnitId : Banner_IOS_adUnitId;
#else
        adUnitId = "unexpected_platform";
#endif
        adBanner = new BannerView(adUnitId, adSize, adPosition);
        //adBanner = new BannerView(adUnitId, adSize, start_X, start_Y);

        //Debug.Log($"Banner start_X: {adBanner.GetWidthInPixels()}, start_Y: {adBanner.GetHeightInPixels()}");

    }
    public void DestroyBannerAd()
    {
        if (adBanner != null)
        {
            adBanner.Destroy();
            adBanner = null;
        }
    }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~Interstital~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    public void RequestInterstitial()
    {
#if UNITY_ANDROID
        string adUnitId = Interstitial_ANDROID_adUnitId;
#elif UNITY_IPHONE
        string adUnitId = Interstitial_IOS_adUnitId;
#else
        string adUnitId = "unexpected_platform";
#endif
        if (interstitial != null && !interstitial.CanShowAd())
        {
            interstitial.Destroy();
            interstitial = null;
        }
        // create our request used to load the ad.
        var adRequest = new AdRequest();
        adRequest.Keywords.Add("unity-admob-sample");

        // send the request to load the ad.
        InterstitialAd.Load(adUnitId, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("interstitial ad failed to load an ad " + "with error : " + error);
                    return;
                }

                Debug.Log("Interstitial ad loaded with response : " + ad.GetResponseInfo());

                interstitial = ad;
            });
        interstitial.OnAdClicked += () =>
        {
            Time.timeScale = 1;
            RequestInterstitial();

        };
        // Raised when the ad failed to open full screen content.
        this.interstitial.OnAdFullScreenContentFailed += HandleOnAdFailedToLoad;
        // Raised when an ad opened full screen content.
        this.interstitial.OnAdFullScreenContentOpened += HandleOnAdOpened;
        // Raised when the ad closed full screen content.
        this.interstitial.OnAdFullScreenContentClosed += HandleOnAdClosed;
    }
    public void ShowInterstitial()
    {
         if (interstitial != null && interstitial.CanShowAd())
        {
            interstitial.Show();

        }
        else if (interstitial == null || !interstitial.CanShowAd())
        {
            RequestInterstitial();
            //Try to plat **REWARDED ADS
            if (rewardedAd != null && rewardedAd.CanShowAd()) 
            {
                ShowRewardAdsNow();//No need Check connection
            }
        }
    }
    public void HandleOnAdFailedToLoad(AdError error)
    {
        Time.timeScale = 1;
        RequestInterstitial();
    }
    void HandleOnAdLoaded()
    {
    }
    public void HandleOnAdOpened()
    {

    }

    public void HandleOnAdClosed()
    {
        interstitial.Destroy();

    }

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~Reward Ads~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public void CreateAndLoadRewardedAd()
    {
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }
#if UNITY_ANDROID
        string adUnitId = RewardedAD_ANDROID_adUnitId;
#elif UNITY_IPHONE
        string adUnitId = TestMode? RewardedAD_IOS_TEST_adUnitId : RewardedAD_IOS_adUnitId;
#else
        string adUnitId = "unexpected_platform";
#endif
        // create our request used to load the ad.
        var adRequest = new AdRequest();
        adRequest.Keywords.Add("unity-admob-sample");
        // send the request to load the ad.
        RewardedAd.Load(adUnitId, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("Rewarded ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }
                Debug.Log("Rewarded ad loaded with response : "
                          + ad.GetResponseInfo());

                rewardedAd = ad;
            });
        rewardedAd.OnAdFullScreenContentClosed += HandleRewardedAdClosed;
        rewardedAd.OnAdFullScreenContentFailed += HandleRewardedAdFailedToShow;
        rewardedAd.OnAdFullScreenContentOpened += HandleRewardedAdOpening;
    }
    public void HandleRewardedAdLoaded(object sender, EventArgs args)
    {
    }
    public void HandleRewardedAdFailedToLoad()
    {
        Request_RewardAd();
    }
    public void HandleRewardedAdOpening()
    {
    }
    public void HandleRewardedAdFailedToShow(AdError error)
    {

    }

    public void HandleRewardedAdClosed()// Cut video
    {
        Request_RewardAd();
    }
    public void HandleUserEarnedReward(Reward reward)
    {
        Request_RewardAd();
    }

    public void Request_RewardAd()
    {
            Debug.Log("Request Rewarded ad ");
        if (rewardedAd == null || !rewardedAd.CanShowAd())//Load rewardAD 1
        {
            CreateAndLoadRewardedAd();
        }
    }
    public void ReloadVideo()//inspector button
    {
        Show_Rewarded_Ad();
    }


    public void Show_Rewarded_Ad()
    {

        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            ShowRewardAdsNow();
        }
        else if (interstitial != null && interstitial.CanShowAd())
        {
            ShowInterstitial();
        }


    }
    void ShowRewardAdsNow()
    {
        if (rewardedAd != null && rewardedAd.CanShowAd())//try load reward 1
        {
            rewardedAd.Show((Reward reward) => { HandleUserEarnedReward(reward); });

        }

    }



}




