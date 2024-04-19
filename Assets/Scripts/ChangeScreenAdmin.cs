using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeScreenAdmin : MonoBehaviour
{
    private StoryDisplay storyDisplay;
    public void ShowStoryList()
    {
        storyDisplay = GameObject.FindGameObjectWithTag("StoryDisplay").GetComponent<StoryDisplay>();

        storyDisplay.GetStory();
    }
}
