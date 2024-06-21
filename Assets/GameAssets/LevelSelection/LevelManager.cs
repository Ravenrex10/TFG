using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public string selectedEnemyType;
    public int selectedEnemyHealth;
    public int selectedEnemyDamage;
    public int levelSelected;
    public GameObject levelPrefab;
    public GameObject clouds;
    public float distanciaNubes;

    public float levelSpacing;
    public int rows;

    public int firstEnemyHealth;
    public int firstEnemyDamage;

    public float animationSpeed;

    private string nextGame;

    private GameConfig config;

    private void Start()
    {
        config = ConfigLoader.getGameConfig();
        float posX, posY;
        if(PlayerPrefs.HasKey("PlayerPosX") && PlayerPrefs.HasKey("PlayerPosY"))
        {
            posX = PlayerPrefs.GetFloat("PlayerPosX");
            posY = PlayerPrefs.GetFloat("PlayerPosY");
        }
        else
        {
            posX = GameObject.Find("Level1").GetComponent<Transform>().position.x;
            posY = GameObject.Find("Level1").GetComponent<Transform>().position.y;
        }
        GameObject.Find("player").GetComponent<Transform>().position = new Vector3(posX, posY, 0);
        clouds.transform.position = GameObject.Find("player").GetComponent<Transform>().position;
        clouds.transform.position += new Vector3(distanciaNubes, 0, 0);

        if (!PlayerPrefs.HasKey("EnemyDamage1"))
        {
            GameObject level = GameObject.Find("Level1");
            PlayerPrefs.SetInt(new string("EnemyHealth" + level.GetComponent<LevelDetails>().level), level.GetComponent<LevelDetails>().enemyHealth);
            PlayerPrefs.SetInt(new string("EnemyDamage" + level.GetComponent<LevelDetails>().level), level.GetComponent<LevelDetails>().enemyDamage);
            PlayerPrefs.SetString(new string("EnemyType" + level.GetComponent<LevelDetails>().level), level.GetComponent<LevelDetails>().enemyType);

            selectedEnemyType = level.GetComponent<LevelDetails>().enemyType;
            selectedEnemyHealth = level.GetComponent<LevelDetails>().enemyHealth;
            selectedEnemyDamage = level.GetComponent<LevelDetails>().enemyDamage;
            levelSelected = level.GetComponent<LevelDetails>().level;
        }
        DateTime currentDate = DateTime.Now;
        DateTime savedDate = DateTime.Parse(PlayerPrefs.GetString("Date"));
        if(currentDate.Date != savedDate.Date)
        {
            PlayerPrefs.SetString("Date", currentDate.Date.ToString("yyyy - MM - dd HH: mm:ss"));
            PlayerPrefs.SetInt("PartidasHoy", 0);
            Debug.Log("Dia nuevo");
        }

        nextGame = PlayerPrefs.GetString("NextAvailableBattle");
    }

    public void generateLevel(int levelNumber, Vector3 position)
    {
        levelNumber++;
        position.y = - (levelNumber % rows) * levelSpacing;
        position.x += levelSpacing;
        GameObject level = Instantiate(levelPrefab, position, Quaternion.identity);
        
        //ESPECIFICAR VALORES DE LOS ENEMIGOS
        if(levelNumber == 2) // SEGUNDA BATALLA (tutorial de hielo)
        {
            level.GetComponent<LevelDetails>().enemyHealth = firstEnemyHealth;
            level.GetComponent<LevelDetails>().enemyDamage = firstEnemyDamage;
            level.GetComponent<LevelDetails>().enemyType = "Ice";
        }
        else
        {
            // comprobamos si debemos crear un nivel de fuego o de hielo, como el fuego sube mas rapido siempre nos aseguraremos de no subir mas de fuego hasta que el hielo le alcance
            if(PlayerPrefs.GetInt("IceLevel") < PlayerPrefs.GetInt("FireLevel"))
            {
                level.GetComponent<LevelDetails>().enemyType = "Fire";
                level.GetComponent<LevelDetails>().enemyHealth = config.Jugador.ataque * PlayerPrefs.GetInt("AtaquesPorPartidaHielo");

            }
            else
            {
                level.GetComponent<LevelDetails>().enemyType = "Ice";
                level.GetComponent<LevelDetails>().enemyHealth = config.Jugador.ataque * PlayerPrefs.GetInt("AtaquesPorPartidaFuego");
            }
            level.GetComponent<LevelDetails>().enemyDamage = config.Enemigo.ataqueBase;
        }
        level.GetComponent<LevelDetails>().level = levelNumber;


        // GUARDAR EL NIVEL EN LA MEMORIA
        PlayerPrefs.SetInt(new string("EnemyHealth" + levelNumber), level.GetComponent<LevelDetails>().enemyHealth);
        PlayerPrefs.SetInt(new string("EnemyDamage" + levelNumber), level.GetComponent<LevelDetails>().enemyDamage);
        PlayerPrefs.SetString(new string("EnemyType" + levelNumber), level.GetComponent<LevelDetails>().enemyType);

        // MUEVE EL JUGADOR Y LAS NUBES
        StartCoroutine(ControlCoroutines(level));

        // CAMBIAR DATOS DEL NIVEL
        selectedEnemyType = level.GetComponent<LevelDetails>().enemyType;
        selectedEnemyHealth = level.GetComponent<LevelDetails>().enemyHealth;
        selectedEnemyDamage = level.GetComponent<LevelDetails>().enemyDamage;
        levelSelected = level.GetComponent<LevelDetails>().level;
    }

    public void generateLevelWithParameters(int levelNumber, string enemyType, int enemyHealth, int enemyDamage, Vector3 position)
    {
        levelNumber++;
        position.y = -(levelNumber % rows) * levelSpacing;
        position.x += levelSpacing;
        GameObject level = Instantiate(levelPrefab, position, Quaternion.identity);

        level.GetComponent<LevelDetails>().level = levelNumber;
        level.GetComponent<LevelDetails>().enemyDamage = enemyDamage;
        level.GetComponent<LevelDetails>().enemyType = enemyType;
        level.GetComponent<LevelDetails>().enemyHealth = enemyHealth;
    }


    private IEnumerator movePlayer(Vector3 end)
    {
        Vector3 start = GameObject.Find("player").GetComponent<Transform>().position;
        float journeyLength = Vector3.Distance(start, end);
        float startTime = Time.time;

        while (Vector3.Distance(GameObject.Find("player").GetComponent<Transform>().position, end) > 0.1f)
        {
            float distCovered = (Time.time - startTime) * animationSpeed;
            float fractionOfJourney = distCovered / journeyLength;

            GameObject.Find("player").GetComponent<Transform>().position = Vector3.Lerp(start, end, fractionOfJourney);

            yield return null;
        }

        GameObject.Find("player").GetComponent<Transform>().position = end; // Ensure the player is exactly at the end position
        // CAMBIAR POSICION DE JUGADOR EN LA MEMORIA
        PlayerPrefs.SetFloat("PlayerPosX", GameObject.Find("player").GetComponent<Transform>().position.x);
        PlayerPrefs.SetFloat("PlayerPosY", GameObject.Find("player").GetComponent<Transform>().position.y);
    }

    private IEnumerator moveClouds()
    {
        Vector3 start = clouds.transform.position;
        Vector3 end = clouds.transform.position + new Vector3(2, 0, 0);
        float journeyLength = Vector3.Distance(start, end);
        float startTime = Time.time;

        while (Vector3.Distance(clouds.transform.position, end) > 0.1f)
        {
            float distCovered = (Time.time - startTime) * animationSpeed;
            float fractionOfJourney = distCovered / journeyLength;
            clouds.transform.position = Vector3.Lerp(start, end, fractionOfJourney);
            yield return null;
        }
    }

    IEnumerator ControlCoroutines(GameObject level)
    {
        yield return StartCoroutine(moveClouds());
        yield return StartCoroutine(movePlayer((level.GetComponent<Transform>().position)));
    }

    public void play()
    {
        int comparisonResult = compareDates();
        if (comparisonResult >= 0)
        {
            PlayerPrefs.SetInt("SelectedEnemyHealth", selectedEnemyHealth);
            PlayerPrefs.SetString("SelectedEnemyType", selectedEnemyType);
            PlayerPrefs.SetInt("SelectedEnemyDamage", selectedEnemyDamage);
            PlayerPrefs.SetInt("LevelSelected", levelSelected);

            SceneManager.LoadScene("Battle");
        }
    }

    public int compareDates()
    {
        DateTime nextGameDateTime;
        DateTime.TryParseExact(nextGame, "yyyy-MM-ddTHH:mm:ss", null, System.Globalization.DateTimeStyles.None, out nextGameDateTime);
        DateTime currentDate = DateTime.Now;
        int comparisonResult = DateTime.Compare(currentDate, nextGameDateTime);

        return comparisonResult;
    }

    public void tienda()
    {
        SceneManager.LoadScene("Shop");
    }
}
