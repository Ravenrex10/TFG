using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelDetails : MonoBehaviour
{
    public string enemyType;
    public int enemyHealth;
    public int enemyDamage;
    public bool completed;
    public int level;
    private LevelManager levelManager;

    private void Start()
    {
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        if(PlayerPrefs.GetInt(new string("level" + level)) == 1){
            completed = true;
        }
        else
        {
            completed = false;
        }
        
        // CAMBIAR EL SPRITE DEL NIVEL SI SE HA COMPLETADO Y GENERAR NUEVO NIVEL
        if (completed)
        {
            if (PlayerPrefs.GetInt(new string("level" + (level+1))) == 1) // el siguiente nivel ha sido completado
            {
                int health = PlayerPrefs.GetInt(new string("EnemyHealth" + (level + 1)));
                string type = PlayerPrefs.GetString(new string("EnemyType" + (level + 1)));
                int damage = PlayerPrefs.GetInt(new string("EnemyDamage" + (level + 1)));
                levelManager.generateLevelWithParameters(level, type, health, damage, this.gameObject.transform.position);
                
            }
            else // el siguiente nivel es nuevo
            {
                
                levelManager.generateLevel(level, this.gameObject.transform.position);
            }
            
        }
    }
}
