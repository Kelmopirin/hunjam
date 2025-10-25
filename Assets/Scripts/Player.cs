using UnityEngine;

public class Player
{
    private float speed;
    private float gravity;
    private float mouseSensitivity;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector3 velocity;

    private float xRotation = 0f;

    public Player(float speed, float gravity, float mouseSensitivity = 100f)
    {
        this.speed = speed;
        this.gravity = gravity;
        this.mouseSensitivity = mouseSensitivity;
        velocity = Vector3.zero;
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
}
