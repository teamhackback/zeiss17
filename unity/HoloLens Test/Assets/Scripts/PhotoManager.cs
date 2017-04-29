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


        void IdentifyLandmark (PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
        {
                if (result.success) {
                        photoCaptureFrame.UploadImageDataToTexture(targetTexture);
                        //List<byte> imageBufferList = new List<byte>();
                        //photoCaptureFrame.CopyRawImageDataIntoBuffer(imageBufferList);

                        Renderer rend = gameObject.GetComponent<Renderer>() as Renderer;
                        rend.material = new Material(Shader.Find("Custom/Unlit/UnlitTexture"));
                        rend.material.SetTexture("_MainTex", targetTexture);

                        // TODO: Cognition API call
                }
                photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
        }	

	// Update is called once per frame
	void Update () {
        count += 1;
        if (PhotoCapture.SupportedResolutions.Count() == 0)
        {
            return;
        }

                if (count == 60) {
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
