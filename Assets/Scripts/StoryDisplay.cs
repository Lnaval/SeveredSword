using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using UnityEngine.UI;
using TMPro; 
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Firebase.Storage;

public class StoryDisplay : MonoBehaviour
{
    DatabaseReference reference;

    [SerializeField] private GameObject storyElement;
    [SerializeField] private Transform storyContent;

    string title;
    string chapID;    

    public void GetStory()
    {
        StartCoroutine(LoadStory());
    }

    public IEnumerator LoadStory()
    {
        var dbTask = reference.Child("Story").OrderByChild("ChapterID").GetValueAsync();
        yield return new WaitUntil(predicate: () => dbTask.IsCompleted);

        if (dbTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed due to {dbTask.Exception}");
        }
        else 
        {
            DataSnapshot storySnapshot = dbTask.Result;
            Debug.Log(storySnapshot);
            // DataSnapshot firstTitleChild = storySnapshot.Children.First();
            foreach (Transform child in storyContent.transform)
            {
                Destroy(child.gameObject);
            }
            

            foreach (DataSnapshot childSnap in storySnapshot.Children)
            {
                title = childSnap.Key.ToString(); 
                Debug.Log(childSnap);
                chapID = childSnap.Child("ChapterID").Value.ToString();

                
                GameObject storyListElement = Instantiate(storyElement, storyContent); 
                storyListElement.GetComponent<StoryElement>().NewStoryElement(title, chapID);
            }
        }
    }

    void Start()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }
}
