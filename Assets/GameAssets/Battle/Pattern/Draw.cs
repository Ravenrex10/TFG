using System.Collections.Generic;
using UnityEngine;

using PatternManagement;
using System.Linq;
using System;
using System.Diagnostics;

public class Draw : MonoBehaviour
{
    public string enemyType;
    private firebaseBattle firebase;
    private int fireLevel;
    private int iceLevel;
    private int criticalHit;

    private LineRenderer lineRenderer;
    private bool isMousePressed = false;
    private List<GameObject> nodes;
    private PatternData patternData;
    private int nodeIndex;
    private int[] pattern;
    private int drawIndex;

    //green: 53FF00
    //red: FF0500
    //white: FFFFFF

    private DateTime comienzoPatron;
    private DateTime finPatron;

    public BattleState state;
    private Stopwatch stopwatch;
    private List<string> timePerNode;

    GameConfig config;
    // Start is called before the first frame update
    void Start()
    {
        config = ConfigLoader.getGameConfig();
        criticalHit = config.Jugador.ataque;
        pattern = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        //pattern[nodeIndex] = 1;

        nodeIndex = int.Parse(gameObject.name.Substring(5));
        drawIndex = 0;

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;

        nodes = new List<GameObject>(GameObject.FindGameObjectsWithTag("Node"));

        PatternLoader patternLoader = gameObject.AddComponent<PatternLoader>();
        patternData = patternLoader.getPatternData();

        //fireLevel = GameObject.Find("PlayerManager").GetComponent<PlayerManager>().fireLevel;
        //iceLevel = GameObject.Find("PlayerManager").GetComponent<PlayerManager>().iceLevel;
        fireLevel = PlayerPrefs.GetInt("FireLevel");
        iceLevel = PlayerPrefs.GetInt("IceLevel");

        firebase = GameObject.Find("firebase").GetComponent<firebaseBattle>();

        stopwatch = new Stopwatch();
        timePerNode = new List<string>();
    }

    void OnMouseDown()
    {
        if(GameObject.Find("BattleSystem").GetComponent<BattleSystem>().state.Equals(BattleState.PLAYERTURN))
        {
            isMousePressed = true;
            lineRenderer.positionCount = 1;
            lineRenderer.SetPosition(0, transform.position);
            stopwatch.Start();
        }
        
    }

    void OnMouseDrag()
    {
        if (isMousePressed && GameObject.Find("BattleSystem").GetComponent<BattleSystem>().state.Equals(BattleState.PLAYERTURN))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            mousePos.z = 0;
            if (lineRenderer.positionCount < 2)
            {
                lineRenderer.positionCount = 2;
            }

            lineRenderer.SetPosition(lineRenderer.positionCount - 1, mousePos);

            SnapToNearestCircle(mousePos);
        }
    }

    public int getIndex()
    {
        return nodeIndex;
    }

    void SnapToNearestCircle(Vector3 currentPos)
    {
        float minDistance = 0.8f;
        Vector3 closestCirclePos;

        List<GameObject> copyOfNodes = new List<GameObject>(nodes);

        // Find the closest circle
        foreach (GameObject circle in copyOfNodes)
        {
            float distance = Vector3.Distance(currentPos, circle.transform.position);
            if (distance < minDistance)
            {
                drawIndex++;
                minDistance = distance;
                closestCirclePos = circle.transform.position;

                // Snap to the closest circle
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, circle.transform.position);
                lineRenderer.positionCount++;
                nodes.Remove(circle);
                pattern[circle.GetComponent<Draw>().getIndex()] = drawIndex;
                TimeSpan elapsedTime = stopwatch.Elapsed;
                timePerNode.Add(elapsedTime.ToString(@"hh\:mm\:ss\.fff"));
            }
        }
    }

    void OnMouseUp()
    {
        if (GameObject.Find("BattleSystem").GetComponent<BattleSystem>().state.Equals(BattleState.PLAYERTURN))
        {
            isMousePressed = false;
            lineRenderer.positionCount = 0;
            nodes = new List<GameObject>(GameObject.FindGameObjectsWithTag("Node"));
            drawIndex = 0;

            comparePattern(pattern, patternData);
            pattern = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            stopwatch.Stop();
        }
        GameObject.Find("Timer").GetComponent<temporizadorBatalla>().stopTimer();
    }

    private void comparePattern(int[] pattern, PatternData patternData)
    {
        string correct;
        // TIPO FUEGO
        switch (GameObject.Find("Enemy").GetComponent<Enemy>().enemyType)
        {
            case "Ice":
                if(compareSequence(pattern, patternData.patterns[0].levels[fireLevel - 1].pattern))
                {
                    PlayerPrefs.SetInt("TimesWrongPattern", 0);
                    GameObject.Find("Enemy").GetComponent<Enemy>().TakeDamage(criticalHit);
                    correct = "si";
                }
                else
                {
                    PlayerPrefs.SetInt("TimesWrongPattern", PlayerPrefs.GetInt("TimesWrongPattern") + 1);
                    correct = "no";
                }
                changeColour(pattern, patternData.patterns[0].levels[fireLevel - 1].pattern);
                break;
            case "Fire":
                if(compareSequence(pattern, patternData.patterns[1].levels[iceLevel - 1].pattern))
                {
                    PlayerPrefs.SetInt("TimesWrongPattern", 0);
                    GameObject.Find("Enemy").GetComponent<Enemy>().TakeDamage(criticalHit);
                    correct = "si";
                }
                else
                {
                    PlayerPrefs.SetInt("TimesWrongPattern", PlayerPrefs.GetInt("TimesWrongPattern") + 1);
                    correct = "no";
                }
                changeColour(pattern, patternData.patterns[1].levels[iceLevel - 1].pattern);
                break;
            default:
                correct = "no";
                break;
        }

        // COMPROBAR SI HA MUERTO O NO
        if (GameObject.Find("Enemy").GetComponent<Enemy>().currentHealth <= 0)
        {
            GameObject.Find("BattleSystem").GetComponent<BattleSystem>().Win();
        }
        else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(GameObject.Find("BattleSystem").GetComponent<BattleSystem>().EnemyTurn());
        }

        // Enviamos los datos a la base de datos
        String tiempoTotal = stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.fff");
        firebase.uploadPattern(pattern, correct, tiempoTotal, timePerNode);
    }

    string FormatTime(TimeSpan timeSpan)
    {
        return string.Format("{0:D2}:{1:D2}",
                             timeSpan.Minutes,
                             timeSpan.Seconds);
    }

    private void changeColour(int[] patternDrawn, int[] correctPattern)
    {
        for (int i = 0; i < patternDrawn.Length; i++)
        {
            if (patternDrawn[i] == correctPattern[i] && patternDrawn[i] != 0)
            {
                GameObject.Find("Node_" + i).GetComponent<SpriteRenderer>().color = Color.green;
            }else if (patternDrawn[i] != correctPattern[i] && patternDrawn[i] != 0)
            {
                GameObject.Find("Node_" + i).GetComponent<SpriteRenderer>().color = Color.red;
            }
        }
    }

    private bool compareSequence(int[] drawnPattern, int[] correctPattern)
    {
        for (int i = 0; i < drawnPattern.Length; i++)
        {
            if (drawnPattern[i] != correctPattern[i])
            {
                return false;
            }
        }
        return true;
    }
}
