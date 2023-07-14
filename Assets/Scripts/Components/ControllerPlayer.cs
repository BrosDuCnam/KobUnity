using System;
using System.Collections;
using System.Collections.Generic;
using Interfaces;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ControllerPlayer : NetworkBehaviour
{
    [Header("Components")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private Camera playerCamera;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 7.5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 3f;

    [Header("Camera")]
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private Vector2 pitchMinMax = new Vector2(-45f, 45f);
    
    [Header("Events")]
    [SerializeField] private UnityEvent onTouchGround;
    
    public Vector3 velocity = Vector3.zero;
    public Vector3 parentVelocity = Vector3.zero;
    
    private Vector2 _direction;
    private Vector2 _lookDelta;
    private bool _isJumping;
    private bool _isSprinting;
    
    private bool _lastFrameGrounded;
    private IMovingObject _currentMovingParent;
    
    private void Update()
    {
        if (controller.isGrounded)
        {
            if (!_lastFrameGrounded)
            {
                onTouchGround.Invoke();
            }
        }
        _lastFrameGrounded = controller.isGrounded;

        if (!IsOwner) return;
        
        UpdatePosition();
        UpdateLook();
    }
    
    private void Start()
    {
        if (!IsOwner) return;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    private void UpdatePosition()
    {
        // Set the velocity to the direction, take into account the move speed, delta time, and the player's rotation.
        Vector3 move = transform.right * _direction.x + transform.forward * _direction.y;
        velocity.x = move.x;
        velocity.z = move.z;
        
        if (controller.isGrounded)
        {
            velocity.y = 0f;
        }

        velocity.y += gravity * Time.deltaTime;

        if (_isJumping)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            _isJumping = false;
        }

        // Move the player
        float speed = _isSprinting ? sprintSpeed : moveSpeed;
        controller.Move(velocity * (Time.deltaTime * speed) + parentVelocity);
    }

    private void UpdateLook()
    {
        // Calculate the change in mouse position
        float yaw = _lookDelta.x * mouseSensitivity * Time.deltaTime;
        float pitch = _lookDelta.y * mouseSensitivity * Time.deltaTime;

        // Rotate the player object around the up axis
        transform.Rotate(Vector3.up * yaw);

        // Rotate the camera object around the left axis
        playerCamera.transform.Rotate(Vector3.left * pitch);

        // Clamp the camera rotation around the x-axis to prevent it from flipping
        playerCamera.transform.localRotation = ClampRotationAroundXAxis(playerCamera.transform.localRotation);
    }


    public void OnParentMove(Vector3 delta)
    {
        if (!IsOwner) return;
        
        parentVelocity = delta;
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
            _lookDelta = context.ReadValue<Vector2>();
            
            if (context.control.device is Gamepad)
            {
                _lookDelta *= 7; // Gamepad is too slow compared to mouse
            }
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            _lookDelta = Vector2.zero;
        }
    }
    
    public void Call_Jump(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;
        if (!controller.isGrounded) return;
        
        if (context.phase == InputActionPhase.Performed)
        {
            _isJumping = true;
        }
    }
    
    public void Call_Sprint(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;

        switch (context.phase)
        {
            case InputActionPhase.Performed:
                _isSprinting = true;
                break;
            case InputActionPhase.Canceled:
                _isSprinting = false;
                break;
        }
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

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!IsOwner) return;
        
        // Check if hit is IMovingObject or has IMovingObject in parents hierarchy
        GameObject toTest = hit.gameObject;
        
        while (toTest != null)
        {
            IMovingObject movingObject = toTest.GetComponent<IMovingObject>();
            if (movingObject != null)
            { 
                SetMovingParent(movingObject);
                return;
            }

            if (toTest.transform.parent == null) break;
            toTest = toTest.transform.parent.gameObject;
        }
        
        SetMovingParent(null);
    }

    public void SetMovingParent(IMovingObject parent)
    {
        if (!IsOwner) return;
		if (parent == _currentMovingParent) return;
        
        _currentMovingParent?.OnMove.RemoveListener(OnParentMove);

        if (parent == null)
        {
            _currentMovingParent = null;
            parentVelocity = Vector3.zero;
            return;
        }
        
        _currentMovingParent = parent;
        _currentMovingParent.OnMove.AddListener(OnParentMove);
    }
    
    void OnGUI()
    {
        if (Application.isEditor)  // or check the app debug flag
        {
            GUIStyle style = new GUIStyle
            {
                fontSize = 34,
            };

            GUI.Label(new Rect(10, 10, 1000, 20), GetDebugString(), style);
        }
    }

    public string GetDebugString()
    {
        string value = "";
        value += $"IsOwner: {IsOwner}\n";
        value += $"Velocity: {velocity.ToString("F8")}\n";
        value += $"Is grounded: {controller.isGrounded}\n";
        
        value += $"Is on platform: {(_currentMovingParent != null)}\n";
        if (_currentMovingParent != null)
        {
            value += $"Platform velocity: {parentVelocity.ToString("F8")}\n";
        }
        
        return value;
    }
}