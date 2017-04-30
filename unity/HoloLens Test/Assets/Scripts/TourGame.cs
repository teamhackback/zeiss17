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

        var headPosition = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward;
        txt2.text = "Marienplatz";
        txt2.characterSize = 0.05f;
        txt2.fontSize = 100;
        txt2.transform.position = headPosition + gazeDirection * 2f; // two meters awaz from initial view
    }

    // Update is called once per frame
    void Update()
    {
        txt.transform.position = new Vector3(1.94f, 1.26f + currentscore % 5 * 0.1f, 2.94f);
        txt.text = "F : " + currentscore;
        currentscore++;
    }
}