using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Auth;

public class StartGameToggle : MonoBehaviour
{
    private DatabaseReference reference;
    [SerializeField] private Toggle toggleStart;

    void Start()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        
        StartCoroutine(GetCanPlayerPlay());
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
            DataSnapshot snapshot = res.Result;

            toggleStart.isOn = Convert.ToBoolean(snapshot.Value);
        }
    }

    public void ToggleStart()
    {
        Debug.Log(toggleStart.isOn);
        StartCoroutine(SetCanPlayerPlay(toggleStart.isOn));
    }

    private IEnumerator SetCanPlayerPlay(bool isActive)
    {
        var res = reference.Child("Admin").Child("CanPlay").SetValueAsync(isActive);

        yield return new WaitUntil(predicate: () => res.IsCompleted);
        if (res.Exception != null)
        {
            Debug.LogWarning(message: $"Failed due to {res.Exception}");
        }
        else 
        {
            //successfully saved, add action on what to do next after saving
            Debug.Log("Saved");
        }
    }
}
