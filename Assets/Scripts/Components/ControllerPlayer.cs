using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerPlayer : NetworkBehaviour
{
    [Header("Components")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private Camera playerCamera;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 3f;
    

    [Header("Camera")]
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private Vector2 pitchMinMax = new Vector2(-45f, 45f);

    public Vector3 velocity = Vector3.zero;
    
    private Vector2 _direction;
    private Vector2 _lookDelta;
    private bool _isJumping;
    
    private void Update()
    {
        if (!IsLocalPlayer) return;
        Move();
    }
    
    private void Start()
    {
        if (!IsLocalPlayer) return;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    private void Move()
    {
        if (controller.isGrounded)
        {
            // Set the velocity to the direction, take into account the move speed, delta time, and the player's rotation.
            Vector3 move = transform.right * _direction.x + transform.forward * _direction.y;
            velocity.x = move.x;
            velocity.z = move.z;
            
            velocity.y = 0f;
        }

        velocity.y += gravity * Time.deltaTime;

        if (_isJumping)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            _isJumping = false;
        }

        // Move the player
        controller.Move(velocity * (moveSpeed * Time.deltaTime));
    }


    /// <summary>
    /// Callback function for the Move input action.
    /// </summary>
    /// <param name="context">The context of the input action callback.</param>
    public void Call_Move(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;

        if (context.phase == InputActionPhase.Performed)
        {
            _direction = context.ReadValue<Vector2>();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            _direction = Vector2.zero;
        }
    }
    
    /// <summary>
    /// Handles the "Look" input action callback to rotate the player's view.
    /// </summary>
    /// <param name="context">The input action callback context.</param>
    public void Call_Look(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;
        if (context.phase == InputActionPhase.Performed)
        {
            Vector2 lookDirection = context.ReadValue<Vector2>();
            float yaw = lookDirection.x * mouseSensitivity * Time.deltaTime;
            float pitch = lookDirection.y * mouseSensitivity * Time.deltaTime;
            transform.Rotate(Vector3.up * yaw);
            playerCamera.transform.Rotate(Vector3.left * pitch);
            playerCamera.transform.localRotation = ClampRotationAroundXAxis(playerCamera.transform.localRotation);
        }
    }
    
    public void Call_Jump(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;
        if (!controller.isGrounded) return;
        
        if (context.phase == InputActionPhase.Performed)
        {
            Debug.Log("Jumping");
            _isJumping = true;
        }
    }
    
    /// <summary>
    /// Gets the direction that the player is looking in.
    /// </summary>
    /// <returns>The direction that the player is looking in.</returns>
    /// <remarks>
    /// This function returns the forward direction of the player's camera.
    /// </remarks>
    public Vector3 GetLookDirection()
    {
        return playerCamera.transform.forward;
    }    

    /// <summary>
    /// Clamps the rotation around the x-axis of a quaternion.
    /// </summary>
    /// <param name="q">The quaternion to clamp.</param>
    /// <returns>The clamped quaternion.</returns>
    private Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1f;

        float angleX = 2f * Mathf.Rad2Deg * Mathf.Atan(q.x);

        angleX = Mathf.Clamp(angleX, pitchMinMax.x, pitchMinMax.y);

        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }
}