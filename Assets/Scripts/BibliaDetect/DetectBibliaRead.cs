using System.Diagnostics;
using UnityEngine;
using System.Diagnostics;
using UnityEngine.InputSystem; // New Input System
using UnityEngine.UI; // <-- Add this to use Slider

public class DetectBibliaRead : MonoBehaviour
{
    [Header("References")]

    public GameObject playerObj;                    // Reference to Player script
    public PlayerController playerController; // Reference to PlayerController (for currentIndex)
    public Transform targetObject;            // Object to check proximity against
    public GameObject useIcon; // ✅ This is now controlled automatically

    [Header("Settings")]
    public float detectionRange = 3f;         // How close player must be
    private PlayerInput playerInput;

    public ItemProgressBar itemProgressScript;

    public GameObject tick;
    
    void Start()
    {
        // Auto-assign references if missing

        playerInput = playerController.GetComponent<PlayerInput>();

        if (playerController == null)
            playerController = FindObjectOfType<PlayerController>();
    

    }

    void Update()
    {
        

        // Check distance first
        float distance = Vector3.Distance(playerObj.transform.position, targetObject.position);
        if (distance <= detectionRange)
        {
            var player = playerController.player;

            Sprite currentItem = player.GetCurrentItem(playerController.selectedIndex);

            if (currentItem != null && currentItem.name == "biblia")
            {

                useIcon.SetActive(true);
                // Then check for right-click
                if (playerInput.actions["Use"].triggered)
                {
                    UnityEngine.Debug.Log("success");
                    tick.SetActive(true);
                    playerController.player.DrainEnergy(100f);
                    player.RemoveItemAt(playerController.selectedIndex);
                    playerController.UpdateInventoryUI();
                    itemProgressScript.FillForItem("hell");

                }
            }
            else
        {
                useIcon.SetActive(false);
            if (playerInput.actions["Use"].triggered)
                {
                    UnityEngine.Debug.Log("success");
                    tick.SetActive(true);
                    playerController.player.DrainEnergy(150f);
                    player.RemoveItemAt(playerController.selectedIndex);
                    playerController.UpdateInventoryUI();
                    itemProgressScript.FillForItem("hell");

                }

            
        }

        }
        else
        {
            useIcon.SetActive(false);
            
        }
        
    }
}
