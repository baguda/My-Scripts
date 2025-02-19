using UnityEngine;



public class ObserverCameraController : MonoBehaviour
{
    public float moveSpeed = 10f; // Speed of movement
    public float lookSpeed = 2f; // Speed of camera rotation
    public float sprintMultiplier = 2f; // Speed multiplier when holding Shift

    private float yaw = 0f; // Horizontal rotation
    private float pitch = 0f; // Vertical rotation

    void Update()
    {
        // Handle camera rotation with mouse input
        HandleMouseLook();

        // Handle camera movement with WASD keys
        HandleKeyboardMovement();
    }

    void HandleMouseLook()
    {
        // Get mouse input
        yaw += lookSpeed * Input.GetAxis("Mouse X");
        pitch -= lookSpeed * Input.GetAxis("Mouse Y");

        // Clamp vertical rotation to avoid flipping
        pitch = Mathf.Clamp(pitch, -90f, 90f);

        // Apply rotation to the camera
        transform.eulerAngles = new Vector3(pitch, yaw, 0f);
    }

    void HandleKeyboardMovement()
    {
        // Calculate movement direction based on input
        Vector3 moveDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) // Move forward
            moveDirection += transform.forward;
        if (Input.GetKey(KeyCode.S)) // Move backward
            moveDirection -= transform.forward;
        if (Input.GetKey(KeyCode.A)) // Move left
            moveDirection -= transform.right;
        if (Input.GetKey(KeyCode.D)) // Move right
            moveDirection += transform.right;
        if (Input.GetKey(KeyCode.Space)) // Move up
            moveDirection += Vector3.up;
        if (Input.GetKey(KeyCode.LeftControl)) // Move down
            moveDirection -= Vector3.up;

        // Normalize the direction to prevent faster diagonal movement
        moveDirection.Normalize();

        // Apply sprint multiplier if Shift is held
        float currentSpeed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
            currentSpeed *= sprintMultiplier;

        // Move the camera
        transform.position += moveDirection * currentSpeed * Time.deltaTime;
    }
}