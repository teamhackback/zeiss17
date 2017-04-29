using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TourGame : MonoBehaviour
{
    TextMesh txt;
    private int currentscore = 0;

    // Use this for initialization
    void Start()
    {
        txt = gameObject.transform.Find("Text").GetComponent<TextMesh>();
        txt.text = "F : " + currentscore;
        TextMesh txt2 = gameObject.AddComponent<TextMesh>();
        txt2.text = "Hallo";
        txt2.transform.localPosition.Set(0, 0, 1);
    }

    // Update is called once per frame
    void Update()
    {
        txt.transform.localPosition.Set(0, 0, currentscore % 2);
        txt.text = "F : " + currentscore;
        currentscore++;
    }
}