using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class healthPotion : MonoBehaviour
{
    public GameObject player;
    public GameObject BattleSystem;
    public GameObject Inventory;

    private void Start()
    {
        player = GameObject.Find("Player");
        BattleSystem = GameObject.Find("BattleSystem");
        Inventory = GameObject.Find("Inventory");
    }
    void OnMouseDown()
    {
        if(BattleSystem.GetComponent<BattleSystem>().state == BattleState.PLAYERTURN)
        {
            player.GetComponent<player>().Heal(25);
            PlayerPrefs.SetInt("HealthPotions", PlayerPrefs.GetInt("HealthPotions") - 1);
            Destroy(GameObject.Find("healthPotion(Clone)"));
            Inventory.GetComponent<InventoryManager>().loadHealthPotions();
        }
    }
}
