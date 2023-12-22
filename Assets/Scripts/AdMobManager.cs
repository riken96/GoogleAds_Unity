using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;
using System;
using GoogleMobileAds.Common;
using GoogleMobileAds.Ump.Api;

public class AdMobManager : MonoBehaviour   
{
    public static AdMobManager Instance;

    public int NoAds
    {
        get
        {
            return PlayerPrefs.GetInt("NoAds", 0);
        }
        set
        {
            PlayerPrefs.SetInt("NoAds", value);
        }
    }

    internal static List<string> TestDeviceIds = new List<string>()
        {
            AdRequest.TestDeviceSimulator,
#if UNITY_IPHONE
            "96e23e80653bb28980d3f40beb58915c",
#elif UNITY_ANDROID
            "702815ACFC14FF222DA1DC767672A573"
#endif
        };

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

       // AppStateEventNotifier.AppStateChanged += OnAppStateChanged;
    }

    public void Start()
    {
        // Initialize the Google Mobile Ads SDK.

        MobileAds.RaiseAdEventsOnUnityMainThread = true;

        MobileAds.SetRequestConfiguration(new RequestConfiguration
        {
            TestDeviceIds = TestDeviceIds
        });


        MobileAds.Initialize(initStatus => {
            ShowBanner();
            LoadInterstitialAd();
            LoadRewardedAd();
            LoadRewardedInterstitialAd();
            //LoadAppOpenAd();
        });
        
    }

    //  Banner Ads

#if UNITY_ANDROID
    public  string _adUnitBANId = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IPHONE
  private string _adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
  private string _adUnitId = "unused";
#endif

   BannerView _bannerView;

    /// <summary>
    /// Creates a 320x50 banner view at top of the screen.
    /// </summary>
    public void CreateBannerView()
    {
        Debug.Log("Creating banner view");

        // If we already have a banner, destroy the old one.
        if (_bannerView != null)
        {
            DestroyBannerView();
        }

        // Create a 320x50 banner at top of the screen
        _bannerView = new BannerView(_adUnitBANId, AdSize.Banner, AdPosition.Top);

        // Use the AdSize argument to set a custom size for the ad.
        AdSize adSize = new AdSize(320, 50);
        _bannerView = new BannerView(_adUnitBANId, adSize, AdPosition.Bottom);
    }

    public void DestroyBannerView()
    {
        if (_bannerView != null)
        {
            Debug.Log("Destroying banner view.");
            _bannerView.Destroy();
            _bannerView = null;
        }
    }

    public void HideBanner()
    {
        DestroyBannerView();
    }

    /// <summary>
    /// Creates the banner view and loads a banner ad.
    /// </summary>
    public void LoadBannerAd()
    {
        // create an instance of a banner view first.
        if (_bannerView == null)
        {
            CreateBannerView();
        }

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        Debug.Log("Loading banner ad.");
        _bannerView.LoadAd(adRequest);
    }

    public void ShowBanner()
    {
        LoadBannerAd();
    }




                         // Interstitial ads
#if UNITY_ANDROID
    public string _adUnitINId = "ca-app-pub-3940256099942544/1033173712";
#elif UNITY_IPHONE
  private string _adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
  private string _adUnitId = "unused";
#endif

    private InterstitialAd _interstitialAd;

  /// <summary>
  /// Loads the interstitial ad.
  /// </summary>
  public void LoadInterstitialAd()
    {
        // Clean up the old ad before loading a new one.
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }

        Debug.Log("Loading the interstitial ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        InterstitialAd.Load(_adUnitINId, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
              // if error is not null, the load request failed.
              if (error != null || ad == null)
                {
                    Debug.LogError("interstitial ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("Interstitial ad loaded with response : "
                          + ad.GetResponseInfo());

                _interstitialAd = ad;
            });
    }

    public void ShowInterstitialAd()
    {
        if (_interstitialAd != null && _interstitialAd.CanShowAd())
        {
            Debug.Log("Showing interstitial ad.");
            _interstitialAd.Show();
        }
        else
        {
            Debug.LogError("Interstitial ad is not ready yet.");
        }
    }


                                                                                 // Rewarded ads
#if UNITY_ANDROID
    public string _adUnitREId = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IPHONE
  private string _adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
  private string _adUnitId = "unused";
#endif

    private RewardedAd _rewardedAd;

    /// <summary>
    /// Loads the rewarded ad.
    /// </summary>
    public void LoadRewardedAd()
    {
        // Clean up the old ad before loading a new one.
        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }

        Debug.Log("Loading the rewarded ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        RewardedAd.Load(_adUnitREId, adRequest,
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

                _rewardedAd = ad;
            });
    }


    public void ShowRewardedAd()
    {
        const string rewardMsg =
            "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";

        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            _rewardedAd.Show((Reward reward) =>
            {
                // TODO: Reward the user.
                Debug.Log(String.Format(rewardMsg, reward.Type, reward.Amount));
            });
        }
    }

    private void RegisterEventHandlers(RewardedAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Rewarded ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Rewarded ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded ad full screen content closed.");
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open full screen content " +
                           "with error : " + error);
        };
    }

    private void RegisterReloadHandler(RewardedAd ad)
    {
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
    {
            Debug.Log("Rewarded Ad full screen content closed.");

            // Reload the ad so that we can show another as soon as possible.
            LoadRewardedAd();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open full screen content " +
                           "with error : " + error);

            // Reload the ad so that we can show another as soon as possible.
            LoadRewardedAd();
        };
    }


                                                                                  // Rewarded interstitial ads 

#if UNITY_ANDROID
    public string _adUnitREINId = "ca-app-pub-3940256099942544/5354046379";
#elif UNITY_IPHONE
  private string _adUnitId = "ca-app-pub-3940256099942544/6978759866";
#else
  private string _adUnitId = "unused";
#endif

    private RewardedInterstitialAd _rewardedInterstitialAd;

    /// <summary>
    /// Loads the rewarded interstitial ad.
    /// </summary>
    public void LoadRewardedInterstitialAd()
    {
        // Clean up the old ad before loading a new one.
        if (_rewardedInterstitialAd != null)
        {
            _rewardedInterstitialAd.Destroy();
            _rewardedInterstitialAd = null;
        }

        Debug.Log("Loading the rewarded interstitial ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();
        adRequest.Keywords.Add("unity-admob-sample");

        // send the request to load the ad.
        RewardedInterstitialAd.Load(_adUnitREINId, adRequest,
            (RewardedInterstitialAd ad, LoadAdError error) =>
            {
              // if error is not null, the load request failed.
              if (error != null || ad == null)
                {
                    Debug.LogError("rewarded interstitial ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("Rewarded interstitial ad loaded with response : "
                          + ad.GetResponseInfo());

                _rewardedInterstitialAd = ad;
            });
    }


    public void ShowRewardedInterstitialAd()
    {
        const string rewardMsg =
            "Rewarded interstitial ad rewarded the user. Type: {0}, amount: {1}.";

        if (_rewardedInterstitialAd != null && _rewardedInterstitialAd.CanShowAd())
        {
            _rewardedInterstitialAd.Show((Reward reward) =>
            {
                // TODO: Reward the user.
                Debug.Log(String.Format(rewardMsg, reward.Type, reward.Amount));
            });
        }
    }

    private void RegisterEventHandlers(RewardedInterstitialAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Rewarded interstitial ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Rewarded interstitial ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded interstitial ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded interstitial ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded interstitial ad full screen content closed.");
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded interstitial ad failed to open " +
                           "full screen content with error : " + error);
        };
    }

    private void RegisterReloadHandler(RewardedInterstitialAd ad)
    {
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += ()=>
    {
            Debug.Log("Rewarded interstitial ad full screen content closed.");

            // Reload the ad so that we can show another as soon as possible.
            LoadRewardedInterstitialAd();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded interstitial ad failed to open " +
                           "full screen content with error : " + error);

            // Reload the ad so that we can show another as soon as possible.
            LoadRewardedInterstitialAd();
        };
    }

    private void contentclosedEvent()
    {
        throw new NotImplementedException();
    }


                                                                                           // APP Open Add..........
#if UNITY_ANDROID
    public string _adUnitAPPOId = "ca-app-pub-3940256099942544/9257395921";
#elif UNITY_IPHONE
   string _adUnitId = "ca-app-pub-3940256099942544/5575463023";
#else
  private string _adUnitId = "unused";
#endif

    private AppOpenAd appOpenAd;
    private DateTime _expireTime;
    private object _appOpenAd;

    /// <summary>
    /// Loads the app open ad.
    /// </summary>
    public void LoadAppOpenAd()
    {
        // Clean up the old ad before loading a new one.
        if (appOpenAd != null)
        {
            appOpenAd.Destroy();
            appOpenAd = null;
        }

        Debug.Log("Loading the app open ad.");

        // Create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        AppOpenAd.Load(_adUnitAPPOId, adRequest,
            (AppOpenAd ad, LoadAdError error) =>
            {
              // if error is not null, the load request failed.
              if (error != null || ad == null)
                {
                    Debug.LogError("app open ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("App open ad loaded with response : "
                          + ad.GetResponseInfo());

                appOpenAd = ad;
                RegisterEventHandlers(ad);
            });
    }

    private void RegisterEventHandlers(AppOpenAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("App open ad paid {0} {1}.",
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
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("App open ad failed to open full screen content " +
                           "with error : " + error);
        };
    }


    public void ShowAppOpenAd()
    {
        if (appOpenAd != null && appOpenAd.CanShowAd())
        {
            Debug.Log("Showing app open ad.");
            appOpenAd.Show();
        }
        else
        {
            Debug.LogError("App open ad is not ready yet.");
        }
    }

    private bool IsAdAvailable
    {
        get
        {
            return appOpenAd != null;
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

     public bool IsAppOpenAdAvailable
    {
        get
        {
            return appOpenAd != null
                   && appOpenAd.CanShowAd()
                   && DateTime.Now < _expireTime;
        }
    }



}
