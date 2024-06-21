using PatternManagement;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public float drawSpeed = 1.0f; // Speed at which the line is drawn
    public float fadeSpeed = 1.0f; // Speed at which the icon fades in/out
    private int[] tutorialPattern;
    private PatternData patternData;
    private int index;
    public GameObject HandIcon;
    public int vecesQueSeRepite = 3;
    private Vector3 vectorRectificar = new Vector3(2f, -2f, -5f);

    void Start()
    {
        PatternLoader patternLoader = gameObject.AddComponent<PatternLoader>();
        patternData = patternLoader.getPatternData();
        switch (PlayerPrefs.GetString("SelectedEnemyType"))
        {
            case "Ice":
                tutorialPattern = patternData.patterns[0].levels[PlayerPrefs.GetInt("FireLevel") - 1].pattern;
                break;

            case "Fire":
                tutorialPattern = patternData.patterns[1].levels[PlayerPrefs.GetInt("IceLevel") - 1].pattern;
                break;
        }
    }

    public void startTutorial()
    {
        StartCoroutine(TutorialController());
    }

    IEnumerator TutorialController()
    {
        for(int i = 0; i < vecesQueSeRepite; i++)
        {
            index = 1;
            yield return DrawTutorial();
        }
    }
    IEnumerator DrawTutorial()
    {
        GameObject startNode = null;
        GameObject endNode = null;
        bool last = true;
        for(int i = 0; i < tutorialPattern.Length; i++)
        {
            if (tutorialPattern[i].Equals(index))
            {
                startNode = GameObject.Find(new string("Node_") + i);
            }
            else if (tutorialPattern[i].Equals(index + 1))
            {
                endNode = GameObject.Find(new string("Node_") + i);
                last = false;
            }
        }
        if(index == 1)
        {
            HandIcon.SetActive(true);
            setAlpha(0);
            HandIcon.transform.position = startNode.transform.position + vectorRectificar;
            yield return appearIcon();
            yield return DrawBetween(startNode, endNode);
            index++;
            yield return DrawTutorial();

        }
        else if (last)
        {
            yield return disappearIcon();
            HandIcon.SetActive(false);
        }
        else
        {
            index++;
            yield return DrawBetween(startNode, endNode);
            yield return DrawTutorial();
        }
    }

    IEnumerator DrawBetween(GameObject startNode, GameObject endNode)
    {
        yield return null;
        float elapsedTime = 0.0f;
        while (elapsedTime < drawSpeed)
        {
            elapsedTime += Time.deltaTime;
            Vector3 currentPos = Vector3.Lerp(startNode.transform.position + vectorRectificar, endNode.transform.position + vectorRectificar, elapsedTime / drawSpeed);
            HandIcon.transform.position = currentPos;
            yield return null;
        }

        HandIcon.transform.position = endNode.transform.position + vectorRectificar;
    }

    IEnumerator appearIcon()
    {
        float elapsedTime = 0.0f;
        while(elapsedTime < fadeSpeed)
        {
            elapsedTime += Time.deltaTime;
            float currentAlpha = Mathf.Lerp(0, 1, elapsedTime / drawSpeed);
            setAlpha(currentAlpha);
            yield return null;
        }
        setAlpha(1);
    }

    IEnumerator disappearIcon()
    {
        float elapsedTime = 0.0f;
        while(elapsedTime < fadeSpeed)
        {
            elapsedTime += Time.deltaTime;
            float currentAlpha = Mathf.Lerp(1, 0, elapsedTime / drawSpeed);
            setAlpha(currentAlpha);
            yield return null;
        }
        setAlpha(0);
    }

    private void setAlpha(float alpha)
    {
        Color color = HandIcon.GetComponent<Renderer>().material.color;
        color.a = alpha;
        HandIcon.GetComponent<Renderer>().material.color = color;
    }
}
