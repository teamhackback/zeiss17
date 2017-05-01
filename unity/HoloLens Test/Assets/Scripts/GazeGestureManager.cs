using UnityEngine;
using UnityEngine.VR.WSA.Input;

public class GazeGestureManager : MonoBehaviour
{
    public static GazeGestureManager Instance { get; private set; }

    // Represents the hologram that is currently being gazed at.
    public GameObject FocusedObject { get; private set; }

    GestureRecognizer recognizer;

    // Use this for initialization
    void Start()
    {
        Instance = this;

        // Set up a GestureRecognizer to detect Select gestures.
        recognizer = new GestureRecognizer();
        recognizer.SetRecognizableGestures(GestureSettings.Tap | GestureSettings.Hold);
        recognizer.TappedEvent += (source, tapCount, ray) =>
        {
            Debug.Log("TappedEvent!");
            // Send an OnSelect message to the focused object and its ancestors.
            if (FocusedObject != null)
            {
                FocusedObject.SendMessage("OnSelect", null, SendMessageOptions.DontRequireReceiver);
            }

        };
        recognizer.HoldStartedEvent += (source, ray) =>
        {
            Debug.Log("HoldStarted!");
            /*
            TextMesh txt2 = gameObject.AddComponent<TextMesh>();
            txt2.text = "Hallo";
            txt2.characterSize = 0.05f;
            txt2.fontSize = 100;
            txt2.transform.position = new Vector3(1.74f, 1.26f, 2.94f);
            */
        };
        recognizer.ManipulationCompletedEvent += (source, second, third) =>
        {
            Debug.Log("ManipulationCompleted!");
        };
        recognizer.NavigationCompletedEvent += (source, second, third) =>
        {
            Debug.Log("NavigationCompleted!");
        };
        recognizer.StartCapturingGestures();
    }

    // Update is called once per frame
    void Update()
    {
        // Figure out which hologram is focused this frame.
        GameObject oldFocusObject = FocusedObject;

        // Do a raycast into the world based on the user's
        // head position and orientation.
        var headPosition = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward;

        RaycastHit hitInfo;
        if (Physics.Raycast(headPosition, gazeDirection, out hitInfo))
        {
            // If the raycast hit a hologram, use that as the focused object.
            FocusedObject = hitInfo.collider.gameObject;
        }
        else
        {
            // If the raycast did not hit a hologram, clear the focused object.
            FocusedObject = null;
        }

        // If the focused object changed this frame,
        // start detecting fresh gestures again.
        if (FocusedObject != oldFocusObject)
        {
            recognizer.CancelGestures();
            recognizer.StartCapturingGestures();
        }
    }
}