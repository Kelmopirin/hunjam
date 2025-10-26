using UnityEngine;

public class CigiShop : MonoBehaviour
{
    [Header("References")]
    public RepontMoney repontAutomata;  // Reference to the RepontAutomata script
    public GameObject tick;              // Tick mark to show success
    public Transform spawnPoint;         // Where the object should spawn
    public GameObject itemPrefab;        // What object to spawn (e.g. cigi prefab)
    public ItemProgressBar itemProgressScript;

    [Header("Settings")]
    public int itemCost = 150;           // How much it costs

    void Start()
    {
        if (tick != null)
            tick.SetActive(false);
    }

    public void checkMoney()
    {
        if (repontAutomata == null)
        {
            Debug.LogWarning("RepontAutomata reference not set!");
            return;
        }

        if (repontAutomata.money >= itemCost)
        {
            Debug.Log("Success!");
            tick?.SetActive(true);

            // Deduct the money
            repontAutomata.money -= itemCost;

            // ✅ Spawn the item on the desk
            if (spawnPoint != null && itemPrefab != null)
            {
                Instantiate(itemPrefab, spawnPoint.position, spawnPoint.rotation);
                Debug.Log($"Spawned {itemPrefab.name} at {spawnPoint.name}");
                itemProgressScript.FillForItem("hell");

            }
            else
            {
                Debug.LogWarning("Missing spawnPoint or itemPrefab reference!");
            }
        }
        else
        {
            FindObjectOfType<MessageAlertSystem>()?.ShowMessage(
                "Nincs elég pénzed cigire! (150)", Color.red);
        }
    }
}
