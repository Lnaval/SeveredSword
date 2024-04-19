using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Auth;

public class RandomChapterScript : MonoBehaviour
{
    private DatabaseReference reference;
    [SerializeField] private Toggle toggleRandom;

    void Start()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        
        StartCoroutine(GetChaptersRandom());
    }

    private IEnumerator GetChaptersRandom()
    {
        var res = reference.Child("Admin").Child("Randomize").GetValueAsync();

        yield return new WaitUntil(predicate: () => res.IsCompleted);
        if (res.Exception != null)
        {
            Debug.LogWarning(message: $"Failed due to {res.Exception}");
        }
        else 
        {
            DataSnapshot snapshot = res.Result;

            toggleRandom.isOn = Convert.ToBoolean(snapshot.Value);
        }
    }

    public void ToggleRandom()
    {
        Debug.Log(toggleRandom.isOn);
        StartCoroutine(SetChaptersRandom(toggleRandom.isOn));
    }

    private IEnumerator SetChaptersRandom(bool isActive)
    {
        var res = reference.Child("Admin").Child("Randomize").SetValueAsync(isActive);

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
