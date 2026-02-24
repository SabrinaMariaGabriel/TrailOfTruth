using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    public Transform target;      // Player
    public float smooth = 8f;

    // Optional: Clamp, damit Kamera nicht aus der Map rausgeht
    public Vector2 minPos;
    public Vector2 maxPos;
    public bool useClamp = false;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desired = new Vector3(target.position.x, target.position.y, transform.position.z);
        Vector3 smoothed = Vector3.Lerp(transform.position, desired, smooth * Time.deltaTime);

        if (useClamp)
        {
            smoothed.x = Mathf.Clamp(smoothed.x, minPos.x, maxPos.x);
            smoothed.y = Mathf.Clamp(smoothed.y, minPos.y, maxPos.y);
        }

        transform.position = smoothed;
    }
}