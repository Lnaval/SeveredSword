using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using UnityEngine.UI;
using TMPro; 
using Firebase;
using Firebase.Database;
using Firebase.Storage;
using System.Linq;
using System.Threading.Tasks;

public class QrCodeGenerator : MonoBehaviour
{
    [SerializeField] private RawImage rawImageReceiver;
    [SerializeField] private Texture2D storeEncodedTexture;
    [SerializeField] private TMP_InputField textInputField;
    [SerializeField] private TMP_InputField textInputChapterID;

    private DatabaseReference reference;

    private FirebaseStorage storage;
    private StorageReference storageReference;

    private TrySetActive trySetActive;
    [SerializeField] private GameObject qrDisplayObject;
    private QrDisplay qrDisplay;

    [SerializeField] private GameObject confirmationPopUp;
    ConfirmationPopUp confirmationPopUpScr;

    void Start()
    {
        storeEncodedTexture = new Texture2D(256,256);
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        storage = FirebaseStorage.DefaultInstance;

        trySetActive = GameObject.FindGameObjectWithTag("UIHandler").GetComponent<TrySetActive>();
        confirmationPopUpScr = GameObject.FindGameObjectWithTag("ConfirmPopUp").GetComponent<ConfirmationPopUp>();
    }

    private Color32 [] Encode (string textForEncoding, int height, int width)
    {
        BarcodeWriter writer = new BarcodeWriter()
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Height = height,
                Width = width
            }
        };
        return writer.Write(textForEncoding); 
    }

    public void OnClickEncode()
    {
        if (CheckInputFields() != false)
        {
            string qrKey = GenerateQrKey();
            int chapterID;
            
            if (int.TryParse(textInputChapterID.text, out chapterID))
            {
                EncodeTextToQRCode(qrKey, chapterID);
            }
            else
            {
                // Handle the case where the text cannot be parsed as an integer.
                Debug.LogError("Invalid Chapter ID input");
                confirmationPopUpScr.CheckStatus("Invalid Chapter ID input. Must be a number");
                confirmationPopUp.SetActive(true);

            }
        } else {
            Debug.LogError("not saved");

            confirmationPopUpScr.CheckStatus("All data fields must have value");
            confirmationPopUp.SetActive(true);
        }
    }

    private string GenerateQrKey()
    {
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var stringChars = new char[8];
        var random = new System.Random();

        for (int i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[random.Next(chars.Length)];
        }

        var finalString = new String(stringChars);

        return finalString;
    }

    private void WriteImageOnDisk()
    {
        string fileName = textInputField.text;

        byte[] textureBytes = ImageConversion.EncodeToPNG(storeEncodedTexture);

        NativeGallery.SaveImageToGallery(storeEncodedTexture, "QR Codes", fileName);
        Debug.Log("saved");
    }

    private void EncodeTextToQRCode(string qrKey, int chapID)
    {
        string textWrite = qrKey;
        Color32[] convertPixelToTexture = Encode(textWrite, storeEncodedTexture.height, storeEncodedTexture.width);
        storeEncodedTexture.SetPixels32(convertPixelToTexture);
        storeEncodedTexture.Apply();
        rawImageReceiver.texture = storeEncodedTexture;

        StartCoroutine(SaveQrNameToDb(qrKey, chapID));
    }

    private bool CheckInputFields()
    {
        if(textInputField.text == "" || textInputChapterID.text == "")
        {
            Debug.LogError("All data fields must contain value");
            return false;
        } 
        else
        {
            return true;
        }
    }

    //the following IEnumerator handles uploading user input sa DB
    private IEnumerator SaveQrNameToDb(string name, int chapID)
    {
        var dbTaskName = reference.Child("QR Table").Child(textInputField.text.ToString()).Child("Code").SetValueAsync(name);

        yield return new WaitUntil (predicate: () => dbTaskName.IsCompleted);

        if (dbTaskName.Exception != null)
        {
            Debug.LogWarning(message: $"Failed due to {dbTaskName.Exception}");
        }
        else 
        {
            Debug.Log("success");
        }

        var dbTaskID = reference.Child("QR Table").Child(textInputField.text.ToString()).Child("ChapterID").SetValueAsync(chapID);

        yield return new WaitUntil (predicate: () => dbTaskID.IsCompleted);

        if (dbTaskID.Exception != null)
        {
            Debug.LogWarning(message: $"Failed due to {dbTaskID.Exception}");
        }
        else 
        {
            Debug.Log("success");
        }

        WriteImageOnDisk();
        UploadQrImage();
    }

    private void UploadQrImage()
    {
        string pathName = "QrImages/" + textInputField.text + ".png";
        storageReference = storage.RootReference.Child(pathName);

        byte[] customBytes = ImageConversion.EncodeToPNG(storeEncodedTexture);

        var newMetadata = new MetadataChange();
        newMetadata.ContentType = "image/png";
        storageReference.PutBytesAsync(customBytes, newMetadata).ContinueWith((Task<StorageMetadata> task) => {
            if (task.IsFaulted || task.IsCanceled) {
                Debug.Log(task.Exception.ToString());

                confirmationPopUpScr.CheckStatus("Not Saved");
                confirmationPopUp.SetActive(true);
            }
            else {
                // Metadata contains file metadata such as size, content-type, and md5hash.
                StorageMetadata metadata = task.Result;
                string md5Hash = metadata.Md5Hash;
                // Debug.Log("Finished uploading...");
                // Debug.Log("md5 hash = " + md5Hash);
                confirmationPopUpScr.CheckStatus("Saved");
                confirmationPopUp.SetActive(true);
            }
        });
    }

    
    public void OnClickViewList()
    {
        trySetActive.QRDisplayScreen();

        textInputField.text = "";
        textInputChapterID.text = "";
    }
}