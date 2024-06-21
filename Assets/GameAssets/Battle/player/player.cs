using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    public HealthBar healthBar;
    public int maxHealth = 100;
    public int currentHealth;

    private Renderer playerRenderer;
    public float blinkDuration = 0.1f; // Duración de cada parpadeo
    public int blinkCount = 5; // Número de parpadeos

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.setMaxHealth(maxHealth);
        playerRenderer = GetComponent<Renderer>();
    }

    // Method to decrease the player's health
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        healthBar.setHealth(currentHealth);
        StartCoroutine(BlinkCharacter());

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
        Debug.Log("Player has died!");
    }

    // Update is called once per frame
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(10);
        }
    }

    private IEnumerator BlinkCharacter()
    {
        for (int i = 0; i < blinkCount; i++)
        {
            playerRenderer.enabled = false; // Oculta al personaje
            yield return new WaitForSeconds(blinkDuration); // Espera un momento
            playerRenderer.enabled = true; // Muestra al personaje
            yield return new WaitForSeconds(blinkDuration); // Espera un momento
        }
    }

}
