using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;

public class firebaseBattle : MonoBehaviour
{
    private DatabaseReference dbReference;
    void Start()
    {
        // Initialize Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;
                dbReference = FirebaseDatabase.DefaultInstance.RootReference;
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + task.Result);
            }
        });

    }

    public void uploadPattern(int[] pattern, string correct, string time, List<string> timePerNode)
    {
        string userID = SystemInfo.deviceUniqueIdentifier;
        string timestamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");

        DatabaseReference userReference = dbReference.Child("patron").Push();
        userReference.Child("userID").SetValueAsync(userID);
        userReference.Child("patron").SetValueAsync(pattern);
        userReference.Child("correcto").SetValueAsync(correct);
        userReference.Child("tiempoEmpleado").SetValueAsync(time);
        userReference.Child("tiempoPorNodo").SetValueAsync(timePerNode);
        userReference.Child("HoraDeConexión").SetValueAsync(timestamp).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("User ID and timestamp uploaded successfully.");
            }
            else
            {
                Debug.LogError("Failed to upload data: " + task.Exception);
            }
        });
    }
}
