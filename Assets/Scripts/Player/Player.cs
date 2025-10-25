using UnityEngine;
using System.Collections.Generic;

public class Player
{
    private float speed;
    private float gravity;
    private float mouseSensitivity;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector3 velocity;

    private float xRotation = 0f;

    // Inventory
    private List<Sprite> inventory = new List<Sprite>();
    public IReadOnlyList<Sprite> Items => inventory;
    private const int MaxInventorySize = 3;
    public int InventoryCount => inventory.Count;

    // Energy
    private float maxEnergy = 500f;
    private float currentEnergy;
    private float moveDrainRate = 10f; // per second
    private bool isCollapsed = false;
    public bool IsCollapsed => isCollapsed;
    public Vector2 MoveInput => moveInput;
    // Add these properties if not already
    public float CurrentEnergy => currentEnergy;
    public float MaxEnergy => maxEnergy;


    public Player(float speed, float gravity, float mouseSensitivity = 100f)
    {
        this.speed = speed;
        this.gravity = gravity;
        this.mouseSensitivity = mouseSensitivity;
        velocity = Vector3.zero;

        currentEnergy = maxEnergy;
    }

    // Inventory Methods
    public bool TryPickupItem(GameObject item)
    {
        if (inventory.Count >= MaxInventorySize)
            return false;

        SpriteRenderer sr = item.GetComponent<SpriteRenderer>();
        if (sr == null || sr.sprite == null)
            return false;

        inventory.Add(sr.sprite);
        item.SetActive(false);
        return true;
    }

    public bool RemoveOneItem()
    {
        if (inventory.Count == 0)
            return false;
        inventory.RemoveAt(inventory.Count - 1);
        return true;
    }

    // Removes the item at a specific index
    public bool RemoveItemAt(int index)
    {
        if (index >= 0 && index < inventory.Count)
        {
            inventory.RemoveAt(index);
            return true;
        }
        return false;
    }


    // Inputs
    public void SetMoveInput(Vector2 input) => moveInput = input;
    public void SetLookInput(Vector2 input) => lookInput = input;

    // Movement
    public void Move(Transform transform, CharacterController controller, float deltaTime)
    {
        if (isCollapsed) return;

        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        move *= speed;

        // Drain energy if moving
        if (moveInput.magnitude > 0.1f)
            DrainEnergy(moveDrainRate * deltaTime);

        velocity.y += gravity * deltaTime;
        Vector3 displacement = (move + velocity) * deltaTime;
        controller.Move(displacement);

        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;
    }

    // Looking
    public void Look(Transform playerBody, Transform cameraTransform)
    {
        if (isCollapsed) return;

        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }

    // Energy Methods
    private void DrainEnergy(float amount)
    {
        currentEnergy -= amount;
        if (currentEnergy <= 0f)
        {
            currentEnergy = 0f;
            Collapse();
        }
    }

    public void RestoreEnergy(float amount)
    {
        currentEnergy = Mathf.Clamp(currentEnergy + amount, 0f, maxEnergy);
    }


    private void Collapse() => isCollapsed = true;


    public GameObject CheckForInteractable(Transform cameraTransform, float distance)
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distance))
        {
            if (hit.collider.CompareTag("Interactable") ||
                hit.collider.CompareTag("Cauldron") ||
                hit.collider.CompareTag("Bed"))
            {
                return hit.collider.gameObject;
            }
        }
        return null;
    }


    // Returns the Sprite of the currently selected item, or null if empty hand
    public Sprite GetCurrentItem(int selectedIndex)
    {
        if (selectedIndex < 0 || selectedIndex >= inventory.Count)
            return null; // Empty hand

        return inventory[selectedIndex];
    }

}