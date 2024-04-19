using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour 
{
    public static UIManager instance;

    //Screen object variables
    public GameObject playerUI;
    public GameObject adminUI;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    //Functions to change the login screen UI
    public void PlayerScreen() //Back button
    {
        playerUI.SetActive(true);
        adminUI.SetActive(false);
    }
    public void AdminScreen() // Regester button
    {
        playerUI.SetActive(false);
        adminUI.SetActive(true);
    }
}
