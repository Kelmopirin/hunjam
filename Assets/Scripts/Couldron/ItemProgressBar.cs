using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ItemProgressBar : MonoBehaviour
{
    [Header("UI")]
    public Slider progressBar;
    public PlayerController playerController; // Reference to PlayerController (for currentIndex)


    [Header("Settings")]
    [Tooltip("How fast the bar fills (acts as a multiplier). Higher = faster.")]
    public float fillSpeed = 1f;

    // Item name -> fill value table
    private readonly Dictionary<string, float> itemFillValues = new Dictionary<string, float>()
    {
        { "alma", 2f },
        { "banan", 2f },
        { "cigi", 2f },
        { "csotany", 3f },
        { "hell", 5f },
        { "kobanyai", 10f },
        { "patkany", 10f },
        { "patkany2", 10f },
        { "smack", 15f },
        { "wckefe", 4f },
        
    };

    private float currentFill = 0f;
    private Coroutine fillRoutine;

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();

        if (progressBar != null)
        {
            progressBar.minValue = 0f;
            progressBar.maxValue = 100f;
            progressBar.value = 0f;
        }
    }

    /// <summary>
    /// Called externally to fill the progress bar based on an item's name.
    /// </summary>
    public void FillForItem(string itemName)
    {
        if (string.IsNullOrEmpty(itemName) || progressBar == null)
            return;

        Debug.Log(itemName);

        itemName = itemName.ToLower();
        if (itemName == "biblia")
        {
            playerController.player.DrainEnergy(500f);
            FindObjectOfType<MessageAlertSystem>().ShowMessage("Hülye maradsz!", Color.red);
            return;

        }

        // Check if the item exists in the dictionary
        if (itemFillValues.TryGetValue(itemName, out float baseValue))
        {
            // Calculate target value using fillSpeed as a multiplier
            float addedValue = baseValue * fillSpeed;
            float targetFill = Mathf.Clamp(currentFill + addedValue, 0f, 100f);

            if (fillRoutine != null)
                StopCoroutine(fillRoutine);

            fillRoutine = StartCoroutine(FillProgressBar(targetFill));

            

            Debug.Log($"Filling with {itemName}: +{addedValue} → {targetFill}%");
        }
        else
        {
            Debug.LogWarning($"No fill value found for item '{itemName}'");
        }
    }

    private IEnumerator FillProgressBar(float target)
    {
        float start = currentFill;
        float speed = Mathf.Max(0.1f, fillSpeed); // Ensure non-zero multiplier

        while (currentFill < target)
        {
            currentFill += Time.deltaTime * (20f * speed); // smooth animation speed
            currentFill = Mathf.Min(currentFill, target);
            progressBar.value = currentFill;
            yield return null;
        }

        currentFill = target;
        fillRoutine = null;

        if (currentFill >= progressBar.maxValue)
        {
            int sceneIndex = SceneManager.GetActiveScene().buildIndex;
            Debug.Log(sceneIndex);
            if (sceneIndex != 2 && sceneIndex != 5)
            {
                BedInteract bed = FindObjectOfType<BedInteract>();
                if (bed != null)
                {
                    bed.canNextLevel = true;
                    Debug.Log("Bar is full! You can now go to the next level.");
                    FindObjectOfType<MessageAlertSystem>().ShowMessage("Kész a kaja! Már aludhatsz!", Color.green);

                }
            }
            else if (sceneIndex == 5)
            {
                BedInteract bed = FindObjectOfType<BedInteract>();
                if (bed != null)
                {
                    GameObject tickGajdos = GameObject.Find("tickPenz");
                    if (tickGajdos != null)
                    {
                        if (tickGajdos.activeSelf)
                        {
                            bed.canNextLevel = true;
                            FindObjectOfType<MessageAlertSystem>().ShowMessage("Utolsó nap túlélve! Szabad vagy!", Color.green);
                        }
                    }
                    else
                    {
                        Debug.LogWarning("tickGajdos not found in the scene!");
                    }
                    Debug.Log("Bar is full! You can now go to the next level.");
                }
            }
            else
            {
                BedInteract bed = FindObjectOfType<BedInteract>();
                if (bed != null)
                {
                    GameObject tickGajdos = GameObject.Find("tickGajdos");
                    if (tickGajdos != null)
                    {
                        if (tickGajdos.activeSelf)
                        {
                            bed.canNextLevel = true;
                            FindObjectOfType<MessageAlertSystem>().ShowMessage("Nap túlélve! Mehetsz aludni!", Color.green);
                        }
                    }
                    else
                    {
                        Debug.LogWarning("tickGajdos not found in the scene!");
                    }
                    Debug.Log("Bar is full! You can now go to the next level.");
                }
            } 
        }
    }
}
