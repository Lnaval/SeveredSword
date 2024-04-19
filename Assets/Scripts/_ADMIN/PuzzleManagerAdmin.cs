using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;

public class PuzzleManagerAdmin : MonoBehaviour
{
    // [SerializeField] private GameObject puzzleElement;
    // [SerializeField] private Transform puzzleContent;
    // private DatabaseReference reference;


    // private IEnumerator LoadPuzzleData()
    // {
    //     Debug.Log("load function done");
    //     var dbTask = reference.Child("Puzzle").GetValueAsync();
    //     yield return new WaitUntil(predicate: () => dbTask.IsCompleted);

    //     Debug.Log("load function wait");
    //     if (dbTask.Exception != null)
    //     {
    //         Debug.LogWarning(message: $"Failed due to {dbTask.Exception}");
    //     }
    //     else 
    //     {
    //         DataSnapshot qrSnapshot = dbTask.Result;
    //         Debug.Log(qrSnapshot);
    //         foreach (Transform child in puzzleContent.transform)
    //         {
    //             Destroy(child.gameObject);
    //         }

    //         foreach (DataSnapshot childSnap in qrSnapshot.Children)
    //         {
    //             Debug.Log(childSnap);
    //             string chapID = childSnap.Key.ToString(); 
    //             string answer = childSnap.Child("Answer").Value.ToString();

    //             GameObject puzzleListElement = Instantiate(puzzleElement, puzzleContent); 
    //             puzzleListElement.GetComponent<PuzzleElement>().NewPuzzleElement(answer, chapID);
    //         }
    //     }
    // }

    // public void LoadPuzzles()
    // {
    //     StartCoroutine(LoadPuzzleData());
    // }

    // void Start()
    // {
    //     reference = FirebaseDatabase.DefaultInstance.RootReference;
    //     Debug.Log("reference decl");
    //     LoadPuzzles();
    // }
}
