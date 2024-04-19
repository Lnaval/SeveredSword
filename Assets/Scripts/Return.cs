using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Return : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Check if the "Back" button (KeyCode.Escape) is pressed
            Application.Quit();
        }
    }
}
