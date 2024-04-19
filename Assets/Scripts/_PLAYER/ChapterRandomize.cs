using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Auth;

public class ChapterRandomize : MonoBehaviour
{
    private DatabaseReference reference;

    private long totalChapter;
    bool shouldRandomize;

    PlayerManager playerManager;
    StoryDialogue storyDialogue;

    void Start()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        playerManager = GameObject.FindGameObjectWithTag("PlayerManager").GetComponent<PlayerManager>();
        storyDialogue = GameObject.FindGameObjectWithTag("StoryDialogue").GetComponent<StoryDialogue>();
    }

    public void GetShouldRandomize()
    {
        StartCoroutine(GetShouldRandomizeInDb());
    }

    private IEnumerator GetShouldRandomizeInDb()
    {
        Debug.Log("Get Should Randomize Works");
        var resRandom = reference.Child("Admin").Child("Randomize").GetValueAsync();
        yield return new WaitUntil(predicate: () => resRandom.IsCompleted);

        DataSnapshot snapshotRandom;
        if (resRandom.Exception != null)
        {
            Debug.LogWarning(message: $"Failed due to {resRandom.Exception}");
        }
        else 
        {
            snapshotRandom = resRandom.Result;
            shouldRandomize = Convert.ToBoolean(snapshotRandom.Value);
        }

        var resChapterCount = reference.Child("Story").GetValueAsync();
        yield return new WaitUntil(predicate: () => resChapterCount.IsCompleted);
        
        DataSnapshot snapshotTotalChap;
        if (resChapterCount.Exception != null)
        {
            Debug.LogWarning(message: $"Failed due to {resRandom.Exception}");
        }
        else
        {
            snapshotTotalChap = resChapterCount.Result;
            totalChapter = snapshotTotalChap.ChildrenCount;
        }

        ShouldRandomize(shouldRandomize, totalChapter);
    }


    void ShouldRandomize(bool randomize, long totalChapter)
    {
        int[] seed = new int[totalChapter];
        if(randomize == false)
        {
            seed = NotRandomSeed((int)totalChapter);
        }
        else
        {
            seed = RandomizeChapter((int)totalChapter);
        }
        playerManager.StoreChapterSeed(seed);
        storyDialogue.LoadChapter();
    }

    int[] RandomizeChapter(int totalChapter)
    {
        List<int> availableDigits = Enumerable.Range(1, totalChapter).ToList();
        System.Random random = new System.Random();
        int[] seed = new int[totalChapter];
        
        for (int i = 0; i < totalChapter; i++)
        {
            int randomIndex = random.Next(availableDigits.Count);
            seed[i] = availableDigits[randomIndex];
            MainManager.Instance.chapterSeed.Add(seed[i]);
            availableDigits.RemoveAt(randomIndex);
            Debug.Log(seed[i]);
        }
        return seed;
    }

    int[] NotRandomSeed(long totalChapter)
    {
        int[] seed = new int[totalChapter];
        for(int i = 0; i< totalChapter; i++)
        {
            seed[i] = i+1;
            MainManager.Instance.chapterSeed.Add(seed[i]);
        }
        return seed;
    }
}
