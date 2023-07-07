using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerPlayer : NetworkBehaviour
{
    /// <summary>
    /// The character controller component.
    /// </summary>
    [Header("Components")]
    [SerializeField] private CharacterController controller;

    /// <summary>
    /// The player's camera.
    /// </summary>
    [SerializeField] private Camera playerCamera;

    /// <summary>
    /// The movement speed of the player.
    /// </summary>
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    /// <summary>
    /// The mouse sensitivity for camera movement.
    /// </summary>
    [Header("Camera")]
    [SerializeField] private float mouseSensitivity = 100f;

    /// <summary>
    /// The minimum and maximum pitch values for camera movement.
    /// </summary>
    [SerializeField] private Vector2 pitchMinMax = new Vector2(-45f, 45f);

    /// <summary>
    /// The direction that the player is moving in.
    /// </summary>
    private Vector2 _direction;
    
    private void Update()
    {
        if (!IsLocalPlayer) return;
        Move();
    }
    
    
    private void Move() 
    { 
        // Convert the 2D direction to a 3D vector with zero Y component and normalize it.
        Vector3 moveDirection = new Vector3(_direction.x, 0f, _direction.y).normalized; // Transform the move direction relative to the game object's rotation.
        moveDirection = transform.TransformDirection(moveDirection); // Calculate the move velocity by multiplying the move direction with the move speed.
        Vector3 moveVelocity = moveDirection * moveSpeed; // Move the game object using a character controller and delta time.
        controller.SimpleMove(moveVelocity);
        
        // Uncomment the following line to update the "Speed" parameter of an animator.
        // animator.SetFloat("Speed", moveVelocity.magnitude);
    }
   
    
    /// <summary>
    /// Callback function for the Move input action.
    /// </summary>
    /// <param name="context">The context of the input action callback.</param>
    public void Call_Move(InputAction.CallbackContext context)
    {
        if (NetworkManager.IsClient && !IsOwner)
        {
            Debug.LogWarning("Client is not the local player.");
            return;
        }
        
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