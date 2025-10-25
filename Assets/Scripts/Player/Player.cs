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

    private List<Sprite> inventory = new List<Sprite>();
    public IReadOnlyList<Sprite> Items => inventory;
    private const int MaxInventorySize = 3;

    public int InventoryCount => inventory.Count;

    public Player(float speed, float gravity, float mouseSensitivity = 100f)
    {
        this.speed = speed;
        this.gravity = gravity;
        this.mouseSensitivity = mouseSensitivity;
        velocity = Vector3.zero;
    }

    public bool TryPickupItem(GameObject item)
    {
        if (inventory.Count >= MaxInventorySize)
            return false;

        SpriteRenderer sr = item.GetComponent<SpriteRenderer>();
        if (sr == null || sr.sprite == null)
            return false;

        // Store sprite
        inventory.Add(sr.sprite);

        // Hide item in world
        item.SetActive(false);

        return true;
    }

    public bool RemoveOneItem()
    {
        if (inventory.Count == 0)
            return false;

        // Remove last item (like Minecraft hotbar behavior)
        inventory.RemoveAt(inventory.Count - 1);
        return true;
    }

    public void SetMoveInput(Vector2 input)
    {
        moveInput = input;
    }

    public void SetLookInput(Vector2 input)
    {
        lookInput = input;
    }

    // Movement
    public void Move(Transform transform, CharacterController controller)
    {
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        move *= speed;

        velocity.y += gravity * Time.deltaTime;

        Vector3 displacement = (move + velocity) * Time.deltaTime;
        controller.Move(displacement);

        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;
    }

    // Camera / player rotation
    public void Look(Transform playerBody, Transform cameraTransform)
    {
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }

    public GameObject CheckForInteractable(Transform cameraTransform, float distance)
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distance))
        {
            if (hit.collider.CompareTag("Interactable") || hit.collider.CompareTag("Cauldron"))
            {
                return hit.collider.gameObject;
            }
        }

        return null;
    }

}
