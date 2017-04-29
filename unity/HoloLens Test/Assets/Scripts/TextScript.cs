using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextScript : MonoBehaviour
{
    TextMesh txt;
    private int currentscore = 0;

    // Use this for initialization
    void Start()
    {
        txt = gameObject.GetComponent<TextMesh>();
        txt.text = "Score : " + currentscore;
    }

    // Update is called once per frame
    void Update()
    {
        txt.transform.localPosition.Set(0, 0, currentscore % 2);
        txt.text = "Score : " + currentscore;
        currentscore++;
    }
}