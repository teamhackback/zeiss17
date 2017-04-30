using UnityEngine.VR.WSA.WebCam;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


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

        IENumerator IdentifyLandmark (byte[] data)
        {
                List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
                formData.Add( new MultipartFormDataSection(data) );

                string requestParameters = "model=landmarks";
                string uri = "https://westus.api.cognitive.microsoft.com/vision/v1.0/models/landmarks/analyze?" + requestParameters;

                UnityWebRequest www = UnityWebRequest.Post(uri, formData);
                www.SetRequestHeader("Ocp-Apim-Subscription-Key", "c293c74b91e94980be5a2108e63bdc0e");
                yield return www.Send();

                if (www.isError) {
                        gameObject.GetComponent<Renderer>().material.color = Color.red;
                } else {
                        gameObject.GetComponent<Renderer>().material.color = Color.green;
                        TextMesh txt2 = gameObject.AddComponent<TextMesh>();
                        txt2.text = www;
                        txt2.characterSize = 0.05f;
                        txt2.fontSize = 50;
                        txt2.transform.position = new Vector3(1.74f, 1.26f, 2.94f);
                }

        }

        void StartIdentification (PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
        {
                if (result.success) {
                        List<byte> imageBufferList = new List<byte>();
                        photoCaptureFrame.CopyRawImageDataIntoBuffer(imageBufferList);

                        StartCoroutine(IdentifyLandmark(imageBufferList.ToArray()));
                }
                photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
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
                        PhotoCapture.CreateAsync(false, delegate (PhotoCapture captureObject) {
                                photoCaptureObject = captureObject;
                                CameraParameters cameraParameters = new CameraParameters();
                                cameraParameters.hologramOpacity = 0.0f;
                                cameraParameters.cameraResolutionWidth = cameraResolution.width;
                                cameraParameters.cameraResolutionHeight = cameraResolution.height;
                                cameraParameters.pixelFormat = CapturePixelFormat.BGRA32;

                                // Activate the camera
                                photoCaptureObject.StartPhotoModeAsync(cameraParameters, delegate (PhotoCapture.PhotoCaptureResult result) {
                                        // Take a picture
                                        photoCaptureObject.TakePhotoAsync(IdentifyLandmark);
                                });
                        });
                        count = 0;
                }
	}

        void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result) {
                // Shutdown the photo capture resource
                photoCaptureObject.Dispose();
                photoCaptureObject = null;
        }
}
