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

public class DownloadDelete : MonoBehaviour
{
    private FirebaseStorage storage;
    private StorageReference storageReference;
    private DatabaseReference reference;

    QrDisplay qrDisplay;
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

        //var dbTaskDelete = reference.Child("QR Table").Child(deleteChildKey).SetValueAsync(null);
        StartCoroutine(DeleteData(deleteChildKey));

        deleteImg.DeleteAsync().ContinueWithOnMainThread(task => {
        if (task.IsCompleted) {
            Debug.Log("File deleted successfully.");
        }
        else {
            // Uh-oh, an error occurred!
        }
        });
        qrDisplay = GameObject.FindGameObjectWithTag("QrDisplay").GetComponent<QrDisplay>();
        qrDisplay.GetQrInfo();
    }

    public void Download(string fileName)
    {
        string imagePath = fileName + ".png";

        StorageReference img = storageReference.Child(imagePath);


        img.GetBytesAsync(1024*1024).ContinueWith(task => 
        {
            if (!task.IsFaulted && !task.IsCanceled)
            {
                //System.IO.File.WriteAllBytes(localFilePath, task.Result);
                NativeGallery.SaveImageToGallery(task.Result, "QR Codes", imagePath);
                Debug.Log("img saved");
            }
            else
            {
                Debug.Log("img not saved");
            }
        });
    }


    void Start()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        storage = FirebaseStorage.DefaultInstance;
        storageReference = storage.GetReferenceFromUrl("gs://severed-sword-33d81.appspot.com/QrImages");
    }
}
