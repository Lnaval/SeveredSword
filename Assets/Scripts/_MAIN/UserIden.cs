using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserIden : MonoBehaviour
{
    public static UserIden UserInstance;

    public string userName = "";

    private void Awake()
    {
        if (UserInstance != null)
        {
            Destroy(gameObject);
            return;
        }

        UserInstance = this;
        DontDestroyOnLoad(gameObject);
    }
}
