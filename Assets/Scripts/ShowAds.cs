using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;

public class ShowAds : MonoBehaviour
{
    private InterstitialAd Interstitial;
    private RewardedAd Rewarded;
    private RewardedInterstitialAd RewardedInterstitial;
    public void Start()
    {
        //AdMobManager.Instance.ShowBanner();
        
    }


    public void ShowInterstitial()
    {
        AdMobManager.Instance.ShowInterstitialAd();


    }

    public void ShowRewarded()
    {
        AdMobManager.Instance.ShowRewardedAd();
    }

    public void ShowRewardedinterstitial()
    {
        if (AdMobManager.Instance.NoAds == 0)
        {
            AdMobManager.Instance.ShowRewardedInterstitialAd();
        }
    }


    public void HideBanner()
    {
        AdMobManager.Instance.DestroyBannerView();
    }

    public void ShowBannerAD()
    {
        AdMobManager.Instance.ShowBanner();
    }

    public void AppOpenShow()
    {
        AdMobManager.Instance.ShowAppOpenAd();
    }

    
}
