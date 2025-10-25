using UnityEngine;

public class SpriteRotator : MonoBehaviour
{
    public Transform playerCamera;

    void LateUpdate()
    {
        // Direction to camera but ignore vertical tilt
        Vector3 direction = playerCamera.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            // Calculate only the Y rotation needed
            float targetY = Quaternion.LookRotation(direction).eulerAngles.y;

            // Keep the current X and Z rotation
            Vector3 current = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(current.x, targetY, current.z);
        }
    }
}
