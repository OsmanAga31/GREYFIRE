using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class MouseLook : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private float mouseX;
    [SerializeField] private float mouseY;

    [SerializeField] private Transform playerBody;

    private float xRotation = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }

    public void OnMouseLook(CallbackContext ctx)
    {
        var mouseInput = ctx.ReadValue<Vector2>();
        mouseX = mouseInput.x * mouseSensitivity * Time.deltaTime;
        mouseY = mouseInput.y * mouseSensitivity * Time.deltaTime;
    }

}
