using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using UnityEngine.UI;
using TMPro; 
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Firebase.Storage;

public class QrDisplay : MonoBehaviour
{
    [SerializeField] private GameObject qrElement;
    [SerializeField] private Transform qrContent;

    private FirebaseStorage storage;
    private StorageReference storageReference;
    private DatabaseReference reference;

    [SerializeField] private GameObject confirmationPopUp;
    ConfirmationPopUp confirmationPopUpScr;

    
    public void GetQrInfo ()
    {
        StartCoroutine(LoadQrData());
    }

    public IEnumerator LoadQrData()
    {
        var dbTask = reference.Child("QR Table").OrderByChild("ChapterID").GetValueAsync();
        yield return new WaitUntil(predicate: () => dbTask.IsCompleted);

        if (dbTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed due to {dbTask.Exception}");
        }
        else 
        {
            DataSnapshot qrSnapshot = dbTask.Result;
            Debug.Log(qrSnapshot);
            foreach (Transform child in qrContent.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (DataSnapshot childSnap in qrSnapshot.Children)
            {
                string name = childSnap.Key.ToString(); 
                string chapID = childSnap.Child("ChapterID").Value.ToString();

                GameObject qrListElement = Instantiate(qrElement, qrContent); 
                qrListElement.GetComponent<QrElement>().NewQrElement(name, chapID);
            }
        }
    }

    private IEnumerator DeleteData(string deleteChildKey)
    {
        var dbTask = reference.Child("QR Table").Child(deleteChildKey).RemoveValueAsync();
        yield return new WaitUntil(predicate: () => dbTask.IsCompleted);

        if (dbTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed due to {dbTask.Exception}");
        }
        else 
        {
            Debug.Log("Deleted");
        }
    }

    public void Delete(string deleteChildKey)
    {
        string imagePath = string.Concat(deleteChildKey, ".png");

        StorageReference deleteImg = storageReference.Child(imagePath);

        StartCoroutine(DeleteData(deleteChildKey));

        deleteImg.DeleteAsync().ContinueWithOnMainThread(taskDelete => {
        if (taskDelete.IsCompleted) {
            Debug.Log("File deleted successfully.");
            confirmationPopUpScr.CheckStatus("Qr Deleted");
            confirmationPopUp.SetActive(true);
        }
        else {
            confirmationPopUpScr.CheckStatus("Qr Not Deleted");
            confirmationPopUp.SetActive(true);
        }
        });

        
        GetQrInfo();
    }

    public void Download(string fileName)
    {
        string imagePath = fileName + ".png";
        Debug.Log(imagePath);

        StorageReference img = storageReference.Child(imagePath);

        Debug.Log("download clicked");
        img.GetBytesAsync(1024*1024).ContinueWithOnMainThread(taskDownload => 
        {
            if (taskDownload.IsCompleted)
            {
                //System.IO.File.WriteAllBytes(localFilePath, task.Result);
                NativeGallery.SaveImageToGallery(taskDownload.Result, "QR Codes", imagePath);
                Debug.Log("img saved");
                confirmationPopUpScr.CheckStatus("Image Saved");
                confirmationPopUp.SetActive(true);
            }
            else
            {
                Debug.Log("img not saved");
                confirmationPopUpScr.CheckStatus("Image Not Saved");
                confirmationPopUp.SetActive(true);
            }
        });
    }

    void Start()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        storage = FirebaseStorage.DefaultInstance;
        storageReference = storage.GetReferenceFromUrl("gs://severed-sword-33d81.appspot.com/QrImages");
        confirmationPopUpScr = GameObject.FindGameObjectWithTag("ConfirmPopUp").GetComponent<ConfirmationPopUp>();
    }
}
