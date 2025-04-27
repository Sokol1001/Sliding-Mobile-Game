using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SceneManagement : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] private Animator anim;
    [SerializeField] private Player player;
    [SerializeField] private SlideSpawner slideSpawner;
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject loseCanvas;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private GameObject gameCanvas;
    public AudioSource canvasSource;
    public AudioClip buttonClicked;
    public Button buyButton;
    public Button buyButtonTrail;

    [SerializeField] RewardedAdsButton rab;
    [SerializeField] InterstitialAdsButton IAB;

    [Header("Skins")]
    private GameObject currentIcon;
    private int id;
    public GameObject[] UIIcons; // Array of UI icons for different skins
    public Sprite[] greenUIIcons;
    public Sprite[] defaultUIIcons;
    public GameObject[] icons;
    public GameObject[] podium;
    public GameObject[] playerSkins;
    public GameObject[] defeatedSkins;
    private bool[] purchasedSkins; // Array to store which skins have been purchased
    private int[] skinPrices = { 0, 1000, 1500, 2000, 2500, 3000, 7000 }; // Prices for each skin
    private int currentPrice = 0; // Current price selected by the player

    [Header("Trails")]
    private GameObject currentTrailIcon;
    private int idTrail;
    public GameObject[] UIIconsTrails; // Array of UI icons for different trails
    public Sprite[] greenUIIconsTrails;
    public Sprite[] defaultUIIconsTrails;
    public GameObject[] trailIcons;
    public GameObject[] trailPlayerSkins;
    private bool[] purchasedTrail; // Array to store which skins have been purchased
    private int[] trailPrices = { 0, 200, 300, 400, 500, 600, 700, 800 }; // Prices for each skin
    private int currentPriceTrail = 0; // Current price selected by the player

    private int playerCoins;
    public float rotationSpeed = 1f; // Adjust as needed

    // Static variable to store the restarting state
    private static bool isRestarting = false;
    // Enum to represent the button states
    public enum SceneManagementButtonState
    {
        Buy,
        NotEnoughMoney,
        Set,
        AlreadySet // Renamed from "SkinAlreadySet"
    }
    private void Awake()
    {
        Application.targetFrameRate = 60;
        gameCanvas.SetActive(false);

        PlayerDataManager.LoadPlayerData();
        LoadPlayerData();
        UpdatePriceText();
        purchasedSkins = new bool[skinPrices.Length]; // Initialize the array
        for (int i = 0; i < skinPrices.Length; i++)
        {
            purchasedSkins[i] = PlayerPrefs.GetInt("Skin" + i, 0) == 1;
        }

        purchasedTrail = new bool[trailPrices.Length]; // Initialize the array
        for (int i = 0; i < trailPrices.Length; i++)
        {
            purchasedTrail[i] = PlayerPrefs.GetInt("Trail" + i, 0) == 1;
        }

        // Check if this is the initial scene load or a restart
        if (!IsRestarting())
        {
            // If not restarting, activate the menu canvas
            mainMenuCanvas.SetActive(true);
            player.getCanMovePlayer(false);
            gameCanvas.SetActive(false);
            slideSpawner.getSceneManagmentData(false, 0, 0, 0);
            slideSpawner.GetBoolSpawnSlide(false);
        }
        else
        {
            // If restarting, activate the gameplay canvas
            mainMenuCanvas.SetActive(false);
            player.getCanMovePlayer(true);
            gameCanvas.SetActive(true);
            slideSpawner.GetBoolSpawnSlide(true);
            slideSpawner.getSceneManagmentData(true, 27, 1, 55);
        }
    }


    private void Start()
    {
        rab.LoadAd();
        UpdatePriceText();
        currentIcon = UIIcons[0];
        currentTrailIcon = UIIconsTrails[0];
        loseCanvas.SetActive(false);

        // Activate the purchased skin
        int purchasedSkinID = PlayerDataManager.PurchasedSkinID;
        if (purchasedSkinID != -1)
        {
            ActivateSkin(purchasedSkinID);
        }

        // Activate the purchased skin
        int purchasedTrailID = PlayerDataManager.PurchasedTrailID;
        if (purchasedTrailID != -1)
        {
            //Debug.Log("Activating trail");
            ActivateTrail(purchasedTrailID);
        }

    }

    void ActivateSkin(int skinID)
    {
        foreach (GameObject skin in podium)
        {
            skin.SetActive(false);
        }
        foreach (GameObject skin in playerSkins)
        {
            skin.SetActive(false);
        }
        foreach (GameObject skin in defeatedSkins)
        {
            skin.SetActive(false);
        }
        for(int i = 0; i < icons.Length; i++)
        {
            icons[i].GetComponent<RawImage>().texture = defaultUIIcons[i].texture;
        }
        id = skinID;
        // Activate the child object associated with the purchased skin
        podium[skinID].SetActive(true);
        playerSkins[skinID].SetActive(true);
        defeatedSkins[skinID].SetActive(true);
        icons[skinID].GetComponent<RawImage>().texture = greenUIIcons[skinID].texture;
    }
    void ActivateTrail(int trailID)
    {
        foreach (GameObject trail in trailPlayerSkins)
        {
            trail.SetActive(false);
        }
        for (int i = 0; i < UIIconsTrails.Length; i++)
        {
            trailIcons[i].GetComponent<RawImage>().texture = defaultUIIconsTrails[i].texture;
        }
        idTrail = trailID;
        // Activate the child object associated with the purchased trail
        trailPlayerSkins[trailID].SetActive(true);
        trailIcons[trailID].GetComponent<RawImage>().texture = greenUIIconsTrails[trailID].texture;
    }
    void UpdatePriceText()
    {
        // Check if a skin or trail is selected
        if (currentPrice > 0)
        {
            priceText.text = currentPrice.ToString() + " <sprite name=\"coin_0\">"; // Update text with coin icon
        }
        else if (currentPriceTrail > 0)
        {
            priceText.text = currentPriceTrail.ToString() +" <sprite name=\"coin_0\">"; // Update text with coin icon for trail price
        }
        else
        {
            priceText.text = ""; // Clear the text if nothing is selected
        }
    }


    public void IconClick(int price)
    {
        canvasSource.PlayOneShot(buttonClicked);

        currentPrice = price;
        UpdatePriceText();

        // Hide all UI icons
        foreach (GameObject icon in UIIcons)
        {
            icon.SetActive(false);
        }

        // Show the selected UI icon
        int skinID = GetSkinIDByPrice(price);
        if (skinID != -1)
        {
            UIIcons[skinID].SetActive(true);
        }

        bool isPurchased = purchasedSkins[skinID];
        bool isActive = PlayerDataManager.PurchasedSkinID == skinID;

        if (PlayerDataManager.CoinCount >= currentPrice && !isPurchased)
        {
            SetButtonState(SceneManagementButtonState.Buy);
        }
        else if (isPurchased && !isActive)
        {
            SetButtonState(SceneManagementButtonState.Set);
            GameObject padlock = GameObject.FindGameObjectWithTag("Padlock");
            padlock.GetComponent<Image>().enabled = false;
        }
        else if (isActive)
        {
            SetButtonState(SceneManagementButtonState.AlreadySet);
            GameObject padlock = GameObject.FindGameObjectWithTag("Padlock");
            padlock.GetComponent<Image>().enabled = false;
        }
        else
        {
            SetButtonState(SceneManagementButtonState.NotEnoughMoney);
        }
    }
    public void IconClickTrail(int price)
    {
        canvasSource.PlayOneShot(buttonClicked);

        currentPriceTrail = price;
        UpdatePriceText();

        // Hide all UI icons
        foreach (GameObject icon in UIIconsTrails)
        {
            icon.SetActive(false);
        }

        // Show the selected UI icon
        int trailID = GetTrailIDByPrice(price);
        if (trailID != -1)
        {
            UIIconsTrails[trailID].SetActive(true);
        }

        bool isPurchased = purchasedTrail[trailID];
        bool isActive = PlayerDataManager.PurchasedTrailID == trailID;

        if (PlayerDataManager.CoinCount >= currentPrice && !isPurchased)
        {
            SetButtonStateTrail(SceneManagementButtonState.Buy);
        }
        else if (isPurchased && !isActive)
        {
            SetButtonStateTrail(SceneManagementButtonState.Set);
            GameObject padlock = GameObject.FindGameObjectWithTag("Padlock");
            padlock.GetComponent<Image>().enabled = false;
        }
        else if (isActive)
        {
            SetButtonStateTrail(SceneManagementButtonState.AlreadySet);
            GameObject padlock = GameObject.FindGameObjectWithTag("Padlock");
            padlock.GetComponent<Image>().enabled = false;
        }
        else
        {
            SetButtonStateTrail(SceneManagementButtonState.NotEnoughMoney);
        }
    }
    public void BuyOrSetSkin()
    {
        canvasSource.PlayOneShot(buttonClicked);

        int skinID = GetSkinIDByPrice(currentPrice);
        if (purchasedSkins[skinID])
        {
            // If the skin is already purchased, set it
            SetSkin();
            SetButtonState(SceneManagementButtonState.AlreadySet);
        }
        else if (PlayerDataManager.CoinCount >= currentPrice)
        {
            // If the player has enough coins, buy the skin
            PlayerDataManager.CoinCount -= currentPrice; // Deduct coins
            purchasedSkins[skinID] = true; // Mark skin as purchased
            PlayerPrefs.SetInt("Skin" + skinID, 1); // Save purchase to PlayerPrefs
            PlayerDataManager.SavePlayerData(); // Save changes

            UpdatePriceText();
            SetButtonState(SceneManagementButtonState.Set); // Update button state to Set

            GameObject padlock = GameObject.FindGameObjectWithTag("Padlock");
            padlock.GetComponent<Image>().enabled = false;

            // Find the "Skin affect" GameObject by its tag and play its ParticleSystem
            GameObject skinAffect = GameObject.FindGameObjectWithTag("SkinAffect");
            if (skinAffect != null)
            {
                ParticleSystem particleSystem = skinAffect.GetComponent<ParticleSystem>();
                particleSystem.Play();
            }
            else
            {
                Debug.LogError("Skin affect not found");
            }
        }
    }
    public void BuyOrSetTrail()
    {
        canvasSource.PlayOneShot(buttonClicked);

        int trailID = GetTrailIDByPrice(currentPriceTrail);
        if (purchasedTrail[trailID])
        {
            // If the skin is already purchased, set it
            SetTrail();
            SetButtonStateTrail(SceneManagementButtonState.AlreadySet);
        }
        else if (PlayerDataManager.CoinCount >= currentPrice)
        {
            // If the player has enough coins, buy the trail
            PlayerDataManager.CoinCount -= currentPrice; // Deduct coins
            purchasedTrail[trailID] = true; // Mark trail as purchased
            PlayerPrefs.SetInt("Trail" + trailID, 1); // Save purchase to PlayerPrefs
            PlayerDataManager.SavePlayerData(); // Save changes

            UpdatePriceText();
            SetButtonStateTrail(SceneManagementButtonState.Set); // Update button state to Set

            GameObject padlock = GameObject.FindGameObjectWithTag("Padlock");
            padlock.GetComponent<Image>().enabled = false;

            // Find the "Skin affect" GameObject by its tag and play its ParticleSystem
            GameObject skinAffect = GameObject.FindGameObjectWithTag("SkinAffect");
            if (skinAffect != null)
            {
                ParticleSystem particleSystem = skinAffect.GetComponent<ParticleSystem>();
                particleSystem.Play();
            }
            else
            {
                Debug.LogError("Skin affect not found");
            }
        }
    }
    public void SetTrail()
    {
        int trailID = GetTrailIDByPrice(currentPriceTrail);
        if (purchasedTrail[trailID])
        {
            PlayerDataManager.PurchasedTrailID = trailID; // Update purchased trail ID
            PlayerDataManager.SavePlayerData(); // Save changes

            ActivateTrail(trailID); // Activate the purchased trail
        }
    }
    public void SetSkin()
    {
        int skinID = GetSkinIDByPrice(currentPrice);
        if (purchasedSkins[skinID])
        {
            PlayerDataManager.PurchasedSkinID = skinID; // Update purchased skin ID
            PlayerDataManager.SavePlayerData(); // Save changes

            ActivateSkin(skinID); // Activate the purchased skin
        }
    }
    void LoadPlayerData()
    {
        // Load player coins from PlayerPrefs
        playerCoins = PlayerPrefs.GetInt("CoinCount", 0);
    }

    void SavePlayerData()
    {
        // Save updated player coins to PlayerPrefs
        PlayerPrefs.SetInt("CoinCount", playerCoins);
        // Save the purchased skin or set skin to PlayerPrefs
        PlayerPrefs.SetInt("SelectedSkinPrice", currentPrice);
        PlayerPrefs.SetInt("SelectedTrailPrice", currentPriceTrail);
    }

    public void RestartGame(bool restart)
    {
        System.Random random = new System.Random();
        int randomNumber = random.Next(2); // Generates 0 or 1

        if (randomNumber == 0)
        {
            Time.timeScale = 0;
            IAB.ShowAd();
        }
        SetRestartingFlag(restart);
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void StartGame()
    {
        System.Random random = new System.Random();
        int randomNumber = random.Next(2); // Generates 0 or 1

        if (randomNumber == 0)
        {
            Time.timeScale = 0;
            IAB.ShowAd();
        }

        RestartGame(false);
    }
    public void StartGameMainMenu(bool restart)
    {
        SetRestartingFlag(restart);
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
    public void SetShopState()
    {
        anim.SetFloat("SetState", 2);
    }

    public void SetMenuState()
    {
        anim.SetFloat("SetState", 0);
    }

    private void SetRestartingFlag(bool value)
    {
        // Set the static variable
        isRestarting = value;
    }

    private bool IsRestarting()
    {
        // Retrieve the static variable
        return isRestarting;
    }

    public void OnSkinPurchased(int skinID)
    {
        PlayerDataManager.PurchasedSkinID = skinID;
        PlayerDataManager.SavePlayerData();

        // Activate the purchased skin
        ActivateSkin(skinID);
    }

    public void OnTrailPurchased(int trailID)
    {
        PlayerDataManager.PurchasedTrailID = trailID;
        PlayerDataManager.SavePlayerData();

        // Activate the purchased skin
        ActivateTrail(trailID);
    }
    private int GetSkinIDByPrice(int price)
    {
        switch (price)
        {
            case 0:
                return 0; // Base icon skin
            case 1000:
                return 1;
            case 1500:
                return 2;
            case 2000:
                return 3;
            case 2500:
                return 4;
            case 3000:
                return 5;
            case 7000:
                return 6;
            default:
                return -1; // Price not found, return -1 to indicate an error
        }
    }
    private int GetTrailIDByPrice(int price)
    {
        switch (price)
        {
            case 0:
                return 0; // Empty trail
            case 200:
                return 1;
            case 300:
                return 2;
            case 400:
                return 3;
            case 500:
                return 4;
            case 600:
                return 5;
            case 700:
                return 6;
            case 800:
                return 7;
            default:
                return -1; // Price not found, return -1 to indicate an error
        }
    }
    // Method to update the state of the buy button
    private void SetButtonState(SceneManagementButtonState state)
    {
        switch (state)
        {
            case SceneManagementButtonState.Buy:
                buyButton.interactable = true;
                buyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Buy";
                break;
            case SceneManagementButtonState.NotEnoughMoney:
                buyButton.interactable = false;
                buyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Not Enough Coins";
                break;
            case SceneManagementButtonState.Set:
                buyButton.interactable = true;
                buyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Set";
                break;
            case SceneManagementButtonState.AlreadySet:
                buyButton.interactable = false;
                buyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Already Set";
                break;
            default:
                Debug.LogWarning("Unsupported button state: " + state);
                break;
        }
    }
    private void SetButtonStateTrail(SceneManagementButtonState state)
    {
        switch (state)
        {
            case SceneManagementButtonState.Buy:
                buyButtonTrail.interactable = true;
                buyButtonTrail.GetComponentInChildren<TextMeshProUGUI>().text = "Buy";
                break;
            case SceneManagementButtonState.NotEnoughMoney:
                buyButtonTrail.interactable = false;
                buyButtonTrail.GetComponentInChildren<TextMeshProUGUI>().text = "Not Enough Coins";
                break;
            case SceneManagementButtonState.Set:
                buyButtonTrail.interactable = true;
                buyButtonTrail.GetComponentInChildren<TextMeshProUGUI>().text = "Set";
                break;
            case SceneManagementButtonState.AlreadySet:
                buyButtonTrail.interactable = false;
                buyButtonTrail.GetComponentInChildren<TextMeshProUGUI>().text = "Already Set";
                break;
            default:
                Debug.LogWarning("Unsupported button state: " + state);
                break;
        }
    }
}
