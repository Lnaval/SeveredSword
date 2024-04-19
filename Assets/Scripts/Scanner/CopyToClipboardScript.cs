using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CopyToClipboard : MonoBehaviour
{

    public TextMeshProUGUI scannedText;
    public GameObject qrCodeScannerObject;
    public GameObject copyButton;
    public TMP_Text menuTitle;
    QrCodeScanner qrCodeScanner;

    void Start(){
        qrCodeScanner = qrCodeScannerObject.GetComponent<QrCodeScanner>();
    }


    public void PopUp()
    {
        menuTitle.text = "";
        scannedText.text = "";

        string copyText = qrCodeScanner.textToCopy;
        string menu = "Scanned Text";
        bool isInFutureChapter = qrCodeScanner.isForFutureChap;
        bool isInPastChapter = qrCodeScanner.isForPastChap;
        bool isQrCollected = qrCodeScanner.isCollected;
        bool newQrCollect = qrCodeScanner.newCollect;

        if (isInFutureChapter)
        {
            menu = "Warning";
            copyButton.SetActive(false);
            copyText = "Wrong adnuculi for this chapter";
            qrCodeScanner.isForFutureChap = false;
        }
        else if (isInPastChapter)
        {
            menu = "Warning";
            copyButton.SetActive(false);
            copyText = "Wrong adnuculi for this chapter";
            qrCodeScanner.isForPastChap = false;
        }
        else if (isQrCollected)
        {
            menu = "Warning";
            copyButton.SetActive(false);
            copyText = "Already collected";
            qrCodeScanner.isCollected = false;
        } 
        else if (newQrCollect)
        {
            menu = "Congratulations!";
            copyButton.SetActive(false);
            copyText = "You found something";
            qrCodeScanner.newCollect = false;
        }
        Debug.Log(copyText);
        menuTitle.text = menu;
        scannedText.text = copyText;
    }

    public void Back()
    {
        menuTitle.text = "";
        scannedText.text = "";
    }

    public void CopyText()
    {
        GUIUtility.systemCopyBuffer = scannedText.text;
    }
}


