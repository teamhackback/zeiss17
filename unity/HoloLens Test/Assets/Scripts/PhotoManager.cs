using UnityEngine.VR.WSA.WebCam;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Windows;

public class LandmarkEntry
{
    public string name;
    public double confidence;
    public LandmarkEntry(string name, double confidence)
    {
        this.name = name;
        this.confidence = confidence;
    }
}

public class PhotoManager : MonoBehaviour {
        private int count = 0;
        private PhotoCapture photoCaptureObject = null;
        private bool changecolor = true;
        private Texture2D targetTexture = null;
    private bool isInPhotoMode;
    private Resolution cameraResolution;
    TextMesh text;
    TextMesh subCaption;
    SpriteRenderer audioButton;
    SpriteRenderer playButton;

    public int LandmarkName
        {
                get;
                private set;
        }

    public static string getDescription(string name)
    {
        string text = "";
        switch (name)
        {
            case "Colloseum":
                text= "The Colosseum, also known as the Flavian Amphitheatre,\nis an oval amphitheatre in the centre of the city of Rome, Italy.\nBuilt of concrete and sand, it is the largest amphitheatre ever built.\nThe Colosseum is situated just east of the Roman Forum.";
                break;
            case "Brandenburg Gate":
                text= "The Brandenburg Gate is an 18th-century neoclassical monument in Berlin.\nIt is built on the site of a former\ncity gate that marked the start of the road from Berlin\nto the town of Brandenburg an der Havel.";
                break;
            case "Marienplatz":
                text = "Marienplatz is a central square in the city centre of Munich, Germany.\nIt has been the city's main square since 1158.\nMarienplatz was named after the Mariensäule, a Marian column\nerected in its centre in 1638 to celebrate the end of Swedish occupation.";
                break;
            default:
                break;
               
        }
        text = text.Replace("\\n", "\n");
        return text;
    }

        IEnumerator IdentifyLandmark (byte[] data)
        {
                string requestParameters = "model=landmarks";
                string uri = "https://westus.api.cognitive.microsoft.com/vision/v1.0/models/landmarks/analyze?" + requestParameters;

      
        WWWForm form = new WWWForm();
        form.AddBinaryData("fileUpload", data, "screenShot.png", "image/jpeg");
        UnityWebRequest www = UnityWebRequest.Post(uri, form);
                www.SetRequestHeader("Ocp-Apim-Subscription-Key", "c293c74b91e94980be5a2108e63bdc0e");
                //www.SetRequestHeader("Content-Type", "application/octet-stream");
                yield return www.Send();

        if (www.isError) {
            //gameObject.GetComponent<Renderer>().material.color = Color.red;
        } else {
            Debug.Log(www.downloadHandler.text);
            var arr = SimpleJSON.JSON.Parse(www.downloadHandler.text)["result"]["landmarks"].AsArray;
            Debug.Log(arr);
            List<LandmarkEntry> entries = new List<LandmarkEntry>();
            foreach (SimpleJSON.JSONObject element in arr)
            {
                entries.Add(new LandmarkEntry(element["name"].ToString(),element["confidence"].AsDouble));
            }
            entries.Sort(delegate(LandmarkEntry a, LandmarkEntry b)
            {
                return a.confidence.CompareTo(b.confidence);
            });
            Debug.Log(entries);


            // gameObject.GetComponent<Renderer>().material.color = Color.green;
         
            var headPosition = Camera.main.transform.position;
            var gazeDirection = Camera.main.transform.forward;
            if (entries.Count() == 0)
                    {
                        text.text = "";
                        subCaption.text = "";
                        playButton.enabled = false;
                        audioButton.enabled = false;
                    }
                    else
                    {
                        playButton.enabled = true;
                        audioButton.enabled = true;        
                        audioButton.transform.position = headPosition + new Vector3(gazeDirection.x, gazeDirection.y, 2.0f); // two meters away from initial view
                        playButton.transform.position = headPosition + new Vector3(gazeDirection.x, gazeDirection.y, 2.0f);
                      
                        string name = entries.First().name;
                        text.text = name;
                        subCaption.text = getDescription(name);
                        text.fontSize = 100;
                        subCaption.characterSize = 0.02f;
                        subCaption.fontSize = 90;
                        text.transform.position = headPosition +  new Vector3(gazeDirection.x, gazeDirection.y, 6); // two meters away from initial view
                        subCaption.transform.position = headPosition + new Vector3(gazeDirection.x, gazeDirection.y - 0.3f, 6); // two meters away from initial view
            }
                 
                   
        }

        }

        void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
        {
                if (result.success)
                {
                        Texture2D texture = new Texture2D(cameraResolution.width, cameraResolution.height);
                        photoCaptureFrame.UploadImageDataToTexture(texture);
                   

                        // Now we could do something with the array such as texture.SetPixels() or run image processing on the lis
                        StartCoroutine(IdentifyLandmark(texture.EncodeToJPG()));
                }
              
        }


        void OnPhotoCaptureCreated(PhotoCapture captureObject)
        {
                photoCaptureObject = captureObject;

                cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).Last();

                CameraParameters c = new CameraParameters();
                c.hologramOpacity = 0.0f;
                c.cameraResolutionWidth = cameraResolution.width;
                c.cameraResolutionHeight = cameraResolution.height;
                c.pixelFormat = CapturePixelFormat.BGRA32;

                captureObject.StartPhotoModeAsync(c, OnPhotoModeStarted);
        }

        void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result)
        {
                if (result.success)
                {
                        isInPhotoMode = true;
                       // photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
                }
                else
                {
                        Debug.LogError("Unable to start photo mode!");
                }
        }

    private void Start()
    {
        // Create a PhotoCapture object
  
        //   byte[] data = System.IO.File.ReadAllBytes("C:/Users/User/Pictures/marienplatz.jpg");
        // IdentifyLandmark(data);

        playButton = gameObject.transform.Find("Play").GetComponent<SpriteRenderer>();
        audioButton = gameObject.transform.Find("Audio").GetComponent<SpriteRenderer>();
        text = gameObject.transform.Find("text").GetComponent<TextMesh>();
        subCaption = gameObject.transform.Find("subcaption").GetComponent<TextMesh>();
    }

    void Stop()
    {
        photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
    }

    // Update is called once per frame
    void Update () {
        if (count == 0 && !isInPhotoMode)
        {
            PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);
        }
        count++;
                if (PhotoCapture.SupportedResolutions.Count() == 0)
                {
                return;
                }

                if (count >= 100 && isInPhotoMode) {
                        photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
                        count = 1;
                }

      
	}

        void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result) {
                // Shutdown the photo capture resource
                photoCaptureObject.Dispose();
                photoCaptureObject = null;
        }
}
