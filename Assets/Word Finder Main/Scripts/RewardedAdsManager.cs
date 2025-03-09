using UnityEngine;

public class RewardedAdsManager : MonoBehaviour
{
    private string appKey = "85460dcd"; 

    public static RewardedAdsManager instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        IronSource.Agent.setMetaData("is_test_suite", "true");

        IronSource.Agent.init(appKey);
        Debug.Log("IronSource Initialized with App Key: " + appKey);
        IronSource.Agent.setAdaptersDebug(true);

        IronSource.Agent.loadRewardedVideo();
    }

    public void ShowRewardedAd()
    {
        if (IronSource.Agent.isRewardedVideoAvailable())
        {
            IronSource.Agent.showRewardedVideo();
        }
        else
        {
            Debug.Log("Ads is not ready yet, trying to load again...");
            IronSource.Agent.loadRewardedVideo();
        }
    }

    void OnEnable()
    {
        IronSourceEvents.onRewardedVideoAdRewardedEvent += OnRewarded;
        IronSourceEvents.onRewardedVideoAdClosedEvent += OnAdClosed;
    }

    void OnDisable()
    {
        IronSourceEvents.onRewardedVideoAdRewardedEvent -= OnRewarded;
        IronSourceEvents.onRewardedVideoAdClosedEvent -= OnAdClosed;
    }

    private void OnRewarded(IronSourcePlacement placement)
    {
        Debug.Log("Player has watch the full ads, request for the hint");
        LetterHintManager.instance.GiveLetterHint();
    }

    private void OnAdClosed()
    {
        Debug.Log("The ads is close, loading again.");
        IronSource.Agent.loadRewardedVideo();
    }
}
