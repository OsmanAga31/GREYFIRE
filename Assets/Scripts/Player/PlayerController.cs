using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(Collider)), RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    
    private PlayerInput playerInput;
    private bool isPlayerMovementLocked;
    private Rigidbody rb;
    private Vector2 movementInput;
    private Transform cameraTransform;
    [SerializeField] private float actualMovementSpeed;
    [SerializeField] private float jumpStrength;
    private bool isGrounded;
    
    [Header("Player Rotation")]
    [SerializeField] private  Transform cinemachineCameraTarget;
    
    public bool IsPlayerMovementLocked
    {
        get { return isPlayerMovementLocked; }
        set { isPlayerMovementLocked = value; }
    }
    
    private void Awake()
    {
        // Implement singleton pattern to persist this object across scenes
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Initializes input actions, camera reference, and Rigidbody.
    /// </summary>
    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        isPlayerMovementLocked = false;
        
        playerInput.actions["Move"].performed += OnMove;
        playerInput.actions["Move"].canceled += OnMove;
        
        playerInput.actions["Jump"].performed += OnJump;
        playerInput.actions["Jump"].canceled += OnJump;
        
        cameraTransform = Camera.main.transform;
        
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Handles movement input from the player.
    /// </summary>
    /// <param name="ctx">Input callback context containing movement vector.</param>
    private void OnMove(InputAction.CallbackContext ctx)
    {
        movementInput = ctx.ReadValue<Vector2>();
    }
    
    /// <summary>
    /// Handles jump input from the player. Only allows jumping if grounded.
    /// </summary>
    /// <param name="ctx">Input callback context for jump action.</param>
    private void OnJump(InputAction.CallbackContext ctx)
    {
        if (isPlayerMovementLocked) return;
        if (ctx.performed && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpStrength, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    /// <summary>
    /// Handles physics-based movement and checks if the player is grounded.
    /// </summary>
    private void FixedUpdate()
    {
        if (isPlayerMovementLocked) return;
        
        Vector3 moveDirection = new Vector3(movementInput.x, 0, movementInput.y);
        moveDirection = cameraTransform.TransformDirection(moveDirection);
        moveDirection.y = 0; // Prevent vertical movement from camera tilt
        rb.MovePosition(rb.position + moveDirection * (actualMovementSpeed * Time.fixedDeltaTime));
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        // Check if the player has collided with the ground to allow jumping again
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    /// <summary>
    /// Rotates the player to face the direction of the camera's Y (yaw) axis.
    /// Called in LateUpdate to ensure camera has updated.
    /// </summary>
    private void PlayerRotation()
    {
        if (cinemachineCameraTarget == null) return;

        Vector3 targetEuler = cinemachineCameraTarget.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(0f, targetEuler.y, 0f);
    }

    /// <summary>
    /// Ensures player rotation is updated after camera movement.
    /// </summary>
    private void LateUpdate()
    {
        PlayerRotation();
    }

}
