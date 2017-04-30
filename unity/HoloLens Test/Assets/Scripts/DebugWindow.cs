using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugWindow : MonoBehaviour
{
    TextMesh textMesh;

    // Use this for initialization
    void Start()
    {
        textMesh = gameObject.GetComponentInChildren<TextMesh>();
    }

    public void LogMessage(string message)
    {
            textMesh.text += message + "\n";
    }
}