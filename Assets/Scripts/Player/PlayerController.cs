using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private Player player;
    private CharacterController characterController;
    private PlayerInput playerInput;
    private Rigidbody rb;

    public ItemProgressBar progressBar; // assign in inspector


    public Image fadeImage; // assign in Inspector
    public float fadeDuration = 2f;
    private bool isFading = false;

    [Header("Settings")]
    public float speed = 5f;
    public float gravity = -9.81f;
    public float mouseSensitivity = 100f;
    public float interactDistance = 3f;

    [Header("UI")]
    public Slider energySlider;          // Assign your energy bar
    public RawImage[] inventorySlots;    // Assign your hotbar UI
    public GameObject interactIcon;      // Assign interact icon

    private AudioSource collapseAudio;  // assign in Inspector

    [Header("References")]
    public Transform playerCamera;

    private GameObject currentTarget;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        collapseAudio = GetComponent<AudioSource>();

        player = new Player(speed, gravity, mouseSensitivity);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (energySlider != null)
        {
            energySlider.minValue = 0f;
            energySlider.maxValue = player.MaxEnergy;
            energySlider.value = player.CurrentEnergy;
        }

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

    private void Update()
    {
        // Player collapse check
        if (player.IsCollapsed && rb == null)
            CollapsePlayer();

        // Call Player movement/look
        player.Move(transform, characterController, Time.deltaTime);
        player.Look(transform, playerCamera);

        // Update energy slider
        if (energySlider != null)
            energySlider.value = player.CurrentEnergy;

        // Check interactables
        currentTarget = player.CheckForInteractable(playerCamera, interactDistance);
        if (interactIcon != null)
            interactIcon.SetActive(currentTarget != null);

        // Fade and reload if collapsed
        if (player.IsCollapsed && !isFading)
        {
            // Play collapse audio once
            if (collapseAudio != null && !collapseAudio.isPlaying)
                collapseAudio.Play();

            StartCoroutine(FadeAndReload());
        }
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

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (currentTarget == null) return;

        // Check if current target is cauldron
        if (currentTarget.CompareTag("Cauldron"))
        {
            if (player.InventoryCount > 0)
            {
                // string usedItem = player.GetCurrentItem();

            player.RemoveOneItem();
            UpdateInventoryUI();

            // CALL FUNCTION FROM ANOTHER SCRIPT
            if (progressBar != null)
                progressBar.FillForItem(usedItem);

            // Play cauldron effect
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
                UpdateInventoryUI();
            else
                Debug.Log("Inventory full!");
        }
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
                inventorySlots[i].color = new Color(1, 1, 1, 0); // transparent
            }
        }
    }

    private void CollapsePlayer()
    {
        if (rb != null) return; // already collapsed

        // Disable CharacterController
        characterController.enabled = false;

        // Enable collider for physics
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = true;

        // Add Rigidbody for physics
        rb = gameObject.AddComponent<Rigidbody>();
        rb.mass = 70f;

        // Optional: freeze rotations you don’t want
        rb.constraints = RigidbodyConstraints.None; // full ragdoll rotation

        // Optional: add small forward force
        rb.AddForce(transform.forward * 1f, ForceMode.VelocityChange);
    }

    private IEnumerator FadeAndReload()
    {
        isFading = true;

        float elapsed = 0f;
        Color c = fadeImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }

        // Ensure alpha = 1
        c.a = 1f;
        fadeImage.color = c;

        // Reload current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
