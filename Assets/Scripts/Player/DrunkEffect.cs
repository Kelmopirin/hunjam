using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DrunkEffect : MonoBehaviour
{
    [Header("Drunk State")]
    public bool isDrunk = false;
    [Range(0f, 1f)] public float drunkIntensity = 0f;

    [Header("Sway Settings")]
    public float swayAmount = 0.05f;   // Horizontal movement
    public float swaySpeed = 1.5f;     // Sway speed
    public float rollAmount = 2f;      // Camera roll tilt

    [Header("Post-Processing Settings")]
    public float chromaticMultiplier = 1f;
    public float lensMultiplier = -0.4f;

    private Vector3 originalPos;
    private Quaternion originalRot;

    private Volume volume;
    private ChromaticAberration chromatic;
    private LensDistortion lens;

    void Start()
    {
        originalPos = transform.localPosition;
        originalRot = transform.localRotation;

        // Find any volume in the scene
        volume = FindObjectOfType<Volume>();
        if (volume != null && volume.profile != null)
        {
            volume.profile.TryGet(out chromatic);
            volume.profile.TryGet(out lens);
        }
    }

    void Update()
    {
        // Smoothly interpolate intensity
        float target = isDrunk ? 1f : 0f;
        drunkIntensity = Mathf.Lerp(drunkIntensity, target, Time.deltaTime * 1f);

        // Camera horizontal sway
        float swayX = Mathf.Sin(Time.time * swaySpeed) * swayAmount * drunkIntensity;
        transform.localPosition = originalPos + new Vector3(swayX, 0f, 0f);

        // Camera roll tilt
        float roll = Mathf.Sin(Time.time * swaySpeed * 0.5f) * rollAmount * drunkIntensity;
        transform.localRotation = originalRot * Quaternion.Euler(0f, 0f, roll);

        // Post-processing adjustments
        if (chromatic != null)
            chromatic.intensity.value = drunkIntensity * chromaticMultiplier;

        if (lens != null)
            lens.intensity.value = drunkIntensity * lensMultiplier;
    }
}