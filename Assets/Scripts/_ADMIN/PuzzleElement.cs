using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class PuzzleElement : MonoBehaviour
{
    [SerializeField] private TMP_Text correctAnswer;
    [SerializeField] private TMP_Text chapId;
    [SerializeField] private GameObject buttonCallObject;

    public void NewPuzzleElement(string answer, string chapterId)
    {
        correctAnswer.text = answer;
        chapId.text = chapterId;
    }

    public string CallFunction()
    {
        ButtonText1 btxt = buttonCallObject.GetComponent<ButtonText1>();
        return btxt.GetQrName();
    }

    PuzzleDisplayIndiv puzzleDisplayIndiv;
    TrySetActive trySetActive;

    public void OnClickEdit(){
        string editChildKey = CallFunction();
        Debug.Log(editChildKey);

        puzzleDisplayIndiv = GameObject.FindGameObjectWithTag("PuzzleDisplayIndiv").GetComponent<PuzzleDisplayIndiv>();
        puzzleDisplayIndiv.GetPuzzle(editChildKey);

        trySetActive = GameObject.FindGameObjectWithTag("UIHandler").GetComponent<TrySetActive>();
        trySetActive.PuzzleDisplayIndiv();
    }
}
