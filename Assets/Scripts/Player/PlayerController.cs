using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private Player player;
    private CharacterController characterController;
    private PlayerInput playerInput;

    [Header("Settings")]
    public float speed = 5f;
    public float gravity = -9.81f;
    public float mouseSensitivity = 100f;
    public float interactDistance = 3f;

    [Header("References")]
    public Transform playerCamera;
    public GameObject interactIcon;
    public RawImage[] inventorySlots;
    private GameObject currentTarget;


    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();

        player = new Player(speed, gravity, mouseSensitivity);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        playerInput.actions["Move"].performed += OnMove;
        playerInput.actions["Move"].canceled += OnMove;

        playerInput.actions["Look"].performed += OnLook;
        playerInput.actions["Look"].canceled += OnLook;

        playerInput.actions["Interact"].performed += OnInteract;
    }

    private void OnDisable()
    {
        playerInput.actions["Move"].performed -= OnMove;
        playerInput.actions["Move"].canceled -= OnMove;

        playerInput.actions["Look"].performed -= OnLook;
        playerInput.actions["Look"].canceled -= OnLook;

        playerInput.actions["Interact"].performed -= OnInteract;
    }

    private void UpdateInventoryUI()
    {
        var items = player.Items;

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (i < items.Count)
            {
                inventorySlots[i].texture = items[i].texture; // Convert Sprite → Texture
                inventorySlots[i].color = Color.white;
            }
            else
            {
                inventorySlots[i].texture = null;
                inventorySlots[i].color = new Color(1,1,1,0); // transparent
            }
        }
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (currentTarget == null) return;

        // Check if current target is cauldron
        if (currentTarget.CompareTag("Cauldron"))
        {
            // Only activate if player has at least 1 item
            if (player.InventoryCount > 0) // We'll add InventoryCount to Player
            {
                player.RemoveOneItem();
                UpdateInventoryUI();

                // Play particle effect
                Cauldron cauldron = currentTarget.GetComponent<Cauldron>();
                if (cauldron != null)
                    cauldron.Activate();
            }
            else
            {
                Debug.Log("No items to use!");
            }
        }
        else // Normal pickup
        {
            if (player.TryPickupItem(currentTarget))
            {
                UpdateInventoryUI();
            }
            else
            {
                Debug.Log("Inventory full!");
            }
        }
    }




    private void Update()
    {
        player.Move(transform, characterController);
        player.Look(transform, playerCamera);

        currentTarget = player.CheckForInteractable(playerCamera, interactDistance);

        if (interactIcon != null)
            interactIcon.SetActive(currentTarget != null);
    }


    private void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        player.SetMoveInput(input);
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        player.SetLookInput(input);
    }
}
