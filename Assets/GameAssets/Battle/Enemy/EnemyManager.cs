using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    public string enemyType;
    public int enemyHealth;
    public int enemyDamage;

    void Start()
    {
        enemyType = PlayerPrefs.GetString("SelectedEnemyType");
        enemyHealth = PlayerPrefs.GetInt("SelectedEnemyHealth");
        enemyDamage = PlayerPrefs.GetInt("SelectedEnemyDamage");
    }
}
