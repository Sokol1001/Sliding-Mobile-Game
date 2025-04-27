using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    public TextMeshProUGUI coinCountTXT;
    int coinCount = 0;

    private void Update()
    {
        PlayerDataManager.LoadPlayerData();
        coinCount = PlayerDataManager.CoinCount;
        coinCountTXT.text = coinCount + ""; //+ " <sprite name=\"coin_0\">";
    }
}
