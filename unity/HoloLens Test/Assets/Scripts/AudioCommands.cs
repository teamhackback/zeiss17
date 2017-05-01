using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VR.WSA.Input;

public class AudioCommands : MonoBehaviour
{

    SpriteRenderer btn;

    // Use this for initialization
    void Start()
    {
        btn = gameObject.GetComponent<SpriteRenderer>();
    }

    void OnSelect()
    {
        btn.transform.position = new Vector3(btn.transform.position.x, btn.transform.position.y + 3.0f, btn.transform.position.z);
        btn.enabled = false;
    }
}
