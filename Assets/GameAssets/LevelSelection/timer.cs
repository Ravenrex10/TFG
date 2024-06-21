using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class timer : MonoBehaviour
{
    public TextMeshProUGUI timerText;

    private DateTime nextGameDateTime;
    private bool isCountingDown;

    void Start()
    {
        string nextGame = PlayerPrefs.GetString("NextAvailableBattle");
        // Parse the nextGame string to DateTime
        if (DateTime.TryParseExact(nextGame, "yyyy-MM-ddTHH:mm:ss", null, System.Globalization.DateTimeStyles.None, out nextGameDateTime))
        {
            isCountingDown = true;
        }
        else
        {
            Debug.LogError("Failed to parse the nextGame string.");
        }
    }

    void Update()
    {
        if (isCountingDown)
        {
            TimeSpan timeRemaining = nextGameDateTime - DateTime.Now;
            if (timeRemaining.TotalSeconds > 0)
            {
                timerText.text = FormatTime(timeRemaining);
            }
            else
            {
                timerText.text = "00:00";
                isCountingDown = false;
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
