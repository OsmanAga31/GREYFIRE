using System;
using System.Numerics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(Collider)), RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    
    private PlayerInput playerInput;
    private bool isPlayerMovementLocked;
    private Rigidbody rb;
    private Vector2 movementInput;
    private Transform cameraTransform;
    
    [Header("Movement Settings")]
    [SerializeField] private float actualMovementSpeed;
    private float originalMovementSpeed;
    [SerializeField] private float sneakSpeedMultiplier = 0.5f; // Multiplier for movement speed when sneaking
    [SerializeField] private float jumpStrength;
    
    [Header("Ground Check Settings")]
    [SerializeField] private float groundCheckHeightOffset = 0.05f; // Additional height offset for ground check to prevent false negatives
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private float groundCheckDistance = 0.15f;
    [SerializeField] private LayerMask layerToIgnore;
    private bool isGrounded;
    
    
    [Header("Player Rotation")]
    [SerializeField] private  Transform cinemachineCameraTarget;

    private CapsuleCollider capsuleCollider;

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
        capsuleCollider = transform.gameObject.GetComponent<CapsuleCollider>();
        originalMovementSpeed = actualMovementSpeed;
        
        playerInput = GetComponent<PlayerInput>();
        isPlayerMovementLocked = false;
        
        playerInput.actions["Move"].performed += OnMove;
        playerInput.actions["Move"].canceled += OnMove;
        
        playerInput.actions["Jump"].performed += OnJump;
        playerInput.actions["Jump"].canceled += OnJump;
        
        playerInput.actions["Sneak"].performed += OnSneak;
        playerInput.actions["Sneak"].canceled += OnSneak;
        
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
    /// Handles sneak input from the player. Reduces movement speed when sneaking and resets it when not sneaking.
    /// </summary>
    /// <param name="ctx"></param>
    private void OnSneak(InputAction.CallbackContext ctx)
    {
        if (isPlayerMovementLocked) return;
        if (ctx.performed)
        {
            actualMovementSpeed = originalMovementSpeed * sneakSpeedMultiplier; // Reduce speed by 50% when sneaking
        }
        else if (ctx.canceled)
        {
            actualMovementSpeed = originalMovementSpeed; // Reset to original speed when not sneaking
        }
    }

    /// <summary>
    /// Handles physics-based movement and checks if the player is grounded.
    /// </summary>
    private void FixedUpdate()
    {
        if (isPlayerMovementLocked) return;

        RaycastHit hit;
        Vector3 rayOrigin = new Vector3(
            transform.position.x,
            transform.position.y - (capsuleCollider.height / 2) + groundCheckRadius + groundCheckHeightOffset,
            transform.position.z
        );
        bool didHit = Physics.SphereCast(rayOrigin, groundCheckRadius, Vector3.down, out hit, groundCheckDistance, ~layerToIgnore, QueryTriggerInteraction.Ignore); // ~ means to ignore the specified layer
        isGrounded = didHit;
        Color rayColor = didHit ? Color.green : Color.red;
        Debug.DrawRay(rayOrigin, Vector3.down * groundCheckDistance, rayColor, 0.1f);
        
        // Draw a sphere at the ground check position for visualization
        Debug.DrawLine(rayOrigin + Vector3.left * groundCheckRadius, rayOrigin + Vector3.right * groundCheckRadius, rayColor, 0.1f);
        Debug.DrawLine(rayOrigin + Vector3.forward * groundCheckRadius, rayOrigin + Vector3.back * groundCheckRadius, rayColor, 0.1f);
        
        Vector3 moveDirection = new Vector3(movementInput.x, 0, movementInput.y);
        moveDirection = cameraTransform.TransformDirection(moveDirection);
        moveDirection.y = 0; // Prevent vertical movement from camera tilt
        rb.MovePosition(rb.position + moveDirection * (actualMovementSpeed * Time.fixedDeltaTime));
    }

    /// <summary>
    /// Rotates the player to face the direction of the camera's Y (yaw) axis.
    /// Called in LateUpdate to ensure camera has updated.
    /// </summary>
    private void PlayerRotation()
    {
        if (!cinemachineCameraTarget) return;

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
    
    private void OnDestroy()
    {
        UnscribeControlls();
    }

    private void OnDisable()
    {
        UnscribeControlls();
    }

    private void UnscribeControlls()
    {
        // Unsubscribe from input events to prevent memory leaks
        if (playerInput != null)
        {
            playerInput.actions["Move"].performed -= OnMove;
            playerInput.actions["Move"].canceled -= OnMove;
            playerInput.actions["Jump"].performed -= OnJump;
            playerInput.actions["Jump"].canceled -= OnJump;
        }
    }

}
