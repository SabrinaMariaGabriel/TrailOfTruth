using UnityEngine;

public class TapDebugger : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            Debug.Log("Mouse down!");

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            Debug.Log("Touch began!");
    }
}