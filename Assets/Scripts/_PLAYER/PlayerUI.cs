using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Google;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    public TMP_Text collectedQrText;
    public TMP_Text puzzleSolvedText;
    public TMP_Text chapterText;

    PlayerManager playerManager;
    StoryDialogue storyDialogue;

    public GameObject buttonPuzzle;
    public GameObject swordPiece;
    public GameObject puzzlePopUp;
    public GameObject loadingUi;
    public GameObject endingUi;

    public GameObject character;
    public GameObject character1;
    public GameObject character2;
    public GameObject character3;
    public GameObject character4;
    public GameObject character5;


    public void ScannerScreen()
    {
        SceneManager.LoadScene("Scanner");
    }

    public void MenuScreen()
    {
        SceneManager.LoadScene("Menu");
    }

    public void PlayerScreen()
    {
        SceneManager.LoadSceneAsync("PlayerSide");
    }

    void Start()
    {
        buttonPuzzle.SetActive(false);
    }
}
