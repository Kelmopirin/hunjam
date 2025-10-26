using UnityEngine;

public class DrunkEffect : MonoBehaviour
{
    [Header("Drunk Effect Settings")]
    public bool isDrunk = false;
    public float swayAmount = 0.5f;      // How far the camera moves
    public float swaySpeed = 1f;         // How fast it sways

    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.localPosition;
    }

    void Update()
    {
        if (!isDrunk)
        {
            transform.localPosition = originalPosition;
            return;
        }

        float swayX = Mathf.Sin(Time.time * swaySpeed) * swayAmount;
        float swayY = Mathf.Cos(Time.time * swaySpeed * 0.5f) * swayAmount;

        transform.localPosition = originalPosition + new Vector3(swayX, swayY, 0);
    }

}
