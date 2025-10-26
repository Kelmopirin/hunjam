using System.Diagnostics;
using UnityEngine;
using System.Diagnostics;
using UnityEngine.InputSystem; // New Input System
using UnityEngine.UI; // <-- Add this to use Slider
using TMPro;
using System.Diagnostics.Contracts; // <-- For TextMeshPro
public class RepontMoney : MonoBehaviour
{
    [Header("References")]

    public GameObject playerObj;                    // Reference to Player script
    public PlayerController playerController; // Reference to PlayerController (for currentIndex)
    public Transform targetObject;            // Object to check proximity against
    public GameObject useIcon; // ✅ This is now controlled automatically

    public TMP_Text moneyText;                     // ✅ Added text reference

    [Header("Settings")]
    public float detectionRange = 3f;         // How close player must be
    private PlayerInput playerInput;

    public int money = 0;

    private AudioSource cashSound;

    void Start()
    {
        // Auto-assign references if missing

        playerInput = playerController.GetComponent<PlayerInput>();

        if (playerController == null)
            playerController = FindObjectOfType<PlayerController>();
    
        cashSound = GetComponent<AudioSource>();

    }

    void Update()
    {
                    moneyText.text = "Pénz: " + money;

        

        // Check distance first
        float distance = Vector3.Distance(playerObj.transform.position, targetObject.position);
        if (distance <= detectionRange)
        {
            var player = playerController.player;

            Sprite currentItem = player.GetCurrentItem(playerController.selectedIndex);

            if (currentItem != null && (currentItem.name == "kobanyai"))
            {

                useIcon.SetActive(true);
                // Then check for right-click
                if (playerInput.actions["Use"].triggered)
                {
                    UnityEngine.Debug.Log("leadva");
                    money += 50;
                    player.RemoveItemAt(playerController.selectedIndex);
                    playerController.UpdateInventoryUI();
                    cashSound.Play();

                }
            }
            else if (currentItem != null && (currentItem.name == "patkany" || currentItem.name == "csotany" || currentItem.name == "wckefe" || currentItem.name == "banan" ||currentItem.name == "patkany2"))
            {
                useIcon.SetActive(true);

                if (playerInput.actions["Use"].triggered)
                {
                    UnityEngine.Debug.Log("leadva");
                    money += 25;
                    player.RemoveItemAt(playerController.selectedIndex);
                    playerController.UpdateInventoryUI();
                    cashSound.Play();
                }
            }
            
            else
            {
                useIcon.SetActive(false);
                if (playerInput.actions["Use"].triggered)
                {
                    FindObjectOfType<MessageAlertSystem>().ShowMessage("Nincs kezedben leadható tárgy!", Color.red);

                }

            }

        }
        else
        {
            useIcon.SetActive(false);
            
        }
        
    }
}
