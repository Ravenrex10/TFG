using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PatternData
{
    public Pattern[] patterns;
}

[System.Serializable]
public class Pattern
{
    public string name;
    public Level[] levels;
}

[System.Serializable]
public class Level
{
    public string name;
    public int[] pattern;
}
namespace PatternManagement
{
    public class PatternLoader : MonoBehaviour
    {
        private PatternData patternData;
        void Start()
        {
            
        }

        public PatternData getPatternData()
        {
            TextAsset jsonFile = Resources.Load<TextAsset>("patterns");
            if (jsonFile != null)
            {
                patternData = JsonUtility.FromJson<PatternData>(jsonFile.text);
                return patternData;
            }
            else
            {
                // Handle the case where the file is not found (e.g., log an error message)
                Debug.LogError("Patterns file not found!");
                return null;
            }
        }
    }
}