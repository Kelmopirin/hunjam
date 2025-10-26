using UnityEngine;

public class SkySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject objectToSpawn;
    public float spawnInterval = 1.5f;

    [Header("Spawn Area Size")]
    public float width = 10f;
    public float depth = 10f;
    public float height = 20f; // height above ground where objects appear

    private void Start()
    {
        InvokeRepeating(nameof(SpawnObject), 0f, spawnInterval);
    }

    void SpawnObject()
    {
        // Pick random point inside area
        float x = Random.Range(-width / 2, width / 2);
        float z = Random.Range(-depth / 2, depth / 2);

        Vector3 spawnPos = transform.position + new Vector3(x, height, z);

        Instantiate(objectToSpawn, spawnPos, Quaternion.identity);
    }

    private void OnDrawGizmosSelected()
    {
        // Draw the spawn area in editor for clarity
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + Vector3.up * (height / 2), new Vector3(width, height, depth));
    }
}
