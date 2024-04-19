using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZXing;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System;
public class QrCodeScanner : MonoBehaviour
{
    [SerializeField] private RawImage rawImageBackground;
    [SerializeField] private AspectRatioFitter aspectRatioFitter;
    // [SerializeField] private TextMeshProUGUI textOut;
    // [SerializeField] private RectTransform scanZone;

    private bool _isCamAvail;
    private WebCamTexture webCamTexture;

    private DatabaseReference reference;
    private DependencyStatus dependencyStatus;
    private FirebaseAuth auth;

    [SerializeField] private GameObject playerHandlerObject;
    [SerializeField] private GameObject popUp;
    [SerializeField] private GameObject copyclipscr;

    CopyToClipboard copyToClipboard;
    public string textToCopy;
    public bool isForFutureChap = false;
    public bool isForPastChap = false;
    public bool isCollected = false;
    public bool newCollect = false;

    private void SetUpCamera()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length == 0)
        {
            _isCamAvail = false;
            return;
        } 

        Debug.Log("camera avail");

        for (int i = 0; i < devices.Length; i++)
        {
            if (devices[i].isFrontFacing == false)
            {
                //webCamTexture = new WebCamTexture(devices[i].name, (int)scanZone.rect.width, (int)scanZone.rect.height);
                webCamTexture = new WebCamTexture(devices[i].name, 1080, 2400);
            }
        }

        webCamTexture.Play();
        rawImageBackground.texture = webCamTexture;

        _isCamAvail = true;
    }

    private void UpdateCameraRender()
    {
        if (_isCamAvail == false)
        {
            
            return;
        }
        float ratio = (float) webCamTexture.width / (float) webCamTexture.height;

        aspectRatioFitter.aspectRatio = ratio;

        int orientation = -webCamTexture.videoRotationAngle;

        rawImageBackground.rectTransform.localEulerAngles = new Vector3(0, 0, orientation);
    }

    private void Scan()
    {
        string decodedQr;
        try 
        {
            IBarcodeReader barcodeReader = new BarcodeReader();
            Result result = barcodeReader.Decode(webCamTexture.GetPixels32(), webCamTexture.width, webCamTexture.height);

            if (result != null)
            {
                decodedQr = result.Text;
            } 
            else
            {
                decodedQr = "Fail";
            }
        }

        catch
        {
            decodedQr = "Fail";
        }

        StartCoroutine(VerifyQr(decodedQr));
    }

    private IEnumerator VerifyQr(string decodedQr)
    {
        //Verify if the QR scanned is an adnuculi or another QR
        Debug.Log (decodedQr);
        var res = reference.Child("QR Table").OrderByChild("Code").EqualTo(decodedQr).GetValueAsync();
        yield return new WaitUntil(predicate: () => res.IsCompleted);
        if (res.Exception != null)
        {
            Debug.LogWarning(message: $"Failed due to {res.Exception}");
        }
        else 
        {
            Debug.Log("1stVerifyQr Accepted");
            DataSnapshot snap = res.Result;
            Debug.Log(snap);
            if (!snap.Exists)
            {
                Debug.Log("does not exist in QR DB");
                
                //display popUP
                if(decodedQr != "Fail")
                {
                    textToCopy = decodedQr;
                    // CopyToClipboard copyToClipboard = copyclipscr.GetComponent<CopyToClipboard>();
                    copyToClipboard.PopUp();
                    popUp.SetActive(true);
                }
            }
            else
            {
                Debug.Log("it exists");

                foreach (DataSnapshot childSnap in snap.Children)
                {
                    string nameResult = childSnap.Child("Code").Value.ToString();
                    int chapID = int.Parse(childSnap.Child("ChapterID").Value.ToString());
                    string keyName =childSnap.Key.ToString();
                    Debug.Log(keyName);

                    Debug.Log(chapID + " " + MainManager.Instance.current_Chapter);

                    if(chapID == MainManager.Instance.chapterSeed[MainManager.Instance.current_Chapter-1]) //checks if the user is scanning a qr for their current chapter
                    { 
                        StartCoroutine(VerifyQrInUserDb(nameResult, keyName, auth.CurrentUser.UserId, chapID));
                        break;
                    } 
                    // else if (chapID > MainManager.Instance.chapterSeed[MainManager.Instance.current_Chapter-1]) //to fix
                    // {
                    //     isForFutureChap = true;
                    //     // CopyToClipboard copyToClipboard = copyclipscr.GetComponent<CopyToClipboard>();
                    //     copyToClipboard.PopUp();
                    //     popUp.SetActive(true);
                    // }
                    else 
                    {
                        isForPastChap = true;
                        // CopyToClipboard copyToClipboard = copyclipscr.GetComponent<CopyToClipboard>();
                        copyToClipboard.PopUp();
                        popUp.SetActive(true);
                    }
                }
            }
        }
    }

    private IEnumerator VerifyQrInUserDb(string decodedQr, string keyName, string userId, int chapID) //function to check whether the QR was already scanned and store in the USER DB
    {
        var res = reference.Child("User").Child(userId).Child("QR Collected").OrderByChild("Code").EqualTo(decodedQr).GetValueAsync();
        yield return new WaitUntil(predicate: () => res.IsCompleted);
        if (res.Exception != null)
        {
            Debug.LogWarning(message: $"Failed due to {res.Exception}");
        }
        else 
        {
            Debug.Log("2ndVerifyQr Accepted");
            DataSnapshot snap2 = res.Result;
            Debug.Log(snap2);
            if (!snap2.Exists)
            {
                Debug.Log("does not exist in User DB");

                newCollect = true;
                Debug.Log(newCollect);

                // CopyToClipboard copyToClipboard = copyclipscr.GetComponent<CopyToClipboard>();
                copyToClipboard.PopUp();
                
                popUp.SetActive(true);

                StartCoroutine(StoreQrNameInUserDb(decodedQr, keyName, auth.CurrentUser.UserId));
                StartCoroutine(StoreQrChapIdInUserDb(chapID, keyName, auth.CurrentUser.UserId));
            }
            else
            {
                Debug.Log("does exist in User DB");
                isCollected = true;
                Debug.Log(isCollected);

                // CopyToClipboard copyToClipboard = copyclipscr.GetComponent<CopyToClipboard>();
                copyToClipboard.PopUp();

                popUp.SetActive(true);
            }
            PlayerManager playerManager = playerHandlerObject.GetComponent<PlayerManager>();
            playerManager.CallQrFunctions();
        }
    }

    private IEnumerator StoreQrNameInUserDb(string decodedQr, string keyName, string userId)
    {
        var res = reference.Child("User").Child(userId).Child("QR Collected").Child(keyName).Child("Code").SetValueAsync(decodedQr);
        yield return new WaitUntil (predicate: () => res.IsCompleted);

        if (res.Exception != null)
        {
            Debug.LogWarning(message: $"Failed due to {res.Exception}");
        }
        else 
        {
            Debug.Log("success");
        }   
    }

    private IEnumerator StoreQrChapIdInUserDb(int chapID, string keyName, string userId)
    {
        var res = reference.Child("User").Child(userId).Child("QR Collected").Child(keyName).Child("ChapterID").SetValueAsync(chapID);
        yield return new WaitUntil (predicate: () => res.IsCompleted);

        if (res.Exception != null)
        {
            Debug.LogWarning(message: $"Failed due to {res.Exception}");
        }
        else 
        {
            Debug.Log("success");
        }   
    }

    public void OnClickReturn()
    {
        SceneManager.LoadScene("PlayerSide");
    }

    public void OnClickScan()
    {
        //StartCoroutine(VerifyQr());
    }

    void Awake()
    {
        //Check that all of the necessary dependencies for Firebase are present on the system
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    private void InitializeFirebase()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        auth = FirebaseAuth.DefaultInstance;
    }

    void Start()
    {
        SetUpCamera();
        copyToClipboard = copyclipscr.GetComponent<CopyToClipboard>();
    }

    float interval = 5f;

    float time;

    void Update()
    {
        UpdateCameraRender();

        time += Time.deltaTime;
        if (!popUp.activeSelf){
            while(time >= interval)
            {
                Scan();
                time -= interval;
            }
        }
    }
}
