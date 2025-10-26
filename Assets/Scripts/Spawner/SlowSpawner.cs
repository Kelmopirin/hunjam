using UnityEngine;

public class SlowSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject slowObjectPrefab;    // Prefab that slows player
    public float spawnRate = 0.1f;         // Time between spawns in seconds
    public float lifetime = 5f;            // How long the object lasts

    [Header("Spawn Offset")]
    public Vector3 offset = Vector3.zero;  // Optional offset from NPC position
    public Vector3 rotationOffset = Vector3.zero; // Additional rotation offset

    private float spawnTimer = 0f;

    private void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnRate)
        {
            SpawnSlowObject();
            spawnTimer = 0f;
        }
    }

    private void SpawnSlowObject()
    {
        if (slowObjectPrefab == null) return;

        // Spawn position
        Vector3 spawnPos = transform.position + offset;

        // Base horizontal rotation
        Quaternion baseRotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);

        // Apply rotation offset
        Quaternion spawnRot = baseRotation * Quaternion.Euler(rotationOffset);

        GameObject obj = Instantiate(slowObjectPrefab, spawnPos, spawnRot);

        // Destroy after lifetime seconds
        Destroy(obj, lifetime);
    }
}
