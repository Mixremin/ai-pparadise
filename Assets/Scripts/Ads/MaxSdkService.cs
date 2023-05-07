using System;
using UnityEngine;

public class MaxSdkService : MonoBehaviour
{
    [SerializeField] private string _sdkKey;
    [SerializeField] private string _bannerUnitId;
    [SerializeField] private string _interstitialUnitId;
    [SerializeField] private string _rewardedUnitId;

    [Space] [SerializeField] private InterstitialTimer _interstitialTimer;
    [SerializeField] private Color _bannerBackgroundColor;

    private Action _rewardAction;
    private static MaxSdkService _instance;

    public bool IsInterstitialReady => MaxSdk.IsInterstitialReady(_interstitialUnitId);
    public bool IsRewardedReady => MaxSdk.IsRewardedAdReady(_rewardedUnitId);

    public static MaxSdkService Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<MaxSdkService>();

                if (_instance == null)
                    Debug.LogError("No active MaxSdkService script found in scene.");
            }

            return _instance;
        }
    }

    private void Awake()
    {
        MaxSdkCallbacks.OnSdkInitializedEvent += _ =>
        {
            LoadAds();
            Debug.Log($"AppLovin SDK is initialized");
            MaxSdk.ShowMediationDebugger();
        };
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += GetReward;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedDisplayed;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedHidden;
        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayed;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHidden;

        MaxSdk.SetSdkKey(_sdkKey);
        MaxSdk.InitializeSdk();
    }

    private void OnDestroy()
    {
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent -= GetReward;
    }

    public void ShowInterstitial()
    {
        MaxSdk.ShowInterstitial(_interstitialUnitId);
        LoadInterstitial();
    }

    public void ShowRewarded(Action rewardAction)
    {
        _rewardAction = rewardAction;
        MaxSdk.ShowRewardedAd(_rewardedUnitId);
        LoadRewarded();
    }

    private void GetReward(string placement, MaxSdkBase.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        _rewardAction?.Invoke();
    }

    private void OnRewardedDisplayed(string placement, MaxSdkBase.AdInfo adInfo)
    {
        _interstitialTimer.Pause();
    }

    private void OnRewardedHidden(string placement, MaxSdkBase.AdInfo adInfo)
    {
        _interstitialTimer.ResetTimer();
    }

    private void OnInterstitialDisplayed(string placement, MaxSdkBase.AdInfo adInfo)
    {
        _interstitialTimer.Pause();
    }

    private void OnInterstitialHidden(string placement, MaxSdkBase.AdInfo adInfo)
    {
        _interstitialTimer.ResetTimer();
    }

    private void LoadAds()
    {
        CreateBanner();
        LoadInterstitial();
        LoadRewarded();
    }

    private void LoadRewarded()
    {
        MaxSdk.LoadRewardedAd(_rewardedUnitId);
    }

    private void LoadInterstitial()
    {
        MaxSdk.LoadInterstitial(_interstitialUnitId);
    }

    private void CreateBanner()
    {
        MaxSdk.CreateBanner(_bannerUnitId, MaxSdkBase.BannerPosition.BottomCenter);
        MaxSdk.SetBannerBackgroundColor(_bannerUnitId, _bannerBackgroundColor);
        MaxSdk.ShowBanner(_bannerUnitId);
    }
}