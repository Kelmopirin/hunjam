using UnityEngine;

public class Pulse : MonoBehaviour
{
    [Header("Pulsate Settings")]
    public float pulseAmount = 0.2f;      // scale variation relative to default
    public float speed = 2f; 
                 // speed of pulsation
    public float speedrot = 2f;

    [Header("Tilt Settings")]
    public float tiltAngle = 10f;         // max rotation angle left/right in degrees

    private RectTransform rectTransform;
    private Vector3 defaultScale;
    private Quaternion defaultRotation;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogWarning("PulsatingButton script requires a RectTransform.");
        }
        else
        {
            defaultScale = rectTransform.localScale;
            defaultRotation = rectTransform.localRotation;
        }
    }

    void Update()
    {
        if (rectTransform != null)
        {
            // Pulsate scale
            float scaleFactor = (Mathf.Sin(Time.time * speed) + 1f) / 2f;
            float scale = Mathf.Lerp(1 - pulseAmount, 1 + pulseAmount, scaleFactor);
            rectTransform.localScale = defaultScale * scale;

            // Tilt rotation
            float tiltFactor = Mathf.Sin(Time.time * speedrot);
            float angle = tiltFactor * tiltAngle;
            rectTransform.localRotation = defaultRotation * Quaternion.Euler(0f, 0f, angle);
        }
    }
}
