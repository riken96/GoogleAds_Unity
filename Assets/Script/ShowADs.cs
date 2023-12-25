using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowADs : MonoBehaviour
{
   
    public void ShowBannerADs()
    {
        Admobmanager.instance.ShowBanner();
    }

    public void HideBanner()
    {
        Admobmanager.instance.DestroyBannerView();
    }

    public void ShowIntrestitialADs()
    {
        Admobmanager.instance.ShowInterstitialAd();
    }

    public void ShowRewardADs()
    {
        Admobmanager.instance.ShowRewardedAd();
    }

    public void ShowRewardIntrestital()
    {
        Admobmanager.instance.ShowRewardedInterstitialAd();
    }

}
