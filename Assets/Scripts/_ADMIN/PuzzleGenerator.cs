using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Firebase;
using Firebase.Database;
using Firebase.Auth;


public class PuzzleGenerator : MonoBehaviour
{
    [SerializeField] private TMP_InputField correctAnswer;
    [SerializeField] private TMP_InputField chapterId;
    
    private TrySetActive trySetActive;
    private PuzzleDisplay puzzleDisplay;
    [SerializeField] private GameObject puzzleDisplayObject;
    [SerializeField] private GameObject confirmationPopUp;
    ConfirmationPopUp confirmationPopUpScr;



    private DatabaseReference reference;
    private DependencyStatus dependencyStatus;

    private IEnumerator SavePuzzleAnswerToDb(string chapter)
    {
        string answer = correctAnswer.text.ToString();
        Debug.Log(chapter + answer);
        var res = reference.Child("Puzzle").Child(chapter).Child("Answer").SetValueAsync(answer);

        yield return new WaitUntil(predicate: () => res.IsCompleted);
        if (res.Exception != null)
        {
            Debug.LogWarning(message: $"Failed due to {res.Exception}");
            confirmationPopUpScr.CheckStatus(res.Exception.ToString());
            confirmationPopUp.SetActive(true);
        }
        else 
        {
            //successfully saved, add action on what to do next after saving
            Debug.Log("Saved");
            confirmationPopUpScr.CheckStatus("Puzzle Saved");
            confirmationPopUp.SetActive(true);
        }
    }

    public void OnClickStoreCorrectAnswer()
    {
        if(CheckInputFields() == true)
        {
            int chapterID;
            if (int.TryParse(chapterId.text, out chapterID))
            {
                StartCoroutine(SavePuzzleAnswerToDb(chapterId.text.ToString()));
            } else {
                confirmationPopUpScr.CheckStatus("Invalid chapter ID input. Must be a number");
                confirmationPopUp.SetActive(true);
            }
        } else {
            confirmationPopUpScr.CheckStatus("All data fields must contain value");
            confirmationPopUp.SetActive(true);
        }
    }

    private bool CheckInputFields()
    {
        if(correctAnswer.text == "" || chapterId.text == "")
        {
            Debug.LogError("All data fields must contain value");
            return false;
        } 
        else
        {
            return true;
        }
    }

    public void OnClickViewAll()
    {
        trySetActive.PuzzleDisplayAll();

        correctAnswer.text = "";
        chapterId.text = "";
    }  

    void Start()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        trySetActive = GameObject.FindGameObjectWithTag("UIHandler").GetComponent<TrySetActive>();
        confirmationPopUpScr = GameObject.FindGameObjectWithTag("ConfirmPopUp").GetComponent<ConfirmationPopUp>();
    }
}
