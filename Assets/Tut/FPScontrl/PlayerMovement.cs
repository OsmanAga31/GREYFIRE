using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] private float pushBackForce = 5f;
    private float x;
    private float z;
    bool doJump = false;


    [SerializeField] private CharacterController controller;

    [SerializeField] private float speed = 12f;
    [SerializeField] private float gravity = -9.80665f;
    [SerializeField] private float jumpHeight = 3f;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;

    private Vector3 velocity;
    private bool isGrounded = false;

    private void FixedUpdate()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    // Update is called once per frame
    void Update()
    {

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -7.5f;
        }

        var move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

        if (doJump && isGrounded)
        {
            velocity.y = Mathf.Sqrt(-2f * gravity * jumpHeight);
        }

        velocity.y += gravity * Time.deltaTime;
       
        controller.Move(velocity * Time.deltaTime);
    }

    public void OnMove(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        var movementInput = ctx.ReadValue<Vector2>();
        x = movementInput.x;
        z = movementInput.y;
    }

    public void OnJump(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            doJump = true;
        }
        else if (ctx.canceled)
        {
            doJump = false;
        }
    }

    // Pushes RigidBody objects away from the player when colliding with them
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;
        if (body == null || body.isKinematic)
        {
            return;
        }
        if (hit.moveDirection.y < -0.3f)
        {
            return;
        }
        Vector3 pushDirection = new Vector3(hit.moveDirection.x, hit.moveDirection.y, hit.moveDirection.z);
        body.AddForceAtPosition(pushDirection * pushBackForce, hit.point, ForceMode.Impulse);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }
}
