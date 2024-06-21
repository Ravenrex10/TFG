using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System;
using TMPro;
using UnityEngine;

public class firebase : MonoBehaviour
{
    private DatabaseReference dbReference;
    public GameObject panel;
    public UnityEngine.UI.Button submit;
    public TMP_InputField nombre;
    public TMP_InputField apellidos;
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
                CheckUserIDExists(SystemInfo.deviceUniqueIdentifier);
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + task.Result);
            }
        });

    }

    void UploadUserIDWithTimestamp()
    {
        string userID = SystemInfo.deviceUniqueIdentifier;
        string timestamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");

        DatabaseReference userReference = dbReference.Child("inicio").Push();
        userReference.Child("usuarioID").SetValueAsync(userID);
        if(compareDates() >= 0)
        {
            userReference.Child("puedeJugar").SetValueAsync("si");
        }
        else
        {
            userReference.Child("puedeJugar").SetValueAsync("no");
        }
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

    private int compareDates()
    {
        string nextGame = PlayerPrefs.GetString("NextAvailableBattle");
        DateTime nextGameDateTime;
        DateTime.TryParseExact(nextGame, "yyyy-MM-ddTHH:mm:ss", null, System.Globalization.DateTimeStyles.None, out nextGameDateTime);
        DateTime currentDate = DateTime.Now;
        int comparisonResult = DateTime.Compare(currentDate, nextGameDateTime);

        return comparisonResult;
    }

    void CheckUserIDExists(string userID)
    {
        dbReference.Child("usuarios").OrderByChild("usuarioID").EqualTo(userID).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error checking user ID: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (!snapshot.Exists)
                {
                    panel.SetActive(true);
                }
            }
        });
    }

    public void registrarUsuario()
    {
        string userID = SystemInfo.deviceUniqueIdentifier;
        string nombre = this.nombre.text;
        string apellidos = this.apellidos.text;
        DatabaseReference userReference = dbReference.Child("usuarios").Push();
        userReference.Child("usuarioID").SetValueAsync(userID);
        userReference.Child("nombre").SetValueAsync(nombre);
        userReference.Child("apellidos").SetValueAsync(apellidos).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Usuario subido con éxito");
            }
            else
            {
                Debug.LogError("Error al subir al usuario: " + task.Exception);
            }
        }); ;
        panel.SetActive(false);
    }
}
