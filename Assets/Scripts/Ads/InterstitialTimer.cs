using UnityEngine;

public class InterstitialTimer : MonoBehaviour
{
    [SerializeField] private float _timeBetweenAds;
    private float _timeBeforeAd;
    
    public bool Paused { get; private set; }

    private void Awake()
    {
        ResetTimer();
    }

    private void Update()
    {
        if (Paused)
            return;
        
        _timeBeforeAd -= Time.deltaTime;
        if (_timeBeforeAd <= 0f && MaxSdkService.Instance.IsInterstitialReady)
            MaxSdkService.Instance.ShowInterstitial();
    }

    public void ResetTimer()
    {
        _timeBeforeAd = _timeBetweenAds;
        Unpause();
    }

    public void Pause()
    {
        Paused = true;
    }

    public void Unpause()
    {
        Paused = false;
    }
}