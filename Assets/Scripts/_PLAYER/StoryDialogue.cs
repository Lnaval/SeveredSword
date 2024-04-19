using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Auth;


public class StoryDialogue : MonoBehaviour
{
    private FirebaseAuth auth;
    private DatabaseReference reference;
    private DependencyStatus dependencyStatus;

    private DialogueSystem ds;
    private TextArchitect architect;
    private string chapterProgress;
    private int dialogueCounter;
    private long totalDialogue;
    private List<string> dialogue = new List<string>();
    string checkDialogue;

    private PlayerManager playerManager;
    private PuzzleManagerPlayer puzzleManagerPlayer;
    //private GameObject puzzleButton;
    private PlayerUI playerUI;
    private ChapterRandomize chapterRandomize;


    string chapterTitle;
    int puzzleSolved;
    long chapTotal;

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        
        ds = DialogueSystem.instance;
        architect = new TextArchitect(ds.dialogueContainer.dialogueText);
        architect.buildMethod = TextArchitect.BuildMethod.typewriter;

        playerManager = GameObject.FindGameObjectWithTag("PlayerManager").GetComponent<PlayerManager>();
        puzzleManagerPlayer = GameObject.FindGameObjectWithTag("PuzzleManagerPlayer").GetComponent<PuzzleManagerPlayer>();
        playerUI = GameObject.FindGameObjectWithTag("PlayerUI").GetComponent<PlayerUI>();
        chapterRandomize = GameObject.FindGameObjectWithTag("ChapterRandomize").GetComponent<ChapterRandomize>();

        GetSeedFunc();
        StartCoroutine(GetChapTotal());
    }

    public void GetSeedFunc()
    {
        StartCoroutine(GetSeed());


    }

    private IEnumerator GetSeed()
    {
        Debug.Log("Get Seed Works");
        var res = reference.Child("User").Child(auth.CurrentUser.UserId).Child("Chapter Seed").OrderByKey().GetValueAsync();

        yield return new WaitUntil(predicate: () => res.IsCompleted);
        if (res.Exception != null)
        {
            Debug.LogWarning(message: $"Failed due to {res.Exception}");
        }
        else 
        {
            DataSnapshot snapshot = res.Result;

            if (snapshot.Exists)
            {
                foreach(DataSnapshot childSnap in snapshot.Children)
                {
                    int newChapterSeed = int.Parse(childSnap.Value.ToString());
                    if (MainManager.Instance.chapterSeed.Contains(newChapterSeed)) //checks if chapterseed already contains seed retrieved
                    {
                        break;
                    }
                    else
                    {
                        MainManager.Instance.chapterSeed.Add(newChapterSeed);
                    }
                }
                LoadChapter();
            }
            else
            {
                chapterRandomize.GetShouldRandomize();
            }
            
        } // ONCE seed is retrieved > get chapter progress > load chapter based on what chapter user is in > load qr base on chapter > load puzzle based on chapter
    }

    public void LoadChapter()
    {
        StartCoroutine(LoadChapterProgress());
    }

    private IEnumerator LoadChapterProgress()
    {
        Debug.Log("Load Chapter Works");

        Debug.Log(auth.CurrentUser.UserId);
        var res = reference.Child("User").Child(auth.CurrentUser.UserId).Child("Chapter Progress").GetValueAsync();
        yield return new WaitUntil(predicate: () => res.IsCompleted);

        if (res.Exception != null)
        {
            Debug.LogWarning(message: $"Failed due to {res.Exception}");
            Debug.Log("exception not null");
        }
        else 
        {
            DataSnapshot snap = res.Result;

            if (snap.Exists)
            {
                chapterProgress = snap.Value.ToString();
                Debug.Log(chapterProgress);
                Debug.Log(MainManager.Instance.chapterSeed.Count);
                // if (int.Parse(chapterProgress) > MainManager.Instance.chapterSeed.Count)
                // {
                //     //chapter done
                //     Debug.Log("Congratulations!");
                // }
                // else
                // {
                    Debug.Log(MainManager.Instance.chapterSeed[int.Parse(chapterProgress)-1].ToString());

                    GetStory(MainManager.Instance.chapterSeed[int.Parse(chapterProgress)-1].ToString());
                    MainManager.Instance.current_Chapter = int.Parse(chapterProgress);
                    Debug.Log("chapter progress exists");
                // }
            }
            else
            {
                Debug.Log("chapter progress doesnt exist");
                MainManager.Instance.chapterSeed[0].ToString();
                GetStory(MainManager.Instance.chapterSeed[0].ToString());
            }
        }
    }

    public void GetStory(string chapter)
    {
        StartCoroutine(GetStoryTitle(chapter));
    }

    
    private IEnumerator GetStoryTitle(string chapter)
    {
        Debug.Log("Get Story Title Works");
        Debug.Log(chapter);
        var res = reference.Child("Story").OrderByChild("ChapterID").EqualTo(chapter).GetValueAsync();
        yield return new WaitUntil(predicate: () => res.IsCompleted);
        if (res.Exception != null)
        {
            Debug.LogWarning(message: $"Failed due to {res.Exception}");
        }
        else 
        {
            DataSnapshot snapshot = res.Result;
            foreach (DataSnapshot childSnap in snapshot.Children)
            {
                chapterTitle = childSnap.Key.ToString();
                Debug.Log(chapterTitle);
            }

            StartCoroutine(GetDialogue(chapterTitle));
        }
    }

    private IEnumerator GetDialogue(string chapterTitle)
    {
        Debug.Log("Get Dialogue Works");
        Debug.Log(chapterTitle);

        var res = reference.Child("Story").Child(chapterTitle).Child("Dialogue").GetValueAsync();
        yield return new WaitUntil(predicate: () => res.IsCompleted);
        if (res.Exception != null)
        {
            Debug.LogWarning(message: $"Failed due to {res.Exception}");
        }
        else 
        {
            DataSnapshot snapshot = res.Result;

            foreach(DataSnapshot child in snapshot.Children)
            {
                string newDialogue = child.Value.ToString();
                Debug.Log(newDialogue);
                dialogue.Add(newDialogue);
            }
            StartCoroutine(GetPuzzleSolved());
        }
    }

    public IEnumerator GetPuzzleSolved()
    {
        Debug.Log("Get Puzzle Solved Works");
        var res = reference.Child("User").Child(auth.CurrentUser.UserId).Child("Puzzle Solved").GetValueAsync();
        yield return new WaitUntil(predicate: () => res.IsCompleted);
        if (res.Exception != null)
        {
            Debug.LogWarning(message: $"Failed due to {res.Exception}");
        }
        else 
        {
            DataSnapshot snapshot = res.Result;

            if (snapshot.Exists)
            {
                puzzleSolved = int.Parse(snapshot.Value.ToString());
                Debug.Log("Puzzle Solved" + puzzleSolved);
            }
            else
            {
                puzzleSolved = 0;
            }
            StartCoroutine(GetDialogueProgress());
        }
    }

    public IEnumerator GetDialogueProgress()
    {
        Debug.Log("Get Dialogue Progress Works");
        var res = reference.Child("User").Child(auth.CurrentUser.UserId).Child("Dialogue Progress").GetValueAsync();
        yield return new WaitUntil(predicate: () => res.IsCompleted);
        if (res.Exception != null)
        {
            Debug.LogWarning(message: $"Failed due to {res.Exception}");
        }
        else 
        {
            DataSnapshot snapshot = res.Result;

            if (snapshot.Exists)
            {
                dialogueCounter = int.Parse(snapshot.Value.ToString());
                Debug.Log(dialogueCounter);

                // dialogue[dialogueCounter] = dialogue[dialogueCounter].Replace("***","");
                // dialogue[dialogueCounter] = dialogue[dialogueCounter].Replace("[]","");
                // architect.Build(dialogue[dialogueCounter]);
            }
            else
            {
                dialogueCounter = 0;
            }

            playerUI.chapterText.text = chapterTitle;
            playerManager.CallPuzzleFunctions();
            //playerManager.CallQrFunctions();
        }
    }

    public IEnumerator GetChapTotal()
    {
        var res = reference.Child("Story").GetValueAsync();
        yield return new WaitUntil(predicate: () => res.IsCompleted);
        if (res.Exception != null)
        {
            Debug.LogWarning(message: $"Failed due to {res.Exception}");
        }
        else 
        {
            DataSnapshot snap = res.Result;
            if (snap.Exists)
                chapTotal = snap.ChildrenCount;
        }
    }
    

    void DisplayExpression(string text)
    {
        playerUI.character.SetActive(false);
        playerUI.character1.SetActive(false);
        playerUI.character2.SetActive(false);
        playerUI.character3.SetActive(false);
        playerUI.character4.SetActive(false);
        playerUI.character5.SetActive(false);

        if (text.Length > 4)
        {
            string expression = text.Substring(0,5);
            switch(expression)
            {
                case "||1||":
                    playerUI.character1.SetActive(true);
                    checkDialogue = text.Replace("||1||","");
                    break;
                case "||2||":
                    playerUI.character2.SetActive(true);
                    checkDialogue = text.Replace("||2||","");
                    break;
                case "||3||":
                    playerUI.character3.SetActive(true);
                    checkDialogue = text.Replace("||3||","");
                    break;
                case "||4||":
                    playerUI.character4.SetActive(true);
                    checkDialogue = text.Replace("||4||","");
                    break;
                case "||5||":
                    playerUI.character5.SetActive(true);
                    checkDialogue = text.Replace("||5||","");
                    break;
                default:
                    playerUI.character.SetActive(true);
                    break;
            }
        }
    }

    public void Next()
    {
        if (!architect.isBuilding)
        {
            Debug.Log(dialogue.Count + " " + dialogueCounter);
            if(dialogue.Count == dialogueCounter)
            {
                Debug.Log("Puzzle Solved" + puzzleSolved);
                Debug.Log("Current CHapter" + MainManager.Instance.current_Chapter);
                if (puzzleSolved == MainManager.Instance.current_Chapter || MainManager.Instance.current_Chapter == chapTotal)
                    puzzleManagerPlayer.ChangeChapter();
            }
            else
            {
                checkDialogue = dialogue[dialogueCounter];
                if(dialogue[dialogueCounter].Contains("***"))
                {
                    //build only after all adnuculi is collected
                    if (playerManager.allCollected)
                    {
                        Debug.Log(dialogue[dialogueCounter]);
                        Debug.Log(checkDialogue);

                        DisplayExpression(dialogue[dialogueCounter]);
                        architect.Build(checkDialogue.Replace("***",""));
                        dialogueCounter++;
                        playerManager.StoreDialogueProgress(dialogueCounter);
                    }
                }
                else if(dialogue[dialogueCounter].Contains("[]"))
                {
                    Debug.Log("after puzzle answer");

                    if (puzzleSolved == MainManager.Instance.current_Chapter)
                    {
                        playerUI.buttonPuzzle.SetActive(false);
                        
                        Debug.Log(dialogue[dialogueCounter]);
                        Debug.Log(checkDialogue);

                        DisplayExpression(dialogue[dialogueCounter]);
                        architect.Build(checkDialogue.Replace("[]",""));

                        dialogueCounter++;
                        playerManager.StoreDialogueProgress(dialogueCounter);
                    }
                }
                else
                {
                    Debug.Log(dialogue[dialogueCounter]);
                    Debug.Log(checkDialogue);
                    
                    DisplayExpression(dialogue[dialogueCounter]);
                    architect.Build(checkDialogue);
                    dialogueCounter++;
                    playerManager.StoreDialogueProgress(dialogueCounter);
                }
            }
        } 
        else 
        {
            if (!architect.hurryUp)
                architect.hurryUp = true;
            else
                architect.ForceComplete();
        }
    }

    public void Previous()
    {
        if (dialogueCounter > 1)
        {
            if (!architect.isBuilding)
            {
                checkDialogue = dialogue[dialogueCounter-2];
                if (dialogue[dialogueCounter-2].Contains("***"))
                {
                    checkDialogue = dialogue[dialogueCounter-2].Replace("***","");
                }
                else if (dialogue[dialogueCounter-2].Contains("[]"))
                {
                    checkDialogue = dialogue[dialogueCounter-2].Replace("[]","");
                }
                
                Debug.Log(checkDialogue);
                
                DisplayExpression(checkDialogue);
                architect.Build(checkDialogue);
                
                dialogueCounter--;
                playerManager.StoreDialogueProgress(dialogueCounter);
            } 
            else
            {
                if (!architect.hurryUp)
                        architect.hurryUp = true;
                else
                    architect.ForceComplete();
            }
        }
    }
}
