using UnityEngine.VR.WSA.WebCam;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class PhotoManager : MonoBehaviour {
        private int count = 0;
        private PhotoCapture photoCaptureObject = null;
        private bool changecolor = true;
        private Texture2D targetTexture = null;


        public int LandmarkName
        {
                get;
                private set;
        }

        IEnumerator IdentifyLandmark (byte[] data)
        {
                string requestParameters = "model=landmarks";
                string uri = "https://westus.api.cognitive.microsoft.com/vision/v1.0/models/landmarks/analyze?" + requestParameters;

                UnityWebRequest www = UnityWebRequest.Post(uri, data);
                www.SetRequestHeader("Ocp-Apim-Subscription-Key", "c293c74b91e94980be5a2108e63bdc0e");
                www.SetRequestHeader("Content-Type", "application/octet-stream");
                yield return www;

                string bla = www.ToString();
                Debug.Log(bla);


                if (www.isError) {
                        gameObject.GetComponent<Renderer>().material.color = Color.red;
                } else {
                        gameObject.GetComponent<Renderer>().material.color = Color.green;
                        /*
                        TextMesh txt2 = gameObject.AddComponent<TextMesh>();
                        txt2.text = www.ToString();
                        txt2.characterSize = 0.05f;
                        txt2.fontSize = 50;
                        txt2.transform.position = new Vector3(1.74f, 1.26f, 2.94f);
                        */
                }

        }

        void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
        {
                if (result.success)
                {
                        List<byte> imageBufferList = new List<byte>();
                        // Copy the raw IMFMediaBuffer data into our empty byte list.
                        photoCaptureFrame.CopyRawImageDataIntoBuffer(imageBufferList);

                        // Now we could do something with the array such as texture.SetPixels() or run image processing on the lis
                        StartCoroutine(IdentifyLandmark(imageBufferList.ToArray()));
                }
                photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
        }


        void OnPhotoCaptureCreated(PhotoCapture captureObject)
        {
                photoCaptureObject = captureObject;

                Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();

                CameraParameters c = new CameraParameters();
                c.hologramOpacity = 0.0f;
                c.cameraResolutionWidth = cameraResolution.width;
                c.cameraResolutionHeight = cameraResolution.height;
                c.pixelFormat = CapturePixelFormat.BGRA32;

                captureObject.StartPhotoModeAsync(c, false, OnPhotoModeStarted);
        }

        private void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result)
        {
                if (result.success)
                {
                        photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
                }
                else
                {
                        Debug.LogError("Unable to start photo mode!");
                }
        }

	// Update is called once per frame
	void Update () {
                count++;
                if (PhotoCapture.SupportedResolutions.Count() == 0)
                {
                return;
                }

                if (count == 400) {
                        Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
                        targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);

                        // Create a PhotoCapture object
                        PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);
                        count = 0;
                }
	}

        void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result) {
                // Shutdown the photo capture resource
                photoCaptureObject.Dispose();
                photoCaptureObject = null;
        }
}
