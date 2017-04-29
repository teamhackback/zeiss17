using UnityEngine.VR.WSA.WebCam;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotoManager : MonoBehaviour {
        private int count = 0;
        private PhotoCapture photoCaptureObject = null;
        private bool changecolor = true;

	// Use this for initialization
	void Start () {
	}

        void OnPhotoCaptureCreated(PhotoCapture captureObject) {
                photoCaptureObject = captureObject;

                Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();

                CameraParameters c = new CameraParameters();
                c.hologramOpacity = 0.0f;
                c.cameraResolutionWidth = cameraResolution.width;
                c.cameraResolutionHeight = cameraResolution.height;
                c.pixelFormat = CapturePixelFormat.BGRA32;

                captureObject.StartPhotoModeAsync(c, false, OnPhotoModeStarted);
        }

        private void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result) {
                if (result.success) {
                        photoCaptureObject.TakePhotoAsync(IdentifyLandmark);
                }
                else {
                        Debug.LogError("Unable to start photo mode!");
                }
        }

        void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result) {
		photoCaptureObject.Dispose();
		photoCaptureObject = null;
	}

        void IdentifyLandmark (PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
        {
                if (result.success) {
                        List<byte> imageBufferList = new List<byte>();
                        // Copy the raw IMFMediaBuffer data into our empty byte list.
                        photoCaptureFrame.CopyRawImageDataIntoBuffer(imageBufferList);
                      
                        if (changecolor) {
                                gameObject.GetComponent<Renderer>().material.color = Color.red;
                                changecolor = false;
                        } else {
                                changecolor = true;
                                gameObject.GetComponent<Renderer>().material.color = Color.green;
                        }
                        // TODO: Cognition API call
                }
                photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
        }	

	// Update is called once per frame
	void Update () {
                PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);
	}
}
