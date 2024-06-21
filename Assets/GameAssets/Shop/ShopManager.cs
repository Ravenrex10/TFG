using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    //Valores modificables
    private int healthPotionPrice;
    private int xpPotionPrice;
    private int experienciaASubir;

    //Objetos del juego
    public Slider FireSlider;
    public Slider IceSlider;

    public TextMeshProUGUI FireLevelText;
    public TextMeshProUGUI IceLevelText;
    public TextMeshProUGUI CantidadPociones;

    public GameObject firebase;

    private GameConfig config;

    private void Start()
    {
        config = ConfigLoader.getGameConfig();
        healthPotionPrice = config.Tienda.precioPocionVida;
        xpPotionPrice = config.Tienda.precioPocionExperiencia;
        experienciaASubir = config.Tienda.experienciaPorPocion;

        updateCoins();
        FireSlider.maxValue = PlayerPrefs.GetInt("MaxFireExp");
        IceSlider.maxValue = PlayerPrefs.GetInt("MaxIceExp");
        FireLevelText.text = PlayerPrefs.GetInt("FireLevel").ToString();
        IceLevelText.text = PlayerPrefs.GetInt("IceLevel").ToString();
        FireSlider.value = PlayerPrefs.GetInt("FireExp");
        IceSlider.value = PlayerPrefs.GetInt("IceExp");
        CantidadPociones.text = PlayerPrefs.GetInt("HealthPotions").ToString();
    }

    public void buyHealthPotion()
    {
        if (PlayerPrefs.GetInt("Coins") >= healthPotionPrice)
        {
            // Restamos el dinero
            PlayerPrefs.SetInt("Coins", (PlayerPrefs.GetInt("Coins") - healthPotionPrice));
            // Añadimos la poción al inventario
            PlayerPrefs.SetInt("HealthPotions", (PlayerPrefs.GetInt("HealthPotions") + 1));
            CantidadPociones.text = PlayerPrefs.GetInt("HealthPotions").ToString();

            updateCoins();

            firebase.GetComponent<firebaseShop>().UploadPotionBought("vida");
        }
    }

    public void buyXpPotion()
    {
        if (PlayerPrefs.GetInt("Coins") >= xpPotionPrice)
        {
            // Restamos el dinero
            PlayerPrefs.SetInt("Coins", (PlayerPrefs.GetInt("Coins") - xpPotionPrice));

            updateCoins();

            //Damos experiencia al jugador
            StartCoroutine(levelUpFire(experienciaASubir));
            StartCoroutine(levelUpIce(experienciaASubir));

            firebase.GetComponent<firebaseShop>().UploadPotionBought("experiencia");
        }
    }

    void updateCoins()
    {
        GameObject.Find("CoinsNumber").GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetInt("Coins").ToString();
    }

    public void exit()
    {
        SceneManager.LoadScene("LevelSelector");
    }

    // Funciones para subir de nivel

    private IEnumerator levelUpFire(int experienciaASubir)
    {
        int actualFireExp = PlayerPrefs.GetInt("FireExp");
        int newFireExp = actualFireExp + experienciaASubir;
        while (newFireExp >= PlayerPrefs.GetInt("MaxFireExp")) // Se va a subir de nivel
        {
            newFireExp -= PlayerPrefs.GetInt("MaxFireExp");
            PlayerPrefs.SetInt("FireLevel", PlayerPrefs.GetInt("FireLevel") + 1);
            PlayerPrefs.SetInt("AtaquesPorPartidaFuego", config.Jugador.ataquesPorPartidaIniciales);
            PlayerPrefs.SetInt("NuevoNivelFuego", 1);
            yield return StartCoroutine(AnimateExperienceBar(actualFireExp, PlayerPrefs.GetInt("MaxFireExp"), FireSlider)); // Animacion para subir de nivel
            FireLevelText.text = PlayerPrefs.GetInt("FireLevel").ToString();
            PlayerPrefs.SetInt("MaxFireExp", PlayerPrefs.GetInt("MaxFireExp") + 10);    // Subimos la experiencia maxima para subir de nivel
            actualFireExp = 0;
            FireSlider.value = 0;
        }
        PlayerPrefs.SetInt("FireExp", newFireExp);
        yield return StartCoroutine(AnimateExperienceBar(actualFireExp, newFireExp, FireSlider));
    }

    private IEnumerator levelUpIce(int experienciaASubir)
    {
        int actualIceExp = PlayerPrefs.GetInt("IceExp");
        int newIceExp = actualIceExp + experienciaASubir;
        while (newIceExp >= PlayerPrefs.GetInt("MaxIceExp"))
        {
            newIceExp -= PlayerPrefs.GetInt("MaxIceExp");
            PlayerPrefs.SetInt("IceLevel", PlayerPrefs.GetInt("IceLevel") + 1);
            PlayerPrefs.SetInt("AtaquesPorPartidaHielo", config.Jugador.ataquesPorPartidaIniciales);
            PlayerPrefs.SetInt("NuevoNivelHielo", 1);
            yield return StartCoroutine(AnimateExperienceBar(actualIceExp, PlayerPrefs.GetInt("MaxIceExp"), IceSlider));
            IceLevelText.text = PlayerPrefs.GetInt("IceLevel").ToString();
            PlayerPrefs.SetInt("MaxIceExp", PlayerPrefs.GetInt("MaxIceExp") + 10);
            actualIceExp = 0;
            IceSlider.value = 0;
        }
        PlayerPrefs.SetInt("IceExp", newIceExp);
        yield return StartCoroutine(AnimateExperienceBar(actualIceExp, newIceExp, IceSlider));
    }

    private IEnumerator AnimateExperienceBar(int startValue, int endValue, Slider experienceBar)
    {
        float duration = 1.0f; // Duracion de la animacion
        float elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            experienceBar.value = Mathf.Lerp(startValue, endValue, elapsedTime / duration);
            yield return null;
        }

        experienceBar.value = endValue; // Aseguramos que esté bien el valor final
    }
}
