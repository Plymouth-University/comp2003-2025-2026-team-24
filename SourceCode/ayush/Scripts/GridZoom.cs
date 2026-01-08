using UnityEngine;

public class GridZoom : MonoBehaviour
{
    public float zoomSpeed = 0.01f;
    public float minScale = 0.5f;
    public float maxScale = 2.5f;

    float lastPinchDistance;

    void Update()
    {
        // mouse wheel (useful in editor)
        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) > 0.01f)
        {
            Zoom(scroll * 0.1f);
        }

        // touch pinch (for mobile)
        if (Input.touchCount == 2)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            Vector2 pos0 = t0.position - t0.deltaPosition;
            Vector2 pos1 = t1.position - t1.deltaPosition;

            float prevDist = (pos0 - pos1).magnitude;
            float currentDist = (t0.position - t1.position).magnitude;

            float diff = currentDist - prevDist;

            Zoom(diff * zoomSpeed);
        }
    }

    void Zoom(float delta)
    {
        Vector3 scale = transform.localScale;
        float newScale = Mathf.Clamp(scale.x + delta, minScale, maxScale);
        transform.localScale = new Vector3(newScale, newScale, 1f);
    }
}
