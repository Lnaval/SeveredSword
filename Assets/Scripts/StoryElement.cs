using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;
using System;
using System.Text;

public class StoryElement : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text chapterNumberText;

    public GameObject editStoryPrefab;

    private string chapterID; 

    StoryDisplay storyDisplay;

    public void NewStoryElement(string title, string chapID)
    {
        titleText.text = title;
        chapterNumberText.text = chapID;
        chapterID = chapID; 
    }

    // public void OnClickDelete()
    // {
    //     storyDisplay = GameObject.FindGameObjectWithTag("StoryDisplay").GetComponent<StoryDisplay>();
    //     storyDisplay.Delete();
    // }

    public void OnButtonClick() // edit button
    {
        TMP_Text storyTitleText = GetComponentInChildren<TMP_Text>();
        string storyTitle = (storyTitleText != null) ? storyTitleText.text : "Default Title";

        PlayerPrefs.SetString("StoryTitle", storyTitle);
        PlayerPrefs.SetString("ChapterID", chapterID); 

        GameObject editStoryObject = Instantiate(editStoryPrefab);


        TMP_InputField storyTitleInputField = editStoryObject.GetComponentInChildren<TMP_InputField>(); // for story title
        InputField chapterInputField = editStoryObject.GetComponentInChildren<InputField>(); // for chap id

        if (storyTitleInputField != null)
        {
            storyTitleInputField.text = storyTitle;
        }
        else
        {
            Debug.LogError("TMP_InputField for story title not found in the EditStory prefab.");
        }

        if (chapterInputField != null)
        {
            chapterInputField.text = chapterID.ToString(); 
        }
        else
        {
            Debug.LogError("TMP_InputField for chapter ID not found in the EditStory prefab.");
        }

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            DatabaseReference databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

            databaseReference.Child("Story").Child(storyTitle).Child("Dialogue").GetValueAsync().ContinueWithOnMainThread(dialogueTask =>
            {
                if (dialogueTask.IsFaulted)
                {
                    Debug.LogError("Failed to retrieve Dialogue: " + dialogueTask.Exception);
                    return;
                }

                if (dialogueTask.IsCompleted)
                {
                    DataSnapshot dialogueSnapshot = dialogueTask.Result;

                    StringBuilder dialogueLinesBuilder = new StringBuilder();

                    foreach (DataSnapshot childSnapshot in dialogueSnapshot.Children)
                    {
                        string dialogueLine = childSnapshot.Value.ToString();
                        dialogueLinesBuilder.Append(dialogueLine).Append("\n");
                    }

                    string dialogueLines = dialogueLinesBuilder.ToString().Trim();

                    Debug.Log("Dialogue Lines: \n" + dialogueLines);

                    TMP_InputField dialogueInputField = editStoryObject.transform.Find("DialogueInputField").GetComponent<TMP_InputField>();

                    if (dialogueInputField != null)
                    {
                        dialogueInputField.text = dialogueLines;
                    }
                    else
                    {
                        Debug.LogError("DialogueInputField not found in the EditStory prefab.");
                    }
                }
            });
        });

        editStoryObject.SetActive(true);
    }
}







