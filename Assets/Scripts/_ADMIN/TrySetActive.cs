using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using UnityEngine.SceneManagement;


public class TrySetActive : MonoBehaviour
{
    public GameObject generateScreenQR;
    public GameObject displayScreenQR;
    public GameObject generateScreenPuzzle;
    public GameObject displayScreenPuzzleAll;
    public GameObject displayScreenPuzzleIndiv;
    public GameObject adminMainScreen;

    private QrDisplay qrDisp;
    private PuzzleDisplay puzzleDisp;
    private StoryDisplay storyDisplay;

    private FirebaseAuth auth;

    public void QRDisplayScreen()
    {
        adminMainScreen.SetActive(false);
        generateScreenQR.SetActive(false);
        generateScreenPuzzle.SetActive(false);
        displayScreenPuzzleAll.SetActive(false);
        displayScreenPuzzleIndiv.SetActive(false);
        displayScreenQR.SetActive(true);
        qrDisp.GetQrInfo();
    }

    public void PuzzleDisplayAll()
    {
        adminMainScreen.SetActive(false);
        generateScreenPuzzle.SetActive(false);
        displayScreenQR.SetActive(false);
        generateScreenQR.SetActive(false);
        displayScreenPuzzleIndiv.SetActive(false);
        displayScreenPuzzleAll.SetActive(true);

        puzzleDisp.LoadPuzzles();
    }

    public void PuzzleDisplayIndiv()
    {
        generateScreenPuzzle.SetActive(false);
        displayScreenQR.SetActive(false);
        generateScreenQR.SetActive(false);
        adminMainScreen.SetActive(false);
        displayScreenPuzzleIndiv.SetActive(true);
    }
    public void AdminSignOut()
    {
        auth.SignOut();
        SceneManager.LoadScene("Authentication");
    }

    public void AdminMain()
    {
        displayScreenPuzzleAll.SetActive(false);
        generateScreenPuzzle.SetActive(false);
        displayScreenQR.SetActive(false);
        generateScreenQR.SetActive(false);
        displayScreenPuzzleIndiv.SetActive(false);
        adminMainScreen.SetActive(true);
    }

    public void ShowStoryList()
    {
        storyDisplay = GameObject.FindGameObjectWithTag("StoryDisplay").GetComponent<StoryDisplay>();

        storyDisplay.GetStory();
    }

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;

        qrDisp = GameObject.FindGameObjectWithTag("QrDisplay").GetComponent<QrDisplay>();
        puzzleDisp = GameObject.FindGameObjectWithTag("PuzzleDisplay").GetComponent<PuzzleDisplay>();
        AdminMain();
    }
}
