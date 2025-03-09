using UnityEngine;
using System.Collections;
using com.unity3d.mediation;
using UnityEngine.UI;
using UnityEngine.Purchasing;

public class BannerAdsManager : MonoBehaviour
{
    public static BannerAdsManager instance;
    public Text priceText;

    [SerializeField] private string appKey = "demoAppKey";
    [SerializeField] private string bannerAdUnitId = "defaultBannerAdUnitId";
    [SerializeField] private GameObject removeAdsButton;
    [SerializeField] private GameObject fakeBanner;

    private bool adsRemoved = false;
    private LevelPlayBannerAd bannerAd;

    private void Awake()
    {
        IronSource.Agent.setMetaData("is_test_suite", "true");
        IronSource.Agent.setAdaptersDebug(true);
        instance = this;
        InitializeAds();

        adsRemoved = PlayerPrefs.GetInt("AdsRemoved", 0) == 1;
        if (adsRemoved)
        {
            Debug.Log("User already removed ads. Hiding ads.");
            RemoveAds();
        }
        else
        {
            StartCoroutine(CheckForPreviousPurchase()); 
        }
    }

    private void Start()
    {
      
        if (CodelessIAPStoreListener.Instance == null ||
            CodelessIAPStoreListener.Instance.StoreController == null)
        {
            Debug.LogError("IAP StoreController is not initialized yet!");
            priceText.text = "N/A";
            return;
        }

        var product = CodelessIAPStoreListener.Instance.StoreController.products.WithID("com.OutbreakCompany.WutWord.RemoveAds");

        if (product != null && product.metadata != null)
        {
            priceText.text = product.metadata.localizedPriceString;
        }
        else
        {
            Debug.LogError("Product not found in StoreController.");
            priceText.text = "N/A";
        }

        if (removeAdsButton != null)
            removeAdsButton.SetActive(!adsRemoved);
    }

    private IEnumerator CheckForPreviousPurchase()
    {
        yield return new WaitForSeconds(1f); 

        if (CodelessIAPStoreListener.Instance != null &&
            CodelessIAPStoreListener.Instance.StoreController != null)
        {
            var product = CodelessIAPStoreListener.Instance.StoreController.products.WithID("com.OutbreakCompany.WutWord.RemoveAds");

            if (product != null && product.hasReceipt)  
            {
                Debug.Log("Restoring previous purchase: Remove Ads.");
                RemoveAds();
            }
        }
    }

    void InitializeAds()
    {
        IronSource.Agent.init(appKey);
        Debug.Log("IronSource Initialized with App Key: " + appKey);

        bannerAd = new LevelPlayBannerAd(
            bannerAdUnitId,
            LevelPlayAdSize.BANNER,
            LevelPlayBannerPosition.BottomCenter,
            "DefaultPlacement",
            false
        );

        bannerAd.OnAdLoaded += ShowBannerAd;
        bannerAd.OnAdLoadFailed += OnBannerLoadFailed;

        bannerAd.LoadAd();
        Debug.Log("Banner Ad Requested with ID: " + bannerAdUnitId);
    }

    private void ShowBannerAd(LevelPlayAdInfo adInfo)
    {
        Debug.Log("Banner ad successfully loaded.");
        bannerAd.ShowAd();
    }

    private void OnBannerLoadFailed(LevelPlayAdError error)
    {
        Debug.LogError("Failed to load banner ad: " + error.ToString());
    }

    public void ShowBannerAds()
    {
        if (adsRemoved)
        {
            fakeBanner.SetActive(false);
            Debug.Log("Ads are already removed. No need to show banners.");
            return;
        }

        bannerAd.ShowAd();
        Debug.Log("Banner ad displayed.");
    }

    public void RemoveAds()
    {
        StartCoroutine(RemoveAdsCoroutine());
    }

    private IEnumerator RemoveAdsCoroutine()
    {
        if (adsRemoved)
        {
            fakeBanner.SetActive(false);
            Debug.Log("Ads are already removed. Skipping redundant removal.");
            yield break;
        }

        yield return new WaitForEndOfFrame();
        fakeBanner.SetActive(false);
        Debug.Log("Removing ads...");
        adsRemoved = true;
        PlayerPrefs.SetInt("AdsRemoved", 1);
        PlayerPrefs.Save();

        bannerAd.HideAd();
        Debug.Log("Ads should now be hidden!");
    }

    public void OnPurchaseComplete(Product product)
    {
        if (product.definition.id == "com.OutbreakCompany.WutWord.RemoveAds")
        {
            Debug.Log("Purchase successful: Remove Ads!");
            RemoveAds();
        }
    }
}
