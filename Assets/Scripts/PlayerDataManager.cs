using UnityEngine;

public static class PlayerDataManager
{
    private const string CoinCountKey = "CoinCount";
    private const string SelectedSkinPriceKey = "SelectedSkinPrice";
    private const string PurchasedSkinKey = "PurchasedSkinID";
    private const string SelectedTrailPriceKey = "SelectedTrailPrice";
    private const string PurchasedTrailKey = "PurchasedTrailID";

    private static int coinCount;
    private static int selectedSkinPrice; // Price of the selected skin
    private static int purchasedSkinID; // ID of the purchased skin

    private static int selectedTrailPrice; // Price of the selected trail
    private static int purchasedTrailID; // ID of the purchased trail

    public static int CoinCount
    {
        get { return coinCount; }
        set { coinCount = value; }
    }

    public static int SelectedSkinPrice
    {
        get { return selectedSkinPrice; }
        set { selectedSkinPrice = value; }
    }

    public static int PurchasedSkinID
    {
        get { return purchasedSkinID; }
        set { purchasedSkinID = value; }
    }

    public static int SelectedTrailPrice
    {
        get { return selectedTrailPrice; }
        set { selectedTrailPrice = value; }
    }

    public static int PurchasedTrailID
    {
        get { return purchasedTrailID; }
        set { purchasedTrailID = value; }
    }

    public static void SavePlayerData()
    {
        PlayerPrefs.SetInt(CoinCountKey, coinCount);
        PlayerPrefs.SetInt(SelectedSkinPriceKey, selectedSkinPrice);
        PlayerPrefs.SetInt(PurchasedSkinKey, purchasedSkinID);
        PlayerPrefs.SetInt(SelectedTrailPriceKey, selectedTrailPrice);
        PlayerPrefs.SetInt(PurchasedTrailKey, purchasedTrailID);
        PlayerPrefs.Save();
    }

    public static void LoadPlayerData()
    {
        coinCount = PlayerPrefs.GetInt(CoinCountKey, 0);
        selectedSkinPrice = PlayerPrefs.GetInt(SelectedSkinPriceKey, 0);
        purchasedSkinID = PlayerPrefs.GetInt(PurchasedSkinKey, -1);

        selectedTrailPrice = PlayerPrefs.GetInt(SelectedTrailPriceKey, 0);
        purchasedTrailID = PlayerPrefs.GetInt(PurchasedTrailKey, -1);
    }
}
