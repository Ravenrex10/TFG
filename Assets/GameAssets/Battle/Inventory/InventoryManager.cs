using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryManager : MonoBehaviour
{
    private int healthPotions;
    private int xpPotions;
    public GameObject healhPotionPrefab;
    void Start()
    {
        loadHealthPotions();
        Debug.Log("Tienes " + healthPotions + " pociones de vida");
    }

    public void loadHealthPotions()
    {
        healthPotions = PlayerPrefs.GetInt("HealthPotions");
        if (healthPotions > 0)
        {
            Vector3 position = GameObject.Find("Slot1").transform.position + new Vector3(0, 0, -5);
            GameObject healthPotion = Instantiate(healhPotionPrefab, position, Quaternion.identity);
        }
    }
}
