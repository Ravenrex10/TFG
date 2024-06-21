using System;
using TMPro;
using UnityEngine;

public class temporizadorBatalla : MonoBehaviour
{
    public TextMeshProUGUI timerText;

    private DateTime TimeEnds;
    private bool isCountingDown;

    void Start()
    {
        isCountingDown = false;
    }

    public void startTimer()
    {
        string timeBattle = PlayerPrefs.GetString("TimeEnds");
        // Parse the nextGame string to DateTime
        if (DateTime.TryParseExact(timeBattle, "yyyy-MM-ddTHH:mm:ss", null, System.Globalization.DateTimeStyles.None, out TimeEnds))
        {
            isCountingDown = true;
        }
        else
        {
            Debug.LogError("Failed to parse the timeBattle string.");
        }
    }

    public void stopTimer()
    {
        isCountingDown = false;
    }

    void Update()
    {
        if (isCountingDown)
        {
            TimeSpan timeRemaining = TimeEnds - DateTime.Now;
            if (timeRemaining.TotalSeconds > 0)
            {
                timerText.text = FormatTime(timeRemaining);
            }
            else
            {
                timerText.text = "00:00";
                isCountingDown = false;
                StartCoroutine(GameObject.Find("BattleSystem").GetComponent<BattleSystem>().EnemyTurn());
            }
        }
    }

    string FormatTime(TimeSpan timeSpan)
    {
        return string.Format("{0:D2}:{1:D2}",
                             timeSpan.Minutes,
                             timeSpan.Seconds);
    }
}
