using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    public CanvasGroup puzzleCanvas;
    
    void OnEnable()
    {
        puzzleCanvas.alpha = 0;
        puzzleCanvas.LeanAlpha(1, 0.3f);
    }

    public void Close()
    {
        puzzleCanvas.LeanAlpha(0, 0.3f).setOnComplete(OnComplete);
    }

    void OnComplete()
    {
        gameObject.SetActive(false);
    }
}
