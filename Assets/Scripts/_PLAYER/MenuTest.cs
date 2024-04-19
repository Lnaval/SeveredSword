using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Google;

public class MenuTest : MonoBehaviour
{
    private FirebaseAuth auth;
    private DatabaseReference reference;
    public GameObject popUP;
    bool canPlay;
    bool progressDone;
    [SerializeField] private TMP_Text nameDisplay;

    ConfirmationPopUp confirmationPopUpScr;

    void Start()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        auth = FirebaseAuth.DefaultInstance;

        nameDisplay.text = UserIden.UserInstance.userName + "!";
        confirmationPopUpScr = GameObject.FindGameObjectWithTag("ConfirmPopUp").GetComponent<ConfirmationPopUp>();

        StartCoroutine(GetCanPlayerPlay());
        StartCoroutine(GetCompletion());
    }

    private IEnumerator GetCanPlayerPlay()
    {
        var res = reference.Child("Admin").Child("CanPlay").GetValueAsync();

        yield return new WaitUntil(predicate: () => res.IsCompleted);
        if (res.Exception != null)
        {
            Debug.LogWarning(message: $"Failed due to {res.Exception}");
        }
        else 
        {
            DataSnapshot snap = res.Result;
            canPlay = Convert.ToBoolean(snap.Value);
        }
    }

    private IEnumerator GetCompletion()
    {
        var res = reference.Child("User").Child(auth.CurrentUser.UserId).Child("Complete").GetValueAsync();

        yield return new WaitUntil(predicate: () => res.IsCompleted);
        if (res.Exception != null)
        {
            Debug.LogWarning(message: $"Failed due to {res.Exception}");
        }
        else 
        {
            DataSnapshot snap = res.Result;
            if(snap.Exists) {
                progressDone = Convert.ToBoolean(snap.Value);
            } else {
                progressDone = false;
            }
        }
    }
    
    public void Story()
    {
        Debug.Log(canPlay);
        if(canPlay == true){
            if(progressDone == true)
            {
                confirmationPopUpScr.CheckStatus("You have already reached the end of this adventure");
                popUP.SetActive(true);
            } else {
                SceneManager.LoadSceneAsync("PlayerSide");
            }
        } else {
            //Debug.Log("Di pa ready");
            confirmationPopUpScr.CheckStatus("Story is currently unavailable. ");
            popUP.SetActive(true);
        }
    }

    public void LogOut()
    {
        GoogleSignIn.DefaultInstance.SignOut();
        SceneManager.LoadScene("Authentication");
    }
}
