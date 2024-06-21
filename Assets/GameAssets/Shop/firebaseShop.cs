using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System;
using UnityEngine;

public class firebaseShop : MonoBehaviour
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
                UploadUserIDWithTimestamp();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + task.Result);
            }
        });

    }

    void UploadUserIDWithTimestamp() // Cuando se conecta
    {
        string userID = SystemInfo.deviceUniqueIdentifier;
        string timestamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");

        DatabaseReference userReference = dbReference.Child("tienda").Push();
        userReference.Child("userID").SetValueAsync(userID);
        userReference.Child("monedas").SetValueAsync(PlayerPrefs.GetInt("Coins"));
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

    public void UploadPotionBought(string pocion) // Cuando compra una poción
    {
        string userID = SystemInfo.deviceUniqueIdentifier;
        string timestamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");

        DatabaseReference userReference = dbReference.Child("compra").Push();
        userReference.Child("userID").SetValueAsync(userID);
        userReference.Child("monedas").SetValueAsync(PlayerPrefs.GetInt("Coins"));
        userReference.Child("pocionesDeVidaTotales").SetValueAsync(PlayerPrefs.GetInt("HealthPotions"));
        userReference.Child("pocionComprada").SetValueAsync(pocion);
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
