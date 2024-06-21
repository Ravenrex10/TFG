using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class play : MonoBehaviour
{

    private void Start()
    {
        GameConfig config = ConfigLoader.getGameConfig();

        if (!PlayerPrefs.HasKey("FireLevel") || !PlayerPrefs.HasKey("IceLevel"))
        {
            PlayerPrefs.SetInt("FireLevel", 1);
            PlayerPrefs.SetInt("IceLevel", 1);

            PlayerPrefs.SetInt("FireExp", 0);
            PlayerPrefs.SetInt("IceExp", 0);

            PlayerPrefs.SetInt("PartidasHoy", 0);
            PlayerPrefs.SetString("Date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            PlayerPrefs.SetString("NextAvailableBattle", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"));

            PlayerPrefs.SetInt("HealthPotions", 0);

            PlayerPrefs.SetInt("Coins", 0);

            PlayerPrefs.SetInt("TimesWrongPattern", 0);

            PlayerPrefs.SetInt("MaxFireExp", config.Jugador.experienciaMaximaFuegoInicial);
            PlayerPrefs.SetInt("MaxIceExp", config.Jugador.experienciaMaximaHieloInicial);

            PlayerPrefs.SetInt("PartidasConTutorial", config.AjustesDelJuego.combatesConTutorial);

            PlayerPrefs.SetInt("AtaquesPorPartidaHielo", config.Jugador.ataquesPorPartidaIniciales);
            PlayerPrefs.SetInt("AtaquesPorPartidaFuego", config.Jugador.ataquesPorPartidaIniciales);

            PlayerPrefs.SetInt("NuevoNivelFuego", 0);
            PlayerPrefs.SetInt("NuevoNivelHielo", 0);
        }
    }
    public void jugar()
    {
        SceneManager.LoadScene("LevelSelector");
    }

    public void tienda()
    {
        SceneManager.LoadScene("Shop");
    }

    //AÑADIR MÁS BOTONES SI ES NECESARIO
}
