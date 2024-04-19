using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using TMPro;
using UnityEngine.SceneManagement;

public class PuzzleManagerPlayer : MonoBehaviour
{
    private DatabaseReference reference;
    [SerializeField] private TMP_InputField userAnswer;
    [SerializeField] private TMP_Text resultField;
    //[SerializeField] private TMP_Text verificationText;
    [SerializeField] private GameObject playerHandlerObject;

    PlayerManager playerManager;
    PlayerUI playerUI;

    private IEnumerator LoadPuzzleAnswer()
    {
        //handles getting the correct answer to the current puzzle
        string chapter = MainManager.Instance.chapterSeed[MainManager.Instance.current_Chapter-1].ToString();
        Debug.Log(chapter);

        var res = reference.Child("Puzzle").Child(chapter).Child("Answer").GetValueAsync();
        yield return new WaitUntil(predicate: () => res.IsCompleted);
        if (res.Exception != null)
        {
            Debug.LogWarning(message: $"Failed due to {res.Exception}");
        }
        else 
        {
            DataSnapshot snap = res.Result;
            string correctAnswer = snap.Value.ToString();
            CheckAnswer(userAnswer.text.ToString(), correctAnswer);
        }
    }

    void CheckAnswer(string userAnswer, string correctAnswer)
    {
        //checks if the user's answer is correct, empty, or incorrect
        if (userAnswer != null)
        {
            if(userAnswer.ToLower() == correctAnswer.ToLower())
            {
                //store solved puzzles in player DB
                playerManager.StorePuzzleSolved(MainManager.Instance.current_Chapter);
                
                //activate swordpiece acquired popup
                playerUI.puzzlePopUp.SetActive(false);
                playerUI.swordPiece.SetActive(true);
            } 
            else
            {
                //do something to handle incorrect answer
                resultField.text = "Incorrect Answer";
            }
        } 
        else
        {
            resultField.text = "Insert Answer";
        }
    }

    public void ChangeChapter()
    {
        //store chapter progress and resets dialogue progress to 0
        MainManager.Instance.current_Chapter += 1;   
        int storeChap = MainManager.Instance.current_Chapter;
        int totalChapter = MainManager.Instance.chapterSeed.Count;
        Debug.Log("Chapter Progress: " + storeChap);
        Debug.Log("Total Chapter: " + totalChapter);

        if(storeChap > totalChapter)
        {
            //trigger animation signalling the end of the game
            playerManager.Completion(true);
            MainManager.Instance.current_Chapter = MainManager.Instance.chapterSeed.Count;
            Debug.Log("Chapter progress is greater than chapter count");

            playerUI.loadingUi.SetActive(true);
            playerUI.endingUi.SetActive(true);
        }
        else
        {
            playerManager.StoreChapterProgress(storeChap);
            playerManager.StoreDialogueProgress(0);
            SceneManager.LoadSceneAsync("PlayerSide");
        }
    }

    public void OnClickSubmit(){
        StartCoroutine(LoadPuzzleAnswer());
    }

    public void OnClickReturn()
    {
        resultField.text = "Puzzle Answer";
        userAnswer.text = "";
    }

    void Start()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        playerManager = playerHandlerObject.GetComponent<PlayerManager>();
        playerUI = GameObject.FindGameObjectWithTag("PlayerUI").GetComponent<PlayerUI>();
    }
}
