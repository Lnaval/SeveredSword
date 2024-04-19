using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Uninteractable : MonoBehaviour
{
    public CanvasGroup group;
    public CanvasGroup popup;
    
    // Start is called before the first frame update
    void Start()
    {
        group.interactable = false;
        popup.interactable = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void onClick(){
        popup.interactable = false;
    }
}
