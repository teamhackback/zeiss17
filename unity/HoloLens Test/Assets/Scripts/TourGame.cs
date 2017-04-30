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
        txt2.characterSize = 1;
        txt2.transform.position = new Vector3(1.74f, 1.26f, 2.94f);
    }

    // Update is called once per frame
    void Update()
    {
        txt.transform.position = new Vector3(1.94f, 1.26f + currentscore % 5 * 0.1f, 2.94f);
        txt.text = "F : " + currentscore;
        currentscore++;
    }
}