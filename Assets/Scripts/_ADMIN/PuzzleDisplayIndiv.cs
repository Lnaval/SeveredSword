using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Firebase;
using Firebase.Database;

public class PuzzleDisplayIndiv : MonoBehaviour
{
    [SerializeField] private TMP_Text chapter;
    [SerializeField] private TMP_InputField answer;
    private DatabaseReference reference;
    private string storeChapter;
    private TrySetActive trySetActive;
    private PuzzleDisplay puzzleDisplay;
    
    [SerializeField] private GameObject confirmationPopUp;
    ConfirmationPopUp confirmationPopUpScr;

    public void GetPuzzle(string chapter)
    {
        storeChapter = chapter;
        Debug.Log(chapter);
        StartCoroutine(GetIndivPuzzle(chapter));
    }

    private IEnumerator GetIndivPuzzle(string chapter)
    {
        var dbTask = reference.Child("Puzzle").Child(chapter).GetValueAsync();
        yield return new WaitUntil(predicate: () => dbTask.IsCompleted);

        if (dbTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed due to {dbTask.Exception}");
        }
        else 
        {
            DataSnapshot qrSnapshot = dbTask.Result;
            Debug.Log(qrSnapshot);
            string chap = qrSnapshot.Key.ToString();
            string ans = qrSnapshot.Child("Answer").Value.ToString();
            LoadIndivPuzzle(chap, ans);
        }
    }

    private void LoadIndivPuzzle(string chap, string ans)
    {
        chapter.text = chap;
        answer.text = ans;
    }

    private IEnumerator UpdatePuzzle(string chap, string ans)
    {
        var dbTask = reference.Child("Puzzle").Child(chap).Child("Answer").SetValueAsync(ans);

        yield return new WaitUntil(predicate: () => dbTask.IsCompleted);

        if (dbTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed due to {dbTask.Exception}");
            confirmationPopUpScr.CheckStatus(dbTask.Exception.ToString());
            confirmationPopUp.SetActive(true);

        }
        else 
        {
            Debug.Log("Updated");
            confirmationPopUpScr.CheckStatus("Puzzle Updated");
            confirmationPopUp.SetActive(true);
        }
    }

    private IEnumerator DeletePuzzle(string chap)
    {
        var dbTask = reference.Child("Puzzle").Child(chap).RemoveValueAsync();
        yield return new WaitUntil(predicate: () => dbTask.IsCompleted);

        if (dbTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed due to {dbTask.Exception}");
            confirmationPopUpScr.CheckStatus(dbTask.Exception.ToString());
            confirmationPopUp.SetActive(true);
        }
        else 
        {
            Debug.Log("Deleted");
            confirmationPopUpScr.CheckStatus("Puzzle Deleted");
            confirmationPopUp.SetActive(true);
        }
    }

    public void OnClickUpdate()
    {
        StartCoroutine(UpdatePuzzle(storeChapter, answer.text.ToString()));
        puzzleDisplay.LoadPuzzles();
        trySetActive.PuzzleDisplayAll();
    }

    public void OnClickPrevious()
    {
        trySetActive.PuzzleDisplayAll();
    }

    public void OnClickDelete()
    {
        StartCoroutine(DeletePuzzle(storeChapter));
        puzzleDisplay.LoadPuzzles();
        trySetActive.PuzzleDisplayAll();
    }

    
    void Start()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        trySetActive = GameObject.FindGameObjectWithTag("UIHandler").GetComponent<TrySetActive>();
        puzzleDisplay = GameObject.FindGameObjectWithTag("PuzzleDisplay").GetComponent<PuzzleDisplay>();
        confirmationPopUpScr = GameObject.FindGameObjectWithTag("ConfirmPopUp").GetComponent<ConfirmationPopUp>();
    }
}
