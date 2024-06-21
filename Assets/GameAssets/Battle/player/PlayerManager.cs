using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    public int iceLevel;
    public int fireLevel;
    public int coins;

    private void Start()
    {
        coins = PlayerPrefs.GetInt("Coins");
        fireLevel = PlayerPrefs.GetInt("FireLevel");
        iceLevel = PlayerPrefs.GetInt("IceLevel");
    }
}
