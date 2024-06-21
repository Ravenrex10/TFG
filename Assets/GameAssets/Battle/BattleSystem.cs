using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{
    public BattleState state;
    public GameObject victory;
    public GameObject derrota;
    public GameObject pattern;

    public Slider FireSlider;
    public Slider IceSlider;

    public TextMeshProUGUI FireLevelText;
    public TextMeshProUGUI IceLevelText;
    public TextMeshProUGUI CoinNumber;

    private int monedasBonus;
    private int partidasParaBonus;
    private int monedasNormal;
    private int timer;
    private int experiencia;

    private GameConfig config;

    // Start is called before the first frame update
    void Start()
    {
        config = ConfigLoader.getGameConfig();
        timer = config.AjustesDelJuego.temporizadorPartidaMin;
        monedasNormal = config.AjustesDelJuego.monedasPorCombate;
        monedasBonus = config.AjustesDelJuego.monedasBonus;
        partidasParaBonus = config.AjustesDelJuego.partidasParaBonus;
        experiencia = config.AjustesDelJuego.experienciaPorCombate;

        state = BattleState.START;
        SetupBattle();

        FireSlider.maxValue = PlayerPrefs.GetInt("MaxFireExp");
        IceSlider.maxValue = PlayerPrefs.GetInt("MaxIceExp");
    }

    void SetupBattle()
    {
        Debug.Log("Nuevo nivel fuego: " + PlayerPrefs.GetInt("NuevoNivelFuego"));
        Debug.Log("Nuevo nivel hielo: " + PlayerPrefs.GetInt("NuevoNivelHielo"));
        state = BattleState.PLAYERTURN;
        StartCoroutine(PlayerTurn());
        if(PlayerPrefs.GetInt("PartidasConTutorial") > 0)
        {
            pattern.GetComponent<Tutorial>().startTutorial();
            PlayerPrefs.SetInt("PartidasConTutorial", PlayerPrefs.GetInt("PartidasConTutorial") - 1);
        }else if (PlayerPrefs.GetString("SelectedEnemyType").Equals("Ice") && PlayerPrefs.GetInt("NuevoNivelFuego").Equals(1))
        {
            pattern.GetComponent<Tutorial>().startTutorial();
            PlayerPrefs.SetInt("NuevoNivelFuego", 0);
        }else if (PlayerPrefs.GetString("SelectedEnemyType").Equals("Fire") && PlayerPrefs.GetInt("NuevoNivelHielo").Equals(1))
        {
            pattern.GetComponent<Tutorial>().startTutorial();
            PlayerPrefs.SetInt("NuevoNivelHielo", 0);
        }
    }

    IEnumerator PlayerTurn()
    {
        state = BattleState.PLAYERTURN;
        yield return new WaitForSeconds(1f);
        GameObject.Find("Pattern").SetActive(true);
        if(PlayerPrefs.GetInt("TimesWrongPattern") >= 3)
        {
            pattern.GetComponent<Tutorial>().startTutorial();
        }
        DateTime timerEnds = DateTime.Now.AddSeconds(config.AjustesDelJuego.temporizadorPatronSeg);
        PlayerPrefs.SetString("TimeEnds", timerEnds.ToString("yyyy-MM-ddTHH:mm:ss"));
        GameObject.Find("Timer").GetComponent<temporizadorBatalla>().startTimer();
    }

    public IEnumerator EnemyTurn()
    {
        state = BattleState.ENEMYTURN;
        yield return new WaitForSeconds(1.2f);
        changeColour();
        GameObject.Find("Player").GetComponent<player>().TakeDamage(GameObject.Find("Enemy").GetComponent<Enemy>().damage);
        if(GameObject.Find("Player").GetComponent<player>().currentHealth <= 0)
        {
            Lose();
        }
        else
        {
            StartCoroutine(PlayerTurn());
        }
    }

    public void Win()
    {
        state = BattleState.WON;

        PlayerPrefs.SetInt(new string("level" + PlayerPrefs.GetInt("LevelSelected")), 1);

        int previousCoins = PlayerPrefs.GetInt("Coins");
        int newCoins;

        // Dar Experiencia y monedas al jugador
        if(PlayerPrefs.GetInt("PartidasHoy") % partidasParaBonus == 0)
        {
            PlayerPrefs.SetInt("Coins", (PlayerPrefs.GetInt("Coins") + monedasBonus));
        }
        else
        {
            PlayerPrefs.SetInt("Coins", (PlayerPrefs.GetInt("Coins") + monedasNormal));
        }
        newCoins = PlayerPrefs.GetInt("Coins");

        // Aumentamos las partidas jugadas hoy
        PlayerPrefs.SetInt("PartidasHoy", (PlayerPrefs.GetInt("PartidasHoy") + 1));

        GameObject.Find("Pattern").SetActive(false);

        // Animacion de muerte de enemigo
        GameObject.Find("Enemy").transform.position += new Vector3(0, -20, 0);

        //Abrir menú de resumen y continuar para volver al mapa
        victory.SetActive(true);
        FireLevelText.text = PlayerPrefs.GetInt("FireLevel").ToString();
        IceLevelText.text = PlayerPrefs.GetInt("IceLevel").ToString();
        CoinNumber.text = PlayerPrefs.GetInt("Coins").ToString();
        FireSlider.value = PlayerPrefs.GetInt("FireExp");
        IceSlider.value = PlayerPrefs.GetInt("IceExp");

        // Dar experiencia al jugador
        StartCoroutine(levelUp(experiencia));
        StartCoroutine(gainCoins(previousCoins, newCoins));

        // Hora a la que puede volver a jugar otra partida
        DateTime date = DateTime.Now.AddMinutes(timer);
        PlayerPrefs.SetString("NextAvailableBattle", date.ToString("yyyy-MM-ddTHH:mm:ss"));
        Debug.Log(date.ToString("yyyy-MM-ddTHH:mm:ss"));

        // Aumentamos los ataques necesarios para la próxima partida
        string enemyType = GameObject.Find("Enemy").GetComponent<Enemy>().enemyType;
        switch (enemyType){
            case "Fire":
                PlayerPrefs.SetInt("AtaquesPorPartidaFuego", PlayerPrefs.GetInt("AtaquesPorPartidaFuego") + 1);
                break;
            case "Ice":
                PlayerPrefs.SetInt("AtaquesPorPartidaHielo", PlayerPrefs.GetInt("AtaquesPorPartidaHielo") + 1);
                break;
        }
        
    }

    public void Lose()
    {
        state = BattleState.LOST;

        //Abrir menú de resumen y continuar para volver al mapa
        derrota.SetActive(true);
    }

    public void changeScene()
    {
        SceneManager.LoadScene("LevelSelector");
    }
    private IEnumerator gainCoins(int startCoins, int endCoins)
    {
        float duration = 1.0f; // Animation duration
        float elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float currentCoins = Mathf.Lerp(startCoins, endCoins, elapsedTime / duration);
            CoinNumber.text = Mathf.FloorToInt(currentCoins).ToString();
            yield return null;
        }

        CoinNumber.text = endCoins.ToString(); // Ensure the bar is set to the final value
    }
    

    private IEnumerator levelUp(int experienciaASubir)
    {
        int actualFireExp = PlayerPrefs.GetInt("FireExp");
        int actualIceExp = PlayerPrefs.GetInt("IceExp");
        string type = GameObject.Find("Enemy").GetComponent<Enemy>().enemyType;

        switch(type){
            case "Ice":
                int newFireExp = actualFireExp + experienciaASubir;
                while(newFireExp >= PlayerPrefs.GetInt("MaxFireExp")) // Se va a subir de nivel
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
                break;

            case "Fire":
                int newIceExp = actualIceExp + experienciaASubir;
                while(newIceExp >= PlayerPrefs.GetInt("MaxIceExp"))
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
                break;
        }
    }

    private IEnumerator AnimateExperienceBar(int startValue, int endValue, Slider experienceBar)
    {
        float duration = 1.0f; // Animation duration
        float elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            experienceBar.value = Mathf.Lerp(startValue, endValue, elapsedTime / duration);
            yield return null;
        }

        experienceBar.value = endValue; // Ensure the bar is set to the final value
    }

    private void changeColour()
    {
        for (int i = 0; i < 9; i++)
        {
            GameObject.Find("Node_" + i).GetComponent<SpriteRenderer>().color = Color.white;
        }
    }

}
