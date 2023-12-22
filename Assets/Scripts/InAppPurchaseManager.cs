using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using System;

public class InAppPurchase : MonoBehaviour, IStoreListener
{
    private static IStoreController m_StoreController;

    private static IExtensionProvider m_StoreExtensionProvider;

    //public static string Get_400_Coin = "com.superstar.mazerescue.400coins";

    //public static string Get_1300_Coin = "com.superstar.mazerescue.1300coins";

    //public static string Get_5000_Coin = "com.superstar.mazerescue.5000coins";

    //public static string Get_15000_Coin = "com.superstar.mazerescue.15000coins";

    public static string PRODUCT_REMOVE_ADS = "com.admob.test.noads";

    private List<string> prices = new List<string>();

    public static InAppPurchase Instance { get; set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        if (m_StoreController == null)
        {
            InitializePurchasing();
        }
    }

    public void InitializePurchasing()
    {
        if (!IsInitialized())
        {
            ConfigurationBuilder configurationBuilder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            configurationBuilder.AddProduct(PRODUCT_REMOVE_ADS, ProductType.NonConsumable);
            //configurationBuilder.AddProduct(Get_400_Coin, ProductType.Consumable);
            //configurationBuilder.AddProduct(Get_1300_Coin, ProductType.Consumable);
            //configurationBuilder.AddProduct(Get_5000_Coin, ProductType.Consumable);
            //configurationBuilder.AddProduct(Get_15000_Coin, ProductType.Consumable);
            UnityPurchasing.Initialize(this, configurationBuilder);
        }
    }

    private bool IsInitialized()
    {
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }

   /* public void BuyCoin(int Coin)
    {
        switch (Coin)
        {
            case 0:
                Buy400Coin();
                break;
            case 1:
                Buy1300Coin();
                break;
            case 2:
                Buy5000Coin();
                break;
            case 3:
                Buy15000Coin();
                break;
        }
    }*/

    //public void Buy400Coin()
    //{
    //    BuyProductID(Get_400_Coin);
    //}

    //public void Buy1300Coin()
    //{
    //    BuyProductID(Get_1300_Coin);
    //}

    //public void Buy5000Coin()
    //{
    //    BuyProductID(Get_5000_Coin);
    //}

    //public void Buy15000Coin()
    //{
    //    BuyProductID(Get_15000_Coin);
    //}

    public void BuyRemoveAds()
    {
        BuyProductID(PRODUCT_REMOVE_ADS);
    }

    private void BuyProductID(string productId)
    {
        if (IsInitialized())
        {
            Product product = m_StoreController.products.WithID(productId);
            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                m_StoreController.InitiatePurchase(product);
                //ApplovinAdmanager.Instance.SetRemoveAds = 1;
                //ApplovinAdmanager.Instance.hideBanner();
            }
            else
            {
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        else
        {
            Debug.Log("BuyProductID FAIL. Not initialized.");
        }
    }

    public void RestorePurchases()
    {
        if (!IsInitialized())
        {
            Debug.Log("RestorePurchases FAIL. Not initialized.");
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer)
        {
            Debug.Log("RestorePurchases started ...");
            IAppleExtensions extension = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            extension.RestoreTransactions(delegate (bool result)
            {
                Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
            });
        }
        else
        {
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("OnInitialized: PASS");
        m_StoreController = controller;
        m_StoreExtensionProvider = extensions;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }


    //Auto Calling

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        //if (string.Equals(args.purchasedProduct.definition.id, Get_400_Coin, StringComparison.Ordinal))
        //{
        //    GameDataController.Instance.Addcoin(400);
        //}
        //if (string.Equals(args.purchasedProduct.definition.id, Get_1300_Coin, StringComparison.Ordinal))
        //{
        //    GameDataController.Instance.Addcoin(1300);
        //}
        //if (string.Equals(args.purchasedProduct.definition.id, Get_5000_Coin, StringComparison.Ordinal))
        //{
        //    GameDataController.Instance.Addcoin(5000);
        //}
        //if (string.Equals(args.purchasedProduct.definition.id, Get_15000_Coin, StringComparison.Ordinal))
        //{
        //    GameDataController.Instance.Addcoin(15000);
        //}
        if (string.Equals(args.purchasedProduct.definition.id, PRODUCT_REMOVE_ADS, StringComparison.Ordinal))
        {
            PlayerPrefs.SetInt("Noads", 1);
            AdMobManager.Instance.HideBanner();
            //UIManager.Instance.buttonNoads.interactable = false;
        }
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
    }

    public List<string> GetPrices()
    {
        Product[] all = m_StoreController.products.all;
        foreach (Product product in all)
        {
            prices.Add(product.metadata.localizedPriceString);
        }
        return prices;
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        throw new NotImplementedException();
    }
}
