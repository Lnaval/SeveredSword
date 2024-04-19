using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpInteractable : MonoBehaviour
{
    public CanvasGroup group;
    public Canvas background;
    
    // Start is called before the first frame update
    void Start()
    {
        background.enabled = false;
        background.enabled = true;
        group.interactable = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickBack(){
        group.interactable = true;
    }
}
