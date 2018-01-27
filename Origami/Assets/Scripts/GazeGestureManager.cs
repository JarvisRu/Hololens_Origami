using UnityEngine;
using UnityEngine.XR.WSA.Input;

public class GazeGestureManager : MonoBehaviour
{
    public static GazeGestureManager Instance { get; private set; }

    // Represents the hologram that is currently being gazed at.
    public GameObject FocusedObject { get; private set; }

    GestureRecognizer recognizer;


    bool updateFocusedObject = true;

    // Use this for initialization
    void Start()
    {
        Instance = this;

        // Set up a GestureRecognizer to detect Select gestures.
        recognizer = new GestureRecognizer();
        recognizer.SetRecognizableGestures(GestureSettings.Hold | GestureSettings.Tap);

        recognizer.Tapped += (args) =>
        {
            // Send an OnSelect message to the focused object and its ancestors.
            // Ignore the error
            if (FocusedObject != null)
            {
                FocusedObject.SendMessageUpwards("OnSelect", SendMessageOptions.DontRequireReceiver);
            }
        };

        recognizer.HoldStartedEvent += (source, ray) =>
        {
            if (FocusedObject != null)
            {
                updateFocusedObject = false;
                FocusedObject.SendMessageUpwards("OnHoldStart", SendMessageOptions.DontRequireReceiver);
            }
        };
        recognizer.HoldCompletedEvent += (source, ray) =>
        {
            if (FocusedObject != null)
            {
                updateFocusedObject = true;
                FocusedObject.SendMessageUpwards("OnHoldCompleted", SendMessageOptions.DontRequireReceiver);
            }
        };

        recognizer.HoldCanceledEvent += (source, ray) =>
        {
            if (FocusedObject != null)
            {
                updateFocusedObject = true;
                FocusedObject.SendMessageUpwards("OnHoldCompleted", SendMessageOptions.DontRequireReceiver);
            }
        };

        recognizer.StartCapturingGestures();
    }

    // Update is called once per frame
    void Update()
    {
        if (!updateFocusedObject)
        {
            return;
        }

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