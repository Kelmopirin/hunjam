using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct HandHoldPair
{
    public Sprite itemSprite;
    public Texture2D handTexture;
}

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public Player player;
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
    public GameObject useIcon; // ✅ This is now controlled automatically

    [Header("Hotbar UI")]
    public Image[] slotHighlights;

    [Header("Hand UI")]
    public RawImage handImage;
    public Texture2D emptyHandTexture;
    public List<HandHoldPair> handPairs = new List<HandHoldPair>();

    [Header("Audio")]
    public AudioSource collapseAudio;
    public AudioSource regainAudio;

    [Header("References")]
    public Transform playerCamera;

    [Header("Hotbar / Selection")]
    public int selectedIndex = 0;
    public ItemProgressBar progressBar;

    private GameObject currentTarget;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();

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
        playerInput.actions["Use"].performed += OnUse;
    }

    private void OnDisable()
    {
        playerInput.actions["Move"].performed -= OnMove;
        playerInput.actions["Move"].canceled -= OnMove;

        playerInput.actions["Look"].performed -= OnLook;
        playerInput.actions["Look"].canceled -= OnLook;

        playerInput.actions["Interact"].performed -= OnInteract;
        playerInput.actions["Use"].performed -= OnUse;
    }

    private void Update()
    {
        if (player.IsCollapsed && rb == null)
            CollapsePlayer();

        player.Move(transform, characterController, Time.deltaTime);
        player.Look(transform, playerCamera);

        if (energySlider != null)
            energySlider.value = player.CurrentEnergy;

        currentTarget = player.CheckForInteractable(playerCamera, interactDistance);
        if (interactIcon != null)
            interactIcon.SetActive(currentTarget != null);

        // ✅ Keep Use Icon updated
        UpdateUseIcon();

        if (player.IsCollapsed && !isFading)
        {
            if (collapseAudio != null && !collapseAudio.isPlaying)
                collapseAudio.Play();
            StartCoroutine(FadeAndReload());
        }

        if (!player.IsCollapsed)
        {
            HandleHotbarScroll();
        }
    }

    private void OnMove(InputAction.CallbackContext context) => player.SetMoveInput(context.ReadValue<Vector2>());
    private void OnLook(InputAction.CallbackContext context) => player.SetLookInput(context.ReadValue<Vector2>());

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (currentTarget == null) return;

        // ✅ Bed interaction
        if (currentTarget.CompareTag("Bed"))
        {
            BedInteract bed = currentTarget.GetComponent<BedInteract>();
            if (bed != null)
                bed.TrySleep();
            return;
        }


        if (currentTarget.CompareTag("Cauldron"))
        {
            if (player.InventoryCount > 0 && selectedIndex < player.InventoryCount)
            {
                Sprite usedSprite = player.GetCurrentItem(selectedIndex);
                player.RemoveItemAt(selectedIndex);

                if (selectedIndex >= player.InventoryCount)
                    selectedIndex = Mathf.Max(player.InventoryCount - 1, 0);

                UpdateInventoryUI();

                if (progressBar != null && usedSprite != null)
                    progressBar.FillForItem(usedSprite.name);

                Cauldron cauldron = currentTarget.GetComponent<Cauldron>();
                if (cauldron != null)
                    cauldron.Activate();
            }
        }
        else
        {
            if (player.TryPickupItem(currentTarget))
            {
                selectedIndex = player.InventoryCount - 1;
                UpdateInventoryUI();
            }
        }

        UpdateUseIcon(); // ✅
    }

    private void OnUse(InputAction.CallbackContext context)
    {
        if (selectedIndex < 0 || selectedIndex >= player.InventoryCount) return;

        Sprite currentItem = player.GetCurrentItem(selectedIndex);
        if (currentItem == null) return;

        // ✅ Only usable item check
        if (currentItem.name.ToLower().Contains("hell"))
        {
            player.RestoreEnergy(150f);
            player.RemoveItemAt(selectedIndex);
            regainAudio.Play();

            if (selectedIndex >= player.InventoryCount)
                selectedIndex = Mathf.Max(player.InventoryCount - 1, 0);

            UpdateInventoryUI();
        }

        UpdateUseIcon(); // ✅
    }

    public void UpdateInventoryUI()
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
        UpdateHotbarHighlight();
        UpdateUseIcon(); // ✅
    }

    private void UpdateHotbarHighlight()
    {
        selectedIndex = Mathf.Clamp(selectedIndex, 0, inventorySlots.Length - 1);

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (i >= player.InventoryCount)
                inventorySlots[i].color = (i == selectedIndex) ? Color.white : new Color(1f, 1f, 1f, 0.0f);
            else
                inventorySlots[i].color = (i == selectedIndex) ? Color.white : new Color(1f, 1f, 1f, 0.1f);
        }
    }

    private void UpdateHandTexture()
    {
        if (selectedIndex >= player.InventoryCount)
        {
            handImage.texture = emptyHandTexture;
            return;
        }

        Sprite selectedItem = player.Items[selectedIndex];
        foreach (var pair in handPairs)
        {
            if (pair.itemSprite == selectedItem)
            {
                handImage.texture = pair.handTexture;
                return;
            }
        }

        handImage.texture = emptyHandTexture;
    }

    private void HandleHotbarScroll()
    {
        float scroll = Mouse.current.scroll.ReadValue().y;

        if (scroll > 0f)
            selectedIndex = (selectedIndex - 1 + inventorySlots.Length) % inventorySlots.Length;
        else if (scroll < 0f)
            selectedIndex = (selectedIndex + 1) % inventorySlots.Length;

        UpdateHandTexture();
        UpdateHotbarHighlight();
        UpdateUseIcon(); // ✅
    }

    // ✅ NEW FUNCTION — controls when useIcon shows
    private void UpdateUseIcon()
    {
        if (useIcon == null) return;

        if (selectedIndex < 0 || selectedIndex >= player.InventoryCount)
        {
            useIcon.SetActive(false);
            return;
        }

        Sprite currentItem = player.GetCurrentItem(selectedIndex);
        if (currentItem == null)
        {
            useIcon.SetActive(false);
            return;
        }

        bool isEnergyDrink = currentItem.name.ToLower().Contains("hell");
        useIcon.SetActive(isEnergyDrink);
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