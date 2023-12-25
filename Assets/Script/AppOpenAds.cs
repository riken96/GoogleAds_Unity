using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

public class AppOpenAds : MonoBehaviour
{
#if UNITY_ANDROID
    public string AppOpenID = "ca-app-pub-1407232796132402/4274834424";
#elif UNITY_IOS
    public string AppOpenID = "ca-app-pub-3940256099942544/5575463023";
#endif


    private AppOpenAd appOpenAd;

    public bool isShowingAd = false;


    private void Awake()
    {
        AppStateEventNotifier.AppStateChanged += OnAppStateChanged;
    }

    private void Start()
    {
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            // This callback is called once the MobileAds SDK is initialized.
        });

        RequestAppOpenAds();
    }

    private bool IsAdAvailable
    {
        get
        {
            return appOpenAd != null;
        }
    }

    public void ShowAppOpenAd()
    {
        if (IsAppOpenAdAvailable)
        {
            Debug.Log("Showing app open ad.");
            appOpenAd.Show();
        }
        else
        {
            Debug.LogError("App open ad is not ready yet.");
        }
    }
    private void OnAppStateChanged(AppState state)
    {
        Debug.Log("App State changed to : " + state);

        // if the app is Foregrounded and the ad is available, show it.
        if (state == AppState.Foreground)
        {
            if (IsAdAvailable)
            {
                ShowAppOpenAd();
            }
        }
    }
    public void RequestAppOpenAds()
    {
        Debug.Log("isShowingAd " + isShowingAd);

        if (IsAdAvailable && !isShowingAd)
        {
            ShowAppOpenAd();
            return;
        }

        LoadAppOpenAd();
    }

    public bool isFirstOpenAdd = true;
    public void LoadAppOpenAd()
    {
        // Clean up the old ad before loading a new one.
        if (appOpenAd != null)
        {
            appOpenAd.Destroy();
            appOpenAd = null;
        }

        // Create our request used to load the ad.
        var adRequest = new AdRequest();
        string adUnitId = AppOpenID;
        AppOpenAd.Load(adUnitId, adRequest,
          (AppOpenAd ad, LoadAdError error) =>
          {
              // if error is not null, the load request failed.
              if (error != null || ad == null)
              {
                  Debug.LogError("app open ad failed to load an ad " +
                                 "with error : " + error);
                  return;
              }

              Debug.LogError("App open ad loaded with response : "
                        + ad.GetResponseInfo());

              appOpenAd = ad;
              RegisterEventHandlers(ad);

              if (isFirstOpenAdd)
              {
                  ShowAppOpenAd();
                  isFirstOpenAdd = false;
              }
          });

    }

    System.DateTime _expireTime;
    public bool IsAppOpenAdAvailable
    {
        get
        {
            return appOpenAd != null
                   && appOpenAd.CanShowAd();
        }
    }

    private void RegisterEventHandlers(AppOpenAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(System.String.Format("App open ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("App open ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("App open ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("App open ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("App open ad full screen content closed.");
            LoadAppOpenAd();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("App open ad failed to open full screen content " +
                           "with error : " + error);
            LoadAppOpenAd();
        };
    }
}
