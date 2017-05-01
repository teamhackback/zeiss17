using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class CubeManager : MonoBehaviour, IInputClickHandler, IInputHandler
{
    SpriteRenderer btn;

    void Start()
    {
        btn = gameObject.GetComponent<SpriteRenderer>();
    }


    public void OnInputClicked(InputClickedEventData eventData)
    {
        btn.transform.position = new Vector3(btn.transform.position.x, btn.transform.position.y + 3.0f, btn.transform.position.z);
        btn.enabled = false;
    }
    public void OnInputDown(InputEventData eventData)
    { }
    public void OnInputUp(InputEventData eventData)
    { }
}