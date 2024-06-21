using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public HealthBar healthBar;
    private int maxHealth;
    public int currentHealth;
    public string enemyType;
    public int damage;
    public Sprite fireSprite;
    public Sprite iceSprite;

    private Renderer enemyRenderer;
    public float blinkDuration = 0.1f; // Duración de cada parpadeo
    public int blinkCount = 5; // Número de parpadeos

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = GameObject.Find("EnemyManager").GetComponent<EnemyManager>().enemyHealth;
        enemyType = GameObject.Find("EnemyManager").GetComponent<EnemyManager>().enemyType;
        damage = GameObject.Find("EnemyManager").GetComponent<EnemyManager>().enemyDamage;
        currentHealth = maxHealth;
        healthBar.setMaxHealth(maxHealth);
        if (enemyType.Equals("Fire"))
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = fireSprite;
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = iceSprite;
        }
        enemyRenderer = gameObject.GetComponent<Renderer>();
    }

    // Method to decrease the player's health
    public void TakeDamage(int damageAmount)
    {
        StartCoroutine(BlinkCharacter());
        currentHealth -= damageAmount;
        healthBar.setHealth(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Method to increase the player's health
    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        healthBar.setHealth(currentHealth);

        // Make sure the player's health doesn't exceed the maximum health
        currentHealth = Mathf.Min(currentHealth, maxHealth);
    }

    private void Die()
    {
        Debug.Log("Enemy has died!");
    }

    private IEnumerator BlinkCharacter()
    {
        for (int i = 0; i < blinkCount; i++)
        {
            enemyRenderer.enabled = false; // Oculta al personaje
            yield return new WaitForSeconds(blinkDuration); // Espera un momento
            enemyRenderer.enabled = true; // Muestra al personaje
            yield return new WaitForSeconds(blinkDuration); // Espera un momento
        }
    }
}
