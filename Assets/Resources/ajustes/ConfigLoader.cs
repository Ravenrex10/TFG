using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ConfigLoader : MonoBehaviour
{
    // Start is called before the first frame update
    public static GameConfig getGameConfig()
    {
        // Relative path to the JSON configuration file inside the Resources folder
        string relativePath = Path.Combine("Resources", "ajustes", "ajustes.json");

        // Get the full path to the JSON file
        string jsonFilePath = Path.Combine(Application.dataPath, relativePath);

        // Read and parse the JSON file
        if (File.Exists(jsonFilePath))
        {
            string jsonString = File.ReadAllText(jsonFilePath);

            // Deserialize the JSON into GameConfig object
            GameConfig config = JsonUtility.FromJson<GameConfig>(jsonString);

            return config;
        }
        else
        {
            Debug.LogError("JSON file not found: " + jsonFilePath);
            return null;
        }
    }
}
