using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct HandHoldPair
{
    public Sprite itemSprite;      // Sprite from item pickup
    public Texture2D handTexture;  // Hand texture showing item held
}

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private Player player;
    private CharacterController characterController;
    private PlayerInput playerInput;
    private Rigidbody rb;

    [Header("Fade")]
    public Image fadeImage;
    public float fadeDuration = 2f;
    private bool isFading = false;

    [Header("Settings")]
    public float speed = 5f;
    public float gravity = -9.81f;
    public float mouseSensitivity = 100f;
    public float interactDistance = 3f;

    [Header("UI")]
    public Slider energySlider;
    public RawImage[] inventorySlots;
    public GameObject interactIcon;

    [Header("Hotbar UI")]
    public Image[] slotHighlights; // Assign one highlight per slot in Inspector


    [Header("Hand UI")]
    public RawImage handImage;
    public Texture2D emptyHandTexture;
    public List<HandHoldPair> handPairs = new List<HandHoldPair>();

    [Header("Audio")]
    public AudioSource collapseAudio;

    [Header("References")]
    public Transform playerCamera;

    [Header("Hotbar / Selection")]
    private int selectedIndex = 0; // Selected inventory index
    public ItemProgressBar progressBar; // drag from scene or assign dynamically


    private GameObject currentTarget;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();

        if (collapseAudio == null)
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
        // Collapse check
        if (player.IsCollapsed && rb == null)
            CollapsePlayer();

        // Movement + Look
        player.Move(transform, characterController, Time.deltaTime);
        player.Look(transform, playerCamera);

        // Update Energy UI
        if (energySlider != null)
            energySlider.value = player.CurrentEnergy;

        // Interact Check
        currentTarget = player.CheckForInteractable(playerCamera, interactDistance);
        if (interactIcon != null)
            interactIcon.SetActive(currentTarget != null);

        // Collapse effect start
        if (player.IsCollapsed && !isFading)
        {
            if (collapseAudio != null && !collapseAudio.isPlaying)
                collapseAudio.Play();
            StartCoroutine(FadeAndReload());
        }

        // Hotbar scroll (only if player alive)
        if (!player.IsCollapsed)
        {
            HandleHotbarScroll();
        }
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        player.SetMoveInput(context.ReadValue<Vector2>());
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        player.SetLookInput(context.ReadValue<Vector2>());
    }

private void OnInteract(InputAction.CallbackContext context)
{
    if (currentTarget == null) return;

    if (currentTarget.CompareTag("Cauldron"))
    {
        if (player.InventoryCount > 0 && selectedIndex < player.InventoryCount)
        {
            // Store item name before removing
            Sprite usedSprite = player.GetCurrentItem(selectedIndex);
            // Remove selected item
            player.RemoveItemAt(selectedIndex);

            // Adjust selected index if now out of range
            if (selectedIndex >= player.InventoryCount)
                selectedIndex = Mathf.Max(player.InventoryCount - 1, 0);

            // Update UI
            UpdateInventoryUI();
            UpdateHandTexture();
            UpdateHotbarHighlight();

            // ✅ Call function in another script
            if (progressBar != null && usedSprite != null)
                progressBar.FillForItem(usedSprite.name);

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
    else // Pickup
    {
        if (player.TryPickupItem(currentTarget))
        {
            selectedIndex = player.InventoryCount - 1;

            UpdateInventoryUI();
            UpdateHandTexture();
            UpdateHotbarHighlight();

            // ✅ Optionally call on pickup
            string pickedItem = currentTarget.name;
            if (progressBar != null)
                progressBar.FillForItem(pickedItem);
        }
        else
        {
            Debug.Log("Inventory full!");
        }
    }
}



    private void UpdateInventoryUI()
    {
        var items = player.Items;

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (i < items.Count)
            {
                inventorySlots[i].texture = items[i].texture;
                inventorySlots[i].color = Color.white;
            }
            else
            {
                inventorySlots[i].texture = null;
                inventorySlots[i].color = new Color(1, 1, 1, 0);
            }
        }

        UpdateHandTexture();
        UpdateHotbarHighlight(); // ✅ Highlight update whenever inventory changes
    }


    private void UpdateHotbarHighlight()
    {
        // Clamp selectedIndex to the number of slots
        selectedIndex = Mathf.Clamp(selectedIndex, 0, inventorySlots.Length - 1);

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (i >= player.InventoryCount)
            {
                // Empty slot
                inventorySlots[i].color = (i == selectedIndex) ? Color.white : new Color(1f, 1f, 1f, 0.0f);
            }
            else
            {
                // Inventory slot
                inventorySlots[i].color = (i == selectedIndex) ? Color.white : new Color(1f, 1f, 1f, 0.1f);
            }
        }
    }


    private void UpdateHandTexture()
    {
        if (selectedIndex >= player.InventoryCount)
        {
            // Empty hand
            handImage.texture = emptyHandTexture;
            return;
        }

        // Show held item
        Sprite selectedItem = player.Items[selectedIndex];
        foreach (var pair in handPairs)
        {
            if (pair.itemSprite == selectedItem)
            {
                handImage.texture = pair.handTexture;
                return;
            }
        }

        handImage.texture = emptyHandTexture; // fallback
    }


    private void HandleHotbarScroll()
    {
        float scroll = Mouse.current.scroll.ReadValue().y;

        if (scroll > 0f) // Scroll up
            selectedIndex = (selectedIndex - 1 + inventorySlots.Length) % inventorySlots.Length;
        else if (scroll < 0f) // Scroll down
            selectedIndex = (selectedIndex + 1) % inventorySlots.Length;

        UpdateHandTexture();
        UpdateHotbarHighlight();
    }



    private void CollapsePlayer()
    {
        characterController.enabled = false;

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = true;

        rb = gameObject.AddComponent<Rigidbody>();
        rb.mass = 70f;
        rb.constraints = RigidbodyConstraints.None;
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

        c.a = 1f;
        fadeImage.color = c;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}