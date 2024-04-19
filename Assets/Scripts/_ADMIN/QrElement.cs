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
using UnityEngine.SceneManagement;

public class QrElement : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text chapterText;
    public RawImage qrDisplayImage;

    private FirebaseStorage storage;
    private StorageReference storageReference;
    private DatabaseReference reference;

    public void NewQrElement( string name, string chapID)
    {
        nameText.text = name;
        chapterText.text = chapID;

        string imagePath = string.Concat(name, ".png");
        storage = FirebaseStorage.DefaultInstance;
        storageReference = storage.GetReferenceFromUrl("gs://severed-sword-33d81.appspot.com/QrImages");
        StorageReference img = storageReference.Child(imagePath);

        img.GetDownloadUrlAsync().ContinueWithOnMainThread(task => {
            Debug.Log("url");
            if (!task.IsFaulted && !task.IsCanceled) {
                Debug.Log("Download URL: " + task.Result);
            
                StartCoroutine(LoadQrImage(Convert.ToString(task.Result)));
            }
            else
            {
                Debug.Log(task.Exception);
            }
        });
    }

    private IEnumerator LoadQrImage(string imageUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            qrDisplayImage.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        }
    }


    
    public GameObject buttonCallObject;
    public string CallFunction()
    {
        ButtonText1 btxt = buttonCallObject.GetComponent<ButtonText1>();
        return btxt.GetQrName();
    }

    //DownloadDelete downloadDelete;
    QrDisplay qrDisplay;
    public void OnClickDelete(){
        string deleteChildKey = CallFunction();
        Debug.Log(deleteChildKey);

        qrDisplay = GameObject.FindGameObjectWithTag("QrDisplay").GetComponent<QrDisplay>();
        qrDisplay.Delete(deleteChildKey);
    }

    public void OnClickDownload(){
        string fileName = CallFunction();
        qrDisplay = GameObject.FindGameObjectWithTag("QrDisplay").GetComponent<QrDisplay>();
        qrDisplay.Download(fileName);
    }
}
