using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Debug_Manager : MonoBehaviour
{

    public Text info_Text_Board;
    int pressTime = 0;
    
    public Admob Admob;
    void Start()
    {

    }
    public void SwitchAdsSupplier(bool on)
    {
        info_Text_Board.text = "SwitchAdsSupplier: " + (on?"UnityAds":"Admob");
    }
    public void AdmobTestModeOn(bool on)
    {
        Admob.TestMode = on;
        info_Text_Board.text = "Admob Test Mode " + on;
    }


    public void AdMob_Initialize_info()
    {
        StopAllCoroutines();
        StartCoroutine(AdMob_Status());
    }
    IEnumerator AdMob_Status()
    {
        info_Text_Board.text = "Checking\n";
        if(Admob.interstitial != null)
            info_Text_Board.text += "LOAD interstitial ADS : " + Admob.interstitial.CanShowAd().ToString() + "\n";
        if(Admob.rewardedAd != null)
            info_Text_Board.text += "LOAD Regarded ADS: " + Admob.rewardedAd.CanShowAd().ToString() + "\n";

        yield return new WaitForSeconds(10f);
        info_Text_Board.text = "";

    }
    public void Click_Load_AD()
    {
        StopAllCoroutines();
        StartCoroutine(CheckSpeed_LoadAds());
        Admob.Request_RewardAd();
        Admob.RequestInterstitial();

    }
    public void Click_Play_Interstitial()
    {
        Admob.ShowInterstitial();
    }
    public void Click_Play_RewardAD()
    {
        Admob.Show_Rewarded_Ad();

    }

    IEnumerator CheckSpeed_LoadAds()
    {
        float startTime = Time.timeSinceLevelLoad;
        float speed = 0;
        info_Text_Board.text = "TestMode: " + Admob.TestMode + "\n";
        info_Text_Board.text += "Start Loading ADS\n";

        while ((Admob.interstitial != null && !Admob.interstitial.CanShowAd()) ||
        (Admob.rewardedAd != null && !Admob.rewardedAd.CanShowAd()))
        {
            speed = Time.timeSinceLevelLoad - startTime;
            info_Text_Board.text = "Loading ADS";
            yield return new WaitForSeconds(0.5f);
            info_Text_Board.text += ".";
            yield return new WaitForSeconds(0.5f);
            info_Text_Board.text += ".";
            yield return new WaitForSeconds(0.5f);
            info_Text_Board.text += ".";
            yield return null;
        }
        info_Text_Board.text += "LOAD ADS done, Load time =" + speed.ToString("00") + " sec\n";
        yield return new WaitForSeconds(10f);
        info_Text_Board.text = "";

    }
    public void Click_CheckDate()
    {
        info_Text_Board.text = "";
        info_Text_Board.text += "Now date time: " + DateTime.Now + "\n";
        info_Text_Board.text += "Now date time ticks: " + DateTime.Now.Ticks + "\n";
        DateTime Gift_Expire_Date;
#if UNITY_IPHONE || UNITY_IOS || UNITY_EDITOR
        Gift_Expire_Date = DateTime.Parse("27/12/2020");//dd/mm/yyyy
#else
        Gift_Expire_Date = DateTime.Parse("12/27/2020");//mm/dd/yyyy

#endif
        //if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WindowsEditor)
        //{
        //    Gift_Expire_Date = DateTime.Parse("12/27/2020");//mm/dd/yyyy

        //}
        //else//IOS
        //{
        //    Gift_Expire_Date = DateTime.Parse("27/12/2020");//dd/mm/yyyy

        //} 
        info_Text_Board.text += "Expire_Date: " + Gift_Expire_Date + "\n";
        info_Text_Board.text += "Expire_Date ticks: " + (ulong)Gift_Expire_Date.Ticks + "\n";
        if ((ulong)DateTime.Now.Ticks < (ulong)Gift_Expire_Date.Ticks)
        {
            info_Text_Board.text += "Gift ready now!!" + "\n";
        }
        else
        {
            info_Text_Board.text += "Gift time is Expired!!" + "\n";
        }
    }
}
