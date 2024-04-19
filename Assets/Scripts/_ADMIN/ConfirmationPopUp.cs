using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ConfirmationPopUp : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text contentText;

    public void CheckStatus(string result)
    {
        titleText.text = "Status";
        contentText.text = result;
    }
    public void Confirm()
    {
        titleText.text = "";
        contentText.text = "";
    }
}
