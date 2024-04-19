using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ButtonText1 : MonoBehaviour
{
    public TMP_Text nameText;

    public string GetQrName()
    {
        return nameText.text.ToString(); // should return the name of the child (ex. adnuculi1) once called sa scr.cs file 
    }
}
