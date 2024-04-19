using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using TMPro;

public class FirebaseDataHandler : MonoBehaviour
{
    public TMP_InputField titleInputField;
    public TMP_InputField storyInputField;
    public InputField chapterNumberInputField;
    public Button saveButton;
    public Button deleteButton;
    public Text statusText; 

    private DatabaseReference databaseReference;
    private int dialogueID = 1;

    private void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        });

        saveButton.onClick.AddListener(SaveData);
        deleteButton.onClick.AddListener(DeleteData);
    }

    private void SaveData()
    {
        string title = titleInputField.text;
        string chapterNumber = chapterNumberInputField.text;
        string fullStory = storyInputField.text;

        if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(chapterNumber) && !string.IsNullOrEmpty(fullStory))
        {
            string[] storyLines = fullStory.Split('\n');

            // save title and chapter number under "Story"
            DatabaseReference storyRef = databaseReference.Child("Story").Child(title);
            storyRef.Child("ChapterID").SetValueAsync(chapterNumber);

            // clear existing dialogue entries
            storyRef.Child("Dialogue").RemoveValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    DatabaseReference dialogueRef = storyRef.Child("Dialogue");

                    int dialogueID = 1;

                    foreach (string line in storyLines)
                    {
                        if (!string.IsNullOrEmpty(line))
                        {
                            // unique key each line
                            string dialogueKey = dialogueID.ToString();

                            // save each line under "Dialogue", dialogueKey as uid
                            dialogueRef.Child(dialogueKey).SetValueAsync(line);

                            dialogueID++;
                        }
                    }

                    titleInputField.text = "";
                    chapterNumberInputField.text = "";
                    storyInputField.text = "";

                    UpdateStatus("Data saved to Firebase under 'Story' node!");
                }
                else
                {
                    UpdateStatus("Failed to remove existing dialogue entries.");
                }
            });
        }
        else
        {
            UpdateStatus("Title, chapter number, and story fields must not be empty.");
        }
    }

    private void DeleteData()
    {
        string titleToDelete = titleInputField.text;

        if (!string.IsNullOrEmpty(titleToDelete))
        {
            // Delete the entire title entry including its children
            DatabaseReference storyRef = databaseReference.Child("Story").Child(titleToDelete);
            storyRef.RemoveValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    titleInputField.text = "";
                    chapterNumberInputField.text = "";
                    storyInputField.text = "";

                    UpdateStatus("Data deleted from Firebase under 'Story' node for title: " + titleToDelete);
                }
                else
                {
                    UpdateStatus("Failed to delete data under 'Story' node for title: " + titleToDelete);
                }
            });
        }
        else
        {
            UpdateStatus("Title must not be empty for deletion.");
        }
    }

    // Helper method to update the status text
    private void UpdateStatus(string message)
    {
        statusText.text = message;
    }
}

[System.Serializable]
public class EntryData
{
    public string dialogue;

    public EntryData(string dialogue)
    {
        this.dialogue = dialogue;
    }
}
