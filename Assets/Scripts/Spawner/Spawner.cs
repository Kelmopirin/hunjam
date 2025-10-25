using UnityEngine;
using System.Collections.Generic;

public class SpriteSpawner3D : MonoBehaviour
{
    [Header("Spawn Settings")]
    [Tooltip("Parent object containing rooms, each with spawnplace children.")]
    public Transform roomsParent;

    [Tooltip("List of 2D sprite prefabs to spawn (each prefab has a SpriteRenderer).")]
    public List<GameObject> spritePrefabs = new List<GameObject>();

    [Tooltip("Total number of items to spawn.")]
    public int itemsToSpawn = 5;

    [Tooltip("Optional random seed (-1 for fully random).")]
    public int randomSeed = -1;

    [Header("Display Settings")]
    [Tooltip("Offset along Z axis to prevent clipping with environment.")]
    public float zOffset = 0f;

    private List<Transform> spawnPoints = new List<Transform>();
    private Camera mainCam;

    void Start()
    {
        if (randomSeed != -1)
            Random.InitState(randomSeed);

        mainCam = Camera.main;
        GatherSpawnPoints();
        SpawnSprites();
    }

    void GatherSpawnPoints()
    {
        spawnPoints.Clear();

        if (roomsParent == null)
        {
            Debug.LogError("❌ Rooms parent not assigned!");
            return;
        }

        // Find all spawnplace children in all rooms
        foreach (Transform room in roomsParent)
        {
            foreach (Transform spawnplace in room)
            {
                spawnPoints.Add(spawnplace);
            }
        }

        if (spawnPoints.Count == 0)
            Debug.LogWarning("⚠️ No spawn points found under rooms parent!");
    }

    void SpawnSprites()
    {
        if (spawnPoints.Count == 0 || spritePrefabs.Count == 0)
        {
            Debug.LogWarning("⚠️ Missing spawn points or sprite prefabs!");
            return;
        }

        int spawnCount = Mathf.Min(itemsToSpawn, spawnPoints.Count);

        // Shuffle spawn points
        List<Transform> shuffledPoints = new List<Transform>(spawnPoints);
        for (int i = 0; i < shuffledPoints.Count; i++)
        {
            int rand = Random.Range(i, shuffledPoints.Count);
            (shuffledPoints[i], shuffledPoints[rand]) = (shuffledPoints[rand], shuffledPoints[i]);
        }

        // Spawn sprites
        for (int i = 0; i < spawnCount; i++)
        {
            Transform spawnPoint = shuffledPoints[i];
            GameObject prefab = spritePrefabs[Random.Range(0, spritePrefabs.Count)];

            GameObject spawned = Instantiate(prefab, spawnPoint.position + Vector3.forward * zOffset, Quaternion.identity, spawnPoint);

            // Make sure the sprite faces the camera (billboard)
            if (mainCam != null)
                FaceCamera(spawned.transform);
        }
    }

    void FaceCamera(Transform obj)
    {
        // Face the camera on Y axis only (stay upright)
        Vector3 camPos = mainCam.transform.position;
        Vector3 lookDir = obj.position - camPos;
        lookDir.y = 0; // keep upright
        obj.rotation = Quaternion.LookRotation(lookDir);
    }
}
