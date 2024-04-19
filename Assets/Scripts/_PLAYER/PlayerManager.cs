using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System;

public class PlayerManager : MonoBehaviour
{
    //database stuff
    private DatabaseReference reference;
    private FirebaseAuth auth;

    private long qrCollected;
    private long totalQr;
    private long puzzleSolved;
    private long totalPuzzle;
    private int neededQr;
    public bool allCollected;

    private GameObject puzzleButton;
    private PlayerUI playerUI;

    public void CallPuzzleFunctions()
    {
        StartCoroutine(PuzzleSolved());
    }

    private IEnumerator PuzzleSolved()
    {
        var res = reference.Child("User").Child(auth.CurrentUser.UserId).Child("Puzzle Solved").GetValueAsync();
        yield return new WaitUntil(predicate: () => res.IsCompleted);
        if (res.Exception != null)
        {
            Debug.LogWarning(message: $"Failed due to {res.Exception}");
        }
        else 
        {
            DataSnapshot snap = res.Result;
            Debug.Log(snap);

            if(snap.Exists)
            {
                puzzleSolved = int.Parse(snap.Value.ToString());
                Debug.Log(puzzleSolved);
            }
            else
            {
                puzzleSolved = 0;
            }
            
            StartCoroutine(TotalPuzzle());
        }
    }

    private IEnumerator TotalPuzzle()
    {

        var resultPuzzle = reference.Child("Puzzle").GetValueAsync();

        yield return new WaitUntil(predicate: () => resultPuzzle.IsCompleted);
        if (resultPuzzle.Exception != null)
        {
            Debug.LogWarning(message: $"Failed due to {resultPuzzle.Exception}");
        }
        else 
        {
            DataSnapshot snap = resultPuzzle.Result;
            Debug.Log(snap);
            if (snap.Exists)
            {
                totalPuzzle = snap.ChildrenCount;
                Debug.Log("Total Puzzle: " + totalPuzzle);
            }
            else //Puzzle Table is empty
            {
                totalPuzzle = 0;
            }

            StartCoroutine(Adnuculi());
        }
    }
    public void CallQrFunctions()
    {
        StartCoroutine(Adnuculi());
    }

    private IEnumerator Adnuculi()
    {
        var adCollected = reference.Child("User").Child(auth.CurrentUser.UserId).Child("QR Collected")
        .OrderByChild("ChapterID").EqualTo(MainManager.Instance.chapterSeed[MainManager.Instance.current_Chapter-1]).GetValueAsync();

        yield return new WaitUntil(predicate: () => adCollected.IsCompleted);
        if (adCollected.Exception != null)
        {
            Debug.LogWarning(message: $"Failed due to {adCollected.Exception}");
        }
        else 
        {
            DataSnapshot snapCollected = adCollected.Result;
            Debug.Log(snapCollected);

            if (snapCollected.Exists){
                qrCollected = snapCollected.ChildrenCount;
                Debug.Log("Collected QR: " + qrCollected);
            }
            else
            {
                qrCollected = 0;
            }
        }

        var totalAd = reference.Child("QR Table").OrderByChild("ChapterID")
        .EqualTo(MainManager.Instance.chapterSeed[MainManager.Instance.current_Chapter-1]).GetValueAsync();
        yield return new WaitUntil(predicate: () => totalAd.IsCompleted);
        if (totalAd.Exception != null)
        {
            Debug.LogWarning(message: $"Failed due to {totalAd.Exception}");
        }
        else 
        {
            DataSnapshot snapTotal = totalAd.Result;

            if (snapTotal.Exists)
            {
                totalQr = snapTotal.ChildrenCount;
            }
            else //QR Table is Empty
            {
                totalQr = 0;
            }
            
            SolveNeededQr(totalQr);
            CheckAdnuculiCollection();
        }
    }
    public void Display()
    {
        playerUI.puzzleSolvedText.text = puzzleSolved.ToString() + "/" + totalPuzzle.ToString();
        playerUI.collectedQrText.text = qrCollected.ToString() + "/" + neededQr.ToString();

        Debug.Log(puzzleSolved.ToString() + "/" + totalPuzzle.ToString());
        Debug.Log(qrCollected.ToString() + "/" + neededQr.ToString());

        playerUI.loadingUi.SetActive(false);
    }

    public void CheckAdnuculiCollection()
    {
        if (neededQr == qrCollected){
            Debug.Log("Qr Collected: " + qrCollected + " Needed: " + neededQr);
            if (playerUI != null)
            {
                Debug.Log("player ui");
                playerUI.buttonPuzzle.SetActive(true);
                Display();
                if (puzzleSolved == totalPuzzle || puzzleSolved == MainManager.Instance.current_Chapter)
                    playerUI.buttonPuzzle.SetActive(false);
            }
            else
            {
                Debug.Log("from scanner");
                SceneManager.LoadSceneAsync("PlayerSide");
            }
            allCollected = true;
        }
        else
        {
            Display();
            allCollected = false;
        }
    }

    void SolveNeededQr(long totalQr)
    {
        neededQr = (int)Math.Ceiling(totalQr * 0.8);
        Debug.Log("Needed Qr: " + neededQr);
    }

    public void StorePuzzleSolved(int solved)
    {
        StartCoroutine(SetPuzzleSolved(solved));
    }

    private IEnumerator SetPuzzleSolved(int solved)
    {
        var res = reference.Child("User").Child(auth.CurrentUser.UserId).Child("Puzzle Solved").SetValueAsync(solved);

        yield return new WaitUntil(predicate: () => res.IsCompleted);
        if (res.Exception != null)
        {
            Debug.LogWarning(message: $"Failed due to {res.Exception}");
        }
        else 
        {
            Debug.Log("success");
        }
    }

    public void StoreChapterProgress(int progress)
    {
        StartCoroutine(SetChapterProgress(progress));
    }

    private IEnumerator SetChapterProgress(int chapProg)
    {
        var res = reference.Child("User").Child(auth.CurrentUser.UserId).Child("Chapter Progress").SetValueAsync(chapProg);
        yield return new WaitUntil(predicate: () => res.IsCompleted);
        if (res.Exception != null)
        {
            Debug.LogWarning(message: $"Failed due to {res.Exception}");
        }
        else 
        {
            Debug.Log("success");
        }
    }

    public void StoreDialogueProgress(int dialogueProgress)
    {
        StartCoroutine(SetUserDialogueProgress(dialogueProgress));
    }

    private IEnumerator SetUserDialogueProgress(int dialogueProgress)
    {
        var res = reference.Child("User").Child(auth.CurrentUser.UserId).Child("Dialogue Progress").SetValueAsync(dialogueProgress);

        yield return new WaitUntil(predicate: () => res.IsCompleted);
        if (res.Exception != null)
        {
            Debug.LogWarning(message: $"Failed due to {res.Exception}");
        }
        else 
        {
            Debug.Log("success");
        }
    }
    
    public void StoreChapterSeed(int[] seed)
    {
        StartCoroutine(StoreChapterSeedRoutine(seed));
    }

    private IEnumerator StoreChapterSeedRoutine(int[] seed)
    {
        var res = reference.Child("User").Child(auth.CurrentUser.UserId).Child("Chapter Seed").SetValueAsync(seed);

        yield return new WaitUntil(predicate: () => res.IsCompleted);
        if (res.Exception != null)
        {
            Debug.LogWarning(message: $"Failed due to {res.Exception}");
        }
        else 
        {
            Debug.Log("success");
        }
    }
    
    public void Completion(bool progress)
    {
        StartCoroutine(StoreCompletion(progress));
    }

    private IEnumerator StoreCompletion(bool progress)
    {
        var res = reference.Child("User").Child(auth.CurrentUser.UserId).Child("Complete").SetValueAsync(progress);

        yield return new WaitUntil(predicate: () => res.IsCompleted);
        if (res.Exception != null)
        {
            Debug.LogWarning(message: $"Failed due to {res.Exception}");
        }
        else 
        {
            Debug.Log("success");
        }
    }
    void Start()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        auth = FirebaseAuth.DefaultInstance;

        playerUI = GameObject.FindGameObjectWithTag("PlayerUI").GetComponent<PlayerUI>();
    }
}
